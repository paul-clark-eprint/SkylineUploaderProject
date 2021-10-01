using System;
using System.Linq;
using System.Windows.Forms;

namespace SkylineUploader
{
    static class Program
    {
        public static PricingService.PricingService PricingService;
        public static SkylineWebService.SkylineWebService SkylineService;
        //public static Guid UserId;
        //public static Guid PortalId;
        //public static string PortalUrl;
        //public static bool ValidUser;
        //public static bool GlobalUser;
        //public static bool GlobalProducts;
        public static bool VersionOk;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmUploader());
        }
    }
}