
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtUtenzeDipendenti : IDisposable
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
            using (ModelDBISE db = new ModelDBISE())
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
        public static ValidationResult EmailSecondariaGiaEsistente(string v, ValidationContext context)
        {
            var dNew = context.ObjectInstance as EmailSecondarieDipModel;
            if(string.IsNullOrEmpty(dNew.Email))
            {
                return new ValidationResult("L'E-mail inserita è vuota");
            }
            using (ModelDBISE db = new ModelDBISE())
            {
                var vli = db.EMAILSECONDARIEDIP.Where(a => a.IDDIPENDENTE == dNew.idDipendente);
                bool found = false;
                
                if (vli != null && vli.Count() > 0)
                {
                    foreach (EMAILSECONDARIEDIP x in vli)
                    {
                        if (x.EMAIL.ToUpper().Trim() == dNew.Email.ToUpper().Trim())
                        {
                            found = true; break;
                        }
                    }
                    if (found == true)
                    {
                        return new ValidationResult("L'E-mail inserita è già presente, inserirne un'altra.");
                    }
                }
                return ValidationResult.Success;
            }
        }

        #endregion Custom validations

        public List<UtentiAutorizzatiModel> GetDipendentiAutorizzati()
        {
            List<UtentiAutorizzatiModel> tmp = new List<UtentiAutorizzatiModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                var ld = db.UTENTIAUTORIZZATI.ToList();
                tmp = (from e in ld where e.IDDIPENDENTE!=null && (decimal)e.IDDIPENDENTE!=0
                       select new UtentiAutorizzatiModel()
                       {
                           idUtenteAutorizzato=e.IDUTENTEAUTORIZZATO,
                           idDipendente = (decimal)e.IDDIPENDENTE,
                           idRouloUtente=e.IDRUOLOUTENTE,
                           Utente=e.UTENTE,
                       }).ToList();
            }
             return tmp;
        }


        public IList<DipendentiModel> GetUtenzeAutorizzate(List<UtentiAutorizzatiModel> autList)
        {
            List<DipendentiModel> ldm = new List<DipendentiModel>();
            DipendentiModel tmp = new DipendentiModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                foreach (var x in autList)
                {
                    var ld = db.DIPENDENTI.Find(x.idDipendente);
                    tmp = new DipendentiModel()
                    {
                        idDipendente = ld.IDDIPENDENTE,
                        matricola = ld.MATRICOLA,
                        nome = ld.NOME,
                        cognome = ld.COGNOME,
                        dataAssunzione = ld.DATAASSUNZIONE,
                        dataCessazione = ld.DATACESSAZIONE,
                        indirizzo = ld.INDIRIZZO,
                        cap = ld.CAP,
                        citta = ld.CITTA,
                        provincia = ld.PROVINCIA,
                        email = ld.EMAIL,
                        telefono = ld.TELEFONO,
                        fax = ld.FAX,
                        abilitato = ld.ABILITATO,
                        dataInizioRicalcoli = ld.DATAINIZIORICALCOLI
                    };
                    ldm.Add(tmp);
                }
            }
            return ldm;
        }

        public DipendentiModel GetDatiUtenzaAutorizzata(decimal idd)
        {
            DipendentiModel ldm = new DipendentiModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ld = db.DIPENDENTI.Where(b=>b.IDDIPENDENTE==idd).ToList();

                ldm = (from e in ld
                       select new DipendentiModel()
                       {
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
                       }).ToList().First();
            }
            return ldm;
        }
       
        public EmailSecondarieDipModel GetDatiEmailSecondaria(decimal idEmailSec)
        {
            EmailSecondarieDipModel ldm = new EmailSecondarieDipModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ld = db.EMAILSECONDARIEDIP.Where(b => b.IDEMAILSECONDARIA == idEmailSec).ToList();

                ldm = (from e in ld
                       select new EmailSecondarieDipModel()
                       {
                           idEmailSecDip = e.IDEMAILSECONDARIA,
                            idDipendente=e.IDDIPENDENTE,
                             Attiva=e.ATTIVA,
                              Email=e.EMAIL
                       }).ToList().First();
            }
            return ldm;
        }
        public void EditStatoDipendente(bool abilitato, decimal idDipendente)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    DIPENDENTI d = db.DIPENDENTI.Find(idDipendente);
                    d.ABILITATO = abilitato;
                    db.Database.BeginTransaction();
                    db.SaveChanges();
                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }
        //
        public void EditStatoEmailSecondaria(bool attiva, decimal idEmailSec)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    EMAILSECONDARIEDIP d = db.EMAILSECONDARIEDIP.Find(idEmailSec);
                    d.ATTIVA = attiva;
                    db.Database.BeginTransaction();
                    db.SaveChanges();
                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }
        public List<EmailSecondarieDipModel> GetListaEmailSecondarioDip(decimal idDipendente)
        {
            List<EmailSecondarieDipModel> lcsm = new List<EmailSecondarieDipModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                DIPENDENTI d = db.DIPENDENTI.Find(idDipendente);

                var lemail =d.EMAILSECONDARIEDIP.Where(
                        a => a.IDDIPENDENTE== idDipendente).ToList();

                if (lemail?.Any() ?? false)
                {
                    foreach (var cs in lemail)
                    {
                        var csm = new EmailSecondarieDipModel()
                        {
                             idEmailSecDip=cs.IDEMAILSECONDARIA,
                             idDipendente=cs.IDDIPENDENTE,
                             Email=cs.EMAIL,
                             Attiva=cs.ATTIVA
                        };
                        lcsm.Add(csm);
                    }
                }
            }
            return lcsm;
        }

        public bool AddMail(string emailSec, decimal idd)
        {
            bool tmp = true;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    EMAILSECONDARIEDIP em =new EMAILSECONDARIEDIP() ;
                    em.EMAIL = emailSec;
                    em.IDDIPENDENTE = idd;
                    em.ATTIVA = true;
                    db.Database.BeginTransaction();
                    db.EMAILSECONDARIEDIP.Add(em);
                    db.SaveChanges();
                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    tmp = false;
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
            return tmp;
        }
        public void DelEmail(decimal idEmailSec)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    EMAILSECONDARIEDIP em = db.EMAILSECONDARIEDIP.Find(idEmailSec);
                    db.Database.BeginTransaction();
                    db.EMAILSECONDARIEDIP.Remove(em);
                    db.SaveChanges();
                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }
        public bool EmailSecondariaGiaInserita(decimal idDipendente, string email)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var vli = db.EMAILSECONDARIEDIP.Where(a => a.IDDIPENDENTE == idDipendente && a.EMAIL.ToUpper().Trim() == email.ToUpper().Trim());
                if (vli != null && vli.Count() > 0)
                    return true;
            }
            return false;
        }
        public  bool emailIsValid(string email)
        {
            string expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}