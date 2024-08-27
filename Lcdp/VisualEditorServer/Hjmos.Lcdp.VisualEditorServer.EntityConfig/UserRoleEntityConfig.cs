using Hjmos.Lcdp.VisualEditorServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hjmos.Lcdp.VisualEditorServer.EntityConfig
{
    class UserRoleEntityConfig : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> entity)
        {
            entity.ToTable("user_role");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.RoleId)
                .HasColumnName("role_id")
                .HasComment("角色ID");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasComment("用户ID");
        }
    }
}
