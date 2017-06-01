using NewISE.Models.DBModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;

namespace NewISE.Areas.Dipendenti.Models.DtObj
{
    public class dtDipendenti : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Custom validations

        /// <summary>
        /// Verifica l'univocità della matricola inserita.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ValidationResult MatricolaUnivoca(string v, ValidationContext context)
        {
            var dNew = context.ObjectInstance as DipendentiModel;
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                //Prelevo il record interessato dalla verifica.
                var vli = db.DIPENDENTI.Where(a => a.IDDIPENDENTE == dNew.idDipendente);
                if (vli != null && vli.Count() > 0)
                {
                    //Se il record interessato ha la stessa matricola, vuol dire che la modifica
                    //effettuata non ha bisogno di verificare l'univocità della matricola.
                    if (vli.First().MATRICOLA == dNew.matricola)
                    {
                        return ValidationResult.Success;
                    }
                }

                var li = db.DIPENDENTI.Where(a => a.MATRICOLA == dNew.matricola);
                if (li != null && li.Count() > 0)
                {
                    var i = li.First();

                    if (i.MATRICOLA == dNew.matricola)
                    {
                        return new ValidationResult("La matricola inserita è già presente, inserirne un altra.");
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
        }

        /// <summary>
        /// verifica l'univocità dell'email inserita.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ValidationResult EmailUnivoca(string v, ValidationContext context)
        {
            var dNew = context.ObjectInstance as DipendentiModel;
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                //Prelevo il record interessato dalla verifica.
                var vli = db.DIPENDENTI.Where(a => a.IDDIPENDENTE == dNew.idDipendente);
                if (vli != null && vli.Count() > 0)
                {
                    //Se il record interessato ha la stessa matricola, vuol dire che la modifica
                    //effettuata non ha bisogno di verificare l'univocità della matricola.
                    if (vli.First().EMAIL == dNew.email)
                    {
                        return ValidationResult.Success;
                    }
                }

                var li = db.DIPENDENTI.Where(a => a.EMAIL == dNew.email);
                if (li != null && li.Count() > 0)
                {
                    var i = li.First();

                    if (i.EMAIL == dNew.email)
                    {
                        return new ValidationResult("L'E-mail inserita è già presente, inserirne un altra.");
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
        }

        #endregion Custom validations

        /// <summary>
        /// preleva il dipendente in base all'id passato.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DipendentiModel GetDipendenteByID(decimal id)
        {
            DipendentiModel dm;

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                DIPENDENTI d = db.DIPENDENTI.Find(id);

                dm = new DipendentiModel()
                {
                    idDipendente = d.IDDIPENDENTE,
                    matricola = d.MATRICOLA,
                    nome = d.NOME,
                    cognome = d.COGNOME,
                    dataAssunzione = d.DATAASSUNZIONE,
                    dataCessazione = d.DATACESSAZIONE,
                    indirizzo = d.INDIRIZZO,
                    cap = d.CAP,
                    citta = d.CITTA,
                    provincia = d.PROVINCIA,
                    email = d.EMAIL,
                    telefono = d.TELEFONO,
                    fax = d.FAX,
                    abilitato = d.ABILITATO,
                    dataInizioRicalcoli = d.DATAINIZIORICALCOLI,
                    cdcGepe = new CDCGepeModel()
                    {
                        iddipendente = d.CDCGEPE.IDDIPENDENTE,
                        codiceCDC = d.CDCGEPE.CODICECDC,
                        descCDC = d.CDCGEPE.DESCCDC,
                        dataInizioValidita = d.CDCGEPE.DATAINIZIOVALIDITA
                    }
                };
            }

            return dm;
        }

        public DipendentiModel GetDipendenteByMatricola(int matricola)
        {
            DipendentiModel dm;

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                DIPENDENTI d = db.DIPENDENTI.Where(a=>a.MATRICOLA == matricola).First();

                dm = new DipendentiModel()
                {
                    idDipendente = d.IDDIPENDENTE,
                    matricola = d.MATRICOLA,
                    nome = d.NOME,
                    cognome = d.COGNOME,
                    dataAssunzione = d.DATAASSUNZIONE,
                    dataCessazione = d.DATACESSAZIONE,
                    indirizzo = d.INDIRIZZO,
                    cap = d.CAP,
                    citta = d.CITTA,
                    provincia = d.PROVINCIA,
                    email = d.EMAIL,
                    telefono = d.TELEFONO,
                    fax = d.FAX,
                    abilitato = d.ABILITATO,
                    dataInizioRicalcoli = d.DATAINIZIORICALCOLI,
                    cdcGepe = new CDCGepeModel()
                    {
                        iddipendente = d.CDCGEPE.IDDIPENDENTE,
                        codiceCDC = d.CDCGEPE.CODICECDC,
                        descCDC = d.CDCGEPE.DESCCDC,
                        dataInizioValidita = d.CDCGEPE.DATAINIZIOVALIDITA
                    }
                };
            }

            return dm;
        }

        public IList<DipendentiModel> GetDipendenti()
        {
            List<DipendentiModel> ldm = new List<DipendentiModel>();

            using (EntitiesDBISE db=new EntitiesDBISE())
            {
                var ld = db.DIPENDENTI.ToList();

                ldm = (from e in ld
                       select new DipendentiModel() {
                           idDipendente = e.IDDIPENDENTE,
                           matricola = e.MATRICOLA,
                           nome = e.NOME,
                           cognome = e.COGNOME,
                           dataAssunzione = e.DATAASSUNZIONE,
                           dataCessazione = e.DATACESSAZIONE,
                           indirizzo = e.INDIRIZZO,
                           cap = e.CAP,
                           citta = e.CITTA,
                           provincia = e.PROVINCIA,
                           email = e.EMAIL,
                           telefono = e.TELEFONO,
                           fax = e.FAX,
                           abilitato = e.ABILITATO,
                           dataInizioRicalcoli = e.DATAINIZIORICALCOLI
                           //cdcGepe = new CDCGepeModel() {
                           //    iddipendente = e.CDCGEPE.IDDIPENDENTE,
                           //    codiceCDC = e.CDCGEPE.CODICECDC,
                           //    descCDC = e.CDCGEPE.DESCCDC,
                           //    dataInizioValidita = e.CDCGEPE.DATAINIZIOVALIDITA
                           //}
                       }).ToList();

            }

            return ldm;

        }

        
    }
}