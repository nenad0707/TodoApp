CREATE PROCEDURE [dbo].[spTodos_UpdateTask]
	@Task NVARCHAR(50),
	@AssignedTo INT,
	@TodoId INT
AS
BEGIN
	UPDATE dbo.Todos
	SET [Task] = @Task
	WHERE [AssignedTo] = @AssignedTo
		AND [Id] = @TodoId;
END
