CREATE PROCEDURE [dbo].[spTodos_Create]
	@Task NVARCHAR(50),
	@AssignedTo INT
AS
BEGIN
	INSERT INTO dbo.Todos ([Task], [AssignedTo])
	VALUES (@Task, @AssignedTo);

	SELECT [Id], [Task], [AssignedTo], [IsCompleted] 
	FROM dbo.Todos
	WHERE Id = SCOPE_IDENTITY();
END

