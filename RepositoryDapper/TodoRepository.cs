using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using RepositoryBase;
using RepositoryBase.Models;
using static Dapper.SqlMapper;


namespace RepositoryDapper;

public class TodoRepository : IRepository<TodoItem>
{
    private readonly string? _connectionString;

    public TodoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySqlConnection");
    }

    public async Task<TodoItem> CreateAsync(TodoItem entity)
    {
        await using var connection = new MySqlConnection(_connectionString);

        var id = await connection.ExecuteScalarAsync<int>(TodoQuery.InsertTodoItem, entity);
        var todoItem = await GetByIdAsync(id);
        return todoItem!;
    }

    public async Task<List<TodoItem>> GetAllAsync()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var todoList = await connection.QueryAsync<TodoItem>(TodoQuery.LoadAllTodoItems);
        return [.. todoList];
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var todoItem = await connection.QueryFirstOrDefaultAsync<TodoItem>(TodoQuery.LoadTodoItem, new { Id = id });
        return todoItem;
    }

    public async Task RemoveAsync(TodoItem entity)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync(TodoQuery.DeleteTodoItem, entity);
    }

    public async Task RemoveAll()
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync(TodoQuery.DeleteAllTodoItems);
    }

    public async Task UpdateAsync(TodoItem entity)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync(TodoQuery.UpdateTodoItem, entity);
    }
}
