using eAgenda.Dominio.Compartilhado;

namespace eAgenda.Dominio.ModuloTarefa;

public class Tarefa : EntidadeBase<Tarefa>
{
    public string Titulo { get; set; }
    public PrioridadeTarefaEnum Prioridade { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataConclusao { get; set; }
    public List<ItemTarefa> Itens { get; set; }

    public decimal PercentualConcluido
    {
        get
        {
            if (Itens.Count == 0)
                return default;

            int qtdConcluidos = Itens.Count(i => i.Concluido);

            decimal percentualBase = qtdConcluidos / (decimal)Itens.Count * 100;

            return Math.Round(percentualBase, 2);
        }
    }

    public bool Concluida { get; set; }

    public Tarefa()
    {
        Itens = new List<ItemTarefa>();
    }

    public Tarefa(string titulo, PrioridadeTarefaEnum prioridade) : this()
    {
        Id = Guid.NewGuid();
        Titulo = titulo;
        Prioridade = prioridade;
        Concluida = false;
        DataCriacao = DateTime.Now;
    }

    public void Concluir()
    {
        Concluida = true;
        DataConclusao = DateTime.Now;    
    }

    public void MarcarPendente()
    {
        Concluida = false;
        DataConclusao = null;
    }

    public ItemTarefa? ObterItem(Guid idItem)
    {
        foreach (var i in Itens)
        {
            if (idItem.Equals(i.Id))
                return i;
        }

        return null;
    }

    public bool AdicionarItem(string titulo)
    {
        var item = new ItemTarefa(titulo);

        foreach (var i in Itens)
        {
            if (item.Id == i.Id)
                return false;
        }

        Itens.Add(item);

        MarcarPendente();

        return true;
    }

    public bool RemoverItem(ItemTarefa item)
    {
        Itens.Remove(item);

        MarcarPendente();

        return true;
    }

    public void ConcluirItem(ItemTarefa item)
    {
        item.Concluir();
    }

    public void MarcarItemPendente(ItemTarefa item)
    {
        item.MarcarPendente();

        MarcarPendente();
    }

    public override void AtualizarRegistro(Tarefa registroEditado)
    {
        Titulo = registroEditado.Titulo;
        Prioridade = registroEditado.Prioridade;
    }
}