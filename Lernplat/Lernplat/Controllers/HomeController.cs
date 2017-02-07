using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Globalization;
using System.Web.Mvc;
using Lernplat.Models;

namespace Lernplat.Controllers
{
    public class HomeController : Controller, Interfaces.IHomeController
    {
        /*Contains a filepath to the csv file
         */
        public static string Pfad = "~/LernplanFiles/Stundenplan.csv";

        /*Object reference to the Model containing all the List Variables
         */
        public static Listen listenObj = new Listen();

        /*Private object used for savekeeping the listenObj data after refreshing the page
         */
        private static Listen originalFacher = new Listen();

        /*Counter for the Lerneinheiten value
         */
        public static int LehreinheitenCount = 0;

        /*Boolean used for checking if the site has been refreshed for the first time
         */
        public static bool SafeToRefresh = false;

        public ActionResult Index()
        {
            /*Object reference to the Header File containing the Methods used by the Controller
             */
            HomeHeader headerObj = new HomeHeader();

            /*Loads the data from the csv File
             */
            listenObj.lernplan = headerObj.DataLoader(Server.MapPath(Pfad));

            /*Calculates the timeusage for every Day
             */
            listenObj.zeitverbracht = headerObj.VebrauchsCalculations(listenObj.lernplan);


            /*Groups the Name values from the LernplanModel list and creates a Subject list
             */
            listenObj.grupiertFach = listenObj.lernplan.Where(o => o.Name.Contains("T2"))
                                                       .GroupBy(o => o.Name)
                                                       .Select(grp => grp.FirstOrDefault())
                                                       .ToList();

            /*Safespace for keeping the listenObj Object for safekeeping
             */
            originalFacher = listenObj;

            return View(new GewichtungsModel());
        }

        [HttpPost]
        public ActionResult Index(GewichtungsModel gew)
        {
            /*Object reference to the HomeHeader
             */
            HomeHeader headerObj = new HomeHeader();

            /*Checks if the site was refreshed for the first time
             */
            if (!SafeToRefresh)
            {
                listenObj = originalFacher;

                int Gewichtungscounter = 0;

                /*Iterates the grupped Subjects and adds the Weight values given back from the View
                 */
                for (int i = 0; i < listenObj.grupiertFach.Count; i++)
                {
                    listenObj.grupiertFach[i].Gewichtung = gew.Gewichtung[i];
                    Gewichtungscounter += gew.Gewichtung[i];
                }

                /*If nothing has been entered as a values, but the Site was refreshed the Counter will be set to the number of Subjects in the list
                 * This prevents the Method from dividing with zero
                 */
                if (Gewichtungscounter == 0) Gewichtungscounter = listenObj.grupiertFach.Count();

                /*Adds the available Lerneinheiten to the Iterated Subject based on his weight
                 */
                for (int i = 0; i < listenObj.grupiertFach.Count; i++)
                    listenObj.grupiertFach[i].LerneinheitenZahl = LehreinheitenCount * gew.Gewichtung[i] / Gewichtungscounter;

                /*Splits the Subjects on the Days
                 */
                listenObj = headerObj.LerneinheitenVerteiler(listenObj);

                SafeToRefresh = true;
            }

            return View(gew);
        }

        private string FDir_AppData = "~/PDFs/";

        public ActionResult PDF(String FileName)
        {
            var slapath = Server.MapPath(FDir_AppData + FileName + ".pdf");

            /*Crazy shit happens here
             * Can´t explain
             */
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "inline;filename=" + slapath);
            Response.ContentType = "application/pdf";
            Response.WriteFile(slapath);
            Response.Flush();
            Response.Clear();
            return View();
        }
    }
}