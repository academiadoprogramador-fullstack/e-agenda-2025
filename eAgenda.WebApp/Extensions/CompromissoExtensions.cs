using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.WebApp.Models;

namespace eAgenda.WebApp.Extensions;

public static class CompromissoExtensions
{
    public static Compromisso ParaEntidade(
        this FormularioCompromissoViewModel formularioVM,
        List<Contato> contatos
    )
    {
        Contato? contatoSelecionado = null;

        foreach (var c in contatos)
        {
            if (c.Id.Equals(formularioVM.ContatoId))
            {
                contatoSelecionado = c;
                break;
            }
        }

        return new Compromisso(
            formularioVM.Assunto,
            formularioVM.Data,
            formularioVM.HoraInicio,
            formularioVM.HoraTermino,
            contatoSelecionado!,
            formularioVM.Tipo,
            formularioVM.Local,
            formularioVM.Link
        );
    }

    public static DetalhesCompromissoViewModel ParaDetalhesVM(this Compromisso compromisso)
    {
        return new DetalhesCompromissoViewModel(
                compromisso.Id,
                compromisso.Assunto,
                compromisso.Data,
                compromisso.HoraInicio,
                compromisso.HoraTermino,
                compromisso.Contato.Nome,
                compromisso.Tipo,
                compromisso.Local,
                compromisso.Link
        );
    }
}
