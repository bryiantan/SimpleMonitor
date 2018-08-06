SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlockedIp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IpAddress] [varchar](50) NOT NULL,
	[IsBlocked] [bit] NOT NULL,
	[BlockHit] [int] NOT NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_BlockedIp] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[iislog]    Script Date: 9/9/2016 10:37:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[iislog](
	[ApplicationName] [varchar](150) NULL,
	[LogFilename] [varchar](255) NULL,
	[LogRow] [int] NULL,
	[date] [datetime] NULL,
	[time] [datetime] NULL,
	[cIp] [varchar](255) NULL,
	[csUsername] [varchar](255) NULL,
	[sSitename] [varchar](255) NULL,
	[sComputername] [varchar](255) NULL,
	[sIp] [varchar](255) NULL,
	[sPort] [int] NULL,
	[csMethod] [varchar](255) NULL,
	[csUriStem] [varchar](255) NULL,
	[csUriQuery] [varchar](255) NULL,
	[scStatus] [int] NULL,
	[scSubstatus] [int] NULL,
	[scWin32Status] [int] NULL,
	[scBytes] [int] NULL,
	[csBytes] [int] NULL,
	[timeTaken] [int] NULL,
	[csVersion] [varchar](255) NULL,
	[csHost] [varchar](255) NULL,
	[csUserAgent] [varchar](255) NULL,
	[csCookie] [varchar](255) NULL,
	[csReferer] [varchar](255) NULL,
	[sEvent] [varchar](255) NULL,
	[sProcessType] [varchar](255) NULL,
	[sUserTime] [real] NULL,
	[sKernelTime] [real] NULL,
	[sPageFaults] [int] NULL,
	[sTotalProcs] [int] NULL,
	[sActiveProcs] [int] NULL,
	[sStoppedProcs] [int] NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_iislog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[vwIISLog]    Script Date: 9/9/2016 10:37:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwIISLog]
AS
SELECT 
	ApplicationName,
	l.ID 'Id'
	,'' LogFilename
	,LogRow
	,
	--CONVERT(DATETIME, CONVERT(CHAR(8), [date], 112) 
	--  + ' ' + CONVERT(CHAR(8), [time],108)) [date], 
	CONVERT(VARCHAR(10), [date], 120) + ' ' + CONVERT(VARCHAR(8), [time], 108) + ' ' + RIGHT(CONVERT(VARCHAR(30), [time], 9), 2) [date]
	,[time]
	,cIp
	,csUsername
	,sSitename
	,sComputername
	,sIp
	,ISNULL(sPort,80) sPort
	,csMethod
	,csUriStem
	,ISNULL(csUriQuery,'') csUriQuery
	,ISNULL(scStatus,0) scStatus
	,scSubstatus
	,scWin32Status
	,scBytes
	,csBytes
	,timeTaken
	,csVersion
	,csHost
	,ISNULL(csUserAgent,'') csUserAgent
	,ISNULL(csCookie,'') csCookie
	,ISNULL(csReferer,'') csReferer
	,sEvent
	,sProcessType
	,sUserTime
	,sKernelTime
	,sPageFaults
	,sTotalProcs
	,sActiveProcs
	,sStoppedProcs, 
	CASE (b.IsBlocked)
		WHEN 1 THEN 'Yes'
		ELSE 'No'
	END IsBlocked, ISNULL(BlockHit, 0) BlockHit
FROM dbo.iislog l LEFT JOIN dbo.BlockedIp b
ON l.cIp = b.IpAddress

GO
ALTER TABLE [dbo].[BlockedIp] ADD  CONSTRAINT [DF_BlockedIp_IsBlocked]  DEFAULT ((0)) FOR [IsBlocked]
GO
ALTER TABLE [dbo].[BlockedIp] ADD  CONSTRAINT [DF_BlockedIp_BlockHit]  DEFAULT ((0)) FOR [BlockHit]
GO
/****** Object:  StoredProcedure [dbo].[InsertUpdateBlockedIp]    Script Date: 9/9/2016 10:37:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		BT
-- Create date: 09/07/2016
-- Description:	SP to update/insert to BlockedIp table
/*
InsertUpdateBlockedIp '207.46.13.109', 'h'
InsertUpdateBlockedIp '164.132.161.38', 'u'
InsertUpdateBlockedIp '207.46.13.109', 'b'
*/
-- =============================================
CREATE PROCEDURE [dbo].[InsertUpdateBlockedIp]
	@ipAddress VARCHAR(50),
	@operation VARCHAR(1)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--block
    IF @operation = 'b' 
		BEGIN
				--exists, update
			IF (EXISTS(SELECT 'b' FROM [dbo].[BlockedIp] WHERE [IpAddress] = @ipAddress AND [IsBlocked] = 0))
				UPDATE [dbo].[BlockedIp] SET [IsBlocked] = 1, [LastUpdatedDate] = GETDATE()
					WHERE IpAddress = @ipAddress
			ELSE
				BEGIN
					--INSERT
					INSERT INTO [dbo].[BlockedIp]
						SELECT @ipAddress, 1, 0, GETDATE()
				END

		END

	--unblock
	 IF @operation = 'u' 
		BEGIN
			UPDATE [dbo].[BlockedIp] SET [IsBlocked] = 0, [LastUpdatedDate] = GETDATE() 
				WHERE IpAddress = @ipAddress
		END

	--update block hit count
	 IF @operation = 'h' 
		BEGIN
			UPDATE [dbo].[BlockedIp] SET BlockHit = BlockHit + 1, [LastUpdatedDate] = GETDATE()
				WHERE IpAddress = @ipAddress
		END

END

GO
