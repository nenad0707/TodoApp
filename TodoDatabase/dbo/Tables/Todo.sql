CREATE TABLE [dbo].[Todo]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Task] NVARCHAR(50) NULL, 
    [AssignedTo] NVARCHAR(50) NULL, 
    [IsCompleted] BIT NULL
)
