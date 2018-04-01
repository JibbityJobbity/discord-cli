using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.WebSocket;

namespace discord_cli
{
    class Program
    {
        string prefix = ";";
        SocketGuild currentGuild;
        ISocketMessageChannel currentChannel;
        DiscordSocketClient client = new DiscordSocketClient();

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            string token = "";

            client.Connected += Connected;
            client.MessageReceived += MessageReceived;
            
            Console.Write("Please enter your Discord token: ");
            token = Console.ReadLine();
            Console.Clear();
            await client.LoginAsync(TokenType.User, token);
            Console.Write("Connecting");
            client.StartAsync();
            while (client.ConnectionState != ConnectionState.Connected)
            {
                Thread.Sleep(1000);
                Console.Write(".");
            }
            Console.Clear();
            Console.WriteLine("Connected");

            await Task.Delay(-1);
        }

        async Task Connected()
        {
            string input;
            string[] inputComponents;
            string command;
            string[] arguments;
            Thread.Sleep(1000);
            Console.WriteLine("Logged in as " + client.CurrentUser.Username + "#" + client.CurrentUser.Discriminator);
            Console.WriteLine("Type \"help\" or \"?\" for a list of commands");

            while (client.ConnectionState == ConnectionState.Connected)
            {
                input = Console.ReadLine();
                inputComponents = input.Split(" ");
                command = inputComponents[0];
                arguments = new string[inputComponents.Length - 1];

                for (int i = 0; i < inputComponents.Length - 1; i++)
                {
                    arguments[i] = inputComponents[i + 1];
                }
                if (command.StartsWith(prefix))
                {
                    if (command == prefix + "exit")
                        Environment.Exit(0);
                    else if (command == prefix + "list")
                    {
                        if (arguments[0] == "servers")
                        {
                            int i = 1;
                            foreach (SocketGuild guild in client.Guilds)
                            {
                                Console.WriteLine("[" + i + "] " + guild.Name);
                                i++;
                            }
                        }
                        else if (arguments[0] == "channels")
                        {
                            try
                            {
                                for (int i = 0; i < currentGuild.TextChannels.Count; i++)
                                {
                                    Console.WriteLine("[" + (i + 1) + "] " + ((ISocketMessageChannel)currentGuild.TextChannels.Skip(i).First()).Name);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                Console.WriteLine("No server selected.");
                                Console.WriteLine("Enter \"list servers\" then \"select server <server number>");
                            }
                        }
                    }
                    else if (command == prefix + "select")
                    {
                        int selection;
                        if (!int.TryParse(arguments[1], out selection))
                        {
                            Console.WriteLine("Invalid server selection.");
                            Console.WriteLine("Usage: select server <server number>");
                        }
                        if (arguments[0] == "server")
                        {
                            currentGuild = client.Guilds.Skip(selection - 1).First();
                            Console.WriteLine("Selected " + currentGuild.Name);
                        }
                        else if (arguments[0] == "channel")
                        {
                            currentChannel = currentGuild.TextChannels.Skip(selection - 1).First();
                            Console.WriteLine("Selected " + currentChannel.Name);
                        }
                    }
                }
                else
                    await currentChannel.SendMessageAsync(input);
            }
            return;
        }

        Task MessageReceived(SocketMessage message)
        {
            if (message.Channel == currentChannel)
                Console.WriteLine(message.Author.Username + "#" + message.Author.Discriminator + ": " + message.Content);
            return Task.CompletedTask;
        }
    }
}
