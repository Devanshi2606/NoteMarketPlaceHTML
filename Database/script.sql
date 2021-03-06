USE [master]
GO
/****** Object:  Database [Notes_Marketplace]    Script Date: 4/10/2021 10:03:16 PM ******/
CREATE DATABASE [Notes_Marketplace]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Notes_Marketplace', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\Notes_Marketplace.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Notes_Marketplace_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\Notes_Marketplace_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [Notes_Marketplace] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Notes_Marketplace].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Notes_Marketplace] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET ARITHABORT OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Notes_Marketplace] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Notes_Marketplace] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Notes_Marketplace] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Notes_Marketplace] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET RECOVERY FULL 
GO
ALTER DATABASE [Notes_Marketplace] SET  MULTI_USER 
GO
ALTER DATABASE [Notes_Marketplace] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Notes_Marketplace] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Notes_Marketplace] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Notes_Marketplace] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Notes_Marketplace] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Notes_Marketplace] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'Notes_Marketplace', N'ON'
GO
ALTER DATABASE [Notes_Marketplace] SET QUERY_STORE = OFF
GO
USE [Notes_Marketplace]
GO
/****** Object:  User [devu]    Script Date: 4/10/2021 10:03:16 PM ******/
CREATE USER [devu] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [devu]
GO
/****** Object:  Table [dbo].[Countries]    Script Date: 4/10/2021 10:03:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Countries](
	[CountryID] [int] IDENTITY(1,1) NOT NULL,
	[CountryName] [varchar](100) NOT NULL,
	[CountryCode] [varchar](100) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[CountryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Downloads]    Script Date: 4/10/2021 10:03:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Downloads](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NoteID] [int] NOT NULL,
	[SellerId] [int] NOT NULL,
	[DownloaderId] [int] NOT NULL,
	[IsSellerHasAllowedDownload] [bit] NOT NULL,
	[AttachmentName] [varchar](max) NOT NULL,
	[AttachmentPath] [varchar](max) NOT NULL,
	[IsAttachmentDownloaded] [bit] NOT NULL,
	[AttachmentDownloadDate] [datetime] NULL,
	[IsPaid] [bit] NOT NULL,
	[PurchasedPrice] [decimal](18, 2) NULL,
	[NoteTitle] [varchar](100) NOT NULL,
	[NoteCategory] [int] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Downloads] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NoteCategories]    Script Date: 4/10/2021 10:03:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoteCategories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [varchar](100) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Note] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NoteTypes]    Script Date: 4/10/2021 10:03:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoteTypes](
	[TypeID] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [varchar](100) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_NoteTypes] PRIMARY KEY CLUSTERED 
(
	[TypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SellerNotes]    Script Date: 4/10/2021 10:03:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SellerNotes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SellerID] [int] NOT NULL,
	[Title] [varchar](100) NOT NULL,
	[Category] [int] NOT NULL,
	[DisplayPicture] [varchar](500) NULL,
	[NoteType] [int] NULL,
	[NumberofPages] [int] NULL,
	[Description] [varchar](max) NOT NULL,
	[Country] [int] NULL,
	[UniversityName] [varchar](200) NULL,
	[Course] [varchar](100) NULL,
	[CourseCode] [varchar](100) NULL,
	[Professor] [varchar](100) NULL,
	[IsPaid] [bit] NOT NULL,
	[SellingPrice] [decimal](18, 2) NULL,
	[NotesPreview] [nvarchar](max) NULL,
	[PublishedDate] [datetime] NOT NULL,
	[Status] [varchar](30) NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[ActionedBy] [int] NULL,
	[AdminRemarks] [varchar](max) NULL,
	[rating] [decimal](18, 1) NULL,
	[FileName] [varchar](100) NOT NULL,
	[FilePath] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[total_Earnings] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_SellerNotes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SellerNotesReportedIssues]    Script Date: 4/10/2021 10:03:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SellerNotesReportedIssues](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NoteID] [int] NOT NULL,
	[ReportedBYID] [int] NOT NULL,
	[AgainstDownloadID] [int] NULL,
	[Remarks] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_SellerNotesReportedIssues] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SellerNotesReviews]    Script Date: 4/10/2021 10:03:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SellerNotesReviews](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NoteID] [int] NOT NULL,
	[Comments] [varchar](max) NOT NULL,
	[Ratings] [decimal](18, 1) NOT NULL,
	[ReviewedByID] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_SellerNotesReviews] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemConfigurations]    Script Date: 4/10/2021 10:03:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemConfigurations](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[K_ey] [varchar](100) NOT NULL,
	[Value] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [int] NULL,
 CONSTRAINT [PK_SystemConfigurations] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserProfile]    Script Date: 4/10/2021 10:03:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfile](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[DOB] [date] NULL,
	[Gender] [varchar](30) NULL,
	[Country_Code] [varchar](5) NULL,
	[Phone_Number] [varchar](20) NULL,
	[Profile_Picture] [varchar](500) NULL,
	[Address_Line1] [varchar](100) NULL,
	[Address_Line2] [varchar](100) NULL,
	[City] [varchar](50) NULL,
	[State] [varchar](50) NULL,
	[Zip_Code] [varchar](50) NULL,
	[Country] [int] NULL,
	[University] [varchar](100) NULL,
	[College] [varchar](100) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[Total_expenses] [decimal](18, 2) NULL,
	[Total_earnings] [decimal](18, 2) NULL,
	[IsActive] [bit] NOT NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 4/10/2021 10:03:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[ID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 4/10/2021 10:03:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[EmailID] [varchar](100) NOT NULL,
	[Password] [varchar](24) NOT NULL,
	[IsEmailVerified] [bit] NOT NULL,
	[ActivationCode] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[JoiningDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBY] [int] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users_Admin]    Script Date: 4/10/2021 10:03:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users_Admin](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[Secondary_Email] [varchar](max) NULL,
	[Country_Code] [varchar](5) NULL,
	[Phone_Number] [varchar](20) NULL,
	[Profile_Picture] [varchar](max) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Users_Admin] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Countries] ON 

INSERT [dbo].[Countries] ([CountryID], [CountryName], [CountryCode], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (1, N'USA', N'+1', CAST(N'2021-03-04T03:10:06.543' AS DateTime), 0, NULL, NULL, 1)
INSERT [dbo].[Countries] ([CountryID], [CountryName], [CountryCode], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (2, N'India', N'+91', CAST(N'2021-03-04T10:12:12.000' AS DateTime), 0, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[Countries] OFF
GO
SET IDENTITY_INSERT [dbo].[Downloads] ON 

INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (1, 7, 64, 64, 1, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(N'2021-03-11T23:55:43.690' AS DateTime), 1, CAST(20.00 AS Decimal(18, 2)), N'demo', 7, NULL, NULL, CAST(N'2021-04-07T12:35:17.740' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (6, 5, 64, 64, 1, N'Notes - MarketPlace - DataBase in excel sheet.xlsx', N'~\UploadedFiles\64\Notes - MarketPlace - DataBase in excel sheet.xlsx', 0, CAST(N'2021-03-11T23:55:43.690' AS DateTime), 0, CAST(0.00 AS Decimal(18, 2)), N'demo', 1, NULL, NULL, CAST(N'2021-04-07T12:35:54.500' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (7, 10, 64, 64, 1, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(N'2021-03-11T23:55:43.690' AS DateTime), 0, CAST(0.00 AS Decimal(18, 2)), N'demo', 6, NULL, NULL, CAST(N'2021-04-07T12:36:00.600' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (10, 17, 64, 64, 1, N'sample (3).pdf', N'~\UploadedFiles\64\sample (3).pdf', 0, CAST(N'2021-03-11T23:55:43.690' AS DateTime), 1, CAST(5.00 AS Decimal(18, 2)), N'Algorithm', 7, NULL, NULL, CAST(N'2021-04-07T12:36:02.237' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (11, 12, 64, 64, 1, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(N'2021-03-11T23:55:43.690' AS DateTime), 0, CAST(0.00 AS Decimal(18, 2)), N'demo', 6, NULL, NULL, CAST(N'2021-04-07T12:36:03.843' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (15, 7, 64, 5, 1, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(N'2021-03-11T23:55:43.690' AS DateTime), 1, CAST(20.00 AS Decimal(18, 2)), N'demo', 7, NULL, NULL, CAST(N'2021-04-07T12:36:05.523' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (16, 12, 64, 5, 1, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(N'2021-03-11T23:55:47.917' AS DateTime), 0, CAST(0.00 AS Decimal(18, 2)), N'demo', 6, NULL, NULL, CAST(N'2021-04-07T12:36:07.097' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (17, 17, 64, 5, 1, N'sample (3).pdf', N'~\UploadedFiles\64\sample (3).pdf', 0, CAST(N'2021-03-11T23:55:51.273' AS DateTime), 1, CAST(5.00 AS Decimal(18, 2)), N'Algorithm', 7, NULL, NULL, CAST(N'2021-04-07T12:35:37.940' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (19, 6, 64, 5, 1, N'OverView_Artificial Intelligence_Unit-1,2.pdf', N'~\UploadedFiles\64\OverView_Artificial Intelligence_Unit-1,2.pdf', 0, CAST(N'2021-04-03T14:12:37.847' AS DateTime), 1, CAST(5.00 AS Decimal(18, 2)), N'Algorithm', 7, NULL, NULL, CAST(N'2021-04-07T12:36:09.123' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (20, 11, 64, 5, 1, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(N'2021-04-03T14:12:40.457' AS DateTime), 0, CAST(0.00 AS Decimal(18, 2)), N'demo', 6, NULL, NULL, CAST(N'2021-04-07T12:36:11.467' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (21, 13, 64, 5, 1, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(N'2021-04-03T14:12:42.670' AS DateTime), 0, CAST(0.00 AS Decimal(18, 2)), N'demo', 6, NULL, NULL, CAST(N'2021-04-07T12:36:13.003' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (22, 14, 64, 5, 1, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(N'2021-04-03T14:12:47.213' AS DateTime), 0, CAST(0.00 AS Decimal(18, 2)), N'demo', 6, NULL, NULL, CAST(N'2021-04-07T12:36:14.520' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (23, 16, 64, 5, 1, N'sample (3).pdf', N'~\UploadedFiles\64\sample (3).pdf', 0, CAST(N'2021-04-03T14:12:50.077' AS DateTime), 0, CAST(0.00 AS Decimal(18, 2)), N'demo', 7, NULL, NULL, CAST(N'2021-04-07T12:36:16.470' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (29, 8, 64, 5, 1, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(N'2021-04-07T12:44:00.467' AS DateTime), 1, CAST(150.00 AS Decimal(18, 2)), N'add notes', 7, NULL, NULL, CAST(N'2021-04-07T12:44:33.410' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (30, 8, 64, 5, 1, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(N'2021-04-07T12:44:03.743' AS DateTime), 1, CAST(150.00 AS Decimal(18, 2)), N'add notes', 7, NULL, NULL, CAST(N'2021-04-07T12:44:40.593' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (43, 8, 64, 64, 1, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(N'2021-04-08T18:46:30.713' AS DateTime), 1, CAST(150.00 AS Decimal(18, 2)), N'add notes', 7, NULL, NULL, CAST(N'2021-04-10T19:37:32.180' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (44, 4, 64, 5, 1, N'Viraj''s Resume (5).pdf', N'~\UploadedFiles\64\Viraj''s Resume (5).pdf', 0, CAST(N'2021-04-08T19:58:46.890' AS DateTime), 0, CAST(0.00 AS Decimal(18, 2)), N'Algorithm', 6, NULL, NULL, CAST(N'2021-04-08T19:59:18.027' AS DateTime), 64, 1)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (49, 20, 0, 5, 0, N'Viraj''s Resume (5) (1).pdf', N'~\UploadedFiles\0\Viraj''s Resume (5) (1).pdf', 0, CAST(N'2021-04-10T15:08:50.443' AS DateTime), 1, CAST(10.00 AS Decimal(18, 2)), N'EditPart', 6, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (50, 20, 0, 64, 0, N'Viraj''s Resume (5) (1).pdf', N'~\UploadedFiles\0\Viraj''s Resume (5) (1).pdf', 0, CAST(N'2021-04-10T19:44:22.403' AS DateTime), 1, CAST(10.00 AS Decimal(18, 2)), N'EditPart', 6, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[Downloads] ([ID], [NoteID], [SellerId], [DownloaderId], [IsSellerHasAllowedDownload], [AttachmentName], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (51, 22, 64, 64, 1, N'Viraj''s Resume (5) (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (5) (1).pdf', 0, CAST(N'2021-04-10T19:44:31.667' AS DateTime), 1, CAST(20.00 AS Decimal(18, 2)), N'demo', 7, NULL, NULL, CAST(N'2021-04-10T19:44:50.947' AS DateTime), 64, 1)
SET IDENTITY_INSERT [dbo].[Downloads] OFF
GO
SET IDENTITY_INSERT [dbo].[NoteCategories] ON 

INSERT [dbo].[NoteCategories] ([CategoryID], [CategoryName], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (1, N'DS', N'Data Science', CAST(N'2021-03-04T03:10:06.543' AS DateTime), 0, NULL, NULL, 1)
INSERT [dbo].[NoteCategories] ([CategoryID], [CategoryName], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (6, N'CD', N'Compiler Designer', CAST(N'2021-03-07T03:10:06.543' AS DateTime), 0, NULL, NULL, 1)
INSERT [dbo].[NoteCategories] ([CategoryID], [CategoryName], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (7, N'OOP', N'Object Oriented Programming', CAST(N'2021-03-04T04:28:06.543' AS DateTime), 0, NULL, NULL, 1)
INSERT [dbo].[NoteCategories] ([CategoryID], [CategoryName], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (1002, N'CG', N'Computer Graphics', CAST(N'2021-03-31T17:59:58.820' AS DateTime), 0, CAST(N'2021-03-31T18:52:53.320' AS DateTime), 0, 0)
SET IDENTITY_INSERT [dbo].[NoteCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[NoteTypes] ON 

INSERT [dbo].[NoteTypes] ([TypeID], [TypeName], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (1, N'Search Tree', N'DFS and BFS', CAST(N'2021-03-04T03:10:06.543' AS DateTime), 0, NULL, NULL, 1)
INSERT [dbo].[NoteTypes] ([TypeID], [TypeName], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (4, N'Sorting', N'Sorting Algorithm', CAST(N'2021-03-04T03:10:04.000' AS DateTime), 0, CAST(N'2021-03-31T20:15:26.100' AS DateTime), 0, 1)
SET IDENTITY_INSERT [dbo].[NoteTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[SellerNotes] ON 

INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (4, 64, N'Algorithm', 6, N'~\UploadedFiles\64\Untitled Diagram.png', 1, 10, N'Nothing much just algorithm', 1, N'UVPCE', N'Data Structure', N'2IT403', N'Hiten Sadani', 0, CAST(0.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\Viraj''s Resume (4).pdf', CAST(N'2021-03-07T21:27:12.143' AS DateTime), N'approved', CAST(N'2021-03-07T21:27:12.143' AS DateTime), 0, NULL, CAST(3.3 AS Decimal(18, 1)), N'Viraj''s Resume (5).pdf', N'~\UploadedFiles\64\Viraj''s Resume (5).pdf', 1, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (5, 64, N'demo', 1, N'~\UploadedFiles\64\FlowChartDiagram.png', 1, 15, N'testing add notes', 1, N'UVPCE', N'Data Structure', N'2IT403', N'Vikram', 0, CAST(0.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\lesson1&2.pdf', CAST(N'2021-03-08T03:17:00.357' AS DateTime), N'inReview', CAST(N'2021-03-08T03:17:00.357' AS DateTime), 0, N'worst', CAST(0.0 AS Decimal(18, 1)), N'Notes - MarketPlace - DataBase in excel sheet.xlsx', N'~\UploadedFiles\64\Notes - MarketPlace - DataBase in excel sheet.xlsx', 0, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (6, 64, N'Algorithm', 7, N'~\UploadedFiles\64\FlowChartDiagram.png', 1, 12, N'Checking errors', 1, N'UVPCE', N'Data Structure', N'2IT403', N'Vikram', 1, CAST(5.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\Training Plan for Batch 2020-2021.pdf', CAST(N'2021-03-08T03:17:00.357' AS DateTime), N'approved', CAST(N'2021-03-08T03:17:00.357' AS DateTime), 0, NULL, CAST(0.0 AS Decimal(18, 1)), N'OverView_Artificial Intelligence_Unit-1,2.pdf', N'~\UploadedFiles\64\OverView_Artificial Intelligence_Unit-1,2.pdf', 1, CAST(5.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (7, 64, N'demo', 7, N'~\UploadedFiles\64\Untitled Diagram.png', 1, 10, N'all erors are solve just checking sell mode radio button', 1, N'UVPCE', N'Data Structure', N'2IT403', N'Hiten Sadani', 1, CAST(20.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\IntroSearch-3-fall10.pdf', CAST(N'2021-03-08T03:17:00.357' AS DateTime), N'draft', CAST(N'2021-03-08T03:17:00.357' AS DateTime), 0, N'Worst', NULL, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(40.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (8, 64, N'add notes', 7, N'~\UploadedFiles\64\FlowChartDiagram.png', 4, 50, N'Nothing much just algorithm', 2, N'UVPCE', N'Object Oriented Programming', N'2IT403', N'Hiten Sadani', 1, CAST(150.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\lesson1&2.pdf', CAST(N'2021-04-07T12:43:41.317' AS DateTime), N'approved', CAST(N'2021-03-26T20:04:28.400' AS DateTime), 0, NULL, CAST(4.0 AS Decimal(18, 1)), N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 1, CAST(600.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (9, 64, N'demo', 6, N'~\UploadedFiles\64\Untitled Diagram.png', 1, 12, N'Checking errors', 1, N'asd', N'rty', N'2IT403', N'Hiten Sadani', 0, CAST(0.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\Training Plan for Batch 2020-2021.pdf', CAST(N'2021-03-08T08:04:37.133' AS DateTime), N'inReview', CAST(N'2021-03-08T08:04:37.133' AS DateTime), NULL, NULL, CAST(0.0 AS Decimal(18, 1)), N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (10, 64, N'demo', 6, N'~\UploadedFiles\64\FlowChartDiagram.png', 1, 12, N'Checking errors', 1, N'asd', N'rty', N'2IT403', N'Hiten Sadani', 0, CAST(0.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\Viraj''s Resume (4).pdf', CAST(N'2021-03-08T08:04:37.133' AS DateTime), N'submitted', CAST(N'2021-03-08T08:04:37.133' AS DateTime), 0, NULL, NULL, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (11, 64, N'demo', 6, N'~\UploadedFiles\64\FlowChartDiagram.png', 1, 12, N'Checking errors', 1, N'asd', N'rty', N'2IT403', N'Hiten Sadani', 0, CAST(0.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\lesson1&2.pdf', CAST(N'2021-03-08T08:04:37.133' AS DateTime), N'removed', CAST(N'2021-03-08T08:04:37.133' AS DateTime), 0, N'Worst book ever', NULL, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (12, 64, N'demo', 6, N'~\UploadedFiles\64\FlowChartDiagram.png', 1, 12, N'Checking errors', 1, N'asd', N'rty', N'2IT403', N'Hiten Sadani', 0, CAST(0.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\Viraj''s Resume (4).pdf', CAST(N'2021-03-08T08:04:37.133' AS DateTime), N'inReview', CAST(N'2021-03-08T08:04:37.133' AS DateTime), 0, NULL, NULL, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 0, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (13, 64, N'demo', 6, N'~\UploadedFiles\64\Untitled Diagram.png', 1, 12, N'Checking errors', 1, N'asd', N'rty', N'2IT403', N'Hiten Sadani', 0, CAST(0.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\lesson1&2.pdf', CAST(N'2021-03-08T08:04:37.133' AS DateTime), N'approved', CAST(N'2021-03-08T08:04:37.133' AS DateTime), 0, NULL, CAST(0.0 AS Decimal(18, 1)), N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 1, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (14, 64, N'demo', 6, N'~\UploadedFiles\64\Untitled Diagram.png', 1, 12, N'Checking errors', 1, N'asd', N'rty', N'2IT403', N'Hiten Sadani', 0, CAST(0.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\Viraj''s Resume (4).pdf', CAST(N'2021-03-08T08:04:37.133' AS DateTime), N'approved', CAST(N'2021-03-08T08:04:37.133' AS DateTime), 0, NULL, NULL, N'Viraj''s Resume (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (1).pdf', 1, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (15, 64, N'Algorithm', 6, N'~\UploadedFiles\64\FlowChartDiagram.png', 1, 11, N'its my resume', 1, N'UVPCE', N'Compiler Designer', N'2IT403', N'Hiten Sadani', 1, CAST(5.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\Training Plan for Batch 2020-2021.pdf', CAST(N'2021-04-10T19:34:37.330' AS DateTime), N'draft', CAST(N'2021-04-10T19:34:37.330' AS DateTime), NULL, NULL, NULL, N'Viraj''sResume(5).pdf', N'~\UploadedFiles\64\Viraj''s Resume (5).pdf', 0, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (16, 64, N'demo', 7, N'~\UploadedFiles\64\Untitled Diagram.png', 1, 2, N'its my resume', 1, N'UVPCE', N'Object Oriented Programming', N'2IT402', N'vygbuhnij', 0, CAST(0.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\lesson1&2.pdf', CAST(N'2021-03-09T22:21:45.297' AS DateTime), N'approved', CAST(N'2021-03-09T22:21:45.297' AS DateTime), 0, NULL, NULL, N'sample (3).pdf', N'~\UploadedFiles\64\sample (3).pdf', 1, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (17, 64, N'Algorithm', 7, N'~\UploadedFiles\64\FlowChartDiagram.png', 1, 11, N'its not my resume', 1, N'UVPCE', N'Object Oriented Programming', N'2IT403', N'vygbuhnij', 1, CAST(5.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\Viraj''s Resume (6).pdf', CAST(N'2021-03-09T22:57:32.710' AS DateTime), N'rejected', CAST(N'2021-03-09T22:57:32.710' AS DateTime), 0, N'Copied', NULL, N'sample (3).pdf', N'~\UploadedFiles\64\sample (3).pdf', 0, CAST(10.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (18, 64, N'asdf', 1, N'~\UploadedFiles\64\53904204_IMG-20210327-WA0006.jpg', 4, 12, N'asdf', 2, N'asd', N'rty', N'bghjkm', N'vygbuhnij', 0, CAST(0.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\InitiateSingleEntryPaymentSummary26-03-2021.pdf', CAST(N'2021-04-08T18:02:59.390' AS DateTime), N'removed', CAST(N'2021-04-08T18:01:19.160' AS DateTime), 0, N'Just a checking email part', NULL, N'InitiateSingleEntryPaymentSummary26-03-2021.pdf', N'~\UploadedFiles\64\InitiateSingleEntryPaymentSummary26-03-2021.pdf', 0, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (19, 64, N'asdf', 6, N'~\UploadedFiles\64\53904204_IMG-20210327-WA0006.jpg', 1, 10, N'testing add notes', 2, N'UVPCE', N'Compiler Designer', N'2IT401', N'Hiten Sadani', 1, CAST(20.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\sample (3).pdf', CAST(N'2021-04-08T20:52:49.300' AS DateTime), N'submitted', CAST(N'2021-04-08T20:52:49.300' AS DateTime), NULL, NULL, NULL, N'InitiateSingleEntryPaymentSummary26-03-2021.pdf', N'~\UploadedFiles\64\InitiateSingleEntryPaymentSummary26-03-2021.pdf', 0, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (20, 0, N'EditPart', 6, N'~\UploadedFiles\0\53904204_IMG-20210327-WA0006.jpg', 4, 12, N'testing add notes', 2, N'UVPCE', N'Compiler Designer', N'2IT401', N'Hiten Sadani', 1, CAST(10.00 AS Decimal(18, 2)), N'~\UploadedFiles\0\sample (3).pdf', CAST(N'2021-04-10T11:04:34.953' AS DateTime), N'approved', CAST(N'2021-04-10T11:03:59.007' AS DateTime), 0, NULL, NULL, N'Viraj''s Resume (5) (1).pdf', N'~\UploadedFiles\0\Viraj''s Resume (5) (1).pdf', 1, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (21, 0, N'add notes', 1, N'~\UploadedFiles\0\53904204_IMG-20210327-WA0006.jpg', 4, 50, N'testing add notes', 1, N'UVPCE', N'Compiler Designer', N'bghjkm', N'Hiten Sadani', 1, CAST(10.00 AS Decimal(18, 2)), N'~\UploadedFiles\0\InitiateSingleEntryPaymentSummary26-03-2021.pdf', CAST(N'2021-04-10T12:33:34.413' AS DateTime), N'draft', CAST(N'2021-04-10T12:33:34.413' AS DateTime), NULL, NULL, NULL, N'InitiateSingleEntryPaymentSummary26-03-2021.pdf', N'~\UploadedFiles\0\InitiateSingleEntryPaymentSummary26-03-2021.pdf', 0, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[SellerNotes] ([ID], [SellerID], [Title], [Category], [DisplayPicture], [NoteType], [NumberofPages], [Description], [Country], [UniversityName], [Course], [CourseCode], [Professor], [IsPaid], [SellingPrice], [NotesPreview], [PublishedDate], [Status], [ModifiedDate], [ActionedBy], [AdminRemarks], [rating], [FileName], [FilePath], [IsActive], [total_Earnings]) VALUES (22, 64, N'demo', 7, N'~\UploadedFiles\64\53904204_IMG-20210327-WA0006.jpg', 4, 12, N'asdf', 2, N'asd', N'Compiler Designer', N'2IT402', N'Hiten Sadani', 1, CAST(20.00 AS Decimal(18, 2)), N'~\UploadedFiles\64\Viraj''s Resume (5) (1).pdf', CAST(N'2021-04-10T19:39:58.207' AS DateTime), N'approved', CAST(N'2021-04-10T19:35:44.573' AS DateTime), 0, NULL, NULL, N'Viraj''s Resume (5) (1).pdf', N'~\UploadedFiles\64\Viraj''s Resume (5) (1).pdf', 1, CAST(20.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[SellerNotes] OFF
GO
SET IDENTITY_INSERT [dbo].[SellerNotesReportedIssues] ON 

INSERT [dbo].[SellerNotesReportedIssues] ([ID], [NoteID], [ReportedBYID], [AgainstDownloadID], [Remarks], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (1, 4, 3, NULL, N'Bad', CAST(N'2021-03-08T03:17:00.357' AS DateTime), NULL, NULL, NULL, 1)
INSERT [dbo].[SellerNotesReportedIssues] ([ID], [NoteID], [ReportedBYID], [AgainstDownloadID], [Remarks], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (4, 4, 4, NULL, N'Poor', CAST(N'2021-03-08T03:17:00.357' AS DateTime), NULL, NULL, NULL, 1)
INSERT [dbo].[SellerNotesReportedIssues] ([ID], [NoteID], [ReportedBYID], [AgainstDownloadID], [Remarks], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (5, 17, 64, NULL, N'This was for testing report issue button', CAST(N'2021-04-05T18:15:04.253' AS DateTime), NULL, NULL, NULL, 1)
INSERT [dbo].[SellerNotesReportedIssues] ([ID], [NoteID], [ReportedBYID], [AgainstDownloadID], [Remarks], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (6, 17, 64, NULL, N'just checking ', CAST(N'2021-04-08T17:54:02.010' AS DateTime), NULL, NULL, NULL, 1)
INSERT [dbo].[SellerNotesReportedIssues] ([ID], [NoteID], [ReportedBYID], [AgainstDownloadID], [Remarks], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (7, 17, 64, NULL, N'just checking whether it works or not', CAST(N'2021-04-08T20:49:03.470' AS DateTime), NULL, NULL, NULL, 1)
INSERT [dbo].[SellerNotesReportedIssues] ([ID], [NoteID], [ReportedBYID], [AgainstDownloadID], [Remarks], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (8, 5, 64, NULL, N'they are asking for money', CAST(N'2021-04-09T16:18:42.557' AS DateTime), NULL, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[SellerNotesReportedIssues] OFF
GO
SET IDENTITY_INSERT [dbo].[SellerNotesReviews] ON 

INSERT [dbo].[SellerNotesReviews] ([ID], [NoteID], [Comments], [Ratings], [ReviewedByID], [CreatedDate], [IsActive], [ModifiedBy], [ModifiedDate]) VALUES (3, 4, N'good', CAST(4.0 AS Decimal(18, 1)), 0, CAST(N'2021-03-08T03:17:00.357' AS DateTime), 0, 0, CAST(N'2021-04-08T20:43:08.583' AS DateTime))
INSERT [dbo].[SellerNotesReviews] ([ID], [NoteID], [Comments], [Ratings], [ReviewedByID], [CreatedDate], [IsActive], [ModifiedBy], [ModifiedDate]) VALUES (4, 4, N'excellent', CAST(3.0 AS Decimal(18, 1)), 64, CAST(N'2021-03-08T03:17:00.357' AS DateTime), 1, NULL, NULL)
INSERT [dbo].[SellerNotesReviews] ([ID], [NoteID], [Comments], [Ratings], [ReviewedByID], [CreatedDate], [IsActive], [ModifiedBy], [ModifiedDate]) VALUES (5, 8, N'excellent', CAST(4.0 AS Decimal(18, 1)), 64, CAST(N'2021-04-05T17:54:10.437' AS DateTime), 1, NULL, NULL)
INSERT [dbo].[SellerNotesReviews] ([ID], [NoteID], [Comments], [Ratings], [ReviewedByID], [CreatedDate], [IsActive], [ModifiedBy], [ModifiedDate]) VALUES (6, 4, N'good', CAST(2.0 AS Decimal(18, 1)), 0, CAST(N'2021-03-08T03:17:00.357' AS DateTime), 1, NULL, NULL)
INSERT [dbo].[SellerNotesReviews] ([ID], [NoteID], [Comments], [Ratings], [ReviewedByID], [CreatedDate], [IsActive], [ModifiedBy], [ModifiedDate]) VALUES (8, 4, N'Excellent', CAST(3.0 AS Decimal(18, 1)), 64, CAST(N'2021-04-08T07:31:43.000' AS DateTime), 0, 0, CAST(N'2021-04-08T20:40:20.813' AS DateTime))
INSERT [dbo].[SellerNotesReviews] ([ID], [NoteID], [Comments], [Ratings], [ReviewedByID], [CreatedDate], [IsActive], [ModifiedBy], [ModifiedDate]) VALUES (9, 8, N'nice work it saved my day', CAST(4.0 AS Decimal(18, 1)), 0, CAST(N'2021-04-08T19:56:51.697' AS DateTime), 0, 0, CAST(N'2021-04-08T20:41:15.020' AS DateTime))
INSERT [dbo].[SellerNotesReviews] ([ID], [NoteID], [Comments], [Ratings], [ReviewedByID], [CreatedDate], [IsActive], [ModifiedBy], [ModifiedDate]) VALUES (10, 4, N'Saved my day', CAST(5.0 AS Decimal(18, 1)), 0, CAST(N'2021-04-08T20:00:32.717' AS DateTime), 1, NULL, NULL)
SET IDENTITY_INSERT [dbo].[SellerNotesReviews] OFF
GO
SET IDENTITY_INSERT [dbo].[SystemConfigurations] ON 

INSERT [dbo].[SystemConfigurations] ([ID], [K_ey], [Value], [CreatedDate], [CreatedBy]) VALUES (1, N'Email', N'parejiyadevanshi9727@gmail.com', CAST(N'2021-04-10T15:03:37.283' AS DateTime), 0)
INSERT [dbo].[SystemConfigurations] ([ID], [K_ey], [Value], [CreatedDate], [CreatedBy]) VALUES (2, N'pno', N'+918160049231', CAST(N'2021-04-10T15:03:37.320' AS DateTime), 0)
INSERT [dbo].[SystemConfigurations] ([ID], [K_ey], [Value], [CreatedDate], [CreatedBy]) VALUES (3, N'admin_Email', N'parejiyadevanshi@gmail.com,devanshiparejiya17@gnu.ac.in', CAST(N'2021-04-10T15:03:37.320' AS DateTime), 0)
INSERT [dbo].[SystemConfigurations] ([ID], [K_ey], [Value], [CreatedDate], [CreatedBy]) VALUES (5, N'Facebook', N'https://www.tatvasoft.com/?utm_source=googlemybusiness&utm_medium=GMB_India&utm_campaign=GoogleMyBusiness', CAST(N'2021-04-10T15:03:37.320' AS DateTime), 0)
INSERT [dbo].[SystemConfigurations] ([ID], [K_ey], [Value], [CreatedDate], [CreatedBy]) VALUES (6, N'Twitter', N'https://www.tatvasoft.com/?utm_source=googlemybusiness&utm_medium=GMB_India&utm_campaign=GoogleMyBusiness', CAST(N'2021-04-10T15:03:37.320' AS DateTime), 0)
INSERT [dbo].[SystemConfigurations] ([ID], [K_ey], [Value], [CreatedDate], [CreatedBy]) VALUES (7, N'LinkedIn', N'https://in.linkedin.com/company/tatvasoft', CAST(N'2021-04-10T15:03:37.320' AS DateTime), 0)
INSERT [dbo].[SystemConfigurations] ([ID], [K_ey], [Value], [CreatedDate], [CreatedBy]) VALUES (8, N'Note_image', N'~\UploadedFiles\defaultfile\computer-science.png', CAST(N'2021-04-08T23:57:28.003' AS DateTime), 0)
INSERT [dbo].[SystemConfigurations] ([ID], [K_ey], [Value], [CreatedDate], [CreatedBy]) VALUES (10, N'Display_image', N'~\UploadedFiles\defaultfile\customer-2.png', CAST(N'2021-04-08T23:57:28.003' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[SystemConfigurations] OFF
GO
SET IDENTITY_INSERT [dbo].[UserProfile] ON 

INSERT [dbo].[UserProfile] ([ID], [UserId], [DOB], [Gender], [Country_Code], [Phone_Number], [Profile_Picture], [Address_Line1], [Address_Line2], [City], [State], [Zip_Code], [Country], [University], [College], [CreatedDate], [Total_expenses], [Total_earnings], [IsActive], [ModifiedDate]) VALUES (1, 64, CAST(N'2000-06-26' AS Date), N'Female', N'1', N'9727662773', N'~\UploadedFiles\64\reviewer-1.png', N'E-401', N'Dada bhagwan complex', N'Surat', N'Gujarat', N'394185', 1, N'TatvaSoft', N'UVPCE', CAST(N'2021-04-08T17:48:10.977' AS DateTime), CAST(0.00 AS Decimal(18, 2)), CAST(320.00 AS Decimal(18, 2)), 1, NULL)
INSERT [dbo].[UserProfile] ([ID], [UserId], [DOB], [Gender], [Country_Code], [Phone_Number], [Profile_Picture], [Address_Line1], [Address_Line2], [City], [State], [Zip_Code], [Country], [University], [College], [CreatedDate], [Total_expenses], [Total_earnings], [IsActive], [ModifiedDate]) VALUES (1002, 0, CAST(N'2000-06-26' AS Date), N'Female', N'1', N'8160058593', N'~\UploadedFiles\0\reviewer-3.png', N'E-401', N'Dada bhagwan complex', N'Surat', N'Gujarat', N'394185', 1, N'Ganpat', N'UVPCE', CAST(N'2021-04-01T18:24:55.820' AS DateTime), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 1, NULL)
INSERT [dbo].[UserProfile] ([ID], [UserId], [DOB], [Gender], [Country_Code], [Phone_Number], [Profile_Picture], [Address_Line1], [Address_Line2], [City], [State], [Zip_Code], [Country], [University], [College], [CreatedDate], [Total_expenses], [Total_earnings], [IsActive], [ModifiedDate]) VALUES (2010, 5, CAST(N'2021-04-09' AS Date), N'Female', N'2', N'9456321785', NULL, N'qwer', N'sdrcfvg', N'Surat', N'Gujarat', N'394185', 1, N'Ganpat', N'UVPCE', CAST(N'2021-04-09T15:04:57.803' AS DateTime), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 1, NULL)
SET IDENTITY_INSERT [dbo].[UserProfile] OFF
GO
INSERT [dbo].[UserRoles] ([ID], [Name]) VALUES (1, N'User')
INSERT [dbo].[UserRoles] ([ID], [Name]) VALUES (2, N'Admin')
INSERT [dbo].[UserRoles] ([ID], [Name]) VALUES (3, N'SuperAdmin')
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [ActivationCode], [IsActive], [JoiningDate], [ModifiedDate], [ModifiedBY]) VALUES (0, 2, N'Devu', N'patel', N'parejiyadevanshi@gmail.com', N'devu', 1, NULL, 1, CAST(N'2021-04-26T19:16:57.440' AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [ActivationCode], [IsActive], [JoiningDate], [ModifiedDate], [ModifiedBY]) VALUES (3, 1, N'patel', N'patel', N'parejiya123@gmail.com', N'as', 1, NULL, 1, CAST(N'2021-03-21T19:16:57.440' AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [ActivationCode], [IsActive], [JoiningDate], [ModifiedDate], [ModifiedBY]) VALUES (4, 1, N'viraj', N'viraj', N'viraj@gmail.com', N'viraj', 1, NULL, 1, CAST(N'2021-03-20T19:16:57.440' AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [ActivationCode], [IsActive], [JoiningDate], [ModifiedDate], [ModifiedBY]) VALUES (5, 1, N'pankti', N'pankti', N'pankti@gmail.com', N'pankti', 1, NULL, 1, CAST(N'2021-03-15T19:16:57.440' AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [ActivationCode], [IsActive], [JoiningDate], [ModifiedDate], [ModifiedBY]) VALUES (6, 1, N'mom', N'mom', N'mom@gmail.com', N'devu', 1, NULL, 1, CAST(N'2021-03-06T19:16:57.440' AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [ActivationCode], [IsActive], [JoiningDate], [ModifiedDate], [ModifiedBY]) VALUES (64, 1, N'Devanshi', N'patel', N'devanshiparejiya17@gnu.ac.in', N'devu', 1, N'9241b0e5-7701-4506-9e39-648cc61940b7', 1, CAST(N'2021-04-26T19:16:57.440' AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [ActivationCode], [IsActive], [JoiningDate], [ModifiedDate], [ModifiedBY]) VALUES (1007, 2, N'viraj', N'patel', N'patelviraj7046@gmail.com', N'viraj', 1, NULL, 1, CAST(N'2021-04-20T19:16:57.440' AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [ActivationCode], [IsActive], [JoiningDate], [ModifiedDate], [ModifiedBY]) VALUES (1009, 2, N'Mahesh', N'patel', N'maheshpatel@gmail.com', N'AdminPassword', 1, NULL, 1, CAST(N'2021-04-09T19:59:33.713' AS DateTime), CAST(N'2021-04-09T19:59:33.713' AS DateTime), 1007)
INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [ActivationCode], [IsActive], [JoiningDate], [ModifiedDate], [ModifiedBY]) VALUES (1013, 1, N'Trial', N'trial', N'trial@gmail.com', N'trial', 0, N'7041fe22-ad01-48c5-8b21-c813927a5609', 0, CAST(N'2021-04-10T15:40:35.923' AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([ID], [RoleID], [FirstName], [LastName], [EmailID], [Password], [IsEmailVerified], [ActivationCode], [IsActive], [JoiningDate], [ModifiedDate], [ModifiedBY]) VALUES (1015, 3, N'Devanshi', N'Patel', N'parejiyadevanshi9727@gmail.com', N'devu', 1, NULL, 1, CAST(N'2021-04-10T04:41:45.000' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET IDENTITY_INSERT [dbo].[Users_Admin] ON 

INSERT [dbo].[Users_Admin] ([ID], [UserID], [Secondary_Email], [Country_Code], [Phone_Number], [Profile_Picture], [IsActive], [CreatedDate], [ModifiedDate]) VALUES (1, 0, N'parejiyadevanshi9727@gmail.com', N'2', N'8160058593', N'~\UploadedFiles\0\member.png', 1, CAST(N'2021-04-09T19:26:20.837' AS DateTime), CAST(N'2021-04-10T19:43:33.660' AS DateTime))
INSERT [dbo].[Users_Admin] ([ID], [UserID], [Secondary_Email], [Country_Code], [Phone_Number], [Profile_Picture], [IsActive], [CreatedDate], [ModifiedDate]) VALUES (2, 1007, N'parejiyadevanshi9727@gmail.com', N'2', N'7990912305', NULL, 1, CAST(N'2021-04-09T19:52:33.970' AS DateTime), CAST(N'2021-04-09T19:52:33.943' AS DateTime))
INSERT [dbo].[Users_Admin] ([ID], [UserID], [Secondary_Email], [Country_Code], [Phone_Number], [Profile_Picture], [IsActive], [CreatedDate], [ModifiedDate]) VALUES (3, 1009, NULL, N'2', N'7568942312', NULL, NULL, CAST(N'2021-04-09T19:59:34.007' AS DateTime), CAST(N'2021-04-09T19:59:33.713' AS DateTime))
SET IDENTITY_INSERT [dbo].[Users_Admin] OFF
GO
ALTER TABLE [dbo].[Countries] ADD  CONSTRAINT [DF_Countries_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[NoteCategories] ADD  CONSTRAINT [DF_NoteCategories_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[NoteTypes] ADD  CONSTRAINT [DF_NoteTypes_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SellerNotes] ADD  CONSTRAINT [DF_SellerNotes_IsPaid]  DEFAULT ((0)) FOR [IsPaid]
GO
ALTER TABLE [dbo].[SellerNotes] ADD  CONSTRAINT [DF_SellerNotes_SellingPrice]  DEFAULT ((0)) FOR [SellingPrice]
GO
ALTER TABLE [dbo].[SellerNotes] ADD  CONSTRAINT [DF_SellerNotes_total_Earnings]  DEFAULT ((0)) FOR [total_Earnings]
GO
ALTER TABLE [dbo].[UserProfile] ADD  CONSTRAINT [DF_UserProfile_DOB]  DEFAULT (getdate()) FOR [DOB]
GO
ALTER TABLE [dbo].[UserProfile] ADD  CONSTRAINT [DF_UserProfile_Total_expenses]  DEFAULT ((0)) FOR [Total_expenses]
GO
ALTER TABLE [dbo].[UserProfile] ADD  CONSTRAINT [DF_UserProfile_Total_earnings]  DEFAULT ((0)) FOR [Total_earnings]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_IsEmailVerified]  DEFAULT ((0)) FOR [IsEmailVerified]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF__Users__IsActive__0D0FEE32]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Downloads]  WITH CHECK ADD  CONSTRAINT [FK_Downloads_SellerNotes] FOREIGN KEY([NoteID])
REFERENCES [dbo].[SellerNotes] ([ID])
GO
ALTER TABLE [dbo].[Downloads] CHECK CONSTRAINT [FK_Downloads_SellerNotes]
GO
ALTER TABLE [dbo].[SellerNotes]  WITH CHECK ADD  CONSTRAINT [FK_SellerNotes_Countries] FOREIGN KEY([Country])
REFERENCES [dbo].[Countries] ([CountryID])
GO
ALTER TABLE [dbo].[SellerNotes] CHECK CONSTRAINT [FK_SellerNotes_Countries]
GO
ALTER TABLE [dbo].[SellerNotes]  WITH CHECK ADD  CONSTRAINT [FK_SellerNotes_NoteCategories] FOREIGN KEY([Category])
REFERENCES [dbo].[NoteCategories] ([CategoryID])
GO
ALTER TABLE [dbo].[SellerNotes] CHECK CONSTRAINT [FK_SellerNotes_NoteCategories]
GO
ALTER TABLE [dbo].[SellerNotes]  WITH CHECK ADD  CONSTRAINT [FK_SellerNotes_NoteTypes] FOREIGN KEY([NoteType])
REFERENCES [dbo].[NoteTypes] ([TypeID])
GO
ALTER TABLE [dbo].[SellerNotes] CHECK CONSTRAINT [FK_SellerNotes_NoteTypes]
GO
ALTER TABLE [dbo].[SellerNotes]  WITH CHECK ADD  CONSTRAINT [FK_SellerNotes_User] FOREIGN KEY([SellerID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[SellerNotes] CHECK CONSTRAINT [FK_SellerNotes_User]
GO
ALTER TABLE [dbo].[SellerNotesReportedIssues]  WITH CHECK ADD  CONSTRAINT [FK_SellerNotesReportedIssues_Downloads] FOREIGN KEY([AgainstDownloadID])
REFERENCES [dbo].[Downloads] ([ID])
GO
ALTER TABLE [dbo].[SellerNotesReportedIssues] CHECK CONSTRAINT [FK_SellerNotesReportedIssues_Downloads]
GO
ALTER TABLE [dbo].[SellerNotesReportedIssues]  WITH CHECK ADD  CONSTRAINT [FK_SellerNotesReportedIssues_SellerNotes] FOREIGN KEY([NoteID])
REFERENCES [dbo].[SellerNotes] ([ID])
GO
ALTER TABLE [dbo].[SellerNotesReportedIssues] CHECK CONSTRAINT [FK_SellerNotesReportedIssues_SellerNotes]
GO
ALTER TABLE [dbo].[SellerNotesReviews]  WITH CHECK ADD  CONSTRAINT [FK_SellerNotesReviews_SellerNotes] FOREIGN KEY([NoteID])
REFERENCES [dbo].[SellerNotes] ([ID])
GO
ALTER TABLE [dbo].[SellerNotesReviews] CHECK CONSTRAINT [FK_SellerNotesReviews_SellerNotes]
GO
ALTER TABLE [dbo].[UserProfile]  WITH CHECK ADD  CONSTRAINT [FK_UserProfile_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[UserProfile] CHECK CONSTRAINT [FK_UserProfile_Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_UserRoles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[UserRoles] ([ID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_UserRoles]
GO
ALTER TABLE [dbo].[Users_Admin]  WITH CHECK ADD  CONSTRAINT [FK_Users_Admin_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Users_Admin] CHECK CONSTRAINT [FK_Users_Admin_Users]
GO
USE [master]
GO
ALTER DATABASE [Notes_Marketplace] SET  READ_WRITE 
GO
