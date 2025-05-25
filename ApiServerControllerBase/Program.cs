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
 * Entity Framework Core 사용 시
 */
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection"));

    // 마이그레이션 파일을 현재 프로젝트에 생성하려면 아래와 같이 사용.
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
