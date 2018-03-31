using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace discord_cli
{
    class Program
    {
        SocketGuild currentGuild;
        SocketChannel currentChannel;
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

        Task Connected()
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
                switch (command)
                {
                    case "exit":
                        Environment.Exit(0);
                    break;
                    case "list":
                    {
                        int i = 1;
                        foreach (SocketGuild guild in client.Guilds)
                        {
                            Console.WriteLine("[" + i + "] " + guild.Name);
                            i++;
                        }
                    }
                    break;
                    case "list channels":
                    {
                        int i = 1;
                        try
                        {
                            foreach (SocketChannel channel in currentGuild.Channels)
                            {
                                Console.WriteLine("[" + i + "] " + channel);
                                i++;
                            }
                        }
                        catch
                        {
                            Console.WriteLine("No guild selected, type \"help\" for information");
                        }
                    }
                    break;
                    case "select":
                    {

                    }
                    break;
                }
            }
            return Task.CompletedTask;
        }

        Task MessageReceived(SocketMessage message)
        {
            Console.WriteLine(message.Author.Username + "#" + message.Author.Discriminator + ": " + message.Content);
            return Task.CompletedTask;
        }
    }
}
