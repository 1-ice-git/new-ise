using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtIndennitaServizio : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<IndennitaBaseModel> GetIndennitaServizio(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var ll = db.INDENNITA.Find(idTrasferimento).INDENNITABASE.Where(a => a.ANNULLATO == false).ToList();

                    //using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                    //{
                    //    RuoloDipendenteModel rdm = dtrd.GetRuoloDipendenteByIdIndennita(idTrasferimento);

                    //    //CoefficientiSedeModel - GetCoefficenteSedeByIdTrasf (dtCoefficenteSede)

                    //    //PercentualeDisagioModel - GetPercentualeDisagioByIdTrasf (dtPercentualeDisagio)

                    //    libm = (from e in ll
                    //            select new IndennitaBaseModel()
                    //            {
                    //                idIndennitaBase = e.IDINDENNITABASE,
                    //                idLivello = e.IDLIVELLO,
                    //                dataInizioValidita = e.DATAINIZIOVALIDITA,
                    //                dataFineValidita = e.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : e.DATAFINEVALIDITA,
                    //                valore = e.VALORE,
                    //                valoreResponsabile = e.VALORERESP,
                    //                dataAggiornamento = e.DATAAGGIORNAMENTO,
                    //                annullato = e.ANNULLATO,
                    //                Livello = new LivelloModel()
                    //                {
                    //                    idLivello = e.LIVELLI.IDLIVELLO,
                    //                    DescLivello = e.LIVELLI.LIVELLO
                    //                },
                    //                RuoloUfficio = new RuoloUfficioModel()
                    //                {
                    //                    idRuoloUfficio = rdm.RuoloUfficio.idRuoloUfficio,
                    //                    DescrizioneRuolo = rdm.RuoloUfficio.DescrizioneRuolo
                    //                }
                    //            }).ToList();
                    //}


                    using (dtCoefficenteSede dtcs = new dtCoefficenteSede())
                    {
                        CoefficientiSedeModel csm = dtcs.GetCoefficenteSedeByIdTrasferimento(idTrasferimento);
                        

                        //PercentualeDisagioModel - GetPercentualeDisagioByIdTrasf (dtPercentualeDisagio)

                        libm = (from e in ll
                                select new IndennitaBaseModel()
                                {
                                    idIndennitaBase = e.IDINDENNITABASE,
                                    idLivello = e.IDLIVELLO,
                                    dataInizioValidita = e.DATAINIZIOVALIDITA,
                                    dataFineValidita = e.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : e.DATAFINEVALIDITA,
                                    valore = e.VALORE,
                                    valoreResponsabile = e.VALORERESP,
                                    dataAggiornamento = e.DATAAGGIORNAMENTO,
                                    annullato = e.ANNULLATO,
                                    CoefficenteSede = new CoefficientiSedeModel
                                    {
                                        idCoefficientiSede = csm.idCoefficientiSede,
                                        idUfficio = csm.idUfficio

                                    }

                                }).ToList();
                    }


                    return libm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}