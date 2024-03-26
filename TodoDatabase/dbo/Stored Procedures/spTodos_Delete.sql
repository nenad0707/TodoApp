CREATE PROCEDURE [dbo].[spTodos_Delete]
	@AssignedTo INT,
	@TodoId INT
AS
BEGIN
	DELETE FROM dbo.Todos
	WHERE [AssignedTo] = @AssignedTo
		AND [Id] = @TodoId;
END

