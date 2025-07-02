using Dapper;
using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.ModuloContato;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.DapperOrm;

public abstract class RepositorioBase<T> where T : EntidadeBase<T>
{
    protected readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaDb;Integrated Security=True";

    protected abstract string SqlInserir { get; }

    protected abstract string SqlEditar { get; }

    protected abstract string SqlExcluir { get; }

    protected abstract string SqlSelecionarPorId { get; }

    protected abstract string SqlSelecionarTodos { get; }

    protected virtual object ObterParametros(T objeto)
    {
        return objeto;
    }

    public virtual void CadastrarRegistro(T novoRegistro)
    {           
        using var conexao = new SqlConnection(connectionString);

        conexao.Execute(SqlInserir, ObterParametros(novoRegistro));
    }

    public virtual bool EditarRegistro(Guid idRegistro, T registroEditado)
    {            
        using var conexao = new SqlConnection(connectionString);

        registroEditado.Id = idRegistro;

        var linhasAfetadas = conexao.Execute(SqlEditar, ObterParametros(registroEditado));

        return linhasAfetadas > 0;
    }

    public virtual bool ExcluirRegistro(Guid idRegistro)
    {           
        using var conexao = new SqlConnection(connectionString);

        var linhasAfetadas = conexao.Execute(SqlExcluir, new { Id = idRegistro });

        return linhasAfetadas > 0;
    }

    public virtual T? SelecionarRegistroPorId(Guid idRegistro)
    {          
        using var conexao = new SqlConnection(connectionString);

        return conexao.QueryFirstOrDefault<T>(SqlSelecionarPorId, new { Id = idRegistro });
    }

    public virtual List<T> SelecionarRegistros()
    {           
        using var conexao = new SqlConnection(connectionString);

        return conexao.Query<T>(SqlSelecionarTodos).ToList();
    }

}
