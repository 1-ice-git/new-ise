using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtConiugePassaporto : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void SetIncludiEscludiPassaporto(decimal idConiugePassaporto, ref bool chk, ref decimal idAttivitaPassaporto)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var cp = db.CONIUGEPASSAPORTO.Find(idConiugePassaporto);

                if (cp?.IDCONIUGEPASSAPORTO > 0)
                {
                    idAttivitaPassaporto = cp.ATTIVAZIONIPASSAPORTI.IDATTIVAZIONIPASSAPORTI;

                    if (cp.INCLUDIPASSAPORTO)
                    {
                        cp.INCLUDIPASSAPORTO = false;
                        chk = false;
                    }
                    else
                    {
                        cp.INCLUDIPASSAPORTO = true;
                        chk = true;
                    }

                    cp.DATAAGGIORNAMENTO = DateTime.Now;

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Non è stato possibile modificare lo stato di includi/escludi passaporto.");
                    }
                    else
                    {
                        decimal idTrasferimento = cp.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO;

                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                "Includi il coniuge per la richiesta del passaporto/visto.", "CONIUGEPASSAPORTO", db,
                                idTrasferimento, cp.IDCONIUGEPASSAPORTO);
                    }
                }
                else
                {
                    throw new Exception("Riga per il coniuge passaporto non presente.");
                }



            }
        }

    }
}