﻿CREATE TABLE [dbo].[Users]
(
	Id INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash VARBINARY(64) NOT NULL,
    PasswordSalt VARBINARY(128) NOT NULL
)
