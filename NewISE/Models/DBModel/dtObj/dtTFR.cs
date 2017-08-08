using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Linq;

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
    }
}