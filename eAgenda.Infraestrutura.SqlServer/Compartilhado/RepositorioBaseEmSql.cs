using eAgenda.Dominio.Compartilhado;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.SqlServer.Compartilhado;

public abstract class RepositorioBaseEmSql<T> where T : EntidadeBase<T>
{
    protected readonly string connectionString =
     "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaDb;Integrated Security=True";

    protected abstract string SqlInserir { get; }
    protected abstract string SqlEditar { get; }
    protected abstract string SqlExcluir { get; }
    protected abstract string SqlSelecionarPorId { get; }
    protected abstract string SqlSelecionarTodos { get; }

    public void CadastrarRegistro(T novoRegistro)
    {
        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoInsercao = new SqlCommand(SqlInserir, conexaoComBanco);

        ConfigurarParametrosRegistro(novoRegistro, comandoInsercao);

        conexaoComBanco.Open();

        comandoInsercao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool EditarRegistro(Guid idRegistro, T registroEditado)
    {
        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoEdicao = new SqlCommand(SqlEditar, conexaoComBanco);

        registroEditado.Id = idRegistro;

        ConfigurarParametrosRegistro(registroEditado, comandoEdicao);

        conexaoComBanco.Open();

        var linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas > 0;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoExclusao = new SqlCommand(SqlExcluir, conexaoComBanco);

        comandoExclusao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        var linhasAfetadas = comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas > 0;
    }

    public virtual T? SelecionarRegistroPorId(Guid idRegistro)
    {
        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoSelecao =
            new SqlCommand(SqlSelecionarPorId, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        T? registro = null;

        if (leitor.Read())
            registro = ConverterParaRegistro(leitor);

        return registro;
    }

    public virtual List<T> SelecionarRegistros()
    {
        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new SqlCommand(SqlSelecionarTodos, conexaoComBanco);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        var registros = new List<T>();

        while (leitor.Read())
        {
            var contato = ConverterParaRegistro(leitor);

            registros.Add(contato);
        }

        conexaoComBanco.Close();

        return registros;
    }

    protected abstract void ConfigurarParametrosRegistro(T entidade, SqlCommand comando);
    
    protected abstract T ConverterParaRegistro(SqlDataReader leitor);
}
