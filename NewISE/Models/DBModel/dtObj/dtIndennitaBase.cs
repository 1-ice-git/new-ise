
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtIndennitaBase : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void AssociaIndennitaBase_Indennita(decimal idTrasferimento, decimal idIndennitaBase, ModelDBISE db)
        {

            try
            {
                var i = db.INDENNITA.Find(idTrasferimento);

                var item = db.Entry<INDENNITA>(i);

                item.State = System.Data.Entity.EntityState.Modified;

                item.Collection(a => a.INDENNITABASE).Load();

                var l = db.INDENNITABASE.Find(idIndennitaBase);

                i.INDENNITABASE.Add(l);

                db.SaveChanges();


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void RimuoviAssociazioneIndennitaBase_Indennita(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            //var i = db.INDENNITA.Find(idTrasferimento);

            //var item = db.Entry<INDENNITA>(i);

            //item.State = System.Data.Entity.EntityState.Modified;

            //item.Collection(a => a.INDENNITABASE).Load();

            //var n = i.INDENNITABASE.ToList().RemoveAll(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA);

            //if (n > 0)
            //{
            //    db.SaveChanges();
            //}

            var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
            var lit = i.INDENNITABASE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).ToList();

            foreach (var item in lit)
            {
                i.INDENNITABASE.Remove(item);
            }
            db.SaveChanges();


        }

        public IndennitaBaseModel GetIndennitaBaseByIdTrasf(decimal idTrasferimento, DateTime dt)
        {
            IndennitaBaseModel ibm = new IndennitaBaseModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                var lib = db.INDENNITA.Find(idTrasferimento).INDENNITABASE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
                if (lib != null && lib.Count > 0)
                {
                    var ib = lib.First();

                    ibm = new IndennitaBaseModel()
                    {
                        idIndennitaBase = ib.IDINDENNITABASE,
                        idLivello = ib.IDLIVELLO,
                        idRiduzioni = ib.IDRIDUZIONI,
                        dataInizioValidita = ib.DATAINIZIOVALIDITA,
                        dataFineValidita = ib.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ib.DATAFINEVALIDITA,
                        valore = ib.VALORE,
                        valoreResponsabile = ib.VALORERESP,
                        dataAggiornamento = ib.DATAAGGIORNAMENTO,
                        annullato = ib.ANNULLATO,
                        Livello = new LivelloModel()
                        {
                            idLivello = ib.LIVELLI.IDLIVELLO,
                            DescLivello = ib.LIVELLI.LIVELLO
                        },
                        Riduzioni = new RiduzioniModel()
                        {
                            idRiduzioni = ib.RIDUZIONI.IDRIDUZIONI,
                            idRegola = ib.RIDUZIONI.IDREGOLA,
                            dataInizioValidita = ib.RIDUZIONI.DATAINIZIOVALIDITA,
                            dataFineValidita = ib.RIDUZIONI.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ib.RIDUZIONI.DATAFINEVALIDITA,
                            percentuale = ib.RIDUZIONI.PERCENTUALE,
                            dataAggiornamento = ib.RIDUZIONI.DATAAGGIORNAMENTO,
                            annullato = ib.ANNULLATO
                        }
                    };
                }
            }

            return ibm;
        }

        public IndennitaBaseModel GetIndennitaBaseByIdTrasf(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            IndennitaBaseModel ibm = new IndennitaBaseModel();

            var lib =
                db.INDENNITA.Find(idTrasferimento)
                    .INDENNITABASE.Where(
                        a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                    .ToList();

            if (lib != null && lib.Count > 0)
            {
                var ib = lib.First();

                ibm = new IndennitaBaseModel()
                {
                    idIndennitaBase = ib.IDINDENNITABASE,
                    idLivello = ib.IDLIVELLO,
                    idRiduzioni = ib.IDRIDUZIONI,
                    dataInizioValidita = ib.DATAINIZIOVALIDITA,
                    dataFineValidita = ib.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ib.DATAFINEVALIDITA,
                    valore = ib.VALORE,
                    valoreResponsabile = ib.VALORERESP,
                    dataAggiornamento = ib.DATAAGGIORNAMENTO,
                    annullato = ib.ANNULLATO,
                    Livello = new LivelloModel()
                    {
                        idLivello = ib.LIVELLI.IDLIVELLO,
                        DescLivello = ib.LIVELLI.LIVELLO
                    },
                    //Riduzioni = new RiduzioniModel()
                    //{
                    //    idRiduzioni = ib.RIDUZIONI.IDRIDUZIONI,
                    //    idRegola = ib.RIDUZIONI.IDREGOLA,
                    //    dataInizioValidita = ib.RIDUZIONI.DATAINIZIOVALIDITA,
                    //    dataFineValidita = ib.RIDUZIONI.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ib.RIDUZIONI.DATAFINEVALIDITA,
                    //    percentuale = ib.RIDUZIONI.PERCENTUALE,
                    //    dataAggiornamento = ib.RIDUZIONI.DATAAGGIORNAMENTO,
                    //    annullato = ib.ANNULLATO
                    //}
                };

                var r = ib.RIDUZIONI;
                if (r?.IDRIDUZIONI > 0)
                {
                    ibm.Riduzioni = new RiduzioniModel()
                    {
                        idRiduzioni = ib.RIDUZIONI.IDRIDUZIONI,
                        idRegola = ib.RIDUZIONI.IDREGOLA,
                        dataInizioValidita = ib.RIDUZIONI.DATAINIZIOVALIDITA,
                        dataFineValidita =
                            ib.RIDUZIONI.DATAFINEVALIDITA == Utility.DataFineStop()
                                ? new DateTime?()
                                : ib.RIDUZIONI.DATAFINEVALIDITA,
                        percentuale = ib.RIDUZIONI.PERCENTUALE,
                        dataAggiornamento = ib.RIDUZIONI.DATAAGGIORNAMENTO,
                        annullato = ib.ANNULLATO
                    };
                }
            }

            return ibm;
        }

        public IndennitaBaseModel GetIndennitaBase(decimal idIndennitaBase)
        {
            IndennitaBaseModel ibm = new IndennitaBaseModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ib = db.INDENNITABASE.Find(idIndennitaBase);

                if (ib != null && ib.IDINDENNITABASE > 0)
                {
                    ibm = new IndennitaBaseModel()
                    {
                        idIndennitaBase = ib.IDINDENNITABASE,
                        idLivello = ib.IDLIVELLO,
                        idRiduzioni = ib.IDRIDUZIONI,
                        dataInizioValidita = ib.DATAINIZIOVALIDITA,
                        dataFineValidita = ib.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ib.DATAFINEVALIDITA,
                        valore = ib.VALORE,
                        valoreResponsabile = ib.VALORERESP,
                        dataAggiornamento = ib.DATAAGGIORNAMENTO,
                        annullato = ib.ANNULLATO,
                        Livello = new LivelloModel()
                        {
                            idLivello = ib.LIVELLI.IDLIVELLO,
                            DescLivello = ib.LIVELLI.LIVELLO
                        },
                        Riduzioni = new RiduzioniModel()
                        {
                            idRiduzioni = ib.RIDUZIONI.IDRIDUZIONI,
                            idRegola = ib.RIDUZIONI.IDREGOLA,
                            dataInizioValidita = ib.RIDUZIONI.DATAINIZIOVALIDITA,
                            dataFineValidita = ib.RIDUZIONI.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ib.RIDUZIONI.DATAFINEVALIDITA,
                            percentuale = ib.RIDUZIONI.PERCENTUALE,
                            dataAggiornamento = ib.RIDUZIONI.DATAAGGIORNAMENTO,
                            annullato = ib.ANNULLATO
                        }
                    };
                }
            }

            return ibm;
        }

        public IndennitaBaseModel GetIndennitaBase(decimal idIndennitaBase, ModelDBISE db)
        {
            IndennitaBaseModel ibm = new IndennitaBaseModel();

            var ib = db.INDENNITABASE.Find(idIndennitaBase);

            if (ib != null && ib.IDINDENNITABASE > 0)
            {
                ibm = new IndennitaBaseModel()
                {
                    idIndennitaBase = ib.IDINDENNITABASE,
                    idLivello = ib.IDLIVELLO,
                    idRiduzioni = ib.IDRIDUZIONI,
                    dataInizioValidita = ib.DATAINIZIOVALIDITA,
                    dataFineValidita = ib.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ib.DATAFINEVALIDITA,
                    valore = ib.VALORE,
                    valoreResponsabile = ib.VALORERESP,
                    dataAggiornamento = ib.DATAAGGIORNAMENTO,
                    annullato = ib.ANNULLATO,
                    Livello = new LivelloModel()
                    {
                        idLivello = ib.LIVELLI.IDLIVELLO,
                        DescLivello = ib.LIVELLI.LIVELLO
                    },
                    Riduzioni = new RiduzioniModel()
                    {
                        idRiduzioni = ib.RIDUZIONI.IDRIDUZIONI,
                        idRegola = ib.RIDUZIONI.IDREGOLA,
                        dataInizioValidita = ib.RIDUZIONI.DATAINIZIOVALIDITA,
                        dataFineValidita = ib.RIDUZIONI.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ib.RIDUZIONI.DATAFINEVALIDITA,
                        percentuale = ib.RIDUZIONI.PERCENTUALE,
                        dataAggiornamento = ib.RIDUZIONI.DATAAGGIORNAMENTO,
                        annullato = ib.ANNULLATO
                    }
                };
            }

            return ibm;
        }

        public IndennitaBaseModel GetIndennitaBaseValida(decimal idLivello, DateTime dt, ModelDBISE db)
        {
            IndennitaBaseModel ibm = new IndennitaBaseModel();

            List<INDENNITABASE> lib = db.INDENNITABASE.Where(a => a.ANNULLATO == false &&
                                                      a.IDLIVELLO == idLivello &&
                                                      dt >= a.DATAINIZIOVALIDITA &&
                                                      dt <= a.DATAFINEVALIDITA)
                                               .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

            if (lib != null && lib.Count > 0)
            {
                var ib = lib.First();

                ibm = new IndennitaBaseModel()
                {
                    idIndennitaBase = ib.IDINDENNITABASE,
                    idLivello = ib.IDLIVELLO,
                    idRiduzioni = ib.IDRIDUZIONI,
                    dataInizioValidita = ib.DATAINIZIOVALIDITA,
                    dataFineValidita = ib.DATAFINEVALIDITA != Utility.DataFineStop() ? ib.DATAFINEVALIDITA : new DateTime?(),
                    valore = ib.VALORE,
                    valoreResponsabile = ib.VALORERESP,
                    dataAggiornamento = ib.DATAAGGIORNAMENTO,
                    annullato = ib.ANNULLATO,
                    Livello = new LivelloModel()
                    {
                        idLivello = ib.LIVELLI.IDLIVELLO,
                        DescLivello = ib.LIVELLI.LIVELLO
                    }
                };

                var r = ib.RIDUZIONI;
                if (r?.IDRIDUZIONI > 0)
                {
                    ibm.Riduzioni = new RiduzioniModel()
                    {
                        idRiduzioni = r.IDRIDUZIONI,
                        idRegola = r.IDREGOLA,
                        dataInizioValidita = r.DATAINIZIOVALIDITA,
                        dataFineValidita = r.DATAFINEVALIDITA != Utility.DataFineStop() ? ib.RIDUZIONI.DATAFINEVALIDITA : new DateTime?(),
                        percentuale = r.PERCENTUALE,
                        dataAggiornamento = r.DATAAGGIORNAMENTO,
                        annullato = r.ANNULLATO
                    };
                }

            }

            return ibm;
        }
    }
}