--Scaffold-DbContext "Server=localhost\SQLEXPRESS02;Initial Catalog=campago;persist security info=True;Integrated Security=SSPI;MultipleActiveResultSets=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir . -Context CampagoDbContext -NoOnConfiguring -Force

IF OBJECTPROPERTY(object_id('dbo.User'), N'IsTable') = 1 DROP TABLE [dbo].[User]
GO
CREATE TABLE [dbo].[User](
	[Id] [int] NOT NULL IDENTITY(1,1),
	[CreatedAt] [bigint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL DEFAULT(GETUTCDATE()),
	[UpdatedBy] [datetime2](7) NULL,
	[Username] [nvarchar](100) NOT NULL,
	[PasswordHash] [nvarchar](100) NOT NULL
 CONSTRAINT [User_PK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
