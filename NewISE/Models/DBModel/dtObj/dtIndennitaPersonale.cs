using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;
using NewISE.Models.dtObj.ModelliCalcolo;

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
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;

                    //var ll = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                    //    .Where(a => a.ANNULLATO == false)
                    //    .OrderBy(a => a.IDLIVELLO)
                    //    .ThenBy(a => a.DATAINIZIOVALIDITA)
                    //    .ThenBy(a => a.DATAFINEVALIDITA).ToList();

                    var ll = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                        .Where(a => a.ANNULLATO == false).ToList();
                       

                    using (dtTrasferimento dttrasf = new dtTrasferimento())
                   {

                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO))
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
                                            },
                                            EvoluzioneIndennita = new EvoluzioneIndennitaModel
                                            {
                                                IndennitaServizio = ci.IndennitaDiServizio,
                                                MaggiorazioniFamiliari = ci.MaggiorazioniFamiliari,
                                                IndennitaPersonale = ci.IndennitaPersonale

                                            }
                                        }).ToList();
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