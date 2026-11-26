using EstudosCSharp.Data;
using EstudosCSharp.Repositories;
using EstudosCSharp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Registra a classe de conexão
builder.Services.AddScoped<Conexao>();

// Registra os repositórios do BD
builder.Services.AddScoped<CidadeRepository>();
builder.Services.AddScoped<EstadoRepository>();
builder.Services.AddScoped<UsuarioRepository>();

// Registra o serviço de autenticação
builder.Services.AddScoped<AutenticacaoService>();

// Autenticação por Cookie.
builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Conta/Login";

        options.LogoutPath = "/Conta/Logout";

        options.AccessDeniedPath = "/Conta/AcessoNegado";

        options.Cookie.Name = "EstudosCSharp.Auth";

        options.Cookie.HttpOnly = true;

        options.Cookie.SecurePolicy =
            CookieSecurePolicy.Always;

        options.Cookie.SameSite = SameSiteMode.Lax;

        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        options.SlidingExpiration = true;
    });

// Autorização.
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// A ordem destes métodos é importante.
app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
