using Hjmos.Lcdp.VisualEditorServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hjmos.Lcdp.VisualEditorServer.EntityConfig
{
    class WidgetLibEntityConfig : IEntityTypeConfiguration<WidgetLib>
    {
        public void Configure(EntityTypeBuilder<WidgetLib> entity)
        {
            entity.ToTable("widget_lib");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.MD5)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("md5")
                .HasComment("文件MD5校验码");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name")
                .HasComment("程序集名称");

            entity.Property(e => e.Path)
                .HasMaxLength(2000)
                .HasColumnName("path")
                .HasComment("程序集存放路径");

            entity.Property(e => e.Status)
                .HasColumnType("tinyint")
                .HasColumnName("status")
                .HasDefaultValueSql("'1'")
                .HasComment("状态标记（0：删除，1：有效）");

            entity.Property(e => e.UploadTime)
                .HasColumnName("upload_time")
                .HasComment("上传时间");
        }
    }
}
