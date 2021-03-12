Truncate Table PhoneNumber
GO

Declare @count int;
Set @count = 1;
While(@count <= 16)
Begin
Insert Into PhoneNumber([Number],[IsRestricted]) Values(@count, 0)
set @count = @count + 1
End

Set @count = 17;
Declare @unitId int
set @unitId = 1;
While(@unitId <= 10)
Begin
Insert Into PhoneNumber([Number],[IsRestricted], [UnitId]) Values(@unitId, 0, @unitId)
set @unitId = @unitId + 1
End
GO

Truncate Table Channel
GO

INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(1, 1, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(2, 2, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(3, 3, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(4, 4, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(5, 5, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(6, 6, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(7, 7, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(8, 8, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(9, 9, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(10, 10, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(11, 11, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(12, 12, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(13, 13, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(14, 14, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(15, 15, 0, 1, 1)
INSERT INTO [Channel]([ChannelId], [PhoneNumberId], [AutoRecording], [AlertEnabled], [MultiDestEnabled]) VALUES(16, 16, 0, 1, 1)
GO

Truncate Table Panel
GO

INSERT INTO [Panel]([PanelId], [POId], [CurrentMode], [CurrentGroupId]) VALUES(1, 1, 1, 1)
INSERT INTO [Panel]([PanelId], [POId], [CurrentMode], [CurrentGroupId]) VALUES(2, 2, 1, 1)
INSERT INTO [Panel]([PanelId], [POId], [CurrentMode], [CurrentGroupId]) VALUES(3, 3, 1, 1)
INSERT INTO [Panel]([PanelId], [POId], [CurrentMode], [CurrentGroupId]) VALUES(4, 4, 1, 1)
GO

Truncate Table Unit
GO

SET IDENTITY_INSERT Unit ON
GO

INSERT INTO [Unit](UnitId, [Name], [NameAbbr], [Password]) VALUES(1, N'Đơn vị 1', 'ĐV1', '123')
INSERT INTO [Unit](UnitId, [Name], [NameAbbr], [Password]) VALUES(2, N'Đơn vị 2', 'ĐV2', '123')
INSERT INTO [Unit](UnitId, [Name], [NameAbbr], [Password]) VALUES(3, N'Đơn vị 3', 'ĐV3', '123')
INSERT INTO [Unit](UnitId, [Name], [NameAbbr], [Password]) VALUES(4, N'Đơn vị 4', 'ĐV4', '123')
INSERT INTO [Unit](UnitId, [Name], [NameAbbr], [Password]) VALUES(5, N'Đơn vị 5', 'ĐV5', '123')
INSERT INTO [Unit](UnitId, [Name], [NameAbbr], [Password]) VALUES(6, N'Đơn vị 6', 'ĐV6', '123')
INSERT INTO [Unit](UnitId, [Name], [NameAbbr], [Password]) VALUES(7, N'Đơn vị 7', 'ĐV7', '123')
INSERT INTO [Unit](UnitId, [Name], [NameAbbr], [Password]) VALUES(8, N'Đơn vị 8', 'ĐV8', '123')
INSERT INTO [Unit](UnitId, [Name], [NameAbbr], [Password]) VALUES(9, N'Đơn vị 9', 'ĐV1', '123')
INSERT INTO [Unit](UnitId, [Name], [NameAbbr], [Password]) VALUES(10, N'Đơn vị 10', 'ĐV10', '123')
GO

SET IDENTITY_INSERT Unit OFF
GO

Truncate Table DayTypeConfig
GO

Insert Into DayTypeConfig([DayOfWeek], DayType) Values(0, 4) -- CN
Insert Into DayTypeConfig([DayOfWeek], DayType) Values(1, 1) -- T2
Insert Into DayTypeConfig([DayOfWeek], DayType) Values(2, 1)
Insert Into DayTypeConfig([DayOfWeek], DayType) Values(3, 1)
Insert Into DayTypeConfig([DayOfWeek], DayType) Values(4, 1)
Insert Into DayTypeConfig([DayOfWeek], DayType) Values(5, 2)
Insert Into DayTypeConfig([DayOfWeek], DayType) Values(6, 4)
GO

--17/1/2012
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (1, N'Báo thức sáng', '1/1/2014 6:00', 3, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (1, N'Chuẩn bị làm việc sáng', '1/1/2014 6:30', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (1, N'Làm việc sáng', '1/1/2014 7:30', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (1, N'Nghỉ trưa', '1/1/2014 11:00', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (1, N'Báo thức trưa', '1/1/2014 13:15', 3, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (1, N'Chuẩn bị làm việc chiều', '1/1/2014 13:20', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (1, N'Làm việc chiều', '1/1/2014 13:30', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (1, N'Nghỉ chiều', '1/1/2014 17:00', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (1, N'Chuẩn bị làm việc tối', '1/1/2014 19:00', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (1, N'Nghỉ làm việc tối', '1/1/2014 21:00', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (1, N'Điểm danh', '1/1/2014 21:30', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (1, N'Ngủ', '1/1/2014 21:40', 0, '')

Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (2, N'Báo thức sáng', '1/1/2014 6:00', 3, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (2, N'Chuẩn bị làm việc sáng', '1/1/2014 6:30', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (2, N'Làm việc sáng', '1/1/2014 7:30', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (2, N'Nghỉ trưa', '1/1/2014 11:00', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (2, N'Báo thức trưa', '1/1/2014 13:15', 3, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (2, N'Chuẩn bị làm việc chiều', '1/1/2014 13:20', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (2, N'Làm việc chiều', '1/1/2014 13:30', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (2, N'Nghỉ chiều', '1/1/2014 17:00', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (2, N'Chuẩn bị làm việc tối', '1/1/2014 19:00', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (2, N'Nghỉ làm việc tối', '1/1/2014 21:00', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (2, N'Điểm danh', '1/1/2014 21:30', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (2, N'Ngủ', '1/1/2014 21:40', 0, '')

Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (3, N'Báo thức sáng', '1/1/2014 6:00', 3, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (3, N'Chuẩn bị làm việc sáng', '1/1/2014 6:30', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (3, N'Làm việc sáng', '1/1/2014 7:30', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (3, N'Nghỉ trưa', '1/1/2014 11:00', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (3, N'Báo thức trưa', '1/1/2014 13:15', 3, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (3, N'Chuẩn bị làm việc chiều', '1/1/2014 13:20', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (3, N'Làm việc chiều', '1/1/2014 13:30', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (3, N'Nghỉ chiều', '1/1/2014 17:00', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (3, N'Chuẩn bị làm việc tối', '1/1/2014 19:00', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (3, N'Nghỉ làm việc tối', '1/1/2014 21:00', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (3, N'Điểm danh', '1/1/2014 21:30', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (3, N'Ngủ', '1/1/2014 21:40', 0, '')

Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (4, N'Báo thức sáng', '1/1/2014 6:00', 3, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (4, N'Chuẩn bị làm việc sáng', '1/1/2014 6:30', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (4, N'Làm việc sáng', '1/1/2014 7:30', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (4, N'Nghỉ trưa', '1/1/2014 11:00', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (4, N'Báo thức trưa', '1/1/2014 13:15', 3, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (4, N'Chuẩn bị làm việc chiều', '1/1/2014 13:20', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (4, N'Làm việc chiều', '1/1/2014 13:30', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (4, N'Nghỉ chiều', '1/1/2014 17:00', 1, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (4, N'Chuẩn bị làm việc tối', '1/1/2014 19:00', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (4, N'Nghỉ làm việc tối', '1/1/2014 21:00', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (4, N'Điểm danh', '1/1/2014 21:30', 0, '')
Insert Into Alarm(DayType, Name, [Time], TimesOfPlaying, [FileName]) VALUES (4, N'Ngủ', '1/1/2014 21:40', 0, '')

-- 05/02/2014
-- Get the latest backdup database for testing

-- 13/02/2014
CREATE TABLE [dbo].[PO](
	[Id] [tinyint] NOT NULL,
	[Address] [tinyint] NOT NULL,
 CONSTRAINT [PK_PO] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[Address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO PO(Id, [Address]) Values(1, 21)
INSERT INTO PO(Id, [Address]) Values(2, 22)
INSERT INTO PO(Id, [Address]) Values(3, 23)
INSERT INTO PO(Id, [Address]) Values(4, 24)
GO

Alter Table PhoneNumber Add [Address] tinyint
GO

Update PhoneNumber Set [Address] = PhoneNumberId where unitid is null
GO

-- 19/02
Alter Table PhoneNumber Add NameAbbr nvarchar(10)
GO

Alter Table PhoneNumber Add Password varchar(10)
GO

Alter Table PhoneNumber Add UnitName nvarchar(50)
GO

ALTER TABLE [dbo].[GroupUnit] DROP CONSTRAINT [PK_GroupUnit]
GO

ALTER TABLE [dbo].[GroupUnit] DROP COLUMN [UnitId]
GO

ALTER TABLE [dbo].[PhoneNumber] DROP COLUMN [UnitId]
GO

EXEC sp_rename '[dbo].[HotLine].[UnitId]', 'PhoneNumberId', 'COLUMN'
GO

ALTER TABLE [dbo].[GroupUnit]
ADD CONSTRAINT [GroupUnit_pk] 
PRIMARY KEY CLUSTERED ([GroupId], [PhoneNumberId])
WITH (
  PAD_INDEX = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
GO

-- 19/05/2014
CREATE TABLE [dbo].[GroupUnitTask] (
    [Id]            INT      IDENTITY (1, 1) NOT NULL,
    [GroupId]       INT      NOT NULL,
    [PhoneNumberId] INT      NOT NULL,
    [CreatedDate]   DATETIME NOT NULL,
    [Task]          TINYINT  NOT NULL,
    [Level]         TINYINT  NOT NULL,
    [Result]        TINYINT  NOT NULL
);
GO


--09/06/2014
Alter Table dbo.GroupUnit Add IsDeleted bit not null default(0)

-- 19/06/2014
Drop Table CallLog
GO

CREATE TABLE [dbo].[CallLog] (
  [CallLogId] uniqueidentifier NOT NULL,
  [POId] int NOT NULL,
  [StartTime] datetime NOT NULL,
  [EndTime] datetime NULL,
  [CallType] tinyint NOT NULL,
  [Record] varchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  CONSTRAINT [CallLog_pk] PRIMARY KEY CLUSTERED ([CallLogId])
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[CallLogDetail] (
  [CallLogDetailId] uniqueidentifier NOT NULL,
  [CallLogId] uniqueidentifier NOT NULL,
  [ChannelId] int NULL,
  [UnitId] int NULL,
  [PhoneNumber] varchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  [StartTime] datetime NOT NULL,
  [EndTime] datetime NULL,
  CONSTRAINT [CallLogDetail_pk] PRIMARY KEY CLUSTERED ([CallLogDetailId])
)
ON [PRIMARY]
GO

-- 25/6
Alter Table CallLogDetail Add UnitName nvarchar(20)
GO

-- 01/10/2014
Alter Table CallLogDetail Add Record varchar(255)
GO

--Update CallLogDetail Set Record = c.Record
--From CallLog c inner join CallogDetail d On c.CallogId = d.CallLogId
--GO

Alter Table CallLog Drop Column Record
GO

-- 12/11/2014
Alter Table CallLogDetail Alter Column PhoneNumber varchar(20) NULL
GO

-- 8/12/2014
Alter Table PhoneNumber Add TypeCde tinyint NOT NULL default(1)
GO

-- night, continue
ALTER TABLE [dbo].[GroupUnitTask]
ADD [Duration] bigint NULL
GO

Create View ViewResult AS
select gut.Id, pn.PhoneNumberId, pn.UnitName, gut.CreatedDate, gut.Task, gut.Level, gut.Result, gut.Duration from GroupUnitTask gut 
inner join PhoneNumber pn on gut.PhoneNumberId = pn.PhoneNumberId
where gut.Id IN (select top 1 Id from GroupUnitTask where PhoneNumberId = gut.PhoneNumberId order by Id DESC)
GO

ALTER TABLE [dbo].[GroupUnitTask]
ADD CONSTRAINT [GroupUnitTask_pk] PRIMARY KEY ([Id])
GO

-- 11/12/2014
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 1, 'BaoThucSang.wav', N'Báo thức sáng')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 2, 'ChuanBiLamViecSang.wav', N'Chuẩn bị làm việc sáng')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 3, 'LamViecSang.wav', N'Làm việc sáng')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 4, 'NghiTrua.wav', N'Nghỉ trưa')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 5, 'BaoThucTrua.wav', N'Báo thức trưa')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 6, 'ChuanBiLamViecChieu.wav', N'Chuẩn bị làm việc chiều')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 7, 'LamViecChieu.wav', N'Làm việc chiều')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 8, 'NghiChieu.wav', N'Nghỉ chiều')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 9, 'ChuanBiLamViecToi.wav', N'Chuẩn bị làm việc tối')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 10, 'NghiLamViecToi.wav', N'Nghỉ làm việc tối')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 11, 'DiemDanh.wav', N'Điểm danh')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 12, 'Ngu.wav', N'Ngủ')
GO

Truncate Table Alarm
GO

Alter Table Alarm Add AlarmType tinyint NOT NULL
GO

Alter Table Alarm Drop Column Name
GO

Alter Table Alarm Drop Column FileName
GO

Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 1, '1/1/2014 6:00', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 2, '1/1/2014 6:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 3, '1/1/2014 7:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 4, '1/1/2014 11:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 5, '1/1/2014 13:15', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 6, '1/1/2014 13:20', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 7, '1/1/2014 13:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 8, '1/1/2014 17:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 9, '1/1/2014 19:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 10, '1/1/2014 21:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 11, '1/1/2014 21:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 12, '1/1/2014 21:40', 0)

Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 1, '1/1/2014 6:00', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 2, '1/1/2014 6:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 3, '1/1/2014 7:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 4, '1/1/2014 11:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 5, '1/1/2014 13:15', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 6, '1/1/2014 13:20', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 7, '1/1/2014 13:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 8, '1/1/2014 17:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 9, '1/1/2014 19:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 10, '1/1/2014 21:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 11, '1/1/2014 21:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 12, '1/1/2014 21:40', 0)

Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 1, '1/1/2014 6:00', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 2, '1/1/2014 6:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 3, '1/1/2014 7:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 4, '1/1/2014 11:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 5, '1/1/2014 13:15', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 6, '1/1/2014 13:20', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 7, '1/1/2014 13:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 8, '1/1/2014 17:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 9, '1/1/2014 19:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 10, '1/1/2014 21:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 11, '1/1/2014 21:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 12, '1/1/2014 21:40', 0)

Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 1, '1/1/2014 6:00', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 2, '1/1/2014 6:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 3, '1/1/2014 7:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 4, '1/1/2014 11:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 5, '1/1/2014 13:15', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 6, '1/1/2014 13:20', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 7, '1/1/2014 13:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 8, '1/1/2014 17:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 9, '1/1/2014 19:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 10, '1/1/2014 21:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 11, '1/1/2014 21:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 12, '1/1/2014 21:40', 0)
GO

-- 16/12/2014
CREATE TABLE [dbo].[AlarmLog] (
    [Id]        INT      IDENTITY (1, 1) NOT NULL,
    [AlarmType] TINYINT  NOT NULL,
    [AlarmTime] DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- 30/12/2014
Alter Table PhoneNumber Drop Constraint DF__PhoneNumb__TypeC__1DE57479
GO

Alter Table PhoneNumber Drop Column TypeCde
GO

-- night, continue
Alter Table Channel Add HotUnitId int NULL
GO

-- 20/01/2015
Alter Table CallLog Add DeletedDate datetime null
GO
Alter Table CallLog Add DeletedBy int null
GO

CREATE TABLE [dbo].[DbLog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[PanelId] [tinyint] NOT NULL,
	[UserId] [int] NULL,
	[Info] [nvarchar](max) NULL,
 CONSTRAINT [PK_DbLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 24/03/2015
IF OBJECT_ID (N'dbo.DisplayData', N'U') IS NOT NULL
DROP TABLE dbo.DisplayData;

CREATE TABLE [dbo].[DisplayData](
	[DisplayId] [int] NOT NULL,
	[PhoneNumber_1] [int] NULL,
	[PhoneNumber_2] [int] NULL,
 CONSTRAINT [PK_DisplayData] PRIMARY KEY CLUSTERED 
(
	[DisplayId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Add data
Declare @index int = 1
While(@index <= 64)
Begin
	Insert Into DisplayData(DisplayId) Values(@index)	
	Set @index = @index + 1
End
GO

ALTER View ViewResult AS
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

-- 24/6/2015
ALTER TABLE [Group] ADD DeletedDate DATETIME NULL
GO

-- 29/6/2015
ALTER TABLE dbo.PhoneNumber ALTER COLUMN NameAbbr NVARCHAR(20) NULL
GO

-- 7/7/2015
DELETE Enum WHERE [Type] = 'AlarmType'
GO

Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 1, 'BaoThucSang.wav', N'Báo thức sáng')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 2, 'ChuanBiLamViecSang.wav', N'Chuẩn bị làm việc sáng')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 3, 'LamViecSang.wav', N'Làm việc sáng')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 4, 'NghiTrua.wav', N'Nghỉ trưa')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 5, 'BaoThucTrua.wav', N'Báo thức trưa')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 6, 'ChuanBiLamViecChieu.wav', N'Chuẩn bị làm việc chiều')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 7, 'LamViecChieu.wav', N'Làm việc chiều')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 8, 'NghiChieu.wav', N'Nghỉ chiều')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 9, 'ChuanBiLamViecToi.wav', N'Chuẩn bị làm việc tối')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 10, 'LamViecToi.wav', N'Làm việc tối')
INSERT Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 11, 'NghiLamViecToi.wav', N'Nghỉ làm việc tối')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 12, 'DiemDanh.wav', N'Điểm danh')
Insert Into Enum([Type], Value, [Desc], Desc_VN) Values('AlarmType', 13, 'Ngu.wav', N'Ngủ')
GO

TRUNCATE TABLE Alarm
GO

Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 1, '1/1/2014 6:00', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 2, '1/1/2014 6:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 3, '1/1/2014 7:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 4, '1/1/2014 11:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 5, '1/1/2014 13:15', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 6, '1/1/2014 13:20', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 7, '1/1/2014 13:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 8, '1/1/2014 17:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 9, '1/1/2014 19:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 10, '1/1/2014 19:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 11, '1/1/2014 21:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 12, '1/1/2014 21:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (1, 13, '1/1/2014 21:40', 0)

Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 1, '1/1/2014 6:00', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 2, '1/1/2014 6:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 3, '1/1/2014 7:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 4, '1/1/2014 11:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 5, '1/1/2014 13:15', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 6, '1/1/2014 13:20', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 7, '1/1/2014 13:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 8, '1/1/2014 17:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 9, '1/1/2014 19:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 10, '1/1/2014 19:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 11, '1/1/2014 21:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 12, '1/1/2014 21:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (2, 13, '1/1/2014 21:40', 0)

Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 1, '1/1/2014 6:00', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 2, '1/1/2014 6:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 3, '1/1/2014 7:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 4, '1/1/2014 11:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 5, '1/1/2014 13:15', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 6, '1/1/2014 13:20', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 7, '1/1/2014 13:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 8, '1/1/2014 17:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 9, '1/1/2014 19:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 10, '1/1/2014 19:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 11, '1/1/2014 21:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 12, '1/1/2014 21:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (3, 13, '1/1/2014 21:40', 0)

Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 1, '1/1/2014 6:00', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 2, '1/1/2014 6:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 3, '1/1/2014 7:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 4, '1/1/2014 11:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 5, '1/1/2014 13:15', 3)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 6, '1/1/2014 13:20', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 7, '1/1/2014 13:30', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 8, '1/1/2014 17:00', 1)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 9, '1/1/2014 19:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 10, '1/1/2014 19:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 11, '1/1/2014 21:00', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 12, '1/1/2014 21:30', 0)
Insert Into Alarm(DayType, AlarmType, [Time], TimesOfPlaying) VALUES (4, 13, '1/1/2014 21:40', 0)
GO

UPDATE dbo.Enum SET Type = 'DayType_1' WHERE [Type] = 'DayType' AND Value = 0
GO

-- 10/7/2015
CREATE TABLE [dbo].[RadioTime] (
  [ListOrder] tinyint NOT NULL,
  [DayType] tinyint NOT NULL,
  [StartTime] datetime NULL,
  [EndTime] datetime NULL,
  PRIMARY KEY CLUSTERED ([ListOrder], [DayType])
)
GO

INSERT INTO RadioTime(DayType, ListOrder) VALUES(1, 1)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(1, 2)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(1, 3)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(1, 4)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(1, 5)
GO

INSERT INTO RadioTime(DayType, ListOrder) VALUES(2, 1)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(2, 2)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(2, 3)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(2, 4)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(2, 5)
GO

INSERT INTO RadioTime(DayType, ListOrder) VALUES(3, 1)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(3, 2)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(3, 3)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(3, 4)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(3, 5)
GO

INSERT INTO RadioTime(DayType, ListOrder) VALUES(4, 1)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(4, 2)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(4, 3)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(4, 4)
INSERT INTO RadioTime(DayType, ListOrder) VALUES(4, 5)
GO

--13/7/2015
ALTER TABLE dbo.RadioTime ADD IsEnabled BIT NOT NULL DEFAULT(0)
GO

ALTER TABLE dbo.Alarm ADD IsEnabled BIT NOT NULL DEFAULT(0)
GO

-- 29/7/2015
CREATE TABLE [dbo].[GlobalSetting] (
  [Parameter] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  [Value] ntext COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  PRIMARY KEY CLUSTERED ([Parameter])
)
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
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
--ALTER TABLE dbo.Channel ADD TSLEnabled BIT NOT NULL DEFAULT(0)
--GO

ALTER TABLE dbo.PhoneNumber ADD TSLAreaCode VARCHAR(5) NULL
ALTER TABLE dbo.PhoneNumber ADD TSLNumber VARCHAR(20) NULL
GO

-- 27/09/2015
--DECLARE @phoneNumberId INT;
--INSERT INTO dbo.PhoneNumber
--        ( AreaCode ,
--          Number ,
--          IsRestricted ,
--          Address          
--        )
--VALUES  ( '' , -- AreaCode - varchar(5)
--          '123' , -- Number - varchar(20)
--          1 , -- IsRestricted - bit
--          17 -- Address - tinyint
--        )
--SELECT @phoneNumberId = @@IDENTITY

--INSERT INTO dbo.Channel
--        ( ChannelId ,
--          PhoneNumberId ,
--          AutoRecording ,
--          AlertEnabled ,
--          MultiDestEnabled ,
--          HotUnitId ,
--          CCPKEnabled ,
--          TSLEnabled
--        )
--VALUES  ( 17 , -- ChannelId - int
--          @phoneNumberId , -- PhoneNumberId - int
--          1 , -- AutoRecording - bit
--          0 , -- AlertEnabled - bit
--          0 , -- MultiDestEnabled - bit
--          0 , -- HotUnitId - int
--          0 , -- CCPKEnabled - bit
--          1  -- TSLEnabled - bit
--        )
--GO

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


-- 22/10/2015
ALTER TABLE dbo.Channel DROP CONSTRAINT DF__Channel__TSLEnab__5AEE82B9
GO

ALTER TABLE dbo.Channel DROP COLUMN TSLEnabled
GO

-- 31/10/2015
CREATE TABLE SubResult
(
	[Id] INT IDENTITY(1,1),
	[DisplayId] INT NOT NULL,
	[ParentId] INT NOT NULL,
	[PhoneNumberId] INT NOT NULL,
    [CreatedDate] Datetime NOT NULL,
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
