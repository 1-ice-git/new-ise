﻿using NewISE.EF;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;
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
                    dataAggiornamento = i.DATAAGGIORNAMENTO

                };
            }


            return indennita;
        }




        public void SetIndennita(IndennitaModel im, ModelDBISE db)
        {
            INDENNITA i = new INDENNITA();

            i.IDTRASFINDENNITA = im.idTrasfIndennita;
            i.DATAAGGIORNAMENTO = im.dataAggiornamento;

            db.INDENNITA.Add(i);

            if (db.SaveChanges() > 0)
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova indennità.", "Indennita", db, im.idTrasfIndennita, im.idTrasfIndennita);
            }
        }

        public void EditIndennita(IndennitaModel im, ModelDBISE db)
        {
            INDENNITA i = db.INDENNITA.Find(im.idTrasfIndennita);
            i.DATAAGGIORNAMENTO = im.dataAggiornamento;

            if (db.SaveChanges() > 0)
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Modifica dell'indennità.", "Indennita", db, im.idTrasfIndennita, im.idTrasfIndennita);
            }
        }



    }
}