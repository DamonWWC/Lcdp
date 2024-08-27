using Hjmos.Lcdp.VisualEditorServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hjmos.Lcdp.VisualEditorServer.EntityConfig
{
    class UserEntityConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("user");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name")
                .HasComment("用户名");

            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("password")
                .HasComment("密码");

            entity.Property(e => e.Age)
                .HasColumnName("age")
                .HasComment("年龄");

            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .HasColumnName("icon")
                .HasComment("头像");

            entity.Property(e => e.Status)
                .HasColumnType("tinyint")
                .HasColumnName("status")
                .HasDefaultValueSql("'1'")
                .HasComment("状态标记（0：删除，1：有效）");

            entity.Ignore(e => e.Menus);
        }
    }
}
