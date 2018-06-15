using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.Tools;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Models.dtObj
{
    public class dtLivelliDipendente : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public LivelloDipendenteModel GetLivelloDipendenteByIdTrasf(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            LivelloDipendenteModel ldm = new LivelloDipendenteModel();

            var lld = db.INDENNITA.Find(idTrasferimento)
                        .LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false &&
                                                 dt >= a.DATAINIZIOVALIDITA &&
                                                 dt <= a.DATAFINEVALIDITA)
                                          .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                          .ToList();
            if (lld != null && lld.Count > 0)
            {
                var ld = lld.First();

                ldm = new LivelloDipendenteModel()
                {
                    idLivDipendente = ld.IDLIVDIPENDENTE,
                    idDipendente = ld.IDDIPENDENTE,
                    idLivello = ld.IDLIVELLO,
                    dataInizioValdita = ld.DATAINIZIOVALIDITA,
                    dataFineValidita = ld.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ld.DATAFINEVALIDITA,
                    dataAggiornamento = ld.DATAAGGIORNAMENTO,
                    annullato = ld.ANNULLATO

                };
            }


            return ldm;
        }

        public IList<LivelloDipendenteModel> GetLivelloDipendenteByIdTrasferimento(decimal idTrasferimento)
        {
            List<LivelloDipendenteModel> lldm = new List<LivelloDipendenteModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lld = db.INDENNITA.Find(idTrasferimento)
                        .LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false)
                                          .OrderBy(a => a.IDDIPENDENTE)
                                          .ToList();
                
                lldm = (from e in lld
                        select new LivelloDipendenteModel()
                        {
                            idLivDipendente = e.IDLIVDIPENDENTE,
                            idDipendente = e.IDDIPENDENTE,
                            idLivello = e.IDLIVELLO,
                            dataInizioValdita = e.DATAINIZIOVALIDITA,
                            dataFineValidita = e.DATAFINEVALIDITA,
                            dataAggiornamento = e.DATAAGGIORNAMENTO,
                            annullato = e.ANNULLATO,
                            Livello = new LivelloModel()
                            {
                                idLivello = e.LIVELLI.IDLIVELLO,
                                DescLivello = e.LIVELLI.LIVELLO
                            }
                        }).ToList();
                
            }
            

            return lldm;
        }


        public LivelloDipendenteModel GetLivelloDipendente(decimal idDipendente, DateTime data)
        {
            LivelloDipendenteModel ldm = new LivelloDipendenteModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ld = db.LIVELLIDIPENDENTI.Where(a => a.IDDIPENDENTE == idDipendente && data >= a.DATAINIZIOVALIDITA && data <= a.DATAFINEVALIDITA && a.ANNULLATO==false).ToList();

                ldm = (from e in ld
                       select new LivelloDipendenteModel()
                       {
                           idLivDipendente = e.IDLIVDIPENDENTE,
                           idDipendente = e.IDDIPENDENTE,
                           idLivello = e.IDLIVELLO,
                           dataInizioValdita = e.DATAINIZIOVALIDITA,
                           dataFineValidita = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                           annullato = e.ANNULLATO,
                           Livello = new LivelloModel()
                           {
                               idLivello = e.LIVELLI.IDLIVELLO,
                               DescLivello = e.LIVELLI.LIVELLO
                           }
                       }).First();
            }

            return ldm;
        }
        /// <summary>
        /// Preleva i dipendenti per il range di date passate.
        /// </summary>
        /// <param name="idDipendente"></param>
        /// <param name="dtIni"></param>
        /// <param name="dtFin"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public IList<LivelloDipendenteModel> GetLivelliDipendentiByRangeDate(decimal idDipendente, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            List<LivelloDipendenteModel> lldm = new List<LivelloDipendenteModel>();

            var lld =
                db.LIVELLIDIPENDENTI.Where(
                    a =>
                        a.ANNULLATO == false && a.IDDIPENDENTE == idDipendente && a.DATAFINEVALIDITA >= dtIni &&
                        a.DATAINIZIOVALIDITA <= dtFin)
                    .OrderBy(a => a.DATAINIZIOVALIDITA);

            lldm = (from e in lld
                    select new LivelloDipendenteModel()
                    {
                        idLivDipendente = e.IDLIVDIPENDENTE,
                        idDipendente = e.IDDIPENDENTE,
                        idLivello = e.IDLIVELLO,
                        dataInizioValdita = e.DATAINIZIOVALIDITA,
                        dataFineValidita = e.DATAFINEVALIDITA,
                        dataAggiornamento = e.DATAAGGIORNAMENTO,
                        annullato = e.ANNULLATO,
                        Livello = new LivelloModel()
                        {
                            idLivello = e.LIVELLI.IDLIVELLO,
                            DescLivello = e.LIVELLI.LIVELLO
                        }
                    }).ToList();


            return lldm;

        }

        public LivelloDipendenteModel GetLivelloDipendente(decimal idDipendente, DateTime data, ModelDBISE db)
        {
            LivelloDipendenteModel ldm = new LivelloDipendenteModel();


            var ld = db.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false &&
                                                a.IDDIPENDENTE == idDipendente &&
                                                data >= a.DATAINIZIOVALIDITA &&
                                                data.Date <= a.DATAFINEVALIDITA)
                                         .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                         .ToList();

            ldm = (from e in ld
                   select new LivelloDipendenteModel()
                   {
                       idLivDipendente = e.IDLIVDIPENDENTE,
                       idDipendente = e.IDDIPENDENTE,
                       idLivello = e.IDLIVELLO,
                       dataInizioValdita = e.DATAINIZIOVALIDITA,
                       dataFineValidita = e.DATAFINEVALIDITA,
                       dataAggiornamento = e.DATAAGGIORNAMENTO,
                       annullato = e.ANNULLATO,
                       Livello = new LivelloModel()
                       {
                           idLivello = e.LIVELLI.IDLIVELLO,
                           DescLivello = e.LIVELLI.LIVELLO
                       }
                   }).First();


            return ldm;
        }


        public void AssociaLivelloDipendente_Indennita(decimal idTrasferimento, decimal idLivelloDipendente, ModelDBISE db)
        {

            try
            {
                var i = db.INDENNITA.Find(idTrasferimento);

                var item = db.Entry<INDENNITA>(i);

                item.State = System.Data.Entity.EntityState.Modified;

                item.Collection(a => a.LIVELLIDIPENDENTI).Load();

                //i.LIVELLIDIPENDENTI.Clear();

                var l = db.LIVELLIDIPENDENTI.Find(idLivelloDipendente);

                i.LIVELLIDIPENDENTI.Add(l);

                if (db.SaveChanges() > 0)
                {
                    //Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Associazione del livello dipendente all'indennità.", "LIVELLIDIPENDENTI", db, idTrasferimento, idLivelloDipendente);
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void RimuoviAssociazioneLivelliDipendente_Indennita(decimal idTrasferimento, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
            var lld =
                i.LIVELLIDIPENDENTI.Where(
                    a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= dtIni && a.DATAINIZIOVALIDITA <= dtFin)
                    .OrderBy(a => a.DATAINIZIOVALIDITA);
            if (lld?.Any() ?? false)
            {
                foreach (var ld in lld)
                {
                    i.LIVELLIDIPENDENTI.Remove(ld);
                }

                db.SaveChanges();
            }
        }


        public void RimuoviAssociazioneLivelloDipendente_Indennita(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            //var i = db.INDENNITA.Find(idTrasferimento);
            //var item = db.Entry<INDENNITA>(i);
            //item.State = System.Data.Entity.EntityState.Modified;
            //item.Collection(a => a.LIVELLIDIPENDENTI).Load();
            //var n = i.LIVELLIDIPENDENTI.ToList().RemoveAll(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA);

            //if (n > 0)
            //{
            //    db.SaveChanges();
            //}

            var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
            var lit = i.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).ToList();

            foreach (var item in lit)
            {
                i.LIVELLIDIPENDENTI.Remove(item);
            }
            db.SaveChanges();

        }
    }
}