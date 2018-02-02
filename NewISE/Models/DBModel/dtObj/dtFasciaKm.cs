using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using Newtonsoft.Json.Schema;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtFasciaKm : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        //public void RimuoviCoefficientiSede_Indennita(decimal idTrasferimento, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        //{
        //    var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
        //    var lcs =
        //        i.COEFFICIENTESEDE.Where(
        //            a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= dtIni && a.DATAINIZIOVALIDITA <= dtFin)
        //            .OrderBy(a => a.DATAINIZIOVALIDITA);
        //    if (lcs?.Any() ?? false)
        //    {
        //        foreach (var cs in lcs)
        //        {
        //            db.COEFFICIENTESEDE.Remove(cs);
        //        }

        //        db.SaveChanges();
        //    }
        //}

        public Fascia_KMModel GetFasciaKmByTrasf(decimal idTrasferimento, DateTime dt)
        {
            Fascia_KMModel fkmm = new Fascia_KMModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var ps = t.PRIMASITEMAZIONE;
                var lpfkm =
                    ps.PERCENTUALEFKM.Where(
                        a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                        .OrderByDescending(a => a.DATAINIZIOVALIDITA);

                if (lpfkm?.Any() ?? false)
                {
                    var pfkm = lpfkm.First();

                    var fkm = pfkm.FASCIA_KM;


                    fkmm = new Fascia_KMModel()
                    {
                        idFKM = fkm.IDFKM,
                        idGruppoFKM = fkm.IDGRUPPOFKM,
                        KM = fkm.KM
                    };

                }
            }

            return fkmm;

        }

        public void RimuoviAssociazionePercentualeFKM(decimal idTrasferimento, ModelDBISE db)
        {
            var ps = db.TRASFERIMENTO.Find(idTrasferimento).PRIMASITEMAZIONE;

            var lpfkm = ps.PERCENTUALEFKM.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIOVALIDITA);

            if (lpfkm?.Any() ?? false)
            {
                foreach (var pfkm in lpfkm)
                {
                    ps.PERCENTUALEFKM.Remove(pfkm);
                }

                db.SaveChanges();
            }


        }


        public void AssociaPercentualeFKMPrimaSistemazione(decimal idPrimaSistemazione, decimal idPercentualeFKM, ModelDBISE db)
        {
            try
            {
                var i = db.PRIMASITEMAZIONE.Find(idPrimaSistemazione);

                var item = db.Entry<PRIMASITEMAZIONE>(i);

                item.State = System.Data.Entity.EntityState.Modified;

                item.Collection(a => a.PERCENTUALEFKM).Load();



                var e = db.PERCENTUALEFKM.Find(idPercentualeFKM);

                i.PERCENTUALEFKM.Add(e);

                db.SaveChanges();


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public PercentualeFasciaKMModel GetPercentualeFKM(decimal idFKM, DateTime dt, ModelDBISE db)
        {
            PercentualeFasciaKMModel pfkmm = new PercentualeFasciaKMModel();

            var lpfkm =
                db.PERCENTUALEFKM.Where(
                    a =>
                        a.IDFKM == idFKM && a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA &&
                        dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAFINEVALIDITA);

            if (lpfkm?.Any() ?? false)
            {
                var pfkm = lpfkm.First();

                pfkmm = new PercentualeFasciaKMModel()
                {
                    idPFKM = pfkm.IDPFKM,
                    idFKM = pfkm.IDFKM,
                    dataInizioValidita = pfkm.DATAINIZIOVALIDITA,
                    dataFineValidita = pfkm.DATAFINEVALIDITA,
                    coefficenteFKM = pfkm.COEFFICIENTEKM,
                    dataAggiornamento = pfkm.DATAAGGIORNAMENTO,
                    annullato = pfkm.ANNULLATO
                };
            }

            return pfkmm;

        }

        public IList<Fascia_KMModel> GetListFascieChilometriche(DateTime? dt = null)
        {
            List<Fascia_KMModel> lFKM = new List<Fascia_KMModel>();
            DateTime dataIntercettazioneFascia = DateTime.Now;

            if (dt.HasValue)
            {
                dataIntercettazioneFascia = dt.Value;
            }

            using (ModelDBISE db = new ModelDBISE())
            {
                var lgfkm =
                    db.GRUPPO_FKM.Where(
                        a =>
                            a.ANNULLATO == false && dataIntercettazioneFascia >= a.DATAINIZIOVALIDITA &&
                            dataIntercettazioneFascia <= a.DATAFINEVALIDITA)
                        .OrderByDescending(a => a.DATAFINEVALIDITA);

                if (lgfkm?.Any() ?? false)
                {
                    var gfkm = lgfkm.First();

                    lFKM = (from e in gfkm.FASCIA_KM
                            select new Fascia_KMModel()
                            {
                                idFKM = e.IDFKM,
                                idGruppoFKM = e.IDGRUPPOFKM,
                                KM = e.KM,
                                GruppoFKM = new Gruppo_FKMModel()
                                {
                                    idGruppoFK = e.GRUPPO_FKM.IDGRUPPOFK,
                                    leggeFasciaKM = e.GRUPPO_FKM.LEGGEFASCIAKM,
                                    dataInizioValidita = e.GRUPPO_FKM.DATAINIZIOVALIDITA,
                                    dataFineValidita = e.GRUPPO_FKM.DATAFINEVALIDITA,
                                    dataAggiornamento = e.GRUPPO_FKM.DATAAGGIORNAMENTO,
                                    annullato = e.GRUPPO_FKM.ANNULLATO
                                }
                            }).ToList();
                }


            }

            return lFKM;

        }





    }
}