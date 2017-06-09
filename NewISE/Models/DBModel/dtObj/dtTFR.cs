using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtTFR : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public TFRModel GetTFRValido(decimal idUfficio, DateTime dt, EntitiesDBISE db)
        {
            TFRModel tfrm = new TFRModel();

            using (dtUffici dtu=new dtUffici())
            {
                UfficiModel ufm = dtu.GetUffici(idUfficio, db);
                ValuteModel vm = new ValuteModel();

                if (ufm.pagatoValutaUfficio == false)
                {
                    using (dtValute dtv=new dtValute())
                    {
                        vm = dtv.GetValutaUfficiale(db);

                        if (vm.HasValue())
                        {
                            var ltfr = db.TFR.Where(a=>a.ANNULLATO == false && 
                                                    a.IDVALUTA == vm.idValuta && 
                                                    dt >= a.DATAINIZIOVALIDITA && 
                                                    dt <= a.DATAFINEVALIDITA)
                                             .OrderByDescending(a=>a.DATAINIZIOVALIDITA)
                                             .ToList();

                            if (ltfr != null && ltfr.Count > 0)
                            {
                                var tfr = ltfr.First();

                                tfrm = new TFRModel()
                                {
                                    idTFR = tfr.IDTFR,
                                    idValuta = tfr.IDVALUTA,
                                    dataInizioValidita = tfr.DATAINIZIOVALIDITA,
                                    dataFineValidita = tfr.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : tfr.DATAFINEVALIDITA,
                                    dataAggiornamento = tfr.DATAAGGIORNAMENTO,
                                    tassoCambio = tfr.TASSOCAMBIO,
                                    Annullato = tfr.ANNULLATO                                    
                            };
                            }

                            
                        }
                        else
                        {
                            throw new Exception("La valuta ufficiale non risulta registrata, provvedere prima di procedere.");
                        }
                    }
                }

            }

            return tfrm;
        }




    }
}