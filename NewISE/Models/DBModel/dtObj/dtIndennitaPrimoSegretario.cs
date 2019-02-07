using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtIndennitaPrimoSegretario : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IndennitaPrimoSegretModel GetIndennitaPrimoSegretario(decimal idFiglio, DateTime dt, ModelDBISE db)
        {
            IndennitaPrimoSegretModel ipsm = new IndennitaPrimoSegretModel();

            var f = db.FIGLI.Find(idFiglio);

            var lips =
                f.INDENNITAPRIMOSEGRETARIO.Where(
                    a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                    .ToList();

            if (lips?.Any() ?? false)
            {
                var ips = lips.First();

                ipsm = new IndennitaPrimoSegretModel()
                {
                    idIndPrimoSegr = ips.IDINDPRIMOSEGR,
                    dataInizioValidita = ips.DATAINIZIOVALIDITA,
                    dataFineValidita = ips.DATAFINEVALIDITA,
                    indennita = ips.INDENNITA,
                    dataAggiornamento = ips.DATAAGGIORNAMENTO,
                    annullato = ips.ANNULLATO
                };
            }

            return ipsm;

        }


        public IList<IndennitaPrimoSegretModel> GetIndennitaPrimoSegretario(DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            List<IndennitaPrimoSegretModel> lipsm = new List<IndennitaPrimoSegretModel>();

            var lips =
                db.INDENNITAPRIMOSEGRETARIO.Where(
                    a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= dtFin && a.DATAFINEVALIDITA >= dtIni)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();

            if (lips?.Any() ?? false)
            {
                lipsm = (from e in lips
                         select new IndennitaPrimoSegretModel()
                         {
                             idIndPrimoSegr = e.IDINDPRIMOSEGR,
                             dataInizioValidita = e.DATAINIZIOVALIDITA,
                             dataFineValidita = e.DATAFINEVALIDITA,
                             indennita = e.INDENNITA,
                             dataAggiornamento = e.DATAAGGIORNAMENTO,
                             annullato = e.ANNULLATO
                         }).ToList();
            }

            return lipsm;
        }


        public void AssociaIndennitaPrimoSegretarioFiglio(decimal idFiglio, decimal idIndennitaPrimoSegretario, ModelDBISE db)
        {
            try
            {
                var f = db.FIGLI.Find(idFiglio);
                var item = db.Entry<FIGLI>(f);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.INDENNITAPRIMOSEGRETARIO).Load();
                var ips = db.INDENNITAPRIMOSEGRETARIO.Find(idIndennitaPrimoSegretario);
                f.INDENNITAPRIMOSEGRETARIO.Add(ips);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare l'indennità di primo segretario per il figlio {0}.", f.COGNOME + " " + f.NOME));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void RimuoviAssociazione_Figlio_IndennitaPrimoSegretario(decimal idFiglio, ModelDBISE db)
        {
            var f = db.FIGLI.Find(idFiglio);
            var lps = f.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).ToList();
            if (lps?.Any() ?? false)
            {
                foreach (var ps in lps)
                {
                    f.INDENNITAPRIMOSEGRETARIO.Remove(ps);
                }

                db.SaveChanges();
            }

        }


    }
}