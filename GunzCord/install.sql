CREATE PROCEDURE [dbo].[spDiscordGetCharInfoByName]
	@Name VARCHAR(24)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [c].[AID], [c].[CID], [c].[Name], [c].[Level], [c].[Sex], [c].[XP], [c].[RegDate], [c].[LastTime], [c].[PlayTime], [c].[KillCount], [c].[DeathCount], [a].[UGradeID], [cl].[Name]
	FROM [dbo].[Character] AS [c]
	INNER JOIN [dbo].[Account] AS [a] ON [c].[AID] = [a].[AID]
	LEFT OUTER JOIN [dbo].[ClanMember] AS [clm] ON [c].[CID] = [clm].[CID]
	LEFT OUTER JOIN [dbo].[Clan] AS [cl] ON [clm].[CLID] = [cl].[CLID]
	WHERE [c].[Name] = @Name
END
GO

CREATE PROCEDURE [dbo].[spDiscordGetServerInfo]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [s].[ServerID], [s].[ServerName], [s].[CurrPlayer], [s].[MaxPlayer], [s].[Time]
	FROM [dbo].[ServerStatus] AS [s]
END
GO