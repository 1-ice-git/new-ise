﻿using NewISE.EF;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.ModelRest;
using NewISE.Models.Tools;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtMaggiorazioniFamiliari : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void AttivaRichiesta(decimal idMaggiorazioniFamiliari)
        {
            bool rinunciaMagFam = false;
            bool richiestaAttivazione = false;
            bool attivazione = false;
            bool datiConiuge = false;
            bool datiParzialiConiuge = false;
            bool datiFigli = false;
            bool datiParzialiFigli = false;
            bool siDocConiuge = false;
            bool siDocFigli = false;
            int i = 0;

            this.SituazioneMagFam(idMaggiorazioniFamiliari, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli);

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    if (rinunciaMagFam == true && richiestaAttivazione == true && attivazione == false)
                    {
                        var mf = db.MAGGIORAZIONEFAMILIARI.Find(idMaggiorazioniFamiliari);

                        mf.ATTIVAZIONEMAGGIOARAZIONI = true;

                        i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception("Errore nella fase di attivazione delle maggiorazioni familiari.");
                        }
                    }
                    else if (rinunciaMagFam == false && richiestaAttivazione == true && attivazione == false)
                    {
                        if (datiConiuge == true || datiFigli == true)
                        {
                            if (datiParzialiConiuge == false && datiParzialiFigli == false)
                            {
                                if (datiConiuge == true && siDocConiuge == true || datiFigli == true && siDocFigli == true)
                                {
                                    var mf = db.MAGGIORAZIONEFAMILIARI.Find(idMaggiorazioniFamiliari);

                                    mf.ATTIVAZIONEMAGGIOARAZIONI = true;

                                    i = db.SaveChanges();

                                    if (i <= 0)
                                    {
                                        throw new Exception("Errore nella fase di attivazione delle maggiorazioni familiari.");
                                    }
                                }

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

        private void InvioEmailMagFam(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();
            TrasferimentoModel trm = new TrasferimentoModel();
            DipendentiModel dipendente = new DipendentiModel();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario cc = new Destinatario();

            try
            {

                mfm = this.GetMaggiorazioniFamiliariByID(idMaggiorazioniFamiliari);
                if (mfm != null && mfm.HasValue())
                {
                    am = Utility.UtenteAutorizzato();

                    mittente.Nominativo = am.nominativo;
                    mittente.EmailMittente = am.eMail;

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        trm = dtt.GetSoloTrasferimentoById(mfm.idTrasferimento, db);

                        using (dtDipendenti dtd = new dtDipendenti())
                        {
                            dipendente = dtd.GetDipendenteByID(trm.idDipendente, db);
                            if (dipendente != null && dipendente.HasValue())
                            {
                                if (am.idRuoloUtente == (decimal)EnumRuoloAccesso.SuperAmministratore)
                                {
                                    cc = new Destinatario()
                                    {
                                        Nominativo = am.nominativo,
                                        EmailDestinatario = am.eMail
                                    };
                                }
                                else
                                {
                                    cc = new Destinatario()
                                    {
                                        Nominativo = dipendente.Nominativo,
                                        EmailDestinatario = dipendente.email
                                    };
                                }


                                using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                                {
                                    using (GestioneEmail gmail = new GestioneEmail())
                                    {
                                        //luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.SuperAmministratore).ToList());
                                        luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore).ToList());

                                        if (luam?.Any() ?? false)
                                        {
                                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                                            {
                                                msgMail.mittente = mittente;

                                                foreach (var uam in luam)
                                                {
                                                    //var uteConn = dtd.GetDipendenteByMatricola(uam.matricola);

                                                    var client = new RestSharp.RestClient("http://128.1.50.97:82");
                                                    var req = new RestSharp.RestRequest("api/dipendente", RestSharp.Method.POST);
                                                    req.RequestFormat = RestSharp.DataFormat.Json;
                                                    req.AddParameter("matricola", uam.matricola);

                                                    RestSharp.IRestResponse<RetDipendenteJson> resp = client.Execute<RetDipendenteJson>(req);

                                                    RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

                                                    RetDipendenteJson retDip = deserial.Deserialize<RetDipendenteJson>(resp);

                                                    if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                                                    {
                                                        if (retDip.success == true)
                                                        {
                                                            if (retDip.items != null)
                                                            {

                                                                if (am.idRuoloUtente == (decimal)EnumRuoloAccesso.SuperAmministratore)
                                                                {
                                                                    Destinatario dest = new Destinatario()
                                                                    {
                                                                        Nominativo = am.nominativo,
                                                                        EmailDestinatario = am.eMail
                                                                    };

                                                                    msgMail.destinatario.Add(dest);
                                                                    return;
                                                                }
                                                                else
                                                                {
                                                                    Destinatario dest = new Destinatario()
                                                                    {
                                                                        Nominativo = retDip.items.nominativo,
                                                                        EmailDestinatario = retDip.items.email
                                                                    };

                                                                    msgMail.destinatario.Add(dest);
                                                                }




                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        using (Config.Config cfg = new Config.Config())
                                                        {
                                                            sAdmin sad = new sAdmin();
                                                            sad = cfg.SuperAmministratore();
                                                            if (sad.s_admin?.Any() ?? false)
                                                            {
                                                                var lute =
                                                                    sad.s_admin.Where(a => a.username == uam.matricola)
                                                                        .ToList();
                                                                if (lute?.Any() ?? false)
                                                                {
                                                                    var ute = lute.First();

                                                                    Destinatario dest = new Destinatario()
                                                                    {
                                                                        Nominativo = ute.nominatico,
                                                                        EmailDestinatario = ute.email
                                                                    };

                                                                    msgMail.destinatario.Add(dest);
                                                                }
                                                            }
                                                        }
                                                    }


                                                }

                                                if (msgMail.destinatario?.Any() ?? false)
                                                {
                                                    msgMail.cc.Add(cc);

                                                    msgMail.oggetto =
                                                        Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioniFamiliari;
                                                    msgMail.corpoMsg =
                                                        string.Format(
                                                            Resources.msgEmail
                                                                .MessaggioNotificaRichiestaMaggiorazioniFamiliari,
                                                            dipendente.Nominativo);

                                                    gmail.sendMail(msgMail);
                                                }
                                                else
                                                {
                                                    throw new Exception("Non è stato possibile inviare l'email.");
                                                }


                                            }

                                        }
                                    }

                                }
                            }
                        }


                    }
                }
                else
                {
                    throw new Exception("Non è stato possibile inviare l'email.");
                }




            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public void NotificaRichiesta(decimal idMaggiorazioniFamiliari)
        {
            bool rinunciaMagFam = false;
            bool richiestaAttivazione = false;
            bool attivazione = false;
            bool datiConiuge = false;
            bool datiParzialiConiuge = false;
            bool datiFigli = false;
            bool datiParzialiFigli = false;
            bool siDocConiuge = false;
            bool siDocFigli = false;
            int i = 0;

            this.SituazioneMagFam(idMaggiorazioniFamiliari, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli);

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var mf = db.MAGGIORAZIONEFAMILIARI.Find(idMaggiorazioniFamiliari);

                        if (rinunciaMagFam == false && richiestaAttivazione == false && attivazione == false)
                        {
                            if (datiConiuge == false && datiFigli == false)
                            {
                                mf.RINUNCIAMAGGIORAZIONI = true;
                                mf.RICHIESTAATTIVAZIONE = true;
                                i = db.SaveChanges();
                                if (i <= 0)
                                {
                                    throw new Exception("Errore nella fase d'inserimento per la rinuncia delle maggiorazioni familiari.");
                                }
                                else
                                {
                                    this.InvioEmailMagFam(idMaggiorazioniFamiliari, db);
                                }
                            }
                            else if (datiConiuge == true || datiFigli == true)
                            {
                                if (datiParzialiConiuge == false && datiParzialiFigli == false)
                                {
                                    mf.RICHIESTAATTIVAZIONE = true;
                                    i = db.SaveChanges();
                                    if (i <= 0)
                                    {
                                        throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione per le maggiorazioni familiari.");
                                    }
                                    else
                                    {
                                        this.InvioEmailMagFam(idMaggiorazioniFamiliari, db);
                                    }
                                }
                            }
                        }

                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        db.Database.CurrentTransaction.Rollback();
                        throw ex;
                    }



                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public void SituazioneMagFam(decimal idMaggiorazioniFamiliari, out bool rinunciaMagFam,
                                       out bool richiestaAttivazione, out bool Attivazione,
                                       out bool datiConiuge, out bool datiParzialiConiuge,
                                       out bool datiFigli, out bool datiParzialiFigli,
                                       out bool siDocConiuge, out bool siDocFigli)
        {
            rinunciaMagFam = false;
            richiestaAttivazione = false;
            Attivazione = false;
            datiConiuge = false;
            datiParzialiConiuge = false;
            datiFigli = false;
            datiParzialiFigli = false;
            siDocConiuge = false;
            siDocFigli = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONEFAMILIARI.Find(idMaggiorazioniFamiliari);

                if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    rinunciaMagFam = mf.RINUNCIAMAGGIORAZIONI;
                    richiestaAttivazione = mf.RICHIESTAATTIVAZIONE;
                    Attivazione = mf.ATTIVAZIONEMAGGIOARAZIONI;


                    if (mf.CONIUGE != null)
                    {
                        var lc = mf.CONIUGE.Where(a => a.ANNULLATO == false).ToList();
                        if (lc?.Any() ?? false)
                        {
                            datiConiuge = true;
                            foreach (var c in lc)
                            {
                                var nadc = c.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);

                                if (nadc > 0)
                                {
                                    datiParzialiConiuge = false;
                                }
                                else
                                {
                                    datiParzialiConiuge = true;
                                    break;
                                }
                            }

                            foreach (var c in lc)
                            {
                                var ndocc = c.DOCUMENTI.Count;

                                if (ndocc > 0)
                                {
                                    siDocConiuge = true;
                                }
                                else
                                {
                                    siDocConiuge = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            datiConiuge = false;
                        }

                    }

                    if (mf.FIGLI != null)
                    {
                        var lf = mf.FIGLI.Where(a => a.ANNULLATO == false).ToList();

                        if (lf?.Any() ?? false)
                        {
                            datiFigli = true;
                            foreach (var f in lf)
                            {
                                var nadf = f.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);

                                if (nadf > 0)
                                {
                                    datiParzialiFigli = false;
                                }
                                else
                                {
                                    datiParzialiFigli = true;
                                    break;
                                }
                            }

                            foreach (var f in lf)
                            {
                                var ndocf = f.DOCUMENTI.Count;
                                if (ndocf > 0)
                                {
                                    siDocFigli = true;
                                }
                                else
                                {
                                    siDocFigli = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            datiFigli = false;
                        }
                    }
                }

            }

        }



        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliariByID(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();


            var mf = db.MAGGIORAZIONEFAMILIARI.Find(idMaggiorazioniFamiliari);

            if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
            {
                mcm = new MaggiorazioniFamiliariModel()
                {
                    idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,
                    idTrasferimento = mf.IDTRASFERIMENTO,
                    rinunciaMaggiorazioni = mf.RINUNCIAMAGGIORAZIONI,
                    richiestaAttivazione = mf.RICHIESTAATTIVAZIONE,
                    attivazioneMaggiorazioni = mf.ATTIVAZIONEMAGGIOARAZIONI,
                    dataAggiornamento = mf.DATAAGGIORNAMENTO,
                    annullato = mf.ANNULLATO,
                };
            }


            return mcm;
        }


        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliariByID(decimal idMaggiorazioniFamiliari)
        {
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONEFAMILIARI.Find(idMaggiorazioniFamiliari);

                if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    mcm = new MaggiorazioniFamiliariModel()
                    {
                        idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,
                        idTrasferimento = mf.IDTRASFERIMENTO,
                        rinunciaMaggiorazioni = mf.RINUNCIAMAGGIORAZIONI,
                        richiestaAttivazione = mf.RICHIESTAATTIVAZIONE,
                        attivazioneMaggiorazioni = mf.ATTIVAZIONEMAGGIOARAZIONI,
                        dataAggiornamento = mf.DATAAGGIORNAMENTO,
                        annullato = mf.ANNULLATO,
                    };
                }
            }

            return mcm;
        }




        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliariByIDTrasf(decimal idTrasferimento)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lmf =
                    db.MAGGIORAZIONEFAMILIARI.Where(
                        a => a.ANNULLATO == false && a.IDTRASFERIMENTO == idTrasferimento)
                        .OrderByDescending(a => a.DATAAGGIORNAMENTO)
                        .ToList();

                if (lmf?.Any() ?? false)
                {
                    var mf = lmf.First();
                    mfm = new MaggiorazioniFamiliariModel()
                    {
                        idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,
                        idTrasferimento = mf.IDTRASFERIMENTO,
                        rinunciaMaggiorazioni = mf.RINUNCIAMAGGIORAZIONI,
                        richiestaAttivazione = mf.RICHIESTAATTIVAZIONE,
                        attivazioneMaggiorazioni = mf.ATTIVAZIONEMAGGIOARAZIONI,
                        dataAggiornamento = mf.DATAAGGIORNAMENTO,
                        annullato = mf.ANNULLATO,
                    };
                }
            }

            return mfm;
        }

        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliariByIDTrasf(decimal idTrasferimento, ModelDBISE db)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();

            //var lmf =
            //    db.MAGGIORAZIONEFAMILIARI.Where(
            //        a => a.ANNULLATO == false && a.IDTRASFERIMENTO == idTrasferimento)
            //        .OrderByDescending(a => a.DATAAGGIORNAMENTO)
            //        .ToList();

            var lmf =
                db.TRASFERIMENTO.Find(idTrasferimento)
                    .MAGGIORAZIONEFAMILIARI.Where(a => a.ANNULLATO == false)
                    .OrderByDescending(a => a.IDMAGGIORAZIONIFAMILIARI)
                    .ToList();

            if (lmf?.Any() ?? false)
            {
                var mf = lmf.First();
                mfm = new MaggiorazioniFamiliariModel()
                {
                    idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,
                    idTrasferimento = mf.IDTRASFERIMENTO,
                    rinunciaMaggiorazioni = mf.RINUNCIAMAGGIORAZIONI,
                    richiestaAttivazione = mf.RICHIESTAATTIVAZIONE,
                    attivazioneMaggiorazioni = mf.ATTIVAZIONEMAGGIOARAZIONI,
                    dataAggiornamento = mf.DATAAGGIORNAMENTO,
                    annullato = mf.ANNULLATO,
                };
            }


            return mfm;
        }

        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliaribyFiglio(decimal idFiglio)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.FIGLI.Find(idFiglio).MAGGIORAZIONEFAMILIARI;

                if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    mfm = new MaggiorazioniFamiliariModel()
                    {
                        idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,
                        idTrasferimento = mf.IDTRASFERIMENTO,
                        rinunciaMaggiorazioni = mf.RINUNCIAMAGGIORAZIONI,
                        richiestaAttivazione = mf.RICHIESTAATTIVAZIONE,
                        attivazioneMaggiorazioni = mf.ATTIVAZIONEMAGGIOARAZIONI,
                        dataAggiornamento = mf.DATAAGGIORNAMENTO,
                        annullato = mf.ANNULLATO,
                    };
                }
            }

            return mfm;
        }

        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliaribyConiuge(decimal idConiuge)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.CONIUGE.Find(idConiuge).MAGGIORAZIONEFAMILIARI;

                if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    mfm = new MaggiorazioniFamiliariModel()
                    {
                        idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,
                        idTrasferimento = mf.IDTRASFERIMENTO,
                        rinunciaMaggiorazioni = mf.RINUNCIAMAGGIORAZIONI,
                        richiestaAttivazione = mf.RICHIESTAATTIVAZIONE,
                        attivazioneMaggiorazioni = mf.ATTIVAZIONEMAGGIOARAZIONI,
                        dataAggiornamento = mf.DATAAGGIORNAMENTO,
                        annullato = mf.ANNULLATO,
                    };
                }
            }
            return mfm;
        }


        public void SetMaggiorazioneFamiliari(ref MaggiorazioniFamiliariModel mfm, ModelDBISE db)
        {
            MAGGIORAZIONEFAMILIARI mf = new MAGGIORAZIONEFAMILIARI()
            {
                IDTRASFERIMENTO = mfm.idTrasferimento,
                RINUNCIAMAGGIORAZIONI = mfm.rinunciaMaggiorazioni,
                RICHIESTAATTIVAZIONE = mfm.richiestaAttivazione,
                ATTIVAZIONEMAGGIOARAZIONI = mfm.attivazioneMaggiorazioni,
                DATAAGGIORNAMENTO = mfm.dataAggiornamento,
                ANNULLATO = mfm.annullato
            };

            db.MAGGIORAZIONEFAMILIARI.Add(mf);

            if (db.SaveChanges() > 0)
            {
                mfm.idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento  delle Maggiorazioni familiari",
                    "MAGGIORAZIONEFAMILIARI", db, mfm.idTrasferimento, mf.IDMAGGIORAZIONIFAMILIARI);
            }
        }


        public void InserisciFiglio(FigliModel fm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtFigli dtf = new dtFigli())
                    {
                        fm.dataAggiornamento = DateTime.Now;
                        fm.Annullato = false;
                        using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                        {
                            var pm = dtpp.GetPassaportoByIdMagFam(fm.idMaggiorazioniFamiliari);
                            fm.idPassaporto = pm.idPassaporto;
                        }
                        dtf.SetFiglio(ref fm, db);
                        using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                        {
                            DateTime dtIni = fm.dataInizio.Value;
                            DateTime dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                            List<PercentualeMagFigliModel> lpmfm =
                                dtpf.GetPercentualeMaggiorazioneFigli((TipologiaFiglio)fm.idTipologiaFiglio, dtIni,
                                    dtFin, db).ToList();

                            if (lpmfm?.Any() ?? false)
                            {
                                foreach (var pmfm in lpmfm)
                                {
                                    dtpf.AssociaPercentualeMaggiorazioneFigli(fm.idFigli, pmfm.idPercMagFigli, db);
                                }
                            }
                            else
                            {
                                throw new Exception("Non è presente nessuna percentuale per il figlio.");
                            }
                        }

                        using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                        {
                            DateTime dtIni = fm.dataInizio.Value;
                            DateTime dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                            List<IndennitaPrimoSegretModel> lipsm =
                                dtips.GetIndennitaPrimoSegretario(dtIni, dtFin, db).ToList();

                            if (lipsm?.Any() ?? false)
                            {
                                foreach (var ipsm in lipsm)
                                {
                                    dtips.AssociaIndennitaPrimoSegretarioFiglio(fm.idFigli, ipsm.idIndPrimoSegr, db);
                                }
                            }
                            else
                            {
                                throw new Exception(
                                    "Non è presente nessuna indennità di primo segretario per il figlio che si vuole inserire.");
                            }
                        }
                    }


                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void InserisciConiuge(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtConiuge dtc = new dtConiuge())
                    {
                        cm.dataAggiornamento = DateTime.Now;
                        cm.annullato = false;
                        using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                        {
                            var pm = dtpp.GetPassaportoByIdMagFam(cm.idMaggiorazioniFamiliari);
                            cm.idPassaporto = pm.idPassaporto;
                        }
                        dtc.SetConiuge(ref cm, db);
                        using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                        {
                            DateTime dtIni = cm.dataInizio.Value;
                            DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();


                            List<PercentualeMagConiugeModel> lpmcm =
                                dtpc.GetListaPercentualiMagConiugeByRangeDate(cm.idTipologiaConiuge, dtIni, dtFin, db)
                                    .ToList();

                            if (lpmcm?.Any() ?? false)
                            {
                                foreach (var pmcm in lpmcm)
                                {
                                    dtpc.AssociaPercentualeMaggiorazioneConiuge(cm.idConiuge, pmcm.idPercentualeConiuge,
                                        db);
                                }
                            }
                            else
                            {
                                throw new Exception("Non è presente nessuna percentuale del coniuge.");
                            }
                        }
                    }

                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void ModificaConiuge(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    using (dtConiuge dtc = new dtConiuge())
                    {
                        dtc.EditConiuge(cm, db);
                    }


                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        //public MaggiorazioniFamiliariModel GetMaggiorazioneFamiliare(decimal idTrasferimento, DateTime dt)
        //{
        //    MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        var lmf = db.MAGGIORAZIONEFAMILIARI.Where(a => a.ANNULLATO == false && a.PRATICACONCLUSA == false && a.IDTRASFERIMENTO == idTrasferimento).OrderByDescending(a => a.DATACONCLUSIONE).ToList();
        //        if (lmf?.Any() ?? false)
        //        {
        //            var mc = lmf.First();
        //            var lpmg = mc.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
        //            if (lpmg != null && lpmg.Count > 0)
        //            {
        //                var pmg = lpmg.First();

        //                var lpc = mc.PENSIONE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIO && dt <= a.DATAFINE).OrderByDescending(a => a.DATAINIZIO).ToList();

        //                if (lpc != null && lpc.Count > 0)
        //                {
        //                    //var pc = lpc.First();

        //                    mcm = new MaggiorazioniFamiliariModel()
        //                    {
        //                        idMaggiorazioneConiuge = mc.IDMAGGIORAZIONECONIUGE,
        //                        idTrasferimento = mc.IDTRASFERIMENTO,
        //                        idPercentualeMaggiorazioneConiuge = pmg.IDPERCMAGCONIUGE,
        //                        lPensioneConiuge = (from e in lpc
        //                                            select new PensioneConiugeModel()
        //                                            {
        //                                                idPensioneConiuge = e.IDPENSIONE,
        //                                                importoPensione = e.IMPORTOPENSIONE,
        //                                                dataInizioValidita = e.DATAINIZIO,
        //                                                dataFineValidita = e.DATAFINE,
        //                                                dataAggiornamento = e.DATAAGGIORNAMENTO,
        //                                                annullato = e.ANNULLATO
        //                                            }).ToList(),
        //                        dataInizioValidita = mc.DATAINIZIOVALIDITA,
        //                        dataFineValidita = mc.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : mc.DATAFINEVALIDITA,
        //                        dataAggiornamento = mc.DATAAGGIORNAMENTO,
        //                        annullato = mc.ANNULLATO,
        //                    };
        //                }
        //                else
        //                {
        //                    mcm = new MaggiorazioniFamiliariModel()
        //                    {
        //                        idMaggiorazioneConiuge = mc.IDMAGGIORAZIONECONIUGE,
        //                        idTrasferimento = mc.IDTRASFERIMENTO,
        //                        idPercentualeMaggiorazioneConiuge = pmg.IDPERCMAGCONIUGE,
        //                        dataInizioValidita = mc.DATAINIZIOVALIDITA,
        //                        dataFineValidita = mc.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : mc.DATAFINEVALIDITA,
        //                        dataAggiornamento = mc.DATAAGGIORNAMENTO,
        //                        annullato = mc.ANNULLATO,
        //                    };
        //                }
        //            }
        //        }
        //    }

        //    return mcm;
        //}
    }
}