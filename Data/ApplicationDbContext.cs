using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SPSB.Models;

namespace SPSB.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Venda>().HasMany(i => i.Itens).WithOne(i => i.Venda).HasForeignKey(i => i.VendaId);
            base.OnModelCreating(builder);
        }

        public DbSet<Produto> Produto { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<Venda> Venda { get; set; }
        public DbSet<Notificacao> Notificacao { get; set; }
    }
}
