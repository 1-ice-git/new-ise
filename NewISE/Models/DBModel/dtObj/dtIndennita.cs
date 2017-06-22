using NewISE.Models.Tools;
using NewISE.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtIndennita : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void DeleteIndennita(decimal idTrasferimento, ModelDBISE db)
        {
            var i = db.INDENNITA.Find(idTrasferimento);

            db.INDENNITA.Remove(i);

            db.SaveChanges();


        }


        public IndennitaModel GetIndennitaByIdTrasferimento(decimal idTrasferimento, ModelDBISE db)
        {
            IndennitaModel indennita = new IndennitaModel();
            //RuoloDipendenteModel rdm = new RuoloDipendenteModel();
            

            var i = db.INDENNITA.Find(idTrasferimento);
            //rdm = (from e in i.RUOLODIPENDENTE
            //       where e.ANNULLATO == false && 
            //         )


            if (i != null && i.IDTRASFINDENNITA > 0)
            {
                indennita = new IndennitaModel()
                {
                    idTrasfIndennita = i.IDTRASFINDENNITA,
                    dataInizio = i.DATAINIZIO,
                    dataFine = i.DATAFINE,
                    dataAggiornamento = i.DATAAGGIORNAMENTO
                    
                };
            }


            return indennita;
        }




        public void SetIndennita(IndennitaModel im, ModelDBISE db)
        {
            INDENNITA i = new INDENNITA();

            i.IDTRASFINDENNITA = im.idTrasfIndennita;
            i.DATAINIZIO = im.dataInizio;
            i.DATAFINE = im.dataFine.HasValue ? im.dataFine.Value : Utility.DataFineStop();
            i.DATAAGGIORNAMENTO = im.dataAggiornamento;

            db.INDENNITA.Add(i);

            db.SaveChanges();

            

        }



    }
}