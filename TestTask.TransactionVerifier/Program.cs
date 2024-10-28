using Microsoft.OpenApi.Models;
using TestTask.TransactionVerifier.DataAccess.Setup;
using TestTask.TransactionVerifier.WebApi.Services.Implementations;
using TestTask.TransactionVerifier.WebApi.Services.Interfaces;
using TestTask.TransactionVerifier.WebApi.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // чтобы было удобнее дебагать
    c.SchemaFilter<EnumSchemaFilter>();
});

builder.Services.AddDefaultApiVersioning();
builder.Services.AddCustomCors();

builder.Services.RegisterDbContext(o =>
{
    o.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
});

builder.Services.AddScoped<ICsvProcessingService, CsvTransactionService>();
builder.Services.AddScoped<ICsvFileService, CsvFileService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

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
