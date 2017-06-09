using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtIndennitaBase : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IndennitaBaseModel GetIndennitaBaseValida(decimal idLivello, DateTime dt, EntitiesDBISE db)
        {

            IndennitaBaseModel ibm = new IndennitaBaseModel();

            List<INDENNITABASE> lib = db.INDENNITABASE.Where(a => a.ANNULLATO == false &&
                                                      a.IDLIVELLO == idLivello &&
                                                      dt >= a.DATAINIZIOVALIDITA &&
                                                      dt <= a.DATAFINEVALIDITA)
                                               .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

            if (lib != null && lib.Count > 0)
            {
                var ib = lib.First();

                ibm = new IndennitaBaseModel()
                {
                    idIndennitaBase = ib.IDINDENNITABASE,
                    idLivello = ib.IDLIVELLO,
                    idRiduzioni = ib.IDRIDUZIONI,
                    dataInizioValidita = ib.DATAINIZIOVALIDITA,
                    dataFineValidita = ib.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? ib.DATAFINEVALIDITA : new DateTime?(),
                    valore = ib.VALORE,
                    valoreResponsabile = ib.VALORERESP,
                    dataAggiornamento = ib.DATAAGGIORNAMENTO,
                    annullato = ib.ANNULLATO,
                    Livello = new LivelloModel()
                    {
                        idLivello = ib.LIVELLI.IDLIVELLO,
                        DescLivello = ib.LIVELLI.LIVELLO
                    },
                    Riduzioni = new RiduzioniModel()
                    {
                        idRiduzioni = ib.RIDUZIONI.IDRIDUZIONI,
                        idRegola = ib.RIDUZIONI.IDREGOLA,
                        dataInizioValidita = ib.RIDUZIONI.DATAINIZIOVALIDITA,
                        dataFineValidita = ib.RIDUZIONI.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? ib.RIDUZIONI.DATAFINEVALIDITA : new DateTime?(),
                        percentuale = ib.RIDUZIONI.PERCENTUALE,
                        dataAggiornamento = ib.RIDUZIONI.DATAAGGIORNAMENTO,
                        annullato = ib.RIDUZIONI.ANNULLATO
                    }
                };
            }

            return ibm;


        }



    }
}