using FreelanceParser.Core.Model;
using FreelanceParser.Data;
using System.Collections.Generic;
using System.Linq;

namespace FreelanceParser
{
    public class DatabaseWorker
    {
        private readonly IDataContextFactory dataContextFactory;

        public DatabaseWorker(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        public void EnsureDatabase()
        {
            using (FreelanceContext context = dataContextFactory.Create())
            {
                context.Database.EnsureCreated();
            }
        }

        public IEnumerable<Client> GetClientsFromDatabase()
        {
            using (FreelanceContext context = dataContextFactory.Create())
            {
                return context.Clients.ToArray();
            }
        }
    }
}