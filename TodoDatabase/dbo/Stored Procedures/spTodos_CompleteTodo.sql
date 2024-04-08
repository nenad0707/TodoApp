CREATE PROCEDURE [dbo].[spTodos_CompleteTodo]
	@AssignedTo INT,
	@TodoId INT
AS
BEGIN 
	UPDATE dbo.Todos
	SET [IsCompleted] = 1
	WHERE [AssignedTo] = @AssignedTo
		AND [Id] = @TodoId;
	SELECT @@ROWCOUNT
END