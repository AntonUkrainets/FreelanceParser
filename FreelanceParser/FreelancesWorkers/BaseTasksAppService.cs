using FreelanceParser.Core;
using FreelanceParser.Core.Model;
using FreelanceParser.Data;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;

namespace FreelanceParser.FreelancesWorkers
{
    public abstract class BaseTasksAppService<T>
        where T : TaskItem
    {
        protected IDataContextFactory dataContextFactory;
        private TelegramBotClient telegramBotClient;

        public BaseTasksAppService(
            IDataContextFactory dataContextFactory,
            TelegramBotClient telegramBotClient
            )
        {
            this.dataContextFactory = dataContextFactory;
            this.telegramBotClient = telegramBotClient;
        }

        public void SaveNewTasks(IEnumerable<T> tasks)
        {
            var newTasks = GetNewTasks(tasks);

            SaveTasks(newTasks);
            SendNewTasksToTelegram(newTasks);
        }

        private IEnumerable<Client> GetClients()
        {
            using (FreelanceContext context = dataContextFactory.Create())
            {
                return context.Clients.ToArray();
            }
        }

        private async void SendNewTasksToTelegram(IEnumerable<T> newTasks)
        {
            foreach (var newTask in newTasks)
            {
                var message = newTask.Uri;

                var clients = GetClients();

                foreach (var client in clients)
                {
                    if (client.IsActive == true)
                        await telegramBotClient.SendTextMessageAsync(client.Id, message);
                }
            }
        }

        protected abstract void SaveTasks(IEnumerable<T> newTasks);
        protected abstract IEnumerable<T> GetNewTasks(IEnumerable<T> tasks);
        protected abstract int GetLatestTaskIdInDatabase();
    }
}