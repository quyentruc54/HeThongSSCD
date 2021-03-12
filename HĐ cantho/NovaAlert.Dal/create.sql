-- SQL Manager 2008 for SQL Server 3.2.0.1
-- ---------------------------------------
-- Host      : Dat-Laptop
-- Database  : NovaAlert2
-- Version   : Microsoft SQL Server  11.0.2100.60


SET NOCOUNT ON
GO

--
-- Dropping view ViewResult : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ViewResult]') AND OBJECTPROPERTY(id, N'IsView') = 1)
  DROP VIEW [dbo].[ViewResult]
GO

--
-- Dropping table User : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[User]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[User]
GO

--
-- Dropping table Unit : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[Unit]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[Unit]
GO

--
-- Dropping table RadioTime : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[RadioTime]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[RadioTime]
GO

--
-- Dropping table PO : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[PO]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[PO]
GO

--
-- Dropping table PhoneNumber : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[PhoneNumber]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[PhoneNumber]
GO

--
-- Dropping table Panel : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[Panel]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[Panel]
GO

--
-- Dropping table HotLine : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[HotLine]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[HotLine]
GO

--
-- Dropping table GroupUnitTask : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[GroupUnitTask]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[GroupUnitTask]
GO

--
-- Dropping table GroupUnit : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[GroupUnit]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[GroupUnit]
GO

--
-- Dropping table Group : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[Group]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[Group]
GO

--
-- Dropping table GlobalSetting : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[GlobalSetting]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[GlobalSetting]
GO

--
-- Dropping table Enum : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[Enum]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[Enum]
GO

--
-- Dropping table DisplayData : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DisplayData]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[DisplayData]
GO

--
-- Dropping table DbLog : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DbLog]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[DbLog]
GO

--
-- Dropping table DayTypeConfig : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DayTypeConfig]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[DayTypeConfig]
GO

--
-- Dropping table Channel : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[Channel]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[Channel]
GO

--
-- Dropping table CallLogDetail : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[CallLogDetail]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[CallLogDetail]
GO

--
-- Dropping table CallLog : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[CallLog]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[CallLog]
GO

--
-- Dropping table AlarmLog : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[AlarmLog]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[AlarmLog]
GO

--
-- Dropping table Alarm : 
--

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[Alarm]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  DROP TABLE [dbo].[Alarm]
GO

--
-- Definition for table User : 
--

CREATE TABLE [dbo].[User] (
  [Id] int IDENTITY(1, 1) NOT NULL,
  [Name] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  [Password] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
)
ON [PRIMARY]
GO

--
-- Definition for table Unit : 
--

CREATE TABLE [dbo].[Unit] (
  [UnitId] int IDENTITY(1, 1) NOT NULL,
  [Name] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  [NameAbbr] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  [Password] varchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
)
ON [PRIMARY]
GO

--
-- Definition for table RadioTime : 
--

CREATE TABLE [dbo].[RadioTime] (
  [ListOrder] tinyint NOT NULL,
  [DayType] tinyint NOT NULL,
  [StartTime] datetime NULL,
  [EndTime] datetime NULL,
  [IsEnabled] bit CONSTRAINT [DF__RadioTime__IsEna__34C8D9D1] DEFAULT 0 NOT NULL
)
ON [PRIMARY]
GO

--
-- Definition for table PO : 
--

CREATE TABLE [dbo].[PO] (
  [Id] tinyint NOT NULL,
  [Address] tinyint NOT NULL
)
ON [PRIMARY]
GO

--
-- Definition for table PhoneNumber : 
--

CREATE TABLE [dbo].[PhoneNumber] (
  [PhoneNumberId] int IDENTITY(1, 1) NOT NULL,
  [AreaCode] varchar(5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [Number] varchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  [IsRestricted] bit CONSTRAINT [DF_PhoneNumber_IsRestricted] DEFAULT 1 NOT NULL,
  [Address] tinyint NULL,
  [NameAbbr] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [Password] varchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [UnitName] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
ON [PRIMARY]
GO

--
-- Definition for table Panel : 
--

CREATE TABLE [dbo].[Panel] (
  [PanelId] tinyint NOT NULL,
  [POId] tinyint NOT NULL,
  [CurrentMode] tinyint NULL,
  [CurrentGroupId] int NULL
)
ON [PRIMARY]
GO

--
-- Definition for table HotLine : 
--

CREATE TABLE [dbo].[HotLine] (
  [HotLineId] int IDENTITY(1, 1) NOT NULL,
  [ChannelId] int NOT NULL,
  [GroupId] int NOT NULL,
  [PhoneNumberId] int NOT NULL
)
ON [PRIMARY]
GO

--
-- Definition for table GroupUnitTask : 
--

CREATE TABLE [dbo].[GroupUnitTask] (
  [Id] int IDENTITY(1, 1) NOT NULL,
  [GroupId] int NOT NULL,
  [PhoneNumberId] int NOT NULL,
  [CreatedDate] datetime NOT NULL,
  [Task] tinyint NOT NULL,
  [Level] tinyint NOT NULL,
  [Result] tinyint NOT NULL,
  [Duration] bigint NULL
)
ON [PRIMARY]
GO

--
-- Definition for table GroupUnit : 
--

CREATE TABLE [dbo].[GroupUnit] (
  [GroupId] int NOT NULL,
  [PhoneNumberId] int NOT NULL,
  [ListOrder] int NOT NULL,
  [IsDeleted] bit CONSTRAINT [DF__GroupUnit__IsDel__1920BF5C] DEFAULT 0 NOT NULL
)
ON [PRIMARY]
GO

--
-- Definition for table Group : 
--

CREATE TABLE [dbo].[Group] (
  [GroupId] int IDENTITY(1, 1) NOT NULL,
  [Name] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  [DeletedDate] datetime NULL
)
ON [PRIMARY]
GO

--
-- Definition for table GlobalSetting : 
--

CREATE TABLE [dbo].[GlobalSetting] (
  [Parameter] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  [Value] ntext COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO

--
-- Definition for table Enum : 
--

CREATE TABLE [dbo].[Enum] (
  [Type] varchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  [Value] tinyint NOT NULL,
  [Desc] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  [ListOrder] tinyint NULL,
  [Desc_VN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
)
ON [PRIMARY]
GO

--
-- Definition for table DisplayData : 
--

CREATE TABLE [dbo].[DisplayData] (
  [DisplayId] int NOT NULL,
  [PhoneNumber_1] int NULL,
  [PhoneNumber_2] int NULL
)
ON [PRIMARY]
GO

--
-- Definition for view ViewResult : 
--
GO
CREATE View [dbo].ViewResult AS
Select DisplayId as Id, sscd.PhoneNumberId, sscd.CreatedDate, 
(Case when sscd.UnitName Is NULL then ccpk.UnitName Else sscd.UnitName End) as UnitName,
sscd.Task, sscd.Level, sscd.Result, sscd.Duration,
ccpk.Task as Task_2, ccpk.Level as Level_2, ccpk.Result as Result_2, ccpk.Duration as Duration_2
From DisplayData dd
LEFT JOIN (select pn.PhoneNumberId, pn.UnitName, gut.CreatedDate, gut.Task, gut.Level, gut.Result, gut.Duration
	From GroupUnitTask gut 
	INNER JOIN PhoneNumber pn ON gut.PhoneNumberId = pn.PhoneNumberId
	where gut.Id IN (select top 1 Id from GroupUnitTask where PhoneNumberId = gut.PhoneNumberId order by Id DESC)) 
	sscd ON dd.PhoneNumber_1 = sscd.PhoneNumberId
LEFT JOIN (select pn.PhoneNumberId, pn.UnitName, gut.CreatedDate, gut.Task, gut.Level, gut.Result, gut.Duration
	From GroupUnitTask gut 
	INNER JOIN PhoneNumber pn ON gut.PhoneNumberId = pn.PhoneNumberId
	where gut.Id IN (select top 1 Id from GroupUnitTask where PhoneNumberId = gut.PhoneNumberId order by Id DESC))
	ccpk ON dd.PhoneNumber_2 = ccpk.PhoneNumberId
GO

--
-- Definition for table DbLog : 
--

CREATE TABLE [dbo].[DbLog] (
  [Id] bigint IDENTITY(1, 1) NOT NULL,
  [CreatedDate] datetime NOT NULL,
  [PanelId] tinyint NOT NULL,
  [UserId] int NULL,
  [Info] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
ON [PRIMARY]
GO

--
-- Definition for table DayTypeConfig : 
--

CREATE TABLE [dbo].[DayTypeConfig] (
  [DayOfWeek] tinyint NOT NULL,
  [DayType] tinyint NOT NULL
)
ON [PRIMARY]
GO

--
-- Definition for table Channel : 
--

CREATE TABLE [dbo].[Channel] (
  [ChannelId] int NOT NULL,
  [PhoneNumberId] int NOT NULL,
  [AutoRecording] bit CONSTRAINT [DF_Channel_AutoRecording] DEFAULT 0 NOT NULL,
  [AlertEnabled] bit CONSTRAINT [DF__Channel__AlertEn__1273C1CD] DEFAULT 0 NOT NULL,
  [MultiDestEnabled] bit CONSTRAINT [DF__Channel__MultiDe__1367E606] DEFAULT 0 NOT NULL,
  [HotUnitId] int NULL
)
ON [PRIMARY]
GO

--
-- Definition for table CallLogDetail : 
--

CREATE TABLE [dbo].[CallLogDetail] (
  [CallLogDetailId] uniqueidentifier NOT NULL,
  [CallLogId] uniqueidentifier NOT NULL,
  [ChannelId] int NULL,
  [UnitId] int NULL,
  [PhoneNumber] varchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [StartTime] datetime NOT NULL,
  [EndTime] datetime NULL,
  [UnitName] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [Record] varchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
ON [PRIMARY]
GO

--
-- Definition for table CallLog : 
--

CREATE TABLE [dbo].[CallLog] (
  [CallLogId] uniqueidentifier NOT NULL,
  [POId] int NOT NULL,
  [StartTime] datetime NOT NULL,
  [EndTime] datetime NULL,
  [CallType] tinyint NOT NULL,
  [DeletedDate] datetime NULL,
  [DeletedBy] int NULL
)
ON [PRIMARY]
GO

--
-- Definition for table AlarmLog : 
--

CREATE TABLE [dbo].[AlarmLog] (
  [Id] int IDENTITY(1, 1) NOT NULL,
  [AlarmType] tinyint NOT NULL,
  [AlarmTime] datetime NOT NULL
)
ON [PRIMARY]
GO

--
-- Definition for table Alarm : 
--

CREATE TABLE [dbo].[Alarm] (
  [AlarmId] int IDENTITY(1, 1) NOT NULL,
  [DayType] tinyint NOT NULL,
  [Time] datetime NOT NULL,
  [TimesOfPlaying] tinyint NOT NULL,
  [AlarmType] tinyint NOT NULL,
  [IsEnabled] bit CONSTRAINT [DF__Alarm__IsEnabled__35BCFE0A] DEFAULT 0 NOT NULL
)
ON [PRIMARY]
GO

--
-- Data for table dbo.Alarm  (LIMIT 0,500)
--

SET IDENTITY_INSERT [dbo].[Alarm] ON
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (1, 1, '20140101 06:00:00', 3, 1, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (2, 1, '20140101 06:30:00', 0, 2, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (3, 1, '20140101 07:30:00', 1, 3, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (4, 1, '20140101 11:30:00', 3, 4, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (5, 1, '20140101 13:20:00', 3, 5, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (6, 1, '20140101 13:20:00', 0, 6, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (7, 1, '20140101 13:35:00', 1, 7, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (8, 1, '20140101 16:30:00', 1, 8, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (9, 1, '20140101 19:00:00', 0, 9, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (10, 1, '20140101 19:30:00', 0, 10, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (11, 1, '20140101 21:00:00', 0, 11, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (12, 1, '20140101 21:00:00', 1, 12, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (13, 1, '20140101 21:30:00', 1, 13, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (14, 2, '20140101 06:00:00', 3, 1, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (15, 2, '20140101 06:30:00', 0, 2, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (16, 2, '20140101 07:30:00', 1, 3, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (17, 2, '20140101 09:50:00', 2, 4, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (18, 2, '20140101 13:15:00', 3, 5, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (19, 2, '20140101 13:30:00', 0, 6, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (20, 2, '20140101 13:30:00', 1, 7, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (21, 2, '20140101 16:30:00', 1, 8, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (22, 2, '20140101 19:00:00', 0, 9, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (23, 2, '20140101 19:30:00', 0, 10, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (24, 2, '20140101 21:00:00', 0, 11, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (25, 2, '20140101 21:30:00', 0, 12, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (26, 2, '20140101 21:45:00', 0, 13, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (27, 3, '20140101 06:30:00', 3, 1, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (28, 3, '20140101 06:30:00', 0, 2, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (29, 3, '20140101 07:30:00', 1, 3, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (30, 3, '20140101 11:00:00', 1, 4, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (31, 3, '20140101 13:15:00', 3, 5, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (32, 3, '20140101 13:20:00', 0, 6, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (33, 3, '20140101 13:30:00', 1, 7, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (34, 3, '20140101 17:00:00', 1, 8, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (35, 3, '20140101 19:00:00', 0, 9, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (36, 3, '20140101 19:30:00', 0, 10, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (37, 3, '20140101 21:00:00', 0, 11, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (38, 3, '20140101 21:30:00', 0, 12, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (39, 3, '20140101 21:45:00', 0, 13, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (40, 4, '20140101 06:30:00', 3, 1, 1)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (41, 4, '20140101 06:30:00', 0, 2, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (42, 4, '20140101 07:30:00', 1, 3, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (43, 4, '20140101 11:00:00', 1, 4, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (44, 4, '20140101 13:15:00', 3, 5, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (45, 4, '20140101 13:20:00', 0, 6, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (46, 4, '20140101 13:30:00', 1, 7, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (47, 4, '20140101 17:00:00', 1, 8, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (48, 4, '20140101 19:00:00', 0, 9, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (49, 4, '20140101 19:30:00', 0, 10, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (50, 4, '20140101 21:00:00', 0, 11, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (51, 4, '20140101 21:30:00', 0, 12, 0)
GO

INSERT INTO [dbo].[Alarm] ([AlarmId], [DayType], [Time], [TimesOfPlaying], [AlarmType], [IsEnabled])
VALUES 
  (52, 4, '20140101 22:00:00', 0, 13, 1)
GO

SET IDENTITY_INSERT [dbo].[Alarm] OFF
GO

--
-- Data for table dbo.Channel  (LIMIT 0,500)
--

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (1, 1, 1, 1, 1, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (2, 2, 1, 1, 1, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (3, 3, 1, 1, 1, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (4, 4, 1, 1, 1, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (5, 5, 1, 1, 1, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (6, 6, 1, 1, 1, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (7, 7, 1, 1, 1, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (8, 8, 1, 1, 1, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (9, 9, 1, 1, 1, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (10, 10, 1, 1, 1, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (11, 11, 1, 1, 1, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (12, 12, 1, 1, 1, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (13, 13, 1, 0, 0, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (14, 14, 1, 0, 0, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (15, 15, 1, 0, 0, NULL)
GO

INSERT INTO [dbo].[Channel] ([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled], [HotUnitId])
VALUES 
  (16, 16, 1, 0, 0, NULL)
GO

--
-- Data for table dbo.DayTypeConfig  (LIMIT 0,500)
--

INSERT INTO [dbo].[DayTypeConfig] ([DayOfWeek], [DayType])
VALUES 
  (0, 3)
GO

INSERT INTO [dbo].[DayTypeConfig] ([DayOfWeek], [DayType])
VALUES 
  (1, 1)
GO

INSERT INTO [dbo].[DayTypeConfig] ([DayOfWeek], [DayType])
VALUES 
  (2, 1)
GO

INSERT INTO [dbo].[DayTypeConfig] ([DayOfWeek], [DayType])
VALUES 
  (3, 1)
GO

INSERT INTO [dbo].[DayTypeConfig] ([DayOfWeek], [DayType])
VALUES 
  (4, 1)
GO

INSERT INTO [dbo].[DayTypeConfig] ([DayOfWeek], [DayType])
VALUES 
  (5, 2)
GO

INSERT INTO [dbo].[DayTypeConfig] ([DayOfWeek], [DayType])
VALUES 
  (6, 4)
GO

--
-- Data for table dbo.DisplayData  (LIMIT 0,500)
--

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (1, 17, 73)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (2, 18, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (3, 19, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (4, 20, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (5, 21, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (6, 22, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (7, 23, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (8, 24, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (9, 25, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (10, 26, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (11, 27, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (12, 28, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (13, 29, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (14, 30, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (15, 31, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (16, 32, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (17, 33, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (18, 34, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (19, 35, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (20, 36, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (21, 37, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (22, 38, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (23, 39, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (24, 40, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (25, 57, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (26, 17, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (27, 54, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (28, 62, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (29, 57, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (30, 17, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (31, 17, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (32, 17, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (33, 17, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (34, 73, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (35, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (36, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (37, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (38, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (39, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (40, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (41, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (42, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (43, 53, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (44, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (45, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (46, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (47, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (48, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (49, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (50, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (51, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (52, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (53, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (54, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (55, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (56, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (57, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (58, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (59, NULL, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (60, 43, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (61, 66, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (62, 60, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (63, 63, NULL)
GO

INSERT INTO [dbo].[DisplayData] ([DisplayId], [PhoneNumber_1], [PhoneNumber_2])
VALUES 
  (64, NULL, 61)
GO

--
-- Data for table dbo.Enum  (LIMIT 0,500)
--

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 1, N'BaoThucSang.wav', NULL, N'Báo thức sáng')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 2, N'ChuanBiLamViecSang.wav', NULL, N'Chuẩn bị làm việc sáng')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 3, N'LamViecSang.wav', NULL, N'Làm việc sáng')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 4, N'NghiTrua.wav', NULL, N'Nghỉ trưa')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 5, N'BaoThucTrua.wav', NULL, N'Báo thức trưa')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 6, N'ChuanBiLamViecChieu.wav', NULL, N'Chuẩn bị làm việc chiều')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 7, N'LamViecChieu.wav', NULL, N'Làm việc chiều')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 8, N'NghiChieu.wav', NULL, N'Nghỉ chiều')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 9, N'ChuanBiLamViecToi.wav', NULL, N'Chuẩn bị làm việc tối')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 10, N'LamViecToi.wav', NULL, N'Làm việc tối')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 11, N'NghiLamViecToi.wav', NULL, N'Nghỉ làm việc tối')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 12, N'DiemDanh.wav', NULL, N'Điểm danh')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'AlarmType', 13, N'Ngu.wav', NULL, N'Ngủ')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'CallType', 1, N'Out', NULL, N'Gọi đi')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'CallType', 2, N'In', NULL, N'Gọi đến')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'CallType', 3, N'Missed', NULL, N'Gọi nhỡ')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'DayOfWeek', 0, N'Sunday', NULL, N'Chủ nhật')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'DayOfWeek', 1, N'Monday', NULL, N'Thứ hai')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'DayOfWeek', 2, N'Tuesday', NULL, N'Thứ ba')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'DayOfWeek', 3, N'Wedesday', NULL, N'Thứ tư')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'DayOfWeek', 4, N'Thusday', NULL, N'Thứ năm')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'DayOfWeek', 5, N'Friday', NULL, N'Thứ sáu')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'DayOfWeek', 6, N'Saturday', NULL, N'Thứ bảy')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'DayType', 1, N'Working', NULL, N'Ngày làm việc')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'DayType', 2, N'BeforeOff', NULL, N'Ngày kế ngày nghỉ')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'DayType', 3, N'OffWithAlarm', NULL, N'Ngày nghỉ có điểm danh')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'DayType', 4, N'Off', NULL, N'Ngày nghỉ')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'DayType_1', 0, N'None', NULL, N'Không khai báo')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'PhoneNumberType', 1, N'Army', NULL, N'Quân sự')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'PhoneNumberType', 2, N'Civil', NULL, N'Dân sự')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'PhoneNumberType', 3, N'HotLine', NULL, N'Đường Dây Nóng')
GO

INSERT INTO [dbo].[Enum] ([Type], [Value], [Desc], [ListOrder], [Desc_VN])
VALUES 
  (N'PhoneNumberType', 4, N'Mobile', NULL, N'Di Động')
GO

--
-- Data for table dbo.Group  (LIMIT 0,500)
--

SET IDENTITY_INSERT [dbo].[Group] ON
GO

INSERT INTO [dbo].[Group] ([GroupId], [Name], [DeletedDate])
VALUES 
  (1, N'Nhóm 1', NULL)
GO

INSERT INTO [dbo].[Group] ([GroupId], [Name], [DeletedDate])
VALUES 
  (2, N'QQ', '20150831 15:35:04.990')
GO

INSERT INTO [dbo].[Group] ([GroupId], [Name], [DeletedDate])
VALUES 
  (3, N'ABCDE', '20150812 14:26:11.613')
GO

INSERT INTO [dbo].[Group] ([GroupId], [Name], [DeletedDate])
VALUES 
  (4, N'quyen', '20150812 14:26:08.593')
GO

INSERT INTO [dbo].[Group] ([GroupId], [Name], [DeletedDate])
VALUES 
  (5, N'nhom 2', '20150811 10:30:30.820')
GO

INSERT INTO [dbo].[Group] ([GroupId], [Name], [DeletedDate])
VALUES 
  (6, N'AAAAAAAAAAAAAAAAAAAA', '20150624 14:46:15.363')
GO

INSERT INTO [dbo].[Group] ([GroupId], [Name], [DeletedDate])
VALUES 
  (7, N'Test Group', '20150629 14:07:40.100')
GO

INSERT INTO [dbo].[Group] ([GroupId], [Name], [DeletedDate])
VALUES 
  (8, N'Test Group', '20150811 09:30:23.757')
GO

INSERT INTO [dbo].[Group] ([GroupId], [Name], [DeletedDate])
VALUES 
  (9, N'Test group 2', '20150707 09:06:50.820')
GO

INSERT INTO [dbo].[Group] ([GroupId], [Name], [DeletedDate])
VALUES 
  (10, N'test group 3', '20150629 14:21:03.927')
GO

INSERT INTO [dbo].[Group] ([GroupId], [Name], [DeletedDate])
VALUES 
  (11, N'Nhóm 2', NULL)
GO

SET IDENTITY_INSERT [dbo].[Group] OFF
GO

--
-- Data for table dbo.GroupUnit  (LIMIT 0,500)
--

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 17, 1, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 18, 2, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 19, 3, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 20, 4, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 21, 5, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 22, 6, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 23, 7, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 24, 8, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 25, 9, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 26, 10, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 27, 11, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 28, 12, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 29, 13, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 30, 14, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 31, 15, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 32, 16, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 33, 17, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 34, 18, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 35, 19, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 36, 20, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 37, 21, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 38, 22, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 39, 23, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 40, 24, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 41, 17, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 42, 27, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 43, 28, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 44, 29, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 45, 30, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 46, 32, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 47, 27, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 48, 24, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 49, 24, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 50, 24, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 51, 24, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 52, 38, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 53, 39, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 54, 40, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 55, 41, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (1, 56, 31, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (2, 17, 1, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (2, 18, 2, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (2, 19, 3, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (2, 20, 4, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (2, 23, 5, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (2, 24, 6, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (2, 25, 7, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (2, 26, 8, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (2, 27, 9, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (2, 33, 10, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 17, 1, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 18, 2, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 19, 3, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 20, 4, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 23, 5, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 24, 6, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 25, 7, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 26, 8, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 27, 9, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 43, 23, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 53, 24, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 57, 10, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 59, 11, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 60, 21, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 61, 22, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 62, 13, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 63, 14, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 64, 15, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 65, 16, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 66, 17, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 67, 18, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 68, 19, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 69, 20, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (3, 73, 12, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (4, 17, 1, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (4, 18, 2, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (4, 19, 3, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (4, 20, 4, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (4, 23, 5, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (4, 24, 6, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (4, 25, 7, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (5, 18, 1, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (5, 19, 7, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (5, 20, 2, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (5, 21, 3, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (5, 22, 4, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (5, 23, 5, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (5, 61, 6, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (6, 17, 3, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (6, 19, 1, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (6, 23, 1, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (6, 26, 2, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (6, 30, 4, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (6, 32, 5, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (6, 68, 4, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (6, 69, 5, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (6, 71, 6, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (6, 72, 7, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (7, 49, 1, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (7, 50, 2, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (7, 51, 3, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (7, 52, 4, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (7, 53, 5, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (7, 54, 6, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (8, 18, 1, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (8, 22, 4, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (8, 23, 3, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (8, 27, 2, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (8, 49, 3, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (8, 50, 2, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (8, 51, 3, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (8, 52, 4, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (8, 53, 1, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (8, 54, 2, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (9, 70, 1, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (9, 71, 2, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (9, 72, 3, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (10, 20, 1, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (10, 21, 2, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (10, 22, 3, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (10, 23, 4, 1)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 17, 1, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 18, 2, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 19, 3, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 20, 4, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 21, 5, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 22, 6, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 23, 7, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 24, 8, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 25, 9, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 47, 10, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 48, 11, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 49, 12, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 50, 13, 0)
GO

INSERT INTO [dbo].[GroupUnit] ([GroupId], [PhoneNumberId], [ListOrder], [IsDeleted])
VALUES 
  (11, 51, 14, 0)
GO

--
-- Data for table dbo.Panel  (LIMIT 0,500)
--

INSERT INTO [dbo].[Panel] ([PanelId], [POId], [CurrentMode], [CurrentGroupId])
VALUES 
  (1, 1, 1, 1)
GO

INSERT INTO [dbo].[Panel] ([PanelId], [POId], [CurrentMode], [CurrentGroupId])
VALUES 
  (2, 2, 1, 1)
GO

INSERT INTO [dbo].[Panel] ([PanelId], [POId], [CurrentMode], [CurrentGroupId])
VALUES 
  (3, 3, 1, 1)
GO

INSERT INTO [dbo].[Panel] ([PanelId], [POId], [CurrentMode], [CurrentGroupId])
VALUES 
  (4, 4, 1, 1)
GO

--
-- Data for table dbo.PhoneNumber  (LIMIT 0,500)
--

SET IDENTITY_INSERT [dbo].[PhoneNumber] ON
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (1, N'069', N'212', 1, 1, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (2, N'069', N'213', 1, 2, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (3, N'069', N'214', 1, 3, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (4, N'069', N'215', 1, 4, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (5, N'072', N'21', 1, 5, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (6, N'072', N'22', 1, 6, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (7, N'072', N'23', 1, 7, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (8, N'072', N'24', 0, 8, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (9, N'069', N'208', 1, 9, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (10, N'069', N'209', 1, 10, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (11, N'069', N'209', 1, 11, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (12, N'069', N'210', 1, 12, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (13, NULL, N'0', 0, 13, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (14, NULL, N'0', 0, 14, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (15, NULL, N'0', 0, 15, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (16, NULL, N'0', 0, 16, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (17, N'069', N'216', 0, NULL, N'SCH', N'1', N'SỞ CHỈ HUY')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (18, N'069', N'217', 0, NULL, N'P. THAM MƯU', N'2', N'PHÒNG THAM MƯU')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (19, N'069', N'218', 0, NULL, N'P. CHÍNH TRỊ', N'3', N'PHÒNG CHÍNH TRỊ')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (20, N'069', N'219', 0, NULL, N'P. HẬU CẦN', N'4', N'PHÒNG HẬU CẦN')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (21, N'069', N'220', 0, NULL, N'P. KỸ THUẬT', N'5', N'PHÒNG KỸ THUẬT')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (22, N'069', N'221', 0, NULL, N'eBB738', N'6', N'TRUNG ĐOÀN BB738')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (23, N'069', N'222', 0, NULL, N'H. TÂN HƯNG', N'7', N'BAN CHQS H. TÂN HƯNG')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (24, N'069', N'223', 0, NULL, N'H. VĨNH HƯNG', N'8', N'BAN CHQS H. VĨNH HƯNG')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (25, N'069', N'9', 0, NULL, N'TX. KIẾN TƯỜNG', NULL, N'BAN CHQS TX. KIẾN TƯỜNG')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (26, N'069', N'10', 0, NULL, N'H. MỘC HÓA', N'0', N'BAN CHQS H. MỘC HÓA')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (27, N'069', N'444', 0, NULL, N'H. THẠNH HÓA', N'', N'BAN CHQS H. THẠNH HÓA')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (28, N'', N'12', 0, NULL, N'H. ĐỨC HUỆ', N'0', N'BAN CHQS H. ĐỨC HUỆ')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (29, NULL, N'13', 0, NULL, N'dBB1', NULL, N'TIỂU ĐOÀN BB1')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (30, N'', N'14', 0, NULL, N'TRƯỜNG QS', NULL, N'TRƯỜNG QUÂN SỰ')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (31, N'08', N'15', 0, NULL, N'TP. TÂN AN', N'0', N'BAN CHQS TP. TÂN AN')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (32, N'08', N'16', 0, NULL, N'H. TÂN TRỤ', NULL, N'BAN CHQS H .TÂN TRỤ')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (33, NULL, N'17', 0, NULL, N'H. CHÂU THÀNH', NULL, N'BAN CHQS H. CHÂU THÀNH')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (34, NULL, N'18', 0, NULL, N'H. THỦ THỪA', N'', N'BAN CHQS H. THỦ THỪA')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (35, N'072', N'111', 0, NULL, N'H. BẾN LỨC', N'14', N'BAN CHQS H. BẾN LỨC')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (36, NULL, N'20', 0, NULL, N'H. ĐỨC HÒA', NULL, N'BAN CHQS H. ĐỨC HÒA')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (37, NULL, N'21', 0, NULL, N'H. TÂN THẠNH', NULL, N'BAN CHQS H. TÂN THẠNH')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (38, NULL, N'22', 0, NULL, N'H. CẦN ĐƯỚC', NULL, N'BAN CHQS H. CẦN ĐƯỚC')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (39, NULL, N'23', 0, NULL, N'H. CẦN GIUỘC', NULL, N'BAN CHQS H. CẦN GIUỘC')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (40, NULL, N'24', 0, NULL, N'ĐV24', NULL, N'ĐƠN VỊ 24')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (41, NULL, N'25', 0, NULL, N'ĐV25', NULL, N'ĐƠN VỊ 25')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (42, N'445', N'26', 0, NULL, N'ĐV26', N'', N'26')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (43, NULL, N'27', 0, NULL, N'ĐV27', NULL, N'27')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (44, NULL, N'28', 0, NULL, N'ĐV28', NULL, N'28')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (45, NULL, N'29', 0, NULL, N'ĐV29', NULL, N'29')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (46, NULL, N'30', 0, NULL, N'ĐV30', NULL, N'30')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (47, N'08', N'31', 0, NULL, N'ĐV31', NULL, N'31')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (48, N'', N'32', 0, NULL, N'ĐV32', N'', N'32')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (49, N'072', N'33', 0, NULL, N'ĐV33', N'6', N'33')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (50, N'072', N'34', 0, NULL, N'ĐV34', N'6', N'34')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (51, N'3', N'5', 0, NULL, N'ĐV35', N'6', N'35')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (52, NULL, N'36', 0, NULL, N'ĐV36', NULL, N'36')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (53, NULL, N'37', 0, NULL, N'ĐV37', NULL, N'37')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (54, NULL, N'38', 0, NULL, N'ĐV38', NULL, N'38')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (55, NULL, N'39', 0, NULL, N'ĐV39', NULL, N'39')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (56, NULL, N'40', 0, NULL, N'ĐV40', NULL, N'40')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (57, N'11', N'0000000', 0, NULL, N'ĐV41', N'41', N'Don vi 41')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (58, N'33', N'11', 0, NULL, N'mmmm', N'00', N'llllllllllllllll')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (59, N'069', N'665787', 0, NULL, N'SHDK', NULL, N'SHDK')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (60, N'069', N'665420', 0, NULL, N'CNM-VCNTT', NULL, N'VCNTT')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (61, NULL, N'228', 0, NULL, N'test 1', NULL, N'test 1')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (62, N'6565', N'65656', 0, NULL, N'them', NULL, N'them')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (63, N'sss', N'rrr', 0, NULL, N'fdfd', N'333', N'fdfd')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (64, N'gfgf', N'gfgfg', 0, NULL, N'fgfg', N'4545', N'fgfg')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (65, N'5454', N'gfgfg', 0, NULL, N'tgfg', N'4545', N'gfgfg')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (66, N'gfgfg', N'5454', 0, NULL, N'tgfg', N'4545', N'gfgfg')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (67, N'trt', N'gfgf', 0, NULL, N'tgfg', N'***', N'gfgfg')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (68, N'ggg', N'fff', 0, NULL, N'fdas', N'3444444444', N'fssss')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (69, N'54grg', N'fff', 0, NULL, N'fdas', N'444', N'fdas')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (70, N'hghgh', N'fff', 0, NULL, N'test 1', N'444', N'fdas')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (71, N'545', N'fdfd', 0, NULL, N'fdas', N'444', N'eeee')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (72, N'aqz', N'aqz', 0, NULL, N'aqz', N'3333', N'aqz')
GO

INSERT INTO [dbo].[PhoneNumber] ([PhoneNumberId], [AreaCode], [Number], [IsRestricted], [Address], [NameAbbr], [Password], [UnitName])
VALUES 
  (73, N'069', N'33333', 0, NULL, N'SCH', NULL, N'Sở Chỉ Huy')
GO

SET IDENTITY_INSERT [dbo].[PhoneNumber] OFF
GO

--
-- Data for table dbo.PO  (LIMIT 0,500)
--

INSERT INTO [dbo].[PO] ([Id], [Address])
VALUES 
  (1, 21)
GO

INSERT INTO [dbo].[PO] ([Id], [Address])
VALUES 
  (2, 22)
GO

INSERT INTO [dbo].[PO] ([Id], [Address])
VALUES 
  (3, 23)
GO

INSERT INTO [dbo].[PO] ([Id], [Address])
VALUES 
  (4, 24)
GO

--
-- Data for table dbo.RadioTime  (LIMIT 0,500)
--

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (1, 1, '20150713 05:00:00', '20150713 05:30:00', 1)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (1, 2, '20150713 05:00:00', '20150713 05:30:00', 1)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (1, 3, '20150812 10:00:00', '20150812 11:00:00', 0)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (1, 4, '20150725 08:00:00', '20150725 11:30:00', 1)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (2, 1, '20150713 08:00:00', '20150713 11:30:00', 1)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (2, 2, '20150724 10:35:00', '20150724 12:00:00', 1)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (2, 3, NULL, NULL, 0)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (2, 4, NULL, NULL, 0)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (3, 1, '20150728 14:00:00', '20150728 17:00:00', 1)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (3, 2, '20150724 14:00:00', '20150724 17:00:00', 1)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (3, 3, NULL, NULL, 0)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (3, 4, NULL, NULL, 0)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (4, 1, '20150728', '20150728 23:00:00', 0)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (4, 2, NULL, NULL, 0)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (4, 3, NULL, NULL, 0)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (4, 4, NULL, NULL, 0)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (5, 1, NULL, NULL, 0)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (5, 2, NULL, NULL, 0)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (5, 3, NULL, NULL, 0)
GO

INSERT INTO [dbo].[RadioTime] ([ListOrder], [DayType], [StartTime], [EndTime], [IsEnabled])
VALUES 
  (5, 4, NULL, NULL, 0)
GO

--
-- Data for table dbo.Unit  (LIMIT 0,500)
--

SET IDENTITY_INSERT [dbo].[Unit] ON
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (1, N'Đơn vị 1', N'ÐV1', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (2, N'Đơn vị 2', N'ÐV2', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (3, N'Đơn vị 3', N'ÐV3', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (4, N'Đơn vị 4', N'ÐV4', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (5, N'Đơn vị 5', N'ÐV5', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (6, N'Đơn vị 6', N'ÐV6', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (7, N'Đơn vị 7', N'ÐV7', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (8, N'Đơn vị 8', N'ÐV8', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (9, N'Đơn vị 9', N'ÐV1', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (10, N'Đơn vị 10', N'ÐV10', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (11, N'Đơn vị 11', N'ĐV11', N'11')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (12, N'Đơn vị 12', N'ĐV12', N'12')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (13, N'Đơn vị 13', N'ĐV13', N'13')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (14, N'Đơn vị 14', N'ĐV14', N'14')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (15, N'Đơn vị 15', N'ĐV15', N'15')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (16, N'Đơn vị 16', N'ĐV16', N'16')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (17, N'Đơn vị 17', N'ĐV17', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (18, N'Đơn vị 18', N'ĐV18', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (19, N'Đơn vị 19', N'ĐV19', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (20, N'Đơn vị 20', N'ĐV20', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (21, N'Đơn vị 21', N'ĐV21', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (22, N'Đơn vị 22', N'ĐV22', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (23, N'Đơn vị 23', N'ĐV23', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (24, N'Đơn vị 34', N'ĐV24', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (25, N'Đơn vị 25', N'ĐV25', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (26, N'Đơn vị 26', N'ĐV26', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (27, N'Đơn vị 27', N'ĐV27', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (28, N'Đơn vị 28', N'ĐV28', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (29, N'Đơn vị 29', N'ĐV29', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (30, N'Đơn vị 30', N'ĐV30', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (31, N'Đơn vị 31', N'ĐV31', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (32, N'Đơn vị 32', N'ĐV32', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (33, N'Đơn vị 33', N'ĐV33', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (34, N'Đơn vị 34', N'ĐV34', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (35, N'Đơn vị 35', N'ĐV35', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (36, N'Đơn vị 36', N'ĐV36', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (37, N'Đơn vị 37', N'ĐV37', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (38, N'Đơn vị 38', N'ĐV38', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (39, N'Đơn vị 39', N'ĐV39', N'123')
GO

INSERT INTO [dbo].[Unit] ([UnitId], [Name], [NameAbbr], [Password])
VALUES 
  (40, N'Đơn vị 40', N'ĐV40', N'123')
GO

SET IDENTITY_INSERT [dbo].[Unit] OFF
GO

--
-- Definition for indices : 
--

ALTER TABLE [dbo].[Alarm]
ADD CONSTRAINT [PK_Alarm] 
PRIMARY KEY CLUSTERED ([AlarmId])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[AlarmLog]
ADD PRIMARY KEY CLUSTERED ([Id])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[CallLog]
ADD CONSTRAINT [CallLog_pk] 
PRIMARY KEY CLUSTERED ([CallLogId])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[CallLogDetail]
ADD CONSTRAINT [CallLogDetail_pk] 
PRIMARY KEY CLUSTERED ([CallLogDetailId])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[Channel]
ADD CONSTRAINT [PK_Channel] 
PRIMARY KEY CLUSTERED ([ChannelId])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[DayTypeConfig]
ADD CONSTRAINT [PK_DayTypeConfig] 
PRIMARY KEY CLUSTERED ([DayOfWeek])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[DbLog]
ADD CONSTRAINT [PK_DbLog] 
PRIMARY KEY CLUSTERED ([Id])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[DisplayData]
ADD CONSTRAINT [PK_DisplayData] 
PRIMARY KEY CLUSTERED ([DisplayId])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[Enum]
ADD PRIMARY KEY CLUSTERED ([Type], [Value])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[GlobalSetting]
ADD PRIMARY KEY CLUSTERED ([Parameter])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[Group]
ADD CONSTRAINT [PK_Group] 
PRIMARY KEY CLUSTERED ([GroupId])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[GroupUnit]
ADD CONSTRAINT [GroupUnit_pk] 
PRIMARY KEY CLUSTERED ([GroupId], [PhoneNumberId])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[GroupUnitTask]
ADD CONSTRAINT [GroupUnitTask_pk] 
PRIMARY KEY CLUSTERED ([Id])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[HotLine]
ADD CONSTRAINT [PK_HotLine] 
PRIMARY KEY CLUSTERED ([HotLineId])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[Panel]
ADD CONSTRAINT [PK_Panel] 
PRIMARY KEY CLUSTERED ([PanelId])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[PhoneNumber]
ADD CONSTRAINT [PK_PhoneNumber] 
PRIMARY KEY CLUSTERED ([PhoneNumberId])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[PO]
ADD CONSTRAINT [PK_PO] 
PRIMARY KEY CLUSTERED ([Id], [Address])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[RadioTime]
ADD PRIMARY KEY CLUSTERED ([ListOrder], [DayType])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[Unit]
ADD CONSTRAINT [PK_Unit] 
PRIMARY KEY CLUSTERED ([UnitId])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO



ALTER TABLE [dbo].[User]
ADD CONSTRAINT [PK_User] 
PRIMARY KEY CLUSTERED ([Id])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO

-- 6/9/2015
ALTER TABLE PhoneNumber ADD DeletedDate DATETIME NULL
GO

-- 15/9/2015
ALTER TABLE dbo.GroupUnitTask ADD TaskType TINYINT NOT NULL DEFAULT(1)
GO

ALTER TABLE dbo.Channel ADD CCPKEnabled BIT NOT NULL DEFAULT(0)
GO

ALTER View [dbo].[ViewResult] AS
Select DisplayId as Id, sscd.PhoneNumberId, sscd.CreatedDate, 
sscd.UnitName, sscd.Task, sscd.Level, sscd.Result, sscd.Duration, sscd.TaskType
From DisplayData dd
INNER JOIN (select pn.PhoneNumberId, pn.UnitName, gut.CreatedDate, gut.Task, gut.Level, gut.Result, gut.Duration, gut.TaskType
	From GroupUnitTask gut 
	INNER JOIN PhoneNumber pn ON gut.PhoneNumberId = pn.PhoneNumberId
	where gut.Id IN (select top 1 Id from GroupUnitTask where PhoneNumberId = gut.PhoneNumberId AND TaskType = gut.TaskType order by Id DESC)) 
	sscd ON dd.PhoneNumber_1 = sscd.PhoneNumberId
GO

---- 26/9/2015
ALTER TABLE dbo.PhoneNumber ADD TSLAreaCode VARCHAR(5) NULL
ALTER TABLE dbo.PhoneNumber ADD TSLNumber VARCHAR(20) NULL
GO

-- 27/09/2015
-- night
CREATE TABLE [dbo].[TslStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [tinyint] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[DeletedDate] [datetime] NULL,
	[PhoneNumberId] [int] NOT NULL,
 CONSTRAINT [PK_TslStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- 31/10/2015
CREATE TABLE SubResult
(
	[Id] INT IDENTITY(1,1),
	[DisplayId] INT NOT NULL,
	[ParentId] INT NOT NULL,
	[PhoneNumberId] INT NOT NULL,
    [CreatedDate] DATE NOT NULL,
    [UnitName] NVARCHAR(50) NOT NULL,
    [Task] TINYINT NOT NULL,
    [Level] TINYINT NOT NULL,
    [Result] TINYINT NOT NULL,    
    [TaskType] TINYINT NOT NULL,
	[TimeReceive] datetime,
	[IntervalReceive] bigint,	
	[TimeChange] datetime,
	[IntervalChange] bigint
	CONSTRAINT PK_SubResult PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

CREATE PROCEDURE GetSubResult @phoneNumberId INT, @taskType TINYINT
AS
SELECT [Id]
      ,[DisplayId]
      ,[ParentId]
      ,[PhoneNumberId]
      ,[CreatedDate]
      ,[UnitName]
      ,[Task]
      ,[Level]
      ,[Result]
      ,[TaskType]
      ,[TimeReceive]
      ,[IntervalReceive]
      ,[TimeChange]
      ,[IntervalChange]
FROM SubResult sub
WHERE TaskType = @taskType AND ParentId = @phoneNumberId
AND Id IN (SELECT TOP 1 Id FROM SubResult WHERE PhoneNumberId = sub.PhoneNumberId ORDER BY Id DESC)
GO

-- 15/1/2016
ALTER PROCEDURE [dbo].[GetSubResult] @phoneNumberId INT, @taskType TINYINT
AS
SELECT [Id]
      ,[DisplayId]
      ,[ParentId]
      ,[PhoneNumberId]
      ,[CreatedDate]
      ,[UnitName]
      ,[Task]
      ,[Level]
      ,[Result]
      ,[TaskType]
      ,[TimeReceive]
      ,[IntervalReceive]
      ,[TimeChange]
      ,[IntervalChange]
FROM SubResult sub
WHERE TaskType = @taskType AND ParentId = @phoneNumberId
AND Id = (SELECT TOP 1 Id FROM SubResult WHERE PhoneNumberId = sub.PhoneNumberId AND ParentId = @phoneNumberId ORDER BY Id DESC)
GO