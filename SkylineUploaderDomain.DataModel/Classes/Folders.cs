using System;
using System.Collections.Generic;
using SkylineUploaderDomain.DataModel.Enums;

namespace SkylineUploaderDomain.DataModel.Classes
{
   public class Folder
    {
        public int Id { get; set; }
        public Guid FolderId { get; set; }
        public Guid PortalId { get; set; }
        public string FolderName { get; set; }

        //[Required]
        public Login Login { get; set; }
        //[Required]
        public UserLibrary UserLibrary { get; set; }
        //[Required]
        public SourceFolder SourceFolder { get; set; }
        public FolderStatus Status { get; set; }
        //[Required]
        public List<DocumentType> DocumentTypes { get; set; }
        public bool Enabled { get; set; }
        public bool SynchronizeFiles { get; set; }
        public List<SynchronizedFile> SynchronizedFiles { get; set; }
        public int DeleteAfterDays { get; set; }
        public bool HideOnOrder { get; set; }
        public bool DeleteAfterUpload { get; set; }
        public bool InEditMode{get;set;}
    }

    public class Login
    {
        public int Id { get; set; }
        public Guid FolderId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PortalUrl { get; set; }
    }
    public class Proxy
    {
        public int Id { get; set; }
        public bool UseProxy { get; set; }
        public string ProxyAddress { get; set; }
        public int ProxyPort { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public string ProxyDomain { get; set; }
    }

    public class UserLibrary
    {
        public int Id { get; set;}
        public Guid FolderId { get; set; }
        public string Username { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public string LibraryName { get; set; }
        public Guid LibraryId { get; set; }
    }

    public class SourceFolder
    {
        public int Id { get; set; }
        public Guid FolderId { get; set; }
        public string FolderPath { get; set; }
        public List<DocumentType> SupportedDocumentTypes { get; set; }
        public bool Enabled { get; set; }
        public int FileCount { get; set; }
    }

    public class SynchronizedFile
    {
        public bool Id { get; set; }
        public Guid FolderId { get; set; }
        public string DocumentName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateAccessed { get; set; }
        public int Size { get; set; }
    }

    public class DocumentType
    {
        public int Id { get; set; }
        public Guid FolderId { get; set; }
        public string FileType { get; set; }
    }

    
}