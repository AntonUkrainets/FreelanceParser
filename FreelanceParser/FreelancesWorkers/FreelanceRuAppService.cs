using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

using FreelanceParser.Core.Model;
using FreelanceParser.Data;
using Telegram.Bot;

namespace FreelanceParser.FreelancesWorkers
{
    public sealed class FreelanceRuAppService : BaseTasksAppService<FlRuItem>
    {
        public FreelanceRuAppService(
            IDataContextFactory dataContextFactory,
            TelegramBotClient telegramBotClient)
            : base(dataContextFactory, telegramBotClient)
        {
        }

        protected override void SaveTasks(IEnumerable<FlRuItem> newTasks)
        {
            using (FreelanceContext context = dataContextFactory.Create())
            {
                context.FlRuItems.AddRange(newTasks);
                context.SaveChanges();
            }
        }

        protected override IEnumerable<FlRuItem> GetNewTasks(IEnumerable<FlRuItem> tasks)
        {
            var latestTaskId = GetLatestTaskIdInDatabase();
            var newTasks = tasks.Where(x => x.Id > latestTaskId);

            return newTasks;
        }

        protected override int GetLatestTaskIdInDatabase()
        {
            using (FreelanceContext context = dataContextFactory.Create())
            {
                var latestTaskId = context.FlRuItems
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.Id)
                    .FirstOrDefault();

                return latestTaskId;
            }
        }
    }
}