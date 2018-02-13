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
        
        
        public List<RichiamoModel> GetLista_Richiamo(decimal idtrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                TRASFERIMENTO trasf = db.TRASFERIMENTO.Find(idtrasferimento);
                var tmp = (from e in trasf.RICHIAMO
                           
                           select new RichiamoModel()
                           {
                            //IDTRASFRICHIAMO = e.IDTRASFRICHIAMO,
                            //DATAOPERAZIONE = e.DATAOPERAZIONE
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
                DATARIENTRO = ric.DataRientro,
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


        public void EditTrasferimento(TrasferimentoModel trm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                TRASFERIMENTO tr = db.TRASFERIMENTO.Find(trm.idTrasferimento);

                if (tr != null && tr.IDTRASFERIMENTO > 0)
                {
                    tr.IDTIPOTRASFERIMENTO = trm.idTipoTrasferimento;
                    tr.IDUFFICIO = trm.idUfficio;
                    tr.IDSTATOTRASFERIMENTO = (decimal)trm.idStatoTrasferimento;
                    tr.IDDIPENDENTE = trm.idDipendente;
                    tr.IDTIPOCOAN = trm.idTipoCoan;
                    tr.DATAPARTENZA = trm.dataPartenza;
                    tr.DATARIENTRO = trm.dataRientro;
                    tr.COAN = trm.coan.ToUpper();
                    tr.PROTOCOLLOLETTERA = trm.protocolloLettera.ToUpper();
                    tr.DATALETTERA = trm.dataLettera;
                    tr.DATAAGGIORNAMENTO = trm.dataAggiornamento;

                    if (db.SaveChanges() > 0)
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica del trasferimento.", "Trasferimento", db, tr.IDTRASFERIMENTO, tr.IDTRASFERIMENTO);
                    }


                }
            }

        }

        public void EditTrasferimento(TrasferimentoModel trm, ModelDBISE db)
        {
            TRASFERIMENTO tr = db.TRASFERIMENTO.Find(trm.idTrasferimento);

            if (tr != null && tr.IDTRASFERIMENTO > 0)
            {
                tr.IDTIPOTRASFERIMENTO = trm.idTipoTrasferimento > 0 ? trm.idTipoTrasferimento : tr.IDTIPOTRASFERIMENTO;
                tr.IDUFFICIO = trm.idUfficio > 0 ? trm.idUfficio : tr.IDUFFICIO;
                tr.IDSTATOTRASFERIMENTO = Convert.ToDecimal(trm.idStatoTrasferimento) > 0 ? (decimal)trm.idStatoTrasferimento : tr.IDSTATOTRASFERIMENTO;
                tr.IDDIPENDENTE = trm.idDipendente > 0 ? trm.idDipendente : tr.IDDIPENDENTE;
                tr.IDTIPOCOAN = trm.idTipoCoan > 0 ? trm.idTipoCoan : tr.IDTIPOCOAN;
                tr.DATAPARTENZA = trm.dataPartenza > DateTime.MinValue ? trm.dataPartenza : tr.DATAPARTENZA;
                tr.DATARIENTRO = trm.dataRientro ?? tr.DATARIENTRO;
                tr.COAN = trm.coan ?? tr.COAN;
                tr.PROTOCOLLOLETTERA = trm.protocolloLettera ?? tr.PROTOCOLLOLETTERA;
                tr.DATALETTERA = trm.dataLettera ?? tr.DATALETTERA;
                tr.DATAAGGIORNAMENTO = trm.dataAggiornamento > DateTime.MinValue ? trm.dataAggiornamento : tr.DATAAGGIORNAMENTO;


                if (db.SaveChanges() > 0)
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica del trasferimento.", "Trasferimento", db, tr.IDTRASFERIMENTO, tr.IDTRASFERIMENTO);
                }

            }
        }



    }
}