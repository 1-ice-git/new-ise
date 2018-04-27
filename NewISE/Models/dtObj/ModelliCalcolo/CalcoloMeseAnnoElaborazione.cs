using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;

namespace NewISE.Models.dtObj.ModelliCalcolo
{

    public class MeseAnnoElaborazioneModel
    {
        public decimal IdMeseAnnoElab { get; set; }
        public int Anno { get; set; }
        public int Mese { get; set; }
        public bool Elaborato { get; set; }

    }

    public class CalcoloMeseAnnoElaborazione : IDisposable
    {

        private List<MeseAnnoElaborazioneModel> _mae = new List<MeseAnnoElaborazioneModel>();

        private ModelDBISE _db;


        public IList<MeseAnnoElaborazioneModel> Mae => _mae;



        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }



        public CalcoloMeseAnnoElaborazione(ModelDBISE db)
        {

            //db.Database.BeginTransaction();

            try
            {
                _db = db;

                this.GestioneMeseElaborato();

                //db.Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                //db.Database.CurrentTransaction.Rollback();
                throw ex;
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        private void GestioneMeseElaborato()
        {
            //Svuoto l'attuale oggetto.
            _mae.Clear();

            //Prelevo tutte le righe dei 5 anni precedenti e ordinate in modo descrescente per ordine d'inserimento.
            var AllMae =
                _db.MESEANNOELABORAZIONE.Where(a => a.ANNO >= (DateTime.Now.Year - 5))
                    .OrderByDescending(a => a.IDMESEANNOELAB)
                    .ToList();

            //Se sono presenti nel DB già delle righe
            if (AllMae?.Any() ?? false)
            {

                foreach (var mae in AllMae)
                {
                    MeseAnnoElaborazioneModel maem = new MeseAnnoElaborazioneModel()
                    {
                        IdMeseAnnoElab = mae.IDMESEANNOELAB,
                        Anno = (int)mae.ANNO,
                        Mese = (int)mae.MESE,
                        Elaborato = mae.ELABORATO
                    };

                    _mae.Add(maem);
                }


            }
            //Se non ci sono ancora righe vuol dire che è la prima volta che si procede con l'elaborazione.
            else
            {
                int anno = DateTime.Now.Year;
                int mese = DateTime.Now.Month;

                MESEANNOELABORAZIONE mae = new MESEANNOELABORAZIONE()
                {
                    ANNO = anno,
                    MESE = mese,
                    ELABORATO = false
                };

                _db.MESEANNOELABORAZIONE.Add(mae);

                int i = _db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nell'inserimento del nuovo mese di elaborazione.");
                }

                MeseAnnoElaborazioneModel maem = new MeseAnnoElaborazioneModel()
                {
                    IdMeseAnnoElab = mae.IDMESEANNOELAB,
                    Anno = (int)mae.ANNO,
                    Mese = (int)mae.MESE,
                    Elaborato = mae.ELABORATO
                };

                _mae.Add(maem);
            }
        }
        /// <summary>
        /// Imposta l'ultimo movimento non elaborato in elaborato.
        /// </summary>
        /// <param name="db"></param>
        public void SetMeseElaborato()
        {
            var lastMae =
                _db.MESEANNOELABORAZIONE.Where(a => a.ELABORATO == false)
                    .OrderByDescending(a => a.IDMESEANNOELAB)
                    .ToList();

            if (lastMae?.Any() ?? false)
            {
                var mae = lastMae.First();

                mae.ELABORATO = true;

                int i = _db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Non è stato possibile impostare il mese in elaborazione in fase ELABORATO.");
                }
                else
                {
                    this.GestioneMeseElaborato();

                }
            }

        }

        public void NewMeseDaElaborare()
        {
            //Prelevo tutte le righe dei 5 anni precedenti e ordinate in modo descrescente per ordine d'inserimento.
            var AllMae =
                _db.MESEANNOELABORAZIONE.Where(a => a.ANNO >= (DateTime.Now.Year - 5))
                    .OrderByDescending(a => a.IDMESEANNOELAB)
                    .ToList();

            //Prelevo l'ultima riga inserita.
            var lastMae = AllMae.First();
            //Se l'ultima riga inserita risulta elaborata incremento di un mese, inserisco una nuova riga e carico tutte le righe sulla proprietà.
            if (lastMae.ELABORATO == true)
            {
                var mese = 0;
                var anno = 0;

                if (lastMae.MESE < 12)
                {
                    mese = (int)lastMae.MESE + 1;
                    anno = (int)lastMae.ANNO;
                }
                else
                {
                    mese = 1;
                    anno = (int)lastMae.ANNO + 1;
                }

                MESEANNOELABORAZIONE newMae = new MESEANNOELABORAZIONE()
                {
                    ANNO = anno,
                    MESE = mese,
                    ELABORATO = false
                };

                _db.MESEANNOELABORAZIONE.Add(newMae);

                int i = _db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nell'inserimento del nuovo mese di elaborazione.");
                }

                this.GestioneMeseElaborato();

            }
            //Altrimenti se l'ultima riga non risulta elaborata leggo tutte le righe.
            else
            {
                this.GestioneMeseElaborato();

            }


        }



    }
}