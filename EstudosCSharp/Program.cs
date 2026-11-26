using EstudosCSharp.Data;
using EstudosCSharp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Registra a classe de conexão
builder.Services.AddScoped<Conexao>();

// Registra os repositórios do BD
builder.Services.AddScoped<CidadeRepository>();
builder.Services.AddScoped<EstadoRepository>();
builder.Services.AddScoped<UsuariosRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
