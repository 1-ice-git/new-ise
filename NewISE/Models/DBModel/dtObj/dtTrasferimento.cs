﻿using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;


using System.Data.Entity;
using Newtonsoft.Json.Schema;
using NewISE.Models.ViewModel;

using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.ModelRest;
using System.Diagnostics;
using System.IO;
using NewISE.Models.Config;
using NewISE.Models.DBModel.Enum;
using NewISE.Models.Config.s_admin;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.dtObj;

namespace NewISE.Models.DBModel.dtObj
{
    public enum EnumTipologiaCoan
    {
        Servizi_Istituzionali = 1,
        Servizi_Promozionali = 2
    }

    public class dtTrasferimento : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static ValidationResult VerificaRequiredCoan(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var tr = context.ObjectInstance as TrasferimentoModel;

            if (tr != null)
            {
                if (tr.idTipoCoan == Convert.ToDecimal(EnumTipologiaCoan.Servizi_Promozionali))
                {
                    if (tr.coan != null && tr.coan != string.Empty && tr.coan.Length == 10)
                    {
                        vr = ValidationResult.Success;
                    }
                    else
                    {
                        vr = new ValidationResult("Il CO.AN. è richiesto e deve essere composto da 10 caratteri.");
                    }
                }
                else if (tr.idTipoCoan == Convert.ToDecimal(EnumTipologiaCoan.Servizi_Istituzionali))
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = new ValidationResult("Il CO.AN. è richiesto e deve essere composto da 10 caratteri.");
            }

            return vr;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTrasfOld"></param>
        /// <param name="dtTrasfNew"></param>
        /// <returns></returns>
        public bool VerificaDataInizioTrasferimentoNew(decimal idTrasfOld, DateTime dtTrasfNew)
        {
            bool ret = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var tOld = db.TRASFERIMENTO.Find(idTrasfOld);

                if (tOld.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato)
                {
                    if (tOld.DATAPARTENZA >= dtTrasfNew)
                    {
                        ret = true;
                    }
                }


            }

            return ret;
        }


        public TrasferimentoModel GetTrasferimentoByIdTEPartenza(decimal idTEPartenza)
        {
            TrasferimentoModel trm = new TrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tep = db.TEPARTENZA.Find(idTEPartenza);
                var tr = tep.TRASFERIMENTO;
                if (tr != null && tr.IDTRASFERIMENTO > 0)
                {
                    trm = new TrasferimentoModel()
                    {
                        idTrasferimento = tr.IDTRASFERIMENTO,
                        idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                        idUfficio = tr.IDUFFICIO,
                        idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                        idDipendente = tr.IDDIPENDENTE,
                        idTipoCoan = tr.IDTIPOCOAN,
                        dataPartenza = tr.DATAPARTENZA,
                        dataRientro = tr.DATARIENTRO,
                        coan = tr.COAN,
                        protocolloLettera = tr.PROTOCOLLOLETTERA,
                        dataLettera = tr.DATALETTERA,
                        notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                        dataAggiornamento = tr.DATAAGGIORNAMENTO,
                        StatoTrasferimento = new StatoTrasferimentoModel()
                        {
                            idStatoTrasferimento = tr.STATOTRASFERIMENTO.IDSTATOTRASFERIMENTO,
                            descrizioneStatoTrasferimento = tr.STATOTRASFERIMENTO.DESCRIZIONE
                        },
                        TipoTrasferimento = new TipoTrasferimentoModel()
                        {
                            idTipoTrasferimento = tr.TIPOTRASFERIMENTO.IDTIPOTRASFERIMENTO,
                            descTipoTrasf = tr.TIPOTRASFERIMENTO.TIPOTRASFERIMENTO1
                        },
                        Ufficio = new UfficiModel()
                        {
                            idUfficio = tr.UFFICI.IDUFFICIO,
                            codiceUfficio = tr.UFFICI.CODICEUFFICIO,
                            descUfficio = tr.UFFICI.DESCRIZIONEUFFICIO
                        },
                        Dipendente = new DipendentiModel()
                        {
                            idDipendente = tr.DIPENDENTI.IDDIPENDENTE,
                            matricola = tr.DIPENDENTI.MATRICOLA,
                            nome = tr.DIPENDENTI.NOME,
                            cognome = tr.DIPENDENTI.COGNOME,
                            dataAssunzione = tr.DIPENDENTI.DATAASSUNZIONE,
                            dataCessazione = tr.DIPENDENTI.DATACESSAZIONE,
                            indirizzo = tr.DIPENDENTI.INDIRIZZO,
                            cap = tr.DIPENDENTI.CAP,
                            citta = tr.DIPENDENTI.CITTA,
                            provincia = tr.DIPENDENTI.PROVINCIA,
                            email = tr.DIPENDENTI.EMAIL,
                            telefono = tr.DIPENDENTI.TELEFONO,
                            fax = tr.DIPENDENTI.FAX,
                            abilitato = tr.DIPENDENTI.ABILITATO,
                            dataInizioRicalcoli = tr.DIPENDENTI.DATAINIZIORICALCOLI
                        },
                        TipoCoan = new TipologiaCoanModel()
                        {
                            idTipoCoan = tr.TIPOLOGIACOAN.IDTIPOCOAN,
                            descrizione = tr.TIPOLOGIACOAN.DESCRIZIONE
                        }
                    };
                }

            }

            return trm;

        }


        public bool EsisteTrasferimentoSuccessivo(decimal idTrasferimento)
        {
            bool ret = false;
            List<TRASFERIMENTO> ts = new List<TRASFERIMENTO>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                if (t.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato)
                {
                    ts = db.TRASFERIMENTO.Where(a => a.DATAPARTENZA > t.DATARIENTRO && a.IDDIPENDENTE == t.IDDIPENDENTE).ToList();
                }
                else
                {
                    ts = db.TRASFERIMENTO.Where(a => a.IDTRASFERIMENTO > t.IDTRASFERIMENTO && a.IDDIPENDENTE == t.IDDIPENDENTE).ToList();
                }

                if (ts?.Any() ?? false)
                {
                    ret = true;
                }

            }

            return ret;

        }


        public IList<TrasferimentoModel> GetListaTrasferimento(int matricola)
        {
            List<TrasferimentoModel> ltm = new List<TrasferimentoModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var d = db.DIPENDENTI.First(a => a.MATRICOLA == matricola);

                    if (d?.IDDIPENDENTE > 0)
                    {
                        var lt = d.TRASFERIMENTO.OrderByDescending(a => a.DATAPARTENZA);

                        if (lt?.Any() ?? false)
                        {
                            ltm = (from t in lt
                                   select new TrasferimentoModel()
                                   {
                                       idTrasferimento = t.IDTRASFERIMENTO,
                                       idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                                       idUfficio = t.IDUFFICIO,
                                       idStatoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO,
                                       idDipendente = t.IDDIPENDENTE,
                                       idTipoCoan = t.IDTIPOCOAN,
                                       dataPartenza = t.DATAPARTENZA,
                                       dataRientro = t.DATARIENTRO,
                                       coan = t.COAN,
                                       protocolloLettera = t.PROTOCOLLOLETTERA,
                                       dataLettera = t.DATALETTERA,
                                       notificaTrasferimento = t.NOTIFICATRASFERIMENTO,
                                       dataAggiornamento = t.DATAAGGIORNAMENTO,
                                       StatoTrasferimento = new StatoTrasferimentoModel()
                                       {
                                           idStatoTrasferimento = t.STATOTRASFERIMENTO.IDSTATOTRASFERIMENTO,
                                           descrizioneStatoTrasferimento = t.STATOTRASFERIMENTO.DESCRIZIONE
                                       },
                                       TipoTrasferimento = new TipoTrasferimentoModel()
                                       {
                                           idTipoTrasferimento = t.TIPOTRASFERIMENTO.IDTIPOTRASFERIMENTO,
                                           descTipoTrasf = t.TIPOTRASFERIMENTO.TIPOTRASFERIMENTO1
                                       },
                                       Ufficio = new UfficiModel()
                                       {
                                           idUfficio = t.UFFICI.IDUFFICIO,
                                           codiceUfficio = t.UFFICI.CODICEUFFICIO,
                                           descUfficio = t.UFFICI.DESCRIZIONEUFFICIO
                                       },
                                       Dipendente = new DipendentiModel()
                                       {
                                           idDipendente = t.DIPENDENTI.IDDIPENDENTE,
                                           matricola = t.DIPENDENTI.MATRICOLA,
                                           nome = t.DIPENDENTI.NOME,
                                           cognome = t.DIPENDENTI.COGNOME,
                                           dataAssunzione = t.DIPENDENTI.DATAASSUNZIONE,
                                           dataCessazione = t.DIPENDENTI.DATACESSAZIONE,
                                           indirizzo = t.DIPENDENTI.INDIRIZZO,
                                           cap = t.DIPENDENTI.CAP,
                                           citta = t.DIPENDENTI.CITTA,
                                           provincia = t.DIPENDENTI.PROVINCIA,
                                           email = t.DIPENDENTI.EMAIL,
                                           telefono = t.DIPENDENTI.TELEFONO,
                                           fax = t.DIPENDENTI.FAX,
                                           abilitato = t.DIPENDENTI.ABILITATO,
                                           dataInizioRicalcoli = t.DIPENDENTI.DATAINIZIORICALCOLI
                                       },
                                       TipoCoan = new TipologiaCoanModel()
                                       {
                                           idTipoCoan = t.TIPOLOGIACOAN.IDTIPOCOAN,
                                           descrizione = t.TIPOLOGIACOAN.DESCRIZIONE
                                       },
                                   }).ToList();
                        }


                    }
                    else
                    {
                        throw new Exception("Nessun dipendente presente sul database per la matricola selezionata.");
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ltm;
        }


        public void GestioneAttivitaTrasferimento(decimal idTrasferimento,
                                                      out bool richiestaMF, out bool attivazioneMF,
                                                      out bool richiestaPP, out bool conclusePP,
                                                      out bool faseRichiestaPPattivata, out bool faseInvioPPattivata, 
                                                      out bool richiesteTV, out bool concluseTV,
                                                      out bool richiestaTE, out bool attivazioneTE,
                                                      out bool richiestaAnticipi, out bool attivazioneAnticipi,
                                                      out bool richiestaMAB, out bool attivazioneMAB,
                                                      out bool richiestaPS, out bool attivazionePS,
                                                      out bool solaLettura)
        {
            richiestaMF = false;
            attivazioneMF = false;

            richiestaPP = false;
            conclusePP = false;
            faseRichiestaPPattivata = false;
            faseInvioPPattivata = false;

            richiesteTV = false;
            concluseTV = false;

            richiestaTE = false;
            attivazioneTE = false;

            richiestaAnticipi = false;
            attivazioneAnticipi = false;

            richiestaMAB = false;
            attivazioneMAB = false;

            richiestaPS = false;
            attivazionePS = false;

            solaLettura = true;

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                if (t.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Da_Attivare)
                {
                    solaLettura = false;
                }

                #region MaggiorazioniFamiliari

                var mf = t.MAGGIORAZIONIFAMILIARI;
                if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    var lamf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONEMAGFAM);

                    if (lamf?.Any() ?? false)
                    {
                        var amf = lamf.First();

                        richiestaMF = amf.RICHIESTAATTIVAZIONE;
                        attivazioneMF = amf.ATTIVAZIONEMAGFAM;
                    }
                    else
                    {
                        throw new Exception("Errore 'GestioneAttivitaTrasferimento' record ATTIVAZIONIMAGFAM non trovato.");
                    }
                }
                #endregion

                #region Pratiche passaporto

                var p = t.PASSAPORTI;
                if (p != null && p.IDPASSAPORTI > 0)
                {
                    var lap_richiesta = p.ATTIVAZIONIPASSAPORTI.Where(
                                a =>
                                        a.ANNULLATO == false &&
                                        a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Richiesta_Passaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI);

                    if (lap_richiesta?.Any() ?? false)
                    {
                        var ap_richiesta = lap_richiesta.First();
                        if(ap_richiesta.PRATICACONCLUSA)
                        {
                            faseRichiestaPPattivata = true;
                        }
                        richiestaPP = ap_richiesta.NOTIFICARICHIESTA;
                        conclusePP = ap_richiesta.PRATICACONCLUSA;
                    }
                    else
                    {
                        throw new Exception("Errore 'GestioneAttivitaTrasferimento' record ATTIVAZIONIPASSAPORTI (fase Richiesta) non trovato.");
                    }



                    var lap_invio = p.ATTIVAZIONIPASSAPORTI
                            .Where(a => a.ANNULLATO == false &&
                                        a.IDFASEPASSAPORTI==(decimal)EnumFasePassaporti.Invio_Passaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI);

                    if (lap_invio?.Any() ?? false)
                    {
                        var ap_invio = lap_invio.First();
                        if (ap_invio.PRATICACONCLUSA)
                        {
                            faseInvioPPattivata = true;
                        }
                        if(faseRichiestaPPattivata)
                        {
                            richiestaPP = ap_invio.NOTIFICARICHIESTA;
                            conclusePP = ap_invio.PRATICACONCLUSA;
                        }
                    }
                }
                #endregion

                #region Titoli di viaggio
                var tv = t.TITOLIVIAGGIO;
                if (tv != null && tv.IDTITOLOVIAGGIO > 0)
                {
                    var latv = tv.ATTIVAZIONETITOLIVIAGGIO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO);

                    if (latv?.Any() ?? false)
                    {
                        var atv = latv.First();

                        richiesteTV = atv.NOTIFICARICHIESTA;
                        concluseTV = atv.ATTIVAZIONERICHIESTA;
                    }
                    //else
                    //{
                    //    throw new Exception("Errore 'GestioneAttivitaTrasferimento' record ATTIVAZIONITITOLIVIAGGIO non trovato.");
                    //}
                }

                #endregion

                #region Trasporto effetti partenza
                var tep = t.TEPARTENZA;
                if (tep != null && tep.IDTEPARTENZA > 0)
                {
                    var latep = tep.ATTIVITATEPARTENZA.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATEPARTENZA).ToList();

                    if (latep?.Any() ?? false)
                    {
                        var atep = latep.First();

                        richiestaTE = atep.RICHIESTATRASPORTOEFFETTI;
                        attivazioneTE = atep.ATTIVAZIONETRASPORTOEFFETTI;
                    }
                    //else
                    //{
                    //    throw new Exception("Errore 'GestioneAttivitaTrasferimento' record ATTIVITATEPARTENZA non trovato.");
                    //}
                }

                #endregion

                #region Anticipi
                var ps = t.PRIMASITEMAZIONE;
                if (ps != null && ps.IDPRIMASISTEMAZIONE > 0)
                {
                    var laa = ps.ATTIVITAANTICIPI.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVITAANTICIPI).ToList();

                    if (laa?.Any() ?? false)
                    {
                        var aa = laa.First();

                        richiestaAnticipi = aa.NOTIFICARICHIESTA;
                        attivazioneAnticipi = aa.ATTIVARICHIESTA;
                    }
                }

                #endregion

                #region MaggiorazioneAbitazione
                var lma = t.MAGGIORAZIONEABITAZIONE.Where(x => x.VARIAZIONE == false).OrderBy(x => x.IDMAB);
                if (lma?.Any() ?? false)
                {
                    var ma = lma.First();

                    if (ma != null && ma.IDMAB > 0)
                    {
                        var lam = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList();

                        if (lam?.Any() ?? false)
                        {
                            var am = lam.First();

                            richiestaMAB = am.NOTIFICARICHIESTA;
                            attivazioneMAB = am.ATTIVAZIONE;
                        }
                    }
                }

                #endregion

                #region ProvvidenzeScolastiche
                var lps = t.PROVVIDENZESCOLASTICHE;
                if (lps != null && lps.IDTRASFPROVSCOLASTICHE > 0)
                {
                    
                    var laps = lps.ATTIVAZIONIPROVSCOLASTICHE.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDPROVSCOLASTICHE).ToList();

                    if (laps?.Any() ?? false)
                    {
                        var atps = laps.First();

                        richiestaPS = atps.NOTIFICARICHIESTA;
                        attivazionePS = atps.ATTIVARICHIESTA;
                    }
                   
                }
                #endregion

            }
        }

        public DateTime? DataInizioTrasferimento(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                if (t != null && t.IDTRASFERIMENTO > 0)
                {
                    return t.DATAPARTENZA.Date;
                }
                else
                {
                    return null;
                }
            }
        }

        public DateTime? DataFineTrasferimento(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                if (t != null && t.IDTRASFERIMENTO > 0)
                {
                    return t.DATARIENTRO?.Date;
                }
                else
                {
                    return null;
                }
            }
        }

        public TrasferimentoModel GetTrasferimentoByIdAttMagFam(decimal idAttivazioneMagFam)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);
                    if (amf?.IDATTIVAZIONEMAGFAM > 0)
                    {
                        var mf = amf.MAGGIORAZIONIFAMILIARI;
                        var t = mf.TRASFERIMENTO;

                        if (t != null && t.IDTRASFERIMENTO > 0)
                        {
                            tm = new TrasferimentoModel()
                            {
                                idTrasferimento = t.IDTRASFERIMENTO,
                                idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                                idUfficio = t.IDUFFICIO,
                                idStatoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO,
                                idDipendente = t.IDDIPENDENTE,
                                idTipoCoan = t.IDTIPOCOAN,
                                dataPartenza = t.DATAPARTENZA,
                                dataRientro = t.DATARIENTRO,
                                coan = t.COAN,
                                protocolloLettera = t.PROTOCOLLOLETTERA,
                                dataLettera = t.DATALETTERA,
                                notificaTrasferimento = t.NOTIFICATRASFERIMENTO,
                                dataAggiornamento = t.DATAAGGIORNAMENTO

                            };
                        }
                        else
                        {
                            throw new Exception("Non è stato possibile intercettare il trasferimento con l'id Attivazione maggiorazione familiare: (" + idAttivazioneMagFam + ")");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return tm;
        }




        public TrasferimentoModel GetTrasferimentoByIdFiglio(decimal idFiglio)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.FIGLI.Find(idFiglio).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

                    if (t?.IDTRASFERIMENTO > 0)
                    {
                        tm = new TrasferimentoModel()
                        {
                            idTrasferimento = t.IDTRASFERIMENTO,
                            idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                            idUfficio = t.IDUFFICIO,
                            idStatoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO,
                            idDipendente = t.IDDIPENDENTE,
                            idTipoCoan = t.IDTIPOCOAN,
                            dataPartenza = t.DATAPARTENZA,
                            dataRientro = t.DATARIENTRO,
                            coan = t.COAN,
                            protocolloLettera = t.PROTOCOLLOLETTERA,
                            dataLettera = t.DATALETTERA,
                            notificaTrasferimento = t.NOTIFICATRASFERIMENTO,
                            dataAggiornamento = t.DATAAGGIORNAMENTO

                        };
                    }
                    else
                    {
                        throw new Exception("Non è stato possibile intercettare il trasferimento con l'id figlio: (" + idFiglio + ")");
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return tm;
        }


        public TrasferimentoModel GetTrasferimentoByIdConiuge(decimal idConiuge)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.CONIUGE.Find(idConiuge).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

                    if (t?.IDTRASFERIMENTO > 0)
                    {
                        tm = new TrasferimentoModel()
                        {
                            idTrasferimento = t.IDTRASFERIMENTO,
                            idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                            idUfficio = t.IDUFFICIO,
                            idStatoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO,
                            idDipendente = t.IDDIPENDENTE,
                            idTipoCoan = t.IDTIPOCOAN,
                            dataPartenza = t.DATAPARTENZA,
                            dataRientro = t.DATARIENTRO,
                            coan = t.COAN,
                            protocolloLettera = t.PROTOCOLLOLETTERA,
                            dataLettera = t.DATALETTERA,
                            notificaTrasferimento = t.NOTIFICATRASFERIMENTO,
                            dataAggiornamento = t.DATAAGGIORNAMENTO

                        };
                    }
                    else
                    {
                        throw new Exception("Non è stato possibile intercettare il trasferimento con l'id coniuge: (" + idConiuge + ")");
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return tm;
        }

        public TrasferimentoModel GetTrasferimentoByIDMagFam(decimal idMaggiorazioniFamiliari)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari).TRASFERIMENTO;

                    if (t != null && t.IDTRASFERIMENTO > 0)
                    {
                        tm = new TrasferimentoModel()
                        {
                            idTrasferimento = t.IDTRASFERIMENTO,
                            idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                            idUfficio = t.IDUFFICIO,
                            idStatoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO,
                            idDipendente = t.IDDIPENDENTE,
                            idTipoCoan = t.IDTIPOCOAN,
                            dataPartenza = t.DATAPARTENZA,
                            dataRientro = t.DATARIENTRO,
                            coan = t.COAN,
                            protocolloLettera = t.PROTOCOLLOLETTERA,
                            dataLettera = t.DATALETTERA,
                            notificaTrasferimento = t.NOTIFICATRASFERIMENTO,
                            dataAggiornamento = t.DATAAGGIORNAMENTO

                        };
                    }
                    else
                    {
                        throw new Exception("Non è stato possibile intercettare il trasferimento con l'id maggiorazione familiare: (" + idMaggiorazioniFamiliari + ")");
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return tm;
        }
        public TrasferimentoModel GetTrasferimentoByIDMagFam(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            try
            {

                var t = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari).TRASFERIMENTO;

                if (t != null && t.IDTRASFERIMENTO > 0)
                {
                    tm = new TrasferimentoModel()
                    {
                        idTrasferimento = t.IDTRASFERIMENTO,
                        idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                        idUfficio = t.IDUFFICIO,
                        idStatoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO,
                        idDipendente = t.IDDIPENDENTE,
                        idTipoCoan = t.IDTIPOCOAN,
                        dataPartenza = t.DATAPARTENZA,
                        dataRientro = t.DATARIENTRO,
                        coan = t.COAN,
                        protocolloLettera = t.PROTOCOLLOLETTERA,
                        dataLettera = t.DATALETTERA,
                        notificaTrasferimento = t.NOTIFICATRASFERIMENTO,
                        dataAggiornamento = t.DATAAGGIORNAMENTO

                    };
                }
                else
                {
                    throw new Exception("Non è stato possibile intercettare il trasferimento con l'id maggiorazione familiare: (" + idMaggiorazioniFamiliari + ")");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return tm;
        }

        public IList<TrasferimentoModel> GetTrasferimentiPrecedenti(decimal idDipendente, DateTime dataPartenza)
        {
            List<TrasferimentoModel> ltm = new List<TrasferimentoModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lt = db.TRASFERIMENTO.Where(a => a.IDDIPENDENTE == idDipendente && a.DATAPARTENZA < dataPartenza).ToList();

                ltm = (from t in lt
                       select new TrasferimentoModel()
                       {
                           idTrasferimento = t.IDTRASFERIMENTO,
                           idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                           idUfficio = t.IDUFFICIO,
                           idStatoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO,
                           idDipendente = t.IDDIPENDENTE,
                           idTipoCoan = t.IDTIPOCOAN,
                           dataPartenza = t.DATAPARTENZA,
                           dataRientro = t.DATARIENTRO,
                           coan = t.COAN,
                           protocolloLettera = t.PROTOCOLLOLETTERA,
                           dataLettera = t.DATALETTERA,
                           notificaTrasferimento = t.NOTIFICATRASFERIMENTO,
                           dataAggiornamento = t.DATAAGGIORNAMENTO,
                           StatoTrasferimento = new StatoTrasferimentoModel()
                           {
                               idStatoTrasferimento = t.STATOTRASFERIMENTO.IDSTATOTRASFERIMENTO,
                               descrizioneStatoTrasferimento = t.STATOTRASFERIMENTO.DESCRIZIONE
                           },
                           TipoTrasferimento = new TipoTrasferimentoModel()
                           {
                               idTipoTrasferimento = t.TIPOTRASFERIMENTO.IDTIPOTRASFERIMENTO,
                               descTipoTrasf = t.TIPOTRASFERIMENTO.TIPOTRASFERIMENTO1
                           },
                           Ufficio = new UfficiModel()
                           {
                               idUfficio = t.UFFICI.IDUFFICIO,
                               codiceUfficio = t.UFFICI.CODICEUFFICIO,
                               descUfficio = t.UFFICI.DESCRIZIONEUFFICIO
                           },
                           Dipendente = new DipendentiModel()
                           {
                               idDipendente = t.DIPENDENTI.IDDIPENDENTE,
                               matricola = t.DIPENDENTI.MATRICOLA,
                               nome = t.DIPENDENTI.NOME,
                               cognome = t.DIPENDENTI.COGNOME,
                               dataAssunzione = t.DIPENDENTI.DATAASSUNZIONE,
                               dataCessazione = t.DIPENDENTI.DATACESSAZIONE,
                               indirizzo = t.DIPENDENTI.INDIRIZZO,
                               cap = t.DIPENDENTI.CAP,
                               citta = t.DIPENDENTI.CITTA,
                               provincia = t.DIPENDENTI.PROVINCIA,
                               email = t.DIPENDENTI.EMAIL,
                               telefono = t.DIPENDENTI.TELEFONO,
                               fax = t.DIPENDENTI.FAX,
                               abilitato = t.DIPENDENTI.ABILITATO,
                               dataInizioRicalcoli = t.DIPENDENTI.DATAINIZIORICALCOLI
                           },
                           TipoCoan = new TipologiaCoanModel()
                           {
                               idTipoCoan = t.TIPOLOGIACOAN.IDTIPOCOAN,
                               descrizione = t.TIPOLOGIACOAN.DESCRIZIONE
                           },
                       }).ToList();
            }

            return ltm;
        }


        public TrasferimentoModel GetTrasferimentoAttivoNotificato(string matricola)
        {
            TrasferimentoModel tm = new TrasferimentoModel();
            int matr = Convert.ToInt16(matricola);
            using (ModelDBISE db = new ModelDBISE())
            {
                var ldp = db.DIPENDENTI.Where(a => a.MATRICOLA == matr).ToList();
                if (ldp?.Any() ?? false)
                {
                    var lt =
                        ldp.First()
                            .TRASFERIMENTO.Where(
                                a =>
                                    a.DATAPARTENZA == ldp.First().TRASFERIMENTO.Max(b => b.DATAPARTENZA) &&
                                    a.NOTIFICATRASFERIMENTO == true)
                            .ToList();

                    if (lt?.Any() ?? false)
                    {
                        var t = lt.OrderBy(a => a.DATAPARTENZA).Last();

                        tm = new TrasferimentoModel()
                        {
                            idTrasferimento = t.IDTRASFERIMENTO,
                            idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                            idUfficio = t.IDUFFICIO,
                            idStatoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO,
                            idDipendente = t.IDDIPENDENTE,
                            idTipoCoan = t.IDTIPOCOAN,
                            dataPartenza = t.DATAPARTENZA,
                            dataRientro = t.DATARIENTRO,
                            coan = t.COAN,
                            protocolloLettera = t.PROTOCOLLOLETTERA,
                            dataLettera = t.DATALETTERA,
                            notificaTrasferimento = t.NOTIFICATRASFERIMENTO,
                            dataAggiornamento = t.DATAAGGIORNAMENTO,

                        };
                    }
                }
            }

            return tm;
        }


        public TrasferimentoModel GetUltimoSoloTrasferimentoByMatricola(string matricola)
        {
            TrasferimentoModel tm = new TrasferimentoModel();
            int matr = Convert.ToInt16(matricola);
            using (ModelDBISE db = new ModelDBISE())
            {
                var ldp = db.DIPENDENTI.Where(a => a.MATRICOLA == matr).ToList();
                if (ldp?.Any() ?? false)
                {
                    var lt = ldp.First().TRASFERIMENTO.Where(a => a.DATAPARTENZA == ldp.First().TRASFERIMENTO.Max(b => b.DATAPARTENZA)).ToList();

                    if (lt?.Any() ?? false)
                    {
                        var t = lt.OrderBy(a => a.DATAPARTENZA).Last();

                        tm = new TrasferimentoModel()
                        {
                            idTrasferimento = t.IDTRASFERIMENTO,
                            idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                            idUfficio = t.IDUFFICIO,
                            idStatoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO,
                            idDipendente = t.IDDIPENDENTE,
                            idTipoCoan = t.IDTIPOCOAN,
                            dataPartenza = t.DATAPARTENZA,
                            dataRientro = t.DATARIENTRO,
                            coan = t.COAN,
                            protocolloLettera = t.PROTOCOLLOLETTERA,
                            dataLettera = t.DATALETTERA,
                            notificaTrasferimento = t.NOTIFICATRASFERIMENTO,
                            dataAggiornamento = t.DATAAGGIORNAMENTO,

                        };
                    }
                }
            }

            return tm;
        }

        public TrasferimentoModel GetUltimoTrasferimentoByMatricola(string matricola)
        {
            TrasferimentoModel tm = new TrasferimentoModel();
            int matr = Convert.ToInt16(matricola);


            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ldp = db.DIPENDENTI.Where(a => a.MATRICOLA == matr).ToList();

                    if (ldp?.Any() ?? false)
                    {
                        var lt = ldp.First().TRASFERIMENTO.Where(a => a.DATAPARTENZA == ldp.First().TRASFERIMENTO.Max(b => b.DATAPARTENZA)).ToList();

                        if (lt?.Any() ?? false)
                        {
                            var t = lt.OrderBy(a => a.DATAPARTENZA).Last();


                            tm = new TrasferimentoModel()
                            {
                                idTrasferimento = t.IDTRASFERIMENTO,
                                idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                                idUfficio = t.IDUFFICIO,
                                idStatoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO,
                                idDipendente = t.IDDIPENDENTE,
                                idTipoCoan = t.IDTIPOCOAN,
                                dataPartenza = t.DATAPARTENZA,
                                dataRientro = t.DATARIENTRO,
                                coan = t.COAN,
                                protocolloLettera = t.PROTOCOLLOLETTERA,
                                dataLettera = t.DATALETTERA,
                                notificaTrasferimento = t.NOTIFICATRASFERIMENTO,
                                dataAggiornamento = t.DATAAGGIORNAMENTO,
                                StatoTrasferimento = new StatoTrasferimentoModel()
                                {
                                    idStatoTrasferimento = t.STATOTRASFERIMENTO.IDSTATOTRASFERIMENTO,
                                    descrizioneStatoTrasferimento = t.STATOTRASFERIMENTO.DESCRIZIONE
                                },
                                TipoTrasferimento = new TipoTrasferimentoModel()
                                {
                                    idTipoTrasferimento = t.TIPOTRASFERIMENTO.IDTIPOTRASFERIMENTO,
                                    descTipoTrasf = t.TIPOTRASFERIMENTO.TIPOTRASFERIMENTO1
                                },
                                Ufficio = new UfficiModel()
                                {
                                    idUfficio = t.UFFICI.IDUFFICIO,
                                    codiceUfficio = t.UFFICI.CODICEUFFICIO,
                                    descUfficio = t.UFFICI.DESCRIZIONEUFFICIO
                                },
                                Dipendente = new DipendentiModel()
                                {
                                    idDipendente = t.DIPENDENTI.IDDIPENDENTE,
                                    matricola = t.DIPENDENTI.MATRICOLA,
                                    nome = t.DIPENDENTI.NOME,
                                    cognome = t.DIPENDENTI.COGNOME,
                                    dataAssunzione = t.DIPENDENTI.DATAASSUNZIONE,
                                    dataCessazione = t.DIPENDENTI.DATACESSAZIONE,
                                    indirizzo = t.DIPENDENTI.INDIRIZZO,
                                    cap = t.DIPENDENTI.CAP,
                                    citta = t.DIPENDENTI.CITTA,
                                    provincia = t.DIPENDENTI.PROVINCIA,
                                    email = t.DIPENDENTI.EMAIL,
                                    telefono = t.DIPENDENTI.TELEFONO,
                                    fax = t.DIPENDENTI.FAX,
                                    abilitato = t.DIPENDENTI.ABILITATO,
                                    dataInizioRicalcoli = t.DIPENDENTI.DATAINIZIORICALCOLI
                                },
                                TipoCoan = new TipologiaCoanModel()
                                {
                                    idTipoCoan = t.TIPOLOGIACOAN.IDTIPOCOAN,
                                    descrizione = t.TIPOLOGIACOAN.DESCRIZIONE
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tm;
        }

        public TrasferimentoModel GetUltimoTrasferimentoByMatricola(string matricola, ModelDBISE db)
        {
            TrasferimentoModel tm = new TrasferimentoModel();
            int matr = Convert.ToInt16(matricola);
            try
            {
                var ldp = db.DIPENDENTI.Where(a => a.MATRICOLA == matr).ToList();

                if (ldp?.Any() ?? false)
                {
                    var lt = ldp.First().TRASFERIMENTO.Where(a => a.DATAPARTENZA == ldp.First().TRASFERIMENTO.Max(b => b.DATAPARTENZA)).ToList();

                    if (lt?.Any() ?? false)
                    {
                        List<IndennitaModel> lim = new List<IndennitaModel>();
                        var t = lt.OrderBy(a => a.DATAPARTENZA).Last();

                        tm = new TrasferimentoModel()
                        {
                            idTrasferimento = t.IDTRASFERIMENTO,
                            idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                            idUfficio = t.IDUFFICIO,
                            idStatoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO,
                            idDipendente = t.IDDIPENDENTE,
                            idTipoCoan = t.IDTIPOCOAN,
                            dataPartenza = t.DATAPARTENZA,
                            dataRientro = t.DATARIENTRO,
                            coan = t.COAN,
                            protocolloLettera = t.PROTOCOLLOLETTERA,
                            dataLettera = t.DATALETTERA,
                            notificaTrasferimento = t.NOTIFICATRASFERIMENTO,
                            dataAggiornamento = t.DATAAGGIORNAMENTO,
                            StatoTrasferimento = new StatoTrasferimentoModel()
                            {
                                idStatoTrasferimento = t.STATOTRASFERIMENTO.IDSTATOTRASFERIMENTO,
                                descrizioneStatoTrasferimento = t.STATOTRASFERIMENTO.DESCRIZIONE
                            },
                            TipoTrasferimento = new TipoTrasferimentoModel()
                            {
                                idTipoTrasferimento = t.TIPOTRASFERIMENTO.IDTIPOTRASFERIMENTO,
                                descTipoTrasf = t.TIPOTRASFERIMENTO.TIPOTRASFERIMENTO1
                            },
                            Ufficio = new UfficiModel()
                            {
                                idUfficio = t.UFFICI.IDUFFICIO,
                                codiceUfficio = t.UFFICI.CODICEUFFICIO,
                                descUfficio = t.UFFICI.DESCRIZIONEUFFICIO
                            },
                            Dipendente = new DipendentiModel()
                            {
                                idDipendente = t.DIPENDENTI.IDDIPENDENTE,
                                matricola = t.DIPENDENTI.MATRICOLA,
                                nome = t.DIPENDENTI.NOME,
                                cognome = t.DIPENDENTI.COGNOME,
                                dataAssunzione = t.DIPENDENTI.DATAASSUNZIONE,
                                dataCessazione = t.DIPENDENTI.DATACESSAZIONE,
                                indirizzo = t.DIPENDENTI.INDIRIZZO,
                                cap = t.DIPENDENTI.CAP,
                                citta = t.DIPENDENTI.CITTA,
                                provincia = t.DIPENDENTI.PROVINCIA,
                                email = t.DIPENDENTI.EMAIL,
                                telefono = t.DIPENDENTI.TELEFONO,
                                fax = t.DIPENDENTI.FAX,
                                abilitato = t.DIPENDENTI.ABILITATO,
                                dataInizioRicalcoli = t.DIPENDENTI.DATAINIZIORICALCOLI
                            },
                            TipoCoan = new TipologiaCoanModel()
                            {
                                idTipoCoan = t.TIPOLOGIACOAN.IDTIPOCOAN,
                                descrizione = t.TIPOLOGIACOAN.DESCRIZIONE
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tm;
        }

        public dipInfoTrasferimentoModel GetInfoTrasferimento(decimal idTrasferimento)
        {
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();
            TrasferimentoModel tm = new TrasferimentoModel();

            DateTime dtDatiParametri;

            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    tm = dtt.GetTrasferimentoById(idTrasferimento);

                    if (tm != null && tm.idTrasferimento > 0)
                    {
                        dit.statoTrasferimento = tm.idStatoTrasferimento;
                        dit.UfficioDestinazione = tm.Ufficio;
                        dit.Decorrenza = tm.dataPartenza;
                        if (tm.dataRientro.HasValue)
                        {
                            dtDatiParametri = tm.dataRientro.Value;
                        }
                        else
                        {
                            dtDatiParametri = tm.dataPartenza > Utility.GetDtInizioMeseCorrente() ? tm.dataPartenza : Utility.GetDtInizioMeseCorrente();
                        }

                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                        {
                            RuoloUfficioModel rum = new RuoloUfficioModel();
                            rum = dtrd.GetRuoloDipendenteByIdTrasferimento(tm.idTrasferimento, dtDatiParametri).RuoloUfficio;

                            dit.RuoloUfficio = rum;
                        }

                        if (dit.statoTrasferimento == EnumStatoTraferimento.Attivo || dit.statoTrasferimento == EnumStatoTraferimento.Da_Attivare || dit.statoTrasferimento == EnumStatoTraferimento.Terminato)
                        {
                            using (CalcoliIndennita ci = new CalcoliIndennita(tm.idTrasferimento))
                            {
                                dit.indennitaBase = ci.IndennitaDiBase;
                                dit.indennitaServizio = ci.IndennitaDiServizio;
                                dit.maggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                                dit.indennitaPersonale = ci.IndennitaPersonale;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dit;
        }

        public dipInfoTrasferimentoModel GetInfoTrasferimento(string matricola)
        {
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();
            TrasferimentoModel tm = new TrasferimentoModel();

            DateTime dtDatiParametri;

            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    tm = dtt.GetUltimoTrasferimentoByMatricola(matricola);

                    if (tm != null && tm.idTrasferimento > 0)
                    {
                        dit.statoTrasferimento = (EnumStatoTraferimento)tm.idStatoTrasferimento;
                        dit.UfficioDestinazione = tm.Ufficio;
                        dit.Decorrenza = tm.dataPartenza;
                        if (tm.dataRientro.HasValue)
                        {
                            dtDatiParametri = tm.dataRientro.Value;
                        }
                        else
                        {
                            dtDatiParametri = tm.dataPartenza > Utility.GetDtInizioMeseCorrente() ? tm.dataPartenza : Utility.GetDtInizioMeseCorrente();
                        }

                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                        {
                            RuoloUfficioModel rum = new RuoloUfficioModel();
                            rum = dtrd.GetRuoloDipendenteByIdTrasferimento(tm.idTrasferimento, dtDatiParametri).RuoloUfficio;

                            dit.RuoloUfficio = rum;
                        }

                        if (dit.statoTrasferimento == EnumStatoTraferimento.Attivo || dit.statoTrasferimento == EnumStatoTraferimento.Da_Attivare)
                        {
                            using (CalcoliIndennita ci = new CalcoliIndennita(tm.idTrasferimento))
                            {
                                dit.indennitaBase = ci.IndennitaDiBase;
                                dit.indennitaServizio = ci.IndennitaDiServizio;
                                dit.maggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                                dit.indennitaPersonale = ci.IndennitaPersonale;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dit;
        }


        public void TerminaTrasferimento(decimal idTrasferimentoOld, DateTime dataPartenzaNewTrasf, ModelDBISE db)
        {

            try
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimentoOld);


                if (t.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato)
                {
                    if (!t.DATARIENTRO.HasValue)
                    {
                        t.DATARIENTRO = dataPartenzaNewTrasf.AddDays(-1);

                        int i = db.SaveChanges();
                        if (i <= 0)
                        {
                            throw new Exception("Impossibile terminare il trasferimento.");
                        }

                        this.SetStatoTrasferimento(idTrasferimentoOld, EnumStatoTraferimento.Terminato, db);

                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public void SetStatoTrasferimento(decimal idTrasferimento, EnumStatoTraferimento stato)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                t.IDSTATOTRASFERIMENTO = (decimal)stato;



                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Non è stato possibile modificare lo stato del trasferimento." + " Stato: " + stato.ToString() + " Trasferimento: " + idTrasferimento);
                }



            }
        }
        public void SetStatoTrasferimento(decimal idTrasferimento, EnumStatoTraferimento stato, ModelDBISE db)
        {

            var t = db.TRASFERIMENTO.Find(idTrasferimento);

            t.IDSTATOTRASFERIMENTO = (decimal)stato;



            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Non è stato possibile modificare lo stato del trasferimento." + " Stato: " + stato.ToString() + " Trasferimento: " + idTrasferimento);
            }

        }

        public void SetTrasferimento(ref TrasferimentoModel trm, ModelDBISE db)
        {
            TRASFERIMENTO tr;

            tr = new TRASFERIMENTO()
            {
                IDTIPOTRASFERIMENTO = trm.idTipoTrasferimento,
                IDUFFICIO = trm.idUfficio,
                IDSTATOTRASFERIMENTO = (decimal)trm.idStatoTrasferimento,
                IDDIPENDENTE = trm.idDipendente,
                IDTIPOCOAN = trm.idTipoCoan,
                DATAPARTENZA = trm.dataPartenza,
                DATARIENTRO = trm.dataRientro,
                COAN = trm.coan?.ToUpper(),
                PROTOCOLLOLETTERA = trm.protocolloLettera?.ToUpper(),
                DATALETTERA = trm.dataLettera,
                NOTIFICATRASFERIMENTO = trm.notificaTrasferimento,
                DATAAGGIORNAMENTO = trm.dataAggiornamento,
            };

            db.TRASFERIMENTO.Add(tr);

            int i = db.SaveChanges();

            if (i > 0)
            {
                trm.idTrasferimento = tr.IDTRASFERIMENTO;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo trasferimento.", "Trasferimento", db, trm.idTrasferimento, trm.idTrasferimento);
            }



        }

        public void EditTrasferimento(TrasferimentoModel trm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                TRASFERIMENTO tr = db.TRASFERIMENTO.Find(trm.idTrasferimento);

                if (tr != null && tr.IDTRASFERIMENTO > 0)
                {
                    tr.IDTIPOTRASFERIMENTO = trm.idTipoTrasferimento;
                    tr.IDUFFICIO = trm.idUfficio;
                    tr.IDSTATOTRASFERIMENTO = (decimal)trm.idStatoTrasferimento;
                    tr.IDDIPENDENTE = trm.idDipendente;
                    tr.IDTIPOCOAN = trm.idTipoCoan;
                    tr.DATAPARTENZA = trm.dataPartenza;
                    tr.DATARIENTRO = trm.dataRientro;
                    tr.COAN = trm.coan.ToUpper();
                    tr.PROTOCOLLOLETTERA = trm.protocolloLettera.ToUpper();
                    tr.DATALETTERA = trm.dataLettera;
                    tr.DATAAGGIORNAMENTO = trm.dataAggiornamento;

                    if (db.SaveChanges() > 0)
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica del trasferimento.", "Trasferimento", db, tr.IDTRASFERIMENTO, tr.IDTRASFERIMENTO);
                    }


                }
            }

        }

        public void EditTrasferimento(TrasferimentoModel trm, ModelDBISE db)
        {
            TRASFERIMENTO tr = db.TRASFERIMENTO.Find(trm.idTrasferimento);

            if (tr != null && tr.IDTRASFERIMENTO > 0)
            {
                tr.IDTIPOTRASFERIMENTO = trm.idTipoTrasferimento > 0 ? trm.idTipoTrasferimento : tr.IDTIPOTRASFERIMENTO;
                tr.IDUFFICIO = trm.idUfficio > 0 ? trm.idUfficio : tr.IDUFFICIO;
                tr.IDSTATOTRASFERIMENTO = Convert.ToDecimal(trm.idStatoTrasferimento) > 0 ? (decimal)trm.idStatoTrasferimento : tr.IDSTATOTRASFERIMENTO;
                tr.IDDIPENDENTE = trm.idDipendente > 0 ? trm.idDipendente : tr.IDDIPENDENTE;
                tr.IDTIPOCOAN = trm.idTipoCoan > 0 ? trm.idTipoCoan : tr.IDTIPOCOAN;
                tr.DATAPARTENZA = trm.dataPartenza > DateTime.MinValue ? trm.dataPartenza : tr.DATAPARTENZA;
                tr.DATARIENTRO = trm.dataRientro ?? tr.DATARIENTRO;
                tr.COAN = trm.coan ?? tr.COAN;
                tr.PROTOCOLLOLETTERA = trm.protocolloLettera ?? tr.PROTOCOLLOLETTERA;
                tr.DATALETTERA = trm.dataLettera ?? tr.DATALETTERA;
                tr.DATAAGGIORNAMENTO = trm.dataAggiornamento > DateTime.MinValue ? trm.dataAggiornamento : tr.DATAAGGIORNAMENTO;


                if (db.SaveChanges() > 0)
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica del trasferimento.", "Trasferimento", db, tr.IDTRASFERIMENTO, tr.IDTRASFERIMENTO);
                }

            }
        }

        public TrasferimentoModel GetTrasferimentoOldPrimaDiTrasfAnnullato(decimal idTrasfAnnullato)
        {
            TrasferimentoModel trm = new TrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var trAnn = db.TRASFERIMENTO.Find(idTrasfAnnullato);
                var lTrBefore =
                    db.TRASFERIMENTO.Where(
                        a =>
                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                            a.IDDIPENDENTE == trAnn.IDDIPENDENTE &&
                            a.DATARIENTRO < trAnn.DATAPARTENZA).OrderByDescending(a => a.DATARIENTRO).ToList();

                if (lTrBefore?.Any() ?? false)
                {
                    var trb = lTrBefore.First();

                    trm = new TrasferimentoModel()
                    {
                        idTrasferimento = trb.IDTRASFERIMENTO,
                        idTipoTrasferimento = trb.IDTIPOTRASFERIMENTO,
                        idUfficio = trb.IDUFFICIO,
                        idStatoTrasferimento = (EnumStatoTraferimento)trb.IDSTATOTRASFERIMENTO,
                        idDipendente = trb.IDDIPENDENTE,
                        idTipoCoan = trb.IDTIPOCOAN,
                        dataPartenza = trb.DATAPARTENZA,
                        dataRientro = trb.DATARIENTRO,
                        coan = trb.COAN,
                        protocolloLettera = trb.PROTOCOLLOLETTERA,
                        dataLettera = trb.DATALETTERA,
                        notificaTrasferimento = trb.NOTIFICATRASFERIMENTO,
                        dataAggiornamento = trb.DATAAGGIORNAMENTO,
                    };

                }

                return trm;
            }
        }

        public TrasferimentoModel GetSoloTrasferimentoById(decimal idTrasferimento)
        {
            TrasferimentoModel trm = new TrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tr = db.TRASFERIMENTO.Find(idTrasferimento);
                if (tr != null && tr.IDTRASFERIMENTO > 0)
                {
                    trm = new TrasferimentoModel()
                    {
                        idTrasferimento = tr.IDTRASFERIMENTO,
                        idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                        idUfficio = tr.IDUFFICIO,
                        idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                        idDipendente = tr.IDDIPENDENTE,
                        idTipoCoan = tr.IDTIPOCOAN,
                        dataPartenza = tr.DATAPARTENZA,
                        dataRientro = tr.DATARIENTRO,
                        coan = tr.COAN,
                        protocolloLettera = tr.PROTOCOLLOLETTERA,
                        dataLettera = tr.DATALETTERA,
                        notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                        dataAggiornamento = tr.DATAAGGIORNAMENTO,

                    };
                }

            }

            return trm;

        }

        public TrasferimentoModel GetSoloTrasferimentoById(decimal idTrasferimento, ModelDBISE db)
        {
            TrasferimentoModel trm = new TrasferimentoModel();

            var tr = db.TRASFERIMENTO.Find(idTrasferimento);
            if (tr != null && tr.IDTRASFERIMENTO > 0)
            {
                trm = new TrasferimentoModel()
                {
                    idTrasferimento = tr.IDTRASFERIMENTO,
                    idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                    idUfficio = tr.IDUFFICIO,
                    idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                    idDipendente = tr.IDDIPENDENTE,
                    idTipoCoan = tr.IDTIPOCOAN,
                    dataPartenza = tr.DATAPARTENZA,
                    dataRientro = tr.DATARIENTRO,
                    coan = tr.COAN,
                    protocolloLettera = tr.PROTOCOLLOLETTERA,
                    dataLettera = tr.DATALETTERA,
                    notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                    dataAggiornamento = tr.DATAAGGIORNAMENTO,

                };
            }

            return trm;

        }

        public TrasferimentoModel GetTrasferimentoByIdAttPassaporto(decimal idAttivazionePassaporto)
        {
            TrasferimentoModel trm = new TrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tr = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto).PASSAPORTI.TRASFERIMENTO;

                trm = new TrasferimentoModel()
                {
                    idTrasferimento = tr.IDTRASFERIMENTO,
                    idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                    idUfficio = tr.IDUFFICIO,
                    idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                    idDipendente = tr.IDDIPENDENTE,
                    idTipoCoan = tr.IDTIPOCOAN,
                    dataPartenza = tr.DATAPARTENZA,
                    dataRientro = tr.DATARIENTRO,
                    coan = tr.COAN,
                    protocolloLettera = tr.PROTOCOLLOLETTERA,
                    dataLettera = tr.DATALETTERA,
                    notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                    dataAggiornamento = tr.DATAAGGIORNAMENTO,
                    StatoTrasferimento = new StatoTrasferimentoModel()
                    {
                        idStatoTrasferimento = tr.STATOTRASFERIMENTO.IDSTATOTRASFERIMENTO,
                        descrizioneStatoTrasferimento = tr.STATOTRASFERIMENTO.DESCRIZIONE
                    },
                    TipoTrasferimento = new TipoTrasferimentoModel()
                    {
                        idTipoTrasferimento = tr.TIPOTRASFERIMENTO.IDTIPOTRASFERIMENTO,
                        descTipoTrasf = tr.TIPOTRASFERIMENTO.TIPOTRASFERIMENTO1
                    },
                    Ufficio = new UfficiModel()
                    {
                        idUfficio = tr.UFFICI.IDUFFICIO,
                        codiceUfficio = tr.UFFICI.CODICEUFFICIO,
                        descUfficio = tr.UFFICI.DESCRIZIONEUFFICIO
                    },
                    Dipendente = new DipendentiModel()
                    {
                        idDipendente = tr.DIPENDENTI.IDDIPENDENTE,
                        matricola = tr.DIPENDENTI.MATRICOLA,
                        nome = tr.DIPENDENTI.NOME,
                        cognome = tr.DIPENDENTI.COGNOME,
                        dataAssunzione = tr.DIPENDENTI.DATAASSUNZIONE,
                        dataCessazione = tr.DIPENDENTI.DATACESSAZIONE,
                        indirizzo = tr.DIPENDENTI.INDIRIZZO,
                        cap = tr.DIPENDENTI.CAP,
                        citta = tr.DIPENDENTI.CITTA,
                        provincia = tr.DIPENDENTI.PROVINCIA,
                        email = tr.DIPENDENTI.EMAIL,
                        telefono = tr.DIPENDENTI.TELEFONO,
                        fax = tr.DIPENDENTI.FAX,
                        abilitato = tr.DIPENDENTI.ABILITATO,
                        dataInizioRicalcoli = tr.DIPENDENTI.DATAINIZIORICALCOLI
                    },
                    TipoCoan = new TipologiaCoanModel()
                    {
                        idTipoCoan = tr.TIPOLOGIACOAN.IDTIPOCOAN,
                        descrizione = tr.TIPOLOGIACOAN.DESCRIZIONE
                    }
                };
            }

            return trm;

        }

        public TrasferimentoModel GetTrasferimentoByIdPassaporto(decimal idPassaporto)
        {
            TrasferimentoModel trm = new TrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var p = db.PASSAPORTI.Find(idPassaporto);
                var tr = p.TRASFERIMENTO;
                if (tr != null && tr.IDTRASFERIMENTO > 0)
                {
                    trm = new TrasferimentoModel()
                    {
                        idTrasferimento = tr.IDTRASFERIMENTO,
                        idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                        idUfficio = tr.IDUFFICIO,
                        idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                        idDipendente = tr.IDDIPENDENTE,
                        idTipoCoan = tr.IDTIPOCOAN,
                        dataPartenza = tr.DATAPARTENZA,
                        dataRientro = tr.DATARIENTRO,
                        coan = tr.COAN,
                        protocolloLettera = tr.PROTOCOLLOLETTERA,
                        dataLettera = tr.DATALETTERA,
                        notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                        dataAggiornamento = tr.DATAAGGIORNAMENTO,
                        StatoTrasferimento = new StatoTrasferimentoModel()
                        {
                            idStatoTrasferimento = tr.STATOTRASFERIMENTO.IDSTATOTRASFERIMENTO,
                            descrizioneStatoTrasferimento = tr.STATOTRASFERIMENTO.DESCRIZIONE
                        },
                        TipoTrasferimento = new TipoTrasferimentoModel()
                        {
                            idTipoTrasferimento = tr.TIPOTRASFERIMENTO.IDTIPOTRASFERIMENTO,
                            descTipoTrasf = tr.TIPOTRASFERIMENTO.TIPOTRASFERIMENTO1
                        },
                        Ufficio = new UfficiModel()
                        {
                            idUfficio = tr.UFFICI.IDUFFICIO,
                            codiceUfficio = tr.UFFICI.CODICEUFFICIO,
                            descUfficio = tr.UFFICI.DESCRIZIONEUFFICIO
                        },
                        Dipendente = new DipendentiModel()
                        {
                            idDipendente = tr.DIPENDENTI.IDDIPENDENTE,
                            matricola = tr.DIPENDENTI.MATRICOLA,
                            nome = tr.DIPENDENTI.NOME,
                            cognome = tr.DIPENDENTI.COGNOME,
                            dataAssunzione = tr.DIPENDENTI.DATAASSUNZIONE,
                            dataCessazione = tr.DIPENDENTI.DATACESSAZIONE,
                            indirizzo = tr.DIPENDENTI.INDIRIZZO,
                            cap = tr.DIPENDENTI.CAP,
                            citta = tr.DIPENDENTI.CITTA,
                            provincia = tr.DIPENDENTI.PROVINCIA,
                            email = tr.DIPENDENTI.EMAIL,
                            telefono = tr.DIPENDENTI.TELEFONO,
                            fax = tr.DIPENDENTI.FAX,
                            abilitato = tr.DIPENDENTI.ABILITATO,
                            dataInizioRicalcoli = tr.DIPENDENTI.DATAINIZIORICALCOLI
                        },
                        TipoCoan = new TipologiaCoanModel()
                        {
                            idTipoCoan = tr.TIPOLOGIACOAN.IDTIPOCOAN,
                            descrizione = tr.TIPOLOGIACOAN.DESCRIZIONE
                        }
                    };
                }

            }

            return trm;

        }

        public TrasferimentoModel GetTrasferimentoById(decimal idTrasferimento)
        {
            TrasferimentoModel trm = new TrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tr = db.TRASFERIMENTO.Find(idTrasferimento);
                if (tr != null && tr.IDTRASFERIMENTO > 0)
                {
                    trm = new TrasferimentoModel()
                    {
                        idTrasferimento = tr.IDTRASFERIMENTO,
                        idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                        idUfficio = tr.IDUFFICIO,
                        idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                        idDipendente = tr.IDDIPENDENTE,
                        idTipoCoan = tr.IDTIPOCOAN,
                        dataPartenza = tr.DATAPARTENZA,
                        dataRientro = tr.DATARIENTRO,
                        coan = tr.COAN,
                        protocolloLettera = tr.PROTOCOLLOLETTERA,
                        dataLettera = tr.DATALETTERA,
                        notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                        dataAggiornamento = tr.DATAAGGIORNAMENTO,
                        StatoTrasferimento = new StatoTrasferimentoModel()
                        {
                            idStatoTrasferimento = tr.STATOTRASFERIMENTO.IDSTATOTRASFERIMENTO,
                            descrizioneStatoTrasferimento = tr.STATOTRASFERIMENTO.DESCRIZIONE
                        },
                        TipoTrasferimento = new TipoTrasferimentoModel()
                        {
                            idTipoTrasferimento = tr.TIPOTRASFERIMENTO.IDTIPOTRASFERIMENTO,
                            descTipoTrasf = tr.TIPOTRASFERIMENTO.TIPOTRASFERIMENTO1
                        },
                        Ufficio = new UfficiModel()
                        {
                            idUfficio = tr.UFFICI.IDUFFICIO,
                            codiceUfficio = tr.UFFICI.CODICEUFFICIO,
                            descUfficio = tr.UFFICI.DESCRIZIONEUFFICIO
                        },
                        Dipendente = new DipendentiModel()
                        {
                            idDipendente = tr.DIPENDENTI.IDDIPENDENTE,
                            matricola = tr.DIPENDENTI.MATRICOLA,
                            nome = tr.DIPENDENTI.NOME,
                            cognome = tr.DIPENDENTI.COGNOME,
                            dataAssunzione = tr.DIPENDENTI.DATAASSUNZIONE,
                            dataCessazione = tr.DIPENDENTI.DATACESSAZIONE,
                            indirizzo = tr.DIPENDENTI.INDIRIZZO,
                            cap = tr.DIPENDENTI.CAP,
                            citta = tr.DIPENDENTI.CITTA,
                            provincia = tr.DIPENDENTI.PROVINCIA,
                            email = tr.DIPENDENTI.EMAIL,
                            telefono = tr.DIPENDENTI.TELEFONO,
                            fax = tr.DIPENDENTI.FAX,
                            abilitato = tr.DIPENDENTI.ABILITATO,
                            dataInizioRicalcoli = tr.DIPENDENTI.DATAINIZIORICALCOLI
                        },
                        TipoCoan = new TipologiaCoanModel()
                        {
                            idTipoCoan = tr.TIPOLOGIACOAN.IDTIPOCOAN,
                            descrizione = tr.TIPOLOGIACOAN.DESCRIZIONE
                        }
                    };
                }

            }

            return trm;

        }


        public TrasferimentoModel GetTrasferimentoById(decimal idTrasferimento, ModelDBISE db)
        {
            TrasferimentoModel trm = new TrasferimentoModel();


            var tr = db.TRASFERIMENTO.Find(idTrasferimento);
            if (tr != null && tr.IDTRASFERIMENTO > 0)
            {
                trm = new TrasferimentoModel()
                {
                    idTrasferimento = tr.IDTRASFERIMENTO,
                    idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                    idUfficio = tr.IDUFFICIO,
                    idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                    idDipendente = tr.IDDIPENDENTE,
                    idTipoCoan = tr.IDTIPOCOAN,
                    dataPartenza = tr.DATAPARTENZA,
                    dataRientro = tr.DATARIENTRO,
                    coan = tr.COAN,
                    protocolloLettera = tr.PROTOCOLLOLETTERA,
                    dataLettera = tr.DATALETTERA,
                    notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                    dataAggiornamento = tr.DATAAGGIORNAMENTO

                };


                using (dtStatoTrasferimento dtst = new dtStatoTrasferimento())
                {
                    trm.StatoTrasferimento = dtst.GetStatoTrasferimentoByID(trm.idStatoTrasferimento, db);
                }

                using (dtTipoTrasferimento dttt = new dtTipoTrasferimento())
                {
                    trm.TipoTrasferimento = dttt.GetTipoTrasferimentoByID(trm.idTipoTrasferimento, db);
                }

                using (dtUffici dtu = new dtUffici())
                {
                    trm.Ufficio = dtu.GetUffici(trm.idUfficio, db);
                }

                using (dtDipendenti dtd = new dtDipendenti())
                {
                    trm.Dipendente = dtd.GetDipendenteByID(trm.idDipendente, db);
                }

                using (dtTipologiaCoan dttc = new dtTipologiaCoan())
                {
                    trm.TipoCoan = dttc.GetTipologiaCoanByID(trm.idTipoCoan, db);
                }
            }

            return trm;

        }


        public bool NotificaTrasferimento(decimal idTrasferimento)
        {
            bool ret = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                //db.Database.BeginTransaction();

                var tr = db.TRASFERIMENTO.Find(idTrasferimento);

                if (tr != null && tr.IDTRASFERIMENTO > 0)
                {
                    tr.NOTIFICATRASFERIMENTO = true;

                    var i = db.SaveChanges();
                    if (i > 0)
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Notifica del trasferimento.", "Trasferimento", db, idTrasferimento, tr.IDTRASFERIMENTO);
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                else
                {
                    throw new Exception("Nessun trasferimento per l'ID: " + idTrasferimento);
                }

            }

            return ret;
        }

        public TrasferimentoModel GetTrasferimentoByIdTitoloViaggio(decimal idTitoloViaggio)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tv = db.TITOLIVIAGGIO.Find(idTitoloViaggio);
                var tr = tv.TRASFERIMENTO;

                tm = new TrasferimentoModel()
                {
                    idTrasferimento = tr.IDTRASFERIMENTO,
                    idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                    idUfficio = tr.IDUFFICIO,
                    idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                    idDipendente = tr.IDDIPENDENTE,
                    idTipoCoan = tr.IDTIPOCOAN,
                    dataPartenza = tr.DATAPARTENZA,
                    dataRientro = tr.DATARIENTRO,
                    coan = tr.COAN,
                    protocolloLettera = tr.PROTOCOLLOLETTERA,
                    dataLettera = tr.DATALETTERA,
                    notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                    dataAggiornamento = tr.DATAAGGIORNAMENTO
                };
            }

            return tm;
        }

        public TrasferimentoModel GetTrasferimentoByIdSosp(decimal idSospensione)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var s = db.SOSPENSIONE.Find(idSospensione);
                var tr = s.TRASFERIMENTO;

                tm = new TrasferimentoModel()
                {
                    idTrasferimento = tr.IDTRASFERIMENTO,
                    idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                    idUfficio = tr.IDUFFICIO,
                    idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                    idDipendente = tr.IDDIPENDENTE,
                    idTipoCoan = tr.IDTIPOCOAN,
                    dataPartenza = tr.DATAPARTENZA,
                    dataRientro = tr.DATARIENTRO,
                    coan = tr.COAN,
                    protocolloLettera = tr.PROTOCOLLOLETTERA,
                    dataLettera = tr.DATALETTERA,
                    notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                    dataAggiornamento = tr.DATAAGGIORNAMENTO
                };
            }

            return tm;
        }

        public TrasferimentoModel GetTrasferimentoByIDProvvScolastiche(decimal idTrasfProvScolastiche)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var s = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche);
                var tr = s.TRASFERIMENTO;

                tm = new TrasferimentoModel()
                {
                    idTrasferimento = tr.IDTRASFERIMENTO,
                    idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                    idUfficio = tr.IDUFFICIO,
                    idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                    idDipendente = tr.IDDIPENDENTE,
                    idTipoCoan = tr.IDTIPOCOAN,
                    dataPartenza = tr.DATAPARTENZA,
                    dataRientro = tr.DATARIENTRO,
                    coan = tr.COAN,
                    protocolloLettera = tr.PROTOCOLLOLETTERA,
                    dataLettera = tr.DATALETTERA,
                    notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                    dataAggiornamento = tr.DATAAGGIORNAMENTO
                };
            }

            return tm;
        }

        public TrasferimentoModel GetTrasferimentoByRichiamo(decimal idTrasfRichiamo)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var s = db.RICHIAMO.Find(idTrasfRichiamo);
                var tr = s.TRASFERIMENTO;

                tm = new TrasferimentoModel()
                {
                    idTrasferimento = tr.IDTRASFERIMENTO,
                    idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                    idUfficio = tr.IDUFFICIO,
                    idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                    idDipendente = tr.IDDIPENDENTE,
                    idTipoCoan = tr.IDTIPOCOAN,
                    dataPartenza = tr.DATAPARTENZA,
                    dataRientro = tr.DATARIENTRO,
                    coan = tr.COAN,
                    protocolloLettera = tr.PROTOCOLLOLETTERA,
                    dataLettera = tr.DATALETTERA,
                    notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                    dataAggiornamento = tr.DATAAGGIORNAMENTO
                };
            }

            return tm;
        }

        public void AllineaDateTrasferimentoPartenza(TRASFERIMENTO t, ModelDBISE db, DateTime dataPartenzaOriginale)
        {
            try
            {
                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                {

                    #region modello parziale del trasferimento
                    TrasferimentoModel tm = new TrasferimentoModel()
                    {
                        idTrasferimento=t.IDTRASFERIMENTO
                    };
                    #endregion

                    #region allinea data ruolo dipendente
                    var rd = t.RUOLODIPENDENTE.First();
                    rd.DATAINZIOVALIDITA = t.DATAPARTENZA;
                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore di correzione data inizio validita ruolo dipendente da " + rd.DATAINZIOVALIDITA + " a " + t.DATAPARTENZA);
                    }
                    #endregion

                    #region legge indennita
                    var i = t.INDENNITA;
                    #endregion

                    #region riassocia livelli dipendente
                    using (dtLivelliDipendente dtld = new dtLivelliDipendente())
                    {
                        var lld = i.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false).ToList();
                        foreach (var ld in lld)
                        {
                            i.LIVELLIDIPENDENTI.Remove(ld);
                        }
                        var lldm = 
                            dtld.GetLivelliDipendentiByRangeDate(t.IDDIPENDENTE, t.DATAPARTENZA,
                                t.DATARIENTRO.Value, db).ToList();
                        if (lldm?.Any() ?? false)
                        {
                            foreach (var ldm in lldm)
                            {
                                dtld.AssociaLivelloDipendente_Indennita(t.IDTRASFERIMENTO,
                                    ldm.idLivDipendente, db);

                                using (dtIndennitaBase dtib = new dtIndennitaBase())
                                {
                                    List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

                                    DateTime dataInizio = Utility.GetData_Inizio_Base();
                                    DateTime dataFine = Utility.DataFineStop();

                                    if (t.DATAPARTENZA > ldm.dataInizioValdita)
                                    {
                                        dataInizio = t.DATAPARTENZA;
                                    }
                                    else
                                    {
                                        dataInizio = t.DATAPARTENZA;
                                    }

                                    if (t.DATARIENTRO.HasValue)
                                    {
                                        if (t.DATARIENTRO > ldm.dataFineValidita)
                                        {
                                            dataFine = ldm.dataFineValidita.HasValue == true
                                                ? ldm.dataFineValidita.Value
                                                : Utility.DataFineStop();
                                        }
                                        else
                                        {
                                            dataFine = t.DATARIENTRO.Value;
                                        }
                                    }
                                    else
                                    {
                                        dataFine = ldm.dataFineValidita.HasValue == true
                                            ? ldm.dataFineValidita.Value
                                            : Utility.DataFineStop();
                                    }

                                    libm =
                                        dtib.GetIndennitaBaseByRangeDate(ldm.idLivello, dataInizio,
                                            dataFine, db).ToList();

                                    if (libm?.Any() ?? false)
                                    {
                                        foreach (var ibm in libm)
                                        {
                                            dtib.AssociaIndennitaBase_Indennita(t.IDTRASFERIMENTO, ibm.idIndennitaBase, db);
                                        }


                                    }
                                    else
                                    {
                                        throw new Exception("Non risulta l'indennità base per il livello interessato.");
                                    }
                                }

                            }
                        }
                        else
                        {
                            throw new Exception("Non risulta assegnato nessun livello per il dipendente " + t.DIPENDENTI.COGNOME + " " + t.DIPENDENTI.NOME + " (" + t.DIPENDENTI.MATRICOLA + ")");
                        }
                    }
                    #endregion

                    #region riassocia TFR
                    using (dtTFR dttfr = new dtTFR())
                    {
                        var ltfr = i.TFR.Where(a=>a.ANNULLATO==false).ToList();
                        foreach (var tfr in ltfr)
                        {
                            i.TFR.Remove(tfr);
                        }

                        List<TFRModel> ltfrm =
                            dttfr.GetTfrIndennitaByRangeDate(t.IDUFFICIO, t.DATAPARTENZA,
                                t.DATARIENTRO.Value, db).ToList();

                        if (ltfrm?.Any() ?? false)
                        {
                            foreach (var tfrm in ltfrm)
                            {
                                dttfr.AssociaTFR_Indennita(t.IDTRASFERIMENTO, tfrm.idTFR, db);
                            }
                        }
                        else
                        {
                            throw new Exception("Non risulta il tasso fisso di ragguaglio per l'ufficio interessato.");
                        }
                    }
                    #endregion

                    #region riassocia percentuale disagio
                    using (dtPercentualeDisagio dtpd = new dtPercentualeDisagio())
                    {
                        var lpd = i.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false).ToList();
                        foreach (var pd in lpd)
                        {
                            i.PERCENTUALEDISAGIO.Remove(pd);
                        }

                        List<PercentualeDisagioModel> lpdm =
                            dtpd.GetPercentualeDisagioIndennitaByRange(t.IDUFFICIO,
                                t.DATAPARTENZA, t.DATARIENTRO.Value, db).ToList();


                        if (lpdm?.Any() ?? false)
                        {
                            foreach (var pdm in lpdm)
                            {
                                dtpd.AssociaPercentualeDisagio_Indennita(t.IDTRASFERIMENTO, pdm.idPercentualeDisagio, db);
                            }
                        }
                        else
                        {
                            throw new Exception("Non risulta la percentuale di disagio per l'ufficio interessato.");
                        }


                    }
                    #endregion

                    #region riassocia coefficiente sede
                    using (dtCoefficenteSede dtcs = new dtCoefficenteSede())
                    {
                        var lcs = i.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false).ToList();
                        foreach (var cs in lcs)
                        {
                            i.COEFFICIENTESEDE.Remove(cs);
                        }

                        List<CoefficientiSedeModel> lcsm =
                            dtcs.GetCoefficenteSedeIndennitaByRangeDate(t.IDUFFICIO,
                                t.DATAPARTENZA, t.DATARIENTRO.Value, db).ToList();

                        if (lcsm?.Any() ?? false)
                        {
                            foreach (var csm in lcsm)
                            {
                                dtcs.AssociaCoefficenteSede_Indennita(t.IDTRASFERIMENTO, csm.idCoefficientiSede, db);
                            }
                        }
                        else
                        {
                            throw new Exception("Non risulta il valore di coefficiente di sede per l'ufficio interessato.");
                        }
                    }
                    #endregion

                    #region riassocia fascia KM
                    using (dtFasciaKm dtfkm = new dtFasciaKm())
                    {
                        using (dtPrimaSistemazione dtps = new dtPrimaSistemazione())
                        {
                            var psm = dtps.GetPrimaSistemazioneBtIdTrasf(t.IDTRASFERIMENTO, db);
                            var ps = db.PRIMASITEMAZIONE.Find(psm.idPrimaSistemazione);
                            var lpfk = ps.PERCENTUALEFKM.Where(a => a.ANNULLATO == false).ToList();
                            foreach (var pfk in lpfk)
                            {
                                ps.PERCENTUALEFKM.Remove(pfk);
                            }

                            var pfkmm = dtfkm.GetPercentualeFKM(lpfk.First().IDFKM, t.DATAPARTENZA, db);
                            if (pfkmm?.idPFKM > 0)
                            {
                                dtfkm.AssociaPercentualeFKMPrimaSistemazione(psm.idPrimaSistemazione, pfkmm.idPFKM, db);
                            }
                            else
                            {
                                throw new Exception("Non risulta il valore della percentuale fascia chilometrica.");
                            }
                        }
                    }
                    #endregion

                    #region legge MaggiorazioniFamiliari
                    var mf = t.MAGGIORAZIONIFAMILIARI;
                    #endregion

                    #region allinea date Maggiorazioni Familiari Coniuge e riassocia perc magg coniuge
                    var lc = mf.CONIUGE.Where(a => 
                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato
                        //&& a.DATAINIZIOVALIDITA<t.DATAPARTENZA
                        ).ToList();
                    if(lc?.Any()??false)
                    {
                        foreach(var c in lc)
                        {
                            if (c.DATAINIZIOVALIDITA == dataPartenzaOriginale ||
                                c.DATAINIZIOVALIDITA <t.DATAPARTENZA)
                            {
                                c.DATAINIZIOVALIDITA = t.DATAPARTENZA;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore di correzione data inizio validita coniuge " + c.COGNOME + " " + c.NOME + " da " + c.DATAINIZIOVALIDITA + " a " + t.DATAPARTENZA);
                                }
                            
                                using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                                {
                                    //elimina le associazioni perc magg coniuge
                                    var lpmc = c.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false).ToList();
                                    foreach(var pmc in lpmc)
                                    {
                                        c.PERCENTUALEMAGCONIUGE.Remove(pmc);
                                    }

                                    //ricalcola perc magg coniuge
                                    DateTime dtIni = c.DATAINIZIOVALIDITA;
                                    DateTime dtFin = c.DATAFINEVALIDITA;

                                    List<PercentualeMagConiugeModel> lpmcm =
                                        dtpc.GetListaPercentualiMagConiugeByRangeDate((EnumTipologiaConiuge)c.IDTIPOLOGIACONIUGE, dtIni, dtFin, db)
                                            .ToList();

                                    if (lpmcm?.Any() ?? false)
                                    {
                                        foreach (var pmcm in lpmcm)
                                        {
                                            dtpc.AssociaPercentualeMaggiorazioneConiuge(c.IDCONIUGE, pmcm.idPercentualeConiuge, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Non è presente nessuna percentuale del coniuge.");
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region allinea date Maggiorazioni Familiari Pensione Coniuge
                    //prende tutti i coniugi validi
                    lc = mf.CONIUGE.Where(a =>
                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                    if (lc?.Any() ?? false)
                    {
                        foreach (var c in lc)
                        {
                            var lpc = c.PENSIONE.Where(a => 
                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato
                                            //&& a.DATAINIZIO<t.DATAPARTENZA
                                            ).ToList();
                            foreach(var pc in lpc)
                            {
                                if (pc.DATAINIZIO == dataPartenzaOriginale ||
                                    pc.DATAINIZIO < t.DATAPARTENZA)
                                {
                                    pc.DATAINIZIO = t.DATAPARTENZA;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore di correzione data inizio pensione coniuge " + c.COGNOME + " " + c.NOME + " da " + pc.DATAINIZIO + " a " + t.DATAPARTENZA);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
    
                    #region allinea date Maggiorazioni Familiari Figli e riassocia perc magg figli e perc primo segretario
                    var lf = mf.FIGLI.Where(a =>
                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato
                        //&& a.DATAINIZIOVALIDITA < t.DATAPARTENZA
                        ).ToList();
                    if (lf?.Any() ?? false)
                    {
                        foreach (var f in lf)
                        {
                            if (f.DATAINIZIOVALIDITA == dataPartenzaOriginale ||
                                f.DATAINIZIOVALIDITA < t.DATAPARTENZA)
                            {

                                f.DATAINIZIOVALIDITA = t.DATAPARTENZA;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore di correzione data inizio validita figlio " + f.COGNOME + " " + f.NOME + " da " + f.DATAINIZIOVALIDITA + " a " + t.DATAPARTENZA);
                                }
                                using (dtPercentualeMagFigli dtpmf = new dtPercentualeMagFigli())
                                {
                                    //elimina le associazioni perc magg figli
                                    var lpmf = f.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false).ToList();
                                    foreach (var pmf in lpmf)
                                    {
                                        f.PERCENTUALEMAGFIGLI.Remove(pmf);
                                    }
    
                                    //ricalcola perc magg figli
                                    DateTime dtIni = f.DATAINIZIOVALIDITA;
                                    DateTime dtFin = f.DATAFINEVALIDITA;
    
                                    List<PercentualeMagFigliModel> lpmfm =
                                        dtpmf.GetPercentualeMaggiorazioneFigli((EnumTipologiaFiglio)f.IDTIPOLOGIAFIGLIO, dtIni, dtFin, db)
                                            .ToList();
    
                                    if (lpmfm?.Any() ?? false)
                                    {
                                        foreach (var pmfm in lpmfm)
                                        {
                                            dtpmf.AssociaPercentualeMaggiorazioneFigli(f.IDFIGLI, pmfm.idPercMagFigli, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Non è presente nessuna percentuale maggiorazione figli.");
                                    }
                                }

                                using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                                {
                                    //elimina le associazioni perc indennita primo segretario
                                    var lpps = f.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).ToList();
                                    foreach (var pps in lpps)
                                    {
                                        f.INDENNITAPRIMOSEGRETARIO.Remove(pps);
                                    }

                                    DateTime dtIni = f.DATAINIZIOVALIDITA;
                                    DateTime dtFin = f.DATAFINEVALIDITA;

                                    List<IndennitaPrimoSegretModel> lipsm =
                                        dtips.GetIndennitaPrimoSegretario(dtIni, dtFin, db).ToList();

                                    if (lipsm?.Any() ?? false)
                                    {
                                        foreach (var ipsm in lipsm)
                                        {
                                            dtips.AssociaIndennitaPrimoSegretarioFiglio(f.IDFIGLI, ipsm.idIndPrimoSegr, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Non è presente nessuna indennità di primo segretario per il figlio " + f.COGNOME + " " + f.NOME + ".");
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region legge Maggiorazioni Abitazione
                    var lma = t.MAGGIORAZIONEABITAZIONE.Where(a => a.VARIAZIONE == false).OrderBy(a=>a.IDMAB).ToList();
                    if (!lma?.Any() ?? false)
                    {
                        throw new Exception("Maggiorazione Abitazione non trovata.");
                    }
                    var ma = lma.First();
                    #endregion

                    #region allinea date VariazioniMAB e riassocia percMAB e magg annuali
                    var lmann = ma.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false).ToList();
                    foreach (var mann in lmann)
                    {
                        ma.MAGGIORAZIONIANNUALI.Remove(mann);
                    }

                    VariazioniMABModel vmabm = new VariazioniMABModel();

                    var lvma = ma.VARIAZIONIMAB.Where(a =>
                             a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                    if (lvma?.Any() ?? false)
                    {
                        foreach (var vma in lvma)
                        {
                            vma.DATAINIZIOMAB = t.DATAPARTENZA;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore di correzione data inizio maggiorazione abitazione su VariazioniMAB da " + vma.DATAINIZIOMAB + " a " + t.DATAPARTENZA);
                            }

                            //elimina le associazioni percentualeMAB variazioniMAB
                            var lpm = vma.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
                            foreach (var pm in lpm)
                            {
                                vma.PERCENTUALEMAB.Remove(pm);
                            }

                            vmabm = new VariazioniMABModel()
                            {
                                idVariazioniMAB =vma.IDVARIAZIONIMAB
                            };

                            var lpmab = dtma.GetListaPercentualeMAB(vmabm, tm, db);
                            foreach (var pmab in lpmab)
                            {
                                dtma.Associa_VariazioniMAB_PercentualeMAB(vmabm.idVariazioniMAB, pmab.IDPERCMAB, db);
                            }
                        }

                        vmabm = new VariazioniMABModel()
                        {
                            idVariazioniMAB = lvma.First().IDVARIAZIONIMAB
                        };

                        //riassocia maggiorazioni annuali
                        var mam = dtma.GetMaggiorazioneAnnuale(vmabm, db);
                        if (mam.idMagAnnuali > 0)
                        {
                            if (mam.annualita)
                            {
                                dtma.Associa_MAB_MaggiorazioniAnnuali(ma.IDMAB, mam.idMagAnnuali, db);
                            }
                        }
                    }
                    #endregion

                    #region allinea date PagatoCondivisoMAB e riassocia perc condiviso MAB
                    var lpcma = ma.PAGATOCONDIVISOMAB.Where(a =>
                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                    if (lpcma?.Any() ?? false)
                    {
                        foreach (var pcma in lpcma)
                        {
                            pcma.DATAINIZIOVALIDITA = t.DATAPARTENZA;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore di correzione data inizio validita su PagatoCondivisoMAB da " + pcma.DATAINIZIOVALIDITA + " a " + t.DATAPARTENZA);
                            }

                            var lpc = pcma.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).ToList();
                            foreach (var pc in lpc)
                            {
                                pcma.PERCENTUALECONDIVISIONE.Remove(pc);
                            }

                            lpc = dtma.GetListaPercentualeCondivisione(pcma.DATAINIZIOVALIDITA, pcma.DATAFINEVALIDITA, db);
                            foreach (var pc in lpc)
                            {
                                dtma.Associa_PagatoCondivisoMAB_PercentualeCondivisione(pcma.IDPAGATOCONDIVISO, pc.IDPERCCOND, db);
                            }
                        }
                    }
                    #endregion

                    #region allinea date CanoneMAB e riassocia TFR
                    var lcma = ma.CANONEMAB.Where(a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                    if (lcma?.Any() ?? false)
                    {
                        foreach (var cma in lcma)
                        {
                            cma.DATAINIZIOVALIDITA = t.DATAPARTENZA;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore di correzione data inizio validita su CanoneMAB da " + cma.DATAINIZIOVALIDITA + " a " + t.DATAPARTENZA);
                            }

                            var ltfr = cma.TFR.Where(a => a.ANNULLATO == false).ToList();
                            foreach (var tfr in ltfr)
                            {
                                cma.TFR.Remove(tfr);
                            }

                            using (dtTFR dtTfr = new dtTFR())
                            {
                                var ltfrm = dtTfr.GetListaTfrByValuta_RangeDate(tm, cma.IDVALUTA, cma.DATAINIZIOVALIDITA, cma.DATAFINEVALIDITA, db);
                           

                                foreach (var tfrm in ltfrm)
                                {
                                    dtma.Associa_TFR_CanoneMAB(tfrm.idTFR, cma.IDCANONE, db);
                                }
                            }

                        }
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void AttivaTrasf(decimal idTrasferimento, DateTime dataPartenzaEffettiva)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    if (t?.IDTRASFERIMENTO > 0)
                    {
                        if (t.NOTIFICATRASFERIMENTO == true)
                        {
                            DateTime dataPartenzaOriginale = t.DATAPARTENZA;

                            t.IDSTATOTRASFERIMENTO = (decimal)EnumStatoTraferimento.Attivo;
                            t.DATAAGGIORNAMENTO = DateTime.Now;
                            t.DATAPARTENZA = dataPartenzaEffettiva;
                            t.DATARIENTRO = (t.DATARIENTRO == null) ? Utility.DataFineStop() : t.DATARIENTRO;


                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile completare l'attivazione del trasferimento.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Attivazione trasferimento.", "TRASFERIMENTO", db,
                                    t.IDTRASFERIMENTO, t.IDTRASFERIMENTO);


                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(t.IDTRASFERIMENTO, EnumFunzioniEventi.AttivaTrasferimento, db);
                                }

                                if (dataPartenzaOriginale != dataPartenzaEffettiva)
                                {
                                    this.AllineaDateTrasferimentoPartenza(t, db, dataPartenzaOriginale);
                                }

                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        using (dtUffici dtu = new dtUffici())
                                        {
                                            var dip = dtd.GetDipendenteByID(t.IDDIPENDENTE);
                                            var uff = dtu.GetUffici(t.IDUFFICIO);

                                            var messaggioAttiva = Resources.msgEmail.MessaggioAttivaTrasferimento;
                                            var oggettoAttiva = Resources.msgEmail.OggettoAttivaTrasferimento;


                                            EmailTrasferimento.EmailAttiva(t.IDTRASFERIMENTO,
                                                                            oggettoAttiva,
                                                                            string.Format(messaggioAttiva, dip.cognome + " " + dip.nome + " (" + dip.matricola + ") ", uff.descUfficio + " (" + uff.codiceUfficio + ")", t.DATAPARTENZA.ToShortDateString()),
                                                                            db);

                                        }
                                    }
                                }

                                //this.EmailAttivaTrasf(t.IDTRASFERIMENTO, db);

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

        public void AnnullaTrasf(decimal idTrasferimento, string testoAnnullaTrasf)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    if (t?.IDTRASFERIMENTO > 0)
                    {
                        if (t.NOTIFICATRASFERIMENTO == true)
                        {
                            t.IDSTATOTRASFERIMENTO = (decimal)EnumStatoTraferimento.Annullato;
                            t.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile completare l'annullamento del trasferimento.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Annullamento trasferimento.", "TRASFERIMENTO", db,
                                    t.IDTRASFERIMENTO, t.IDTRASFERIMENTO);

                                EmailTrasferimento.EmailAnnulla(t.IDTRASFERIMENTO,
                                                                Resources.msgEmail.OggettoAnnullamentoTrasferimento,
                                                                testoAnnullaTrasf,
                                                                db);
                                //this.EmailAnnullaTrasf(t.IDTRASFERIMENTO, testoAnnullaTrasf, db);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.AnnullaMessaggioEvento(t.IDTRASFERIMENTO, EnumFunzioniEventi.AttivaTrasferimento, db);
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

        private void EmailAttivaTrasf(decimal idTrasferimento, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();


            try
            {
                am = Utility.UtenteAutorizzato();
                mittente.Nominativo = am.nominativo;
                mittente.EmailMittente = am.eMail;

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                if (t?.IDTRASFERIMENTO > 0)
                {
                    DIPENDENTI d = t.DIPENDENTI;
                    UFFICI u = t.UFFICI;

                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                    {
                        using (GestioneEmail gmail = new GestioneEmail())
                        {
                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                            {

                                cc = new Destinatario()
                                {
                                    Nominativo = am.nominativo,
                                    EmailDestinatario = am.eMail
                                };

                                msgMail.mittente = mittente;
                                msgMail.cc.Add(cc);

                                luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore).ToList());

                                foreach (var uam in luam)
                                {
                                    var amministratore = db.DIPENDENTI.Find(uam.idDipendente);
                                    if (amministratore != null && amministratore.IDDIPENDENTE > 0)
                                    {
                                        to = new Destinatario()
                                        {
                                            Nominativo = amministratore.COGNOME + " " + amministratore.NOME,
                                            EmailDestinatario = amministratore.EMAIL
                                        };

                                        msgMail.destinatario.Add(to);
                                    }


                                }
                                msgMail.oggetto = Resources.msgEmail.OggettoAttivaTrasferimento;

                                msgMail.corpoMsg =
                                        string.Format(
                                            Resources.msgEmail.MessaggioAttivaTrasferimento,
                                            d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                                            t.DATAPARTENZA.ToLongDateString(),
                                            u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")");

                                gmail.sendMail(msgMail);

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

        public void EmailAnnullaTrasf(decimal idTrasferimento, string testoAnnullaTrasf, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();

            try
            {
                am = Utility.UtenteAutorizzato();
                if (am.RuoloAccesso.idRuoloAccesso != (decimal)EnumRuoloAccesso.SuperAmministratore)
                {
                    mittente.Nominativo = am.nominativo;
                    mittente.EmailMittente = am.eMail;
                }

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                if (t?.IDTRASFERIMENTO > 0)
                {
                    DIPENDENTI dip = t.DIPENDENTI;
                    UFFICI uff = t.UFFICI;

                    using (GestioneEmail gmail = new GestioneEmail())
                    {
                        using (ModelloMsgMail msgMail = new ModelloMsgMail())
                        {
                            //cc = new Destinatario()
                            //{
                            //    Nominativo = am.nominativo,
                            //    EmailDestinatario = am.eMail
                            //};

                            to = new Destinatario()
                            {
                                Nominativo = dip.NOME + " " + dip.COGNOME,
                                EmailDestinatario = dip.EMAIL,
                            };

                            var lua = db.UTENTIAUTORIZZATI.Where(a => a.IDRUOLOUTENTE == (decimal)EnumRuoloAccesso.Amministratore).ToList();
                            foreach (var ua in lua)
                            {
                                var dipAdmin = ua.DIPENDENTI;

                                if (dipAdmin != null)
                                {
                                    cc = new Destinatario()
                                    {
                                        Nominativo = dipAdmin.NOME + " " + dipAdmin.COGNOME,
                                        EmailDestinatario = dipAdmin.EMAIL,
                                    };

                                    msgMail.cc.Add(cc);
                                }
                            }

                            msgMail.mittente = mittente;
                            //msgMail.cc.Add(cc);
                            msgMail.destinatario.Add(to);

                            msgMail.oggetto =
                            Resources.msgEmail.OggettoAnnullamentoTrasferimento;
                            //msgMail.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullamentoTrasferimento, dip.NOME + " " + dip.COGNOME, uff.DESCRIZIONEUFFICIO + " (" + uff.CODICEUFFICIO + ")");
                            msgMail.corpoMsg = testoAnnullaTrasf;

                            gmail.sendMail(msgMail);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public TrasferimentoModel GetTrasferimentoByIdPrimaSistemazione(decimal idPrimaSistemazione)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ps = db.PRIMASITEMAZIONE.Find(idPrimaSistemazione);
                var tr = ps.TRASFERIMENTO;

                tm = new TrasferimentoModel()
                {
                    idTrasferimento = tr.IDTRASFERIMENTO,
                    idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                    idUfficio = tr.IDUFFICIO,
                    idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                    idDipendente = tr.IDDIPENDENTE,
                    idTipoCoan = tr.IDTIPOCOAN,
                    dataPartenza = tr.DATAPARTENZA,
                    dataRientro = tr.DATARIENTRO,
                    coan = tr.COAN,
                    protocolloLettera = tr.PROTOCOLLOLETTERA,
                    dataLettera = tr.DATALETTERA,
                    notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                    dataAggiornamento = tr.DATAAGGIORNAMENTO
                };
            }

            return tm;
        }

        public TrasferimentoModel GetTrasferimentoByIdMAB(decimal idMAB)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mab = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
                var tr = mab.TRASFERIMENTO;

                tm = new TrasferimentoModel()
                {
                    idTrasferimento = tr.IDTRASFERIMENTO,
                    idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                    idUfficio = tr.IDUFFICIO,
                    idStatoTrasferimento = (EnumStatoTraferimento)tr.IDSTATOTRASFERIMENTO,
                    idDipendente = tr.IDDIPENDENTE,
                    idTipoCoan = tr.IDTIPOCOAN,
                    dataPartenza = tr.DATAPARTENZA,
                    dataRientro = tr.DATARIENTRO,
                    coan = tr.COAN,
                    protocolloLettera = tr.PROTOCOLLOLETTERA,
                    dataLettera = tr.DATALETTERA,
                    notificaTrasferimento = tr.NOTIFICATRASFERIMENTO,
                    dataAggiornamento = tr.DATAAGGIORNAMENTO
                };
            }

            return tm;
        }

        public ModelloMsgMail GetMessaggioAnnullaTrasf(decimal idTrasferimento)
        {
            ModelloMsgMail msg = new ModelloMsgMail();

            try
            {
                using (dtDipendenti dtd = new dtDipendenti())
                {
                    using (dtUffici dtu = new dtUffici())
                    {
                        var t = this.GetTrasferimentoById(idTrasferimento);

                        if (t?.idTrasferimento > 0)
                        {
                            var dip = dtd.GetDipendenteByID(t.idDipendente);
                            var uff = dtu.GetUffici(t.idUfficio);

                            msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullamentoTrasferimento, dip.nome + " " + dip.cognome, uff.descUfficio + " (" + uff.codiceUfficio + ")");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return msg;

        }


    }
}