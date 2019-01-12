using FreelanceParser.Core;
using FreelanceParser.Core.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace FreelanceParser.Data
{
    public class FreelanceContext : DbContext
    {
        public DbSet<FreelanceHuntItem> FreelanceHuntItems { get; set; }
        public DbSet<FlRuItem> FlRuItems { get; set; }

        public DbSet<Client> Clients { get; set; }

        public FreelanceContext(DbContextOptions<FreelanceContext> options)
            : base(options)
        {
        }
    }
}