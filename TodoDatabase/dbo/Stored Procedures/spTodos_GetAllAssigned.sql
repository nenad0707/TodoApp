CREATE PROCEDURE [dbo].[spTodos_GetAllAssigned]
    @AssignedTo INT,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SELECT [Id], [Task], [AssignedTo], [IsCompleted] 
    FROM Todos 
    WHERE AssignedTo = @AssignedTo
    ORDER BY Id
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
