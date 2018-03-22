using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtTFR : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void AssociaTFR_Indennita(decimal idTrasferimento, decimal idTFR, ModelDBISE db)
        {
            try
            {
                var i = db.INDENNITA.Find(idTrasferimento);

                var item = db.Entry<INDENNITA>(i);

                item.State = System.Data.Entity.EntityState.Modified;

                item.Collection(a => a.TFR).Load();

                var l = db.TFR.Find(idTFR);

                i.TFR.Add(l);

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RimuoviAsscoiazioniTFR_Indennita(decimal idTrasferimento, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
            var ltfr =
                i.TFR.Where(a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= dtIni && a.DATAINIZIOVALIDITA <= dtFin)
                    .OrderBy(a => a.DATAINIZIOVALIDITA);
            if (ltfr?.Any() ?? false)
            {
                foreach (var tfr in ltfr)
                {
                    i.TFR.Remove(tfr);
                }

                db.SaveChanges();
            }
        }

        public void RimuoviAssociaTFR_Indennita(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            //var i = db.INDENNITA.Find(idTrasferimento);

            //var item = db.Entry<INDENNITA>(i);

            //item.State = System.Data.Entity.EntityState.Modified;

            //item.Collection(a => a.TFR).Load();

            //var n = i.TFR.ToList().RemoveAll(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA);

            //if (n > 0)
            //    db.SaveChanges();

            var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
            var lit = i.TFR.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).ToList();

            foreach (var item in lit)
            {
                i.TFR.Remove(item);
            }
            db.SaveChanges();


        }


        public IList<TFRModel> GetTfrIndennitaByRangeDate(decimal idUfficio, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            List<TFRModel> ltfrm = new List<TFRModel>();

            var u = db.UFFICI.Find(idUfficio);

            if (u.PAGATOVALUTAUFFICIO == false)
            {
                var lv = db.VALUTE.Where(a => a.VALUTAUFFICIALE == true);

                if (lv?.Any() ?? false)
                {
                    var v = lv.First();

                    var ltfr =
                        v.TFR.Where(
                            a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= dtIni && a.DATAINIZIOVALIDITA <= dtFin)
                            .OrderBy(a => a.DATAINIZIOVALIDITA);
                    if (ltfr?.Any() ?? false)
                    {
                        ltfrm = (from tfr in ltfr
                                 select new TFRModel()
                                 {
                                     idTFR = tfr.IDTFR,
                                     idValuta = tfr.IDVALUTA,
                                     dataInizioValidita = tfr.DATAINIZIOVALIDITA,
                                     dataFineValidita =
                                         tfr.DATAFINEVALIDITA == Utility.DataFineStop()
                                             ? new DateTime?()
                                             : tfr.DATAFINEVALIDITA,
                                     dataAggiornamento = tfr.DATAAGGIORNAMENTO,
                                     tassoCambio = tfr.TASSOCAMBIO,
                                     Annullato = tfr.ANNULLATO
                                 }).ToList();
                    }
                    else
                    {
                        throw new Exception("Non è presente il tasso di cambio per la valuta " + v.DESCRIZIONEVALUTA + " nel periodo: (" + dtIni + " - " + dtFin + ")");
                    }

                }
            }
            else
            {
                var lvu =
                    db.VALUTAUFFICIO.Where(
                        a =>
                            a.ANNULLATO == false && a.IDUFFICIO == u.IDUFFICIO && a.DATAFINEVALIDITA >= dtIni &&
                            a.DATAINIZIOVALIDITA <= dtFin)
                        .OrderBy(a => a.DATAINIZIOVALIDITA);

                if (lvu?.Any() ?? false)
                {
                    foreach (var vu in lvu)
                    {

                        DateTime dtInizio = Utility.GetData_Inizio_Base();
                        DateTime dtFine = Utility.DataFineStop();

                        if (dtIni > vu.DATAINIZIOVALIDITA)
                        {
                            dtInizio = dtIni;
                        }
                        else
                        {
                            dtInizio = vu.DATAINIZIOVALIDITA;
                        }

                        if (dtFin > vu.DATAFINEVALIDITA)
                        {
                            dtFine = vu.DATAFINEVALIDITA;
                        }
                        else
                        {
                            dtFine = dtFin;
                        }

                        var ltfr =
                            vu.VALUTE.TFR.Where(
                                a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= dtInizio && a.DATAINIZIOVALIDITA <= dtFine)
                                .OrderBy(a => a.DATAINIZIOVALIDITA);

                        var ltfrmTemp = (from tfr in ltfr
                                         select new TFRModel()
                                         {
                                             idTFR = tfr.IDTFR,
                                             idValuta = tfr.IDVALUTA,
                                             dataInizioValidita = tfr.DATAINIZIOVALIDITA,
                                             dataFineValidita =
                                                 tfr.DATAFINEVALIDITA == Utility.DataFineStop()
                                                     ? new DateTime?()
                                                     : tfr.DATAFINEVALIDITA,
                                             dataAggiornamento = tfr.DATAAGGIORNAMENTO,
                                             tassoCambio = tfr.TASSOCAMBIO,
                                             Annullato = tfr.ANNULLATO
                                         }).ToList();


                        ltfrm.AddRange(ltfrmTemp);
                    }
                }
                else
                {
                    throw new Exception("Non risulta assegnata la valuta per l'ufficio " + u.DESCRIZIONEUFFICIO + "(" + u.CODICEUFFICIO + ") nel periodo: (" + dtIni + " - " + dtFin + ")");
                }

            }

            return ltfrm;


        }


        public TFRModel GetTFRValido(decimal idUfficio, DateTime dt, ModelDBISE db)
        {
            TFRModel tfrm = new TFRModel();

            using (dtUffici dtu = new dtUffici())
            {
                UfficiModel ufm = dtu.GetUffici(idUfficio, db);
                ValuteModel vm = new ValuteModel();

                if (ufm.pagatoValutaUfficio == false)
                {
                    using (dtValute dtv = new dtValute())
                    {
                        vm = dtv.GetValutaUfficiale(db);

                        if (vm.HasValue())
                        {
                            var ltfr = db.TFR.Where(a => a.ANNULLATO == false &&
                                                    a.IDVALUTA == vm.idValuta &&
                                                    dt >= a.DATAINIZIOVALIDITA &&
                                                    dt <= a.DATAFINEVALIDITA)
                                             .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                             .ToList();

                            if (ltfr != null && ltfr.Count > 0)
                            {
                                var tfr = ltfr.First();

                                tfrm = new TFRModel()
                                {
                                    idTFR = tfr.IDTFR,
                                    idValuta = tfr.IDVALUTA,
                                    dataInizioValidita = tfr.DATAINIZIOVALIDITA,
                                    dataFineValidita = tfr.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : tfr.DATAFINEVALIDITA,
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

        public List<TFRModel> GetListaTfrByValuta_RangeDate(TrasferimentoModel trm,decimal idValuta, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            List<TFRModel> ltfrm = new List<TFRModel>();

            using (dtUffici dtu = new dtUffici())
            {
                var t = db.TRASFERIMENTO.Find(trm.idTrasferimento);
                UFFICI u = t.UFFICI;

                UfficiModel ufm = dtu.GetUffici(u.IDUFFICIO, db);

                if (ufm.pagatoValutaUfficio == false)
                {
                    using (dtValute dtv = new dtValute())
                    {
                        var ltfr = db.TFR.Where(a => a.ANNULLATO == false &&
                                                a.IDVALUTA == idValuta &&
                                                dtIni >= a.DATAINIZIOVALIDITA &&
                                                dtFin <= a.DATAFINEVALIDITA)
                                             .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                             .ToList();

                        if (ltfr != null && ltfr.Count > 0)
                        {
                            foreach(var tfr in ltfr)
                            {
                                TFRModel tfrm = new TFRModel()
                                {
                                    idTFR = tfr.IDTFR,
                                    idValuta = tfr.IDVALUTA,
                                    dataInizioValidita = tfr.DATAINIZIOVALIDITA,
                                    dataFineValidita = tfr.DATAFINEVALIDITA, //== Utility.DataFineStop() ? new DateTime?() : tfr.DATAFINEVALIDITA,
                                    dataAggiornamento = tfr.DATAAGGIORNAMENTO,
                                    tassoCambio = tfr.TASSOCAMBIO,
                                    Annullato = tfr.ANNULLATO
                                };
                                ltfrm.Add(tfrm);
                            }
                        }
                    }
                }
            }

            return ltfrm;
        }


    }
}