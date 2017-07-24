using NewISE.EF;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtConiuge : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static ValidationResult VerificaCodiceFiscale(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var cm = context.ObjectInstance as ConiugeModel;

            if (cm != null)
            {

                if (cm.codiceFiscale != null && cm.codiceFiscale != string.Empty)
                {
                    if (Utility.CheckCodiceFiscale(cm.codiceFiscale))
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

        public static ValidationResult VerificaCodFiscMaggiorazioneConiugeVModel(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var cm = context.ObjectInstance as MaggiorazioneConiugeVModel;

            if (cm != null)
            {

                if (cm.codiceFiscale != null && cm.codiceFiscale != string.Empty)
                {
                    if (Utility.CheckCodiceFiscale(cm.codiceFiscale))
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


        public ConiugeModel GetConiuge(decimal idMaggiorazioneConiuge)
        {
            ConiugeModel cm = new ConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idMaggiorazioneConiuge);

                if (c != null && c.IDMAGGIORAZIONECONIUGE > 0)
                {
                    cm = new ConiugeModel()
                    {
                        idMaggiorazioneConiuge = c.IDMAGGIORAZIONECONIUGE,
                        nome = c.NOME,
                        cognome = c.COGNOME,
                        codiceFiscale = c.CODICEFISCALE
                    };


                }


            }

            return cm;

        }


        public void SetConiuge(ConiugeModel cm, ModelDBISE db)
        {
            CONIUGE c = new CONIUGE()
            {
                IDMAGGIORAZIONECONIUGE = cm.idMaggiorazioneConiuge,
                NOME = cm.nome,
                COGNOME = cm.cognome,
                CODICEFISCALE = cm.codiceFiscale
            };

            db.CONIUGE.Add(c);

            db.SaveChanges();
        }

        public void EditConiuge(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(cm.idMaggiorazioneConiuge);

                if (c != null && c.IDMAGGIORAZIONECONIUGE > 0)
                {
                    c.IDMAGGIORAZIONECONIUGE = cm.idMaggiorazioneConiuge;
                    c.NOME = cm.nome;
                    c.COGNOME = cm.cognome;
                    c.CODICEFISCALE = cm.codiceFiscale;

                    db.SaveChanges();
                }
            }
        }

    }
}