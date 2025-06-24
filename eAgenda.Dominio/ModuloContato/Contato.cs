using eAgenda.Dominio.Compartilhado;

namespace eAgenda.Dominio.ModuloContato;

public class Contato : EntidadeBase<Contato>
{
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string Email { get; set; }
    public string? Empresa { get; set; }
    public string? Cargo { get; set; }

    public Contato() { }

    public Contato(string nome, string telefone, string email, string? empresa, string? cargo) : this()
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Telefone = telefone;
        Email = email;
        Empresa = empresa;
        Cargo = cargo;
    }

    public override void AtualizarRegistro(Contato registroEditado)
    {
        Nome = registroEditado.Nome;
        Email = registroEditado.Email;
        Telefone = registroEditado.Telefone;
        Cargo = registroEditado.Cargo;
        Empresa = registroEditado.Empresa;
    }
}