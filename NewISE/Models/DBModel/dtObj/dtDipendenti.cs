﻿
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel.dtObj
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
        public static ValidationResult EmailUnivoca(string v, ValidationContext context)
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

        public DipendentiModel GetDipendenteByEmail(string email, ModelDBISE db)
        {
            DipendentiModel dm = new DipendentiModel();

            var ld = db.DIPENDENTI.Where(a => a.EMAIL == email).ToList();

            if (ld?.Any() ?? false)
            {
                var d = ld.First();

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



        /// <summary>
        /// preleva il dipendente in base all'id trasferimento passato.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DipendentiModel GetDipendenteByIDTrasf(decimal idTrasf, ModelDBISE db)
        {
            DipendentiModel dm = new DipendentiModel();

            TRASFERIMENTO t = db.TRASFERIMENTO.Find(idTrasf);

            var d = db.DIPENDENTI.Find(t.IDDIPENDENTE);

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

            return dm;
        }

        /// <summary>
        /// preleva il dipendente in base all'id trasferimento passato.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DipendentiModel GetDipendenteByIDTrasf(decimal idTrasf)
        {
            DipendentiModel dm = new DipendentiModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                TRASFERIMENTO t = db.TRASFERIMENTO.Find(idTrasf);

                var d = t.DIPENDENTI;

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



        /// <summary>
        /// preleva il dipendente in base all'id passato.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DipendentiModel GetDipendenteByID(decimal id, ModelDBISE db)
        {
            DipendentiModel dm;


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


            return dm;
        }




        /// <summary>
        /// preleva il dipendente in base all'id passato.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DipendentiModel GetDipendenteByID(decimal id)
        {
            DipendentiModel dm;

            using (ModelDBISE db = new ModelDBISE())
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
                    //cdcGepe = new CDCGepeModel()
                    //{
                    //    iddipendente = d.CDCGEPE.IDDIPENDENTE,
                    //    codiceCDC = d.CDCGEPE.CODICECDC,
                    //    descCDC = d.CDCGEPE.DESCCDC,
                    //    dataInizioValidita = d.CDCGEPE.DATAINIZIOVALIDITA
                    //}
                };
            }

            return dm;
        }


        public DipendentiModel GetDipendenteByMatricola(string matricola)
        {
            DipendentiModel dm = new DipendentiModel();
            int matr = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    matr = Convert.ToInt32(matricola);

                    DIPENDENTI d = db.DIPENDENTI.Where(a => a.MATRICOLA == matr).First();

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
                catch (Exception ex)
                {
                    throw ex;
                }



            }

            return dm;
        }


        public DipendentiModel GetDipendenteByMatricola(string matricola, ModelDBISE db)
        {
            DipendentiModel dm = new DipendentiModel();
            int matr = 0;

            try
            {
                matr = Convert.ToInt32(matricola);

                DIPENDENTI d = db.DIPENDENTI.Where(a => a.MATRICOLA == matr).First();

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
            catch (Exception ex)
            {

            }





            return dm;
        }




        public DipendentiModel GetDipendenteByMatricola(int matricola)
        {
            DipendentiModel dm;

            using (ModelDBISE db = new ModelDBISE())
            {
                DIPENDENTI d = db.DIPENDENTI.Where(a => a.MATRICOLA == matricola).First();

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



        public DipendentiModel GetDipendenteByMatricola(int matricola, ModelDBISE db)
        {
            DipendentiModel dm;


            DIPENDENTI d = db.DIPENDENTI.Where(a => a.MATRICOLA == matricola).First();

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


            return dm;
        }



        public IList<DipendentiModel> GetDipendenti()
        {
            List<DipendentiModel> ldm = new List<DipendentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ld = db.DIPENDENTI.Where(a => a.NOSISTEMA == false).ToList();

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


        /// <summary>
        /// Imposta la data di inizio ricalcoli per il dipendente passato come parametro.
        /// </summary>
        /// <param name="idDipendente"></param>
        /// <param name="dtIniRicalcoli"></param>
        /// <param name="db"></param>
        public void DataInizioRicalcoliDipendente(decimal idTrasferimento, DateTime dtIniRicalcoli, ModelDBISE db, bool saveDb = false)
        {
            try
            {

                DateTime dtIniRicalcoliLav = Utility.GetDataInizioMese(dtIniRicalcoli);

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                if (t.DATAPARTENZA <= dtIniRicalcoli && t.DATARIENTRO >= dtIniRicalcoli)
                {
                    var d = t.DIPENDENTI;

                    if (d.DATAINIZIORICALCOLI > dtIniRicalcoliLav)
                    {
                        d.DATAINIZIORICALCOLI = dtIniRicalcoliLav;
                    }

                    d.RICALCOLARE = true;

                    if (saveDb)
                    {
                        int i = db.SaveChanges();

                        if (d.DATAINIZIORICALCOLI > dtIniRicalcoliLav)
                        {
                            if (i <= 0)
                            {
                                throw new Exception("Impossibile aggiornare la data di inizio ricalcoli per il dipendente " + d.COGNOME + " " + d.NOME + "(" + d.MATRICOLA + ")");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }
        /// <summary>
        /// Imposta la data del dipendente trasferito alla data di partenza del trasferimento passato come parametro.
        /// </summary>
        /// <param name="idTrasferimento"></param>
        /// <param name="db"></param>
        /// <param name="saveDB">Se vero salva immediatamente la modifica effettuata sul database.</param>
        public void ResetDataInizioRicalcoli(decimal idTrasferimento, ModelDBISE db, bool saveDB = false, bool ricalcolare = false)
        {
            var t = db.TRASFERIMENTO.Find(idTrasferimento);

            var d = t.DIPENDENTI;

            d.DATAINIZIORICALCOLI = t.DATAPARTENZA;

            d.RICALCOLARE = ricalcolare;

            if (saveDB)
            {
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile resettare la data di inizio ricalcoli per il dipendente " + d.COGNOME + " " + d.NOME + "(" + d.MATRICOLA + ")");
                }
            }
        }

        public void SetLastMeseElabDataInizioRicalcoli(decimal idDipendente, decimal idMeseAnnoElaborato, ModelDBISE db, bool saveDB = false)
        {
            var d = db.DIPENDENTI.Find(idDipendente);
            var mae = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElaborato);

            d.DATAINIZIORICALCOLI = Convert.ToDateTime("01/" + mae.MESE.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + mae.ANNO);
            d.RICALCOLARE = false;
            if (saveDB)
            {
                int i = db.SaveChanges();

                //if (i <= 0)
                //{
                //    throw new Exception("Impossibile resettare la data di inizio ricalcoli per il dipendente " + d.COGNOME + " " + d.NOME + "(" + d.MATRICOLA + ")");
                //}
            }
        }
        /// <summary>
        /// Preleva i dipendenti che hanno avuto almeno un trasferimento.
        /// </summary>
        /// <returns></returns>
        public IList<DipendentiModel> GetDipendentiAnyTrasf()
        {
            List<DipendentiModel> ldipm = new List<DipendentiModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ldip =
                        db.DIPENDENTI.Where(
                            a =>
                                a.NOSISTEMA == false &&
                                a.TRASFERIMENTO.Any(
                                    b => b.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato))
                            .OrderBy(a => a.COGNOME)
                            .ThenBy(a => a.NOME)
                            .ToList();


                    ldipm = (from d in ldip
                             select new DipendentiModel()
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

                             }).ToList();

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ldipm;

        }



    }
}