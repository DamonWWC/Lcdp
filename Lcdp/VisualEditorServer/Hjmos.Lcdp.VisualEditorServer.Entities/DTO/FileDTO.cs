using Hjmos.Lcdp.VisualEditorServer.Entities.Enums;

namespace Hjmos.Lcdp.VisualEditorServer.Entities.DTO
{
    public class FileDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ParentId { get; set; }

        public string Icon { get; set; }

        public int Sort { get; set; }

        public int? Level { get; set; }

        public string Suffix { get; set; }

        public FileType FileType { get; set; }

        public string Guid { get; set; }
    }
}
