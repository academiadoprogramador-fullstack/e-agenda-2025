namespace eAgenda.Dominio.ModuloTarefa;

public interface IRepositorioTarefa
{
    public void Cadastrar(Tarefa tarefa);
    public bool Editar(Guid idTarefa, Tarefa tarefaEditada);
    public bool Excluir(Guid idTarefa);
    public void AdicionarItem(ItemTarefa item);
    public bool AtualizarItem(ItemTarefa itemAtualizado);
    public bool RemoverItem(ItemTarefa item);
    public List<Tarefa> SelecionarTarefas();
    public List<Tarefa> SelecionarTarefasPendentes();
    public List<Tarefa> SelecionarTarefasConcluidas();
    public Tarefa? SelecionarTarefaPorId(Guid idTarefa);
};