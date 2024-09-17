using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using ServerLibrary.Data;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(c =>
{
    c.PostProcess = doc =>
    {
        doc.Info.Version = "v1";
        doc.Info.Title = "Sales service";
        doc.Info.Description = "Sales REST API";
    };

    c.AddSecurity("Bearer", new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        Description = "Type into the textbox: {your JWT token}."
    });
    c.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));

    c.SchemaSettings.UseXmlDocumentation = true;
});

builder.Services.AddDbContext<AppDbContext>(op =>
{
    op.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ??
                 throw new InvalidOperationException("Connection string not found"));
});

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi();

app.UseHttpsRedirection();

app.Run();