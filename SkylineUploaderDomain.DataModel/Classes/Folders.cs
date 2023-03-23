using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkylineUploaderDomain.DataModel.Classes
{
   public class Folder
    {
        public int Id { get; set; }
        public Guid FolderId { get; set; }
        public Guid PortalId { get; set; }
        [StringLength(1000)]
        public string FolderName { get; set; }

        //[Required]
        public Login Login { get; set; }
        //[Required]
        public UserLibrary UserLibrary { get; set; }
        //[Required]
        public SourceFolder SourceFolder { get; set; }

        [StringLength(200)]
        public string Status { get; set; }
        public int Files { get; set; }
        public bool Enabled { get; set; }
        public bool SynchronizeFiles { get; set; }
        public List<SynchronizedFile> SynchronizedFiles { get; set; }
        public int DeleteAfterDays { get; set; }
        public bool HideOnOrder { get; set; }
        public bool DeleteAfterUpload { get; set; }
        public bool InEditMode { get; set; }
        public bool WaitForXml { get; set; }
        public bool EmailUser { get; set; }
        public DateTime DateUpdated { get; set; }
        [StringLength(1000)]
        public string FileType { get; set; }

    }

    public class Login
    {
        public int Id { get; set; }
        public Guid FolderId { get; set; }
        [StringLength(1000)]
        public string Username { get; set; }
        [StringLength(1000)]
        public string Password { get; set; }
        [StringLength(1000)]
        public string PortalUrl { get; set; }
    }
    public class Proxy
    {
        public int Id { get; set; }
        public bool UseProxy { get; set; }
        [StringLength(1000)]
        public string ProxyAddress { get; set; }
        public int ProxyPort { get; set; }
        [StringLength(1000)]
        public string ProxyUsername { get; set; }
        [StringLength(1000)]
        public string ProxyPassword { get; set; }
        [StringLength(1000)]
        public string ProxyDomain { get; set; }
    }

    public class UserLibrary
    {
        public int Id { get; set;}
        public Guid FolderId { get; set; }
        [StringLength(1000)]
        public string Username { get; set; }
        public Guid UserId { get; set; }
        [StringLength(1000)]
        public string UserEmail { get; set; }
        [StringLength(1000)]
        public string LibraryName { get; set; }
        public Guid LibraryId { get; set; }
    }

    public class SourceFolder
    {
        public int Id { get; set; }
        public Guid FolderId { get; set; }
        [StringLength(1000)]
        public string FolderPath { get; set; }
        public bool Enabled { get; set; }
        public int FileCount { get; set; }
    }

    public class SynchronizedFile
    {
        public bool Id { get; set; }
        public Guid FolderId { get; set; }
        [StringLength(1000)]
        public string DocumentName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateAccessed { get; set; }
        public int Size { get; set; }
    }

    public class ServiceSettings
    {
        [Key]
        public int Id { get; set; }
        public DateTime LastUpdate { get; set; }
        [StringLength(1000)] 
        public string ServiceMessage { get; set; }
        public int Progress { get; set; }
        public int ProgressMaximum { get; set; }
        public bool Running { get; set; }
        public bool Uploading { get; set; }
        public bool Transferring { get; set; }
    }
}