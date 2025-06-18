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

        if (registroSelecionado != null)
        {
            registros.Remove(registroSelecionado);

            contexto.Salvar();

            return true;
        }

        return false;
    }

    public Tarefa? SelecionarTarefaPorId(Guid idTarefa)
    {
        foreach (var t in registros)
        {
            if (t.Id == idTarefa)
                return t;
        }

        return null;
    }

    public List<Tarefa> SelecionarTarefas()
    {
        return registros;
    }

    public List<Tarefa> SelecionarTarefasPendentes()
    {
        var tarefasAtivas = new List<Tarefa>();

        foreach (var item in registros)
        {
            if (!item.Concluida)
                tarefasAtivas.Add(item);
        }

        return tarefasAtivas;
    }

    public List<Tarefa> SelecionarTarefasConcluidas()
    {
        var tarefasConcluidas = new List<Tarefa>();

        foreach (var item in registros)
        {
            if (item.Concluida)
                tarefasConcluidas.Add(item);
        }

        return tarefasConcluidas;
    }
}
