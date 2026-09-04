using BibliotecaAPI.Extensions;
using BibliotecaAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBibliotecaApi(builder.Configuration);

var app = builder.Build();
await app.InitializeDatabaseAsync();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Biblioteca API v1");
    options.RoutePrefix = "docs";
});
app.UseAuthorization();
app.MapControllers();

app.Run();
