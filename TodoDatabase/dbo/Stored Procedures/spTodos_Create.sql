CREATE PROCEDURE [dbo].[spTodos_Create]
	@Task NVARCHAR(50),
	@AssignedTo INT,
	@IsCompleted BIT
AS
BEGIN
	INSERT INTO dbo.Todos ([Task], [AssignedTo],[IsCompleted])
	VALUES (@Task, @AssignedTo, @IsCompleted);

	SELECT [Id], [Task], [AssignedTo], [IsCompleted] 
	FROM dbo.Todos
	WHERE Id = SCOPE_IDENTITY();
END

