using System;

namespace SkylineUploader.Classes
{
    public class GridData
    {
        public Guid FolderId { get; set; }
        public Guid PortalId { get; set; }
        public string PortalUrl { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
        public string FolderName { get; set; }
        public string LibraryUsername { get; set; }
        public string LibraryName { get; set; }
        public Guid LibraryId { get; set; }
        public Guid LibraryUserId { get; set; }
        public int Files { get; set; }
        public string Status { get; set; }
        public bool Enabled { get; set; }
        public string SourceFolder { get; set; }
        public bool InEditMode { get; set; }
        public bool DeleteAfterUpload { get; set; }
        public string FileTypes { get; set; }
    }

    public class ServiceSettings
    {
        public bool ServiceRunning { get; set; }
        public string ServiceMessage { get; set; }
    }
}
