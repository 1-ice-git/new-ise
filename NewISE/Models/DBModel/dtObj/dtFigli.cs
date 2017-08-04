using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.ViewModel;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtFigli : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public static ValidationResult VerificaCodiceFiscale2(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var fm = context.ObjectInstance as Figli_V_Model;

            if (fm != null)
            {

                if (fm.codiceFiscale != null && fm.codiceFiscale != string.Empty)
                {
                    if (Utility.CheckCodiceFiscale(fm.codiceFiscale))
                    {
                        vr = ValidationResult.Success;
                    }
                    else
                    {
                        vr = new ValidationResult("Il Codice Fiscale non è corretto.");
                    }
                }
                else
                {
                    vr = new ValidationResult("Il Codice Fiscale è richiesto e deve essere composto da 16 caratteri.");
                }
            }
            else
            {
                vr = new ValidationResult("Il Codice Fiscale è richiesto e deve essere composto da 16 caratteri.");
            }

            return vr;
        }

        public static ValidationResult VerificaCodiceFiscale(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var fm = context.ObjectInstance as FigliModel;

            if (fm != null)
            {

                if (fm.codiceFiscale != null && fm.codiceFiscale != string.Empty)
                {
                    if (Utility.CheckCodiceFiscale(fm.codiceFiscale))
                    {
                        vr = ValidationResult.Success;
                    }
                    else
                    {
                        vr = new ValidationResult("Il Codice Fiscale non è corretto.");
                    }
                }
                else
                {
                    vr = new ValidationResult("Il Codice Fiscale è richiesto e deve essere composto da 16 caratteri.");
                }
            }
            else
            {
                vr = new ValidationResult("Il Codice Fiscale è richiesto e deve essere composto da 16 caratteri.");
            }

            return vr;
        }




        public void SetFiglio(ref FigliModel fm, ModelDBISE db)
        {
            FIGLI f = new FIGLI()
            {
                IDMAGGIORAZIONEFIGLI = fm.idMaggiorazioneFigli,
                NOME = fm.nome,
                COGNOME = fm.cognome,
                CODICEFISCALE = fm.codiceFiscale,
                DATAINIZIOVALIDITA = fm.dataInizio.Value,
                DATAFINEVALIDITA = fm.dataFine.HasValue ? fm.dataFine.Value : Convert.ToDateTime("31/12/9999"),
                DATAAGGIORNAMENTO = fm.dataAggiornamento,
                ANNULLATO = fm.Annullato
            };

            db.FIGLI.Add(f);

            int i = db.SaveChanges();

            if (i > 0)
            {
                fm.idFigli = f.IDFIGLI;
            }
            else
            {
                throw new Exception(string.Format("Il figlio {0} non è stato inserito.", fm.nominativo));
            }

        }


        public IList<FigliModel> GetListaFigli(decimal idMaggiorazioneFigli)
        {
            List<FigliModel> lfm = new List<FigliModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lf = db.FIGLI.Where(a => a.ANNULLATO == false && a.IDMAGGIORAZIONEFIGLI == idMaggiorazioneFigli).OrderBy(a => a.COGNOME).ThenBy(a => a.NOME).ToList();

                if (lf != null && lf.Count > 0)
                {

                    foreach (var item in lf)
                    {
                        var fm = new FigliModel()
                        {
                            idFigli = item.IDFIGLI,
                            idMaggiorazioneFigli = item.IDMAGGIORAZIONEFIGLI,
                            nome = item.NOME,
                            cognome = item.COGNOME,
                            codiceFiscale = item.CODICEFISCALE,
                            //lAtriDatiFamiliari = (from e in item.ALTRIDATIFAM
                            //                      where e.ANNULLATO == false
                            //                      select new AltriDatiFamModel()
                            //                      {
                            //                          idAltriDatiFam = e.IDALTRIDATIFAM,
                            //                          idFigli = e.IDFIGLI,
                            //                          idMaggiorazioneConiuge = e.IDMAGGIORAZIONECONIUGE,
                            //                          dataNascita = e.DATANASCITA,
                            //                          capNascita = e.CAPNASCITA,
                            //                          comuneNascita = e.COMUNENASCITA,
                            //                          provinciaNascita = e.PROVINCIANASCITA,
                            //                          nazionalita = e.NAZIONALITA,
                            //                          indirizzoResidenza = e.INDIRIZZORESIDENZA,
                            //                          capResidenza = e.CAPRESIDENZA,
                            //                          comuneResidenza = e.COMUNERESIDENZA,
                            //                          provinciaResidenza = e.PROVINCIARESIDENZA,
                            //                          residente = e.RESIDENTE,
                            //                          studente = e.STUDENTE,
                            //                          ulterioreMagConiuge = e.ULTERIOREMAGCONIUGE,
                            //                          dataAggiornamento = e.DATAAGGIORNAMENTO,
                            //                          annullato = e.ANNULLATO
                            //                      }).ToList()
                        };

                        lfm.Add(fm);
                    }
                }
            }

            return lfm;

        }





    }
}