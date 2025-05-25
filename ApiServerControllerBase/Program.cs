using ApiServerControllerBase;
using ApiServerControllerBase.Config;
using ApiServerControllerBase.DTOs;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RepositoryBase;
using RepositoryBase.Models;
using RepositoryDapper;
using RepositoryEfCore;
using RepositoryEfCore.Data;
using System.Net;


var builder = WebApplication.CreateBuilder(args);

/*
 * Entity Framework Core ��� ��
 */
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection"));

    // ���̱׷��̼� ������ ���� ������Ʈ�� �����Ϸ��� �Ʒ��� ���� ���.
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConntion"),
    //    sqlOptions => sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
builder.Services.AddScoped<IRepository<TodoItem>, Repository<TodoItem>>();    // Ef Core
//builder.Services.AddScoped<IRepository<TodoItem>, TodoRepository>();    // Dapper

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // ���� ȯ�濡�� ���� �� �޽��� ����
    //app.UseDeveloperExceptionPage();

    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "ServerAPI");
    });
}

// ���� ���� ó�� ���
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        if (exception != null)
        {
            await ErrorReporter.NotifyExceptionAsync(exception);
        }

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new ErrorDto
        {
            Message = "���� ������ �߻��߽��ϴ�."
        });
    });
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
