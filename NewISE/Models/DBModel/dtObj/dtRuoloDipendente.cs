using NewISE.EF;
using NewISE.Models.dtObj;
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
                rd.IDRUOLO = trm.idRuoloUfficio;
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


    }
}