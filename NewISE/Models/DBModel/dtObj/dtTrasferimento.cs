using NewISE.Models.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

        public static ValidationResult VerificaRequiredDataLettera(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var tr = context.ObjectInstance as TrasferimentoModel;

            if (tr != null)
            {
                if ((tr.protocolloLettera != null && tr.protocolloLettera.Trim() != string.Empty) || tr.allegaDocumento == true)
                {
                    if (tr.dataLettera.HasValue)
                    {
                        vr = ValidationResult.Success;
                    }
                    else
                    {
                        vr = new ValidationResult("La data della lettera è richiesta.");
                    }
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }

            return vr;
        }

        public static ValidationResult VerificaRequiredDocumentoLettera(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var tr = context.ObjectInstance as TrasferimentoModel;

            if (tr != null)
            {
                if (tr.dataLettera.HasValue || (tr.protocolloLettera != null && tr.protocolloLettera.Trim() != string.Empty))
                {
                    if (tr.allegaDocumento == true)
                    {
                        vr = ValidationResult.Success;
                    }
                    else
                    {
                        vr = new ValidationResult("L'opzione allega lettera di trasferimento è richiesta.");
                    }
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = ValidationResult.Success;
            }

            return vr;
        }

        public static ValidationResult VerificaRequiredProtocolloLettera(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var tr = context.ObjectInstance as TrasferimentoModel;

            if (tr != null)
            {
                if (tr.dataLettera.HasValue || tr.allegaDocumento == true)
                {
                    if (tr.protocolloLettera != null && tr.protocolloLettera.Trim() != string.Empty)
                    {
                        vr = ValidationResult.Success;
                    }
                    else
                    {
                        vr = new ValidationResult("Il Protocollo della lettera è richiesto.");
                    }
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = ValidationResult.Success;
            }

            return vr;
        }

        public IList<TrasferimentoModel> GetTrasferimentiPrecedenti(decimal idDipendente, DateTime dataPartenza)
        {
            List<TrasferimentoModel> ltm = new List<TrasferimentoModel>();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                var lt = db.TRASFERIMENTO.Where(a => a.IDDIPENDENTE == idDipendente && a.ANNULLATO == false && a.DATAPARTENZA < dataPartenza).ToList();

                ltm = (from t in lt
                       select new TrasferimentoModel()
                       {
                           idTrasferimento = t.IDTRASFERIMENTO,
                           idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                           idUfficio = t.IDUFFICIO,
                           idStatoTrasferimento = t.IDSTATOTRASFERIMENTO,
                           idDipendente = t.IDDIPENDENTE,
                           idTipoCoan = t.IDTIPOCOAN,
                           dataPartenza = t.DATAPARTENZA,
                           dataRientro = t.DATARIENTRO,
                           coan = t.COAN,
                           protocolloLettera = t.PROTOCOLLOLETTERA,
                           dataLettera = t.DATALETTERA,
                           dataAggiornamento = t.DATAAGGIORNAMENTO,
                           annullato = t.ANNULLATO,
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
                           RuoloUfficio = new RuoloUfficioModel()
                           {
                               idRuoloUfficio = t.INDENNITA.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.DATAINIZIO).First().RUOLODIPENDENTE.RUOLOUFFICIO.IDRUOLO,
                               DescrizioneRuolo = t.INDENNITA.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.DATAINIZIO).First().RUOLODIPENDENTE.RUOLOUFFICIO.DESCRUOLO
                           }
                       }).ToList();
            }

            return ltm;
        }

        public TrasferimentoModel GetUltimoTrasferimentoByMatricola(string matricola)
        {
            TrasferimentoModel tm = new TrasferimentoModel();
            int matr = Convert.ToInt16(matricola);
            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var ldp = db.DIPENDENTI.Where(a => a.MATRICOLA == matr);

                    if (ldp != null && ldp.Count() > 0)
                    {
                        var lt = ldp.First().TRASFERIMENTO.Where(a => a.ANNULLATO == false).ToList();

                        if (lt != null && lt.Count() > 0)
                        {
                            var t = lt.OrderBy(a => a.DATAPARTENZA).Last();

                            //var ruoloUfficio = t.RUOLODIPENDENTE.Where(a => a.IDTRASFERIMENTO == t.IDTRASFERIMENTO &&
                            //                                           a.ANNULLATO == false &&
                            //                                           t.DATAPARTENZA >= a.DATAINZIOVALIDITA &&
                            //                                           t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                            //                                    .OrderByDescending(a => a.DATAINZIOVALIDITA).First().RUOLOUFFICIO;

                            tm = new TrasferimentoModel()
                            {
                                idTrasferimento = t.IDTRASFERIMENTO,
                                idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                                idUfficio = t.IDUFFICIO,
                                idStatoTrasferimento = t.IDSTATOTRASFERIMENTO,
                                idDipendente = t.IDDIPENDENTE,
                                idTipoCoan = t.IDTIPOCOAN,
                                dataPartenza = t.DATAPARTENZA,
                                dataRientro = t.DATARIENTRO,
                                coan = t.COAN,
                                protocolloLettera = t.PROTOCOLLOLETTERA,
                                dataLettera = t.DATALETTERA,
                                dataAggiornamento = t.DATAAGGIORNAMENTO,
                                annullato = t.ANNULLATO,
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
                                RuoloUfficio = new RuoloUfficioModel()
                                {
                                    idRuoloUfficio = t.INDENNITA.Where(a=>a.ANNULLATO == false).OrderByDescending(a=>a.DATAINIZIO).First().RUOLODIPENDENTE.RUOLOUFFICIO.IDRUOLO,
                                    DescrizioneRuolo = t.INDENNITA.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.DATAINIZIO).First().RUOLODIPENDENTE.RUOLOUFFICIO.DESCRUOLO
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
                            dtDatiParametri = DateTime.Now.Date;
                        }

                        using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                        {
                            RuoloUfficioModel rum = new RuoloUfficioModel();
                            rum = dtru.GetRuoloDipendente(tm.idTrasferimento, dtDatiParametri).RuoloUfficio;

                            dit.RuoloUfficio = rum;
                        }

                        if (dit.statoTrasferimento == EnumStatoTraferimento.Attivo || dit.statoTrasferimento == EnumStatoTraferimento.Da_Attivare)
                        {
                            //TODO:Inserire codice per prelevare le informazioni di: indennità di base; indennità di servizio; maggiorazioni famigliari; indennità personale.
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

        public void SetTrasferimento(ref TrasferimentoModel trm, EntitiesDBISE db)
        {
            TRASFERIMENTO tr;

            tr = new TRASFERIMENTO()
            {
                IDTIPOTRASFERIMENTO = trm.idTipoTrasferimento,
                IDUFFICIO = trm.idUfficio,
                IDSTATOTRASFERIMENTO = trm.idStatoTrasferimento,
                IDDIPENDENTE = trm.idDipendente,
                IDTIPOCOAN = trm.idTipoCoan,
                DATAPARTENZA = trm.dataPartenza,
                DATARIENTRO = trm.dataRientro,
                COAN = trm.coan,
                PROTOCOLLOLETTERA = trm.protocolloLettera,
                DATALETTERA = trm.dataLettera,
                DATAAGGIORNAMENTO = trm.dataAggiornamento,
                ANNULLATO = trm.annullato
            };

            db.TRASFERIMENTO.Add(tr);

            db.SaveChanges();

            using (dtLogAttivita dtla=new dtLogAttivita())
            {
                LogAttivitaModel lam = new LogAttivitaModel();

                lam.idUtenteLoggato = Utility.UtenteAutorizzato().idUtenteAutorizzato;
                lam.idTrasferimento = tr.IDTRASFERIMENTO;
                lam.idAttivitaCrud = (decimal)EnumAttivitaCrud.Inserimento;
                lam.dataOperazione = DateTime.Now;
                lam.descAttivitaSvolta = "Inserimento di un nuovo trasferimento.";
                lam.idTabellaCoinvolta = tr.IDTRASFERIMENTO;

                dtla.SetLogAttivita(lam, db);
            }



        }

        public void EditTrasferimento(TrasferimentoModel trm, EntitiesDBISE db)
        {
            TRASFERIMENTO tr = db.TRASFERIMENTO.Find(trm.idTrasferimento);

            if (tr != null && tr.IDTRASFERIMENTO > 0)
            {
                tr.IDTIPOTRASFERIMENTO = trm.idTipoTrasferimento;
                tr.IDUFFICIO = trm.idUfficio;
                tr.IDSTATOTRASFERIMENTO = trm.idStatoTrasferimento;
                tr.IDDIPENDENTE = trm.idDipendente;
                tr.IDTIPOCOAN = trm.idTipoCoan;
                tr.DATAPARTENZA = trm.dataPartenza;
                tr.DATARIENTRO = trm.dataRientro;
                tr.COAN = trm.coan;
                tr.PROTOCOLLOLETTERA = trm.protocolloLettera;
                tr.DATALETTERA = trm.dataLettera;
                tr.DATAAGGIORNAMENTO = trm.dataAggiornamento;
                tr.ANNULLATO = trm.annullato;

                db.SaveChanges();

                using (dtLogAttivita dtla = new dtLogAttivita())
                {
                    LogAttivitaModel lam = new LogAttivitaModel();

                    lam.idUtenteLoggato = Utility.UtenteAutorizzato().idUtenteAutorizzato;
                    lam.idTrasferimento = tr.IDTRASFERIMENTO;
                    lam.idAttivitaCrud = (decimal)EnumAttivitaCrud.Modifica;
                    lam.dataOperazione = DateTime.Now;
                    lam.descAttivitaSvolta = "Modifica del trasferimento.";
                    lam.idTabellaCoinvolta = tr.IDTRASFERIMENTO;

                    dtla.SetLogAttivita(lam, db);
                }
            }
        }
    }
}