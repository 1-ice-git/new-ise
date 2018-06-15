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

                    //var ll = db.INDENNITA.Find(idTrasferimento).INDENNITABASE.Where(a => a.ANNULLATO == false).ToList();

                    var ll = db.INDENNITA.Find(idTrasferimento).INDENNITABASE.ToList();
                    //var ll = db.INDENNITABASE.ToList();

                    using (dtCoefficenteSede dtcs = new dtCoefficenteSede())
                    {
                        CoefficientiSedeModel csm = dtcs.GetCoefficenteSedeByIdTrasferimento(idTrasferimento);

                        using (dtPercentualeDisagio dtpd = new dtPercentualeDisagio())
                        {
                            PercentualeDisagioModel pdm = dtpd.GetPercentualeDisagioByIdTrasferimento(idTrasferimento);

                            using (dtTrasferimento dttrasf = new dtTrasferimento())
                            {
                                dipInfoTrasferimentoModel dipInfoTrasf = dttrasf.GetInfoTrasferimento(idTrasferimento);

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

                                            },
                                            PercentualeDisagio = new PercentualeDisagioModel
                                            {
                                                idPercentualeDisagio = pdm.idPercentualeDisagio,
                                                idUfficio = pdm.idUfficio,
                                                dataInizioValidita = pdm.dataInizioValidita,
                                                dataFineValidita = pdm.dataFineValidita,
                                                dataAggiornamento = pdm.dataAggiornamento,
                                                annullato = pdm.annullato

                                            },

                                            dipInfoTrasferimento = new dipInfoTrasferimentoModel
                                            {
                                                Decorrenza = dipInfoTrasf.Decorrenza,
                                                indennitaServizio = dipInfoTrasf.indennitaServizio
                                                
                                            }
                                        }).ToList();
                            }
                        }
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