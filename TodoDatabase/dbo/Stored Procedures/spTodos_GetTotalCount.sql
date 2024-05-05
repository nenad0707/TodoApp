CREATE PROCEDURE [dbo].[spTodos_GetTotalCount]
	@AssignedTo INT
AS
BEGIN
    SELECT COUNT(*) FROM Todos WHERE AssignedTo = @AssignedTo;
END