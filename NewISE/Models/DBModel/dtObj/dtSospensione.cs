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
        public void InsertSospensione(ref SospensioneModel cem)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                SOSPENSIONE ca = new SOSPENSIONE();
                ca.ANNULLATO = cem.ANNULLATO;
                ca.DATAINIZIO = cem.DataInizioSospensione;
                ca.DATAFINE = cem.DataFineSospensione;
                ca.IDTRASFERIMENTO = cem.idTrasferimento;
                ca.IDSOSPENSIONE = cem.idSospensione;
                db.SOSPENSIONE.Add(ca);
                int i = db.SaveChanges();
                if (i <= 0)
                {
                    throw new Exception("Errore nella fase d'inserimento della sospensione");
                }
                else
                {
                    cem.idSospensione = ca.IDSOSPENSIONE;//per il ref parametro
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento Sospensione",
                        "SOSPENSIONE", db, ca.IDTRASFERIMENTO, ca.IDSOSPENSIONE);
                }
            }            
        }

        public void DeleteSospensione( SospensioneModel cem)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                SOSPENSIONE ca = new SOSPENSIONE();
                ca.ANNULLATO = cem.ANNULLATO;
                ca.DATAINIZIO = cem.DataInizioSospensione;
                ca.DATAFINE = cem.DataFineSospensione;
                ca.IDTRASFERIMENTO = cem.idTrasferimento;
                ca.IDSOSPENSIONE = cem.idSospensione;
                db.SOSPENSIONE.Remove(ca);
                int i = db.SaveChanges();
                if (i <= 0)
                {
                    throw new Exception("Errore nella fase della cancellazione della sospensione");
                }
                else
                {
                    cem.idSospensione = ca.IDSOSPENSIONE;//per il ref parametro
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Cancellazione Sospensione",
                        "SOSPENSIONE", db, ca.IDTRASFERIMENTO, ca.IDSOSPENSIONE);
                }
            }
        }

        public List<SospensioneModel> GetLista_Sospensioni(decimal matricola)
        {
            AccountModel am = new AccountModel();
            List<SospensioneModel> tmp = new List<SospensioneModel>();
            bool admin = Utility.Amministratore(out am);
            using (ModelDBISE db = new ModelDBISE())
            {
                if (admin)
                {
                    var lce = db.DIPENDENTI.Where(c => c.MATRICOLA == matricola);
                    //decimal g = lce.FirstOrDefault().IDDIPENDENTE;
                    tmp = (from e in db.SOSPENSIONE
                           where e.ANNULLATO == false && e.TRASFERIMENTO.IDDIPENDENTE == lce.FirstOrDefault().IDDIPENDENTE 
                           select new SospensioneModel()
                           {
                               DataInizioSospensione = e.DATAINIZIO,
                               DataFineSospensione = e.DATAFINE,
                               TipoSospensione = e.TIPOSOSPENSIONE.DESCRIZIONE,
                               DataAggiornamento = e.DATAAGGIORNAMENTO,
                               NumeroGiorni = DbFunctions.DiffDays(e.DATAINIZIO, e.DATAFINE).Value
                           }).ToList();
                    return tmp;
                }
                else
                {
                    var lce = db.DIPENDENTI.Where(c => c.MATRICOLA == matricola);
                    //decimal g = lce.FirstOrDefault().IDDIPENDENTE;
                    tmp = (from e in db.SOSPENSIONE
                           where e.ANNULLATO == false && e.TRASFERIMENTO.IDDIPENDENTE == lce.FirstOrDefault().IDDIPENDENTE &&
                            e.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE == am.idDipendente &&
                            e.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE == am.idDipendente
                           select new SospensioneModel()
                           {
                               DataInizioSospensione = e.DATAINIZIO,
                               DataFineSospensione = e.DATAFINE,
                               TipoSospensione = e.TIPOSOSPENSIONE.DESCRIZIONE,
                               DataAggiornamento = e.DATAAGGIORNAMENTO,
                               NumeroGiorni = DbFunctions.DiffDays(e.DATAINIZIO, e.DATAFINE).Value
                           }).ToList();
                    return tmp;
                }
            }       
        }
    }
}