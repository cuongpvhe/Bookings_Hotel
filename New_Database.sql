USE [master]
GO
/****** Object:  Database [HotelBookingSystem]    Script Date: 10/30/2024 10:07:14 PM ******/
CREATE DATABASE [HotelBookingSystem]
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [HotelBookingSystem].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [HotelBookingSystem] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET ARITHABORT OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [HotelBookingSystem] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [HotelBookingSystem] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET  ENABLE_BROKER 
GO
ALTER DATABASE [HotelBookingSystem] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [HotelBookingSystem] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET RECOVERY FULL 
GO
ALTER DATABASE [HotelBookingSystem] SET  MULTI_USER 
GO
ALTER DATABASE [HotelBookingSystem] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [HotelBookingSystem] SET DB_CHAINING OFF 
GO
ALTER DATABASE [HotelBookingSystem] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [HotelBookingSystem] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [HotelBookingSystem] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [HotelBookingSystem] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'HotelBookingSystem', N'ON'
GO
ALTER DATABASE [HotelBookingSystem] SET QUERY_STORE = ON
GO
ALTER DATABASE [HotelBookingSystem] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [HotelBookingSystem]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 10/30/2024 10:07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[Account_ID] [int] IDENTITY(1,1) NOT NULL,
	[ROLE_ID] [int] NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[DOB] [date] NULL,
	[Email] [nvarchar](100) NOT NULL,
	[UseName] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[Avatar] [nvarchar](max) NULL,
	[Phonenumber] [nvarchar](max) NULL,
	[Gender] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdateDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Account_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 10/30/2024 10:07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[Review_ID] [int] IDENTITY(1,1) NOT NULL,
	[Room_ID] [int] NULL,
	[Order_ID] [int] NULL,
	[Account_ID] [int] NULL,
	[Rating] [decimal](2, 1) NULL,
	[ReviewDate] [datetime] NULL,
	[Comment] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Review_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback_Image]    Script Date: 10/30/2024 10:07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback_Image](
	[Review_Image_ID] [int] IDENTITY(1,1) NOT NULL,
	[Image_Url] [nvarchar](max) NULL,
	[Review_ID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Review_Image_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 10/30/2024 10:07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[Order_Date] [datetime] NOT NULL,
	[Total_Money] [decimal](18, 2) NOT NULL,
	[Discount] [decimal](5, 2) NULL,
	[Order_Status] [nvarchar](50) NULL,
	[Account_ID] [int] NULL,
	[Note] [text] NULL,
	[PaymentCode] [nvarchar](50) NULL,
	[Number_Extra_Adult] [int] NULL,
	[Number_Extra_Child] [int] NULL,
 CONSTRAINT [PK__Order__C3905BAF0BB03AF8] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order_Details]    Script Date: 10/30/2024 10:07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order_Details](
	[OD_ID] [int] IDENTITY(1,1) NOT NULL,
	[Room_ID] [int] NULL,
	[Check_In] [date] NOT NULL,
	[Check_Out] [date] NOT NULL,
	[OrderID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[OD_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 10/30/2024 10:07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Role_ID] [int] IDENTITY(1,1) NOT NULL,
	[Role_Name] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Role_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Room]    Script Date: 10/30/2024 10:07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Room](
	[Room_ID] [int] IDENTITY(1,1) NOT NULL,
	[Type_ID] [int] NULL,
	[Room_Number] [int] NOT NULL,
	[Room_Status] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[Created_Date] [date] NOT NULL,
	[Update_Date] [date] NULL,
	[Floor] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Room_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Service]    Script Date: 10/30/2024 10:07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Service](
	[Service_ID] [int] IDENTITY(1,1) NOT NULL,
	[Service_Name] [nvarchar](100) NOT NULL,
	[Created_Date] [date] NOT NULL,
	[Update_Date] [date] NULL,
	[Price] [decimal](18, 2) NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Service_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Service_Image]    Script Date: 10/30/2024 10:07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Service_Image](
	[Service_Image_ID] [int] IDENTITY(1,1) NOT NULL,
	[Image_Url] [nvarchar](max) NULL,
	[Service_ID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Service_Image_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Type_Room]    Script Date: 10/30/2024 10:07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Type_Room](
	[Type_ID] [int] IDENTITY(1,1) NOT NULL,
	[Type_Name] [nvarchar](100) NOT NULL,
	[Number_Of_Child] [int] NULL,
	[Number_Of_Adult] [int] NULL,
	[Number_Of_Bed] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[Price] [decimal](18, 2) NULL,
	[Deleted] [bit] NULL,
	[Maximum_Extra_Adult] [int] NULL,
	[Maximum_Extra_Child] [int] NULL,
	[Extra_Adult_Fee] [decimal](18, 2) NULL,
	[Extra_Child_Fee] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Type_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Type_Room_Image]    Script Date: 10/30/2024 10:07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Type_Room_Image](
	[Type_Room_Image_Id] [int] IDENTITY(1,1) NOT NULL,
	[Image_Url] [nvarchar](max) NULL,
	[Type_ID] [int] NULL,
	[Image_Index] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Type_Room_Image_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Type_Room_Service]    Script Date: 10/30/2024 10:07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Type_Room_Service](
	[Type_Service_ID] [int] IDENTITY(1,1) NOT NULL,
	[Type_ID] [int] NULL,
	[Service_ID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Type_Service_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Account] ON 

INSERT [dbo].[Account] ([Account_ID], [ROLE_ID], [FullName], [DOB], [Email], [UseName], [Password], [Avatar], [Phonenumber], [Gender], [Address], [CreatedDate], [UpdateDate], [Status]) VALUES (1, 1, N'Admin', CAST(N'1188-11-11' AS Date), N'admin@gmail.com', N'admin', N'abc123', NULL, NULL, N'Male', N'Somewhere1', CAST(N'2024-11-11T00:00:00.000' AS DateTime), CAST(N'2024-11-11T00:00:00.000' AS DateTime), N'Active')
INSERT [dbo].[Account] ([Account_ID], [ROLE_ID], [FullName], [DOB], [Email], [UseName], [Password], [Avatar], [Phonenumber], [Gender], [Address], [CreatedDate], [UpdateDate], [Status]) VALUES (3, 2, N'Customer', CAST(N'2024-01-01' AS Date), N'dangchhe176896@fpt.edu.vn', N'customer', N'abc123', NULL, N'0941229227', N'Male', N'Somewhere1', CAST(N'2024-10-30T21:03:12.617' AS DateTime), CAST(N'2024-10-30T21:31:08.703' AS DateTime), N'Active')
INSERT [dbo].[Account] ([Account_ID], [ROLE_ID], [FullName], [DOB], [Email], [UseName], [Password], [Avatar], [Phonenumber], [Gender], [Address], [CreatedDate], [UpdateDate], [Status]) VALUES (4, 3, N'Staff', CAST(N'2024-01-01' AS Date), N'staff@gmail.com', N'staff', N'abc123', NULL, NULL, N'Male', N'Something3', CAST(N'2024-10-30T21:04:16.817' AS DateTime), CAST(N'2024-10-30T21:04:16.817' AS DateTime), N'Active')
SET IDENTITY_INSERT [dbo].[Account] OFF
GO
SET IDENTITY_INSERT [dbo].[Order] ON 

INSERT [dbo].[Order] ([OrderID], [Order_Date], [Total_Money], [Discount], [Order_Status], [Account_ID], [Note], [PaymentCode], [Number_Extra_Adult], [Number_Extra_Child]) VALUES (1, CAST(N'2024-10-30T21:26:51.530' AS DateTime), CAST(3300.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(5, 2)), N'Success', 3, NULL, N'NfG4xrjv', 0, 1)
INSERT [dbo].[Order] ([OrderID], [Order_Date], [Total_Money], [Discount], [Order_Status], [Account_ID], [Note], [PaymentCode], [Number_Extra_Adult], [Number_Extra_Child]) VALUES (2, CAST(N'2024-10-30T21:33:47.343' AS DateTime), CAST(2200.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(5, 2)), N'Success', 3, NULL, N'mxWaZ1HU', 0, 0)
SET IDENTITY_INSERT [dbo].[Order] OFF
GO
SET IDENTITY_INSERT [dbo].[Order_Details] ON 

INSERT [dbo].[Order_Details] ([OD_ID], [Room_ID], [Check_In], [Check_Out], [OrderID]) VALUES (1, 1, CAST(N'2024-10-31' AS Date), CAST(N'2024-11-01' AS Date), 1)
INSERT [dbo].[Order_Details] ([OD_ID], [Room_ID], [Check_In], [Check_Out], [OrderID]) VALUES (2, 2, CAST(N'2024-10-31' AS Date), CAST(N'2024-11-01' AS Date), 1)
INSERT [dbo].[Order_Details] ([OD_ID], [Room_ID], [Check_In], [Check_Out], [OrderID]) VALUES (3, 3, CAST(N'2024-10-31' AS Date), CAST(N'2024-11-01' AS Date), 1)
INSERT [dbo].[Order_Details] ([OD_ID], [Room_ID], [Check_In], [Check_Out], [OrderID]) VALUES (4, 1, CAST(N'2024-11-01' AS Date), CAST(N'2024-11-02' AS Date), 2)
SET IDENTITY_INSERT [dbo].[Order_Details] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([Role_ID], [Role_Name]) VALUES (1, N'Admin')
INSERT [dbo].[Role] ([Role_ID], [Role_Name]) VALUES (2, N'Customer')
INSERT [dbo].[Role] ([Role_ID], [Role_Name]) VALUES (3, N'Staff')
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[Room] ON 

INSERT [dbo].[Room] ([Room_ID], [Type_ID], [Room_Number], [Room_Status], [Description], [Created_Date], [Update_Date], [Floor]) VALUES (1, 1, 101, N'Active', N'', CAST(N'2024-10-30' AS Date), CAST(N'2024-10-30' AS Date), 1)
INSERT [dbo].[Room] ([Room_ID], [Type_ID], [Room_Number], [Room_Status], [Description], [Created_Date], [Update_Date], [Floor]) VALUES (2, 1, 102, N'Active', N'', CAST(N'2024-10-30' AS Date), CAST(N'2024-10-30' AS Date), 1)
INSERT [dbo].[Room] ([Room_ID], [Type_ID], [Room_Number], [Room_Status], [Description], [Created_Date], [Update_Date], [Floor]) VALUES (3, 1, 103, N'Active', N'', CAST(N'2024-10-30' AS Date), CAST(N'2024-10-30' AS Date), 1)
SET IDENTITY_INSERT [dbo].[Room] OFF
GO
SET IDENTITY_INSERT [dbo].[Service] ON 

INSERT [dbo].[Service] ([Service_ID], [Service_Name], [Created_Date], [Update_Date], [Price], [Description], [Status]) VALUES (1, N'Spa', CAST(N'2024-10-30' AS Date), CAST(N'2024-10-30' AS Date), CAST(2000.00 AS Decimal(18, 2)), N'Siêu thoải mái', N'Active')
INSERT [dbo].[Service] ([Service_ID], [Service_Name], [Created_Date], [Update_Date], [Price], [Description], [Status]) VALUES (2, N'Bể Bơi', CAST(N'2024-10-30' AS Date), CAST(N'2024-10-30' AS Date), CAST(3000.00 AS Decimal(18, 2)), N'Kình ngư số 1 việt nam', N'Active')
SET IDENTITY_INSERT [dbo].[Service] OFF
GO
SET IDENTITY_INSERT [dbo].[Service_Image] ON 

INSERT [dbo].[Service_Image] ([Service_Image_ID], [Image_Url], [Service_ID]) VALUES (1, N'http://res.cloudinary.com/dt9hjydap/image/upload/v1730297337/hotel_images/ecgugi9gqwjitunp0ljp.jpg', 1)
INSERT [dbo].[Service_Image] ([Service_Image_ID], [Image_Url], [Service_ID]) VALUES (2, N'http://res.cloudinary.com/dt9hjydap/image/upload/v1730297339/hotel_images/so0trttyvtpmmtf7cykj.jpg', 1)
INSERT [dbo].[Service_Image] ([Service_Image_ID], [Image_Url], [Service_ID]) VALUES (3, N'http://res.cloudinary.com/dt9hjydap/image/upload/v1730297451/hotel_images/gmfzjth9h9yt25okoruv.jpg', 2)
INSERT [dbo].[Service_Image] ([Service_Image_ID], [Image_Url], [Service_ID]) VALUES (4, N'http://res.cloudinary.com/dt9hjydap/image/upload/v1730297452/hotel_images/g8flddkqcqcxq3xjdvy4.jpg', 2)
SET IDENTITY_INSERT [dbo].[Service_Image] OFF
GO
SET IDENTITY_INSERT [dbo].[Type_Room] ON 

INSERT [dbo].[Type_Room] ([Type_ID], [Type_Name], [Number_Of_Child], [Number_Of_Adult], [Number_Of_Bed], [Description], [Price], [Deleted], [Maximum_Extra_Adult], [Maximum_Extra_Child], [Extra_Adult_Fee], [Extra_Child_Fee]) VALUES (1, N'Single', 1, 1, 1, N'Phòng cho người cô đơn', CAST(2000.00 AS Decimal(18, 2)), 0, 1, 1, CAST(1000.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[Type_Room] OFF
GO
SET IDENTITY_INSERT [dbo].[Type_Room_Image] ON 

INSERT [dbo].[Type_Room_Image] ([Type_Room_Image_Id], [Image_Url], [Type_ID], [Image_Index]) VALUES (1, N'http://res.cloudinary.com/dt9hjydap/image/upload/v1730297972/hotel_images/vkvhtnjtx9ldz28dh3rd.jpg', 1, 0)
INSERT [dbo].[Type_Room_Image] ([Type_Room_Image_Id], [Image_Url], [Type_ID], [Image_Index]) VALUES (2, N'http://res.cloudinary.com/dt9hjydap/image/upload/v1730297973/hotel_images/gxw4gycdus8kwb5kdfoj.jpg', 1, 1)
SET IDENTITY_INSERT [dbo].[Type_Room_Image] OFF
GO
SET IDENTITY_INSERT [dbo].[Type_Room_Service] ON 

INSERT [dbo].[Type_Room_Service] ([Type_Service_ID], [Type_ID], [Service_ID]) VALUES (1, 1, 1)
INSERT [dbo].[Type_Room_Service] ([Type_Service_ID], [Type_ID], [Service_ID]) VALUES (2, 1, 2)
SET IDENTITY_INSERT [dbo].[Type_Room_Service] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Account__A9D10534C09B56E6]    Script Date: 10/30/2024 10:07:15 PM ******/
ALTER TABLE [dbo].[Account] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT (getdate()) FOR [UpdateDate]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT ('Active') FOR [Status]
GO
ALTER TABLE [dbo].[Feedback] ADD  DEFAULT (getdate()) FOR [ReviewDate]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD FOREIGN KEY([ROLE_ID])
REFERENCES [dbo].[Role] ([Role_ID])
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD FOREIGN KEY([Account_ID])
REFERENCES [dbo].[Account] ([Account_ID])
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD FOREIGN KEY([Room_ID])
REFERENCES [dbo].[Room] ([Room_ID])
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK__Review__Order_ID__5812160E] FOREIGN KEY([Order_ID])
REFERENCES [dbo].[Order] ([OrderID])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK__Review__Order_ID__5812160E]
GO
ALTER TABLE [dbo].[Feedback_Image]  WITH CHECK ADD FOREIGN KEY([Review_ID])
REFERENCES [dbo].[Feedback] ([Review_ID])
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK__Order__Account_I__5070F446] FOREIGN KEY([Account_ID])
REFERENCES [dbo].[Account] ([Account_ID])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK__Order__Account_I__5070F446]
GO
ALTER TABLE [dbo].[Order_Details]  WITH CHECK ADD  CONSTRAINT [FK__Order_Det__Room___534D60F1] FOREIGN KEY([Room_ID])
REFERENCES [dbo].[Room] ([Room_ID])
GO
ALTER TABLE [dbo].[Order_Details] CHECK CONSTRAINT [FK__Order_Det__Room___534D60F1]
GO
ALTER TABLE [dbo].[Order_Details]  WITH CHECK ADD  CONSTRAINT [FK_Order_Details_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Order] ([OrderID])
GO
ALTER TABLE [dbo].[Order_Details] CHECK CONSTRAINT [FK_Order_Details_Order]
GO
ALTER TABLE [dbo].[Room]  WITH CHECK ADD FOREIGN KEY([Type_ID])
REFERENCES [dbo].[Type_Room] ([Type_ID])
GO
ALTER TABLE [dbo].[Service_Image]  WITH CHECK ADD FOREIGN KEY([Service_ID])
REFERENCES [dbo].[Service] ([Service_ID])
GO
ALTER TABLE [dbo].[Type_Room_Image]  WITH CHECK ADD  CONSTRAINT [FK_Type_Room_Image_Type_Room] FOREIGN KEY([Type_ID])
REFERENCES [dbo].[Type_Room] ([Type_ID])
GO
ALTER TABLE [dbo].[Type_Room_Image] CHECK CONSTRAINT [FK_Type_Room_Image_Type_Room]
GO
ALTER TABLE [dbo].[Type_Room_Service]  WITH CHECK ADD FOREIGN KEY([Service_ID])
REFERENCES [dbo].[Service] ([Service_ID])
GO
ALTER TABLE [dbo].[Type_Room_Service]  WITH CHECK ADD  CONSTRAINT [FK_Type_Room_Service_Type_Room] FOREIGN KEY([Type_ID])
REFERENCES [dbo].[Type_Room] ([Type_ID])
GO
ALTER TABLE [dbo].[Type_Room_Service] CHECK CONSTRAINT [FK_Type_Room_Service_Type_Room]
GO
USE [master]
GO
ALTER DATABASE [HotelBookingSystem] SET  READ_WRITE 
GO
