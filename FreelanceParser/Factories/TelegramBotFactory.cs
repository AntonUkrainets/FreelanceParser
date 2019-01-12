using Microsoft.Extensions.Configuration;
using System.IO;
using Telegram.Bot;

namespace FreelanceParser.Factories
{
    public class TelegramBotFactory
    {
        private string GetToken()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var token = config.GetConnectionString("TokenBotConnection");

            return token;
        }

        public TelegramBotClient Create()
        {
            var token = GetToken();

            var client = new TelegramBotClient(token);

            return client;
        }
    }
}
