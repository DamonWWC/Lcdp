using Hjmos.Lcdp.VisualEditorServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hjmos.Lcdp.VisualEditorServer.EntityConfig
{
    class MenuEntityConfig : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> entity)
        {
            entity.ToTable("menu");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Header)
                .HasMaxLength(255)
                .HasColumnName("header")
                .HasComment("标题");

            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .HasColumnName("icon")
                .HasComment("图标");

            entity.Property(e => e.MenuType)
                .HasColumnName("menu_type")
                .HasComment("菜单类型（0：叶子结点，1：分支结点）");

            entity.Property(e => e.ParentId)
                .HasColumnName("parent_id")
                .HasComment("父ID");

            entity.Property(e => e.Sort)
                .HasColumnName("sort")
                .HasComment("排序");

            entity.Property(e => e.Status)
                .HasColumnType("tinyint")
                .HasColumnName("status")
                .HasDefaultValueSql("'1'")
                .HasComment("状态标记（0：隐藏，1：显示）");

            entity.Property(e => e.TargetView)
                .HasMaxLength(255)
                .HasColumnName("target_view")
                .HasComment("关联视图");
        }
    }
}
