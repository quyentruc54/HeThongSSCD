Truncate Table Enum
GO

-- Loại cuộc gọi
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('CallType', 1, 'Out', N'Gọi đi')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('CallType', 2, 'In', N'Gọi đến')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('CallType', 3, 'Missed', N'Gọi nhỡ')
GO

-- Ngày trong tuần
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('DayOfWeek', 0, 'Sunday', N'Chủ nhật')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('DayOfWeek', 1, 'Monday', N'Thứ hai')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('DayOfWeek', 2, 'Tuesday', N'Thứ ba')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('DayOfWeek', 3, 'Wedesday', N'Thứ tư')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('DayOfWeek', 4, 'Thusday', N'Thứ năm')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('DayOfWeek', 5, 'Friday', N'Thứ sáu')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('DayOfWeek', 6, 'Saturday', N'Thứ bảy')
GO

-- Loại ngày
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('DayType', 0, 'None', N'Không khai báo')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('DayType', 1, 'Working', N'Ngày làm việc')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('DayType', 2, 'BeforeOff', N'Ngày kế ngày nghỉ')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('DayType', 3, 'OffWithAlarm', N'Ngày nghỉ có điểm danh')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('DayType', 4, 'Off', N'Ngày nghỉ')
GO

-- Loại số điện thoại
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('PhoneNumberType', 1, 'Army', N'Quân sự')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('PhoneNumberType', 2, 'Civil', N'Dân sự')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('PhoneNumberType', 3, 'HotLine', N'Đường Dây Nóng')
INSERT INTO [dbo].[Enum]([Type],[Value],[Desc],[Desc_VN]) VALUES('PhoneNumberType', 4, 'Mobile', N'Di Động')

