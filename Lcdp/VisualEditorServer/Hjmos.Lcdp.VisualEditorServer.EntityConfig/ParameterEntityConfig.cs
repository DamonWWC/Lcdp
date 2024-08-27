using Hjmos.Lcdp.VisualEditorServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hjmos.Lcdp.VisualEditorServer.EntityConfig
{
    class ParameterEntityConfig : IEntityTypeConfiguration<Parameter>
    {
        public void Configure(EntityTypeBuilder<Parameter> entity)
        {
            entity.ToTable("parameter");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AppId)
                .HasColumnName("app_id")
                .HasComment("应用ID");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name")
                .HasComment("变量名称");

            entity.Property(e => e.Range)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("range")
                .HasComment("使用范围");

            entity.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(2000)
                .HasColumnName("value")
                .HasComment("值");
        }
    }
}
