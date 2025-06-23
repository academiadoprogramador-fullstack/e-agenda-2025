using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Infraestrura.Compartilhado;

namespace eAgenda.Infraestrutura.ModuloCategoria;

public class RepositorioCategoria : RepositorioBaseEmArquivo<Categoria>, IRepositorioCategoria
{
    public RepositorioCategoria(ContextoDados contexto) : base(contexto) { }

    protected override List<Categoria> ObterRegistros()
    {
        return contexto.Categorias;
    }
}
