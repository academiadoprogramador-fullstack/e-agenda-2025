using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrura.Compartilhado;
using eAgenda.Infraestrutura.ModuloCategoria;
using eAgenda.Infraestrutura.ModuloCompromisso;
using eAgenda.Infraestrutura.ModuloContato;
using eAgenda.Infraestrutura.ModuloDespesa;
using eAgenda.Infraestrutura.ModuloTarefa;
using eAgenda.WebApp.ActionFilters;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace eAgenda.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add<ValidarModeloAttribute>();
            options.Filters.Add<LogActionResponseAttribute>();
        });

        builder.Services.AddScoped<ContextoDados>((_) => new ContextoDados(true));
        builder.Services.AddScoped<IRepositorioContato, RepositorioContato>();
        builder.Services.AddScoped<IRepositorioCompromisso, RepositorioCompromisso>();
        builder.Services.AddScoped<IRepositorioCategoria, RepositorioCategoria>();
        builder.Services.AddScoped<IRepositorioDespesa, RepositorioDespesa>();
        builder.Services.AddScoped<IRepositorioTarefa, RepositorioTarefa>();

        var caminhoAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        var caminhoArquivo = Path.Combine(caminhoAppData, "eAgenda", "error.log");

        Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Information()
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .WriteTo.File(new CompactJsonFormatter(), caminhoArquivo, LogEventLevel.Error)
          .CreateLogger();

        builder.Logging.ClearProviders();

        builder.Services.AddSerilog(Log.Logger);

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
            app.UseExceptionHandler("/erro");
        else
            app.UseDeveloperExceptionPage();

        app.UseAntiforgery();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.MapDefaultControllerRoute();

        app.Run();
    }
}
