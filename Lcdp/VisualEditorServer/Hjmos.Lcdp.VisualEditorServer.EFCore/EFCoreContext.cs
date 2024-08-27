using Hjmos.Lcdp.VisualEditorServer.Entities;
using Hjmos.Lcdp.VisualEditorServer.EntityConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Globalization;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditorServer.EFCore
{
    /// <summary>
    /// EFCore数据连接上下文对象
    /// </summary>
    public partial class EFCoreContext : DbContext
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        private readonly string _conn = string.Empty;

        public EFCoreContext(string connectionStr) => _conn = connectionStr;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseMySQL(_conn);
        // UseSnakeCaseNamingConvention是把Pascal命名转为Snake命名的方法（AppId->app_id）,EntityConfig库中每个表都有重新指定名称的，这个方法可用可不用
        //.UseSnakeCaseNamingConvention();

        /// <summary>
        /// 数据库映射配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 联合主键设置
            modelBuilder.Entity<RoleMenu>().HasKey(pk => new { pk.MenuId, pk.RoleId });
            modelBuilder.Entity<UserRole>().HasKey(pk => new { pk.UserId, pk.RoleId });

            // 菜单表中字体图标值转换
            ValueConverter iconValueConverter = new ValueConverter<string, string>(
                v => string.IsNullOrEmpty(v) ? null : ((int)v.ToArray()[0]).ToString("x"),
                v => v == null ? string.Empty : ((char)int.Parse(v, NumberStyles.HexNumber)).ToString());
            modelBuilder.Entity<Menu>().Property(p => p.Icon).HasConversion(iconValueConverter);
            modelBuilder.Entity<Directory>().Property(p => p.Icon).HasConversion(iconValueConverter);
            modelBuilder.Entity<File>().Property(p => p.Icon).HasConversion(iconValueConverter);

            // 加载EntityConfig程序集中的所有配置
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppEntityConfig).Assembly);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<User> User { get; set; }

        public DbSet<Role> Role { get; set; }

        public DbSet<Menu> Menu { get; set; }

        public DbSet<RoleMenu> RoleMenu { get; set; }

        public DbSet<UserRole> UserRole { get; set; }

        public DbSet<Directory> Directory { get; set; }

        public DbSet<File> File { get; set; }

        public DbSet<Widget> Widget { get; set; }

        public DbSet<WidgetLib> WidgetLib { get; set; }

        public DbSet<Parameter> Parameter { get; set; }

        public DbSet<App> App { get; set; }
    }
}
