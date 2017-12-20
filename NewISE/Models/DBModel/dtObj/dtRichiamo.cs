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
        
        public bool Delete_Richiamo(decimal idTrasfRichiamo, bool permesso = true)
        {
            bool esito = false;
            if (permesso)
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ca = db.RICHIAMO.Find(idTrasfRichiamo);
                    db.RICHIAMO.Remove(ca);
                    int i = db.SaveChanges();
                    if (i <= 0)
                    {
                        esito = false;
                        throw new Exception("Errore nella fase della cancellazione del richiamo");
                    }
                    else
                    {
                        esito = true;
                        //Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Cancellazione Richiamo",
                        //    "RICHIAMO", db, ca.IDTRASFRICHIAMO);
                    }
                }
            }
            else
            {
                throw new Exception("Eliminazione Richiamo non autorizzata");
            }
            return esito;
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


        public string[] InserisciRichiamo(RichiamoModel sosp, decimal idTrasfRichiamo)
        {
            string[] my_array = new string[] { "0", "0" };
           
               
                    using (var db = new ModelDBISE())
                    {
                        try
                        {
                            db.Database.BeginTransaction();
                            var sospnew = new RICHIAMO
                            {
                                //IDTRASFERIMENTO = sosp.idTrasferimento,//tmp.FirstOrDefault().idTrasferimento,
                                //DATAINIZIO = sosp.DataInizioSospensione.Value,
                                //DATAFINE = sosp.DataFineSospensione.Value,
                                //IDTIPOSOSPENSIONE = idTipoSospensione,// sosp.idTipoSospensione,
                                //DATAAGGIORNAMENTO = DateTime.Now

                                //IDTRASFRICHIAMO = sosp.IDTRASFRICHIAMO,
                                //DATAOPERAZIONE = sosp.DATAOPERAZIONE
                            };
                            db.RICHIAMO.Add(sospnew);

                            if (db.SaveChanges() > 0)
                            {
                                //sosp.IDTRASFRICHIAMO = sospnew.IDTRASFRICHIAMO;
                                //Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento RICHIAMO avvenuta con successo", "RICHIAMO", db, sospnew.IDTRASFRICHIAMO, sospnew.IDTRASFRICHIAMO);
                                db.Database.CurrentTransaction.Commit();
                            }
                            else
                            {
                                throw new Exception("L'inserimento del RICHIAMO non è avvenuto.");
                            }
                        }
                        catch (Exception ex)
                        {
                            db.Database.CurrentTransaction.Rollback();
                            throw ex;
                        }
                    }
                
               
            return my_array;
        }

        public string[] Modifica_Richiamo(RichiamoModel sospmod)
        {
            string[] my_array = new string[] { "0", "0" };
            try
            {
              
                    
                        using (ModelDBISE db = new ModelDBISE())
                        {
                            RICHIAMO sosp = db.RICHIAMO.Find(sospmod.IDTRASFRICHIAMO);
                            //sosp.DATAOPERAZIONE = sospmod.DATAOPERAZIONE;
                            
                            int i = db.SaveChanges();

                            if (i < 0)
                            {
                                throw new Exception(string.Format("Errore nella fase di modifica del Richiamo."));
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica Richiamo avvenuta con successo",
                                  "RICHIAMO", db, sospmod.idTrasferimento, sospmod.IDTRASFRICHIAMO);
                            }
                        }
                    
                    
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return my_array;
        }

    }
}