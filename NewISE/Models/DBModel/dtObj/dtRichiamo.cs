using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using NewISE.Models.ViewModel;
using System.Data.Entity;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRichiamo : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }



        public RichiamoModel GetRichiamoByIdTrasf(decimal idTrasferimento)
        {
            RichiamoModel rm = new RichiamoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                TRASFERIMENTO t = db.TRASFERIMENTO.Find(idTrasferimento);
                var lr = t.RICHIAMO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDRICHIAMO);

                if (lr?.Any() ?? false)
                {
                    var r = lr.First();

                    rm = new RichiamoModel()
                    {
                        IdRichiamo = r.IDRICHIAMO,
                        idTrasferimento = r.IDTRASFERIMENTO,
                        DataRichiamo = r.DATARICHIAMO,
                        DataAggiornamento = r.DATAAGGIORNAMENTO,
                        annullato = r.ANNULLATO
                    };
                }
            }

            return rm;
        }

        public List<RichiamoModel> GetLista_Richiamo(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                TRASFERIMENTO trasf = db.TRASFERIMENTO.Find(idTrasferimento);

                var lr = trasf.RICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDRICHIAMO);


                var tmp = (from e in lr

                           select new RichiamoModel()
                           {
                               IdRichiamo = e.IDRICHIAMO,
                               idTrasferimento = e.IDTRASFERIMENTO,
                               DataRichiamo = e.DATARICHIAMO,
                               DataAggiornamento = e.DATAAGGIORNAMENTO,
                               annullato = e.ANNULLATO
                           }).ToList();
                return tmp;
            }
        }

        public RichiamoModel getRichiamoById(decimal idTrasfRichiamo)
        {
            RichiamoModel tmp = new RichiamoModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                //tmp = (from e in db.RICHIAMO
                //       where e.IDTRASFRICHIAMO == idTrasfRichiamo
                //       select new RichiamoModel()
                //       {   
                //           IDTRASFRICHIAMO = e.IDTRASFRICHIAMO,
                //           DATAOPERAZIONE = e.DATAOPERAZIONE

                //       }).ToList().FirstOrDefault();
            }
            return tmp;
        }



        public void SetRichiamo(ref RichiamoModel ric, ModelDBISE db)
        {
            RICHIAMO ri;
            ri = new RICHIAMO()
            {
                IDRICHIAMO = ric.IdRichiamo,
                IDTRASFERIMENTO = ric.idTrasferimento,
                DATARICHIAMO = ric.DataRichiamo,
                DATAAGGIORNAMENTO = DateTime.Now,
                ANNULLATO = ric.annullato
            };

            db.RICHIAMO.Add(ri);

            int i = db.SaveChanges();

            if (i > 0)
            {
                ric.idTrasferimento = ri.IDTRASFERIMENTO;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo richiamo.", "Richiamo", db, ric.idTrasferimento, ri.IDTRASFERIMENTO);
            }

        }


        ////public void EditTrasferimento(TrasferimentoModel trm)
        ////{
        ////    using (ModelDBISE db = new ModelDBISE())
        ////    {
        ////        TRASFERIMENTO tr = db.TRASFERIMENTO.Find(trm.idTrasferimento);

        ////        if (tr != null && tr.IDTRASFERIMENTO > 0)
        ////        {
        ////            tr.IDTIPOTRASFERIMENTO = trm.idTipoTrasferimento;
        ////            tr.IDUFFICIO = trm.idUfficio;
        ////            tr.IDSTATOTRASFERIMENTO = (decimal)trm.idStatoTrasferimento;
        ////            tr.IDDIPENDENTE = trm.idDipendente;
        ////            tr.IDTIPOCOAN = trm.idTipoCoan;
        ////            tr.DATAPARTENZA = trm.dataPartenza;
        ////            tr.DATARIENTRO = trm.dataRientro;
        ////            tr.COAN = trm.coan.ToUpper();
        ////            tr.PROTOCOLLOLETTERA = trm.protocolloLettera.ToUpper();
        ////            tr.DATALETTERA = trm.dataLettera;
        ////            tr.DATAAGGIORNAMENTO = trm.dataAggiornamento;

        ////            if (db.SaveChanges() > 0)
        ////            {
        ////                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica del trasferimento.", "Trasferimento", db, tr.IDTRASFERIMENTO, tr.IDTRASFERIMENTO);
        ////            }


        ////        }
        ////    }

        ////}

        //public void EditTrasferimento(TrasferimentoModel trm, ModelDBISE db)
        //{
        //    TRASFERIMENTO tr = db.TRASFERIMENTO.Find(trm.idTrasferimento);

        //    if (tr != null && tr.IDTRASFERIMENTO > 0)
        //    {
        //        tr.IDTIPOTRASFERIMENTO = trm.idTipoTrasferimento > 0 ? trm.idTipoTrasferimento : tr.IDTIPOTRASFERIMENTO;
        //        tr.IDUFFICIO = trm.idUfficio > 0 ? trm.idUfficio : tr.IDUFFICIO;
        //        tr.IDSTATOTRASFERIMENTO = Convert.ToDecimal(trm.idStatoTrasferimento) > 0 ? (decimal)trm.idStatoTrasferimento : tr.IDSTATOTRASFERIMENTO;
        //        tr.IDDIPENDENTE = trm.idDipendente > 0 ? trm.idDipendente : tr.IDDIPENDENTE;
        //        tr.IDTIPOCOAN = trm.idTipoCoan > 0 ? trm.idTipoCoan : tr.IDTIPOCOAN;
        //        tr.DATAPARTENZA = trm.dataPartenza > DateTime.MinValue ? trm.dataPartenza : tr.DATAPARTENZA;
        //        tr.DATARIENTRO = trm.dataRientro ?? tr.DATARIENTRO;
        //        tr.COAN = trm.coan ?? tr.COAN;
        //        tr.PROTOCOLLOLETTERA = trm.protocolloLettera ?? tr.PROTOCOLLOLETTERA;
        //        tr.DATALETTERA = trm.dataLettera ?? tr.DATALETTERA;
        //        tr.DATAAGGIORNAMENTO = trm.dataAggiornamento > DateTime.MinValue ? trm.dataAggiornamento : tr.DATAAGGIORNAMENTO;


        //        if (db.SaveChanges() > 0)
        //        {
        //            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica del trasferimento.", "Trasferimento", db, tr.IDTRASFERIMENTO, tr.IDTRASFERIMENTO);
        //        }

        //    }
        //}



    }
}