using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using NewISE.Models.ViewModel;
using System.Data.Entity;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtSospensione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Delete_Sospensione( decimal idSospensione,bool permesso=true)
        {
            bool esito = false;
            if (permesso)
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ca = db.SOSPENSIONE.Find(idSospensione);
                    db.SOSPENSIONE.Remove(ca);
                    int i = db.SaveChanges();
                    if (i <= 0)
                    {
                        esito = false;
                        throw new Exception("Errore nella fase della cancellazione della sospensione");
                    }
                    else
                    {
                        esito = true;
                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Cancellazione Sospensione",
                            "SOSPENSIONE", db, ca.IDTRASFERIMENTO, ca.IDSOSPENSIONE);
                    }
                }
            }
            else
            {
                throw new Exception("Eliminazione Sospensione non autorizzata");
            }
            return esito;
        }

        public List<SospensioneModel> GetLista_Sospensioni(decimal idtrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                TRASFERIMENTO trasf = db.TRASFERIMENTO.Find(idtrasferimento);
                var  tmp = (from e in trasf.SOSPENSIONE  where e.ANNULLATO == false
                            orderby e.DATAINIZIO ascending
                            select new SospensioneModel()
                        {
                            idSospensione=e.IDSOSPENSIONE,
                            DataInizioSospensione = e.DATAINIZIO,
                            DataFineSospensione = e.DATAFINE,
                            TipoSospensione = e.TIPOSOSPENSIONE.DESCRIZIONE,
                            DataAggiornamento = e.DATAAGGIORNAMENTO,
                            // NumeroGiorni =DbFunctions.DiffDays(e.DATAINIZIO, e.DATAFINE).Value
                            NumeroGiorni = Convert.ToInt32((e.DATAFINE.AddDays(1) - e.DATAINIZIO).TotalDays)
                        }).ToList();
                    return tmp;
            }       
        }
        public SospensioneModel GetSospensionePerEliminazione(decimal idSospensione)
        {
            SospensioneModel tmp = new SospensioneModel();
            using (ModelDBISE db = new ModelDBISE())
            {
               var  x = db.SOSPENSIONE.Find(idSospensione);
                tmp.DataInizioSospensione = x.DATAINIZIO;
                tmp.DataFineSospensione = x.DATAFINE;
                tmp.TipoSospensione = x.TIPOSOSPENSIONE.DESCRIZIONE;
                tmp.DataAggiornamento = x.DATAAGGIORNAMENTO;                
            }
            return tmp;
        }

        public static bool CompresaInUnIntervallo(DateTime inizio,DateTime fine,decimal id_Trasferimento,decimal idSospensione=0)
        {
            bool result = false;
            using (ModelDBISE db = new ModelDBISE())
            {
                var nsp = 0;
                var t = db.TRASFERIMENTO.Find(id_Trasferimento);

                if (idSospensione == 0)
                    nsp = t.SOSPENSIONE.Where(a => a.ANNULLATO == false && (inizio > a.DATAINIZIO && inizio < a.DATAFINE || fine > a.DATAINIZIO && fine < a.DATAFINE)).Count();
                else
                    nsp = t.SOSPENSIONE.Where(a => a.ANNULLATO == false && (inizio > a.DATAINIZIO && inizio < a.DATAFINE || fine > a.DATAINIZIO 
                    && fine < a.DATAFINE && a.IDSOSPENSIONE!=idSospensione)).Count();

                if (nsp != 0) result = true;
            }
            return result;
        }
        #region Funzioni di validazione custom
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as SospensioneModel;
            if (fm != null)
            {
                if (fm.DataInizioSospensione > fm.DataFineSospensione)
                { 
                        vr = new ValidationResult("La data di inizio sospensione deve essere minore della data fine sospensione.");
                }
                else
                {
                    using (ModelDBISE db = new ModelDBISE())
                    {
                        DateTime t = db.TRASFERIMENTO.Find(fm.idTrasferimento).DATAPARTENZA;
                        if (fm.DataInizioSospensione < t)
                        {
                            vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio sospensione minore della data di partenza del trasferimento ({0}).", t.ToShortDateString()));
                        }
                        else
                        {
                            var nsp = 0;
                            var tr = db.TRASFERIMENTO.Find(fm.idTrasferimento);
                            if (fm.idSospensione == 0)
                                nsp  = tr.SOSPENSIONE.Where(a => a.ANNULLATO == false && a.IDTIPOSOSPENSIONE == fm.idTipoSospensione && fm.DataInizioSospensione >= a.DATAINIZIO && fm.DataInizioSospensione <= a.DATAFINE).Count();
                            else                           
                                nsp = tr.SOSPENSIONE.Where(a => a.ANNULLATO == false && a.IDTIPOSOSPENSIONE == fm.idTipoSospensione && fm.DataInizioSospensione >= a.DATAINIZIO && fm.DataInizioSospensione <= a.DATAFINE 
                                && a.IDSOSPENSIONE!=fm.idSospensione).Count();

                            if (nsp > 0)
                            {
                                vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio sospensione in quanto compresa in un intervallo ({0}).", fm.DataInizioSospensione.Value.ToShortDateString()));
                            }
                            else
                            {
                                if (fm.idSospensione == 0)
                                    nsp = tr.SOSPENSIONE.Where(a => a.ANNULLATO == false && a.IDTIPOSOSPENSIONE == fm.idTipoSospensione && fm.DataFineSospensione >= a.DATAINIZIO && fm.DataFineSospensione <= a.DATAFINE).Count();
                                else
                                    nsp = tr.SOSPENSIONE.Where(a => a.ANNULLATO == false && a.IDTIPOSOSPENSIONE == fm.idTipoSospensione && fm.DataFineSospensione >= a.DATAINIZIO && fm.DataFineSospensione <= a.DATAFINE
                                    && a.IDSOSPENSIONE != fm.idSospensione).Count();
                                if (nsp > 0)
                                {
                                    vr = new ValidationResult(string.Format("Impossibile inserire la data di fine sospensione in quanto compresa in un intervallo ({0}).", fm.DataFineSospensione.Value.ToShortDateString()));
                                }
                                else
                                {
                                    vr = ValidationResult.Success;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                vr = new ValidationResult("La data di inizio sospensione è richiesta.");
            }
            return vr;
        }

        //public static ValidationResult VerificaDataFine(string v, ValidationContext context)
        //{
        //    ValidationResult vr = ValidationResult.Success;
        //    var fm = context.ObjectInstance as SospensioneModel;
        //    if (fm != null)
        //    {
        //        if (fm.DataFineSospensione<fm.DataInizioSospensione)
        //        {
        //            vr = new ValidationResult("La data di fine sospensione deve essere maggiore della data inizio sospensione.");
        //        }
        //        else
        //        {
        //            using (ModelDBISE db = new ModelDBISE())
        //            {
        //                var tr = db.TRASFERIMENTO.Find(fm.idTrasferimento);
        //                DateTime t = tr.DATARIENTRO.HasValue == true ? tr.DATARIENTRO.Value : Utility.DataFineStop();
        //                if (fm.DataFineSospensione > t)
        //                {
        //                    vr = new ValidationResult(string.Format("Impossibile inserire la data fine sospensione maggiore della data di rientro del trasferimento ({0}).", t.ToShortDateString()));
        //                }
        //                else
        //                {
        //                    var t_r = db.TRASFERIMENTO.Find(fm.idTrasferimento);
        //                    var nsp = t_r.SOSPENSIONE.Where(a => a.ANNULLATO == false && fm.DataFineSospensione > a.DATAINIZIO && fm.DataFineSospensione <= a.DATAFINE).Count();
        //                    if (nsp != 0)
        //                    {
        //                        vr = new ValidationResult(string.Format("Impossibile inserire la data di fine sospensione in quanto compresa in un intervallo ({0}).", fm.DataFineSospensione.Value.ToShortDateString()));
        //                    }
        //                    vr = ValidationResult.Success;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        vr = new ValidationResult("La data fine sospensione è richiesta.");
        //    }
        //    return vr;
        //}
        #endregion
        public bool dateValide(DateTime inizio,DateTime fine)
        {
            if (inizio > fine) return false;
            return true;
        }
        public string[] InserisciSospensione(SospensioneModel sosp,decimal idTipoSospensione)
        {
            string[] my_array = new string[] { "0", "0" };
            if (dateValide(sosp.DataInizioSospensione.Value, sosp.DataFineSospensione.Value))
            { 
                if (!CompresaInUnIntervallo(sosp.DataInizioSospensione.Value, sosp.DataFineSospensione.Value, sosp.idTrasferimento,sosp.idSospensione))
                {
                    using (var db = new ModelDBISE())
                    {
                        try
                        {
                            db.Database.BeginTransaction();
                            var sospnew = new SOSPENSIONE
                            {
                                IDTRASFERIMENTO = sosp.idTrasferimento,//tmp.FirstOrDefault().idTrasferimento,
                                DATAINIZIO = sosp.DataInizioSospensione.Value,
                                DATAFINE = sosp.DataFineSospensione.Value,
                                IDTIPOSOSPENSIONE = idTipoSospensione,// sosp.idTipoSospensione,
                                DATAAGGIORNAMENTO = DateTime.Now
                            };
                            db.SOSPENSIONE.Add(sospnew);

                            if (db.SaveChanges() > 0)
                            {
                                sosp.idSospensione = sospnew.IDSOSPENSIONE;
                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento sospensione avvenuta con successo", "SOSPENSIONE", db, sospnew.IDTRASFERIMENTO, sospnew.IDSOSPENSIONE);
                                db.Database.CurrentTransaction.Commit();
                            }
                            else
                            {
                                throw new Exception("L'inserimento della sospensione non è avvenuto.");
                            }
                        }
                        catch (Exception ex)
                        {
                            db.Database.CurrentTransaction.Rollback();
                            throw ex;
                        }
                    }
                }
                else
                {
                    my_array[0] = "2"; my_array[1] = "Impossibile inserire una sopsensione con il periodo incluso in una sopsensione esistente.";                   
                    return my_array;
                    //throw new Exception("Le date non sono valide");
                }
            }
            else
            {
                my_array[0] = "1"; my_array[1] = "Data Inizio non deve essere superiore alla data fine";                
                return my_array;
               // throw new Exception("Controllare i dati inseriti");
            }
            return my_array;
        }

        public IList<TipologiaSospensioneModel> GetListTipologiaSospensione()
        {
            List<TipologiaSospensioneModel> ltcm = new List<TipologiaSospensioneModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                var ltc = db.TIPOSOSPENSIONE.OrderBy(a => a.IDTIPOSOSPENSIONE).ToList();

                if (ltc != null && ltc.Count > 0)
                {
                    ltcm = (from e in ltc
                            select new TipologiaSospensioneModel()
                            {
                                idTipologiaSospensione = e.IDTIPOSOSPENSIONE,
                                Descrizione = e.DESCRIZIONE
                            }).ToList();
                }
            }
            return ltcm;
        }

        public string[] Modifica_Sospensione(SospensioneModel sospmod)
        {
            string[] my_array = new string[] { "0", "0" };
            try
            {
                if (dateValide(sospmod.DataInizioSospensione.Value, sospmod.DataFineSospensione.Value))
                {
                    if (!CompresaInUnIntervallo(sospmod.DataInizioSospensione.Value, sospmod.DataFineSospensione.Value, sospmod.idTrasferimento,sospmod.idSospensione))
                    {
                        using (ModelDBISE db = new ModelDBISE())
                        {
                            SOSPENSIONE sosp = db.SOSPENSIONE.Find(sospmod.idSospensione);
                            sosp.IDTIPOSOSPENSIONE = sospmod.idTipoSospensione;
                            sosp.DATAINIZIO = sospmod.DataInizioSospensione.Value;
                            sosp.DATAFINE = sospmod.DataFineSospensione.Value;
                            sosp.ANNULLATO = sospmod.ANNULLATO;

                            int i = db.SaveChanges();

                            if (i < 0)
                            {
                                throw new Exception(string.Format("Errore nella fase di modifica della Sospensione."));
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica Sospensione avvenuta con successo",
                                  "SOSPENSIONE", db, sospmod.idTrasferimento, sospmod.idSospensione);
                            }
                        }
                    }
                    else
                    {
                        my_array[0] = "2"; my_array[1] = "Impossibile inserire una sopsensione con il periodo incluso in una sopsensione esistente.";
                        return my_array;
                    }
                }
                else
                {
                    my_array[0] = "1"; my_array[1] = "Data Inizio non deve essere superiore alla data fine";
                    return my_array;
                }
            }
            catch (Exception ex)
            {
                 throw ex;
            }
            return my_array;
        }
        public SospensioneModel getSospensioneById(decimal idSospensione)
        {
            SospensioneModel tmp;
            using (ModelDBISE db = new ModelDBISE())
            {
                tmp = (from e in db.SOSPENSIONE
                   where e.IDSOSPENSIONE== idSospensione
                           select new SospensioneModel()
                           {
                               DataInizioSospensione=e.DATAINIZIO,
                               DataFineSospensione=e.DATAFINE,
                               DataAggiornamento=e.DATAAGGIORNAMENTO,
                               ANNULLATO=e.ANNULLATO,
                               idSospensione=e.IDSOSPENSIONE,
                               idTipoSospensione=e.IDTIPOSOSPENSIONE,
                               idTrasferimento=e.IDTRASFERIMENTO,
                               TipoSospensione=e.TIPOSOSPENSIONE.DESCRIZIONE
                           }).ToList().FirstOrDefault();
            }
            return tmp;
        }
    }
}