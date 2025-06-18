namespace eAgenda.Dominio.ModuloTarefa;

public class ItemTarefa
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public bool Concluido { get; set; }

    public ItemTarefa() { }

    public ItemTarefa(string titulo) : this()
    {
        Id = Guid.NewGuid();
        Titulo = titulo;
        Concluido = false;
    }

    public void Concluir()
    {
        Concluido = true;
    }

    public void MarcarPendente()
    {
        Concluido = false;
    }

    public override string ToString()
    {
        return $"{Titulo}";
    }
}