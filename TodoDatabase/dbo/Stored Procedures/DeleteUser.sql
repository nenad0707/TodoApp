CREATE PROCEDURE [dbo].[DeleteUser]
    @Id INT
AS
BEGIN
    DELETE FROM Users WHERE Id = @Id
END
