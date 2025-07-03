﻿using eAgenda.Dominio.ModuloDespesa;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.SqlServer.ModuloDespesa;

public class RepositorioDespesaEmSql : IRepositorioDespesa
{
    private readonly string connectionString =
      "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaDb;Integrated Security=True";

    public void CadastrarRegistro(Despesa novoRegistro)
    {
        var sqlInserir =
            @"INSERT INTO [TBDESPESA]
            (
                [ID],
                [DESCRICAO],
                [VALOR],
                [DATAOCORRENCIA],
                [FORMAPAGAMENTO]
            )
            VALUES
            (
                @ID,
                @DESCRICAO,
                @VALOR,
                @DATAOCORRENCIA,
                @FORMAPAGAMENTO
            );";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

        ConfigurarParametrosDespesa(novoRegistro, comandoInsercao);

        conexaoComBanco.Open();

        comandoInsercao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool EditarRegistro(Guid idRegistro, Despesa registroEditado)
    {
        var sqlEditar =
            @"UPDATE [TBDESPESA]	
		    SET
			    [DESCRICAO] = @DESCRICAO,
			    [VALOR] = @VALOR,
			    [DATAOCORRENCIA] = @DATAOCORRENCIA,
			    [FORMAPAGAMENTO] = @FORMAPAGAMENTO,
		    WHERE
			    [ID] = @ID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

        registroEditado.Id = idRegistro;

        ConfigurarParametrosDespesa(registroEditado, comandoEdicao);

        conexaoComBanco.Open();

        var linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas > 0;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        var sqlExcluir =
            @"DELETE FROM [TBDESPESA]
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

    public Despesa? SelecionarRegistroPorId(Guid idRegistro)
    {
        var sqlSelecionarPorId =
            @"SELECT 
		        [ID], 
		        [DESCRICAO],
		        [VALOR],
		        [DATAOCORRENCIA],
		        [FORMAPAGAMENTO]
	        FROM 
		        [TBDESPESA]
            WHERE
                [ID] = @ID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoSelecao =
            new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        Despesa? registro = null;

        if (leitor.Read())
            registro = ConverterParaDespesa(leitor);

        return registro;
    }

    public List<Despesa> SelecionarRegistros()
    {
        var sqlSelecionarTodos =
            @"SELECT 
		        [ID], 
		        [DESCRICAO],
		        [VALOR],
		        [DATAOCORRENCIA],
		        [FORMAPAGAMENTO]
	        FROM 
		        [TBDESPESA]";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        var registros = new List<Despesa>();

        while (leitor.Read())
        {
            var contato = ConverterParaDespesa(leitor);

            registros.Add(contato);
        }

        conexaoComBanco.Close();

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

    private void ConfigurarParametrosDespesa(Despesa entidade, SqlCommand comando)
    {
        comando.Parameters.AddWithValue("ID", entidade.Id);
        comando.Parameters.AddWithValue("DESCRICAO", entidade.Descricao);
        comando.Parameters.AddWithValue("VALOR", entidade.Valor);
        comando.Parameters.AddWithValue("DATAOCORRENCIA", entidade.DataOcorencia);
        comando.Parameters.AddWithValue("FORMAPAGAMENTO", (int)entidade.FormaPagamento);
    }
}
