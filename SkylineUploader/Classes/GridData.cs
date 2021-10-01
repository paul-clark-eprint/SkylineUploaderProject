using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylineUploader.Classes
{
    public class GridData
    {
        public Guid FolderId { get; set; }
        public Guid PortalId { get; set; }
        public string PortalUrl { get; set; }
        public string FolderName { get; set; }
        public string LibraryUsername { get; set; }
        public string LibraryName { get; set; }
        public int Files { get; set; }
        public string Status { get; set; }
        public bool Enabled { get; set; }
        public string SourceFolder { get; set; }
    }
}
