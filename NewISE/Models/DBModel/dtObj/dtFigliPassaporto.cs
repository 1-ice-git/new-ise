using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtFigliPassaporto : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void SetIncludiEscludiPassaporto(decimal idFiglioPassaporto, ref bool chk)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var fp = db.FIGLIPASSAPORTO.Find(idFiglioPassaporto);

                if (fp?.IDPASSAPORTI > 0)
                {
                    if (fp.INCLUDIPASSAPORTO)
                    {
                        fp.INCLUDIPASSAPORTO = false;
                        chk = false;
                    }
                    else
                    {
                        fp.INCLUDIPASSAPORTO = true;
                        chk = true;
                    }


                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Non è stato possibile modificare lo stato di includi/escludi passaporto.");
                    }
                    else
                    {

                        decimal idTrasferimento = fp.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO;

                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                            "Inclusione/Esclusione del figlio dalla richiesta del passaporto.", "FIGLIPASSAPORTO", db,
                            idTrasferimento, fp.IDFIGLIPASSAPORTO);
                    }
                }
                else
                {
                    throw new Exception("Riga non presente per FIGLIPASSAPORTO.");
                }



            }
        }

    }
}