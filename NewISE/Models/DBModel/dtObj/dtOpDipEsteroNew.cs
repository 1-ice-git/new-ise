using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtOpDipEsteroNew : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<OpDipEsteroModel> GetOpDipEsteroNew(DateTime dtRif, decimal idUfficio, ModelDBISE db)
        {
            List<OpDipEsteroModel> rim = new List<OpDipEsteroModel>();
            List<TRASFERIMENTO> ltrasf = new List<TRASFERIMENTO>();
            
            if(idUfficio>0)
            {
                ltrasf = db.TRASFERIMENTO.Where(a => a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                         a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                                         a.DATAPARTENZA <= dtRif &&
                                                         a.DATARIENTRO >= dtRif &&
                                                         a.IDUFFICIO == idUfficio).ToList();
            }
            else
            {
                ltrasf = db.TRASFERIMENTO.Where(a => a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                         a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                                         a.DATAPARTENZA <= dtRif &&
                                                         a.DATARIENTRO >= dtRif)
                                            .ToList();

            }


            if (ltrasf?.Any() ?? false)
            {
                foreach (var t in ltrasf)
                {

                    #region Valuta Ufficio alla dataRif
                    var lv = t.UFFICI.VALUTAUFFICIO
                                        .Where(a => a.ANNULLATO == false &&
                                                    a.DATAINIZIOVALIDITA <= dtRif &&
                                                    a.DATAFINEVALIDITA >= dtRif)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                        .ToList();

                    if (!lv?.Any() ?? false)
                    {
                        throw new Exception("Errore: Valuta Ufficio non trovata");
                    }
                    var rv = lv.First();
                    var descValuta = rv.VALUTE.DESCRIZIONEVALUTA;
                    //var rufficio = ufficio.First();
                    #endregion

                    #region livello alla dataRif
                    var dip = t.DIPENDENTI;
                    var llivdip = t.DIPENDENTI.LIVELLIDIPENDENTI
                                            .Where(a =>
                                                        a.ANNULLATO == false &&
                                                        a.DATAINIZIOVALIDITA <= dtRif &&
                                                        a.DATAFINEVALIDITA >= dtRif)
                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                            .ToList();

                    var livdip = llivdip.First();
                    var descLivello = livdip.LIVELLI.LIVELLO;

                    #endregion

                    using (CalcoliIndennita ci = new CalcoliIndennita(t.IDTRASFERIMENTO, dtRif, db))
                    {


                        OpDipEsteroModel ldvm = new OpDipEsteroModel()
                        {
                            sede = t.UFFICI.DESCRIZIONEUFFICIO,
                            valuta = descValuta,
                            matricola = t.DIPENDENTI.MATRICOLA,
                            nominativo = t.DIPENDENTI.COGNOME + " " + t.DIPENDENTI.NOME + " (" + t.DIPENDENTI.MATRICOLA + ")",
                            data_trasferimento = Convert.ToDateTime(t.DATAPARTENZA).ToShortDateString(),
                            //qualifica = qualif.LIVELLI.LIVELLO,
                            qualifica = descLivello,
                            IndennitaPersonale = ci.IndennitaPersonale,
                            PercMaggConiuge = ci.PercentualeMaggiorazioneConiuge,
                            PercNumFigli = ci.PercentualeMaggiorazioneFigli,
                            MaggConiuge = ci.MaggiorazioneConiuge,
                            MaggFigli = ci.MaggiorazioneFigli
                        };
                        rim.Add(ldvm);
                    }
                }
            }
            
            return rim;

        }


    }
}