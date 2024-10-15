using Imsat.Bashmaky.Model;
using Imsat.Bashmaky.Model.Database;
using Imsat.Bashmaky.Model.Database.Entities;
using Imsat.Bashmaky.Model.Database.Entities.Devices;
using Imsat.Bashmaky.Model.Database.Types;
using Imsat.Bashmaky.Model.Settings;
using Imsat.Bashmaky.Web;
using Imsat.Bashmaky.Web.Db;
using Imsat.Bashmaky.Web.Helpers;
using Imsat.Bashmaky.Web.Services;
using Imsat.Web.Mqtt;
using Imsat.Web.Toolkits.DatabaseLogger;
using Imsat.Web.Toolkits.DatabaseLogging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Configuration;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using uPLibrary.Networking.M2Mqtt;


var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);
WebApplication app = CreateApp(builder);

//app.MapPost("/signIn", async (string? returnUrl, HttpContext context) =>
//{
//    // получаем из формы email и пароль
//    var form = context.Request.Form;
//    // если email и/или пароль не установлены, посылаем статусный код ошибки 400
//    if (!form.ContainsKey("email") || !form.ContainsKey("password"))
//        return Results.BadRequest("Email и/или пароль не установлены");

//    string email = form["email"];
//    string password = form["password"];

//    return Results.Redirect(returnUrl ?? "/");
//});


if (!await MigrateDatabase(app))
    return;

#if DEBUG
await DebugAdding(app);
#endif
app.Run();


public partial class Program
{
    static void ConfigureServices(WebApplicationBuilder builder)
    {

        // Add services to the container.
        builder.Services.AddRazorPages();
        var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
        builder.Services.AddDbContext<AppDbContext>(option => option.UseNpgsql(connectionString));
        builder.Services.AddDbContext<LoggingDbContext>(options => options.UseNpgsql(connectionString));
        builder.Services.AddDbContext<UserDbContext>(options => options.UseNpgsql(connectionString));
        builder.Services.AddScoped<DomainService>();
        builder.Services.AddSingleton(services => new Func<DomainService>(() => services.CreateScope().ServiceProvider.GetRequiredService<DomainService>()));
        builder.Services.AddHostedService<MqttBackgroudService>();
        builder.Services.AddHostedService<ConnectionCheckerService>();
        builder.Services.AddHostedService<BoxCompletionService>();
        builder.Services.Configure<AppConfig>(builder.Configuration.GetSection(nameof(AppConfig)));
        var logConfig = builder.Configuration.GetSection("Logging").Get<LoggingSettings>();
        builder.Logging.AddProvider(new DbLoggerProvider<LoggingDbContext>(builder.Services.BuildServiceProvider(), logConfig));

        builder.Services.AddSingleton<IMqttMessageHandler, SaveToDbService>();
        builder.Services.AddMqttSystem();

        builder.Services.AddControllersWithViews(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
        })
            .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
        );

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/signIn");
        builder.Services.AddAuthorization();
        builder.Services.AddCors(opts =>
        {
            opts.AddDefaultPolicy(pb =>
            pb.AllowAnyOrigin()
            .AllowCredentials()
            .WithHeaders(new[] { "Authorization", "authorization", "kk-refresh-token", "kk-access-token" })
            );
        });

    }

    async static private Task<bool> MigrateDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        await using var appContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await using var logContext = scope.ServiceProvider.GetRequiredService<LoggingDbContext>();
        await using var userContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        try
        {
            await appContext.Database.MigrateAsync();
            ReloadTypes(appContext);
            await logContext.Database.MigrateAsync();
            ReloadTypes(logContext);
            await userContext.Database.MigrateAsync();
            ReloadTypes(userContext);
            return true;
        }
        catch
        {
            return false;
        }

        void ReloadTypes(DbContext context)
        {
            if (context.Database.GetDbConnection() is NpgsqlConnection npgsqlConnection)
            {
                npgsqlConnection.Open();
                try
                {
                    npgsqlConnection.ReloadTypes();
                }
                finally
                {
                    npgsqlConnection.Close();
                }
            }
        }
    }
    private static async Task DebugAdding(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        await using var gameContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (gameContext.Boxes.Any())
            return;
        var sign = new SignalTerminal { };
        var sign2 = new SignalBashmak { Mac = "C2-E2-82-AD-B5-57" };
        var term1 = new Terminal { Imei = "123qwe" };
        var bash1 = new Bashmak { Mac = "C2-E2-82-AD-B5-57", St=ST.PutInBox };
        var bash2 = new Bashmak { Mac = "C2-E2-82-AD-25-57", St=ST.PutOnRail };
        var bash3 = new Bashmak { Mac = "C2-E2-82-AD-00-58", St=ST.Raised };
        var bash4 = new Bashmak { Mac = "C2-E2-82-AA-99-99", St=ST.Raised };
        var box1 = new Box { Mac = "45-78-92-AA-33-14", Bashmaks = new List<Bashmak> { bash1, bash2, bash3 } };
        var box2 = new Box { Mac = "45-78-92-AA-33-16", Bashmaks = new List<Bashmak> { bash4} };
        gameContext.Add(new Station { Id = 1, Name = "Станция №1", Signals = new List<BaseEntity> { box1, box2, bash4, bash3, bash2, bash1, term1, sign, sign2 } });
        gameContext.Add(new Station { Id = 2, Name = "Станция №2" });
        gameContext.Add(new Railway { Id = 1, Name = "Путь №1" });
        gameContext.Add(new Railway { Id = 2, Name = "Путь №2" });
        await gameContext.SaveChangesAsync();

    }
    static WebApplication CreateApp(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthentication();

        app.MapRazorPages();
        app.MapControllers();
  
        return app;
    }
}


