using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtTrasportoEffetti : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void PreSetTrasportoEffetti(decimal idTrasferimento, ModelDBISE db)
        {
            TEPARTENZA tep = new TEPARTENZA();
            TERIENTRO ter = new TERIENTRO();

            var t = db.TRASFERIMENTO.Find(idTrasferimento);

            tep = new TEPARTENZA()
            {
                IDTEPARTENZA = t.IDTRASFERIMENTO
            };

            db.TEPARTENZA.Add(tep);

            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nell'inserimento della riga di trasporto effetti fase partenza.");
            }
            else
            {
                ter = new TERIENTRO()
                {
                    IDTERIENTRO = t.IDTRASFERIMENTO
                };

                db.TERIENTRO.Add(ter);

                int j = db.SaveChanges();

                if (j <= 0)
                {
                    throw new Exception("Errore nell'inserimento della riga di trasporto effetti fase rientro.");
                }

            }


        }



        public TrasportoEffettiPartenzaModel GetTrasportoEffettiPartenzaByID(decimal idTrasportoEffettiPartenza)
        {
            TrasportoEffettiPartenzaModel tepm = new TrasportoEffettiPartenzaModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tep = db.TEPARTENZA.Find(idTrasportoEffettiPartenza);

                if (tep != null && tep.IDTEPARTENZA > 0)
                {
                    tepm = new TrasportoEffettiPartenzaModel()
                    {
                        idTrasportoEffettiPartenza = tep.IDTEPARTENZA
                    };
                }

            }

            return tepm;
        }

        public TrasportoEffettiRientroModel GetTrasportoEffettiRientroByID(decimal idTrasportoEffettiRientro)
        {
            TrasportoEffettiRientroModel term = new TrasportoEffettiRientroModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ter = db.TERIENTRO.Find(idTrasportoEffettiRientro);

                if (ter != null && ter.IDTERIENTRO > 0)
                {
                    term = new TrasportoEffettiRientroModel()
                    {
                        idTrasportoEffettiRientro = ter.IDTERIENTRO
                    };
                }

            }

            return term;
        }



    }
}