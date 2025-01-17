IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{0}')
BEGIN
   CREATE DATABASE [0}];

   CREATE TABLE [dbo].[account](
	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[phone] [varchar](30) NOT NULL,
	[email] [varchar](100) NOT NULL,
	[passhash] [varchar](100) NOT NULL,
	[firstname] [varchar](100) NOT NULL,
	[lastname] [varchar](100) NOT NULL);

	CREATE TABLE [dbo].[balance](
	[account_id] [int] NOT NULL primary key,
	[amount] [bigint] NOT NULL,
	[maxcredit] [bigint] NOT NULL);

	CREATE TABLE [dbo].[transact](
	[id] [bigint] NOT NULL,
	[account_from] [int] NOT NULL,
	[account_to] [int] NOT NULL,
	[amount] [bigint] NOT NULL,
	[extern_account] [varchar](255) NOT NULL,
	[detail] [varchar](1024) NOT NULL);

END;