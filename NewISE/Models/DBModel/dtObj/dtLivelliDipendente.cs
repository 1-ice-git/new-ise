using NewISE.Models.DBModel;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj
{
    public class dtLivelliDipendente :IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public LivelloDipendenteModel GetLivelloDipendenteByIdTrasf(decimal idTrasferimento, DateTime dt, EntitiesDBISE db)
        {
            LivelloDipendenteModel ldm = new LivelloDipendenteModel();

            var lld = db.INDENNITA.Find(idTrasferimento)
                        .LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false && 
                                                 dt >= a.DATAINIZIOVALIDITA && 
                                                 dt <= a.DATAFINEVALIDITA)
                                          .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                          .ToList();
            if (lld != null && lld.Count > 0)
            {
                var ld = lld.First();

                ldm = new LivelloDipendenteModel()
                {
                    idLivDipendente = ld.IDLIVDIPENDENTE,
                    idDipendente = ld.IDDIPENDENTE,
                    idLivello = ld.IDLIVELLO,
                    dataInizioValdita = ld.DATAINIZIOVALIDITA,
                    dataFineValidita = ld.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ld.DATAFINEVALIDITA,
                    dataAggiornamento = ld.DATAAGGIORNAMENTO,
                    annullato = ld.ANNULLATO
                    
                };
            }
               

        return ldm;
        }

        public LivelloDipendenteModel GetLivelloDipendente(decimal idDipendente, DateTime data)
        {
            LivelloDipendenteModel ldm = new LivelloDipendenteModel();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                var ld = db.LIVELLIDIPENDENTI.Where(a => a.IDDIPENDENTE == idDipendente && data >= a.DATAINIZIOVALIDITA && data.Date <= a.DATAFINEVALIDITA).ToList();

                ldm = (from e in ld
                       select new LivelloDipendenteModel()
                       {
                           idLivDipendente = e.IDLIVDIPENDENTE,
                           idDipendente = e.IDDIPENDENTE,
                           idLivello = e.IDLIVELLO,
                           dataInizioValdita = e.DATAINIZIOVALIDITA,
                           dataFineValidita = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                           annullato = e.ANNULLATO,
                           Livello = new LivelloModel()
                           {
                               idLivello = e.LIVELLI.IDLIVELLO,
                               DescLivello = e.LIVELLI.LIVELLO
                           }
                       }).First();
            }

            return ldm;
        }

        public LivelloDipendenteModel GetLivelloDipendente(decimal idDipendente, DateTime data, EntitiesDBISE db)
        {
            LivelloDipendenteModel ldm = new LivelloDipendenteModel();

            
                var ld = db.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false && 
                                                    a.IDDIPENDENTE == idDipendente && 
                                                    data >= a.DATAINIZIOVALIDITA && 
                                                    data.Date <= a.DATAFINEVALIDITA)
                                             .OrderByDescending(a=>a.DATAINIZIOVALIDITA)
                                             .ToList();

                ldm = (from e in ld
                       select new LivelloDipendenteModel()
                       {
                           idLivDipendente = e.IDLIVDIPENDENTE,
                           idDipendente = e.IDDIPENDENTE,
                           idLivello = e.IDLIVELLO,
                           dataInizioValdita = e.DATAINIZIOVALIDITA,
                           dataFineValidita = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                           annullato = e.ANNULLATO,
                           Livello = new LivelloModel()
                           {
                               idLivello = e.LIVELLI.IDLIVELLO,
                               DescLivello = e.LIVELLI.LIVELLO
                           }
                       }).First();
            

            return ldm;
        }


        public void AssociaLivelloDipendente_Indennita(decimal idTrasferimento, decimal idLivelloDipendente, EntitiesDBISE db)
        {

            try
            {                
                var i = db.INDENNITA.Find(idTrasferimento);

                var item = db.Entry<INDENNITA>(i);

                item.State = System.Data.Entity.EntityState.Modified;

                item.Collection(a => a.LIVELLIDIPENDENTI).Load();

                //i.LIVELLIDIPENDENTI.Clear();

                var l = db.LIVELLIDIPENDENTI.Find(idLivelloDipendente);

                i.LIVELLIDIPENDENTI.Add(l);

                db.SaveChanges();

                
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
    }
}