﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;
using System.Data.Entity;
using NewISE.Models.IseArio.dtObj;
using NewISE.Interfacce.Modelli;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtElaborazioni : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void AssociaAliquoteIndSist(decimal idIndSist, decimal idAliquota, ModelDBISE db)
        {
            var indSist = db.ELABINDSISTEMAZIONE.Find(idIndSist);
            var item = db.Entry<ELABINDSISTEMAZIONE>(indSist);

            item.State = EntityState.Modified;
            item.Collection(a => a.ALIQUOTECONTRIBUTIVE).Load();
            var aliq = db.ALIQUOTECONTRIBUTIVE.Find(idAliquota);

            indSist.ALIQUOTECONTRIBUTIVE.Add(aliq);

            var i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Impossibile associare l'aliquota alla prima sistemazione.");
            }



        }

        public void InviaAnticipoPrimaSistemazioneContabilita(decimal idAttivitaAnticipi, ModelDBISE db)
        {
            try
            {
                var aa = db.ATTIVITAANTICIPI.Find(idAttivitaAnticipi);
                var ps = aa.PRIMASITEMAZIONE;
                var t = ps.TRASFERIMENTO;
                var ra = aa.RINUNCIAANTICIPI;

                if (ra.RINUNCIAANT == false)
                {
                    var lanticipi =
                        aa.ANTICIPI.Where(
                            a =>
                                a.ANNULLATO == false &&
                                a.IDTIPOLOGIAANTICIPI == (decimal)EnumTipoAnticipi.Prima_sistemazione).ToList();

                    if (lanticipi?.Any() ?? false)
                    {
                        var anticipi = lanticipi.First();
                        using (CalcoliIndennita ci = new CalcoliIndennita(t.IDTRASFERIMENTO, t.DATAPARTENZA))
                        {
                            //decimal importoAnticipo = ci.AnticipoPrimaSistemazione(anticipi.PERCENTUALEANTICIPO);
                            ELABINDSISTEMAZIONE eis = new ELABINDSISTEMAZIONE()
                            {
                                IDPRIMASISTEMAZIONE = ps.IDPRIMASISTEMAZIONE,
                                INDENNITABASE = ci.IndennitaDiBase,
                                COEFFICENTESEDE = ci.CoefficienteDiSede,
                                PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                                COEFFICENTEINDSIST = ci.CoefficienteIndennitaSistemazione,
                                PERCENTUALERIDUZIONE = ci.PercentualeRiduzionePrimaSistemazione,
                                PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                                PENSIONECONIUGE = ci.PensioneConiuge,
                                INDENNITAPRIMOSEGRETARIO = ci.IndennitaPrimoSegretario,
                                PERCENTUALEMAGFIGLIO = ci.PercentualeMaggiorazioneFigli,
                                ANTICIPO = true,
                                SALDO = false,
                                UNICASOLUZIONE = false,
                                PERCANTSALDOUNISOL = anticipi.PERCENTUALEANTICIPO,
                                DATAOPERAZIONE = DateTime.Now,
                                ELABORATO = false,
                                ANNULLATO = false
                            };


                            var leis =
                                db.ELABINDSISTEMAZIONE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDPRIMASISTEMAZIONE == ps.IDPRIMASISTEMAZIONE &&
                                        a.ANTICIPO == true)
                                    .OrderByDescending(a => a.IDINDSISTLORDA).ToList();

                            if (leis?.Any() ?? false)
                            {
                                var eisOld = leis.First();
                                eis.FK_IDINDSISTLORDA = eisOld.IDINDSISTLORDA;

                            }

                            db.ELABINDSISTEMAZIONE.Add(eis);

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore nella fase d'inderimento dell'anticipo di prima sistemazione.");
                            }

                            ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                            var lacDetr =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                        t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                    .ToList();


                            if (lacDetr?.Any() ?? false)
                            {
                                detrazioni = lacDetr.First();
                            }
                            else
                            {
                                throw new Exception("Non sono presenti le detrazioni per il periodo del trasferimento elaborato.");
                            }


                            this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, detrazioni.IDALIQCONTR, db);


                            ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

                            var lacPrev =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                        t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                    .ToList();

                            if (lacPrev?.Any() ?? false)
                            {
                                aliqPrev = lacPrev.First();
                            }
                            else
                            {
                                throw new Exception("Non sono presenti le detrazioni per il periodo del trasferimento elaborato.");
                            }


                            this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, aliqPrev.IDALIQCONTR, db);

                            var importoPrimaSistemazioneLorda = CalcoliIndennita.ElaboraPrimaSistemazione(eis.INDENNITABASE,
                                eis.COEFFICENTESEDE, eis.PERCENTUALEDISAGIO, eis.PERCENTUALERIDUZIONE,
                                eis.COEFFICENTEINDSIST);


                            var ImponibilePrevidenziale = importoPrimaSistemazioneLorda - detrazioni.VALORE;
                            var RitenutePrevidenziali = ImponibilePrevidenziale * aliqPrev.VALORE / 100;

                            var dip = t.DIPENDENTI;

                            using (dtAliquotaISE dtai = new dtAliquotaISE())
                            {
                                var aliqIse = dtai.GetAliquotaIse(dip.MATRICOLA, RitenutePrevidenziali);

                                var RitenutaIperf = (ImponibilePrevidenziale - RitenutePrevidenziali) * aliqIse.Aliquota / 100;

                                var Netto = importoPrimaSistemazioneLorda - RitenutePrevidenziali - RitenutaIperf;

                                var anticipoNetto = Netto * (eis.PERCANTSALDOUNISOL / 100);

                                using (CalcoloMeseAnnoElaborazione cmae = new CalcoloMeseAnnoElaborazione(db))
                                {

                                    var lmae = cmae.Mae;

                                    if (lmae?.Any() ?? false)
                                    {
                                        var mae = lmae.First();
                                        if (mae.Elaborato == true)
                                        {
                                            cmae.NewMeseDaElaborare();
                                        }



                                        TEORICI teorici = new TEORICI()
                                        {
                                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                            IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                            IDMESEANNOELAB = mae.IdMeseAnnoElab,
                                            MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                            ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                            ALIQUOTAFISCALE = aliqIse.Aliquota,
                                            GIORNI = 0,
                                            IMPORTO = anticipoNetto,
                                            DATAOPERAZIONE = DateTime.Now,
                                            ANNULLATO = false
                                        };

                                        db.TEORICI.Add(teorici);

                                        int j = db.SaveChanges();

                                        if (j <= 0)
                                        {
                                            throw new Exception("Errore nella fase d'inderimento dell'anticipo di prima sistemazione in contabilità.");
                                        }

                                        CONT_OA contabilita = new CONT_OA()
                                        {
                                            IDTEORICI = teorici.IDTEORICI,
                                            MATRICOLA = dip.MATRICOLA,
                                            LIVELLO = ci.Livello.LIVELLO,
                                            CODICESEDE = t.UFFICI.CODICEUFFICIO,
                                        };

                                        db.CONT_OA.Add(contabilita);

                                        int y = db.SaveChanges();

                                        if (y <= 0)
                                        {
                                            throw new Exception("Errore nella fase d'inderimento dell'anticipo di prima sistemazione in contabilità OA.");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Errore nella fase di lettura del mese di elaborazione.");
                                    }


                                }
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


        public void InvioEmailOAAnticipoPrimaSistemazione(decimal idPrimaSistemazione, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();

            try
            {
                am = Utility.UtenteAutorizzato();
                if (am.RuoloAccesso.idRuoloAccesso != (decimal)EnumRuoloAccesso.SuperAmministratore)
                {
                    mittente.Nominativo = am.nominativo;
                    mittente.EmailMittente = am.eMail;
                }





            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        /// <summary>
        /// Preleva tutti gli anni elaborati.
        /// </summary>
        /// <returns></returns>
        public IList<MeseAnnoElaborazioneModel> PrelevaAnniMesiElaborati()
        {
            List<MeseAnnoElaborazioneModel> lmaem = new List<MeseAnnoElaborazioneModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (CalcoloMeseAnnoElaborazione cmem = new CalcoloMeseAnnoElaborazione(db))
                    {
                        lmaem = cmem.Mae.ToList();
                    }

                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }

                return lmaem;
            }

        }


        public IList<ElencoDipendentiDaCalcolareModel> PrelevaDipendentiDaElaborare()
        {
            List<ElencoDipendentiDaCalcolareModel> ledem = new List<ElencoDipendentiDaCalcolareModel>();
            int anno = DateTime.Now.Year;
            int mese = DateTime.Now.Month;

            using (ModelDBISE db = new ModelDBISE())
            {
                //var ldip =
                //    db.DIPENDENTI.Where(
                //        a =>
                //            a.DATAINIZIORICALCOLI.Year <= anno && a.DATAINIZIORICALCOLI.Month <= mese &&
                //            a.TRASFERIMENTO.Any(
                //                b =>
                //                    b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                //                    b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato))
                //        .OrderBy(a => a.NOME)
                //        .ThenBy(a => a.COGNOME)
                //        .ThenBy(a => a.MATRICOLA)
                //        .ThenBy(a => a.DATAINIZIORICALCOLI)
                //        .ToList();

                var ldip =
                    db.DIPENDENTI
                        .OrderBy(a => a.NOME)
                        .ThenBy(a => a.COGNOME)
                        .ThenBy(a => a.MATRICOLA)
                        .ThenBy(a => a.DATAINIZIORICALCOLI)
                        .ToList();

                if (ldip?.Any() ?? false)
                {
                    foreach (var d in ldip)
                    {
                        ElencoDipendentiDaCalcolareModel edem = new ElencoDipendentiDaCalcolareModel()
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
                            SelezionaDipendenteDaElaborare = false
                        };

                        ledem.Add(edem);
                    }
                }

            }


            return ledem;

        }


    }
}