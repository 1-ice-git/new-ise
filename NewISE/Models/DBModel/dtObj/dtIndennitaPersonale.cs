using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtIndennitaPersonale : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<IndennitaBaseModel> GetIndennitaPersonale(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var ll = db.INDENNITA.Find(idTrasferimento).INDENNITABASE.ToList();
                    //var ll = db.INDENNITABASE.ToList();

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
                                            dipInfoTrasferimento = new dipInfoTrasferimentoModel
                                            {
                                                Decorrenza = dipInfoTrasf.Decorrenza,
                                                indennitaPersonale = dipInfoTrasf.indennitaPersonale,
                                                indennitaServizio = dipInfoTrasf.indennitaServizio,
                                                maggiorazioniFamiliari = dipInfoTrasf.maggiorazioniFamiliari
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