using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SkylineUploaderService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///
        
        public static SkylineUploader.PricingService.PricingService PricingService;
        public static SkylineUploader.SkylineWebService.SkylineWebService SkylineService;

        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SkylineUploaderService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
