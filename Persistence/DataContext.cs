using Domain;
using Domain.DanhMuc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, Int64>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) // phải là DbContextOptions<DataContext>
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName != null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            ////trigger
            //builder.Entity<OD_AdminMenu>()
            //    .ToTable(tb => tb.HasTrigger("trg_Insert_Menu"));

            //builder.Entity<OD_AdminMenu>()
            //    .ToTable(tb => tb.HasTrigger("trg_Delete_Menu"));
        }

        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<CSDL_AdminMenu> CSDL_AdminMenu { get; set; }
        public DbSet<CSDL_MenuPermission> CSDL_MenuPermission { get; set; }
        public DbSet<DM_LinhVuc> DM_LinhVuc { get; set; }
        public DbSet<CSDL_Log> CSDL_Log { get; set; }
    }
}
