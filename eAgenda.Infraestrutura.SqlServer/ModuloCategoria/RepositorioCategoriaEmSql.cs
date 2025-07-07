using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrutura.SqlServer.Compartilhado;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.SqlServer.ModuloCategoria;

public class RepositorioCategoriaEmSql : RepositorioBaseEmSql<Categoria>, IRepositorioCategoria
{
    protected override string SqlInserir => @"
        INSERT INTO [TBCATEGORIA]
        (
            [ID],
            [TITULO]
        )
        VALUES
        (
            @ID,
            @TITULO
        );";

    protected override string SqlEditar => @"
        UPDATE [TBCATEGORIA]	
		SET
			[TITULO] = @TITULO
		WHERE
			[ID] = @ID";

    protected override string SqlExcluir => @"
        DELETE FROM [TBCATEGORIA]
		WHERE
			[ID] = @ID";

    protected override string SqlSelecionarPorId => @"
        SELECT 
		    [ID], 
		    [TITULO]
	    FROM 
		    [TBCATEGORIA]
        WHERE
            [ID] = @ID";

    protected override string SqlSelecionarTodos => @"
        SELECT 
		    [ID], 
		    [TITULO]
	    FROM 
		    [TBCATEGORIA]";

    protected string SqlSelecionarDespesasDaCategoria => 
        @"SELECT
            D.[ID],
            D.[DESCRICAO],
            D.[VALOR],
            D.[DATAOCORRENCIA],
            D.[FORMAPAGAMENTO]
        FROM
            [TBDESPESA] AS D INNER JOIN
            [TBDESPESA_TBCATEGORIA] AS DC
        ON
            D.[ID] = DC.[DESPESA_ID]
        WHERE
            DC.[CATEGORIA_ID] = @CATEGORIA_ID";

    public override Categoria? SelecionarRegistroPorId(Guid idRegistro)
    {
        var registro = base.SelecionarRegistroPorId(idRegistro);

        if (registro is not null)
            CarregarDespesas(registro);

        return registro;
    }

    public override List<Categoria> SelecionarRegistros()
    {
        return base.SelecionarRegistros();

        var registros = base.SelecionarRegistros();

        foreach (var registro in registros)
            CarregarDespesas(registro);

        return registros;
    }

    private Despesa ConverterParaDespesa(SqlDataReader leitor)
    {
        var registro = new Despesa
        {
            Id = Guid.Parse(leitor["ID"].ToString()!),
            Descricao = Convert.ToString(leitor["DESCRICAO"])!,
            Valor = Convert.ToDecimal(leitor["VALOR"])!,
            DataOcorencia = Convert.ToDateTime(leitor["DATAOCORRENCIA"])!,
            FormaPagamento = (FormaPagamento)leitor["FORMAPAGAMENTO"]!,
        };

        return registro;
    }

    private void CarregarDespesas(Categoria categoria)
    {
        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoSelecao = new SqlCommand(SqlSelecionarDespesasDaCategoria, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("CATEGORIA_ID", categoria.Id);

        conexaoComBanco.Open();

        SqlDataReader leitorCategoria = comandoSelecao.ExecuteReader();

        while (leitorCategoria.Read())
        {
            var despesa = ConverterParaDespesa(leitorCategoria);

            despesa.RegistarCategoria(categoria);
        }

        conexaoComBanco.Close();
    }

    protected override void ConfigurarParametrosRegistro(Categoria entidade, SqlCommand comando)
    {
        comando.Parameters.AddWithValue("ID", entidade.Id);
        comando.Parameters.AddWithValue("TITULO", entidade.Titulo);
    }

    protected override Categoria ConverterParaRegistro(SqlDataReader leitor)
    {
        var registro = new Categoria
        {
            Id = Guid.Parse(leitor["ID"].ToString()!),
            Titulo = Convert.ToString(leitor["TITULO"])!
        };

        return registro;
    }
}
