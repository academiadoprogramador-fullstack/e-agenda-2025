using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrura.Compartilhado;

namespace eAgenda.Infraestrutura.ModuloDespesa;

public class RepositorioDespesa : RepositorioBaseEmArquivo<Despesa>, IRepositorioDespesa
{
    public RepositorioDespesa(ContextoDados contexto) : base(contexto) { }

    protected override List<Despesa> ObterRegistros()
    {
        return contexto.Despesas;
    }
}
