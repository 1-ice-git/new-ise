using NewISE.EF;
using NewISE.Models.dtObj;
using NewISE.Models.ViewModel;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRuoloDipendente : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }



        public RuoloDipendenteModel GetRuoloDipendentePartenza(decimal idTrasferimento)
        {
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                var lrd = t.RUOLODIPENDENTE.Where(a => a.ANNULLATO == false && a.DATAINZIOVALIDITA == t.DATAPARTENZA).OrderBy(a => a.DATAINZIOVALIDITA);
                if (lrd?.Any() ?? false)
                {
                    var rd = lrd.First();
                    rdm = new RuoloDipendenteModel()
                    {
                        idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                        idRuolo = rd.IDRUOLO,
                        idTrasferimento = rd.IDTRASFERIMENTO,
                        dataInizioValidita = rd.DATAINZIOVALIDITA,
                        dataFineValidita = rd.DATAFINEVALIDITA,
                        dataAggiornamento = rd.DATAAGGIORNAMENTO,
                        annullato = rd.ANNULLATO,
                        RuoloUfficio = new RuoloUfficioModel()
                        {
                            idRuoloUfficio = rd.RUOLOUFFICIO.IDRUOLO,
                            DescrizioneRuolo = rd.RUOLOUFFICIO.DESCRUOLO
                        }
                    };
                }
                else
                {
                    throw new Exception("Non risulta nessun ruolo dipendente per il trasferimento verso " + t.UFFICI.DESCRIZIONEUFFICIO + " del " + t.DATAPARTENZA.Date);
                }

            }
            return rdm;
        }
        public IList<RuoloDipendenteModel> GetRuoliDipendenteIndennitaByRangeDate(decimal idRuolo, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            List<RuoloDipendenteModel> lrdm = new List<RuoloDipendenteModel>();

            var r = db.RUOLOUFFICIO.Find(idRuolo);

            var lrd =
                r.RUOLODIPENDENTE.Where(
                    a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= dtIni && a.DATAINZIOVALIDITA <= dtFin)
                    .OrderBy(a => a.DATAINZIOVALIDITA);

            if (lrd?.Any() ?? false)
            {
                foreach (var rd in lrd)
                {
                    var rdm = new RuoloDipendenteModel()
                    {
                        idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                        idRuolo = rd.IDRUOLO,
                        idTrasferimento = rd.IDTRASFERIMENTO,
                        dataInizioValidita = rd.DATAINZIOVALIDITA,
                        dataFineValidita = rd.DATAFINEVALIDITA,
                        dataAggiornamento = rd.DATAAGGIORNAMENTO,
                        annullato = rd.ANNULLATO,
                        RuoloUfficio = new RuoloUfficioModel()
                        {
                            idRuoloUfficio = rd.RUOLOUFFICIO.IDRUOLO,
                            DescrizioneRuolo = rd.RUOLOUFFICIO.DESCRUOLO
                        }
                    };

                    lrdm.Add(rdm);
                }
            }

            return lrdm;

        }

        public RuoloDipendenteModel GetRuoloDipendenteById(decimal idRuoloDipendente)
        {

            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var rd = db.RUOLODIPENDENTE.Find(idRuoloDipendente);
                rdm = new RuoloDipendenteModel()
                {
                    idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                    idTrasferimento = rd.IDTRASFERIMENTO,
                    idRuolo = rd.IDRUOLO,
                    dataInizioValidita = rd.DATAINZIOVALIDITA,
                    dataFineValidita = rd.DATAFINEVALIDITA,
                    dataAggiornamento = rd.DATAAGGIORNAMENTO,
                    annullato = rd.ANNULLATO,
                    RuoloUfficio = new RuoloUfficioModel()
                    {
                        idRuoloUfficio = rd.RUOLOUFFICIO.IDRUOLO,
                        DescrizioneRuolo = rd.RUOLOUFFICIO.DESCRUOLO
                    }
                };
            }


            return rdm;
        }
        public IList<RuoloUfficioModel> GetIndennitaBaseComuneRuoloDipendente(decimal idTrasferimento)
        {
            List<RuoloUfficioModel> libm = new List<RuoloUfficioModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                       var ll = db.TRASFERIMENTO.Find(idTrasferimento).RUOLODIPENDENTE.Where(a => a.ANNULLATO == false).ToList();
                    
                        libm = (from e in ll
                                select new RuoloUfficioModel()
                                {
                                    idRuoloUfficio = e.RUOLOUFFICIO.IDRUOLO,
                                    DescrizioneRuolo = e.RUOLOUFFICIO.DESCRUOLO,
                                    
                                }).ToList();
                    }

                    return libm;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public RuoloDipendenteModel GetRuoloDipendenteByIdIndennita(decimal idTrasferimento)
        {

            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lrd = db.TRASFERIMENTO.Find(idTrasferimento).RUOLODIPENDENTE.Where(a => a.ANNULLATO == false);


                var rd = lrd.First();
                if (lrd?.Any() ?? false)
                {
                    rdm = new RuoloDipendenteModel()
                    {
                        idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                        idTrasferimento = rd.IDTRASFERIMENTO,
                        idRuolo = rd.IDRUOLO,
                        dataInizioValidita = rd.DATAINZIOVALIDITA,
                        dataFineValidita = rd.DATAFINEVALIDITA,
                        dataAggiornamento = rd.DATAAGGIORNAMENTO,
                        annullato = rd.ANNULLATO,
                        RuoloUfficio = new RuoloUfficioModel()
                        {
                            idRuoloUfficio = rd.RUOLOUFFICIO.IDRUOLO,
                            DescrizioneRuolo = rd.RUOLOUFFICIO.DESCRUOLO
                        }
                    };
                }
            }
            return rdm;
        }
        public RuoloDipendenteModel GetRuoloDipendenteByIdTrasferimento(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            var t = db.TRASFERIMENTO.Find(idTrasferimento);

            if (t != null && t.IDTRASFERIMENTO > 0)
            {
                var lrd =
                    t.RUOLODIPENDENTE.Where(
                        a => a.ANNULLATO == false && dt >= a.DATAINZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                        .OrderByDescending(a => a.DATAINZIOVALIDITA)
                        .ToList();

                if (lrd?.Any() ?? false)
                {
                    var rd = lrd.First();

                    rdm = new RuoloDipendenteModel()
                    {
                        idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                        idRuolo = rd.IDRUOLO,
                        idTrasferimento = rd.IDTRASFERIMENTO,
                        dataInizioValidita = rd.DATAINZIOVALIDITA,
                        dataFineValidita = rd.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : rd.DATAFINEVALIDITA,
                        dataAggiornamento = rd.DATAAGGIORNAMENTO,
                        annullato = rd.ANNULLATO,
                        RuoloUfficio = new RuoloUfficioModel()
                        {
                            idRuoloUfficio = rd.RUOLOUFFICIO.IDRUOLO,
                            DescrizioneRuolo = rd.RUOLOUFFICIO.DESCRUOLO
                        }
                    };
                }


            }
            return rdm;

        }
        public RuoloDipendenteModel GetRuoloDipendenteByIdTrasferimento(decimal idTrasferimento, DateTime dt)
        {
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                if (dt.Date < t.DATAPARTENZA)
                {
                    dt = t.DATAPARTENZA;
                }

                if (dt.Date > t.DATARIENTRO)
                {
                    dt = t.DATARIENTRO;
                }

                if (t != null && t.IDTRASFERIMENTO > 0)
                {
                    var lrd =
                        t.RUOLODIPENDENTE.Where(
                            a => a.ANNULLATO == false && dt >= a.DATAINZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                            .OrderByDescending(a => a.DATAINZIOVALIDITA)
                            .ToList();


                    if (lrd != null && lrd.Count > 0)
                    {
                        var rd = lrd.First();
                        rdm = new RuoloDipendenteModel()
                        {
                            idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                            idRuolo = rd.IDRUOLO,
                            idTrasferimento = rd.IDTRASFERIMENTO,
                            dataInizioValidita = rd.DATAINZIOVALIDITA,
                            dataFineValidita = rd.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : rd.DATAFINEVALIDITA,
                            dataAggiornamento = rd.DATAAGGIORNAMENTO,
                            annullato = rd.ANNULLATO,
                            RuoloUfficio = new RuoloUfficioModel()
                            {
                                idRuoloUfficio = rd.RUOLOUFFICIO.IDRUOLO,
                                DescrizioneRuolo = rd.RUOLOUFFICIO.DESCRUOLO
                            }
                        };
                    }


                }
            }

            return rdm;

        }
        public void SetNuovoRuoloDipendente(ref RuoloDipendenteModel rdm, ModelDBISE db)
        {
            decimal idTrasferimento = rdm.idTrasferimento;
            DateTime dtIni = rdm.dataInizioValidita;
            DateTime dtFin = rdm.dataFineValidita.HasValue == true ? rdm.dataFineValidita.Value : Utility.DataFineStop();

            var lrd =
                db.RUOLODIPENDENTE.Where(
                    a =>
                        a.ANNULLATO == false && a.IDTRASFERIMENTO == idTrasferimento && a.DATAFINEVALIDITA >= dtIni &&
                        a.DATAINZIOVALIDITA <= dtFin).OrderBy(a => a.DATAINZIOVALIDITA);

            if (lrd?.Any() ?? false)
            {
                foreach (var rd in lrd)
                {
                    rd.ANNULLATO = true;
                    rd.DATAAGGIORNAMENTO = DateTime.Now;

                    db.SaveChanges();
                }

                this.SetRuoloDipendente(ref rdm, db);
            }
        }

        public void SetRuoloDipendente(ref RuoloDipendenteModel rdm, ModelDBISE db)
        {
            RUOLODIPENDENTE rd;

            rd = new RUOLODIPENDENTE()
            {
                IDRUOLO = rdm.idRuolo,
                IDTRASFERIMENTO = rdm.idTrasferimento,
                DATAINZIOVALIDITA = rdm.dataInizioValidita,
                DATAFINEVALIDITA = rdm.dataFineValidita.HasValue == true ? rdm.dataFineValidita.Value : Utility.DataFineStop(),
                DATAAGGIORNAMENTO = rdm.dataAggiornamento,
                ANNULLATO = rdm.annullato
            };

            db.RUOLODIPENDENTE.Add(rd);

            if (db.SaveChanges() > 0)
            {
                rdm.idRuoloDipendente = rd.IDRUOLODIPENDENTE;
            }


            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo ruolo dipendete.", "RuoloDipendente", db, rdm.idTrasferimento, rdm.idRuoloDipendente);

        }

        public void EditRuoloDipendente(RuoloDipendenteModel rdm, ModelDBISE db)
        {
            RUOLODIPENDENTE rd = db.RUOLODIPENDENTE.Find(rdm.idRuoloDipendente);

            if (rd != null && rd.IDRUOLODIPENDENTE > 0)
            {
                rd.IDRUOLO = rdm.idRuolo;
                rd.IDTRASFERIMENTO = rdm.idTrasferimento;
                rd.DATAINZIOVALIDITA = rdm.dataInizioValidita;
                rd.DATAFINEVALIDITA = rdm.dataFineValidita.HasValue == true ? rdm.dataFineValidita.Value : Utility.DataFineStop();
                rd.DATAAGGIORNAMENTO = rdm.dataAggiornamento;
                rd.ANNULLATO = rdm.annullato;

                db.SaveChanges();
            }



        }

        public void AggiornaRuoloDipendentePartenza(ref RuoloDipendenteModel rdm, TrasferimentoModel trm, ModelDBISE db)
        {
            decimal idRuoloDip = rdm.idRuoloDipendente;

            var rd = db.RUOLODIPENDENTE.Find(idRuoloDip);

            if (rd != null && rd.IDRUOLODIPENDENTE > 0)
            {
                //rd.IDRUOLO = trm.idRuoloUfficio;
                rd.IDTRASFERIMENTO = trm.idTrasferimento;
                rd.DATAINZIOVALIDITA = trm.dataPartenza;
                rd.DATAFINEVALIDITA = trm.dataRientro.HasValue == true ? trm.dataRientro.Value : Utility.DataFineStop();
                rd.DATAAGGIORNAMENTO = DateTime.Now;

                db.SaveChanges();

                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica Ruolo Dipendente.", "RuoloDipendente", db, rdm.idTrasferimento, rdm.idRuoloDipendente);
            }
        }

        public RuoloDipendenteModel InserisciRuoloDipendentePartenza(TrasferimentoModel trm, ModelDBISE db)
        {
            decimal idTrasferimento = trm.idTrasferimento;
            DateTime dtIni = trm.dataPartenza;
            DateTime dtFin = trm.dataRientro.HasValue == true ? trm.dataRientro.Value : Utility.DataFineStop();

            RUOLODIPENDENTE rd;

            rd = new RUOLODIPENDENTE()
            {
                IDRUOLO = trm.idRuoloUfficio,
                IDTRASFERIMENTO = trm.idTrasferimento,
                DATAINZIOVALIDITA = dtIni,
                DATAFINEVALIDITA = dtFin,
                DATAAGGIORNAMENTO = DateTime.Now,
                ANNULLATO = false
            };

            db.RUOLODIPENDENTE.Add(rd);

            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            if (db.SaveChanges() > 0)
            {
                rdm.idRuoloDipendente = rd.IDRUOLODIPENDENTE;
                rdm.idRuolo = rd.IDRUOLO;
                rdm.annullato = rd.ANNULLATO;
                rdm.dataAggiornamento = rd.DATAAGGIORNAMENTO;
                rdm.dataFineValidita = rd.DATAFINEVALIDITA;
                rdm.dataInizioValidita = rd.DATAINZIOVALIDITA;
                rdm.idTrasferimento = rd.IDTRASFERIMENTO;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo ruolo dipendente.", "RuoloDipendente", db, rdm.idTrasferimento, rdm.idRuoloDipendente);

            }


            return rdm;
        }

        public List<VariazioneRuoloDipendenteModel> GetListaRuoliDipendente(decimal idTrasferimento)
        {

            List<VariazioneRuoloDipendenteModel> lvrdm = new List<VariazioneRuoloDipendenteModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                var lrd = t.RUOLODIPENDENTE 
                                .Where(a => a.ANNULLATO == false && 
                                            a.DATAINZIOVALIDITA<=t.DATARIENTRO &&
                                            a.DATAFINEVALIDITA>=t.DATAPARTENZA)
                                            .OrderBy(a=>a.DATAINZIOVALIDITA)
                                            .ToList();

                bool trasfValido = (t.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo || t.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) ? true : false;
                var id_row = 0;
                foreach(var rd in lrd)
                {
                    id_row++;
                    var rd_new = new VariazioneRuoloDipendenteModel()
                    {
                        idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                        idTrasferimento = rd.IDTRASFERIMENTO,
                        idRuolo = rd.IDRUOLO,
                        dataInizioValidita = rd.DATAINZIOVALIDITA,
                        dataFineValidita = (t.DATARIENTRO < rd.DATAFINEVALIDITA) ? t.DATARIENTRO : rd.DATAFINEVALIDITA,
                        dataAggiornamento = rd.DATAAGGIORNAMENTO,
                        annullato = rd.ANNULLATO,
                        RuoloUfficio = new RuoloUfficioModel()
                        {
                            idRuoloUfficio = rd.RUOLOUFFICIO.IDRUOLO,
                            DescrizioneRuolo = rd.RUOLOUFFICIO.DESCRUOLO
                        },
                        eliminabile = (id_row == lrd.Count() && lrd.Count() > 1 && trasfValido) ? true : false,
                        ordinamento=id_row
                    };
                    lvrdm.Add(rd_new);
                }
            }
            return lvrdm;
        }

        public void VerificaDataInizioValiditaRuoloDipendente(decimal idTrasferimento, VariazioneRuoloDipendenteModel vrdm, ModelDBISE db)
        {
            var data = vrdm.ut_dataInizioValidita;

            if (data == null)
            {
                throw new Exception("La Data Inizio Validità è obbligatoria.");
            }

            var t = db.TRASFERIMENTO.Find(idTrasferimento);

            if (data < t.DATAPARTENZA)
            {
                throw new Exception(string.Format("Impossibile inserire la Data Inizio Validità minore della data partenza del trasferimento ({0}).", t.DATAPARTENZA.ToShortDateString()));
            }

            if (data > t.DATARIENTRO)
            {
                throw new Exception(string.Format("Impossibile inserire la Data Inizio Validità maggiore della data di rientro ({0}).", t.DATARIENTRO.ToShortDateString()));
            }

            using (dtLivelliDipendente dtld = new dtLivelliDipendente())
            {
                var livelloDipendenteValido = dtld.GetLivelloDipendente(t.IDDIPENDENTE, data.Value);
                if(data<livelloDipendenteValido.dataInizioValdita) 
                {
                    throw new Exception(string.Format("Impossibile inserire la Data Inizio Validità minore della data inizio validita del livello dipendente ({0}).", livelloDipendenteValido.dataInizioValdita.ToShortDateString()));
                }
                if (data >= livelloDipendenteValido.dataFineValidita)
                {
                    throw new Exception(string.Format("Impossibile inserire la Data Inizio Validità uguale o superiore della data fine validita del livello dipendente ({0}).", livelloDipendenteValido.dataFineValidita.Value.ToShortDateString()));
                }
            }

            using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
            {

                var rd = dtrd.GetRuoloDipendenteByIdTrasferimento(idTrasferimento, vrdm.ut_dataInizioValidita.Value);
                var idRuoloAttuale = rd.RuoloUfficio.idRuoloUfficio;
                if(idRuoloAttuale==vrdm.idRuolo && vrdm.chkAggiornaTutti==false)
                {
                    throw new Exception(string.Format("Alla data di riferimento inserita l'utente ha già il ruolo prescelto ({0}).", rd.RuoloUfficio.DescrizioneRuolo.ToString()));
                }
            }

        }

        private IList<RuoloDipendenteModel> PrelevaMovimentiRuoliPrecedenti(decimal idTrasferimento, DateTime dtIni, ModelDBISE db)
        {
            List<RuoloDipendenteModel> lrdm = new List<RuoloDipendenteModel>();

            var lrd =
                db.TRASFERIMENTO.Find(idTrasferimento).RUOLODIPENDENTE.Where(
                        a =>
                            a.ANNULLATO==false &&
                            a.DATAINZIOVALIDITA <= dtIni)
                    .OrderByDescending(a => a.DATAINZIOVALIDITA)
                    .ToList();


            if (lrd?.Any() ?? false)
            {
                lrdm = (from e in lrd
                        select new RuoloDipendenteModel()
                        {
                            idRuoloDipendente=e.IDRUOLODIPENDENTE,
                            idTrasferimento=e.IDTRASFERIMENTO,
                            idRuolo=e.IDRUOLO,
                            dataInizioValidita = e.DATAINZIOVALIDITA,
                            dataFineValidita = e.DATAFINEVALIDITA,
                            dataAggiornamento = e.DATAAGGIORNAMENTO,
                            annullato = e.ANNULLATO
                        }).ToList();
            }

            return lrdm;
        }

        private IList<RuoloDipendenteModel> PrelevaMovimentiRuoliSuccessivi(decimal idTrasferimento, DateTime dtIni, ModelDBISE db)
        {
            List<RuoloDipendenteModel> lrdm = new List<RuoloDipendenteModel>();

            var lrd =
                db.TRASFERIMENTO.Find(idTrasferimento).RUOLODIPENDENTE.Where(
                       a =>
                            a.ANNULLATO == false &&
                            a.DATAINZIOVALIDITA > dtIni)
                    .OrderBy(a => a.DATAINZIOVALIDITA)
                    .ToList();


            if (lrd?.Any() ?? false)
            {
                lrdm = (from e in lrd
                        select new RuoloDipendenteModel()
                        {
                            idRuoloDipendente = e.IDRUOLODIPENDENTE,
                            idTrasferimento = e.IDTRASFERIMENTO,
                            idRuolo = e.IDRUOLO,
                            dataInizioValidita = e.DATAINZIOVALIDITA,
                            dataFineValidita = e.DATAFINEVALIDITA,
                            dataAggiornamento = e.DATAAGGIORNAMENTO,
                            annullato = e.ANNULLATO
                        }).ToList();
            }

            return lrdm;
        }



        public void SetVariazioneRuoloDipendente(RuoloDipendenteModel rdm, decimal idTrasferimento, ModelDBISE db)
        {

            RuoloDipendenteModel ruoloPrecedente = new RuoloDipendenteModel();
            RuoloDipendenteModel ruoloSuccessivo = new RuoloDipendenteModel();
            RuoloDipendenteModel ruoloLav = new RuoloDipendenteModel();
            List<RuoloDipendenteModel> lRuoliPrecedenti = new List<RuoloDipendenteModel>();
            List<RuoloDipendenteModel> lRuoliSuccessivi = new List<RuoloDipendenteModel>();

            try
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                lRuoliPrecedenti =
                    PrelevaMovimentiRuoliPrecedenti(idTrasferimento, rdm.dataInizioValidita, db).ToList();

                lRuoliSuccessivi =
                    PrelevaMovimentiRuoliSuccessivi(idTrasferimento, rdm.dataInizioValidita, db).ToList();

                if (lRuoliPrecedenti.Count == 0)
                {
                    if (lRuoliSuccessivi.Count == 0)
                    {
                        #region creo record
                        ruoloLav = new RuoloDipendenteModel()
                        {
                            idTrasferimento = rdm.idTrasferimento,
                            idRuolo = rdm.idRuolo,
                            dataInizioValidita = rdm.dataInizioValidita,
                            dataFineValidita = t.DATARIENTRO,
                            dataAggiornamento = DateTime.Now,
                            annullato = false
                        };
                        SetRuoloDipendente(ref rdm, db);
                        #endregion
                    }
                    else
                    {
                        ruoloSuccessivo = lRuoliSuccessivi.First();

                        #region creo record
                        ruoloLav = new RuoloDipendenteModel()
                        {
                            idTrasferimento = rdm.idTrasferimento,
                            idRuolo = rdm.idRuolo,
                            dataInizioValidita = rdm.dataInizioValidita,
                            dataFineValidita = ruoloSuccessivo.dataInizioValidita.AddDays(-1),
                            dataAggiornamento = DateTime.Now,
                            annullato = false
                        };
                        SetRuoloDipendente(ref ruoloLav, db);
                        #endregion
                    }
                }
                else
                {
                    ruoloPrecedente = lRuoliPrecedenti.First();

                    if (lRuoliSuccessivi.Count == 0)
                    {
                        if (ruoloPrecedente.dataInizioValidita == rdm.dataInizioValidita)
                        {
                            #region replico record e annullo il dato
                            ruoloPrecedente.AnnullaRecord(db);
                            #endregion

                            #region creo record
                            ruoloLav = new RuoloDipendenteModel()
                            {
                                idTrasferimento = rdm.idTrasferimento,
                                idRuolo = rdm.idRuolo,
                                dataInizioValidita = rdm.dataInizioValidita,
                                dataFineValidita = ruoloPrecedente.dataFineValidita,
                                dataAggiornamento = DateTime.Now,
                                annullato = false
                            };
                            SetRuoloDipendente(ref ruoloLav, db);
                            #endregion

                        }
                        else
                        {
                            if (ruoloPrecedente.dataFineValidita == t.DATARIENTRO)
                            {
                                #region replico record e lo nascondo
                                ruoloPrecedente.AnnullaRecord(db);

                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = ruoloPrecedente.idRuolo,
                                    dataInizioValidita = ruoloPrecedente.dataInizioValidita,
                                    dataFineValidita = rdm.dataInizioValidita.AddDays(-1),
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };
                                SetRuoloDipendente(ref ruoloLav, db);
                                #endregion

                                #region creo record
                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = rdm.idRuolo,
                                    dataInizioValidita = rdm.dataInizioValidita,
                                    dataFineValidita = t.DATARIENTRO,
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };
                                SetRuoloDipendente(ref ruoloLav, db);
                                #endregion
                            }
                            else
                            {
                                ruoloPrecedente.AnnullaRecord(db);
                                
                                #region creo record
                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = ruoloPrecedente.idRuolo,
                                    dataInizioValidita = ruoloPrecedente.dataInizioValidita,
                                    dataFineValidita = rdm.dataInizioValidita.AddDays(-1),
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };
                                SetRuoloDipendente(ref ruoloLav, db);
                                #endregion

                                #region creo record
                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = rdm.idRuolo,
                                    dataInizioValidita = rdm.dataInizioValidita,
                                    dataFineValidita = t.DATARIENTRO,
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };
                                SetRuoloDipendente(ref ruoloLav, db);
                                #endregion
                            }
                        }
                    }

                    else
                    {
                        ruoloSuccessivo = lRuoliSuccessivi.First();

                        if (ruoloPrecedente.dataInizioValidita == rdm.dataInizioValidita)                                                
                        {                           
                            #region replico record e lo nascondo

                            ruoloPrecedente.AnnullaRecord(db);
                            ruoloSuccessivo.AnnullaRecord(db);

                            if (lRuoliPrecedenti.Count() > 1)
                            {
                                //legge periodo ancora precedente
                                var lrprec = lRuoliPrecedenti.Where(a => a.annullato==false && a.idRuoloDipendente < ruoloPrecedente.idRuoloDipendente).OrderByDescending(a => a.idRuoloDipendente).ToList();
                                var rprec = lrprec.First();
                                rprec.AnnullaRecord(db);

                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = rdm.idRuolo,
                                    dataInizioValidita = rprec.dataInizioValidita,
                                    dataFineValidita = ruoloSuccessivo.dataFineValidita,
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };
                                SetRuoloDipendente(ref ruoloLav, db);
                                #endregion
                            }else
                            {
                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = ruoloPrecedente.idRuolo,
                                    dataInizioValidita = rdm.dataInizioValidita,
                                    dataFineValidita = ruoloSuccessivo.dataFineValidita,
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };
                                SetRuoloDipendente(ref ruoloLav, db);
                            }
                        }else
                        {
                            //annullo periodo successivo
                            ruoloSuccessivo.AnnullaRecord(db);

                            //annullo precedente
                            ruoloPrecedente.AnnullaRecord(db);

                            //creo record da ini prec a dtrif-1
                            #region creo record
                            ruoloLav = new RuoloDipendenteModel()
                            {
                                idTrasferimento = rdm.idTrasferimento,
                                idRuolo = ruoloPrecedente.idRuolo,
                                dataInizioValidita = ruoloPrecedente.dataInizioValidita,
                                dataFineValidita = rdm.dataInizioValidita.AddDays(-1),
                                dataAggiornamento = DateTime.Now,
                                annullato = false

                            };
                            SetRuoloDipendente(ref ruoloLav, db);
                            #endregion

                            //creo record da dtrif a dtfinsucc
                            #region creo record
                            ruoloLav = new RuoloDipendenteModel()
                            {
                                idTrasferimento = rdm.idTrasferimento,
                                idRuolo = rdm.idRuolo,
                                dataInizioValidita = rdm.dataInizioValidita,
                                dataFineValidita = ruoloSuccessivo.dataFineValidita,
                                dataAggiornamento = DateTime.Now,
                                annullato = false

                            };
                            SetRuoloDipendente(ref ruoloLav, db);
                            #endregion

                        }
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public void SetVariazioneRuoloDipendente_AggiornaTutti(RuoloDipendenteModel rdm, decimal idTrasferimento, ModelDBISE db)
        {

            RuoloDipendenteModel ruoloPrecedente = new RuoloDipendenteModel();
            RuoloDipendenteModel ruoloSuccessivo = new RuoloDipendenteModel();
            RuoloDipendenteModel ruoloLav = new RuoloDipendenteModel();
            List<RuoloDipendenteModel> lRuoliPrecedenti = new List<RuoloDipendenteModel>();
            List<RuoloDipendenteModel> lRuoliSuccessivi = new List<RuoloDipendenteModel>();

            try
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);


                lRuoliPrecedenti = 
                            PrelevaMovimentiRuoliPrecedenti(idTrasferimento, rdm.dataInizioValidita, db).ToList();

                lRuoliSuccessivi =
                            PrelevaMovimentiRuoliSuccessivi(idTrasferimento, rdm.dataInizioValidita, db).ToList();

                if (lRuoliPrecedenti.Count == 0)
                {
                    if (lRuoliSuccessivi.Count == 0)
                    {
                        #region creo record (periodo dataIniInput - dataRientro)
                        ruoloLav = new RuoloDipendenteModel()
                        {
                            idTrasferimento = rdm.idTrasferimento,
                            idRuolo = rdm.idRuolo,
                            dataInizioValidita = rdm.dataInizioValidita,
                            dataFineValidita = t.DATARIENTRO,
                            dataAggiornamento = DateTime.Now,
                            annullato = false                            
                        };
                        SetRuoloDipendente(ref ruoloLav, db);
                        #endregion
                    }
                    else
                    {
                        ruoloSuccessivo = lRuoliSuccessivi.First();

                        #region annullo tutti record fino al primo buco temporale o dataRientro
                        var cont = 1;
                        //nascondo in ogni caso il primo successivo
                        ruoloSuccessivo.AnnullaRecord(db);

                        var dataFineCorrente = ruoloSuccessivo.dataFineValidita;
                        //annullo solo i successivi record attigui e leggo l'ultima datafine del periodo
                        foreach (var ruoloSucc in lRuoliSuccessivi)
                        {
                            if (cont > 1 && ruoloSucc.dataInizioValidita == dataFineCorrente.Value.AddDays(1))
                            {
                                dataFineCorrente = ruoloSucc.dataFineValidita;
                                ruoloSucc.AnnullaRecord(db);
                            }
                            cont++;
                        }
                        #endregion

                        #region creo record
                        ruoloLav = new RuoloDipendenteModel()
                        {
                            idTrasferimento = rdm.idTrasferimento,
                            idRuolo = rdm.idRuolo,
                            dataInizioValidita = rdm.dataInizioValidita,
                            dataFineValidita = dataFineCorrente,
                            dataAggiornamento = DateTime.Now,
                            annullato = false                        
                        };
                        SetRuoloDipendente(ref ruoloLav, db);
                        #endregion
                    }
                }
                else
                {
                    ruoloPrecedente = lRuoliPrecedenti.First();

                    if (lRuoliSuccessivi.Count == 0)
                    { 
                        if (ruoloPrecedente.dataInizioValidita == rdm.dataInizioValidita)
                        {
                            {
                                ruoloPrecedente.AnnullaRecord(db);

                                #region replico creo record con periodo dataini - dataIniInput-1
                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = ruoloPrecedente.idRuolo,
                                    dataInizioValidita = ruoloPrecedente.dataInizioValidita,
                                    dataFineValidita = t.DATARIENTRO,
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };
                                SetRuoloDipendente(ref ruoloLav, db);
                                #endregion
                            }
                        }
                        else
                        {
                            if (ruoloPrecedente.idRuolo != rdm.idRuolo)
                            {
                                #region replico creo record con periodo dataini - dataIniInput-1
                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = ruoloPrecedente.idRuolo,
                                    dataInizioValidita = ruoloPrecedente.dataInizioValidita,
                                    dataFineValidita = rdm.dataInizioValidita.AddDays(-1),
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };
                                SetRuoloDipendente(ref ruoloLav, db);
                                #endregion

                                #region creo record 
                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = rdm.idRuolo,
                                    dataInizioValidita = rdm.dataInizioValidita,
                                    dataFineValidita = t.DATARIENTRO,
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };
                                SetRuoloDipendente(ref ruoloLav, db);
                                #endregion
                            }else
                            {
                                ruoloPrecedente.AnnullaRecord(db);

                                #region creo record 
                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = rdm.idRuolo,
                                    dataInizioValidita = ruoloPrecedente.dataInizioValidita,
                                    dataFineValidita = t.DATARIENTRO,
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };
                                SetRuoloDipendente(ref ruoloLav, db);
                                #endregion

                            }
                        }
                    }
                    else
                    {
                        ruoloSuccessivo = lRuoliSuccessivi.First();

                        if (ruoloPrecedente.dataInizioValidita == rdm.dataInizioValidita)
                        {
                            #region annullo i record successivi fino al primo buco temporale o dataRientro
                            var cont = 1;
                            //nascondo in ogni caso il primo successivo
                            ruoloSuccessivo.AnnullaRecord(db);

                            var dataFineCorrente = ruoloSuccessivo.dataFineValidita;
                            //annullo solo i successivi record attigui e leggo l'ultima datafine del periodo
                            foreach (var ruoloSucc in lRuoliSuccessivi)
                            {
                                if (cont > 1 && ruoloSucc.dataInizioValidita == dataFineCorrente.Value.AddDays(1))
                                {
                                    dataFineCorrente = ruoloSucc.dataFineValidita;
                                    ruoloSucc.AnnullaRecord(db);
                                }
                                cont++;
                            }
                            #endregion
                            ruoloPrecedente.AnnullaRecord(db);

                            #region replico creo record con periodo dataini - dataFineCorrente
                            ruoloLav = new RuoloDipendenteModel()
                            {
                                idTrasferimento = rdm.idTrasferimento,
                                idRuolo = ruoloPrecedente.idRuolo,
                                dataInizioValidita = ruoloPrecedente.dataInizioValidita,
                                dataFineValidita = dataFineCorrente,
                                dataAggiornamento = DateTime.Now,
                                annullato = false
                            };
                            SetRuoloDipendente(ref ruoloLav, db);
                            #endregion
                           
                        }
                        else
                        {

                            ruoloPrecedente.AnnullaRecord(db);

                            if (ruoloPrecedente.idRuolo != rdm.idRuolo)
                            {
                                #region replico creo record con periodo dataini - dataIniInput-1
                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = ruoloPrecedente.idRuolo,
                                    dataInizioValidita = ruoloPrecedente.dataInizioValidita,
                                    dataFineValidita = rdm.dataInizioValidita.AddDays(-1),
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };
                                SetRuoloDipendente(ref ruoloLav, db);
                                #endregion
                            }

                            #region annullo i record successivi fino al primo buco temporale o dataRientro
                            var cont = 1;
                            //nascondo in ogni caso il primo successivo
                            ruoloSuccessivo.AnnullaRecord(db);

                            var dataFineCorrente = ruoloSuccessivo.dataFineValidita;
                            //annullo solo i successivi record attigui e leggo l'ultima datafine del periodo
                            foreach (var ruoloSucc in lRuoliSuccessivi)
                            {
                                if (cont > 1 && ruoloSucc.dataInizioValidita == dataFineCorrente.Value.AddDays(1))
                                {
                                    dataFineCorrente = ruoloSucc.dataFineValidita;
                                    ruoloSucc.AnnullaRecord(db);
                                }
                                cont++;
                            }
                            #endregion

                            if (ruoloPrecedente.idRuolo != rdm.idRuolo)
                            {
                                #region creo record
                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = rdm.idRuolo,
                                    dataInizioValidita = rdm.dataInizioValidita,
                                    dataFineValidita = dataFineCorrente,
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };

                                SetRuoloDipendente(ref ruoloLav, db);
                                #endregion
                            }
                            else
                            {
                                #region replico creo record con periodo datainiprec - dataFineCorrente
                                ruoloLav = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = rdm.idTrasferimento,
                                    idRuolo = ruoloPrecedente.idRuolo,
                                    dataInizioValidita = ruoloPrecedente.dataInizioValidita,
                                    dataFineValidita = dataFineCorrente,
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false
                                };
                                SetRuoloDipendente(ref ruoloLav, db);
                                #endregion
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



    }
}