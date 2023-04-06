CREATE TABLE Products(	
[ProductId]		UNIQUEIDENTIFIER	NOT NULL PRIMARY KEY,
[CategoryId]		UNIQUEIDENTIFIER	NOT NULL REFERENCES Categories(CategoryId),
[Photo]				VARBINARY(MAX),
[Name]				NVARCHAR(100)		NOT NULL,
[Description]		NTEXT,
[Price]					FLOAT							NOT NULL,
[Count]				INT								NOT NULL	default(0),
[DeleteDt]			DATETIME			
);
