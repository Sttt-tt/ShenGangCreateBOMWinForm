USE test
GO

/****** Object:  Table [dbo].[EFCustT_CreateBOMMessage]    Script Date: 2022/5/29 1:11:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EFCustT_CreateBOMMessage](
	[Bom] [nvarchar](500) NULL,
	[BOMCreatedOn] [datetime] NULL,
	[BOMCreatedBy] [nvarchar](500) NULL,
	[BOMName] [nvarchar](500) NULL,
	[BOMCode] [nvarchar](500) NULL,
	[BOMUOM] [nvarchar](500) NULL,
	[BOMQty] [nvarchar](500) NULL,
	[ItemCode] [nvarchar](500) NULL,
	[ItemName] [nvarchar](500) NULL,
	[ItemUOM] [nvarchar](500) NULL,
	[ItemQty] [nvarchar](500) NULL,
	[ItemSize] [nvarchar](500) NULL,
	[ItemAttribute] [nvarchar](500) NULL,
	[Route] [nvarchar](500) NULL,
	[Memo] [nvarchar](500) NULL,

) ON [PRIMARY]
GO


