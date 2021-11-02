using System;
using System.Net;

namespace HelperClasses
{
    public static class Webcalls
    {
        public class PasswordRequirements
        {
            
        }
        public static bool CheckUrl(string url)
        {
            try
            {
                //Creating the HttpWebRequest
                //Setting the Request method HEAD.
                if (WebRequest.Create(url) is HttpWebRequest request)
                {
                    request.Method = "HEAD";
                    //Getting the Web Response.
                    //Returns TRUE if the Status code == 200
                    if (request.GetResponse() is HttpWebResponse response)
                    {
                        response.Close();
                        return (response.StatusCode == HttpStatusCode.OK);
                    }
                }
                return false;
            }
            catch
            {
                //Any exception will returns false.
                return false;
            }
        }

        
        public class UploadParams
        {
            public string DocumentName { get; set; }
            public string PdfPath { get; set; }
            public string UploadUrl { get; set; }
            public Guid UserLibraryId { get; set; }
            public string username { get; set; }
            public string Password { get; set; }
            public string email { get; set; }
        }

        
        

        
    }
}
