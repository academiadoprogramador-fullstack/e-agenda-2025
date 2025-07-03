using eAgenda.Dominio.ModuloCategoria;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.SqlServer.ModuloCategoria;

public class RepositorioCategoriaEmSql : IRepositorioCategoria
{
    private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaTest;Integrated Security=True";

    public void CadastrarRegistro(Categoria novoRegistro)
    {
        var sqlInserir =
            @"INSERT INTO [TBCATEGORIA]
            (
                [ID],
                [TITULO]
            )
            VALUES
            (
                @ID,
                @TITULO
            );";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

        ConfigurarParametrosCategoria(novoRegistro, comandoInsercao);

        conexaoComBanco.Open();

        comandoInsercao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool EditarRegistro(Guid idRegistro, Categoria registroEditado)
    {
        var sqlEditar =
            @"UPDATE [TBCATEGORIA]	
		    SET
			    [TITULO] = @TITULO
		    WHERE
			    [ID] = @ID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

        registroEditado.Id = idRegistro;

        ConfigurarParametrosCategoria(registroEditado, comandoEdicao);

        conexaoComBanco.Open();

        var linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas > 0;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        var sqlExcluir =
            @"DELETE FROM [TBCATEGORIA]
		    WHERE
			    [ID] = @ID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

        comandoExclusao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        var linhasAfetadas = comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas > 0;
    }

    public Categoria? SelecionarRegistroPorId(Guid idRegistro)
    {
        var sqlSelecionarPorId =
            @"SELECT 
		        [ID], 
		        [TITULO]
	        FROM 
		        [TBCATEGORIA]
            WHERE
                [ID] = @ID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoSelecao =
            new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        Categoria? registro = null;

        if (leitor.Read())
            registro = ConverterParaCategoria(leitor);

        return registro;
    }

    public List<Categoria> SelecionarRegistros()
    {
        var sqlSelecionarTodos =
            @"SELECT 
		        [ID], 
		        [TITULO]
	        FROM 
		        [TBCATEGORIA]";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        var registros = new List<Categoria>();

        while (leitor.Read())
        {
            var contato = ConverterParaCategoria(leitor);

            registros.Add(contato);
        }

        conexaoComBanco.Close();

        return registros;
    }

    private Categoria ConverterParaCategoria(SqlDataReader leitor)
    {
        var registro = new Categoria(
            Convert.ToString(leitor["TITULO"])!
        );

        registro.Id = Guid.Parse(leitor["ID"].ToString()!);

        return registro;
    }

    private void ConfigurarParametrosCategoria(Categoria entidade, SqlCommand comando)
    {
        comando.Parameters.AddWithValue("ID", entidade.Id);
        comando.Parameters.AddWithValue("TITULO", entidade.Titulo);
    }
}
