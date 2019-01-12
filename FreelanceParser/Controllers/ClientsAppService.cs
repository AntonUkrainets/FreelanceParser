using System.Linq;
using FreelanceParser.Core.Model;
using FreelanceParser.Data;

namespace FreelanceParser.Controllers
{
    public class ClientsAppService
    {
        private readonly IDataContextFactory dataContextFactory;

        public ClientsAppService(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        public void ProcessUserCommand(int userId, string userName, string userCommand)
        {
            BotCommand command = GetCommandType(userCommand);

            switch (command)
            {
                case BotCommand.Start:
                    SubscribeUser(userId, userName);
                    break;

                case BotCommand.Stop:
                    UnsubscribeUser(userId, userName);
                    break;
            }
        }

        private BotCommand GetCommandType(string stringCommand)
        {
            switch (stringCommand)
            {
                case "/start":
                    return BotCommand.Start;
                case "/stop":
                    return BotCommand.Stop;
                default:
                    return BotCommand.NonCommand;
            }
        }

        private void SubscribeUser(int userId, string userName)
        {
            using (FreelanceContext context = dataContextFactory.Create())
            {
                if (context.Clients.Any(x => x.Id == userId && x.IsActive == true))
                    return;

                Client newClient = new Client
                {
                    Id = userId,
                    Name = userName,
                    IsActive = true
                };

                bool userExists = context.Clients.Any(x => x.Id == userId);
                if(userExists)
                {
                    context.Clients.Update(newClient);
                }
                else
                {
                    context.Clients.Add(newClient);
                }

                context.SaveChanges();
            }
        }

        private void UnsubscribeUser(int userId, string userName)
        {
            using (FreelanceContext context = dataContextFactory.Create())
            {
                if (context.Clients.Any(x => x.Id == userId && x.IsActive == false))
                    return;

                if (!context.Clients.Any(x => x.Id == userId))
                    return;

                Client client = new Client
                {
                    Id = userId,
                    Name = userName,
                    IsActive = false
                };

                context.Clients.Update(client);
                context.SaveChanges();
            }
        }
    }
}
