using Hjmos.Lcdp.VisualEditorServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hjmos.Lcdp.VisualEditorServer.EntityConfig
{
    class RoleMenuEntityConfig : IEntityTypeConfiguration<RoleMenu>
    {
        public void Configure(EntityTypeBuilder<RoleMenu> entity)
        {
            entity.ToTable("role_menu");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.MenuId)
                .HasColumnName("menu_id")
                .HasComment("菜单ID");

            entity.Property(e => e.RoleId)
                .HasColumnName("role_id")
                .HasComment("角色ID");
        }
    }
}
