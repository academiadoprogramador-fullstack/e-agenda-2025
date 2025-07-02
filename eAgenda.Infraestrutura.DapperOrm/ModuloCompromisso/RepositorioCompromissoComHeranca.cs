using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using Dapper;
using Microsoft.Data.SqlClient;
using eAgenda.Infraestrutura.DapperOrm;

namespace eAgenda.Infraestrutura.DapperOrm.ModuloCompromisso;

// Registered on startup

public class RepositorioCompromissoComHeranca : RepositorioBase<Compromisso>
{
    protected override string SqlInserir =>
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

    protected override string SqlEditar =>
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

    protected override string SqlExcluir =>
        @"DELETE FROM [TBCOMPROMISSO]
            WHERE
                [ID] = @Id";


    protected override string SqlSelecionarPorId =>
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

    protected override string SqlSelecionarTodos =>
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

    public override Compromisso? SelecionarRegistroPorId(Guid idRegistro)
    {
        SqlMapper.AddTypeHandler(new TimeSpanConverter());       

        using var conexaoComBanco = new SqlConnection(connectionString);

        var parametros = new { Id = idRegistro };

        return conexaoComBanco
            .Query<Compromisso, Contato, Compromisso>(SqlSelecionarPorId, (cp, ct) =>
            {
                cp.Contato = ct;
                return cp;
            },
            splitOn: "ID",
            param: parametros
            ).FirstOrDefault();
    }

    public override List<Compromisso> SelecionarRegistros()
    {
        

        using var conexaoComBanco = new SqlConnection(connectionString);

        return conexaoComBanco
            .Query<Compromisso, Contato, Compromisso>(SqlSelecionarPorId, (cp, ct) =>
            {
                cp.Contato = ct;
                return cp;
            },
            splitOn: "ID"            
            ).ToList();
    }

    protected override object ObterParametros(Compromisso registroEditado)
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
