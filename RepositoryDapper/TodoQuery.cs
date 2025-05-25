namespace RepositoryDapper;

public static class TodoQuery
{
    public const string InsertTodoItem = """
        INSERT INTO t_todos (title) VALUES (@Title);
        SELECT LAST_INSERT_ID();
        """;        

    public const string LoadAllTodoItems = """
        SELECT 
        id AS Id, 
        title AS Title, 
        is_completed AS IsCompleted,
        completed_date AS CompletedAt, 
        created_date AS CreatedAt 
        FROM t_todos;
        """;

    public const string LoadTodoItem = """
        SELECT 
        id AS Id, 
        title AS Title, 
        is_completed AS IsCompleted,
        completed_date AS CompletedAt, 
        created_date AS CreatedAt 
        FROM t_todos 
        WHERE id = @Id;
        """;

    public const string UpdateTodoItem = """
        UPDATE t_todos SET title = @Title, is_completed = @IsCompleted, completed_date = @CompletedAt
        WHERE id = @Id;
        """;

    public const string DeleteTodoItem = "DELETE FROM t_todos WHERE id = @Id;";

    public const string DeleteAllTodoItems = "TRUNCATE TABLE t_todos;";
}
