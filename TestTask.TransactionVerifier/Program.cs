using Microsoft.OpenApi.Models;
using TestTask.TransactionVerifier.BusinessLogic.Services;
using TestTask.TransactionVerifier.BusinessLogic.Services.Abstractions;
using TestTask.TransactionVerifier.BusinessLogic.Setup;
using TestTask.TransactionVerifier.DataAccess.Setup;
using TestTask.TransactionVerifier.WebApi.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    c.SchemaFilter<EnumSchemaFilter>();
});

builder.Services.AddDefaultApiVersioning();
builder.Services.AddCustomCors();
builder.Services.RegisterCoreDependencies();

builder.Services.RegisterDbContext(o =>
{
    o.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html", permanent: false);
    return Task.CompletedTask;
});

app.UseCustomCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
