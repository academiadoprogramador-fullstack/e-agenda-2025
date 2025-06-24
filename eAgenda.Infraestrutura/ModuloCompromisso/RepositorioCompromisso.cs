using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Infraestrura.Compartilhado;

namespace eAgenda.Infraestrutura.ModuloCompromisso;

public class RepositorioCompromisso : RepositorioBaseEmArquivo<Compromisso>, IRepositorioCompromisso
{
    public RepositorioCompromisso(ContextoDados contexto) : base(contexto) { }

    protected override List<Compromisso> ObterRegistros()
    {
        return contexto.Compromissos;
    }
}
