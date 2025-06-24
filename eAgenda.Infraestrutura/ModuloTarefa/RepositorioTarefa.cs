using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrura.Compartilhado;

namespace eAgenda.Infraestrutura.ModuloTarefa;

public class RepositorioTarefa : IRepositorioTarefa
{
    private readonly ContextoDados contexto;
    private readonly List<Tarefa> registros;

    public RepositorioTarefa(ContextoDados contexto)
    {
        this.contexto = contexto;
        this.registros = contexto.Tarefas;
    }

    public void Cadastrar(Tarefa tarefa)
    {
        registros.Add(tarefa);

        contexto.Salvar();
    }

    public bool Editar(Guid idTarefa, Tarefa tarefaEditada)
    {
        var tarefaSelecionada = SelecionarTarefaPorId(idTarefa);

        if (tarefaSelecionada is null)
            return false;

        tarefaSelecionada.AtualizarRegistro(tarefaEditada);

        return true;
    }

    public bool Excluir(Guid idTarefa)
    {
        var registroSelecionado = SelecionarTarefaPorId(idTarefa);

        if (registroSelecionado is null)
            return false;

        registros.Remove(registroSelecionado);

        contexto.Salvar();

        return true;
    }

    public Tarefa? SelecionarTarefaPorId(Guid idTarefa)
    {
        return registros.Find(t => t.Id.Equals(idTarefa));
    }

    public List<Tarefa> SelecionarTarefas()
    {
        return registros;
    }

    public List<Tarefa> SelecionarTarefasPendentes()
    {
        return registros.FindAll(t => !t.Concluida);
    }

    public List<Tarefa> SelecionarTarefasConcluidas()
    {
        return registros.FindAll(t => t.Concluida);
    }
}
