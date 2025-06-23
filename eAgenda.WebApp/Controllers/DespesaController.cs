using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrura.Compartilhado;
using eAgenda.Infraestrutura.ModuloCategoria;
using eAgenda.Infraestrutura.ModuloDespesa;
using eAgenda.WebApp.Extensions;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eAgenda.WebApp.Controllers;

[Route("despesas")]
public class DespesaController : Controller
{
    private readonly ContextoDados contexto;
    private readonly IRepositorioDespesa repositorioDespesa;
    private readonly IRepositorioCategoria repositorioCategoria;

    public DespesaController()
    {
        contexto = new ContextoDados(true);
        repositorioDespesa = new RepositorioDespesa(contexto);
        repositorioCategoria = new RepositorioCategoria(contexto);
    }

    [HttpGet]
    public IActionResult Index()
    {
        var registros = repositorioDespesa.SelecionarRegistros();

        var visualizarVM = new VisualizarDespesasViewModel(registros);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        var categoriasDisponiveis = repositorioCategoria.SelecionarRegistros();

        var cadastrarVM = new CadastrarDespesaViewModel(categoriasDisponiveis);

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarDespesaViewModel cadastrarVM)
    {
        var categoriasDisponiveis = repositorioCategoria.SelecionarRegistros();

        if (!ModelState.IsValid)
        {
            foreach (var cd in categoriasDisponiveis)
            {
                var selecionarVM = new SelectListItem(cd.Titulo, cd.Id.ToString());

                cadastrarVM.CategoriasDisponiveis?.Add(selecionarVM);
            }

            return View(cadastrarVM);
        }

        var despesa = cadastrarVM.ParaEntidade();

        // Adiciona as categorias selecionadas à despesa
        var categoriasSelecionadas = cadastrarVM.CategoriasSelecionadas;

        if (categoriasSelecionadas is not null)
        {
            foreach (var cs in categoriasSelecionadas)
            {
                foreach (var cd in categoriasDisponiveis)
                {
                    if (cs.Equals(cd.Id))
                    {
                        despesa.RegistarCategoria(cd);
                        break;
                    }
                }
            }
        }

        repositorioDespesa.CadastrarRegistro(despesa);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public ActionResult Editar(Guid id)
    {
        var categoriasDisponiveis = repositorioCategoria.SelecionarRegistros();

        var registroSelecionado = repositorioDespesa.SelecionarRegistroPorId(id);

        var editarVM = new EditarDespesaViewModel(
            id,
            registroSelecionado.Descricao,
            registroSelecionado.Valor,
            registroSelecionado.DataOcorencia,
            registroSelecionado.FormaPagamento,
            registroSelecionado.Categorias,
            categoriasDisponiveis
        );

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public ActionResult Editar(Guid id, EditarDespesaViewModel editarVM)
    {
        var categoriasDisponiveis = repositorioCategoria.SelecionarRegistros();

        if (!ModelState.IsValid)
        {
            foreach (var cd in categoriasDisponiveis)
            {
                var selecionarVM = new SelectListItem(cd.Titulo, cd.Id.ToString());

                editarVM.CategoriasDisponiveis?.Add(selecionarVM);
            }

            return View(editarVM);
        }

        // Obtém dados editados
        var despesaEditada = editarVM.ParaEntidade();
        var categoriasSelecionadas = editarVM.CategoriasSelecionadas;

        var despesaSelecionada = repositorioDespesa.SelecionarRegistroPorId(id);

        // Remove as categorias anteriores da despesa
        foreach (var categoria in despesaSelecionada.Categorias.ToList())
            despesaSelecionada.RemoverCategoria(categoria);

        // Adiciona as categorias selecionadas
        if (categoriasSelecionadas is not null)
        {
            foreach (var idSelecionado in categoriasSelecionadas)
            {
                foreach (var categoriaDisponivel in categoriasDisponiveis)
                {
                    if (categoriaDisponivel.Id.Equals(idSelecionado))
                    {
                        despesaSelecionada.RegistarCategoria(categoriaDisponivel);
                        break;
                    }
                }
            }
        }

        // Atualiza os dados da despesa selecionada
        repositorioDespesa.EditarRegistro(id, despesaEditada);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        var registroSelecionado = repositorioDespesa.SelecionarRegistroPorId(id);

        var excluirVM = new ExcluirDespesaViewModel(registroSelecionado.Id, registroSelecionado.Descricao);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        repositorioDespesa.ExcluirRegistro(id);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("detalhes/{id:guid}")]
    public IActionResult Detalhes(Guid id)
    {
        var registroSelecionado = repositorioDespesa.SelecionarRegistroPorId(id);

        var detalhesVM = new DetalhesDespesaViewModel(
            id,
            registroSelecionado.Descricao,
            registroSelecionado.Valor,
            registroSelecionado.DataOcorencia,
            registroSelecionado.FormaPagamento,
            registroSelecionado.Categorias
        );

        return View(detalhesVM);
    }
}
