﻿namespace eAgenda.Dominio.ModuloTarefa;

public interface IRepositorioTarefa
{
    public void Cadastrar(Tarefa tarefa);
    public bool Editar(Guid idTarefa, Tarefa tarefaEditada);
    public bool Excluir(Guid idTarefa);
    public List<Tarefa> SelecionarTarefas();
    public List<Tarefa> SelecionarTarefasPendentes();
    public List<Tarefa> SelecionarTarefasConcluidas();
    public Tarefa? SelecionarTarefaPorId(Guid idTarefa);
};