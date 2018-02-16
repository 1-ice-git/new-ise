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


        public AttivazioneMABModel GetUltimaAttivazioneMAB(decimal idTrasferimento)
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

        public AttivazioneMABModel GetAttivitaMAB(decimal idTrasferimento)
        {
            try
            {
                AttivazioneMABModel amm = new AttivazioneMABModel();
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var aml = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList();

                    if (aml?.Any() ?? false)
                    {
                        am = aml.First();

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

                }

                return amm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void VerificaDocumentiPartenza(AttivazioneMABModel amm, out bool siDocCopiaContratto, out bool siDocCopiaRicevuta,
                                                        out bool siDocModulo1, out bool siDocModulo2, out bool siDocModulo3,
                                                        out bool siDocModulo4, out bool siDocModulo5)
        {
            siDocCopiaContratto = false;
            siDocCopiaRicevuta = false;
            siDocModulo1 = false;
            siDocModulo2 = false;
            siDocModulo3 = false;
            siDocModulo4 = false;
            siDocModulo5 = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var am = db.ATTIVAZIONEMAB.Find(amm.idAttivazioneMAB);

                    var docl = am.DOCUMENTI.Where(a => a.MODIFICATO== false).ToList();

                    if (docl?.Any() ?? false)
                    {
                        foreach(var doc in docl)
                        {
                            switch ((EnumTipoDoc)doc.IDTIPODOCUMENTO)
                            {
                                case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                                    siDocModulo1 = true;
                                    break;
                                case EnumTipoDoc.Dichiarazione_Costo_Locazione:
                                    siDocModulo2 = true;
                                    break;
                                case EnumTipoDoc.Attestazione_Spese_Abitazione_Dirigente:
                                    siDocModulo3 = true;
                                    break;
                                case EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore:
                                    siDocModulo4 = true;
                                    break;
                                case EnumTipoDoc.Clausole_Contratto_Alloggio:
                                    siDocModulo5 = true;
                                    break;
                                case EnumTipoDoc.Copia_Contratto_Locazione:
                                    siDocCopiaContratto = true;
                                    break;
                                case EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione:
                                    siDocCopiaRicevuta = true;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public CanoneMABModel GetCanoneMAB(MaggiorazioneAbitazioneModel mam)
        {
            try
            {
                CanoneMABModel cmm = new CanoneMABModel();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var cml = db.MAGGIORAZIONEABITAZIONE.Find(mam.idMAB)
                                .CANOMEMAB.Where(X => X.ANNULLATO == false && 
                                                 X.DATAINIZIOVALIDITA>=mam.dataInizioMAB && 
                                                 X.DATAFINEVALIDITA<=mam.dataFineMAB)
                                .ToList();

                    if (cml?.Any() ?? false)
                    {
                        var cm = cml.First();

                        cmm = new CanoneMABModel()
                        {
                            IDAttivazioneMAB = cm.IDATTIVAZIONEMAB,
                            IDMAB = cm.IDMAB,
                            DataInizioValidita = cm.DATAINIZIOVALIDITA,
                            DataFineValidita = cm.DATAFINEVALIDITA,
                            ImportoCanone = cm.IMPORTOCANONE,
                            DataAggiornamento = cm.DATAAGGIORNAMENTO,
                            Annullato = cm.ANNULLATO
                            //,TFR = cm.TFR.Where(x=>x.DATAINIZIOVALIDITA>=cm.DATAINIZIOVALIDITA && x.DATAFINEVALIDITA<=cmm.DataFineValidita).ToList();
                        };
                    }

                }

                return cmm;
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

        public MaggiorazioneAbitazioneModel GetMaggiorazioneAbitazione(AttivazioneMABModel amm)
        {
            try
            {
                MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();
                //MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE();

                using (ModelDBISE db = new ModelDBISE())
                {
                    //var t =  db.TRASFERIMENTO.Find(idTrasferimento);

                    var mal = db.ATTIVAZIONEMAB.Find(amm.idAttivazioneMAB).MAGGIORAZIONEABITAZIONE.Where(x => x.ANNULLATO == false).OrderBy(x => x.IDMAB).ToList();
                    //var mal = t.MAGGIORAZIONEABITAZIONE.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDMAB).ToList();

                    if (mal?.Any() ?? false)
                    {
                        MAGGIORAZIONEABITAZIONE ma = mal.First();

                        mam = new MaggiorazioneAbitazioneModel()
                        {
                            idMAB = ma.IDATTIVAZIONEMAB,
                            idTrasferimento = ma.IDTRASFERIMENTO,
                            idAttivazioneMAB = ma.IDATTIVAZIONEMAB,
                            dataInizioMAB=ma.DATAINIZIOMAB,
                            dataFineMAB=ma.DATAFINEMAB,
                            AnticipoAnnuale=ma.ANTICIPOANNUALE,
                            dataAggiornamento=ma.DATAAGGIORNAMENTO,
                            Annullato=ma.ANNULLATO
                        };

                    }
                }

                return mam;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}