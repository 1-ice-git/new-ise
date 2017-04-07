using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtIndennita : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IndennitaModel getIndennitaByDataDecorrenza(int matricola, DateTime decorrenza)
        {
            IndennitaModel im = new IndennitaModel();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    var li = db.INDENNITA.Where(a => a.ANNULLATO == false).Where(a => a.DATAINIZIO <= decorrenza && a.DATAFINE >= decorrenza);

                    if (li.Count() == 1)
                    {
                        var i = li.First();

                        im = new IndennitaModel()
                        {
                            idIndennita = i.IDINDENNITA,
                            idTrasferimento = i.IDTRASFERIMENTO,
                            idIndennitaBase = i.IDINDENNITABASE,
                            idTFR = i.IDTFR,
                            idPercentualeDisagio = i.IDPERCENTUALEDISAGIO,
                            idCoefficenteSede = i.IDCOEFFICENTESEDE,
                            dataInizio = i.DATAINIZIO,
                            dataFine = i.DATAFINE,
                            annullato = i.ANNULLATO,
                            IndennitaBase = new IndennitaBaseModel()
                            {
                                idIndennitaBase = i.INDENNITABASE.IDINDENNITABASE,
                                idLivello = i.INDENNITABASE.IDLIVELLO,
                                dataInizioValidita = i.INDENNITABASE.DATAINIZIOVALIDITA,
                                dataFineValidita = i.INDENNITABASE.DATAFINEVALIDITA,
                                valore = i.INDENNITABASE.VALORE,
                                valoreResponsabile = i.INDENNITABASE.VALORERESP,
                                annullato = i.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = i.INDENNITABASE.LIVELLI.IDLIVELLO,
                                    DescLivello = i.INDENNITABASE.LIVELLI.LIVELLO
                                }
                                
                                

                            }
                        };
                    }
                    else
                    {
                        throw new Exception("Il numero di indennità trovate non dovrebbe superare un unità.");
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }


            return im;
        }
    }
}