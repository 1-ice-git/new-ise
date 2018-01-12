using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

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


        public bool EsisteTrasferimentoSuccessivo(decimal idTrasferimento)
        {
            bool ret = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var ts = db.TRASFERIMENTO.Where(a => a.DATAPARTENZA > t.DATARIENTRO);

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
                                                      out bool richiesteTV, out bool concluseTV)
        {
            richiestaMF = false;
            attivazioneMF = false;

            richiestaPP = false;
            conclusePP = false;

            richiesteTV = false;
            concluseTV = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

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
                    var lap = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONIPASSAPORTI);

                    if (lap?.Any() ?? false)
                    {
                        var ap = lap.First();

                        richiestaPP = ap.NOTIFICARICHIESTA;
                        conclusePP = ap.PRATICACONCLUSA;
                    }
                    else
                    {
                        throw new Exception("Errore 'GestioneAttivitaTrasferimento' record ATTIVAZIONIPASSAPORTI non trovato.");
                    }


                }
                #endregion

                #region Titoli di viaggio

                //var tv = t.TITOLIVIAGGIO.OrderBy(a => a.IDTITOLOVIAGGIO).First();
                //if (tv != null && tv.IDTITOLOVIAGGIO > 0)
                //{
                //    richiesteTV = tv.NOTIFICARICHIESTA;
                //    concluseTV = tv.PRATICACONCLUSA;
                //}

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
                            using (CalcoliIndennita ci = new CalcoliIndennita(tm.Dipendente.matricola.ToString()))
                            {
                                dit.indennitaBase = ci.indennitaBaseRiduzione;
                                dit.indennitaServizio = ci.indennitaServizio;
                                dit.maggiorazioniFamiliari = ci.maggiorazioneFamiliari;
                                dit.indennitaPersonale = ci.indennitaPersonaleTeorica;
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
                            using (CalcoliIndennita ci = new CalcoliIndennita(tm.Dipendente.matricola.ToString()))
                            {
                                dit.indennitaBase = ci.indennitaBaseRiduzione;
                                dit.indennitaServizio = ci.indennitaServizio;
                                dit.maggiorazioniFamiliari = ci.maggiorazioneFamiliari;
                                dit.indennitaPersonale = ci.indennitaPersonaleTeorica;
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
                    trm.StatoTrasferimento = dtst.GetStatoTrasferimentoByID(trm.idStatoTrasferimento);
                }

                using (dtTipoTrasferimento dttt = new dtTipoTrasferimento())
                {
                    trm.TipoTrasferimento = dttt.GetTipoTrasferimentoByID(trm.idTipoTrasferimento);
                }

                using (dtUffici dtu = new dtUffici())
                {
                    trm.Ufficio = dtu.GetUffici(trm.idUfficio);
                }

                using (dtDipendenti dtd = new dtDipendenti())
                {
                    trm.Dipendente = dtd.GetDipendenteByID(trm.idDipendente);
                }

                using (dtTipologiaCoan dttc = new dtTipologiaCoan())
                {
                    trm.TipoCoan = dttc.GetTipologiaCoanByID(trm.idTipoCoan);
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
    }
}