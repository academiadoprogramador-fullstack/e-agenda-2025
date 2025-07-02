using Dapper;
using eAgenda.Dominio.ModuloContato;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.DapperOrm.ModuloContato;

public class RepositorioContatoComDapper : IRepositorioContato
{
    private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaDb;Integrated Security=True";

    public void CadastrarRegistro(Contato novoRegistro)
    {
        var sqlInserir =
            @"INSERT INTO [TBCONTATO]
                (
                    [ID],
                    [NOME],
                    [EMAIL],
                    [TELEFONE],
                    [EMPRESA],
                    [CARGO]
                )
                VALUES
                (
                    @Id,
                    @Nome,
                    @Email,
                    @Telefone,
                    @Empresa,
                    @Cargo
                );";

        using var conexao = new SqlConnection(connectionString);

        conexao.Execute(sqlInserir,novoRegistro);
    }

    public bool EditarRegistro(Guid idRegistro, Contato registroEditado)
    {
        var sqlEditar =
            @"UPDATE [TBCONTATO]	
                SET
                    [NOME] = @Nome,
                    [EMAIL] = @Email,
                    [TELEFONE] = @Telefone,
                    [EMPRESA] = @Empresa,
                    [CARGO] = @Cargo
                WHERE
                    [ID] = @Id";

        using var conexao = new SqlConnection(connectionString);

        registroEditado.Id = idRegistro;

        var linhasAfetadas = conexao.Execute(sqlEditar, registroEditado);

        return linhasAfetadas > 0;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        var sqlExcluir =
            @"DELETE FROM [TBCONTATO]
                WHERE
                    [ID] = @Id";

        using var conexao = new SqlConnection(connectionString);

        var linhasAfetadas = conexao.Execute(sqlExcluir, new { Id = idRegistro });

        return linhasAfetadas > 0;
    }

    public Contato? SelecionarRegistroPorId(Guid idRegistro)
    {
        var sqlSelecionarPorId =
            @"SELECT 
                    [ID], 
                    [NOME], 
                    [EMAIL],
                    [TELEFONE],
                    [EMPRESA],
                    [CARGO]
                FROM 
                    [TBCONTATO]
                WHERE
                    [ID] = @Id";

        using var conexao = new SqlConnection(connectionString);

        return conexao.QueryFirstOrDefault<Contato>(sqlSelecionarPorId, new { Id = idRegistro });
    }

    public List<Contato> SelecionarRegistros()
    {
        var sqlSelecionarTodos =
            @"SELECT 
                    [ID], 
                    [NOME], 
                    [EMAIL],
                    [TELEFONE],
                    [EMPRESA],
                    [CARGO]
                FROM 
                    [TBCONTATO]";

        using var conexao = new SqlConnection(connectionString);

        return conexao.Query<Contato>(sqlSelecionarTodos).ToList();
    }

}