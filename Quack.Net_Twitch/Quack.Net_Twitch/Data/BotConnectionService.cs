using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quack.Net_Twitch.Data
{
    public class BotConnectionService
    {
        private TwitchBot _twitchBot;

        public Task InitializeBot(string streamerName)
        {
            _twitchBot = new TwitchBot(streamerName);
            return Task.CompletedTask;
        }

        public Task ConnectTwitchBotTo(string streamerName)
        {
            _twitchBot.JoinChannel(streamerName);
            return Task.CompletedTask;
        }

        public Task LeaveChannel(string streamerName)
        {
            _twitchBot.LeaveChannel(streamerName);
            return Task.CompletedTask;
        }
    }
}
