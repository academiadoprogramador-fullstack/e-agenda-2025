using Dapper;
using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrura.Compartilhado;
using eAgenda.Infraestrutura.DapperOrm.ModuloCompromisso;
using eAgenda.Infraestrutura.DapperOrm.ModuloContato;
using eAgenda.Infraestrutura.ModuloCategoria;
using eAgenda.Infraestrutura.ModuloDespesa;
using eAgenda.Infraestrutura.ModuloTarefa;
using eAgenda.Infraestrutura.SqlServer.ModuloContato;
using eAgenda.WebApp.ActionFilters;
using eAgenda.WebApp.DependencyInjection;
using System.Data;

namespace eAgenda.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add<ValidarModeloAttribute>();
            options.Filters.Add<LogarAcaoAttribute>();
        });

        builder.Services.AddScoped<ContextoDados>((_) => new ContextoDados(true));
        builder.Services.AddScoped<IRepositorioContato, RepositorioContatoComDapper>();
        builder.Services.AddScoped<IRepositorioCompromisso, RepositorioCompromissoComDapper>();
        builder.Services.AddScoped<IRepositorioCategoria, RepositorioCategoria>();
        builder.Services.AddScoped<IRepositorioDespesa, RepositorioDespesa>();
        builder.Services.AddScoped<IRepositorioTarefa, RepositorioTarefa>();

        SqlMapper.AddTypeHandler(new TimeSpanConverter());

        builder.Services.AddSerilogConfig(builder.Logging);

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

public class TimeSpanConverter : SqlMapper.TypeHandler<TimeSpan>
{
    public override TimeSpan Parse(object value)
    {
        return TimeSpan.FromTicks((long)value);
    }

    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        parameter.Value = value;
    }
}