IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[InsertUpdateBlockedIp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].InsertUpdateBlockedIp
GO

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
CREATE PROCEDURE InsertUpdateBlockedIp
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
