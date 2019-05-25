IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[spDiscordGetCharInfoByName]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[spDiscordGetCharInfoByName]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[spDiscordGetClanInfoByName]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[spDiscordGetClanInfoByName]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[spDiscordGetClanRanking]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[spDiscordGetClanRanking]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[spDiscordGetServerInfo]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[spDiscordGetServerInfo]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spDiscordGetCharInfoByName]
	@Name VARCHAR(24)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [c].[AID], [c].[CID], [c].[Name], [c].[Level], [c].[Sex], [c].[XP], [c].[RegDate], [c].[LastTime], [c].[PlayTime], [c].[KillCount], [c].[DeathCount], [a].[UGradeID], [cl].[Name] AS [ClanName]
	FROM [dbo].[Character] (nolock) AS [c]
	INNER JOIN [dbo].[Account] AS [a] ON [c].[AID] = [a].[AID]
	LEFT OUTER JOIN [dbo].[ClanMember] AS [clm] ON [c].[CID] = [clm].[CID]
	LEFT OUTER JOIN [dbo].[Clan] AS [cl] ON [clm].[CLID] = [cl].[CLID]
	WHERE [c].[Name] = @Name 
		AND ([c].[DeleteFlag] = 0 OR [c].[DeleteFlag] IS NULL)
END
GO

CREATE PROCEDURE [dbo].[spDiscordGetClanInfoByName]
	@Name VARCHAR(24)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [cl].[CLID], [cl].[Name], [cl].[MasterCID], [cl].[Point], [cl].[Wins], [cl].[RegDate], [cl].[Losses], [cl].[Draws], [cl].[Ranking], [cl].[EmblemUrl], [c].[Name] AS [Leader]
	FROM [dbo].[Clan] (nolock) AS [cl]
	INNER JOIN [dbo].[Character] AS [c] ON [cl].[MasterCID] = [c].[CID]
	WHERE [cl].[Name] = @Name 
		AND ([cl].[DeleteFlag] = 0 OR [cl].[DeleteFlag] IS NULL) 
		AND ([c].[DeleteFlag] = 0 OR [c].[DeleteFlag] IS NULL)
END
GO

CREATE PROCEDURE [dbo].[spDiscordGetClanRanking]
AS
BEGIN
	SELECT TOP 5 [cl].[CLID], [cl].[Name], [cl].[Point] 
	FROM [dbo].[Clan] (nolock) AS [cl]
    WHERE ([cl].[DeleteFlag] = 0 OR [cl].[DeleteFlag] IS NULL) AND [cl].[Ranking] > 0 
    ORDER BY [cl].[Ranking] ASC
END
GO

CREATE PROCEDURE [dbo].[spDiscordGetServerInfo]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [s].[ServerID], [s].[ServerName], [s].[CurrPlayer], [s].[MaxPlayer], [s].[Time]
	FROM [dbo].[ServerStatus] (nolock) AS [s]
END
GO