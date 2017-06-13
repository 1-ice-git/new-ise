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


        public IList<IndennitaModel> GetIndennitaByIdTrasferimento(decimal idTrasferimento)
        {
            List<IndennitaModel> lIndeniita = new List<IndennitaModel>();

            using (EntitiesDBISE db=new EntitiesDBISE())
            {
                var li = db.INDENNITA.Where(a => a.ANNULLATO == false && a.IDTRASFERIMENTO == idTrasferimento).OrderBy(a => a.DATAINIZIO).ToList();

                if (li != null && li.Count > 0)
                {
                    lIndeniita = (from e in li
                                  select new IndennitaModel()
                                  {
                                      idIndennita = e.IDINDENNITA,
                                      idTrasferimento = e.IDTRASFERIMENTO,
                                      idLivDipendente = e.IDLIVDIPENDENTE,
                                      idIndennitaBase = e.IDINDENNITABASE,
                                      idTFR = e.IDTFR,
                                      idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
                                      idCoefficenteSede = e.IDCOEFFICIENTESEDE,
                                      idMaggiorazioneConiuge = e.IDMAGGIORAZIONECONIUGE,
                                      idMaggiorazioneFigli = e.IDMAGGIORAZIONEFIGLI,
                                      idRuoloDipendente = e.IDRUOLODIPENDENTE,
                                      dataInizio = e.DATAINIZIO,
                                      dataFine = e.DATAFINE,
                                      dataAggiornamento = e.DATAAGGIORNAMENTO,
                                      annullato = e.ANNULLATO
                                  }).ToList();
                }
            }

            return lIndeniita;
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
                            idCoefficenteSede = i.IDCOEFFICIENTESEDE,
                            dataInizio = i.DATAINIZIO,
                            dataFine = i.DATAFINE,
                            dataAggiornamento = i.DATAAGGIORNAMENTO,
                            annullato = i.ANNULLATO,
                            IndennitaBase = new IndennitaBaseModel()
                            {
                                idIndennitaBase = i.INDENNITABASE.IDINDENNITABASE,
                                idLivello = i.INDENNITABASE.IDLIVELLO,
                                dataInizioValidita = i.INDENNITABASE.DATAINIZIOVALIDITA,
                                dataFineValidita = i.INDENNITABASE.DATAFINEVALIDITA,
                                valore = i.INDENNITABASE.VALORE,
                                valoreResponsabile = i.INDENNITABASE.VALORERESP,
                                dataAggiornamento = i.INDENNITABASE.DATAAGGIORNAMENTO,
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


        public void SetIndennita(ref IndennitaModel im, EntitiesDBISE db)
        {
            INDENNITA i = new INDENNITA();


            i.IDTRASFERIMENTO = im.idTrasferimento;
            i.IDLIVDIPENDENTE = im.idLivDipendente;
            i.IDINDENNITABASE = im.idIndennitaBase;
            i.IDTFR = im.idTFR;
            i.IDPERCENTUALEDISAGIO = im.idPercentualeDisagio;
            i.IDCOEFFICIENTESEDE = im.idCoefficenteSede;
            if (im.idMaggiorazioneConiuge > 0)
            {
                i.IDMAGGIORAZIONECONIUGE = im.idMaggiorazioneConiuge;
            }
            if (im.idMaggiorazioneFigli > 0)
            {
                i.IDMAGGIORAZIONEFIGLI = im.idMaggiorazioneFigli;
            }            
            i.IDRUOLODIPENDENTE = im.idRuoloDipendente;
            i.DATAINIZIO = im.dataInizio;
            i.DATAFINE = im.dataFine.HasValue == true ? im.dataFine.Value : Convert.ToDateTime("31/12/9999");
            i.DATAAGGIORNAMENTO = im.dataAggiornamento;
            i.ANNULLATO = im.annullato;

            db.INDENNITA.Add(i);

            if (db.SaveChanges() > 0)
            {
                im.idIndennita = i.IDINDENNITA;
            }
            

        }



    }
}