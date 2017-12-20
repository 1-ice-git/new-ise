using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using NewISE.Models.ViewModel;
using System.Data.Entity;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRichiamo : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        
        
        public List<RichiamoModel> GetLista_Richiamo(decimal idtrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                TRASFERIMENTO trasf = db.TRASFERIMENTO.Find(idtrasferimento);
                var tmp = (from e in trasf.RICHIAMO
                           
                           select new RichiamoModel()
                           {
                            //IDTRASFRICHIAMO = e.IDTRASFRICHIAMO,
                            //DATAOPERAZIONE = e.DATAOPERAZIONE
                           }).ToList();
                return tmp;
            }
        }

        public RichiamoModel getRichiamoById(decimal idTrasfRichiamo)
        {
            RichiamoModel tmp = new RichiamoModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                //tmp = (from e in db.RICHIAMO
                //       where e.IDTRASFRICHIAMO == idTrasfRichiamo
                //       select new RichiamoModel()
                //       {   
                //           IDTRASFRICHIAMO = e.IDTRASFRICHIAMO,
                //           DATAOPERAZIONE = e.DATAOPERAZIONE

                //       }).ToList().FirstOrDefault();
            }
            return tmp;
        }


        

        

    }
}