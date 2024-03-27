CREATE PROCEDURE [dbo].[GetUserByUsername]
	 @Username NVARCHAR(50)
AS
BEGIN
	SELECT * FROM Users WHERE Username = @Username
END

