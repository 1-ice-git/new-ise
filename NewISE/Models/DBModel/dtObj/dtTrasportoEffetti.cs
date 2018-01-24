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