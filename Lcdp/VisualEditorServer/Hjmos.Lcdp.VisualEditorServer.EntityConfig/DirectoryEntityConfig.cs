using Hjmos.Lcdp.VisualEditorServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hjmos.Lcdp.VisualEditorServer.EntityConfig
{
    class DirectoryEntityConfig : IEntityTypeConfiguration<Directory>
    {
        public void Configure(EntityTypeBuilder<Directory> entity)
        {
            entity.ToTable("directory");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AppId)
                .HasColumnName("app_id")
                .HasComment("应用ID");

            entity.Property(e => e.CreateBy)
                .HasColumnName("create_by")
                .HasComment("创建者");

            entity.Property(e => e.CreateTime)
                .HasColumnName("create_time")
                .HasComment("创建时间");

            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .HasColumnName("icon")
                .HasComment("图标");

            entity.Property(e => e.Level)
                .HasColumnName("level")
                .HasComment("目录等级");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name")
                .HasComment("目录名称");

            entity.Property(e => e.ParentId)
                .HasColumnName("parent_id")
                .HasComment("上级目录");

            entity.Property(e => e.ParentIds)
                .IsRequired()
                .HasMaxLength(2000)
                .HasColumnName("parent_ids")
                .HasComment("所有上级目录");

            entity.Property(e => e.Remarks)
                .IsRequired()
                .HasMaxLength(2000)
                .HasColumnName("remarks")
                .HasComment("备注");

            entity.Property(e => e.Sort)
                .HasColumnName("sort")
                .HasComment("排序");

            entity.Property(e => e.Status)
                .HasColumnType("tinyint")
                .HasColumnName("status")
                .HasDefaultValueSql("'1'")
                .HasComment("状态标记（0：隐藏，1：显示）");

            entity.Property(e => e.UpdateBy)
                .HasColumnName("update_by")
                .HasComment("更新者");

            entity.Property(e => e.UpdateTime)
                .HasColumnName("update_time")
                .HasComment("更新时间");
        }
    }
}
