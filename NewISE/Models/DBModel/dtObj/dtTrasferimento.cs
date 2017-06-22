using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
                if ((tr.protocolloLettera != null && tr.protocolloLettera.Trim() != string.Empty) || tr.file != null || tr.idDocumento > 0)
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

        public static ValidationResult VerificaRequiredDocumentoLettera(HttpPostedFileBase v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var tr = context.ObjectInstance as TrasferimentoModel;

            if (tr != null)
            {
                if (tr.dataLettera.HasValue || (tr.protocolloLettera != null && tr.protocolloLettera.Trim() != string.Empty))
                {
                    if (tr.file != null || tr.idDocumento > 0)
                    {
                        vr = ValidationResult.Success;
                    }
                    else
                    {
                        vr = new ValidationResult("La lettera di trasferimento è richiesta.");
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
                if (tr.dataLettera.HasValue || tr.file != null || tr.idDocumento > 0)
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
                       }).ToList();
            }

            return ltm;
        }

        public TrasferimentoModel GetUltimoTrasferimentoByMatricola(string matricola)
        {
            TrasferimentoModel tm = new TrasferimentoModel();
            int matr = Convert.ToInt16(matricola);
            DateTime dtDatiParametri = DateTime.Now;

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var ldp = db.DIPENDENTI.Where(a => a.MATRICOLA == matr).ToList();

                    if (ldp != null && ldp.Count() > 0)
                    {
                        var lt = ldp.First().TRASFERIMENTO.Where(a => a.ANNULLATO == false).ToList();

                        if (lt != null && lt.Count() > 0)
                        {
                            var t = lt.OrderBy(a => a.DATAPARTENZA).Last();

                            if (t.DATARIENTRO.HasValue)
                            {
                                dtDatiParametri = t.DATARIENTRO.Value;
                            }
                            else
                            {
                                dtDatiParametri = t.DATAPARTENZA > Utility.GetDtInizioMeseCorrente() ? t.DATAPARTENZA : Utility.GetDtInizioMeseCorrente();
                            }

                            RUOLOUFFICIO ru = new RUOLOUFFICIO();
                            var lrd = t.INDENNITA.RUOLODIPENDENTE.Where(a => a.ANNULLATO == false && dtDatiParametri >= a.DATAINZIOVALIDITA && dtDatiParametri <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINZIOVALIDITA).ToList();
                            if (lrd != null && lrd.Count() > 0)
                            {
                                ru = lrd.First().RUOLOUFFICIO;
                            }

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
                                idRuoloUfficio = ru.IDRUOLO,
                                RuoloUfficio = new RuoloUfficioModel()
                                {
                                    idRuoloUfficio = ru.IDRUOLO,
                                    DescrizioneRuolo = ru.DESCRUOLO
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

        public TrasferimentoModel GetUltimoTrasferimentoByMatricola(string matricola, EntitiesDBISE db)
        {
            TrasferimentoModel tm = new TrasferimentoModel();
            int matr = Convert.ToInt16(matricola);
            try
            {
                var ldp = db.DIPENDENTI.Where(a => a.MATRICOLA == matr).ToList();

                if (ldp != null && ldp.Count() > 0)
                {
                    var lt = ldp.First().TRASFERIMENTO.Where(a => a.ANNULLATO == false).ToList();

                    if (lt != null && lt.Count() > 0)
                    {
                        List<IndennitaModel> lim = new List<IndennitaModel>();
                        var t = lt.OrderBy(a => a.DATAPARTENZA).Last();

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

            if (db.SaveChanges() > 0)
            {
                trm.idTrasferimento = tr.IDTRASFERIMENTO;
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

                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica del trasferimento.", "Trasferimento", db, tr.IDTRASFERIMENTO, tr.IDTRASFERIMENTO);
            }
        }

        public TrasferimentoModel GetTrasferimentoById(decimal idTrasferimento)
        {
            TrasferimentoModel trm = new TrasferimentoModel();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                var tr = db.TRASFERIMENTO.Find(idTrasferimento);
                if (tr != null && tr.IDTRASFERIMENTO > 0)
                {
                    trm = new TrasferimentoModel()
                    {
                        idTrasferimento = tr.IDTRASFERIMENTO,
                        idTipoTrasferimento = tr.IDTIPOTRASFERIMENTO,
                        idUfficio = tr.IDUFFICIO,
                        idStatoTrasferimento = tr.IDSTATOTRASFERIMENTO,
                        idDipendente = tr.IDDIPENDENTE,
                        idTipoCoan = tr.IDTIPOCOAN,
                        dataPartenza = tr.DATAPARTENZA,
                        dataRientro = tr.DATARIENTRO,
                        coan = tr.COAN,
                        protocolloLettera = tr.PROTOCOLLOLETTERA,
                        dataLettera = tr.DATALETTERA,
                        dataAggiornamento = tr.DATAAGGIORNAMENTO,
                        annullato = tr.ANNULLATO
                    };
                }
                
            }

             return trm;

        }

        public bool NotificaTrasferimento(decimal idTrasferimento)
        {
            bool ret = false;

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                var tr = db.TRASFERIMENTO.Find(idTrasferimento);

                if (tr != null && tr.IDTRASFERIMENTO > 0 )
                {
                    tr.IDSTATOTRASFERIMENTO = (decimal)EnumStatoTraferimento.Attivo;

                    var i = db.SaveChanges();
                    if (i > 0)
                    {
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
    }
}