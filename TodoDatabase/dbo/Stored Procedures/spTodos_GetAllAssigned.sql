CREATE PROCEDURE [dbo].[spTodos_GetAllAssigned]
	@AssignedTo INT
AS
BEGIN
	SELECT [Id], [Task], [AssignedTo], [IsCompleted] 
	FROM Todos 
	WHERE AssignedTo = @AssignedTo;
END

