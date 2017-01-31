using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Lernplat.Models;

namespace Lernplat.Controllers
{
    public class HomeHeader
    {
        /// <summary>
        /// Creates a calculated ZeitverbrachtModel list for the given LernplanModel list
        /// </summary>
        /// <param name="lernplan">LernplanModel list</param>
        /// <returns></returns>
        public static List<ZeitsverbrachtModel> VebrauchsCalculations(List<LernplanModel> lernplan)
        {
            List<ZeitsverbrachtModel> zeitverbracht = new List<ZeitsverbrachtModel>();

            for (int i = 0; i < lernplan.Count() - 1; i++)
            {
                DateTime dt = new DateTime();
                TimeSpan ts = new TimeSpan();

                /*Minimal posible TimeSpan value
                 */
                TimeSpan nullVerbrauch = new TimeSpan(0, 0, 0);

                /*Threshold for 3 instances of the Lerneinheit value
                 */
                TimeSpan mindestVerbrauch = new TimeSpan(7, 29, 0);

                /*Integer containing the number of "Lerneinheiten pro Tag"
                 */
                int lerneinheit;

                /*Skips the current Iteration for the given filtered Subject
                 * Filtered subject won´t be counted into he working hours
                 */
                if (lernplan[i].Name == "Repetitorium Mathematik")
                    continue;

                /*Sets the Timespan ts to 0 for the given filtered Subject
                 */
                if (lernplan[i].Name == "Studium flexibel" || lernplan[i].Name == "Praxisphase" || lernplan[i].Name == "Weihnachten" || lernplan[i].Name == "Neujahr")
                {
                    dt = lernplan[i].Beginn.Date;
                    ts = new TimeSpan(0, 0, 0);
                }
                /*If there are two instances of Name with the same Date they are counted together into the Timespan ts
                 * Also counts the time inbetween the two instances as an Pause
                 */
                else if (lernplan[i + 1].Beginn - lernplan[i].Beginn < new TimeSpan(10, 1, 0))
                {
                    dt = lernplan[i].Beginn.Date;
                    ts = lernplan[i + 1].Ende - lernplan[i].Beginn;
                    i++;
                }
                /*Sets the timespan ts approporiate to the lenght of the Name instance
                 */
                else
                {
                    dt = lernplan[i].Beginn.Date;
                    ts = lernplan[i].Ende - lernplan[i].Beginn;
                }

                /*Sets the lerneinheit number to 1 if the Timespan ts for the Date is longer than 7:29:00
                 */
                if (ts > mindestVerbrauch) lerneinheit = 1;
                /*Sets the lerneinheit number to 6 if the Timespan ts for the Date is 00:00:00
                 */
                else if (ts == nullVerbrauch) lerneinheit = 6;
                /*Sets the lerneinheit number to 3 for the rest of the cases
                 */
                else lerneinheit = 3;

                /*Adds the DateTime dt, TimeSpan ts and Lerneinheiten to the Zeitvebracht model list
                 */
                zeitverbracht.Add(new ZeitsverbrachtModel()
                {
                    Tag = dt,
                    Verbrauch = ts,
                    Lerneinheiten = lerneinheit,
                    LernFacher = ""
                });

                HomeController.LehreinheitenCount += lerneinheit;
            }

            return zeitverbracht;
        }

        /// <summary>
        /// Creates a ZeitverbrachtModel list for the given csv filepath
        /// </summary>
        /// <param name="Path">String containing the csv filepath</param>
        /// <returns></returns>
        public static List<LernplanModel> DataLoader(string Path)
        {
            List<LernplanModel> lernplan = new List<LernplanModel>();
            //Makes a reference to the csv file
            StreamReader reader = new StreamReader(Path);

            /*Reads the whole csv file to the end
             */
            while (!reader.EndOfStream)
            {
                //Reads the current line in the file
                var line = reader.ReadLine();

                /*Converts the current line into an string array
                 * column Name = values[0]
                 * column Beginn = values[1]
                 * column Ende = values[2]
                 */
                var values = line.Split(';');

                /*Checks the value of the current row and continues the loop if its on the first line of the table
                 */
                if (values[0] == "Name" || values[0] == "Repetitorium Mathematik")
                    continue;

                /*Adds the data from the current row to the LernplanModel list
                 */
                lernplan.Add(new LernplanModel
                {
                    Name = values[0].ToString(),
                    Beginn = DateTime.Parse(values[1].ToString()),
                    Ende = DateTime.Parse(values[2].ToString())
                });
            }

            /*Disposes the used StreamReader for the csv file
             */
            reader.Dispose();

            return lernplan;
        }

        public static Listen LerneinheitenVerteiler(Listen listenObj)
        {
            listenObj.grupiertFach = listenObj.grupiertFach.OrderByDescending(o => o.LerneinheitenZahl).ToList();

            for (int i = 0; i < listenObj.zeitverbracht.Count(); i++)
            {
                if (listenObj.zeitverbracht[i].Lerneinheiten == 6)
                {
                    listenObj.zeitverbracht[i].LernFacher += listenObj.grupiertFach[0].Name.Split('(').First() + listenObj.grupiertFach[1].Name.Split('(').First();
                    listenObj.grupiertFach[0].LerneinheitenZahl -= 3;
                    listenObj.grupiertFach[1].LerneinheitenZahl -= 3;
                }
                else
                {
                    listenObj.zeitverbracht[i].LernFacher += listenObj.grupiertFach[0].Name.Split('(').First();
                    listenObj.grupiertFach[0].LerneinheitenZahl -= listenObj.zeitverbracht[i].Lerneinheiten;
                }

                listenObj.grupiertFach = listenObj.grupiertFach.OrderByDescending(o => o.LerneinheitenZahl).ToList();
            }

            return listenObj;
        }
    }
}