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
        
        public void AssociaPercentualeDisagio_Indennita(decimal idTrasferimento,  decimal id, EntitiesDBISE db)
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

        public PercentualeDisagioModel GetPercentualeDisagioByIdTrasf(decimal idTrasferimento, DateTime dt, EntitiesDBISE db)
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
                    dataFineValidita = pd.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : pd.DATAFINEVALIDITA,
                    percentuale = pd.PERCENTUALE,
                    dataAggiornamento = pd.DATAAGGIORNAMENTO,
                    annullato = pd.ANNULLATO,
                    Ufficio = new UfficiModel()
                    {
                        idUfficio = pd.UFFICI.IDUFFICIO,
                        idValuta = pd.UFFICI.IDVALUTA,
                        codiceUfficio = pd.UFFICI.CODICEUFFICIO,
                        descUfficio = pd.UFFICI.DESCRIZIONEUFFICIO,
                        pagatoValutaUfficio = pd.UFFICI.PAGATOVALUTAUFFICIO,
                        ValutaUfficio = new ValuteModel()
                        {
                            idValuta = pd.UFFICI.VALUTE.IDVALUTA,
                            descrizioneValuta = pd.UFFICI.VALUTE.DESCRIZIONEVALUTA
                        }
                    }
                };
            }

            return pdm;
        }

        public PercentualeDisagioModel GetPercentualeDisagio(decimal idPercentualeDisagio, EntitiesDBISE db)
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
                    dataFineValidita = pd.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : pd.DATAFINEVALIDITA,
                    percentuale = pd.PERCENTUALE,
                    dataAggiornamento = pd.DATAAGGIORNAMENTO,
                    annullato = pd.ANNULLATO
                };
            }

            return pdm;
        }


        public PercentualeDisagioModel GetPercentualeDisagioValida(decimal idUfficio, DateTime dt, EntitiesDBISE db)
        {
            PercentualeDisagioModel pdm = new PercentualeDisagioModel();

            var lpd = db.PERCENTUALEDISAGIO.Where(a=>a.ANNULLATO == false && 
                                                  a.IDUFFICIO == idUfficio &&
                                                  dt >= a.DATAINIZIOVALIDITA &&
                                                  dt<= a.DATAFINEVALIDITA)
                                           .OrderByDescending(a=>a.DATAINIZIOVALIDITA).ToList();

            if (lpd != null && lpd.Count > 0)
            {
                PERCENTUALEDISAGIO pd = lpd.First();
                pdm = new PercentualeDisagioModel()
                {
                    idPercentualeDisagio = pd.IDPERCENTUALEDISAGIO,
                    idUfficio = pd.IDUFFICIO,
                    dataInizioValidita = pd.DATAINIZIOVALIDITA,
                    dataFineValidita = pd.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : pd.DATAFINEVALIDITA,
                    percentuale = pd.PERCENTUALE,
                    dataAggiornamento = pd.DATAAGGIORNAMENTO,
                    annullato = pd.ANNULLATO
                };
            }


            return pdm;
        }



    }
}