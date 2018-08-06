USE [SimpleMonitor]
GO

/****** Object:  View [dbo].[vwIISLog]    Script Date: 8/21/2016 10:46:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('vwIISLog', 'V') IS NOT NULL
	DROP VIEW vwIISLog;
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


