using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization; // << dont forget to add this for converting dates to localtime
using NewISE.EF;

namespace NewISE.Models.DBModel
{
    public class DiaryEvent
    {

        public int ID;
        public string Title;
        public int SomeImportantKeyID;
        public string StartDateString;
        public string EndDateString;
        public string StatusString;
        public string StatusColor;
        public string ClassName;
        public string Attivi;
        public string Completati;
        public string Scaduti;

        

        public static List<DiaryEvent> LoadAppointmentSummaryInDateRange(double start, double end)
        {

            var fromDate = ConvertFromUnixTimestamp(start);
            var toDate = ConvertFromUnixTimestamp(end);
            using (ModelDBISE ent = new ModelDBISE())
            {
                //var rsltAttivi = ent.CALENDARIOEVENTI.Where(s => s.DATAINIZIOEVENTO >= fromDate && s.DATASCADENZA >= DateTime.Now && s.COMPLETATO==false)
                //                                        .GroupBy(s => System.Data.Objects.EntityFunctions.TruncateTime(s.DateTimeScheduled))
                //                                        .Select(x => new { DateTimeScheduled = x.Key, Count = x.Count() });

                //attivi SIGNIFICA non completati e non scaduti rispetto alla data odierna
                var rsltAttivi = ent.CALENDARIOEVENTI.Where(s => s.DATAINIZIOEVENTO >= fromDate && s.DATASCADENZA >= toDate && s.COMPLETATO == false)
                 .Select(x => new { DateTimeInizio = x.DATAINIZIOEVENTO, DateTimeScadenza = x.DATASCADENZA });

                List<DiaryEvent> result = new List<DiaryEvent>();
                int i = 0, conteggia = 0;
                DiaryEvent rec = new DiaryEvent();
                foreach (var item in rsltAttivi)
                {                    
                    rec.ID = i; //we dont link this back to anything as its a group summary but the fullcalendar needs unique IDs for each event item (unless its a repeating event)
                    rec.SomeImportantKeyID = -1;  
                    string StringDate = string.Format("{0:yyyy-MM-dd}", item.DateTimeInizio);
                    rec.StartDateString = StringDate + "T00:00:00"; //ISO 8601 format
                    rec.EndDateString = StringDate +"T23:59:59";
                    //rec.Title = "Booked: " + item.Count.ToString();
                   // rec.Attivi = "Attivi: " + Attivi;
                    //result.Add(rec);
                    i++;conteggia++;
                }
                rec.Attivi = "Attivi: " + conteggia.ToString();
                result.Add(rec);

                conteggia = 0;
                var rsltCompletati= ent.CALENDARIOEVENTI.Where(s => s.DATAINIZIOEVENTO.Month==DateTime.Now.Month && s.DATAINIZIOEVENTO.Year == DateTime.Now.Year && s.COMPLETATO ==true)
                .Select(x => new { DateTimeInizio = x.DATAINIZIOEVENTO, DateTimeScadenza = x.DATASCADENZA });
                
                rec = new DiaryEvent();
                foreach (var item in rsltCompletati)
                {
                    rec.ID = i; //we dont link this back to anything as its a group summary but the fullcalendar needs unique IDs for each event item (unless its a repeating event)
                    rec.SomeImportantKeyID = -1;
                    string StringDate = string.Format("{0:yyyy-MM-dd}", item.DateTimeInizio);
                    rec.StartDateString = StringDate + "T00:00:00"; //ISO 8601 format
                    rec.EndDateString = StringDate + "T23:59:59";
                    //rec.Title = "Booked: " + item.Count.ToString();
                    // rec.Attivi = "Attivi: " + Attivi;
                    //result.Add(rec);
                    i++; conteggia++;
                }
                rec.Completati = "Completati: " + conteggia.ToString();
                result.Add(rec);

                conteggia = 0;
                var rsltScaduti = ent.CALENDARIOEVENTI.Where(s => s.DATAINIZIOEVENTO.Month == DateTime.Now.Month && s.DATAINIZIOEVENTO.Year == DateTime.Now.Year && s.COMPLETATO == true)
                .Select(x => new { DateTimeInizio = x.DATAINIZIOEVENTO, DateTimeScadenza = x.DATASCADENZA });
                rec = new DiaryEvent();
                foreach (var item in rsltScaduti)
                {
                    rec.ID = i; //we dont link this back to anything as its a group summary but the fullcalendar needs unique IDs for each event item (unless its a repeating event)
                    rec.SomeImportantKeyID = -1;
                    string StringDate = string.Format("{0:yyyy-MM-dd}", item.DateTimeInizio);
                    rec.StartDateString = StringDate + "T00:00:00"; //ISO 8601 format
                    rec.EndDateString = StringDate + "T23:59:59";
                    //rec.Title = "Booked: " + item.Count.ToString();
                    // rec.Attivi = "Attivi: " + Attivi;
                    //result.Add(rec);
                    i++; conteggia++;
                }
                rec.Scaduti = "Scaduti: " + conteggia.ToString();
                result.Add(rec);
                return result;
            }
        
        }

        


        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }


        
    }
}