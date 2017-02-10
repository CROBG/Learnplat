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

        /*Private object used for savekeeping the listenObj data after refreshing the page
         */
        private MainModel originalFacher = new MainModel();

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

            MainModel MainObj1 = new MainModel();

            /*Loads the data from the csv File
             */
            MainObj1.lernplan = headerObj.DataLoader(Server.MapPath(Pfad));

            /*Calculates the timeusage for every Day
             */
            MainObj1.zeitverbracht = headerObj.VebrauchsCalculations(MainObj1.lernplan);

            /*Groups the Name values from the LernplanModel list and creates a Subject list
             */
            MainObj1.grupiertFach = MainObj1.lernplan.Where(o => o.Name.Contains("T2"))
                                                   .GroupBy(o => o.Name)
                                                   .Select(grp => grp.FirstOrDefault())
                                                   .ToList();

            /*Safespace for keeping the MainObj Object for safekeeping
             */

            originalFacher = MainObj1;

            return View(MainObj1);
        }

        [HttpPost]
        public ActionResult Index(MainModel MainObj2)
        {
            /*Object reference to the HomeHeader
             */
            HomeHeader headerObj = new HomeHeader();

            MainObj2.zeitverbracht = originalFacher.zeitverbracht;

            int Gewichtungscounter = 0;

            /*Iterates the grupped Subjects and adds the Weight values given back from the View
             */
            for (int i = 0; i < MainObj2.grupiertFach.Count; i++)
                Gewichtungscounter += MainObj2.grupiertFach[i].Gewichtung;

            /*If nothing has been entered as a values, but the Site was refreshed the Counter will be set to the number of Subjects in the list
             * This prevents the Method from dividing with zero
             */
            if (Gewichtungscounter == 0)
                Gewichtungscounter = MainObj2.grupiertFach.Count();

            /*Adds the available Lerneinheiten to the Iterated Subject based on his weight
             */
            for (int i = 0; i < MainObj2.grupiertFach.Count; i++)
                originalFacher.grupiertFach[i].LerneinheitenZahl = LehreinheitenCount * MainObj2.grupiertFach[i].Gewichtung / Gewichtungscounter;

            originalFacher = headerObj.LerneinheitenVerteiler(originalFacher);
            /*Splits the Subjects on the Days
             */
            MainObj2 = originalFacher;

            SafeToRefresh = true;

            return View(originalFacher);
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