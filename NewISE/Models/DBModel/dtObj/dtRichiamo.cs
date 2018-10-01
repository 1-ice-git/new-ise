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
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRichiamo : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public decimal Restituisci_ID_CoeffIndRichiamo_Da_Data(RichiamoModel ri, ModelDBISE db)
        {
            decimal tmp = 0;
            var lCIR = db.COEFFICIENTEINDRICHIAMO.ToList().Where(a => a.ANNULLATO == false && ri.DataRichiamo >= a.DATAINIZIOVALIDITA && ri.DataRichiamo <= a.DATAFINEVALIDITA).ToList();
            if (lCIR?.Any() ?? false)
            {
                tmp = lCIR.First().IDCOEFINDRICHIAMO;
            }
            return tmp;
        }
        public decimal Restituisci_ID_PercentualeFKM_Da_Data(RichiamoModel ri, ModelDBISE db)
        {
            decimal tmp = 0;
            var lCIR = db.PERCENTUALEFKM.Where(x => x.IDFKM == ri.CoeffKm && x.ANNULLATO == false).ToList().Where(a => ri.DataRichiamo >= a.DATAINIZIOVALIDITA &&
                ri.DataRichiamo <= a.DATAFINEVALIDITA).ToList();
            if (lCIR?.Any() ?? false)
            {
                tmp = lCIR.First().IDPFKM;
            }
            return tmp;
        }
        public DateTime Restituisci_DataPartenza(decimal idTrasferimento, ModelDBISE db)
        {
            DateTime tmp = new DateTime();
            var CIR = db.TRASFERIMENTO.Find(idTrasferimento);
            if (CIR != null) //CIR.IDTRASFERIMENTO
            {
                tmp = CIR.DATAPARTENZA;
            }
            return tmp;
        }
        public DateTime Restituisci_DataRientro(decimal idTrasferimento, ModelDBISE db)
        {
            DateTime tmp = new DateTime();
            var CIR = db.TRASFERIMENTO.Find(idTrasferimento);
            if (CIR != null) //CIR.IDTRASFERIMENTO
            {
                tmp = CIR.DATARIENTRO;
            }
            return tmp;
        }
        public RichiamoModel Restituisci_Ultimo_Richiamo(decimal idTrasferimento)
        {
            RichiamoModel tmp = new RichiamoModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                var CIR = db.TRASFERIMENTO.Find(idTrasferimento);
                var rich = CIR.RICHIAMO.Where(a => a.ANNULLATO == false).OrderByDescending(x => x.IDRICHIAMO);
                if (rich?.Any() ?? false)
                {
                    var r = rich.First();
                    var p = r.PERCENTUALEFKM.ToList();
                    if (p?.Any() ?? false)
                    {
                        decimal idFKM = p.First().IDFKM;
                        decimal idRichiamo = r.IDRICHIAMO;
                        tmp.IdRichiamo = r.IDRICHIAMO;
                        tmp.idTrasferimento = r.IDTRASFERIMENTO;
                        tmp.DataRientro = r.TRASFERIMENTO.DATARIENTRO;
                        tmp.DataPartenza = r.TRASFERIMENTO.DATAPARTENZA;
                        tmp.DataRichiamo = r.DATARICHIAMO;
                        tmp.DataAggiornamento = r.DATAAGGIORNAMENTO;
                        tmp.CoeffKm = idFKM;
                        tmp.IDPFKM = p.First().IDPFKM;
                    }
                }
            }
            return tmp;
        }
        public DateTime Restituisci_Data_Rientro(decimal idTrasferimento)
        {
            DateTime tmp = new DateTime();
            using (ModelDBISE db = new ModelDBISE())
            {
                var CIR = db.TRASFERIMENTO.Find(idTrasferimento);
                if (CIR != null)
                    tmp = CIR.DATARIENTRO;
            }
            return tmp;
        }
        public RichiamoModel GetRichiamoByIdTrasf(decimal idTrasferimento)
        {
            RichiamoModel rm = new RichiamoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                TRASFERIMENTO t = db.TRASFERIMENTO.Find(idTrasferimento);
                var lr = t.RICHIAMO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDRICHIAMO);

                if (lr?.Any() ?? false)
                {
                    var r = lr.First();

                    rm = new RichiamoModel()
                    {
                        IdRichiamo = r.IDRICHIAMO,
                        idTrasferimento = r.IDTRASFERIMENTO,
                        DataRichiamo = r.DATARICHIAMO,
                        DataAggiornamento = r.DATAAGGIORNAMENTO,
                        annullato = r.ANNULLATO
                    };
                }
            }

            return rm;
        }

        public List<RichiamoModel> GetLista_Richiamo(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                TRASFERIMENTO trasf = db.TRASFERIMENTO.Find(idTrasferimento);
                var lr = trasf.RICHIAMO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDRICHIAMO);
                var tmp = (from e in lr

                           select new RichiamoModel()
                           {
                               IdRichiamo = e.IDRICHIAMO,
                               idTrasferimento = e.IDTRASFERIMENTO,
                               DataRichiamo = e.DATARICHIAMO,
                               DataAggiornamento = e.DATAAGGIORNAMENTO,
                               annullato = e.ANNULLATO,
                               DataPartenza = e.TRASFERIMENTO.DATARIENTRO,

                           }).ToList();
                return tmp;
            }
        }
        public RichiamoModel getRichiamoById(decimal idRichiamo)
        {
            RichiamoModel tmp = new RichiamoModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                RICHIAMO r = db.RICHIAMO.Find(idRichiamo);
                if (r != null)
                {
                    tmp.annullato = r.ANNULLATO;
                    tmp.IdRichiamo = r.IDRICHIAMO;
                    tmp.idTrasferimento = r.IDTRASFERIMENTO;
                    tmp.DataAggiornamento = r.DATAAGGIORNAMENTO;
                    tmp.DataPartenza = r.TRASFERIMENTO.DATAPARTENZA;
                    tmp.DataRichiamo = r.DATARICHIAMO;
                    tmp.DataRientro = r.TRASFERIMENTO.DATARIENTRO;
                }
            }
            return tmp;
        }
        public decimal SetRichiamo(RichiamoModel ric, decimal idCoeffIndRichiamo, decimal idPercentualeFKM, DateTime DataRientro, ModelDBISE db)
        {

            decimal tmp = 0;
            if (idCoeffIndRichiamo == 0 || idPercentualeFKM == 0)
                return 0;

            try
            {
                RICHIAMO ri = new RICHIAMO()
                {
                    IDTRASFERIMENTO = ric.idTrasferimento,
                    DATARICHIAMO = ric.DataRichiamo,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    ANNULLATO = ric.annullato
                };
                db.RICHIAMO.Add(ri);
                int i = db.SaveChanges();
                tmp = ri.IDRICHIAMO;
                var t = db.TRASFERIMENTO.Find(ric.idTrasferimento);
                t.IDSTATOTRASFERIMENTO = (decimal)EnumStatoTraferimento.Terminato;
                t.DATARIENTRO = DataRientro;// ric.DataRichiamo.AddDays(-1);
                t.DATAAGGIORNAMENTO = DateTime.Now;
                db.SaveChanges();
                //DataRientro = ;// ric.DataRichiamo.AddDays(-1);
                using (dtRichiamo dtr = new dtRichiamo())
                {
                    dtr.Associa_Richiamo_CoeffIndRichiamo(ri.IDRICHIAMO, idCoeffIndRichiamo, db);
                    dtr.Associa_Richiamo_PercentualeFKM(ri.IDRICHIAMO, idPercentualeFKM, db);
                }
                if (i > 0)
                {
                    ric.idTrasferimento = ri.IDTRASFERIMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo richiamo.", "Richiamo", db, ric.idTrasferimento, ri.IDTRASFERIMENTO);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tmp;
        }
        public decimal EditRichiamo(RichiamoModel ric, decimal idCoeffIndRichiamo, decimal idPercentualeFKM, DateTime DataRientro, decimal idRichiamo, ModelDBISE db)
        {

            decimal tmp = 0;
            if (idCoeffIndRichiamo == 0 || idPercentualeFKM == 0)
                return 0;

            try
            {
                var rich = db.RICHIAMO.Find(idRichiamo);
                rich.ANNULLATO = true;
                rich.DATAAGGIORNAMENTO = DateTime.Now;
                db.SaveChanges();

                RICHIAMO riNew = new RICHIAMO()
                {
                    IDTRASFERIMENTO = ric.idTrasferimento,
                    DATARICHIAMO = ric.DataRichiamo,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    ANNULLATO = false
                };
                db.RICHIAMO.Add(riNew);

                int i = db.SaveChanges();
                tmp = riNew.IDRICHIAMO;

                var t = db.TRASFERIMENTO.Find(ric.idTrasferimento);
                t.IDSTATOTRASFERIMENTO = (decimal)EnumStatoTraferimento.Terminato;
                t.DATARIENTRO = DataRientro;// ric.DataRichiamo.AddDays(-1);
                t.DATAAGGIORNAMENTO = DateTime.Now;
                db.SaveChanges();
                //DataRientro = ric.DataRichiamo.AddDays(-1);
                using (dtRichiamo dtr = new dtRichiamo())
                {
                    RimuoviAsscoiazioni_Richiamo_CoeffIndRichiamo(idRichiamo, db);
                    RimuoviAsscoiazioni_Richiamo_PercentualeFKM(idRichiamo, db);

                    dtr.Associa_Richiamo_CoeffIndRichiamo(tmp, idCoeffIndRichiamo, db);
                    dtr.Associa_Richiamo_PercentualeFKM(tmp, idPercentualeFKM, db);
                }
                if (i > 0)
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica richiamo.", "Richiamo", db, rich.IDTRASFERIMENTO, rich.IDTRASFERIMENTO);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tmp;
        }
        public void Associa_Richiamo_CoeffIndRichiamo(decimal idRichiamo, decimal idCoeffIndRichiamo, ModelDBISE db)
        {
            var tep = db.RICHIAMO.Find(idRichiamo);
            var item = db.Entry<RICHIAMO>(tep);
            item.State = EntityState.Modified;
            item.Collection(a => a.COEFFICIENTEINDRICHIAMO).Load();
            var percAnticipo = db.COEFFICIENTEINDRICHIAMO.Find(idCoeffIndRichiamo);
            tep.COEFFICIENTEINDRICHIAMO.Add(percAnticipo);
            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Non è stato possibile associare la percentuale fascia KM al Richiamo.");
            }
        }
        public void RimuoviAsscoiazioni_Richiamo_CoeffIndRichiamo(decimal idRichiamo, ModelDBISE db)
        {
            var i = db.RICHIAMO.Find(idRichiamo);
            var lCoefIndRick = i.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList();
            if (lCoefIndRick?.Any() ?? false)
            {
                foreach (var z in lCoefIndRick)
                {
                    i.COEFFICIENTEINDRICHIAMO.Remove(z);
                }
                db.SaveChanges();
            }
        }

        public void Associa_Richiamo_PercentualeFKM(decimal idRichiamo, decimal idPercentualeFKM, ModelDBISE db)
        {
            var tep = db.RICHIAMO.Find(idRichiamo);
            var item = db.Entry<RICHIAMO>(tep);
            item.State = EntityState.Modified;
            item.Collection(a => a.PERCENTUALEFKM).Load();
            var percAnticipo = db.PERCENTUALEFKM.Find(idPercentualeFKM);
            tep.PERCENTUALEFKM.Add(percAnticipo);
            int i = db.SaveChanges();
            if (i <= 0)
            {
                throw new Exception("Non è stato possibile associare la percentuale fascia KM al Richiamo.");
            }
        }
        public void RimuoviAsscoiazioni_Richiamo_PercentualeFKM(decimal idRichiamo, ModelDBISE db)
        {
            var i = db.RICHIAMO.Find(idRichiamo);
            var lperc = i.PERCENTUALEFKM.Where(a => a.ANNULLATO == false).ToList();
            if (lperc?.Any() ?? false)
            {
                foreach (var z in lperc)
                {
                    i.PERCENTUALEFKM.Remove(z);
                }
                db.SaveChanges();
            }
        }
        public decimal GetMatricolaDaIdTrasferimento(decimal idTrasferimento)
        {
            decimal tmp = 0;
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var tr = db.TRASFERIMENTO.Find(idTrasferimento);
                    tmp = Convert.ToDecimal(tr.DIPENDENTI.MATRICOLA);
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return tmp;
        }
        public IList<TrasferimentoModel> GetListaTrasferimentoPerRichiamo(decimal matricola, ModelDBISE db)
        {
            List<TrasferimentoModel> ltm = new List<TrasferimentoModel>();
            try
            {
                var d = db.DIPENDENTI.First(a => a.MATRICOLA == matricola);

                if (d?.IDDIPENDENTE > 0)
                {
                    var lt = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO);

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
        public string GetEmailByIdDipendente(decimal idDipendente)
        {
            string email = "";
            using (ModelDBISE db = new ModelDBISE())
            {
                DIPENDENTI d = db.DIPENDENTI.Find(idDipendente);
                email = d.EMAIL;
            }
            return email;
        }
        public decimal Restituisci_ID_Destinatario(decimal idTrasferimento)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                var atvc = db.TRASFERIMENTO.Find(idTrasferimento);
                tmp = atvc.DIPENDENTI.IDDIPENDENTE;
            }
            return tmp;
        }
        public decimal RestituisciIDdestinatarioDaEmail(string email)
        {
            decimal idDipendente = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    idDipendente = (from e in db.DIPENDENTI
                                    where e.EMAIL.ToUpper() == email.ToUpper() && e.ABILITATO == true
                                    select new DipendentiModel()
                                    {
                                        idDipendente = e.IDDIPENDENTE,
                                    }).ToList().First().idDipendente;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return idDipendente;
        }
        public DipendentiModel RestituisciDipendenteByID(decimal idDipendente, ModelDBISE db)
        {
            DipendentiModel dm = new DipendentiModel();
            DIPENDENTI d = db.DIPENDENTI.Find(idDipendente);
            dm.idDipendente = d.IDDIPENDENTE;
            dm.nome = d.NOME; dm.cognome = d.COGNOME;
            dm.email = d.EMAIL; d.INDIRIZZO = d.INDIRIZZO;
            return dm;
        }
        public string DeterminaSede(decimal idTrasferimento)
        {
            string tmp = "";
            using (ModelDBISE db = new ModelDBISE())
            {
                var atvc = db.TRASFERIMENTO.Find(idTrasferimento);
                tmp = atvc.UFFICI.DESCRIZIONEUFFICIO + " (" + atvc.UFFICI.CODICEUFFICIO + ")";
            }
            return tmp;
        }
        //GetDataRientroPrecedente
    }
}