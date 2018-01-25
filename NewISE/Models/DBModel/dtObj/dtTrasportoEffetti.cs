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


        public TrasportoEffettiModel GetTrasportoEffettiByID(decimal idTrasportoEffetti)
        {
            TrasportoEffettiModel tem = new TrasportoEffettiModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                //var te = db.TRASPORTOEFFETTI.Find(idTrasportoEffetti);

                //if (te != null && te.IDTRASPORTOEFFETTI > 0)
                //{
                //    tem = new TrasportoEffettiModel()
                //    {
                //        idTrasportoEffetti = te.IDTRASPORTOEFFETTI,
                //        idTipoTrasporto = te.IDTIPOTRASPORTO,
                //        idTrasferimento = te.IDTRASFERIMENTO,
                //        dataAggiornamento = te.DATAAGGIORNAMENTO,
                //        annullato = te.ANNULLATO,
                //    };
                //}

            }

            return tem;
        }


    }
}