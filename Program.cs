using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace discord_cli
{
    class Program
    {
        SocketChannel currentChannel;
        DiscordSocketClient client = new DiscordSocketClient();

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            string token = "";

            client.Connected += Connected;

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
            Thread.Sleep(1000);
            Console.WriteLine("Logged in as " + client.CurrentUser.Username + "#" + client.CurrentUser.Discriminator);
            Console.WriteLine("Type \"help\" or \"?\" for a list of commands");
            switch (Console.ReadLine())
            {
                case "list":
                    int i = 1;
                    foreach (SocketGuild guild in client.Guilds)
                    {
                        Console.WriteLine("[" + i + "] " + guild.Name);
                        i++;
                    }
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
