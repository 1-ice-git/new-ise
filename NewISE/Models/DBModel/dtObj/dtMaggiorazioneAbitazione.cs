using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;
using Newtonsoft.Json.Schema;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;

using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.ModelRest;
using System.Diagnostics;
using System.IO;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtMaggiorazioneAbitazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public AttivazioneMABModel GetUltimaAttivitaMAB(decimal idTrasferimento)
        {
            try
            {
                AttivazioneMABModel amm = new AttivazioneMABModel();
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var aml =t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList();

                    if (aml?.Any() ?? false)
                    {
                        am = aml.First();
                    }
                    else
                    {
                        am = this.CreaAttivitaMAB(idTrasferimento, db);
                    }

                    amm = new AttivazioneMABModel()
                    {
                        idAttivazioneMAB = am.IDATTIVAZIONEMAB,
                        idTrasferimento = am.IDTRASFERIMENTO,
                        notificaRichiesta = am.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = am.DATANOTIFICARICHIESTA,
                        Attivazione = am.ATTIVAZIONE,
                        dataAttivazione = am.DATAATTIVAZIONE,
                        dataVariazione = am.DATAVARIAZIONE,
                        dataAggiornamento = am.DATAAGGIORNAMENTO,
                        Annullato = am.ANNULLATO
                    };
                }

                return amm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ATTIVAZIONEMAB CreaAttivitaMAB(decimal idTrasferimento, ModelDBISE db)
        {
            ATTIVAZIONEMAB new_am = new ATTIVAZIONEMAB()
            {
                IDTRASFERIMENTO = idTrasferimento,
                NOTIFICARICHIESTA = false,
                DATAATTIVAZIONE = null,
                ATTIVAZIONE = false,
                DATANOTIFICARICHIESTA = null,
                ANNULLATO = false,
                DATAVARIAZIONE = DateTime.Now,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVAZIONEMAB.Add(new_am);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per la maggiorazione abitazione."));
            }
            else
            {
               // Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione maggiorazione abitazione.", "ATTIVITAZIONAMAB", db, new_am.IDTRASFERIMENTO, new_am.IDATTIVAZIONEMAB);
            }

            return new_am;
        }
        //public MAGGIORAZIONEABITAZIONE CreaMaggiorazioneAttivazioneByAttivazioneMAB(ATTIVAZIONEMAB aMAB, ModelDBISE db)
        //{
        //    MAGGIORAZIONEABITAZIONE new_ma = new MAGGIORAZIONEABITAZIONE()
        //    {
        //        IDTRASFERIMENTO = aMAB.IDTRASFERIMENTO,
        //        IDATTIVAZIONEMAB=aMAB.IDATTIVAZIONEMAB,
        //        DATAINIZIOMAB = aMAB.TRASFERIMENTO.DATAPARTENZA,
        //        DATAFINEMAB = null,
        //        ATTIVAZIONE = false,
        //        DATANOTIFICARICHIESTA = null,
        //        ANNULLATO = false,
        //        DATAVARIAZIONE = DateTime.Now,
        //        DATAAGGIORNAMENTO = DateTime.Now,
        //    };
        //    db.ATTIVAZIONEMAB.Add(new_am);

        //    if (db.SaveChanges() <= 0)
        //    {
        //        throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per la maggiorazione abitazione."));
        //    }
        //    else
        //    {
        //        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione maggiorazione abitazione.", "ATTIVITAZIONAMAB", db, new_am.IDTRASFERIMENTO, new_am.IDATTIVAZIONEMAB);
        //    }

        //    return new_am;
        //}

        public decimal GetNumAttivazioniMAB(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var NumAttivazioni = 0;
                NumAttivazioni = db.TRASFERIMENTO.Find(idTrasferimento).ATTIVAZIONEMAB
                                    .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true)
                                    .OrderByDescending(a => a.IDATTIVAZIONEMAB).Count();
                return NumAttivazioni;
            }
        }

        //public AttivazioneMABModel GetMaggiorazioneAbitazione(decimal idTrasferimento)
        //{
        //    try
        //    {
        //        MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();
        //        MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE();

        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var t = db.TRASFERIMENTO.Find(idTrasferimento);

        //            var mal = t.MAGGIORAZIONEABITAZIONE.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDMAB).ToList();

        //            if (mal?.Any() ?? false)
        //            {
        //                ma = mal.First();

        //                mam = new MaggiorazioneAbitazioneModel()
        //            {
        //                idMAB = ma.IDATTIVAZIONEMAB,
        //                idTrasferimento=ma.IDTRASFERIMENTO,
        //                idAttivazioneMAB=ma.IDATTIVAZIONEMAB


        //            };

        //            }
        //        }

        //        return mam;


        //    }
        //}

    }
}