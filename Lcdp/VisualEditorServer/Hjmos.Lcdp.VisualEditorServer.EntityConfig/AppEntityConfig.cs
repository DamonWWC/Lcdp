using Hjmos.Lcdp.VisualEditorServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hjmos.Lcdp.VisualEditorServer.EntityConfig
{
    public class AppEntityConfig : IEntityTypeConfiguration<App>
    {
        public void Configure(EntityTypeBuilder<App> entity)
        {
            entity.ToTable("app");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.CreateBy)
                .HasColumnName("create_by")
                .HasComment("创建者");

            entity.Property(e => e.CreateTime)
                .HasColumnName("create_time")
                .HasComment("创建时间");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name")
                .HasComment("应用名称");

            entity.Property(e => e.RootDir)
                .HasColumnName("root_dir")
                .HasComment("应用根目录");

            entity.Property(e => e.Status)
                .HasColumnType("tinyint")
                .HasColumnName("status")
                .HasDefaultValueSql("'1'")
                .HasComment("状态标记（0：删除，1：有效）");

            entity.Property(e => e.UpdateBy)
                .HasColumnName("update_by")
                .HasComment("更新者");

            entity.Property(e => e.UpdateTime)
                .HasColumnName("update_time")
                .HasComment("更新时间");
        }
    }
}
