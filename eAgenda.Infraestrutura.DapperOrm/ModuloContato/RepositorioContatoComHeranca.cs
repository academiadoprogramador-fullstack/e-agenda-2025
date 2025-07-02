using eAgenda.Dominio.ModuloContato;

namespace eAgenda.Infraestrutura.DapperOrm.ModuloContato;

public class RepositorioContatoComHeranca : RepositorioBase<Contato>
{
    protected override string SqlInserir =>
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

    protected override string SqlEditar =>
        @"UPDATE [TBCONTATO]	
            SET
                [NOME] = @Nome,
                [EMAIL] = @Email,
                [TELEFONE] = @Telefone,
                [EMPRESA] = @Empresa,
                [CARGO] = @Cargo
            WHERE
                [ID] = @Id";

    protected override string SqlExcluir =>
        @"DELETE FROM [TBCONTATO]
            WHERE
                [ID] = @Id";

    protected override string SqlSelecionarPorId =>
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

    protected override string SqlSelecionarTodos =>
        @"SELECT 
            [ID], 
            [NOME], 
            [EMAIL],
            [TELEFONE],
            [EMPRESA],
            [CARGO]
        FROM 
            [TBCONTATO]";
}
