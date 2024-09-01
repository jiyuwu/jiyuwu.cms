# jiyuwu.cms

## db
if you need database
```
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Base_DbService]') AND type IN ('U'))
	DROP TABLE [dbo].[Base_DbService]
GO

CREATE TABLE [dbo].[Base_DbService] (
  [DbServiceId] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [DbServiceName] nvarchar(20) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [GroupId] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [DbIpAddress] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [DatabaseName] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [UserId] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Pwd] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [PhoneNo] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Address] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [Enable] int  NULL,
  [Remark] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [CreateId] int  NULL,
  [Creator] nvarchar(30) COLLATE Chinese_PRC_CI_AS  NULL,
  [CreateDate] datetime  NULL,
  [ModifyId] int  NULL,
  [Modifier] nvarchar(30) COLLATE Chinese_PRC_CI_AS  NULL,
  [ModifyDate] datetime  NULL
)
GO

ALTER TABLE [dbo].[Base_DbService] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Primary Key structure for table Base_DbService
-- ----------------------------
ALTER TABLE [dbo].[Base_DbService] ADD CONSTRAINT [PK__Sys_DbSe__F01CDD7BDF8FB4D9] PRIMARY KEY CLUSTERED ([DbServiceId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Base_Log]') AND type IN ('U'))
	DROP TABLE [dbo].[Base_Log]
GO

CREATE TABLE [dbo].[Base_Log] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [BeginDate] datetime  NULL,
  [BrowserType] nvarchar(200) COLLATE Chinese_PRC_CI_AS  NULL,
  [ElapsedTime] int  NULL,
  [EndDate] datetime  NULL,
  [ExceptionInfo] nvarchar(max) COLLATE Chinese_PRC_CI_AS  NULL,
  [LogType] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [RequestParameter] nvarchar(max) COLLATE Chinese_PRC_CI_AS  NULL,
  [ResponseParameter] nvarchar(max) COLLATE Chinese_PRC_CI_AS  NULL,
  [Role_Id] int  NULL,
  [ServiceIP] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Success] int  NULL,
  [Url] nvarchar(4000) COLLATE Chinese_PRC_CI_AS  NULL,
  [UserIP] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [UserName] nvarchar(4000) COLLATE Chinese_PRC_CI_AS  NULL,
  [User_Id] int  NULL
)
GO

ALTER TABLE [dbo].[Base_Log] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Auto increment value for Base_Log
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Base_Log]', RESEED, 81)
GO


-- ----------------------------
-- Primary Key structure for table Base_Log
-- ----------------------------
ALTER TABLE [dbo].[Base_Log] ADD CONSTRAINT [PK_Sys_Log] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO
```

## api
C# app and api

### JIYUWU.App
A maui project

### JIYUWU.Api
All api

## uniapp
Mini program or official account

## web
Use vue create a project
