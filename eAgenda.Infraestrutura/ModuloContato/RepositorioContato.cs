using eAgenda.Dominio.ModuloContato;
using eAgenda.Infraestrura.Compartilhado;

namespace eAgenda.Infraestrutura.ModuloContato;
public class RepositorioContato : RepositorioBaseEmArquivo<Contato>, IRepositorioContato
{
    public RepositorioContato(ContextoDados contexto) : base(contexto) { }

    protected override List<Contato> ObterRegistros()
    {
        return contexto.Contatos;
    }
}
