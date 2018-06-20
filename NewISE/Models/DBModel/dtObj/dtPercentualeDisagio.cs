
using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPercentualeDisagio : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void AssociaPercentualeDisagio_Indennita(decimal idTrasferimento, decimal id, ModelDBISE db)
        {

            try
            {
                var i = db.INDENNITA.Find(idTrasferimento);

                var item = db.Entry<INDENNITA>(i);

                item.State = System.Data.Entity.EntityState.Modified;

                item.Collection(a => a.PERCENTUALEDISAGIO).Load();

                var l = db.PERCENTUALEDISAGIO.Find(id);

                i.PERCENTUALEDISAGIO.Add(l);

                db.SaveChanges();


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void RimuoviAssociazioniPercentualeDisagio_Indennita(decimal idTrasferimento, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
            var lpd =
                i.PERCENTUALEDISAGIO.Where(
                    a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= dtIni && a.DATAINIZIOVALIDITA <= dtFin)
                    .OrderBy(a => a.DATAINIZIOVALIDITA);
            if (lpd?.Any() ?? false)
            {
                foreach (var pd in lpd)
                {
                    i.PERCENTUALEDISAGIO.Remove(pd);
                }

                db.SaveChanges();
            }
        }

        public void RimuoviAssociaPercentualeDisagio_Indennita(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            //var i = db.INDENNITA.Find(idTrasferimento);

            //var item = db.Entry<INDENNITA>(i);

            //item.State = System.Data.Entity.EntityState.Modified;

            //item.Collection(a => a.PERCENTUALEDISAGIO).Load();

            //var n = i.PERCENTUALEDISAGIO.ToList().RemoveAll(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA);

            //if (n > 0)
            //    db.SaveChanges();

            var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
            var lit = i.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).ToList();

            foreach (var item in lit)
            {
                i.PERCENTUALEDISAGIO.Remove(item);
            }
            db.SaveChanges();

        }

        public PercentualeDisagioModel GetPercentualeDisagioByIdTrasf(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            PercentualeDisagioModel pdm = new PercentualeDisagioModel();

            var lpd = db.INDENNITA.Find(idTrasferimento)
                                  .PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false &&
                                                            dt >= a.DATAINIZIOVALIDITA &&
                                                            dt <= a.DATAFINEVALIDITA)
                                                     .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                                     .ToList();

            if (lpd != null && lpd.Count > 0)
            {
                var pd = lpd.First();

                pdm = new PercentualeDisagioModel()
                {
                    idPercentualeDisagio = pd.IDPERCENTUALEDISAGIO,
                    idUfficio = pd.IDUFFICIO,
                    dataInizioValidita = pd.DATAINIZIOVALIDITA,
                    dataFineValidita = pd.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : pd.DATAFINEVALIDITA,
                    percentuale = pd.PERCENTUALE,
                    dataAggiornamento = pd.DATAAGGIORNAMENTO,
                    annullato = pd.ANNULLATO,
                    Ufficio = new UfficiModel()
                    {
                        idUfficio = pd.UFFICI.IDUFFICIO,
                        codiceUfficio = pd.UFFICI.CODICEUFFICIO,
                        descUfficio = pd.UFFICI.DESCRIZIONEUFFICIO,
                        pagatoValutaUfficio = pd.UFFICI.PAGATOVALUTAUFFICIO,

                    }
                };
            }

            return pdm;
        }

        public PercentualeDisagioModel GetPercentualeDisagioByIdTrasferimento(decimal idTrasferimento)
        {

            PercentualeDisagioModel pdm = new PercentualeDisagioModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lrd = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false);
                //var lrd = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == 79 && a.ANNULLATO == false).ToList();

                var pd = lrd.First();
                if (lrd?.Any() ?? false)
                {
                    pdm = new PercentualeDisagioModel()
                    {
                        idPercentualeDisagio = pd.IDPERCENTUALEDISAGIO,
                        idUfficio = pd.IDUFFICIO,
                        dataInizioValidita = pd.DATAINIZIOVALIDITA,
                        dataFineValidita = pd.DATAFINEVALIDITA,
                        dataAggiornamento = pd.DATAAGGIORNAMENTO,
                        annullato = pd.ANNULLATO,
                    };
                }
            }
            return pdm;
        }

        public PercentualeDisagioModel GetPercentualeDisagio(decimal idPercentualeDisagio, ModelDBISE db)
        {
            PercentualeDisagioModel pdm = new PercentualeDisagioModel();

            var pd = db.PERCENTUALEDISAGIO.Find(idPercentualeDisagio);

            if (pd != null && pd.IDPERCENTUALEDISAGIO > 0)
            {
                pdm = new PercentualeDisagioModel()
                {
                    idPercentualeDisagio = pd.IDPERCENTUALEDISAGIO,
                    idUfficio = pd.IDUFFICIO,
                    dataInizioValidita = pd.DATAINIZIOVALIDITA,
                    dataFineValidita = pd.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : pd.DATAFINEVALIDITA,
                    percentuale = pd.PERCENTUALE,
                    dataAggiornamento = pd.DATAAGGIORNAMENTO,
                    annullato = pd.ANNULLATO
                };
            }

            return pdm;
        }

        public IList<PercentualeDisagioModel> GetPercentualeDisagioIndennitaByRange(decimal idUfficio, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            List<PercentualeDisagioModel> lPercentualeDisagio = new List<PercentualeDisagioModel>();

            var u = db.UFFICI.Find(idUfficio);

            var lpd =
                u.PERCENTUALEDISAGIO.Where(
                    a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= dtIni && a.DATAFINEVALIDITA <= dtFin)
                    .OrderBy(a => a.DATAINIZIOVALIDITA);

            if (lpd?.Any() ?? false)
            {
                foreach (var pd in lpd)
                {
                    var pdm = new PercentualeDisagioModel()
                    {
                        idPercentualeDisagio = pd.IDPERCENTUALEDISAGIO,
                        idUfficio = pd.IDUFFICIO,
                        dataInizioValidita = pd.DATAINIZIOVALIDITA,
                        dataFineValidita =
                            pd.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : pd.DATAFINEVALIDITA,
                        percentuale = pd.PERCENTUALE,
                        dataAggiornamento = pd.DATAAGGIORNAMENTO,
                        annullato = pd.ANNULLATO
                    };

                    lPercentualeDisagio.Add(pdm);
                }
            }


            return lPercentualeDisagio;

        }
        
        public PercentualeDisagioModel GetPercentualeDisagioValida(decimal idUfficio, DateTime dt, ModelDBISE db)
        {
            PercentualeDisagioModel pdm = new PercentualeDisagioModel();

            var lpd = db.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false &&
                                                  a.IDUFFICIO == idUfficio &&
                                                  dt >= a.DATAINIZIOVALIDITA &&
                                                  dt <= a.DATAFINEVALIDITA)
                                           .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

            if (lpd != null && lpd.Count > 0)
            {
                PERCENTUALEDISAGIO pd = lpd.First();
                pdm = new PercentualeDisagioModel()
                {
                    idPercentualeDisagio = pd.IDPERCENTUALEDISAGIO,
                    idUfficio = pd.IDUFFICIO,
                    dataInizioValidita = pd.DATAINIZIOVALIDITA,
                    dataFineValidita = pd.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : pd.DATAFINEVALIDITA,
                    percentuale = pd.PERCENTUALE,
                    dataAggiornamento = pd.DATAAGGIORNAMENTO,
                    annullato = pd.ANNULLATO
                };
            }


            return pdm;
        }



    }
}