# GunzCord
GunzCord is a Discord Bot for GunZ Servers.
It provides commands that retrieve information from your GunZ server

## Features
### Available commands:
- !help - Shows a list of the available commands
- !char <name> - Display information about the specified GunZ character
- !clan <name> - Display information about the specified GunZ clan
- !clanranking - Display the top 5 clans leaderboard
- !server - Display the GunZ server list and online players on each server
- !online - Display the online player count
  
### Notifications:
GunzCord also includes Clan War notifications, a message will be sent to a specific channel every time Clan War match ends

## Setup
### Create Discord Application and Bot
First, you will need to create a Discord Application and then create a Bot inside that application. 

You will need to retrieve a Bot Token from the application.

For instructions on how to create the application and bot, check this article from the Discord.Net documentation:
https://discord.foxbot.me/stable/guides/getting_started/first-bot.html

### Configure appsettings.json
Configure the Connection String to match with your Database's Name, User and Password.
It's recommended to set the `db_owner` role to your SQL user unless you know exactly which permission to grant

Configure the `Token` setting with the Bot token you generated when the Discord app was created.

If you are not using the Clan War notifications, set `EnableClanWarNotifications` to `false` to prevent configuration errors

Otherwise, if you want to use the Clan War notifications, you must set the `ServerId` and `NotificationsChannelId` with your Discord Server Id and the Channel Id where you want the notifications to be sent. You will also have to do the Enable Service Broker step in the setup.
Check Discord documentation on how to get the server and channel ids here: https://support.discordapp.com/hc/en-us/articles/206346498-Where-can-I-find-my-User-Server-Message-ID-

Configure `EmblemBaseUrl` with the URL prefix for your clan emblems, it's the same you set up at system.xml in your Gunz client.

There are other settings at appsettings.json you can play with.

### Install the Stored Procedures in your Gunz database
Excecute the install.sql script provided with GunzCord to install all the required stored procedures in your Gunz Database

### Optional: Enable Service Broker in your Gunz database
If you want to use Clan War notifications, you must enable the Service Broker for them to work.
To enable the Service Broker execute the following SQL statement in your Gunz Database:

```SQL
ALTER DATABASE GunzDB SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE
```

### Run the software
Once all the setup steps have been made, you may run GunzCord.exe to start running the bot.

Check the console or log files for any errors you may encounter.
## License
This software is licensed under the GNU General Public License v3.0
