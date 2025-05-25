using RepositoryBase.Models;
using RepositoryBase;
using RepositoryEfCore;
using Microsoft.EntityFrameworkCore;
using RepositoryEfCore.Data;
using ApiServerMinimalAPIs.Config;
using ApiServerMinimalAPIs.Endpoints;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using ApiServerMinimalAPIs;
using ApiServerMinimalAPIs.DTOs;
using FluentValidation;
using RepositoryDapper;

var builder = WebApplication.CreateBuilder(args);

/*
 * Entity Framework Core 사용 시
 */
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection"));

//    // 마이그레이션 파일을 현재 프로젝트에 생성하려면 아래와 같이 사용.
//    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConntion"),
//    //    sqlOptions => sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
//});

builder.Services.AddOpenApi();

builder.Services.AddOutputCache();
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
//builder.Services.AddScoped<IRepository<TodoItem>, Repository<TodoItem>>();    // Ef Core
builder.Services.AddScoped<IRepository<TodoItem>, TodoRepository>();    // Dapper

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // 개발 환경에서 예외 상세 메시지 전송
    //app.UseDeveloperExceptionPage();

    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "ServerAPI");
    });
}

// 전역 예외 처리 사용
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
            Message = "서버 오류가 발생했습니다."
        });
    });
});

app.UseOutputCache();

app.UseHttpsRedirection();

app.MapGroup("/api/todos").MapTodosEndpoint();

app.Run();