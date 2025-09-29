--Scaffold-DbContext "Server=localhost\SQLEXPRESS02;Initial Catalog=compago;persist security info=True;Integrated Security=SSPI;MultipleActiveResultSets=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir . -Context CompagoDbContext -NoOnConfiguring -Force

IF OBJECTPROPERTY(object_id('dbo.User'), N'IsTable') = 1 DROP TABLE [dbo].[User]
GO
IF OBJECTPROPERTY(object_id('dbo.Role'), N'IsTable') = 1 DROP TABLE [dbo].[Role]
GO
IF OBJECTPROPERTY(object_id('dbo.InvoiceTag'), N'IsTable') = 1 DROP TABLE [dbo].[InvoiceTag]
GO
IF OBJECTPROPERTY(object_id('dbo.Tag'), N'IsTable') = 1 DROP TABLE [dbo].[Tag]
GO

CREATE TABLE [dbo].[Role](
	[Id] [tinyint] NOT NULL IDENTITY(1,1),
	[CreatedAt] [datetime2] NOT NULL DEFAULT(GETUTCDATE()),
	[CreatedBy] [int] NOT NULL DEFAULT(0),
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [int] NULL,
	[Name] [nvarchar](100) NOT NULL
 CONSTRAINT [Role_PK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

CREATE TABLE [dbo].[User](
	[Id] [int] NOT NULL IDENTITY(1,1),
	[CreatedAt] [datetime2] NOT NULL DEFAULT(GETUTCDATE()),
	[CreatedBy] [int] NOT NULL DEFAULT(0),
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [int] NULL,
	[RoleId] [tinyint] NOT NULL,
	[Username] [nvarchar](100) NOT NULL,
	[PasswordHash] [nvarchar](100) NOT NULL
 CONSTRAINT [User_PK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
ALTER TABLE [dbo].[User] WITH CHECK ADD CONSTRAINT [User_RoleFK] FOREIGN KEY([RoleId]) REFERENCES [dbo].[Role]([Id])
GO

CREATE TABLE [dbo].[Tag](
	[Id] [smallint] NOT NULL IDENTITY(1,1),
	[CreatedAt] [datetime2] NOT NULL DEFAULT(GETUTCDATE()),
	[CreatedBy] [int] NOT NULL DEFAULT(0),
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [int] NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Color] [nvarchar](10) NOT NULL DEFAULT('#000000')
 CONSTRAINT [Tag_PK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

CREATE TABLE [dbo].InvoiceTag(
	[InvoiceId] [nvarchar](100) NOT NULL,
	[TagId] [smallint] NOT NULL,
	[CreatedAt] [datetime2] NOT NULL DEFAULT(GETUTCDATE()),
	[CreatedBy] [int] NOT NULL DEFAULT(0),
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [int] NULL,
 CONSTRAINT [InvoiceTag_PK] PRIMARY KEY CLUSTERED 
(
	[InvoiceId] ASC,
	[TagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
ALTER TABLE [dbo].[InvoiceTag] WITH CHECK ADD CONSTRAINT [InvoiceTag_TagFK] FOREIGN KEY([TagId]) REFERENCES [dbo].[Tag]([Id])
GO

SET IDENTITY_INSERT [Role] ON
GO
INSERT INTO [Role]([Id], [Name]) VALUES (1, 'Admin')
INSERT INTO [Role]([Id], [Name]) VALUES (2, 'User')
SET IDENTITY_INSERT [Role] OFF
GO

SET IDENTITY_INSERT [User] ON
GO
INSERT INTO [User]([Id], [RoleId], [Username], [PasswordHash]) VALUES (1, 1, 'admin@citou.se', 'XXXXXXXXXXXXX')
SET IDENTITY_INSERT [User] OFF
GO