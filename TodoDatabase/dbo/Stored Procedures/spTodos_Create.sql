CREATE PROCEDURE [dbo].[spTodos_Create]
	@Task NVARCHAR(50),
	@AssignedTo INT
AS
BEGIN
	INSERT INTO dbo.Todos ([Task], [AssignedTo])
	VALUES (@Task, @AssignedTo);
END

