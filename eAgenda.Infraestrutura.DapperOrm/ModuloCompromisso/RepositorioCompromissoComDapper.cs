using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace eAgenda.Infraestrutura.DapperOrm.ModuloCompromisso;



public class RepositorioCompromissoComDapper : IRepositorioCompromisso
{
    private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaDb;Integrated Security=True";

    public void CadastrarRegistro(Compromisso novoRegistro)
    {
        var sqlInserir =
            @"INSERT INTO [TBCOMPROMISSO]
            (
                [ID],
                [ASSUNTO],
                [DATA], 
                [HORAINICIO],                    
                [HORATERMINO],
                [TIPO],  
                [LOCAL],       
                [LINK],            
                [CONTATO_ID]
            )
            VALUES
            (
                @Id,
                @Assunto,
                @Data,
                @HoraInicioTicks, 
                @HoraTerminoTicks,
                @Tipo,
                @Local,
                @Link,
                @ContatoId
            );";


        using var conexaoComBanco = new SqlConnection(connectionString);

        var parametros = ObterParametros(novoRegistro);

        conexaoComBanco.Execute(sqlInserir, novoRegistro);
    }

    public bool EditarRegistro(Guid idRegistro, Compromisso registroEditado)
    {
        var sqlEditar =
            @"UPDATE [TBCOMPROMISSO]
            SET 
                [ASSUNTO] = @Assunto,
                [DATA] = @Data, 
                [HORAINICIO] = @HoraInicioTicks, 
                [HORATERMINO] = @HoraTerminoTicks,
                [TIPO] = @Tipo,
                [LOCAL] = @Local, 
                [LINK] = @Link,
                [CONTATO_ID] = @ContatoId
            WHERE 
                [ID] = @Id";

        using var conexaoComBanco = new SqlConnection(connectionString);

        registroEditado.Id = idRegistro;

        var parametros = ObterParametros(registroEditado);

        var numeroRegistrosAfetados = conexaoComBanco.Execute(sqlEditar, parametros);

        return numeroRegistrosAfetados > 0;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        var sqlExcluir =
            @"DELETE FROM [TBCOMPROMISSO]
            WHERE
                [ID] = @Id";

        using var conexaoComBanco = new SqlConnection(connectionString);

        var parametros = new { Id = idRegistro };

        var numeroRegistrosExcluidos = conexaoComBanco.Execute(sqlExcluir, parametros);

        return numeroRegistrosExcluidos > 0;
    }

    public Compromisso? SelecionarRegistroPorId(Guid idRegistro)
    {
        SqlMapper.AddTypeHandler(new TimeSpanConverter());

        var sql =
            @"SELECT
                CP.[ID],
                CP.[ASSUNTO],
                CP.[DATA],
                CP.[HORAINICIO],
                CP.[HORATERMINO],
                CP.[TIPO],
                CP.[LOCAL],
                CP.[LINK],
                CP.[CONTATO_ID],
                CT.[ID],
                CT.[NOME],
                CT.[EMAIL],
                CT.[TELEFONE],
                CT.[CARGO],
                CT.[EMPRESA]
            FROM
                [TBCOMPROMISSO] AS CP LEFT JOIN
                [TBCONTATO] AS CT
            ON
                CT.ID = CP.CONTATO_ID
            WHERE
                CP.[ID] = @Id;";

        using var conexaoComBanco = new SqlConnection(connectionString);

        var parametros = new { Id = idRegistro };

        return conexaoComBanco
            .Query<Compromisso, Contato, Compromisso>(sql, (cp, ct) =>
                {
                    cp.Contato = ct;
                    return cp;
                },
                splitOn: "CONTATO_ID",
                param: parametros
            ).FirstOrDefault();
    }

    public List<Compromisso> SelecionarRegistros()
    {
        SqlMapper.AddTypeHandler(new TimeSpanConverter());

        var sql =
           @"SELECT
                CP.[ID],
                CP.[ASSUNTO],
                CP.[DATA],               
                CP.[TIPO],
                CP.[LOCAL],
                CP.[HORAINICIO],
                CP.[HORATERMINO],
                CP.[LINK],
                CP.[CONTATO_ID],
                CT.[ID],
                CT.[NOME],
                CT.[EMAIL],
                CT.[TELEFONE],
                CT.[CARGO],
                CT.[EMPRESA]
            FROM
                [TBCOMPROMISSO] AS CP LEFT JOIN
                [TBCONTATO] AS CT
            ON
                CT.ID = CP.CONTATO_ID;";

        using var conexaoComBanco = new SqlConnection(connectionString);

        return conexaoComBanco
            .Query<Compromisso, Contato, Compromisso>(sql, (cp, ct) =>
                {
                    cp.Contato = ct;
                    return cp;
                },
                splitOn: "ID"
            ).ToList();
    }

    private object ObterParametros(Compromisso registroEditado)
    {
        return new
        {
            registroEditado.Id,
            registroEditado.Assunto,
            registroEditado.Data,
            HoraInicioTicks = registroEditado.HoraInicio.Ticks,
            HoraTerminoTicks = registroEditado.HoraTermino.Ticks,
            Tipo = (int)registroEditado.Tipo,
            Local = registroEditado.Local ?? null,
            Link = registroEditado.Link ?? null,
            ContatoId = registroEditado.Contato?.Id ?? null
        };
    }
}