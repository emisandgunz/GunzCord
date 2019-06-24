using GunzSharp.Commands.Parameters;

namespace GunzSharp.Commands
{
	public static class MSharedCommandTable
	{
		public static void RegisterCommands(MCommandManager commandManager, MSharedCommandType commandType)
		{
			MCommandManager cm = commandManager;

			cm.C(MSharedCommand.MC_LOCAL_INFO, "Local.Info", "Local information", MCommandDescFlags.MCDT_LOCAL);

			cm.C(MSharedCommand.MC_LOCAL_ECHO, "Local.Echo", "Local echo test", MCommandDescFlags.MCDT_LOCAL)
				.P(MCommandParameterType.MPT_STR, "Message");

			cm.C(MSharedCommand.MC_LOCAL_LOGIN, "Local.Login", "Local Login", MCommandDescFlags.MCDT_LOCAL)
				.P(MCommandParameterType.MPT_UID, "uidComm")
				.P(MCommandParameterType.MPT_UID, "uidPlayer");

			cm.C(MSharedCommand.MC_HELP, "Help", "This command", MCommandDescFlags.MCDT_LOCAL);
			cm.C(MSharedCommand.MC_VERSION, "Version", "Version description", MCommandDescFlags.MCDT_LOCAL);

			cm.C(MSharedCommand.MC_DEBUG_TEST, "DebugTest", "Debug Test", MCommandDescFlags.MCDT_MACHINE2MACHINE);

			cm.C(MSharedCommand.MC_NET_REQUEST_INFO, "Net.RequestInfo", "Request Net information", MCommandDescFlags.MCDT_MACHINE2MACHINE);
			cm.C(MSharedCommand.MC_NET_RESPONSE_INFO, "Net.ResponseInfo", "Response Net information", MCommandDescFlags.MCDT_MACHINE2MACHINE)
				.P(MCommandParameterType.MPT_STR, "Information");
			cm.C(MSharedCommand.MC_NET_ECHO, "Net.Echo", "Echo test", MCommandDescFlags.MCDT_MACHINE2MACHINE)
				.P(MCommandParameterType.MPT_STR, "Message");

			cm.C(MSharedCommand.MC_MATCH_ANNOUNCE, "Match.Announce", "Announce Server Message", MCommandDescFlags.MCDT_MACHINE2MACHINE | MCommandDescFlags.MCCT_NON_ENCRYPTED)
				.P(MCommandParameterType.MPT_UINT, "Type")
				.P(MCommandParameterType.MPT_STR, "Msg");

			if (commandType.HasFlag(MSharedCommandType.Client) || commandType.HasFlag(MSharedCommandType.MatchServer))
			{
				cm.C(MSharedCommand.MC_CLOCK_SYNCHRONIZE, "Clock.Synchronize", "Synchronize Clock", MCommandDescFlags.MCDT_MACHINE2MACHINE)
					.P(MCommandParameterType.MPT_UINT, "GlobalClock(msec);");
				cm.C(MSharedCommand.MC_MATCH_LOGIN, "Match.Login", "Login Match Server", MCommandDescFlags.MCDT_MACHINE2MACHINE)
					.P(MCommandParameterType.MPT_STR, "UserID")
					.P(MCommandParameterType.MPT_STR, "Password")
					.P(MCommandParameterType.MPT_UINT, "CommandVersion")
					.P(MCommandParameterType.MPT_UINT, "nChecksumPack")
					.P(MCommandParameterType.MPT_BLOB, "EncryptMD5Value");
				cm.C(MSharedCommand.MC_MATCH_RESPONSE_LOGIN_FAILED, "", "", MCommandDescFlags.MCDT_MACHINE2MACHINE)
					.P(MCommandParameterType.MPT_STR, "Reason");
				cm.C(MSharedCommand.MC_MATCH_RESPONSE_LOGIN, "Match.ResponseLogin", "Response Login", MCommandDescFlags.MCDT_MACHINE2MACHINE)
					.P(MCommandParameterType.MPT_INT, "Result")
					.P(MCommandParameterType.MPT_STR, "ServerName")
					.P(MCommandParameterType.MPT_CHAR, "ServerMode")
					.P(MCommandParameterType.MPT_STR, "AccountID")
					.P(MCommandParameterType.MPT_UCHAR, "UGradeID")
					.P(MCommandParameterType.MPT_UCHAR, "PGradeID")
					.P(MCommandParameterType.MPT_UID, "uidPlayer")
					.P(MCommandParameterType.MPT_STR, "RandomValue")
					.P(MCommandParameterType.MPT_BLOB, "EncryptMsg");

				cm.C(MSharedCommand.MC_MATCH_RESPONSE_RESULT, "Match.Response.Result", "Response Result", MCommandDescFlags.MCDT_MACHINE2MACHINE)
					.P(MCommandParameterType.MPT_INT, "Result");
				cm.C(MSharedCommand.MC_MATCH_LOGIN_FROM_DBAGENT, "Match.LoginFromDBAgent", "Login from DBAgent", MCommandDescFlags.MCDT_LOCAL)
					.P(MCommandParameterType.MPT_UID, "CommUID")
					.P(MCommandParameterType.MPT_UID, "LoginID")
					.P(MCommandParameterType.MPT_STR, "Name")
					.P(MCommandParameterType.MPT_INT, "Sex")
					.P(MCommandParameterType.MPT_BOOL, "bFreeLoginIP")
					.P(MCommandParameterType.MPT_UINT, "nChecksumPack");
				cm.C(MSharedCommand.MC_MATCH_LOGIN_FROM_DBAGENT_FAILED, "Match.LoginFailedFromDBAgent", "Login Failed from DBAgent", MCommandDescFlags.MCDT_LOCAL)
					.P(MCommandParameterType.MPT_UID, "CommUID")
					.P(MCommandParameterType.MPT_INT, "Result");
				cm.C(MSharedCommand.MC_MATCH_FIND_HACKING, "Match.FinH", "FinH", MCommandDescFlags.MCDT_MACHINE2MACHINE);
				cm.C(MSharedCommand.MC_MATCH_DISCONNMSG, "MC_MATCH_DISCONNMSG", "disconnect reason", MCommandDescFlags.MCDT_MACHINE2MACHINE)
					.P(MCommandParameterType.MPT_UINT, "message id");
			}

			// Locator
			cm.C(MSharedCommand.MC_REQUEST_SERVER_LIST_INFO, "MC_REQUEST_SERVER_LIST_INFO", "request connectable server list info.", MCommandDescFlags.MCDT_MACHINE2MACHINE | MCommandDescFlags.MCCT_NON_ENCRYPTED);
			cm.C(MSharedCommand.MC_RESPONSE_SERVER_LIST_INFO, "MC_RESPONSE_SERVER_LIST_INFO", "response connectable server list info.", MCommandDescFlags.MCDT_MACHINE2MACHINE | MCommandDescFlags.MCCT_NON_ENCRYPTED)
				.P(MCommandParameterType.MPT_BLOB, "server list");
			cm.C(MSharedCommand.MC_RESPONSE_BLOCK_COUNTRY_CODE_IP, "MC_RESPONSE_BLOCK_COUNTRY_CODE_IP", "response connected ip country code is blocked.", MCommandDescFlags.MCDT_MACHINE2MACHINE | MCommandDescFlags.MCCT_NON_ENCRYPTED)
				.P(MCommandParameterType.MPT_STR, "Country Code")
				.P(MCommandParameterType.MPT_STR, "Routing URL");

			// IP filter
			cm.C(MSharedCommand.MC_RESPONSE_BLOCK_COUNTRYCODE, "MC_RESPONSE_BLOCK_COUNTRYCODE", "response block ip connected.", MCommandDescFlags.MCDT_MACHINE2MACHINE | MCommandDescFlags.MCCT_NON_ENCRYPTED)
				.P(MCommandParameterType.MPT_STR, "Comment");
			cm.C(MSharedCommand.MC_LOCAL_UPDATE_USE_COUNTRY_FILTER, "MC_LOCAL_UPDATE_USE_COUNTRY_FILTER", "update use country filter.", MCommandDescFlags.MCDT_LOCAL);

			cm.C(MSharedCommand.MC_LOCAL_GET_DB_IP_TO_COUNTRY, "MC_LOCAL_GET_DB_IP_TO_COUNTRY", "get db ip to country code.", MCommandDescFlags.MCDT_LOCAL);
			cm.C(MSharedCommand.MC_LOCAL_GET_DB_BLOCK_COUNTRY_CODE, "MC_LOCAL_GET_DB_BLOCK_COUNTRY_CODE", "get db block country code.", MCommandDescFlags.MCDT_LOCAL);
			cm.C(MSharedCommand.MC_LOCAL_GET_DB_CUSTOM_IP, "MC_LOCAL_GET_DB_CUSTOM_IP", "get db custom ip.", MCommandDescFlags.MCDT_LOCAL);

			cm.C(MSharedCommand.MC_LOCAL_UPDATE_IP_TO_COUNTRY, "MC_LOCAL_UPDATE_IP_TO_COUNTRY", "update ip to country code.", MCommandDescFlags.MCDT_LOCAL);
			cm.C(MSharedCommand.MC_LOCAL_UPDATE_BLOCK_COUTRYCODE, "MC_LOCAL_UPDATE_BLOCK_COUTRYCODE", "update block country code.", MCommandDescFlags.MCDT_LOCAL);
			cm.C(MSharedCommand.MC_LOCAL_UPDATE_CUSTOM_IP, "MC_LOCAL_UPDATE_CUSTOM_IP", "update custom ip.", MCommandDescFlags.MCDT_LOCAL);
			cm.C(MSharedCommand.MC_LOCAL_UPDATE_ACCEPT_INVALID_IP, "MC_LOCAL_UPDATE_ACCEPT_INVALID_IP", "update accept invalid ip.", MCommandDescFlags.MCDT_LOCAL);

			cm.C(MSharedCommand.MC_MATCH_ROUTE_UPDATE_STAGE_EQUIP_LOOK, "MC_MATCH_ROUTE_UPDATE_STAGE_EQUIP_LOOK", "route updated user equip info", MCommandDescFlags.MCDT_MACHINE2MACHINE)
				.P(MCommandParameterType.MPT_UID, "user uid")
				.P(MCommandParameterType.MPT_INT, "parts")
				.P(MCommandParameterType.MPT_INT, "itemid");
		}

		private static MCommandDesc C(this MCommandManager commandManager, MSharedCommand id, string name, string description, MCommandDescFlags flags)
		{
			MCommandDesc cd = new MCommandDesc((int)id, name, description, flags);

			commandManager.AddCommandDesc(cd);

			return cd;
		}

		private static MCommandDesc P(this MCommandDesc commandDesc, MCommandParameterType parameterType, string description)
		{
			MCommandParameterDesc p = new MCommandParameterDesc(parameterType, description);
			commandDesc.AddParamDesc(p);

			return commandDesc;
		}
	}
}
