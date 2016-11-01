USE [master]
GO
/****** Object:  Database [DMVC]    Script Date: 01/11/2016 01:24:18 ب.ظ ******/
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DMVC')
BEGIN
	CREATE DATABASE [DMVC] ON  PRIMARY 
	( NAME = N'DMVC', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\DMVC.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
	 LOG ON 
	( NAME = N'DMVC_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\DMVC_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
END
GO

USE [DMVC]
GO
/****** Object:  Table [dbo].[TestTable]    Script Date: 01/11/2016 01:24:18 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestTable]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[TestTable](
		[Id] [int] NOT NULL,
		[Name] [nvarchar](50) NULL,
		[LastName] [nvarchar](50) NULL,
		[MobileNo] [nvarchar](50) NULL,
	 CONSTRAINT [PK_TestTable] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	INSERT [dbo].[TestTable] ([Id], [Name], [LastName], [MobileNo]) VALUES (1, N'Behzad', N'Khosravi', N'09149149202         ')
	INSERT [dbo].[TestTable] ([Id], [Name], [LastName], [MobileNo]) VALUES (2, N'Ramin', N'Pourpeyghambar', N'09141231234         ')
	INSERT [dbo].[TestTable] ([Id], [Name], [LastName], [MobileNo]) VALUES (3, N'Behruz', N'Baxtiyari', N'09141231235         ')
	INSERT [dbo].[TestTable] ([Id], [Name], [LastName], [MobileNo]) VALUES (4, N'Hussein', N'Rezaiy', N'09145465465         ')
	INSERT [dbo].[TestTable] ([Id], [Name], [LastName], [MobileNo]) VALUES (5, N'Milad', N'Husseini', N'09146546565         ')
	INSERT [dbo].[TestTable] ([Id], [Name], [LastName], [MobileNo]) VALUES (6, N'Hassan', N'Pourvali', N'09196547891         ')
	INSERT [dbo].[TestTable] ([Id], [Name], [LastName], [MobileNo]) VALUES (7, N'Nima', N'Rostami', N'09149141234         ')
	INSERT [dbo].[TestTable] ([Id], [Name], [LastName], [MobileNo]) VALUES (8, N'Hamed', N'Roshangar', N'09149141379         ')
END