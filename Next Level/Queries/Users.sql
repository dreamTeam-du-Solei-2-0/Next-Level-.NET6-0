﻿CREATE TABLE Users
(
	[UserId]	UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[AccountId]	UNIQUEIDENTIFIER NOT NULL REFERENCES Accounts(AccountId),
	[Surname]	NVARCHAR(50)	 NOT NULL,
	[Name]		NVARCHAR(50)	 NOT NULL,
	[Secname]	NVARCHAR(50)	 NOT NULL,
	[Phone]		NVARCHAR(50),
	[Email]		NVARCHAR(50),
	[BirthDate]	DATETIME,		 
	[DeleteDt]	DATETIME		 
);

