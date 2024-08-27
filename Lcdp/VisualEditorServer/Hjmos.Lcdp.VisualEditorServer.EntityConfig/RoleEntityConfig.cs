using Hjmos.Lcdp.VisualEditorServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hjmos.Lcdp.VisualEditorServer.EntityConfig
{
    class RoleEntityConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> entity)
        {
            entity.ToTable("role");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name")
                .HasComment("角色名称");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValueSql("'1'")
                .HasComment("状态标记（0：删除，1：有效）");
        }
    }
}