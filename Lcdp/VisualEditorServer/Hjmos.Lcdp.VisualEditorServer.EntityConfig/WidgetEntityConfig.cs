using Hjmos.Lcdp.VisualEditorServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hjmos.Lcdp.VisualEditorServer.EntityConfig
{
    class WidgetEntityConfig : IEntityTypeConfiguration<Widget>
    {
        public void Configure(EntityTypeBuilder<Widget> entity)
        {
            entity.ToTable("widget");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .HasColumnName("category")
                .HasComment("类别");

            entity.Property(e => e.DisplayName)
                .HasMaxLength(255)
                .HasColumnName("display_name")
                .HasComment("显示名称");

            entity.Property(e => e.FullTypeName)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("full_type_name")
                .HasComment("组件类型全名");

            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .HasColumnName("icon")
                .HasComment("图标");

            entity.Property(e => e.LibId)
                .HasColumnName("lib_id")
                .HasComment("组件类库ID");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name")
                .HasComment("组件类名");

            entity.Property(e => e.RenderAsSample)
                .HasColumnName("render_as_sample")
                .HasComment("是否渲染成样例（0：否，1：是）");

            entity.Property(e => e.SampleFullName)
                .HasMaxLength(255)
                .HasColumnName("sample_full_name")
                .HasComment("样例组件类型全名");
        }
    }
}
