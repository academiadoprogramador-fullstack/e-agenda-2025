using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrutura.SqlServer.Compartilhado;
using Microsoft.Data.SqlClient;
using System.Data;

namespace eAgenda.Infraestrutura.SqlServer.ModuloTarefa;

public class RepositorioTarefaEmSql : RepositorioBaseEmSql<Tarefa>, IRepositorioTarefa
{
    public RepositorioTarefaEmSql(IDbConnection conexaoComBanco) : base(conexaoComBanco)
    {
    }

    protected override string SqlInserir => @"
        INSERT INTO [TBTAREFA] 
        (
            [ID],
            [TITULO],
            [DATACRIACAO],
            [DATACONCLUSAO],
            [PRIORIDADE],
            [CONCLUIDA]
        )
        VALUES
        (
            @ID,
            @TITULO,
            @DATACRIACAO,
            @DATACONCLUSAO,
            @PRIORIDADE,
            @CONCLUIDA
        );";

    protected override string SqlEditar => @"
        UPDATE [TBTAREFA]	
        SET
            [TITULO] = @TITULO,
            [DATACRIACAO] = @DATACRIACAO,
            [DATACONCLUSAO] = @DATACONCLUSAO,
            [PRIORIDADE] = @PRIORIDADE,
            [CONCLUIDA] = @CONCLUIDA
        WHERE
            [ID] = @ID";

    protected override string SqlExcluir => @"
        DELETE FROM [TBTAREFA]
        WHERE [ID] = @ID";

    protected override string SqlSelecionarPorId => @"
        SELECT
            [ID],
            [TITULO],
            [PRIORIDADE],
            [DATACRIACAO],
            [DATACONCLUSAO],
            [CONCLUIDA]
        FROM 
            [TBTAREFA]
        WHERE 
            [ID] = @ID";

    protected override string SqlSelecionarTodos => @"
        SELECT
            [ID],
            [TITULO],
            [PRIORIDADE],
            [DATACRIACAO],
            [DATACONCLUSAO],
            [CONCLUIDA]
        FROM 
            [TBTAREFA]";
 
    public void AdicionarItem(ItemTarefa item)
    {
        const string sqlAdicionarItemTarefa = @"
            INSERT INTO [TBITEMTAREFA]
            (
                [ID],
                [TITULO],
                [CONCLUIDO],
                [TAREFA_ID]
            )
            VALUES
            (
                @ID,
                @TITULO,
                @CONCLUIDO,
                @TAREFA_ID
            );";

        var comando = conexaoComBanco.CreateCommand();
        comando.CommandText = sqlAdicionarItemTarefa;
        ConfigurarParametrosItemTarefa(item, comando);

        conexaoComBanco.Open();
        comando.ExecuteNonQuery();
        conexaoComBanco.Close();
    }

    public bool AtualizarItem(ItemTarefa itemAtualizado)
    {
        const string sqlEditar = @"
            UPDATE [TBITEMTAREFA]	
            SET
                [TITULO] = @TITULO,
                [CONCLUIDO] = @CONCLUIDO,
                [TAREFA_ID] = @TAREFA_ID
            WHERE
                [ID] = @ID";

        var comando = conexaoComBanco.CreateCommand();
        comando.CommandText = sqlEditar;

        ConfigurarParametrosItemTarefa(itemAtualizado, comando);

        conexaoComBanco.Open();

        var alteracoesRealizadas = comando.ExecuteNonQuery();
        
        conexaoComBanco.Close();

        return alteracoesRealizadas > 0;
    }

    public bool RemoverItem(ItemTarefa item)
    {
        const string sqlExcluir = @"
            DELETE FROM [TBITEMTAREFA]
            WHERE [ID] = @ID";

        var comando = conexaoComBanco.CreateCommand();
        comando.CommandText = sqlExcluir;

        comando.AdicionarParametro("ID", item.Id);

        conexaoComBanco.Open();

        var numeroRegistrosExcluidos = comando.ExecuteNonQuery();
        
        conexaoComBanco.Close();

        return numeroRegistrosExcluidos > 0;
    }

    public override bool ExcluirRegistro(Guid idTarefa)
    {
        ExcluirItensTarefa(idTarefa);
        return base.ExcluirRegistro(idTarefa);
    }
    
    public override List<Tarefa> SelecionarRegistros()
    {
        var registros = base.SelecionarRegistros();

        foreach (var registro in registros)
            CarregarItensTarefa(registro);

        return registros;
    }

    public List<Tarefa> SelecionarTarefasPendentes()
    {
        const string sqlSelecionarTarefasPendentes = @"
            SELECT
                [ID],
                [TITULO],
                [PRIORIDADE],
                [DATACRIACAO],
                [DATACONCLUSAO],
                [CONCLUIDA]
            FROM 
                [TBTAREFA]
            WHERE
                [CONCLUIDA] = 0";

        var comando = conexaoComBanco.CreateCommand();
        comando.CommandText = sqlSelecionarTarefasPendentes;

        conexaoComBanco.Open();

        var leitorTarefa = comando.ExecuteReader();

        var tarefasPendentes = new List<Tarefa>();

        while (leitorTarefa.Read())
        {
            var tarefa = ConverterParaRegistro(leitorTarefa);
            tarefasPendentes.Add(tarefa);
        }

        conexaoComBanco.Close();

        return tarefasPendentes;
    }

    public List<Tarefa> SelecionarTarefasConcluidas()
    {
        const string sqlSelecionarTarefasConcluidas = @"
            SELECT
                [ID],
                [TITULO],
                [PRIORIDADE],
                [DATACRIACAO],
                [DATACONCLUSAO],
                [CONCLUIDA]
            FROM 
                [TBTAREFA]
            WHERE
                [CONCLUIDA] = 1";

        var comando = conexaoComBanco.CreateCommand();
        comando.CommandText = sqlSelecionarTarefasConcluidas;

        conexaoComBanco.Open();

        var leitorTarefa = comando.ExecuteReader();

        var tarefasConcluidas = new List<Tarefa>();

        while (leitorTarefa.Read())
        {
            var tarefa = ConverterParaRegistro(leitorTarefa);
            tarefasConcluidas.Add(tarefa);
        }

        conexaoComBanco.Close();

        return tarefasConcluidas;
    }

    public override Tarefa? SelecionarRegistroPorId(Guid idRegistro)
    {
        var registro = base.SelecionarRegistroPorId(idRegistro);

        if (registro is not null)
            CarregarItensTarefa(registro);

        return registro;
    }

    protected override void ConfigurarParametrosRegistro(Tarefa tarefa, IDbCommand comando)
    {
        comando.AdicionarParametro("ID", tarefa.Id);
        comando.AdicionarParametro("TITULO", tarefa.Titulo);
        comando.AdicionarParametro("PRIORIDADE", tarefa.Prioridade);
        comando.AdicionarParametro("DATACRIACAO", tarefa.DataCriacao);
        comando.AdicionarParametro("DATACONCLUSAO", tarefa.DataConclusao ?? (object)DBNull.Value);
        comando.AdicionarParametro("CONCLUIDA", tarefa.Concluida);
    }
    
    protected override Tarefa ConverterParaRegistro(IDataReader leitor)
    {
        DateTime? dataConclusao = null;

        if (!leitor["DATACONCLUSAO"].Equals(DBNull.Value))
            dataConclusao = Convert.ToDateTime(leitor["DATACONCLUSAO"]);

        var tarefa = new Tarefa
        {
            Id = Guid.Parse(leitor["ID"].ToString()!),
            Titulo = Convert.ToString(leitor["TITULO"])!,
            DataCriacao = Convert.ToDateTime(leitor["DATACRIACAO"]),
            DataConclusao = dataConclusao,
            Prioridade = (PrioridadeTarefa)leitor["PRIORIDADE"],
            Concluida = Convert.ToBoolean(leitor["CONCLUIDA"])
        };

        return tarefa;
    }

    private void ConfigurarParametrosItemTarefa(ItemTarefa item, IDbCommand comando)
    {
        comando.AdicionarParametro("ID", item.Id);
        comando.AdicionarParametro("TITULO", item.Titulo);
        comando.AdicionarParametro("CONCLUIDO", item.Concluido);
        comando.AdicionarParametro("TAREFA_ID", item.Tarefa.Id);
    }

    private ItemTarefa ConverterParaItemTarefa(IDataReader leitorItemTarefa, Tarefa tarefa)
    {
        var itemTarefa = new ItemTarefa
        {
            Id = Guid.Parse(leitorItemTarefa["ID"].ToString()!),
            Titulo = Convert.ToString(leitorItemTarefa["TITULO"])!,
            Concluido = Convert.ToBoolean(leitorItemTarefa["CONCLUIDO"]),
            Tarefa = tarefa
        };

        return itemTarefa;
    }

    private void CarregarItensTarefa(Tarefa tarefa)
    {
        const string sqlSelecionarItensTarefa = @"
            SELECT 
                [ID],
                [TITULO],
                [CONCLUIDO],
                [TAREFA_ID]
            FROM 
                [TBITEMTAREFA]
            WHERE 
                [TAREFA_ID] = @TAREFA_ID";

        var comando = conexaoComBanco.CreateCommand();
        comando.CommandText = sqlSelecionarItensTarefa;
        comando.AdicionarParametro("TAREFA_ID", tarefa.Id);

        conexaoComBanco.Open();

        var leitorItemTarefa = comando.ExecuteReader();

        while (leitorItemTarefa.Read())
        {
            var itemTarefa = ConverterParaItemTarefa(leitorItemTarefa, tarefa);
            tarefa.AdicionarItem(itemTarefa);
        }

        conexaoComBanco.Close();
    }

    private void ExcluirItensTarefa(Guid idTarefa)
    {
        const string sqlExcluirItensTarefa = @"
            DELETE FROM [TBITEMTAREFA]
            WHERE [TAREFA_ID] = @TAREFA_ID";

        var comando = conexaoComBanco.CreateCommand();
        comando.CommandText = sqlExcluirItensTarefa;

        comando.AdicionarParametro("TAREFA_ID", idTarefa);

        conexaoComBanco.Open();

        comando.ExecuteNonQuery();

        conexaoComBanco.Close();
    }
}
