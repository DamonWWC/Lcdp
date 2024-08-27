using Hjmos.Lcdp.VisualEditorServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hjmos.Lcdp.VisualEditorServer.EntityConfig
{
    class FileEntityConfig : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> entity)
        {
            entity.ToTable("file");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AppId)
                .IsRequired()
                .HasColumnName("app_id")
                .HasComment("应用ID");

            entity.Property(e => e.Category)
                .HasColumnName("category")
                .HasComment("文件种类");

            entity.Property(e => e.Content)
                .HasColumnType("longtext")
                .HasColumnName("content")
                .HasComment("文件内容");

            entity.Property(e => e.CreateBy)
                .HasColumnName("create_by")
                .HasComment("创建者");

            entity.Property(e => e.CreateTime)
                .HasColumnName("create_time")
                .HasComment("创建时间");

            entity.Property(e => e.FolderId)
                .HasColumnName("folder_id")
                .HasComment("目录ID");

            entity.Property(e => e.Guid)
                .HasMaxLength(36)
                .HasColumnName("guid")
                .IsFixedLength(true)
                .HasComment("页面文件Guid");

            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .HasColumnName("icon")
                .HasComment("图标");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name")
                .HasComment("文件名称");

            entity.Property(e => e.Path)
                .HasMaxLength(2000)
                .HasColumnName("path")
                .HasComment("文件路径");

            entity.Property(e => e.Sort)
                .HasColumnName("sort")
                .HasComment("排序");

            entity.Property(e => e.Status)
                .HasColumnType("tinyint")
                .HasColumnName("status")
                .HasDefaultValueSql("'1'")
                .HasComment("状态标记（0：隐藏，1：显示）");

            entity.Property(e => e.Suffix)
                .HasMaxLength(50)
                .HasColumnName("suffix")
                .HasComment("文件后缀");

            entity.Property(e => e.UpdateBy)
                .HasColumnName("update_by")
                .HasComment("更新者");

            entity.Property(e => e.UpdateTime)
                .HasColumnName("update_time")
                .HasComment("更新时间");
        }
    }
}
