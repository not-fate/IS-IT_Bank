  
CREATE TABLE [dbo].[account](
[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
[phone] [varchar](30) NOT NULL,
[email] [varchar](100) NOT NULL,
[passhash] [varchar](100) NOT NULL,
[firstname] [nvarchar](100) NOT NULL,
[lastname] [nvarchar](100) NOT NULL,
[isadmin] [bit] NOT NULL default 0);

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
[detail] [nvarchar](1024) NOT NULL);


SET IDENTITY_INSERT [dbo].[account] ON;

INSERT [dbo].[account] ([id], [phone], [email], [passhash], [firstname], [lastname], [isadmin]) VALUES (1, N'89266666666', N'none@gmail.com', N'A6xnQhbz4Vx2HuGl4lXwZ5U2I8iziLRFnhP5eNfIRvQ=', N'Vanno', N'Ivv', 0);

INSERT [dbo].[account] ([id], [phone], [email], [passhash], [firstname], [lastname], [isadmin]) VALUES (2, N'89267777777', N'777@gmail.com', N'A6xnQhbz4Vx2HuGl4lXwZ5U2I8iziLRFnhP5eNfIRvQ=', N'Admin', N'Admin', 1);

INSERT [dbo].[account] ([id], [phone], [email], [passhash], [firstname], [lastname], [isadmin]) VALUES (3, N'89265555555', N'77@gmail.com', N'A6xnQhbz4Vx2HuGl4lXwZ5U2I8iziLRFnhP5eNfIRvQ=', N'Seven', N'Women', 0);

SET IDENTITY_INSERT [dbo].[account] OFF

INSERT [dbo].[balance] ([account_id], [amount], [maxcredit]) VALUES (1, 1300, 0)

INSERT [dbo].[balance] ([account_id], [amount], [maxcredit]) VALUES (2, -2100, 5000000)

INSERT [dbo].[balance] ([account_id], [amount], [maxcredit]) VALUES (3, 800, 0)

INSERT [dbo].[transact] ([id], [account_from], [account_to], [amount], [extern_account], [detail]) VALUES (1737058845516957, 2, 1, 2000, N'', N'');

INSERT [dbo].[transact] ([id], [account_from], [account_to], [amount], [extern_account], [detail]) VALUES (1737062953752356, 2, 3, 500, N'', N'');

INSERT [dbo].[transact] ([id], [account_from], [account_to], [amount], [extern_account], [detail]) VALUES (1737064994241364, 1, 3, 300, N'', N'');

INSERT [dbo].[transact] ([id], [account_from], [account_to], [amount], [extern_account], [detail]) VALUES (1737065093106859, 1, 3, 100, N'', N'');

INSERT [dbo].[transact] ([id], [account_from], [account_to], [amount], [extern_account], [detail]) VALUES (1737065122835465, 1, 3, 100, N'', N'');

INSERT [dbo].[transact] ([id], [account_from], [account_to], [amount], [extern_account], [detail]) VALUES (1737065342030361, 1, 2, 400, N'', N'');

INSERT [dbo].[transact] ([id], [account_from], [account_to], [amount], [extern_account], [detail]) VALUES (1737067631222668, 3, 1, 200, N'', N'');


