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