USE [20220419]
GO

/****** Object:  Table [dbo].[EFCustT_CreateBOMMessage]    Script Date: 2022/5/29 1:11:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EFCustT_CreateBOMMessage](
	[Bom] [nvarchar](500) NULL,
	[BOMCreatedOn] [datetime] NULL,
	[BOMCreatedBy] [nvarchar](500) NULL
) ON [PRIMARY]
GO


