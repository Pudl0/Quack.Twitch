using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Quack.Net_Twitch
{
    public class TwitchBot
    {
        private readonly TwitchClient _twitchClient;
        private string _streamerName;

        public TwitchBot(string streamerName)
        {
            _streamerName = streamerName;
            ConnectionCredentials credentials = new ConnectionCredentials("Quak_der_Bruchpilot", Environment.GetEnvironmentVariable("QUACK_TWITCH_BOTTOKEN"));
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new WebSocketClient(clientOptions);

            _twitchClient = new TwitchClient(customClient);
            _twitchClient.Initialize(credentials);

            _twitchClient.OnJoinedChannel += Client_OnJoinedChannel;
            _twitchClient.OnMessageReceived += Client_OnMessageReceived;
            _twitchClient.OnConnected += Client_OnConnected;
            _twitchClient.OnUserJoined += Client_OnUserJoined;

            _twitchClient.Connect();
            Console.WriteLine("I am Connected");
        }

        public void JoinChannel(string streamerName)
        {
            _streamerName = streamerName;
            _twitchClient.JoinChannel(streamerName);
        }

        public void LeaveChannel(string streamerName)
        {
            _twitchClient.SendMessage(streamerName, "Munter bleiben!");
            _twitchClient.Disconnect();
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            _twitchClient.JoinChannel(_streamerName);
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            _twitchClient.SendMessage(e.Channel, "Vorsicht da unten!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            #region General commands

            if (e.ChatMessage.Message.Contains("!discord"))
                _twitchClient.SendMessage(e.ChatMessage.Channel, "Ab mit euch auf den Discord, ihr Nudeln! https://discord.gg/wAqQ9egPcw");

            if (e.ChatMessage.Message.Contains("!abgang"))
            {
                _twitchClient.SendMessage(e.ChatMessage.Channel, "Ich bin dann mal weg, bis dann!");
                _twitchClient.Disconnect();
            }

            if (e.ChatMessage.Message.Contains("!lurk"))
                _twitchClient.SendMessage(e.ChatMessage.Channel, e.ChatMessage.Username + " meldet sich ab und verkrümelt sich auf die Lurk-Couch!");

            if (e.ChatMessage.Message.Contains("!unlurk"))
                _twitchClient.SendMessage(e.ChatMessage.Channel, e.ChatMessage.Username + " kommt zurück von der Lurk Couch. Ich hoffe du hast die Sitzkuhle wieder rausgeklopft!");

            if (e.ChatMessage.Message.Contains("!joke"))
                TellRandomJoke(e.ChatMessage.Channel);
            #endregion
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            switch (e.Username)
            {
                case "nur_pat":
                    _twitchClient.SendMessage(e.Channel, "Moin Patrick! Schön, dass du da bist!");
                    break;
                case "finch_meister":
                    _twitchClient.SendMessage(e.Channel, "Charles der alte Plattenexperte reitet auf einer Basswelle in den Chat!");
                    break;
                case "t_goldi":
                    _twitchClient.SendMessage(e.Channel, "Irgendwas Pinkes");
                    break;
            }
        }

        private void TellRandomJoke(string channel)
        {
            var jsonstring = File.ReadAllText("./Resources/jokes.json");
            Dictionary<int,string> jokeList = JsonSerializer.Deserialize<Dictionary<int, string>>(jsonstring);
            var rng = new Random();
            var index = rng.Next(0, jokeList.Count - 1);
            _twitchClient.SendMessage(channel, jokeList[index]);
        }
    }
}
