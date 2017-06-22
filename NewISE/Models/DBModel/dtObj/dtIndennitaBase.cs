using NewISE.POCO;
using System;
using System.Collections.Generic;
using System.Linq;

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
                        dataFineValidita = ib.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : ib.DATAFINEVALIDITA,
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
                            dataFineValidita = ib.RIDUZIONI.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : ib.RIDUZIONI.DATAFINEVALIDITA,
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
                    dataFineValidita = ib.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : ib.DATAFINEVALIDITA,
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
                        dataFineValidita = ib.RIDUZIONI.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : ib.RIDUZIONI.DATAFINEVALIDITA,
                        percentuale = ib.RIDUZIONI.PERCENTUALE,
                        dataAggiornamento = ib.RIDUZIONI.DATAAGGIORNAMENTO,
                        annullato = ib.ANNULLATO
                    }
                };
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
                        dataFineValidita = ib.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : ib.DATAFINEVALIDITA,
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
                            dataFineValidita = ib.RIDUZIONI.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : ib.RIDUZIONI.DATAFINEVALIDITA,
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
                    dataFineValidita = ib.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : ib.DATAFINEVALIDITA,
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
                        dataFineValidita = ib.RIDUZIONI.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : ib.RIDUZIONI.DATAFINEVALIDITA,
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
                    dataFineValidita = ib.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? ib.DATAFINEVALIDITA : new DateTime?(),
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
                        dataFineValidita = ib.RIDUZIONI.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? ib.RIDUZIONI.DATAFINEVALIDITA : new DateTime?(),
                        percentuale = ib.RIDUZIONI.PERCENTUALE,
                        dataAggiornamento = ib.RIDUZIONI.DATAAGGIORNAMENTO,
                        annullato = ib.RIDUZIONI.ANNULLATO
                    }
                };
            }

            return ibm;
        }
    }
}