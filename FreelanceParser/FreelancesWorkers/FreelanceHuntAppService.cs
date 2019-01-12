using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

using FreelanceParser.Core.Model;
using FreelanceParser.Data;
using Telegram.Bot;

namespace FreelanceParser.FreelancesWorkers
{
    public sealed class FreelanceHuntAppService : BaseTasksAppService<FreelanceHuntItem>
    {
        public FreelanceHuntAppService(
            IDataContextFactory dataContextFactory,
            TelegramBotClient telegramBotClient)
            : base(dataContextFactory, telegramBotClient)
        {
        }

        protected override void SaveTasks(IEnumerable<FreelanceHuntItem> tasks)
        {
            using (FreelanceContext context = dataContextFactory.Create())
            {
                context.FreelanceHuntItems.AddRange(tasks);
                context.SaveChanges();
            }
        }

        protected override IEnumerable<FreelanceHuntItem> GetNewTasks(
             IEnumerable<FreelanceHuntItem> tasks)
        {
            var latestTaskId = GetLatestTaskIdInDatabase();
            var newTasks = tasks.Where(x => x.Id > latestTaskId);

            return newTasks;
        }

        protected override int GetLatestTaskIdInDatabase()
        {
            using (FreelanceContext context = dataContextFactory.Create())
            {
                var latestTaskId = context.FreelanceHuntItems
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.Id)
                    .FirstOrDefault();

                return latestTaskId;
            }
        }
    }
}