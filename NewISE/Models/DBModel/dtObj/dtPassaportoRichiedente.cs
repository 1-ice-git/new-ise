﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.Tools;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPassaportoRichiedente : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void SetIncludiEscludiPassaporto(decimal idPassaportoRichiedente, ref bool chk, ref decimal idAttivazioniPassaporto)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var pr = db.PASSAPORTORICHIEDENTE.Find(idPassaportoRichiedente);

                if (pr?.IDPASSAPORTORICHIEDENTE > 0)
                {
                    idAttivazioniPassaporto = pr.ATTIVAZIONIPASSAPORTI.IDATTIVAZIONIPASSAPORTI;

                    if (pr.INCLUDIPASSAPORTO)
                    {
                        pr.INCLUDIPASSAPORTO = false;
                        chk = false;
                    }
                    else
                    {
                        pr.INCLUDIPASSAPORTO = true;
                        chk = true;
                    }

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Non è stato possibile modificare lo stato di includi/escludi passaporto.");
                    }
                    else
                    {

                        decimal idTrasferimento = pr.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO;

                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                            "Inclusione/Esclusione del richiedente dalla richiesta del passaporto.",
                            "PASSAPORTORICHIEDENTE", db,
                            idTrasferimento, pr.IDPASSAPORTORICHIEDENTE);
                    }
                }
                else
                {
                    throw new Exception("Riga per il richidente passaporto non presente.");
                }
            }
        }


    }
}