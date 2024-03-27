CREATE PROCEDURE [dbo].[CreateUser]
    @Username NVARCHAR(50),
    @PasswordHash VARBINARY(64),
    @PasswordSalt VARBINARY(128)
AS
BEGIN
    INSERT INTO Users (Username, PasswordHash, PasswordSalt)
    VALUES (@Username, @PasswordHash, @PasswordSalt)
    SELECT SCOPE_IDENTITY() AS Id;
END