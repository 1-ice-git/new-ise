using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;

using Newtonsoft.Json.Schema;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using System.ComponentModel.DataAnnotations;

using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.ModelRest;
using System.Diagnostics;
using System.IO;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtVariazioniMaggiorazioneAbitazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Funzioni di validazione custom
        public static ValidationResult VerificaDataInizioValiditaCanoneMABUtente(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var cmabvm = context.ObjectInstance as CanoneMABViewModel;

            if (cmabvm != null)
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var mab = db.MAB.Find(cmabvm.IDMAB);
                    var pmab = mab.PERIODOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDPERIODOMAB).ToList().First();
                    if (cmabvm.ut_dataInizioValidita < pmab.DATAINIZIOMAB)
                    {
                        vr = new ValidationResult(string.Format("Impossibile inserire la Data Inizio Validità minore della Data Inizio Validità MAB ({0}).", pmab.DATAINIZIOMAB.ToShortDateString()));
                    }

                }

            }
            else
            {
                vr = new ValidationResult("La Data Inizio Validità è richiesta.");
            }

            return vr;
        }

        #endregion


        public void VerificaDataInizioValiditaCanoneMAB_Utente(decimal idMab, DateTime ?data, ModelDBISE db)
        {
            if (data == null)
            {
                throw new Exception("La Data Inizio Validità è obbligatoria.");
            }

            var mab = db.MAB.Find(idMab);
            var pmab = mab.PERIODOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDPERIODOMAB).ToList().First();
            if (data < pmab.DATAINIZIOMAB)
            {
               throw new Exception(string.Format("Impossibile inserire la Data Inizio Validità minore della Data Inizio Validità MAB ({0}).", pmab.DATAINIZIOMAB.ToShortDateString()));
            }
            if (data > pmab.DATAFINEMAB)
            {
                throw new Exception(string.Format("Impossibile inserire la Data Inizio Validità maggiore della Data Fine Vallidita MAB ({0}).", pmab.DATAFINEMAB.ToShortDateString()));
            }
        }

        public void VerificaDateMAB_Utente(MABViewModel mvm, decimal idTrasferimento, DateTime? dataini, DateTime? datafin, bool inserimentoMAB, ModelDBISE db)
        {
            //check dataini NULL
            if (dataini == null)
            {
                throw new Exception("La Data Inizio Validità è obbligatoria.");
            }
            
            var t = db.TRASFERIMENTO.Find(idTrasferimento);

            
            //check datafin> rientro
            if (datafin > t.DATARIENTRO)
            {
                throw new Exception(string.Format("La Data Fine MAB deve essere minore della Data Rientro del trasferimento ({0}).", t.DATARIENTRO.ToShortDateString()));
            }

            var ultimaMabAttiva = GetUltimaMABAttiva(idTrasferimento, db);
            if (ultimaMabAttiva.IDMAB > 0)
            {
                var ultimoPeriodo = GetPeriodoMAB(ultimaMabAttiva.IDMAB, db);
                var periodoPrec = GetPeriodoMABPrecedente(ultimaMabAttiva.IDMAB, db);
                var n_periodi = ultimaMabAttiva.PERIODOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList().Count();

                //check dataini<=datafineMAB precedente
                if (periodoPrec.IDPERIODOMAB > 0)
                {
                    if (dataini <= periodoPrec.DATAFINEMAB)
                    {
                        throw new Exception(string.Format("La Data Inizio MAB deve essere maggiore della Data Fine validita della MAB precedente ({0}).", periodoPrec.DATAFINEMAB.ToShortDateString()));
                    }
                }
                else
                {
                    if (dataini < ultimoPeriodo.DATAFINEMAB && n_periodi>1)
                    {
                        throw new Exception(string.Format("La Data Inizio MAB non può essere inferiore della Data Fine validita della MAB precedente ({0}).", ultimoPeriodo.DATAFINEMAB.ToShortDateString()));
                    }
                }
            }
            else
            {
                if (dataini < t.DATAPARTENZA)
                {
                    throw new Exception(string.Format("La Data Inizio MAB non può essere inferiore della Data Partenza del trasferimento ({0}).", t.DATAPARTENZA.ToShortDateString()));
                }
            }

            //check dataini>datafin
            if (dataini != null && datafin != null)
            {
                if (dataini >= datafin)
                {
                    throw new Exception(string.Format("La Data Fine validità deve essere maggiore della Data Inizio validita ({0}).", dataini.Value.ToShortDateString()));
                }
            }

            //check dataini>=datarientro
            if (dataini >= t.DATARIENTRO)
            {
                throw new Exception(string.Format("La Data Inizio validità deve essere inferiore della Data Rientro del trasferimento ({0}).", t.DATARIENTRO.ToShortDateString()));
            }

            //se sto in modifica la data fine deve essere obbligatoria (solo se la mab è attiva)
            if (inserimentoMAB == false)
            {

                var mab = db.MAB.Find(mvm.idMAB);
                var att = mab.ATTIVAZIONEMAB.Where(a=>a.ANNULLATO==false).OrderByDescending(a=>a.IDATTIVAZIONEMAB).ToList().First();// GetAttivazioneById(mab.IDATTIVAZIONEMAB, db);

                if (inserimentoMAB == false && datafin == null && att.ATTIVAZIONE)
                {
                    throw new Exception("La Data Fine Validità è obbligatoria.");
                }
            }

        }


        public void VerificaDataInizioValiditaPagatoCondivisoMAB_Utente(decimal idMab, DateTime? data, ModelDBISE db)
        {
            if (data == null)
            {
                throw new Exception("La Data Inizio Validità è obbligatoria.");
            }
            var mab = db.MAB.Find(idMab);
            var pmab = mab.PERIODOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDPERIODOMAB).ToList().First();
            if (data < pmab.DATAINIZIOMAB)
            {
                throw new Exception(string.Format("Impossibile inserire la Data Inizio Validità minore della Data Inizio Validità MAB ({0}).", pmab.DATAINIZIOMAB.ToShortDateString()));
            }
            if (data > pmab.DATAFINEMAB)
            {
                throw new Exception(string.Format("Impossibile inserire la Data Inizio Validità maggiore della Data Fine Validità MAB ({0}).", pmab.DATAFINEMAB.ToShortDateString()));
            }
        }

        public void VerificaVariazioniPagatoCondivisoMAB(PagatoCondivisoMABViewModel pcmvm, ModelDBISE db)
        {
            PagatoCondivisoMABModel pcprec = PrelevaMovimentiPagatoCondivisoMABPrecedenti(pcmvm.idMAB, pcmvm.ut_dataInizioValidita.Value, db).ToList().First();

            PAGATOCONDIVISOMAB pcAttiguoPrec = PrelevaMovimentoPagatoCondivisoMABAttiguoPrecedente(pcmvm.idMAB, pcmvm.ut_dataInizioValidita.Value, db);

            IList<PagatoCondivisoMABModel> pcsucc = PrelevaMovimentiPagatoCondivisoMABSuccessivi(pcmvm.idMAB, pcmvm.ut_dataInizioValidita.Value, db).ToList();

            string datafinestring = pcprec.DataFineValidita == Utility.DataFineStop() ? "--/--/----" : pcprec.DataFineValidita.ToShortDateString();
            string datafineAttiguostring = pcAttiguoPrec.DATAFINEVALIDITA == Utility.DataFineStop() ? "--/--/----" : pcAttiguoPrec.DATAFINEVALIDITA.ToShortDateString();

            string periodo = pcprec.DataInizioValidita.ToShortDateString() + " - " + datafinestring;
            string periodoAttiguo = pcAttiguoPrec.DATAINIZIOVALIDITA.ToShortDateString() + " - " + datafineAttiguostring;

            if (pcmvm.chkAggiornaTutti) 
            {
                if (pcsucc.Count() == 1)
                {
                    if (pcprec.Pagato == pcmvm.Pagato && pcprec.Condiviso == pcmvm.Condiviso)
                    {
                        if (pcprec.DataInizioValidita == pcmvm.ut_dataInizioValidita)
                        {
                            throw new Exception(string.Format("Le opzioni selezionate risultano uguali a quelle già impostate per lo stesso periodo ({0}).", periodo));
                        }
                        else
                        {
                            throw new Exception(string.Format("Le opzioni selezionate risultano uguali a quelle del periodo precedente attiguo ({0}).", periodo));
                        }
                    }
                }else
                {
                    if(pcAttiguoPrec.IDPAGATOCONDIVISO>0)
                    {
                        if (pcAttiguoPrec.PAGATO == pcmvm.Pagato && pcAttiguoPrec.CONDIVISO == pcmvm.Condiviso)
                        {
                            throw new Exception(string.Format("Le opzioni selezionate risultano uguali a quelle già impostate per il perido precednte ({0}).", periodoAttiguo));
                        }
                    }
                }

            }
            else
            {
                if (pcprec.Pagato == pcmvm.Pagato && pcprec.Condiviso == pcmvm.Condiviso)
                {
                    if (pcprec.DataInizioValidita == pcmvm.ut_dataInizioValidita)
                    {
                        throw new Exception(string.Format("Le opzioni selezionate risultano uguali a quelle già impostate per lo stesso periodo ({0}).",periodo));
                    }
                    else
                    {
                        throw new Exception(string.Format("Le opzioni selezionate risultano uguali a quelle del periodo precedente attiguo ({0}).",periodo));
                    }
                }
            }
        }

        public MAGGIORAZIONIANNUALI GetMaggiorazioneAnnuale_var(MABViewModel mvm, ModelDBISE db)
        {
            try
            {

                List<MAGGIORAZIONIANNUALI> mal = new List<MAGGIORAZIONIANNUALI>();

                MAGGIORAZIONIANNUALI ma = new MAGGIORAZIONIANNUALI();
                MaggiorazioniAnnualiModel mam = new MaggiorazioniAnnualiModel();
                UfficiModel um = new UfficiModel();

                var t = db.TRASFERIMENTO.Find(mvm.idTrasferimento);
                var u = t.UFFICI;

                mal = u.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false &&
                                                        a.DATAINIZIOVALIDITA <= mvm.dataInizioMAB &&
                                                        a.DATAFINEVALIDITA >= mvm.dataFineMAB)
                                                        .OrderByDescending(a => a.IDMAGANNUALI).ToList();

                if (mal?.Any() ?? false)
                {
                    ma = mal.First();
                }

                return ma;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PERCENTUALEMAB> GetListaPercentualeMAB_var(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                PERCENTUALEMAB p = new PERCENTUALEMAB();
                List<PERCENTUALEMAB> pl = new List<PERCENTUALEMAB>();
                UfficiModel um = new UfficiModel();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var i = t.INDENNITA;
                var mab = i.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDMAB).First();
                var pmab = mab.PERIODOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDPERIODOMAB).First();


                UFFICI u = t.UFFICI;
                DIPENDENTI d = t.DIPENDENTI;
                var l = d.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false).ToList().First();

                um.descUfficio = u.DESCRIZIONEUFFICIO;

                pl = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false &&
                                                    a.DATAINIZIOVALIDITA <= pmab.DATAINIZIOMAB &&
                                                    a.DATAFINEVALIDITA >= pmab.DATAFINEMAB &&
                                                    a.IDUFFICIO == u.IDUFFICIO &&
                                                    a.IDLIVELLO == l.IDLIVELLO).ToList();
                return pl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PERCENTUALECONDIVISIONE> GetListaPercentualeCondivisione_var(DateTime dataIni, DateTime dataFin, ModelDBISE db)
        {
            try
            {

                List<PERCENTUALECONDIVISIONE> lpc = new List<PERCENTUALECONDIVISIONE>();

                lpc = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false &&
                                                        a.DATAINIZIOVALIDITA <= dataIni &&
                                                        a.DATAFINEVALIDITA >= dataFin).ToList();
                return lpc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AttivazioneMABModel GetUltimaAttivazioneMABmodel(decimal idTrasferimento)
        {
            try
            {
                AttivazioneMABModel amm = new AttivazioneMABModel();
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var l_ultimaMAB = t.INDENNITA.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.RINUNCIAMAB==false).OrderByDescending(a => a.IDMAB).ToList();
                    if (l_ultimaMAB?.Any() ?? false)
                    {
                        var ultimaMAB = l_ultimaMAB.First();
                        var aml = ultimaMAB.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList();
                        if(aml?.Any()??false)
                        {
                            am = aml.First();

                            amm = new AttivazioneMABModel()
                            {
                                idAttivazioneMAB = am.IDATTIVAZIONEMAB,
                                idMAB = am.IDMAB,
                                notificaRichiesta = am.NOTIFICARICHIESTA,
                                dataNotificaRichiesta = am.DATANOTIFICARICHIESTA,
                                Attivazione = am.ATTIVAZIONE,
                                dataAttivazione = am.DATAATTIVAZIONE,
                                dataVariazione = am.DATAVARIAZIONE,
                                dataAggiornamento = am.DATAAGGIORNAMENTO,
                                Annullato = am.ANNULLATO
                            };
                        }
                    }
                }

                return amm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ATTIVAZIONEMAB GetUltimaAttivazioneMAB(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();


                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var l_MAB = t.INDENNITA.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.RINUNCIAMAB == false).OrderByDescending(a => a.IDMAB).ToList();
                if (l_MAB?.Any() ?? false)
                {
                    decimal max_id_att = 0;
                    foreach (var mab in l_MAB)
                    {
                        decimal max_id_att_curr = mab.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList().First().IDATTIVAZIONEMAB;
                        if(max_id_att_curr>max_id_att)
                        {
                            max_id_att = max_id_att_curr;
                        }
                    }

                    //var ultimaMAB = l_ultimaMAB.First();
                    //var aml = ultimaMAB.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList();
                    //if (aml?.Any() ?? false)
                    //{
                    am = db.ATTIVAZIONEMAB.Find(max_id_att);
                    
                }

                return am;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ATTIVAZIONEMAB GetUltimaAttivazioneMABCorrente(decimal idMab, ModelDBISE db)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                var mab = db.MAB.Find(idMab);
                if (mab.IDMAB>0)
                {
                    var aml = mab.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList();
                    if (aml?.Any() ?? false)
                    {
                        am = aml.First();
                    }
                }

                return am;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public List<ATTIVAZIONEMAB> GetElencoAttivazioniMABAttive(decimal idTrasferimento, ModelDBISE db)
        //{
        //    try
        //    {
        //        List<ATTIVAZIONEMAB> lam = new List<ATTIVAZIONEMAB>();

        //        var t = db.TRASFERIMENTO.Find(idTrasferimento);

        //        lam = t.ATTIVAZIONEMAB
        //                        .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA && a.ATTIVAZIONE)
        //                        .OrderByDescending(a => a.IDATTIVAZIONEMAB)
        //                        .ToList();



        //        return lam;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        //public List<ATTIVAZIONEMAB> GetElencoAttivazioniMAB(decimal idTrasferimento, ModelDBISE db)
        //{
        //    try
        //    {
        //        List<ATTIVAZIONEMAB> lam = new List<ATTIVAZIONEMAB>();

        //        var t = db.TRASFERIMENTO.Find(idTrasferimento);

        //        lam = t.INDENNITA.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
        //                            .OrderBy(a => a.IDMAB)
        //                            .ToList()
        //                            .First().ATTIVAZIONEMAB
        //                        .Where(a => a.ANNULLATO == false)
        //                        .OrderByDescending(a => a.IDATTIVAZIONEMAB)
        //                        .ToList();



        //        return lam;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public List<MAB> GetElencoMAB(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                List<MAB> lmab = new List<MAB>();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                lmab = t.INDENNITA.MAB
                                .Where(a => 
                                    a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato &&
                                    a.RINUNCIAMAB==false)
                                .OrderByDescending(a => a.IDMAB)
                                .ToList();



                return lmab;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public ATTIVAZIONEMAB CheckAttivazioneMAB(decimal idTrasferimento, ModelDBISE db)
        //{
        //    try
        //    {
        //        ATTIVAZIONEMAB att = new ATTIVAZIONEMAB();

        //        ATTIVAZIONEMAB amab = GetUltimaAttivazioneMAB(idTrasferimento, db);
        //        att = amab;
        //        if (amab.ATTIVAZIONE && amab.NOTIFICARICHIESTA)
        //        {
        //            var att_aperta = GetAttivazioneAperta(att.IDMAB, db);
        //            if (att_aperta.IDATTIVAZIONEMAB > 0)
        //            {
        //                att = att_aperta;
        //            }
        //            else
        //            {
        //                var att_new = CreaAttivazione(att.IDMAB, db);
        //                att = att_new;
        //            }
        //        }
        //        return att;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public ValuteModel GetValutaUfficioModel(decimal idMab)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtValute dtv = new dtValute())
                {

                    ValuteModel vm = new ValuteModel();

                    var mab = db.MAB.Find(idMab);

                    var pmab = GetPeriodoMABModel(idMab, db);

                    var u = mab.INDENNITA.TRASFERIMENTO.UFFICI;
                    var vul = db.VALUTAUFFICIO.Where(a => a.ANNULLATO == false &&
                                    a.IDUFFICIO == u.IDUFFICIO &&
                                    a.DATAINIZIOVALIDITA <= pmab.dataInizioMAB &&
                                    a.DATAFINEVALIDITA >= pmab.dataFineMAB)
                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                    .ToList();



                    if (vul?.Any() ?? false)
                    {
                        var vu = vul.First();

                        vm = dtv.GetValuta(vu.IDVALUTA);
                        if (vm.idValuta <= 0)
                        {
                            throw new Exception("Valuta non trovata.");
                        }
                    }
                    else
                    {
                        throw new Exception("Valuta Ufficio non trovata.");
                    }


                    return vm;
                }
            }
        }

        public ValuteModel GetUltimaValutaInseritaModel(decimal idMab, ModelDBISE db)
        {
            using (dtValute dtv = new dtValute())
            {

                ValuteModel vm = new ValuteModel();

                var mab = db.MAB.Find(idMab);

                var lcmab = mab.CANONEMAB.Where(a => a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato &&
                                                    a.NASCONDI==false)
                                .OrderByDescending(a => a.IDCANONE)
                                .ToList();

                if (lcmab?.Any() ?? false)
                {
                    var cmab = lcmab.First();

                    vm = dtv.GetValuta(cmab.IDVALUTA);
                    if (vm.idValuta <= 0)
                    {
                        throw new Exception("Valuta Canone non trovata.");
                    }
                }
                else
                {
                    throw new Exception("Nessuna canone trovato.");
                }


                return vm;
            }
        }


        public MAB GetUltimaMAB(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                MAB mab = new MAB();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var mabl = t.INDENNITA.MAB.Where(a=> a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a=>a.IDMAB).ToList();

                if (mabl?.Any() ?? false)
                {
                    mab = mabl.First();

                }
                else
                {
                    throw new Exception(string.Format("Nessuna MAB trovata."));
                }

                return mab;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MAB GetUltimaMABAttiva(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                MAB mab = new MAB();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var mabl = t.INDENNITA.MAB
                            .Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                        a.RINUNCIAMAB==false)
                            .OrderByDescending(a => a.IDMAB)
                            .ToList();

                if (mabl?.Any() ?? false)
                {
                    mab = mabl.First();

                }
                    //else
                    //{
                    //    throw new Exception(string.Format("Nessuna MAB trovata."));
                    //}

                return mab;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AnticipoAnnualeMAB(decimal idMAB, ModelDBISE db)
        {
            try
            {
                var mab = GetMAB_ByID_var(idMAB, db);
                var aamabl = mab.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDANTICIPOANNUALEMAB).ToList();

                if (aamabl?.Any() ?? false)
                {
                    return aamabl.First().ANTICIPOANNUALE;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ANTICIPOANNUALEMAB GetAnticipoAnnualeMAB(decimal idMAB, ModelDBISE db)
        {
            try
            {
                ANTICIPOANNUALEMAB aamab = new ANTICIPOANNUALEMAB();

                var mab = GetMAB_ByID_var(idMAB, db);
                var aamabl = mab.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDANTICIPOANNUALEMAB).ToList();

                if (aamabl?.Any() ?? false)
                {
                    return aamabl.First();
                }
                else
                {
                    return aamab;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal VerificaEsistenzaDocumentoMAB_var(decimal idMab, EnumTipoDoc TipoDocumento)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                decimal idDoc = 0;


                using (ModelDBISE db = new ModelDBISE())
                {
                    var mab = db.MAB.Find(idMab);

                    var aml = mab.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false)
                                                .OrderBy(a => a.IDATTIVAZIONEMAB)
                                                .ToList();

                    if (aml?.Any() ?? false)
                    {
                        am = aml.First();

                        var dl = am.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.IDTIPODOCUMENTO == (decimal)TipoDocumento).ToList();
                        if (dl?.Any() ?? false)
                        {
                            if (dl.Count() == 1)
                            {
                                var d = dl.First();

                                idDoc = d.IDDOCUMENTO;
                            }
                            else
                            {
                                throw new Exception(string.Format("Errore in fase di aggiornamento documentazione Maggiorazione Abitazione. Esiste più di un documento del tipo selezionato. Contattare l'amministratore di sistema."));
                            }

                        }

                    }

                    return idDoc;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool VerificaMAB(decimal idAttivazioneMAB)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                bool esisteMAB = false;


                using (ModelDBISE db = new ModelDBISE())
                {
                    var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                    if (amab.IDATTIVAZIONEMAB > 0)
                    {

                        var vmabl = amab.MAB;//.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && a.RINUNCIAMAB == false).ToList();
                        if (vmabl.IDMAB>0)
                        {
                            esisteMAB = true;
                        }
                    }

                    return esisteMAB;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public bool VerificaSeNuovaMAB(decimal idAttivazioneMAB, ModelDBISE db)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                bool nuovaMAB = false;


                var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                if (amab.IDATTIVAZIONEMAB > 0)
                {

                    var mabl = amab.MAB;//.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && a.RINUNCIAMAB == false).OrderByDescending(a=>a.IDMAB).ToList();
                    if (mabl.IDMAB>0)
                    {
                        //se la prima attivazione non è notificata vuol dire che la MAB è stata appena inserita
                        var mab = mabl;
                        var attivazioneMAB = mab.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList().First();
                        if(attivazioneMAB.NOTIFICARICHIESTA==false)
                        {
                            nuovaMAB = true;
                        }
                        //decimal conta_periodi_mab = mab.PERIODOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList().Count();
                        //if(conta_periodi_mab==1)
                        //{
                        //    nuovaMAB = true;
                        //}
                    }

                }
                return nuovaMAB;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool VerificaPagatoCondivisoMAB(decimal idAttivazioneMAB)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                bool esistePagatoCondivisoMAB = false;


                using (ModelDBISE db = new ModelDBISE())
                {
                    var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                    if (amab.IDATTIVAZIONEMAB > 0)
                    {

                        var pcmabl = amab.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                        if (pcmabl?.Any() ?? false)
                        {
                            esistePagatoCondivisoMAB = true;
                        }
                    }

                    return esistePagatoCondivisoMAB;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public void VerificaImportoCanoneMAB(decimal importoCanoneMAB, ModelDBISE db)
        {
            if (importoCanoneMAB<=0)
            {
                throw new Exception(string.Format("il canone MAB inserito ({0}) non è valido.", importoCanoneMAB.ToString()));
            }
        }

        public bool VerificaCanoneMAB(decimal idAttivazioneMAB)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                bool esisteCanoneMAB = false;


                using (ModelDBISE db = new ModelDBISE())
                {
                    var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                    if (amab.IDATTIVAZIONEMAB > 0)
                    {

                        var cmabl = amab.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                        if (cmabl?.Any() ?? false)
                        {
                            esisteCanoneMAB = true;
                        }
                    }

                    return esisteCanoneMAB;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public bool VerificaPeriodoMAB(decimal idAttivazioneMAB)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                bool esistePeriodoMAB = false;


                using (ModelDBISE db = new ModelDBISE())
                {
                    var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                    if (amab.IDATTIVAZIONEMAB > 0)
                    {

                        var pmabl = amab.PERIODOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                        if (pmabl?.Any() ?? false)
                        {
                            esistePeriodoMAB = true;
                        }
                    }

                    return esistePeriodoMAB;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void VerificaDocumenti_var(ATTIVAZIONEMAB am_curr,
                                            out bool siDocCopiaContratto,
                                            out bool siDocCopiaRicevuta,
                                            out bool siDocModulo1,
                                            out bool siDocModulo2,
                                            out bool siDocModulo3,
                                            out bool siDocModulo4,
                                            out bool siDocModulo5,
                                            out decimal idDocCopiaContratto,
                                            out decimal idDocCopiaRicevuta,
                                            out decimal idDocModulo1,
                                            out decimal idDocModulo2,
                                            out decimal idDocModulo3,
                                            out decimal idDocModulo4,
                                            out decimal idDocModulo5)
        {
            siDocCopiaContratto = false;
            siDocCopiaRicevuta = false;
            siDocModulo1 = false;
            siDocModulo2 = false;
            siDocModulo3 = false;
            siDocModulo4 = false;
            siDocModulo5 = false;
            idDocCopiaContratto = 0;
            idDocCopiaRicevuta = 0;
            idDocModulo1 = 0;
            idDocModulo2 = 0;
            idDocModulo3 = 0;
            idDocModulo4 = 0;
            idDocModulo5 = 0;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    //var am = db.ATTIVAZIONEMAB.Find(am_curr.IDATTIVAZIONEMAB);
                    //if (am.IDATTIVAZIONEMAB > 0)
                    //{
                        var docl = am_curr.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione || a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();

                        if (docl?.Any() ?? false)
                        {
                            foreach (var doc in docl)
                            {
                                switch ((EnumTipoDoc)doc.IDTIPODOCUMENTO)
                                {
                                    case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                                        siDocModulo1 = true;
                                        idDocModulo1 = doc.IDDOCUMENTO;
                                        break;
                                    case EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione:
                                        siDocModulo2 = true;
                                        idDocModulo2 = doc.IDDOCUMENTO;
                                        break;
                                    case EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore:
                                        siDocModulo3 = true;
                                        idDocModulo3 = doc.IDDOCUMENTO;
                                        break;
                                    case EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione:
                                        siDocModulo4 = true;
                                        idDocModulo4 = doc.IDDOCUMENTO;
                                        break;
                                    case EnumTipoDoc.Clausole_Contratto_Alloggio:
                                        siDocModulo5 = true;
                                        idDocModulo5 = doc.IDDOCUMENTO;
                                        break;
                                    case EnumTipoDoc.Copia_Contratto_Locazione:
                                        siDocCopiaContratto = true;
                                        idDocCopiaContratto = doc.IDDOCUMENTO;
                                        break;
                                    case EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione:
                                        siDocCopiaRicevuta = true;
                                        idDocCopiaRicevuta = doc.IDDOCUMENTO;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    //}
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void VerificaDocumentiValidi_var(ATTIVAZIONEMAB am_curr,
                                            out bool siDocCopiaContratto,
                                            out bool siDocCopiaRicevuta,
                                            out bool siDocModulo1,
                                            out bool siDocModulo2,
                                            out bool siDocModulo3,
                                            out bool siDocModulo4,
                                            out bool siDocModulo5,
                                            out decimal idDocCopiaContratto,
                                            out decimal idDocCopiaRicevuta,
                                            out decimal idDocModulo1,
                                            out decimal idDocModulo2,
                                            out decimal idDocModulo3,
                                            out decimal idDocModulo4,
                                            out decimal idDocModulo5)
        {
            siDocCopiaContratto = false;
            siDocCopiaRicevuta = false;
            siDocModulo1 = false;
            siDocModulo2 = false;
            siDocModulo3 = false;
            siDocModulo4 = false;
            siDocModulo5 = false;
            idDocCopiaContratto = 0;
            idDocCopiaRicevuta = 0;
            idDocModulo1 = 0;
            idDocModulo2 = 0;
            idDocModulo3 = 0;
            idDocModulo4 = 0;
            idDocModulo5 = 0;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    //var am = db.ATTIVAZIONEMAB.Find(am_curr.IDATTIVAZIONEMAB);
                    //if (am.IDATTIVAZIONEMAB > 0)
                    //{
                    var docl = am_curr.DOCUMENTI.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.In_Lavorazione).ToList();

                    if (docl?.Any() ?? false)
                    {
                        foreach (var doc in docl)
                        {
                            switch ((EnumTipoDoc)doc.IDTIPODOCUMENTO)
                            {
                                case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                                    siDocModulo1 = true;
                                    idDocModulo1 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione:
                                    siDocModulo2 = true;
                                    idDocModulo2 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore:
                                    siDocModulo3 = true;
                                    idDocModulo3 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione:
                                    siDocModulo4 = true;
                                    idDocModulo4 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.Clausole_Contratto_Alloggio:
                                    siDocModulo5 = true;
                                    idDocModulo5 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.Copia_Contratto_Locazione:
                                    siDocCopiaContratto = true;
                                    idDocCopiaContratto = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione:
                                    siDocCopiaRicevuta = true;
                                    idDocCopiaRicevuta = doc.IDDOCUMENTO;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    //}
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CANONEMAB GetUltimoCanoneMAB_var(MABModel mabm)
        {
            try
            {
                CANONEMAB cm = new CANONEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var mab = db.MAB.Find(mabm.idMAB);
                    var cml = mab.CANONEMAB.Where(X => X.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a=>a.IDCANONE).ToList();

                    if (cml?.Any() ?? false)
                    {
                        var cm_row = cml.First();

                        cm = new CANONEMAB()
                        {
                            IDCANONE = cm_row.IDCANONE,
                            IDATTIVAZIONEMAB = cm_row.IDATTIVAZIONEMAB,
                            IDMAB = cm_row.IDMAB,
                            DATAINIZIOVALIDITA = cm_row.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = cm_row.DATAFINEVALIDITA,
                            IMPORTOCANONE = cm_row.IMPORTOCANONE,
                            DATAAGGIORNAMENTO = cm_row.DATAAGGIORNAMENTO,
                            IDSTATORECORD = cm_row.IDSTATORECORD,
                            FK_IDCANONE = cm_row.FK_IDCANONE,
                            IDVALUTA = cm_row.IDVALUTA
                        };

                    }

                }

                return cm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ATTIVAZIONEMAB CreaAttivazioneMAB_var(decimal idMab, ModelDBISE db)
        {
            ATTIVAZIONEMAB new_am = new ATTIVAZIONEMAB()
            {
                IDMAB = idMab,
                NOTIFICARICHIESTA = false,
                DATAATTIVAZIONE = null,
                ATTIVAZIONE = false,
                DATANOTIFICARICHIESTA = null,
                ANNULLATO = false,
                DATAVARIAZIONE = DateTime.Now,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVAZIONEMAB.Add(new_am);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per la maggiorazione abitazione."));
            }

            var mab = db.MAB.Find(idMab);

            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione maggiorazione abitazione.", "ATTIVITAZIONEMAB", db, mab.IDTRASFINDENNITA, new_am.IDATTIVAZIONEMAB);

            return new_am;
        }


        public MABModel GetUltimaMABModel(decimal idTrasferimento)
        {
            try
            {
                MABModel mabm = new MABModel();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var mabl = t.INDENNITA.MAB
                                .Where(a=>a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato && a.RINUNCIAMAB == false)
                                .OrderByDescending(x => x.IDMAB).ToList();

                    if (mabl?.Any() ?? false)
                    {
                        MAB mab = mabl.First();

                        mabm = new MABModel()
                        {
                            idMAB = mab.IDMAB,
                            idTrasfIndennita = mab.IDTRASFINDENNITA,
                            idStatoRecord = mab.IDSTATORECORD,
                            rinunciaMAB = mab.RINUNCIAMAB,
                            dataAggiornamento = mab.DATAAGGIORNAMENTO
                        };
                    }
                }

                return mabm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public bool VerificaVariazioniMAB(decimal idMab, ModelDBISE db, bool xNotifica)
        {
            try
            {

                var mab = GetMAB_ByID_var(idMab,db);

                //var i = t.INDENNITA;

                //var mabl = i.MAB.Where(a =>
                //        a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                //        a.RINUNCIAMAB == false).ToList();

                if (mab.IDMAB>0)
                {
                    //var idAttivazioneMAB = mab.ATTIVAZIONEMAB.;

                    //VERIFICA MAB
                    bool esisteMAB = (mab.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione || mab.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare) ? true : false;

                    //verifica Periodo MAB
                    var lPeriodoMAB = mab.PERIODOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione || a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();


                    //verifica Pagato Condiviso MAB
                    var lPagatoCondivisoMAB = mab.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione || a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();

                    //verifica Canone MAB
                    var lCanoneMAB = mab.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione || a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();

                    //verifica se nuova MAB
                    var att = mab.ATTIVAZIONEMAB.Where(a => a.ANNULLATO==false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList().First();
                    bool nuovaMAB = VerificaSeNuovaMAB(att.IDATTIVAZIONEMAB, db);

                    //verifica documenti
                    bool siDocCopiaContratto;
                    bool siDocCopiaRicevuta;
                    bool siDocModulo1;
                    bool siDocModulo2;
                    bool siDocModulo3;
                    bool siDocModulo4;
                    bool siDocModulo5;
                    decimal idDocCopiaContratto;
                    decimal idDocCopiaRicevuta;
                    decimal idDocModulo1;
                    decimal idDocModulo2;
                    decimal idDocModulo3;
                    decimal idDocModulo4;
                    decimal idDocModulo5;

                    var ultima_att= GetUltimaAttivazioneMABCorrente(idMab, db);
                    VerificaDocumenti_var(ultima_att,
                            out siDocCopiaContratto,
                            out siDocCopiaRicevuta,
                            out siDocModulo1,
                            out siDocModulo2,
                            out siDocModulo3,
                            out siDocModulo4,
                            out siDocModulo5,
                            out idDocCopiaContratto,
                            out idDocCopiaRicevuta,
                            out idDocModulo1,
                            out idDocModulo2,
                            out idDocModulo3,
                            out idDocModulo4,
                            out idDocModulo5);

                    if (nuovaMAB && xNotifica)
                    {
                        if (lPeriodoMAB.Count() > 0 &&
                            lPagatoCondivisoMAB.Count() > 0 &&
                            lCanoneMAB.Count() > 0 &&
                            siDocModulo1)
                        {
                            return true;
                        }
                    }
                    else
                    {

                        if (lCanoneMAB.Count() > 0 ||
                            lPagatoCondivisoMAB.Count() > 0 ||
                            esisteMAB ||
                            lPeriodoMAB.Count() > 0 ||
                            siDocCopiaContratto ||
                            siDocCopiaRicevuta ||
                            siDocModulo1 ||
                            siDocModulo2 ||
                            siDocModulo3 ||
                            siDocModulo4 ||
                            siDocModulo5
                            )
                        {
                            return true;
                        }
                    }
                }

                return false;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<MABModel> GetMABNonAttiveModel(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                List<MABModel> lmabInLavm = new List<MABModel>();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                var i = t.INDENNITA;

                var mabl = i.MAB.Where(a =>
                        a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && 
                        a.RINUNCIAMAB == false).ToList();

                if (mabl?.Any() ?? false)
                {
                    foreach (var mab in mabl)
                    {
                        var pmab = GetPeriodoMABModel(mab.IDMAB, db);
                        if((pmab.dataInizioMAB<=t.DATARIENTRO && pmab.dataFineMAB>=t.DATARIENTRO) || pmab.dataFineMAB<t.DATARIENTRO)
                        { 
                            bool esistonoVariazioni = VerificaVariazioniMAB(mab.IDMAB, db, false);

                            if (esistonoVariazioni)
                            {
                                MABModel mabInLavm = new MABModel()
                                {
                                    idMAB = mab.IDMAB,
                                    idTrasfIndennita = mab.IDTRASFINDENNITA,
                                    idStatoRecord = mab.IDSTATORECORD,
                                    rinunciaMAB = mab.RINUNCIAMAB,
                                    dataAggiornamento = mab.DATAAGGIORNAMENTO
                                };
                                lmabInLavm.Add(mabInLavm);

                                return lmabInLavm;
                            }
                        }
                    }
                }

                return lmabInLavm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<MABModel> GetElencoMABModel(decimal idTrasferimento)
        {
            try
            {
                List<MABModel> lmabm = new List<MABModel>();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    //var mal = db.ATTIVAZIONEMAB.Find(amm.idAttivazioneMAB).TRASFERIMENTO.MAGGIORAZIONEABITAZIONE.OrderByDescending(x => x.IDMAB).ToList();
                    var i = t.INDENNITA;

                    var mabl = i.MAB.Where(a => 
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.RINUNCIAMAB==false).ToList();

                    if (mabl?.Any() ?? false)
                    {
                        foreach (var mabm in mabl)
                        {
                            var pmab = GetPeriodoMABModel(mabm.IDMAB, db);
                            if ((pmab.dataInizioMAB <= t.DATARIENTRO && pmab.dataFineMAB >= t.DATARIENTRO) || pmab.dataFineMAB < t.DATARIENTRO)
                            {
                                MABModel mabm_new = new MABModel()
                                {
                                    idMAB = mabm.IDMAB,
                                    idTrasfIndennita = mabm.IDTRASFINDENNITA,
                                    idStatoRecord = mabm.IDSTATORECORD,
                                    rinunciaMAB = mabm.RINUNCIAMAB,
                                    dataAggiornamento = mabm.DATAAGGIORNAMENTO
                                };
                                lmabm.Add(mabm_new);
                            }
                        }
                    }
                }

                return lmabm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PERIODOMAB GetPeriodoMAB(decimal idMab, ModelDBISE db)
        {
            try
            {
                PERIODOMAB pm = new PERIODOMAB();

                var m = db.MAB.Find(idMab);

                var pml = m.PERIODOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDPERIODOMAB).ToList();

                if (pml?.Any() ?? false)
                {
                    pm = pml.First();
                }

                return pm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PERIODOMAB GetPeriodoMABPrecedente(decimal idMab, ModelDBISE db)
        {
            try
            {
                PERIODOMAB pmab_prec = new PERIODOMAB();

                var m = db.MAB.Find(idMab);

                var i = db.INDENNITA.Find(m.IDTRASFINDENNITA);

                var lmab_prec = i.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                 a.RINUNCIAMAB==false && 
                                                 a.IDMAB < idMab).OrderByDescending(a => a.IDMAB).ToList();

                if (lmab_prec?.Any() ?? false)
                {
                    var mab_prec = lmab_prec.First();

                    pmab_prec = GetPeriodoMAB(mab_prec.IDMAB, db);
                }

                return pmab_prec;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PeriodoMABModel GetPeriodoMABModel(decimal idMab, ModelDBISE db)
        {
            try
            {
                PeriodoMABModel pmm = new PeriodoMABModel();

                var m = db.MAB.Find(idMab);

                var pml = m.PERIODOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDPERIODOMAB).ToList();

                if (pml?.Any() ?? false)
                {
                    var pm = pml.First();

                    pmm = new PeriodoMABModel()
                    {
                        idPeriodoMAB = pm.IDPERIODOMAB,
                        idMAB = pm.IDMAB,
                        idAttivazioneMAB = pm.IDATTIVAZIONEMAB,
                        idStatoRecord = pm.IDSTATORECORD,
                        dataInizioMAB = pm.DATAINIZIOMAB,
                        dataFineMAB = pm.DATAFINEMAB,
                        dataAggiornamento = pm.DATAAGGIORNAMENTO,
                        FK_idPeriodoMAB = pm.FK_IDPERIODOMAB
                    };
                }

                return pmm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PERIODOMAB VerificaEsistenzaMABSuccessiva(decimal idMab, ModelDBISE db)
        {
            try
            {
                
                PERIODOMAB pmab_successivo = new PERIODOMAB();

                var m = db.MAB.Find(idMab);
                var pmab_curr = GetPeriodoMAB(m.IDMAB, db);

                var t = m.INDENNITA.TRASFERIMENTO;
                var lmab = t.INDENNITA.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                foreach(var mab in lmab)
                {
                    var pmab = GetPeriodoMAB(mab.IDMAB,db);
                    if(pmab.DATAINIZIOMAB>pmab_curr.DATAFINEMAB)
                    {
                        pmab_successivo = pmab;
                    }
                }

                return pmab_successivo;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ATTIVAZIONEMAB CreaAttivazione(decimal idMab, ModelDBISE db)
        {
            ATTIVAZIONEMAB new_amab = new ATTIVAZIONEMAB()
            {
                IDMAB = idMab,
                NOTIFICARICHIESTA = false,
                DATANOTIFICARICHIESTA = null,
                ATTIVAZIONE = false,
                DATAATTIVAZIONE = null,
                ANNULLATO = false,
                DATAVARIAZIONE = DateTime.Now,
                DATAAGGIORNAMENTO = DateTime.Now
            };
            db.ATTIVAZIONEMAB.Add(new_amab);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per la Maggiorazione Abitazione."));
            }
            return new_amab;
        }

        public IList<AttivazioneMABModel> GetListaAttivazioniMABconDocumentiModel(decimal idMab, ModelDBISE db)
        {
            List<AttivazioneMABModel> lamabm = new List<AttivazioneMABModel>();

            var mab = db.MAB.Find(idMab);

            var lamab = mab.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList();
            foreach (var amab in lamab)
            {
                var ldoc = amab.DOCUMENTI.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.MODIFICATO == false).ToList();
                if (ldoc.Count() > 0)
                {
                    var amabm = new AttivazioneMABModel()
                    {
                        idAttivazioneMAB = amab.IDATTIVAZIONEMAB,
                        idMAB = amab.IDMAB,
                        Attivazione = amab.ATTIVAZIONE,
                        dataAttivazione = amab.DATAATTIVAZIONE,
                        notificaRichiesta = amab.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = amab.DATANOTIFICARICHIESTA,
                        dataAggiornamento = amab.DATAAGGIORNAMENTO,
                        Annullato = amab.ANNULLATO,
                        dataVariazione = amab.DATAVARIAZIONE
                    };
                    lamabm.Add(amabm);
                }
            }

            return lamabm;
        }



        public ATTIVAZIONEMAB GetAttivazioneAperta(decimal idTrasferimento, ModelDBISE db)
        {
            ATTIVAZIONEMAB attmab = new ATTIVAZIONEMAB();

            var t = db.TRASFERIMENTO.Find(idTrasferimento);
            var lmab = t.INDENNITA.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.RINUNCIAMAB==false).OrderByDescending(a => a.IDMAB).ToList();
            if (lmab?.Any() ?? false)
            {
                var mab = lmab.First();
                var attmabl = mab.ATTIVAZIONEMAB.Where(x => x.ANNULLATO == false && x.NOTIFICARICHIESTA == false).OrderByDescending(x => x.IDATTIVAZIONEMAB).ToList();
                if (attmabl?.Any() ?? false)
                {
                    attmab = attmabl.First();
                }
            }
            return attmab;
        }

        public ATTIVAZIONEMAB GetAttivazioneById(decimal idAttivazione, ModelDBISE db)
        {
            ATTIVAZIONEMAB attmab = new ATTIVAZIONEMAB();

            attmab = db.ATTIVAZIONEMAB.Find(idAttivazione);
            return attmab;
        }

        public void VerificaDataInizioCanoneMAB(decimal idMab, DateTime dataInizioCanone)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.MAB.Find(idMab).INDENNITA.TRASFERIMENTO;

                if (dataInizioCanone < t.DATAPARTENZA)
                {
                    throw new Exception(string.Format("La data d'inizio validità per il canone non può essere inferiore alla data di partenza del trasferimento ({0}).", t.DATAPARTENZA.ToShortDateString()));
                }
            }
        }

        private IList<CanoneMABModel> PrelevaMovimentiCanoneMABPrecedenti(decimal idMab, DateTime dtIni, ModelDBISE db)
        {
            List<CanoneMABModel> lcmabm = new List<CanoneMABModel>();

            var lcmab =
                db.MAB.Find(idMab)
                    .CANONEMAB.Where(
                        a =>
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.NASCONDI == false &&
                            a.DATAINIZIOVALIDITA <= dtIni)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                    .ToList();


            if (lcmab?.Any() ?? false)
            {
                lcmabm = (from e in lcmab
                        select new CanoneMABModel()
                        {
                            idCanone = e.IDCANONE,
                            IDMAB=e.IDMAB,
                            IDAttivazioneMAB=e.IDATTIVAZIONEMAB,
                            idValuta=e.IDVALUTA,
                            ImportoCanone = e.IMPORTOCANONE,
                            DataInizioValidita = e.DATAINIZIOVALIDITA,
                            DataFineValidita = e.DATAFINEVALIDITA,
                            DataAggiornamento = e.DATAAGGIORNAMENTO,
                            idStatoRecord = e.IDSTATORECORD,
                            FK_IDCanone = e.FK_IDCANONE,
                            nascondi = e.NASCONDI
                        }).ToList();
            }

            return lcmabm;
        }

        private IList<CanoneMABModel> PrelevaMovimentiCanoneMABSuccessivi(decimal idMab, DateTime dtIni, ModelDBISE db)
        {
            List<CanoneMABModel> lcmabm = new List<CanoneMABModel>();

            var lcmab =
                db.MAB.Find(idMab)
                    .CANONEMAB.Where(
                        a =>
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.NASCONDI == false &&
                            a.DATAINIZIOVALIDITA > dtIni)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();


            if (lcmab?.Any() ?? false)
            {
                lcmabm = (from e in lcmab
                        select new CanoneMABModel()
                        {
                            idCanone = e.IDCANONE,
                            IDMAB = e.IDMAB,
                            IDAttivazioneMAB = e.IDATTIVAZIONEMAB,
                            idValuta = e.IDVALUTA,
                            ImportoCanone = e.IMPORTOCANONE,
                            DataInizioValidita = e.DATAINIZIOVALIDITA,
                            DataFineValidita = e.DATAFINEVALIDITA,
                            DataAggiornamento = e.DATAAGGIORNAMENTO,
                            idStatoRecord = e.IDSTATORECORD,
                            FK_IDCanone = e.FK_IDCANONE,
                            nascondi = e.NASCONDI
                        }).ToList();
            }

            return lcmabm;
        }


        private IList<PagatoCondivisoMABModel> PrelevaMovimentiPagatoCondivisoMABPrecedenti(decimal idMab, DateTime dtIni, ModelDBISE db)
        {
            List<PagatoCondivisoMABModel> lpcmabm = new List<PagatoCondivisoMABModel>();

            var lpcmab =
                db.MAB.Find(idMab)
                    .PAGATOCONDIVISOMAB.Where(
                        a =>
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.NASCONDI == false &&
                            a.DATAINIZIOVALIDITA <= dtIni)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                    .ToList();


            if (lpcmab?.Any() ?? false)
            {
                lpcmabm = (from e in lpcmab
                          select new PagatoCondivisoMABModel()
                          {
                              idPagatoCondiviso = e.IDPAGATOCONDIVISO,
                              idMAB = e.IDMAB,
                              idAttivazioneMAB = e.IDATTIVAZIONEMAB,
                              idStatoRecord = e.IDSTATORECORD,
                              Condiviso = e.CONDIVISO,
                              Pagato = e.PAGATO,
                              DataInizioValidita = e.DATAINIZIOVALIDITA,
                              DataFineValidita = e.DATAFINEVALIDITA,
                              DataAggiornamento = e.DATAAGGIORNAMENTO,
                              fk_IDPagatoCondiviso = e.FK_IDPAGATOCONDIVISO,
                              Nascondi = e.NASCONDI
                          }).ToList();
            }

            return lpcmabm;
        }

        private PAGATOCONDIVISOMAB PrelevaMovimentoPagatoCondivisoMABAttiguoPrecedente(decimal idMab, DateTime dtIni, ModelDBISE db)
        {
            PAGATOCONDIVISOMAB pcmabm = new PAGATOCONDIVISOMAB();

            var lpcmab =
                db.MAB.Find(idMab)
                    .PAGATOCONDIVISOMAB.Where(
                        a =>
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.NASCONDI == false &&
                            a.DATAINIZIOVALIDITA < dtIni)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                    .ToList();


            if (lpcmab?.Any() ?? false)
            {
                pcmabm = lpcmab.First();
            }

            return pcmabm;
        }

        private IList<PagatoCondivisoMABModel> PrelevaMovimentiPagatoCondivisoMABSuccessivi(decimal idMab, DateTime dtIni, ModelDBISE db)
        {
            List<PagatoCondivisoMABModel> lpcmabm = new List<PagatoCondivisoMABModel>();

            var lpcmab =
                db.MAB.Find(idMab)
                    .PAGATOCONDIVISOMAB.Where(
                        a =>
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.NASCONDI == false &&
                            a.DATAINIZIOVALIDITA > dtIni)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();


            if (lpcmab?.Any() ?? false)
            {
                lpcmabm = (from e in lpcmab
                          select new PagatoCondivisoMABModel()
                          {
                              idPagatoCondiviso = e.IDPAGATOCONDIVISO,
                              idMAB = e.IDMAB,
                              idAttivazioneMAB = e.IDATTIVAZIONEMAB,
                              idStatoRecord = e.IDSTATORECORD,
                              Condiviso = e.CONDIVISO,
                              Pagato = e.PAGATO,
                              DataInizioValidita = e.DATAINIZIOVALIDITA,
                              DataFineValidita = e.DATAFINEVALIDITA,
                              DataAggiornamento = e.DATAAGGIORNAMENTO,
                              fk_IDPagatoCondiviso = e.FK_IDPAGATOCONDIVISO,
                              Nascondi = e.NASCONDI
                          }).ToList();
            }

            return lpcmabm;
        }

        public void SetCanoneMAB(ref CanoneMABModel cmabm, ModelDBISE db)
        {
            try
            {
                var mab = db.MAB.Find(cmabm.IDMAB);
                var item = db.Entry<MAB>(mab);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.CANONEMAB).Load();
                CANONEMAB cmab = new CANONEMAB()
                {
                    IDMAB=mab.IDMAB,
                    IDATTIVAZIONEMAB=cmabm.IDAttivazioneMAB,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                    IDVALUTA=cmabm.idValuta,
                    IMPORTOCANONE = cmabm.ImportoCanone,
                    DATAINIZIOVALIDITA = cmabm.DataInizioValidita,
                    DATAFINEVALIDITA = cmabm.DataFineValidita,
                    DATAAGGIORNAMENTO = cmabm.DataAggiornamento,
                };

                mab.CANONEMAB.Add(cmab);

                int i = db.SaveChanges();

                if (i > 0)
                {
                    cmabm.idCanone = cmab.IDCANONE;

                    #region associa TFR
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        //var t = mab.MAGGIORAZIONEABITAZIONE.INDENNITA.TRASFERIMENTO;
                        var t = mab.INDENNITA.TRASFERIMENTO;
                        var trm = dtt.GetTrasferimentoById(t.IDTRASFERIMENTO);
                        List<TFRModel> ltfrm = new List<TFRModel>();

                        using (dtTFR dtTfr = new dtTFR())
                        {
                            ltfrm = dtTfr.GetListaTfrByValuta_RangeDate(trm, cmab.IDVALUTA, cmab.DATAINIZIOVALIDITA, cmab.DATAFINEVALIDITA, db);
                        }

                        foreach (var tfrm in ltfrm)
                        {
                            Associa_TFR_CanoneMAB_var(tfrm.idTFR, cmab.IDCANONE, db);
                        }
                    }
                    #endregion


                    decimal idTrasferimento = cmab.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un importo canone", "CANONEMAB", db, idTrasferimento, cmab.IDCANONE);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetPagatoCondivisoMAB(ref PagatoCondivisoMABModel pcmabm, ModelDBISE db)
        {
            try
            {
                var mab = db.MAB.Find(pcmabm.idMAB);
                var item = db.Entry<MAB>(mab);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PAGATOCONDIVISOMAB).Load();
                PAGATOCONDIVISOMAB pcmab = new PAGATOCONDIVISOMAB()
                {
                    IDMAB = mab.IDMAB,
                    IDATTIVAZIONEMAB = pcmabm.idAttivazioneMAB,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                    PAGATO = pcmabm.Pagato,
                    CONDIVISO = pcmabm.Condiviso,
                    DATAINIZIOVALIDITA = pcmabm.DataInizioValidita,
                    DATAFINEVALIDITA = pcmabm.DataFineValidita,
                    DATAAGGIORNAMENTO = pcmabm.DataAggiornamento,
                    FK_IDPAGATOCONDIVISO=pcmabm.fk_IDPagatoCondiviso,
                    NASCONDI=pcmabm.Nascondi
                };

                mab.PAGATOCONDIVISOMAB.Add(pcmab);

                int i = db.SaveChanges();

                if (i > 0)
                {
                    pcmabm.idPagatoCondiviso = pcmab.IDPAGATOCONDIVISO;

                    #region associa percentuale condivisione
                    List<PERCENTUALECONDIVISIONE> lperccond = new List<PERCENTUALECONDIVISIONE>();

                    lperccond = GetListaPercentualeCondivisione_var(pcmab.DATAINIZIOVALIDITA, pcmab.DATAFINEVALIDITA, db);

                    foreach (var perccond in lperccond)
                    {
                        Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(pcmabm.idPagatoCondiviso, perccond.IDPERCCOND, db);
                    }
                    #endregion


                    decimal idTrasferimento = pcmab.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un pagato condiviso", "PAGATOCONDIVISOMAB", db, idTrasferimento, pcmab.IDPAGATOCONDIVISO);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SetNuovoImportoCanoneMAB(CanoneMABModel cmabm, decimal idMab, decimal idAttivazioneMAB, DateTime dataFineValiditaMAB, ModelDBISE db)
        {

            CanoneMABModel cmabmPrecedente = new CanoneMABModel();
            CanoneMABModel cmabmSuccessivo = new CanoneMABModel();
            CanoneMABModel cmabmLav = new CanoneMABModel();
            List<CanoneMABModel> lcmabmPrecedenti = new List<CanoneMABModel>();
            List<CanoneMABModel> lcmabmSuccessivi = new List<CanoneMABModel>();

            try
            {
                lcmabmPrecedenti = PrelevaMovimentiCanoneMABPrecedenti(idMab, cmabm.DataInizioValidita, db).ToList();

                lcmabmSuccessivi = PrelevaMovimentiCanoneMABSuccessivi(idMab, cmabm.DataInizioValidita, db).ToList();

                cmabmPrecedente = lcmabmPrecedenti.First();

                if (lcmabmSuccessivi.Count == 0)
                {
                    if (cmabmPrecedente.DataInizioValidita == cmabm.DataInizioValidita)
                    {
                        if (cmabmPrecedente.idStatoRecord == (decimal)EnumStatoRecord.In_Lavorazione)
                        {
                            #region edit record
                            var cmabPrecedente = db.CANONEMAB.Find(cmabmPrecedente.idCanone);
                            cmabPrecedente.IMPORTOCANONE = cmabm.ImportoCanone;
                            cmabPrecedente.IDVALUTA = cmabm.idValuta;
                            cmabPrecedente.DATAFINEVALIDITA = dataFineValiditaMAB;
                            cmabPrecedente.DATAAGGIORNAMENTO = DateTime.Now;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore durante l'inserimento del canone.");
                            }
                            #endregion
                        }
                        else
                        {
                            #region nascondo il record
                            cmabmPrecedente.NascondiRecord(db);
                            #endregion

                            #region creo record
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabm.idValuta,
                                ImportoCanone = cmabm.ImportoCanone,
                                DataInizioValidita = cmabm.DataInizioValidita,
                                DataFineValidita = dataFineValiditaMAB,
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                FK_IDCanone = cmabm.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion
                        }

                    }
                    else
                    {
                        if (cmabmPrecedente.DataFineValidita == dataFineValiditaMAB)
                        {
                            #region replico record e lo nascondo
                            cmabmPrecedente.NascondiRecord(db);

                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabmPrecedente.idValuta,
                                ImportoCanone = cmabmPrecedente.ImportoCanone,
                                DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                DataFineValidita = cmabm.DataInizioValidita.AddDays(-1),
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                FK_IDCanone = cmabmPrecedente.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion

                            #region creo record
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabm.idValuta,
                                ImportoCanone = cmabm.ImportoCanone,
                                DataInizioValidita = cmabm.DataInizioValidita,
                                DataFineValidita = dataFineValiditaMAB,
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                FK_IDCanone = cmabm.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion
                        }
                        else
                        {
                            #region creo record
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabm.idValuta,
                                ImportoCanone = cmabm.ImportoCanone,
                                DataInizioValidita = cmabm.DataInizioValidita,
                                DataFineValidita = dataFineValiditaMAB,
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                FK_IDCanone = cmabm.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion
                        }
                    }
                }

                else
                {
                    if (cmabmPrecedente.DataInizioValidita == cmabm.DataInizioValidita &&
                        cmabmPrecedente.idStatoRecord == (decimal)EnumStatoRecord.In_Lavorazione)
                    {
                        #region edit record
                        var cmabPrecedente = db.CANONEMAB.Find(cmabmPrecedente.idCanone);
                        cmabPrecedente.IMPORTOCANONE = cmabm.ImportoCanone;
                        cmabPrecedente.IDVALUTA = cmabm.idValuta;
                        cmabPrecedente.DATAAGGIORNAMENTO = DateTime.Now;
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore durante l'inserimento del canone.");
                        }
                        #endregion
                    }
                    else
                    {
                        cmabmSuccessivo = lcmabmSuccessivi.First();

                        //controllo periodo attiguo
                        if (cmabmPrecedente.DataFineValidita == cmabmSuccessivo.DataInizioValidita.AddDays(-1))
                        {
                            #region replico record e lo nascondo
                            cmabmPrecedente.NascondiRecord(db);

                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabmPrecedente.idValuta,
                                ImportoCanone = cmabmPrecedente.ImportoCanone,
                                DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                DataFineValidita = cmabm.DataInizioValidita.AddDays(-1),
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                FK_IDCanone = cmabmPrecedente.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion

                            if (cmabm.ImportoCanone != cmabmSuccessivo.ImportoCanone)
                            {
                                #region se dati variati rispetto a succ creo record da dataini a dataini-1(succ)                                cmabmLav = new CanoneMABModel()
                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB = cmabm.IDMAB,
                                    IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                    idValuta = cmabm.idValuta,
                                    ImportoCanone = cmabm.ImportoCanone,
                                    DataInizioValidita = cmabm.DataInizioValidita,
                                    DataFineValidita = cmabmSuccessivo.DataInizioValidita.AddDays(-1),
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDCanone = cmabm.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion
                            }
                            else
                            {
                                #region se dati non variati rispetto a succ creo record da dataini a datafine(succ)
                                cmabmSuccessivo.NascondiRecord(db);

                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB = cmabm.IDMAB,
                                    IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                    idValuta = cmabm.idValuta,
                                    ImportoCanone = cmabm.ImportoCanone,
                                    DataInizioValidita = cmabm.DataInizioValidita,
                                    DataFineValidita = cmabmSuccessivo.DataFineValidita,
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDCanone = cmabm.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion
                            }
                        }
                        else
                        {
                            cmabmPrecedente.NascondiRecord(db);
                            #region replico record
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabmPrecedente.idValuta,
                                ImportoCanone = cmabm.ImportoCanone,
                                DataInizioValidita = cmabmSuccessivo.DataFineValidita,
                                DataFineValidita = cmabm.DataInizioValidita.AddDays(-1),
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                FK_IDCanone = cmabm.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion

                            #region creo record
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabm.idValuta,
                                ImportoCanone = cmabm.ImportoCanone,
                                DataInizioValidita = cmabm.DataInizioValidita,
                                DataFineValidita = cmabmPrecedente.DataFineValidita,
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                FK_IDCanone = cmabm.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetNuovoImportoCanone_AggiornaTutti(CanoneMABModel cmabm, decimal idMab, decimal idAttivazioneMAB, DateTime dataFineValiditaMAB, ModelDBISE db)
        {

            CanoneMABModel cmabmPrecedente = new CanoneMABModel();
            CanoneMABModel cmabmSuccessivo = new CanoneMABModel();
            CanoneMABModel cmabmLav = new CanoneMABModel();
            List<CanoneMABModel> lcmabmPrecedenti = new List<CanoneMABModel>();
            List<CanoneMABModel> lcmabmSuccessivi = new List<CanoneMABModel>();

            try
            {

                lcmabmPrecedenti = PrelevaMovimentiCanoneMABPrecedenti(idMab, cmabm.DataInizioValidita, db).ToList();

                lcmabmSuccessivi = PrelevaMovimentiCanoneMABSuccessivi(idMab, cmabm.DataInizioValidita, db).ToList();

                //if (lcmabmPrecedenti.Count == 0)
                //{
                //    if (lcmabmSuccessivi.Count == 0)
                //    {
                //        #region creo record (periodo dataIniInput - dataRientro)
                //        cmabmLav = new CanoneMABModel()
                //        {
                //            IDMAB= idMab,
                //            IDAttivazioneMAB= cmabm.IDAttivazioneMAB,
                //            idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                //            idValuta= cmabm.idValuta,
                //            ImportoCanone = cmabm.ImportoCanone,
                //            DataInizioValidita = cmabm.DataInizioValidita,
                //            DataFineValidita = dataFineValiditaMAB,
                //            DataAggiornamento = DateTime.Now,
                //            FK_IDCanone = cmabm.FK_IDCanone,
                //            nascondi = cmabm.nascondi
                //        };
                //        SetCanoneMAB(ref cmabmLav, db);
                //        #endregion
                //    }
                //    else
                //    {
                //        cmabmSuccessivo = lcmabmSuccessivi.First();

                //        #region annullo tutti record fino al primo buco temporale o dataRientro
                //        var cont = 1;
                //        //nascondo in ogni caso il primo successivo
                //        cmabmSuccessivo.NascondiRecord(db);
                //        var dataFineCorrente = cmabmSuccessivo.DataFineValidita;
                //        //annullo solo i successivi record attigui e leggo l'ultima datafine del periodo
                //        foreach (var cmabmSucc in lcmabmSuccessivi)
                //        {
                //            if (cont > 1 && cmabmSucc.DataInizioValidita == dataFineCorrente.AddDays(1))
                //            {
                //                dataFineCorrente = cmabmSucc.DataFineValidita;
                //                cmabmSucc.NascondiRecord(db);
                //            }
                //            cont++;
                //        }
                //        #endregion

                //        #region creo record
                //        cmabmLav = new CanoneMABModel()
                //        {
                //            IDMAB=cmabm.IDMAB,
                //            IDAttivazioneMAB=cmabm.IDAttivazioneMAB,
                //            idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                //            idValuta=cmabm.idValuta,
                //            ImportoCanone = cmabm.ImportoCanone,
                //            DataInizioValidita = cmabm.DataInizioValidita,
                //            DataFineValidita = dataFineCorrente,
                //            DataAggiornamento = DateTime.Now,
                //            FK_IDCanone = cmabm.FK_IDCanone,
                //            nascondi = cmabm.nascondi
                //        };
                //        SetCanoneMAB(ref cmabmLav, db);
                //        #endregion
                //    }
                //}
                //else
                //{
                    cmabmPrecedente = lcmabmPrecedenti.First();

                    if (lcmabmSuccessivi.Count == 0)
                    { ////
                        if (cmabmPrecedente.DataInizioValidita == cmabm.DataInizioValidita)
                        {
                            if (cmabmPrecedente.idStatoRecord == (decimal)EnumStatoRecord.In_Lavorazione)
                            {
                                #region edit record
                                var cmabPrecedente = db.CANONEMAB.Find(cmabmPrecedente.idCanone);
                                cmabPrecedente.IMPORTOCANONE = cmabm.ImportoCanone;
                                cmabPrecedente.IDVALUTA = cmabm.idValuta;
                                cmabPrecedente.DATAFINEVALIDITA = dataFineValiditaMAB;
                                cmabPrecedente.DATAAGGIORNAMENTO = DateTime.Now;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante l'inserimento del canone MAB.");
                                }
                                #endregion
                            }
                            else
                            {
                                cmabmPrecedente.NascondiRecord(db);

                                #region replico creo record con periodo dataini - dataIniInput-1
                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB=cmabm.IDMAB,
                                    IDAttivazioneMAB=cmabm.IDAttivazioneMAB,
                                    idValuta=cmabm.idValuta,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    ImportoCanone = cmabm.ImportoCanone,
                                    DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                    DataFineValidita = dataFineValiditaMAB,
                                    DataAggiornamento = DateTime.Now,
                                    FK_IDCanone = cmabm.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion
                            }
                        }
                        else
                        {
                            cmabmPrecedente.NascondiRecord(db);

                            #region replico creo record con periodo dataini - dataIniInput-1
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabmPrecedente.idValuta,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                ImportoCanone = cmabmPrecedente.ImportoCanone,
                                DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                DataFineValidita = cmabm.DataInizioValidita.AddDays(-1),
                                DataAggiornamento = DateTime.Now,
                                FK_IDCanone = cmabmPrecedente.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion

                            #region creo record 
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabm.idValuta,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                ImportoCanone = cmabm.ImportoCanone,
                                DataInizioValidita = cmabm.DataInizioValidita,
                                DataFineValidita = dataFineValiditaMAB,
                                DataAggiornamento = DateTime.Now,
                                FK_IDCanone = cmabm.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion
                        }
                    }
                    else
                    {
                        cmabmSuccessivo = lcmabmSuccessivi.First();

                        if (cmabmPrecedente.DataInizioValidita == cmabm.DataInizioValidita)
                        {
                            #region annullo i record successivi fino al primo buco temporale o dataRientro
                            var cont = 1;
                            //nascondo in ogni caso il primo successivo
                            cmabmSuccessivo.NascondiRecord(db);
                            var dataFineCorrente = cmabmSuccessivo.DataFineValidita;
                            //annullo solo i successivi record attigui e leggo l'ultima datafine del periodo
                            foreach (var cmabmSucc in lcmabmSuccessivi)
                            {
                                if (cont > 1 && cmabmSucc.DataInizioValidita == dataFineCorrente.AddDays(1))
                                {
                                    dataFineCorrente = cmabmSucc.DataFineValidita;
                                    cmabmSucc.NascondiRecord(db);
                                }
                                cont++;
                            }
                            #endregion
                            if (cmabmPrecedente.idStatoRecord == (decimal)EnumStatoRecord.In_Lavorazione)
                            {

                                #region edit record
                                var cmabPrecedente = db.CANONEMAB.Find(cmabmPrecedente.idCanone);
                                cmabPrecedente.IMPORTOCANONE = cmabm.ImportoCanone;
                                cmabPrecedente.IDVALUTA = cmabm.idValuta;
                                cmabPrecedente.DATAFINEVALIDITA = dataFineCorrente;
                                cmabPrecedente.DATAAGGIORNAMENTO = DateTime.Now;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante l'inserimento del canone.");
                                }
                                #endregion
                            }
                            else
                            {
                                cmabmPrecedente.NascondiRecord(db);

                                #region replico creo record con periodo dataini - dataFineCorrente
                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB=cmabm.IDMAB,
                                    IDAttivazioneMAB=cmabm.IDAttivazioneMAB,
                                    idValuta=cmabm.idValuta,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    ImportoCanone = cmabm.ImportoCanone,
                                    DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                    DataFineValidita = dataFineCorrente,
                                    DataAggiornamento = DateTime.Now,
                                    FK_IDCanone = cmabm.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion
                            }
                        }
                        else
                        {

                            cmabmPrecedente.NascondiRecord(db);

                            #region replico creo record con periodo dataini - dataIniInput-1
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabmPrecedente.idValuta,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                ImportoCanone = cmabmPrecedente.ImportoCanone,
                                DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                DataFineValidita = cmabm.DataInizioValidita.AddDays(-1),
                                DataAggiornamento = DateTime.Now,
                                FK_IDCanone = cmabm.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion

                            #region annullo i record successivi fino al primo buco temporale o dataRientro
                            var cont = 1;
                            //nascondo in ogni caso il primo successivo
                            cmabmSuccessivo.NascondiRecord(db);
                            var dataFineCorrente = cmabmSuccessivo.DataFineValidita;
                            //annullo solo i successivi record attigui e leggo l'ultima datafine del periodo
                            foreach (var cmabmSucc in lcmabmSuccessivi)
                            {
                                if (cont > 1 && cmabmSucc.DataInizioValidita == dataFineCorrente.AddDays(1))
                                {
                                    dataFineCorrente = cmabmSucc.DataFineValidita;
                                    cmabmSucc.NascondiRecord(db);
                                }
                                cont++;
                            }
                            #endregion

                            #region creo record
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabm.idValuta,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                ImportoCanone = cmabm.ImportoCanone,
                                DataInizioValidita = cmabm.DataInizioValidita,
                                DataFineValidita = dataFineCorrente,
                                DataAggiornamento = DateTime.Now,
                                FK_IDCanone = cmabm.FK_IDCanone,
                                nascondi = false
                            };

                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion
                        }
                    }

                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void SetNuovoPagatoCondivisoMAB(PagatoCondivisoMABModel pcmabm, decimal idMab, decimal idAttivazioneMAB, DateTime dataFineValiditaMAB, ModelDBISE db)
        {

            PagatoCondivisoMABModel pcmabmPrecedente = new PagatoCondivisoMABModel();
            PagatoCondivisoMABModel pcmabmSuccessivo = new PagatoCondivisoMABModel();
            PagatoCondivisoMABModel pcmabmLav = new PagatoCondivisoMABModel();
            List<PagatoCondivisoMABModel> lpcmabmPrecedenti = new List<PagatoCondivisoMABModel>();
            List<PagatoCondivisoMABModel> lpcmabmSuccessivi = new List<PagatoCondivisoMABModel>();

            try
            {
                lpcmabmPrecedenti = PrelevaMovimentiPagatoCondivisoMABPrecedenti(idMab, pcmabm.DataInizioValidita, db).ToList();

                lpcmabmSuccessivi = PrelevaMovimentiPagatoCondivisoMABSuccessivi(idMab, pcmabm.DataInizioValidita, db).ToList();

                pcmabmPrecedente = lpcmabmPrecedenti.First();

                if (lpcmabmSuccessivi.Count == 0)
                {
                    if (pcmabmPrecedente.DataInizioValidita == pcmabm.DataInizioValidita)
                    {
                        if (pcmabmPrecedente.idStatoRecord == (decimal)EnumStatoRecord.In_Lavorazione)
                        {
                            #region edit record
                            var pcmabPrecedente = db.PAGATOCONDIVISOMAB.Find(pcmabmPrecedente.idPagatoCondiviso);
                            pcmabPrecedente.CONDIVISO = pcmabm.Condiviso;
                            pcmabPrecedente.PAGATO = pcmabm.Pagato;
                            pcmabPrecedente.DATAFINEVALIDITA = dataFineValiditaMAB;
                            pcmabPrecedente.DATAAGGIORNAMENTO = DateTime.Now;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore durante l'inserimento di pagato condiviso.");
                            }
                            #endregion
                        }
                        else
                        {
                            #region nascondo il record
                            pcmabmPrecedente.NascondiRecord(db);
                            #endregion

                            #region creo record
                            pcmabmLav = new PagatoCondivisoMABModel()
                            {
                                idMAB = pcmabm.idMAB,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                Condiviso = pcmabm.Condiviso,
                                Pagato = pcmabm.Pagato,
                                DataInizioValidita = pcmabm.DataInizioValidita,
                                DataFineValidita = dataFineValiditaMAB,
                                DataAggiornamento = DateTime.Now,
                                fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                                Nascondi = false
                            };
                            SetPagatoCondivisoMAB(ref pcmabmLav, db);
                            #endregion
                        }

                    }
                    else
                    {
                        if (pcmabmPrecedente.DataFineValidita == dataFineValiditaMAB)
                        {
                            #region replico record e lo nascondo
                            pcmabmPrecedente.NascondiRecord(db);

                            pcmabmLav = new PagatoCondivisoMABModel()
                            {
                                idMAB = pcmabm.idMAB,
                                idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                Pagato = pcmabmPrecedente.Pagato,
                                Condiviso = pcmabmPrecedente.Condiviso,
                                DataInizioValidita = pcmabmPrecedente.DataInizioValidita,
                                DataFineValidita = pcmabm.DataInizioValidita.AddDays(-1),
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                fk_IDPagatoCondiviso = pcmabmPrecedente.fk_IDPagatoCondiviso,
                                Nascondi = false
                            };
                            SetPagatoCondivisoMAB(ref pcmabmLav, db);
                            #endregion

                            #region creo record
                            pcmabmLav = new PagatoCondivisoMABModel()
                            {
                                idMAB = pcmabm.idMAB,
                                idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                Pagato = pcmabm.Pagato,
                                Condiviso = pcmabm.Condiviso,
                                DataInizioValidita = pcmabm.DataInizioValidita,
                                DataFineValidita = dataFineValiditaMAB,
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                                Nascondi = false
                            };
                            SetPagatoCondivisoMAB(ref pcmabmLav, db);
                            #endregion
                        }
                        else
                        {
                            #region creo record
                            pcmabmLav = new PagatoCondivisoMABModel()
                            {
                                idMAB = pcmabm.idMAB,
                                idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                Condiviso = pcmabm.Condiviso,
                                Pagato = pcmabm.Pagato,
                                DataInizioValidita = pcmabm.DataInizioValidita,
                                DataFineValidita = dataFineValiditaMAB,
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                                Nascondi = false
                            };
                            SetPagatoCondivisoMAB(ref pcmabmLav, db);
                            #endregion
                        }
                    }
                }

                else
                {
                    if (pcmabmPrecedente.DataInizioValidita == pcmabm.DataInizioValidita &&
                        pcmabmPrecedente.idStatoRecord == (decimal)EnumStatoRecord.In_Lavorazione)
                    {
                        #region edit record
                        var pcmabPrecedente = db.PAGATOCONDIVISOMAB.Find(pcmabmPrecedente.idPagatoCondiviso);
                        pcmabPrecedente.PAGATO = pcmabm.Pagato;
                        pcmabPrecedente.CONDIVISO = pcmabm.Condiviso;
                        pcmabPrecedente.DATAAGGIORNAMENTO = DateTime.Now;
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore durante l'inserimento di pagato condiviso.");
                        }
                        #endregion
                    }
                    else
                    {
                        pcmabmSuccessivo = lpcmabmSuccessivi.First();

                        //controllo periodo attiguo
                        if (pcmabmPrecedente.DataFineValidita == pcmabmSuccessivo.DataInizioValidita.AddDays(-1))
                        {
                          
                            #region replico record e lo nascondo
                            pcmabmPrecedente.NascondiRecord(db);

                            pcmabmLav = new PagatoCondivisoMABModel()
                            {
                                idMAB = pcmabm.idMAB,
                                idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                Pagato = pcmabmPrecedente.Pagato,
                                Condiviso = pcmabmPrecedente.Condiviso,
                                DataInizioValidita = pcmabmPrecedente.DataInizioValidita,
                                DataFineValidita = pcmabm.DataInizioValidita.AddDays(-1),
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                fk_IDPagatoCondiviso = pcmabmPrecedente.fk_IDPagatoCondiviso,
                                Nascondi = false
                            };
                            SetPagatoCondivisoMAB(ref pcmabmLav, db);
                            #endregion

                            if (pcmabm.Pagato != pcmabmSuccessivo.Pagato || pcmabm.Condiviso != pcmabmSuccessivo.Condiviso)
                            {
                                #region se dati variati rispetto a succ creo record da dataini a dataini-1(succ)
                                pcmabmLav = new PagatoCondivisoMABModel()
                                {
                                    idMAB = pcmabm.idMAB,
                                    idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                    Pagato = pcmabm.Pagato,
                                    Condiviso = pcmabm.Condiviso,
                                    DataInizioValidita = pcmabm.DataInizioValidita,
                                    DataFineValidita = pcmabmSuccessivo.DataInizioValidita.AddDays(-1),
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                                    Nascondi = false
                                };
                                SetPagatoCondivisoMAB(ref pcmabmLav, db);
                                #endregion
                            }
                            else
                            {
                                #region se dati non variati rispetto a succ creo record da dataini a datafine(succ)

                                pcmabmSuccessivo.NascondiRecord(db);

                                pcmabmLav = new PagatoCondivisoMABModel()
                                {
                                    idMAB = pcmabm.idMAB,
                                    idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                    Pagato = pcmabm.Pagato,
                                    Condiviso = pcmabm.Condiviso,
                                    DataInizioValidita = pcmabm.DataInizioValidita,
                                    DataFineValidita = pcmabmSuccessivo.DataFineValidita,
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                                    Nascondi = false
                                };
                                SetPagatoCondivisoMAB(ref pcmabmLav, db);
                                #endregion
                            }

                        }
                        else
                        {
                            pcmabmPrecedente.NascondiRecord(db);
                            #region replico record
                            pcmabmLav = new PagatoCondivisoMABModel()
                            {
                                idMAB = pcmabm.idMAB,
                                idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                Pagato = pcmabmPrecedente.Pagato,
                                Condiviso = pcmabmPrecedente.Condiviso,
                                DataInizioValidita = pcmabmSuccessivo.DataInizioValidita,
                                DataFineValidita = pcmabm.DataInizioValidita.AddDays(-1),
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                                Nascondi = false
                            };
                            SetPagatoCondivisoMAB(ref pcmabmLav, db);
                            #endregion

                            #region creo record
                            pcmabmLav = new PagatoCondivisoMABModel()
                            {
                                idMAB = pcmabm.idMAB,
                                idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                Pagato = pcmabm.Pagato,
                                Condiviso = pcmabm.Condiviso,
                                DataInizioValidita = pcmabm.DataInizioValidita,
                                DataFineValidita = pcmabmPrecedente.DataFineValidita,
                                DataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                                Nascondi = false
                            };
                            SetPagatoCondivisoMAB(ref pcmabmLav, db);
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetNuovoPagatoCondiviso_AggiornaTutti(PagatoCondivisoMABModel pcmabm, decimal idMab, decimal idAttivazioneMAB, DateTime dataFineValiditaMAB, ModelDBISE db)
        {

            PagatoCondivisoMABModel pcmabmPrecedente = new PagatoCondivisoMABModel();
            PagatoCondivisoMABModel pcmabmSuccessivo = new PagatoCondivisoMABModel();
            PagatoCondivisoMABModel pcmabmLav = new PagatoCondivisoMABModel();
            List<PagatoCondivisoMABModel> lpcmabmPrecedenti = new List<PagatoCondivisoMABModel>();
            List<PagatoCondivisoMABModel> lpcmabmSuccessivi = new List<PagatoCondivisoMABModel>();

            try
            {

                lpcmabmPrecedenti = PrelevaMovimentiPagatoCondivisoMABPrecedenti(idMab, pcmabm.DataInizioValidita, db).ToList();

                lpcmabmSuccessivi = PrelevaMovimentiPagatoCondivisoMABSuccessivi(idMab, pcmabm.DataInizioValidita, db).ToList();

                //if (lpcmabmPrecedenti.Count == 0)
                //{
                //    if (lpcmabmSuccessivi.Count == 0)
                //    {
                //        #region creo record (periodo dataIniInput - dataRientro)
                //        pcmabmLav = new PagatoCondivisoMABModel()
                //        {
                //            idMAB = idMab,
                //            idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                //            idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                //            Pagato = pcmabm.Pagato,
                //            Condiviso = pcmabm.Condiviso,
                //            DataInizioValidita = pcmabm.DataInizioValidita,
                //            DataFineValidita = dataFineValiditaMAB,
                //            DataAggiornamento = DateTime.Now,
                //            fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                //            Nascondi = pcmabm.Nascondi
                //        };
                //        SetPagatoCondivisoMAB(ref pcmabmLav, db);
                //        #endregion
                //    }
                //    else
                //    {
                //        pcmabmSuccessivo = lpcmabmSuccessivi.First();

                //        #region annullo tutti record fino al primo buco temporale o dataRientro
                //        var cont = 1;
                //        //nascondo in ogni caso il primo successivo
                //        pcmabmSuccessivo.NascondiRecord(db);
                //        var dataFineCorrente = pcmabmSuccessivo.DataFineValidita;
                //        //annullo solo i successivi record attigui e leggo l'ultima datafine del periodo
                //        foreach (var pcmabmSucc in lpcmabmSuccessivi)
                //        {
                //            if (cont > 1 && pcmabmSucc.DataInizioValidita == dataFineCorrente.AddDays(1))
                //            {
                //                dataFineCorrente = pcmabmSucc.DataFineValidita;
                //                pcmabmSucc.NascondiRecord(db);
                //            }
                //            cont++;
                //        }
                //        #endregion

                //        #region creo record
                //        pcmabmLav = new PagatoCondivisoMABModel()
                //        {
                //            idMAB = pcmabm.idMAB,
                //            idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                //            idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                //            Pagato = pcmabm.Pagato,
                //            Condiviso = pcmabm.Condiviso,
                //            DataInizioValidita = pcmabm.DataInizioValidita,
                //            DataFineValidita = dataFineCorrente,
                //            DataAggiornamento = DateTime.Now,
                //            fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                //            Nascondi = pcmabm.Nascondi
                //        };
                //        SetPagatoCondivisoMAB(ref pcmabmLav, db);
                //        #endregion
                //    }
                //}
                //else
                //{
                    pcmabmPrecedente = lpcmabmPrecedenti.First();

                    if (lpcmabmSuccessivi.Count == 0)
                    { ////
                        if (pcmabmPrecedente.DataInizioValidita == pcmabm.DataInizioValidita)
                        {
                            if (pcmabmPrecedente.idStatoRecord == (decimal)EnumStatoRecord.In_Lavorazione)
                            {
                                #region edit record
                                var pcmabPrecedente = db.PAGATOCONDIVISOMAB.Find(pcmabmPrecedente.idPagatoCondiviso);
                                pcmabPrecedente.PAGATO = pcmabm.Pagato;
                                pcmabPrecedente.CONDIVISO = pcmabm.Condiviso;
                                pcmabPrecedente.DATAFINEVALIDITA = dataFineValiditaMAB;
                                pcmabPrecedente.DATAAGGIORNAMENTO = DateTime.Now;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante l'inserimento del pagato condiviso MAB.");
                                }
                                #endregion
                            }
                            else
                            {
                                pcmabmPrecedente.NascondiRecord(db);

                                #region replico creo record con periodo dataini - dataIniInput-1
                                pcmabmLav = new PagatoCondivisoMABModel()
                                {
                                    idMAB = pcmabm.idMAB,
                                    idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                    Pagato = pcmabm.Pagato,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    Condiviso = pcmabm.Condiviso,
                                    DataInizioValidita = pcmabmPrecedente.DataInizioValidita,
                                    DataFineValidita = dataFineValiditaMAB,
                                    DataAggiornamento = DateTime.Now,
                                    fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                                    Nascondi = false
                                };
                                SetPagatoCondivisoMAB(ref pcmabmLav, db);
                                #endregion
                            }
                        }
                        else
                        {
                            pcmabmPrecedente.NascondiRecord(db);

                            #region replico creo record con periodo dataini - dataIniInput-1
                            pcmabmLav = new PagatoCondivisoMABModel()
                            {
                                idMAB = pcmabm.idMAB,
                                idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                Pagato = pcmabmPrecedente.Pagato,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                Condiviso = pcmabmPrecedente.Condiviso,
                                DataInizioValidita = pcmabmPrecedente.DataInizioValidita,
                                DataFineValidita = pcmabm.DataInizioValidita.AddDays(-1),
                                DataAggiornamento = DateTime.Now,
                                fk_IDPagatoCondiviso = pcmabmPrecedente.fk_IDPagatoCondiviso,
                                Nascondi = false
                            };
                            SetPagatoCondivisoMAB(ref pcmabmLav, db);
                            #endregion

                            #region creo record 
                            pcmabmLav = new PagatoCondivisoMABModel()
                            {
                                idMAB = pcmabm.idMAB,
                                idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                Pagato = pcmabm.Pagato,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                Condiviso = pcmabm.Condiviso,
                                DataInizioValidita = pcmabm.DataInizioValidita,
                                DataFineValidita = dataFineValiditaMAB,
                                DataAggiornamento = DateTime.Now,
                                fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                                Nascondi = false
                            };
                            SetPagatoCondivisoMAB(ref pcmabmLav, db);
                            #endregion
                        }
                    }
                    else
                    {
                        pcmabmSuccessivo = lpcmabmSuccessivi.First();

                        if (pcmabmPrecedente.DataInizioValidita == pcmabm.DataInizioValidita)
                        {
                            #region annullo i record successivi fino al primo buco temporale o dataRientro
                            var cont = 1;
                            //nascondo in ogni caso il primo successivo
                            pcmabmSuccessivo.NascondiRecord(db);
                            var dataFineCorrente = pcmabmSuccessivo.DataFineValidita;
                            //annullo solo i successivi record attigui e leggo l'ultima datafine del periodo
                            foreach (var pcmabmSucc in lpcmabmSuccessivi)
                            {
                                if (cont > 1 && pcmabmSucc.DataInizioValidita == dataFineCorrente.AddDays(1))
                                {
                                    dataFineCorrente = pcmabmSucc.DataFineValidita;
                                    pcmabmSucc.NascondiRecord(db);
                                }
                                cont++;
                            }
                            #endregion
                            if (pcmabmPrecedente.idStatoRecord == (decimal)EnumStatoRecord.In_Lavorazione)
                            {

                                #region edit record
                                var pcmabPrecedente = db.PAGATOCONDIVISOMAB.Find(pcmabmPrecedente.idPagatoCondiviso);
                                pcmabPrecedente.PAGATO = pcmabm.Pagato;
                                pcmabPrecedente.CONDIVISO = pcmabm.Condiviso;
                                pcmabPrecedente.DATAFINEVALIDITA = dataFineCorrente;
                                pcmabPrecedente.DATAAGGIORNAMENTO = DateTime.Now;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante l'inserimento di pagato condiviso.");
                                }
                                #endregion
                            }
                            else
                            {
                                pcmabmPrecedente.NascondiRecord(db);

                                #region replico creo record con periodo dataini - dataFineCorrente
                                pcmabmLav = new PagatoCondivisoMABModel()
                                {
                                    idMAB = pcmabm.idMAB,
                                    idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                    Pagato = pcmabm.Pagato,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    Condiviso = pcmabm.Condiviso,
                                    DataInizioValidita = pcmabmPrecedente.DataInizioValidita,
                                    DataFineValidita = dataFineCorrente,
                                    DataAggiornamento = DateTime.Now,
                                    fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                                    Nascondi = false
                                };
                                SetPagatoCondivisoMAB(ref pcmabmLav, db);
                                #endregion
                            }
                        }
                        else
                        {

                            pcmabmPrecedente.NascondiRecord(db);

                            #region replico creo record con periodo dataini - dataIniInput-1
                            pcmabmLav = new PagatoCondivisoMABModel()
                            {
                                idMAB = pcmabm.idMAB,
                                idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                Pagato = pcmabmPrecedente.Pagato,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                Condiviso = pcmabmPrecedente.Condiviso,
                                DataInizioValidita = pcmabmPrecedente.DataInizioValidita,
                                DataFineValidita = pcmabm.DataInizioValidita.AddDays(-1),
                                DataAggiornamento = DateTime.Now,
                                fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                                Nascondi = false
                            };
                            SetPagatoCondivisoMAB(ref pcmabmLav, db);
                            #endregion

                            #region annullo i record successivi fino al primo buco temporale o dataRientro
                            var cont = 1;
                            //nascondo in ogni caso il primo successivo
                            pcmabmSuccessivo.NascondiRecord(db);
                            var dataFineCorrente = pcmabmSuccessivo.DataFineValidita;
                            //annullo solo i successivi record attigui e leggo l'ultima datafine del periodo
                            foreach (var pcmabmSucc in lpcmabmSuccessivi)
                            {
                                if (cont > 1 && pcmabmSucc.DataInizioValidita == dataFineCorrente.AddDays(1))
                                {
                                    dataFineCorrente = pcmabmSucc.DataFineValidita;
                                    pcmabmSucc.NascondiRecord(db);
                                }
                                cont++;
                            }
                            #endregion

                            #region creo record
                            pcmabmLav = new PagatoCondivisoMABModel()
                            {
                                idMAB = pcmabm.idMAB,
                                idAttivazioneMAB = pcmabm.idAttivazioneMAB,
                                Pagato = pcmabm.Pagato,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                Condiviso = pcmabm.Condiviso,
                                DataInizioValidita = pcmabm.DataInizioValidita,
                                DataFineValidita = dataFineCorrente,
                                DataAggiornamento = DateTime.Now,
                                fk_IDPagatoCondiviso = pcmabm.fk_IDPagatoCondiviso,
                                Nascondi = false
                            };

                            SetPagatoCondivisoMAB(ref pcmabmLav, db);
                            #endregion
                        }
                    }

                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public MABModel GetMABModelByID(decimal idMAB, ModelDBISE db)
        {
            try
            {
                MABModel mabm = new MABModel();

                var mab = db.MAB.Find(idMAB);

                if (mab.IDMAB>0)
                {
                    mabm = new MABModel()
                    {
                        idMAB = mab.IDMAB,
                        idTrasfIndennita = mab.IDTRASFINDENNITA,
                        idStatoRecord = mab.IDSTATORECORD,
                        rinunciaMAB = mab.RINUNCIAMAB,
                        dataAggiornamento = mab.DATAAGGIORNAMENTO
                    };
                }

                return mabm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public MAGGIORAZIONEABITAZIONE GetMaggiorazioneAbitazioneByID_var(decimal idMagAbitazione)
        //{
        //    try
        //    {
        //        MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE();

        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            ma = db.MAGGIORAZIONEABITAZIONE.Find(idMagAbitazione);
        //        }

        //        return ma;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public MAB GetMAB_ByID_var(decimal idMAB, ModelDBISE db)
        {
            try
            {
                MAB mab = new MAB();

                mab = db.MAB.Find(idMAB);

                return mab;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAGATOCONDIVISOMAB> GetListPagatoCondivisoMAB_var(MABViewModel mabvm)
        {
            try
            {
                List<PAGATOCONDIVISOMAB> lpc = new List<PAGATOCONDIVISOMAB>();

                using (ModelDBISE db = new ModelDBISE())
                {
                    lpc = db.PAGATOCONDIVISOMAB.Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                        mabvm.idMAB == x.IDMAB)
                                                    .OrderByDescending(a => a.IDPAGATOCONDIVISO)
                                                    .ToList();
                }

                return lpc;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public PAGATOCONDIVISOMAB GetUltimoPagatoCondivisoMAB(decimal idMAB)
        //{
        //    try
        //    {
        //        //PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();
        //        PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();

        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var mab = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);

        //            var lpc = mab.PAGATOCONDIVISOMAB
        //                        .Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
        //                        .OrderByDescending(a => a.IDPAGATOCONDIVISO)
        //                        .ToList();
        //            if (lpc?.Any() ?? false)
        //            {
        //                pc = lpc.First();
        //            }
        //        }
        //        return pc;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void SostituisciDocumentoMAB_var(ref DocumentiModel dm, decimal idDocumentoOld, decimal idAttivazioneMAB, ModelDBISE db)
        {
            //inserisce un nuovo documento e imposta il documento sostituito 
            //con MODIFICATO=true e valorizza FK_IDDOCUMENTO

            DOCUMENTI d_new = new DOCUMENTI();
            DOCUMENTI d_old = new DOCUMENTI();
            MemoryStream ms = new MemoryStream();
            dm.file.InputStream.CopyTo(ms);
            var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

            d_new.NOMEDOCUMENTO = dm.nomeDocumento;
            d_new.ESTENSIONE = dm.estensione;
            d_new.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
            d_new.DATAINSERIMENTO = dm.dataInserimento;
            d_new.FILEDOCUMENTO = ms.ToArray();
            d_new.MODIFICATO = false;
            d_new.FK_IDDOCUMENTO = idDocumentoOld;
            d_new.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

            am.DOCUMENTI.Add(d_new);

            if (db.SaveChanges() > 0)
            {
                var mab = db.MAB.Find(am.IDMAB);
                var t = mab.INDENNITA.TRASFERIMENTO;

                dm.idDocumenti = d_new.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo documento (maggiorazione abitazione).", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);

                //aggiorno il documento esistente
                d_old = db.DOCUMENTI.Find(idDocumentoOld);
                if (d_old.IDDOCUMENTO > 0)
                {
                    d_old.MODIFICATO = true;

                    if (db.SaveChanges() > 0)
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modificato documento con FK_idDocumento (maggiorazione abitazione).", "Documenti", db, t.IDTRASFERIMENTO, d_old.IDDOCUMENTO);

                    }

                }
            }
        }

        public void SetDocumentoMAB_var(ref DocumentiModel dm, decimal idAttivazioneMAB, ModelDBISE db)
        {
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();

            dm.file.InputStream.CopyTo(ms);
            var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

            d.NOMEDOCUMENTO = dm.nomeDocumento;
            d.ESTENSIONE = dm.estensione;
            d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
            d.DATAINSERIMENTO = dm.dataInserimento;
            d.FILEDOCUMENTO = ms.ToArray();
            d.MODIFICATO = false;
            d.FK_IDDOCUMENTO = null;
            d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;
            am.DOCUMENTI.Add(d);

            if (db.SaveChanges() > 0)
            {
                var mab = db.MAB.Find(am.IDMAB);
                var t = mab.INDENNITA.TRASFERIMENTO;

                dm.idDocumenti = d.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo documento (maggiorazione abitazione).", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
            }
        }

        #region attiva
        public void AttivaRichiestaMAB_var(decimal idAttivazioneMAB)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtDipendenti dtd = new dtDipendenti())
                    {
                        var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);
                        if (am?.IDATTIVAZIONEMAB > 0)
                        {
                            if (am.NOTIFICARICHIESTA == true)
                            {
                                am.ATTIVAZIONE = true;
                                am.DATAATTIVAZIONE = DateTime.Now;
                                am.DATAAGGIORNAMENTO = DateTime.Now;
                                am.DATAVARIAZIONE = DateTime.Now;

                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore: Impossibile completare l'attivazione maggiorazione abitazione.");
                                }
                                var idTrasferimento = am.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;

                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Attivazione maggiorazione abitazione.", "ATTIVAZIONEMAB", db,
                                    idTrasferimento, am.IDATTIVAZIONEMAB);

                                var mab_corrente = am.MAB; //.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDMAB).ToList().First();

                                #region imposto lo stato su ATTIVO
                                //var mabl = am.MAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDMAB).ToList();
                                //foreach (var mab in mabl)
                                //{
                                if (mab_corrente.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                {
                                    UpdateStatoMAB(mab_corrente.IDMAB, EnumStatoRecord.Attivato, db);
                                }
                                //}

                                var pmabl = am.PERIODOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDPERIODOMAB).ToList();
                                if (pmabl?.Any() ?? false)
                                {
                                    var lpmab_att = am.PERIODOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderBy(a => a.IDPERIODOMAB).ToList();
                                    foreach (var pmab_att in lpmab_att)
                                    {
                                        UpdateStatoPeriodoMAB(pmab_att.IDPERIODOMAB, EnumStatoRecord.Annullato, db);
                                    }
                                }
                                foreach (var pmab in pmabl)
                                {
                                    UpdateStatoPeriodoMAB(pmab.IDPERIODOMAB, EnumStatoRecord.Attivato, db);
                                    dtd.DataInizioRicalcoliDipendente(idTrasferimento, pmab.DATAINIZIOMAB, db);
                                    dtd.DataInizioRicalcoliDipendente(idTrasferimento, pmab.DATAFINEMAB, db);
                                }

                                var cmabl = am.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare && a.NASCONDI == false).OrderBy(a => a.IDCANONE).ToList();
                                foreach (var cmab in cmabl)
                                {
                                    UpdateStatoCanoneMAB(cmab.IDCANONE, EnumStatoRecord.Attivato, db);
                                    dtd.DataInizioRicalcoliDipendente(idTrasferimento, cmab.DATAINIZIOVALIDITA, db);
                            
                                }
                                //annullo i record attivi nascosti  (relativi alla mab perche potrbbero avere idattivazione diverso)
                                cmabl = mab_corrente.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && a.NASCONDI).OrderBy(a => a.IDCANONE).ToList();
                                foreach (var cmab in cmabl)
                                {
                                    UpdateStatoCanoneMAB(cmab.IDCANONE, EnumStatoRecord.Annullato, db);
                                }

                                var aal = am.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDANTICIPOANNUALEMAB).ToList();
                                foreach (var aa in aal)
                                {
                                    UpdateStatoAnticipoAnnualeMAB(aa.IDANTICIPOANNUALEMAB, EnumStatoRecord.Attivato, db);
                                }

                                var pcmabl = am.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare && a.NASCONDI == false).OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
                                foreach (var pcmab in pcmabl)
                                {
                                    UpdateStatoPagatoCondivisoMAB(pcmab.IDPAGATOCONDIVISO, EnumStatoRecord.Attivato, db);
                                    dtd.DataInizioRicalcoliDipendente(idTrasferimento, pcmab.DATAINIZIOVALIDITA, db);
                                }
                                //annullo i record attivi nascosti (relativi alla mab perche potrbbero avere idattivazione diverso)
                                pcmabl = mab_corrente.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && a.NASCONDI).OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
                                foreach (var pcmab in pcmabl)
                                {
                                    UpdateStatoPagatoCondivisoMAB(pcmab.IDPAGATOCONDIVISO, EnumStatoRecord.Annullato, db);
                                }

                                var dmabl = am.DOCUMENTI
                                            .Where(a => a.MODIFICATO == false &&
                                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare &&
                                                        (
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione ||
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione ||
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore ||
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione ||
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Clausole_Contratto_Alloggio ||
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Contratto_Locazione ||
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione
                                                        )
                                                    )
                                            .OrderBy(a => a.IDDOCUMENTO).ToList();
                                foreach (var dmab in dmabl)
                                {
                                    this.UpdateStatoDocumentiMAB(dmab.IDDOCUMENTO, EnumStatoRecord.Attivato, db);
                                }
                                #endregion

                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(am.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione, db);
                                }

                                using (dtTrasferimento dtt = new dtTrasferimento())
                                {
                                    using (dtUffici dtu = new dtUffici())
                                    {
                                        var t = dtt.GetTrasferimentoById(am.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO);

                                        if (t?.idTrasferimento > 0)
                                        {
                                            var dip = dtd.GetDipendenteByID(t.idDipendente);
                                            var uff = dtu.GetUffici(t.idUfficio);

                                            EmailTrasferimento.EmailAttiva(am.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                Resources.msgEmail.OggettoAttivazioneMaggiorazioneAbitazione,
                                                                string.Format(Resources.msgEmail.MessaggioAttivazioneMaggiorazioneAbitazione, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString()),
                                                                db);
                                        }
                                    }
                                }
                                //ATTIVAZIONEMAB att_new = CreaAttivazione(idTrasferimento, db);
                            }
                        }

                        db.Database.CurrentTransaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }
        #endregion

        public AttivazioneMABModel GetAttivazionePartenzaMABAttiva(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();
                AttivazioneMABModel amm = new AttivazioneMABModel();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                var mabl = t.INDENNITA.MAB
                            .Where(a => a.RINUNCIAMAB == false && a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato)
                            .OrderBy(a => a.IDMAB)
                            .ToList();

                if (mabl?.Any() ?? false)
                {
                    var mab = mabl.First();
                    var aml = mab.ATTIVAZIONEMAB
                                .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA && a.ATTIVAZIONE)
                                .OrderBy(a => a.IDATTIVAZIONEMAB)
                                .ToList();

                    if (aml?.Any() ?? false)
                    {
                        am = aml.First();

                        amm = new AttivazioneMABModel()
                        {
                            idAttivazioneMAB = am.IDATTIVAZIONEMAB,
                            idMAB = am.IDMAB,
                            notificaRichiesta = am.NOTIFICARICHIESTA,
                            dataNotificaRichiesta = am.DATANOTIFICARICHIESTA,
                            Attivazione = am.ATTIVAZIONE,
                            dataAttivazione = am.DATAATTIVAZIONE,
                            dataVariazione = am.DATAVARIAZIONE,
                            dataAggiornamento = am.DATAAGGIORNAMENTO,
                            Annullato = am.ANNULLATO
                        };

                    }
                }
                return amm;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        #region annulla notifica
        public void AnnullaRichiestaMAB_var(decimal idAttivazioneMAB, string msg)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var am_Old = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                    var t = am_Old.MAB.INDENNITA.TRASFERIMENTO;
                    List<PERIODOMAB> pmab_Old_l = new List<PERIODOMAB>();
                    List<CANONEMAB> old_canone_l = new List<CANONEMAB>();
                    List<ANTICIPOANNUALEMAB> old_aa_l = new List<ANTICIPOANNUALEMAB>();
                    List<DOCUMENTI> ld_old = new List<DOCUMENTI>();
                    List<PAGATOCONDIVISOMAB> old_pc_l = new List<PAGATOCONDIVISOMAB>();

                    



                    if (am_Old?.IDATTIVAZIONEMAB > 0)
                    {
                        if (am_Old.NOTIFICARICHIESTA == true && am_Old.ATTIVAZIONE == false && am_Old.ANNULLATO == false)
                        {
                            am_Old.ANNULLATO = true;
                            am_Old.DATAAGGIORNAMENTO = DateTime.Now;


                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore - Impossibile annullare la notifica della richiesta maggiorazione abitazione.");
                            }

                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                "Annullamento della riga per il ciclo di attivazione della richiesta di maggiorazione abitazione",
                                "ATTIVAZIONEMAB", db, t.IDTRASFERIMENTO,
                                am_Old.IDATTIVAZIONEMAB);


                            var mab_Old =
                                am_Old.MAB;
                            //.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();

                            //se esistono mab da annullare duplico anche tutto ciò che è collegato
                            //foreach (var mab_Old in mab_Old_l)
                            //{
                            if (mab_Old.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                            {
                                #region MAB
                                MAB mab_New = new MAB()
                                {
                                    IDTRASFINDENNITA = mab_Old.IDTRASFINDENNITA,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    RINUNCIAMAB = mab_Old.RINUNCIAMAB,
                                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                };
                                db.MAB.Add(mab_New);
                                mab_Old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore - Impossibile inserire il record relativo a MAB nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                }

                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                    "Inserimento di una nuova riga MAB.",
                                    "MAB", db,
                                    t.IDTRASFERIMENTO,
                                    mab_New.IDMAB);
                                #endregion

                                #region associa MAB a MaggiorazioniAnnuali
                                var m_annuali_l = mab_Old.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false).ToList();
                                if (m_annuali_l?.Any() ?? false)
                                {
                                    foreach (var m_annuali in m_annuali_l)
                                    {
                                        this.Associa_MAB_MaggiorazioniAnnuali_var(mab_New.IDMAB, m_annuali.IDMAGANNUALI, db);
                                    }
                                }
                                #endregion

                                #region attivazione
                                ATTIVAZIONEMAB am_new = CreaAttivazioneMAB_var(mab_New.IDMAB, db);

                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                    "Inserimento di una nuova riga per il ciclo di attivazione relativo alla richiesta maggiorazione abitazione.",
                                    "ATTIVAZIONEMAB", db, t.IDTRASFERIMENTO,
                                    am_new.IDATTIVAZIONEMAB);
                                #endregion

                                #region PERIODOMAB
                                pmab_Old_l =
                                    am_Old.PERIODOMAB.Where(
                                   a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                foreach (var pmab_Old in pmab_Old_l)
                                {
                                    PERIODOMAB pmab_New = new PERIODOMAB()
                                    {
                                        IDMAB = mab_New.IDMAB,
                                        IDATTIVAZIONEMAB = am_new.IDATTIVAZIONEMAB,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        DATAINIZIOMAB = pmab_Old.DATAINIZIOMAB,
                                        DATAFINEMAB = pmab_Old.DATAFINEMAB,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        FK_IDPERIODOMAB = pmab_Old.FK_IDPERIODOMAB,
                                    };
                                    db.PERIODOMAB.Add(pmab_New);
                                    pmab_Old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile inserire il record relativo a PERIODOMAB nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                    }

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga PERIODOMAB.",
                                        "PERIODIOMAB", db,
                                        t.IDTRASFERIMENTO,
                                        pmab_New.IDPERIODOMAB);


                                    #region associa PERIODOMAB a percentuale MAB
                                    var percMAB_l = pmab_Old.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
                                    if (percMAB_l?.Any() ?? false)
                                    {
                                        foreach (var percMAB in percMAB_l)
                                        {
                                            this.Associa_PERIODOMAB_PercentualeMAB_var(pmab_New.IDPERIODOMAB, percMAB.IDPERCMAB, db);
                                        }
                                    }
                                    #endregion
                                }
                                #endregion

                                #region CANONEMAB
                                old_canone_l = am_Old.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare && a.NASCONDI == false).OrderBy(a => a.IDCANONE).ToList();
                                foreach (var old_canone in old_canone_l)
                                {
                                    CANONEMAB canone_new = new CANONEMAB()
                                    {
                                        IDMAB = mab_New.IDMAB,
                                        IDATTIVAZIONEMAB = am_new.IDATTIVAZIONEMAB,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        IDVALUTA = old_canone.IDVALUTA,
                                        DATAINIZIOVALIDITA = old_canone.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = old_canone.DATAFINEVALIDITA,
                                        IMPORTOCANONE = old_canone.IMPORTOCANONE,
                                        DATAAGGIORNAMENTO = old_canone.DATAAGGIORNAMENTO,
                                        NASCONDI = old_canone.NASCONDI,
                                        FK_IDCANONE = old_canone.FK_IDCANONE
                                    };
                                    db.CANONEMAB.Add(canone_new);
                                    old_canone.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile inserire il record relativo al canoneMAB nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                    }
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga canone per la richiesta maggiorazione abitazione.",
                                        "CANONEMAB", db,
                                        t.IDTRASFERIMENTO,
                                        canone_new.IDCANONE);


                                    #region associa canoneMAB a TFR
                                    var TFR_l = old_canone.TFR.Where(a => a.ANNULLATO == false).ToList();
                                    if (TFR_l?.Any() ?? false)
                                    {
                                        foreach (var TFR in TFR_l)
                                        {
                                            this.Associa_TFR_CanoneMAB_var(TFR.IDTFR, canone_new.IDCANONE, db);
                                        }
                                    }
                                    #endregion

                                }
                                #endregion

                                #region anticipoannualeMAB
                                old_aa_l = am_Old.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDANTICIPOANNUALEMAB).ToList();
                                foreach (var old_aa in old_aa_l)
                                {
                                    ANTICIPOANNUALEMAB aa_new = new ANTICIPOANNUALEMAB()
                                    {
                                        IDMAB = mab_New.IDMAB,
                                        IDATTIVAZIONEMAB = am_new.IDATTIVAZIONEMAB,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        ANTICIPOANNUALE = old_aa.ANTICIPOANNUALE,
                                        DATAAGGIORNAMENTO = old_aa.DATAAGGIORNAMENTO,
                                        FK_IDANTICIPOANNUALEMAB = old_aa.FK_IDANTICIPOANNUALEMAB
                                    };
                                    db.ANTICIPOANNUALEMAB.Add(aa_new);
                                    old_aa.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile inserire il record relativo a AnticipoAnnulale nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                    }

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga AnticipAnnuale per la richiesta maggiorazione abitazione.",
                                        "ANTICIPOANNUALEMAB", db,
                                        t.IDTRASFERIMENTO,
                                        aa_new.IDANTICIPOANNUALEMAB);
                                }
                                #endregion

                                #region pagatocondivisoMAB
                                old_pc_l = am_Old.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare && a.NASCONDI == false).OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
                                foreach (var old_pc in old_pc_l)
                                {
                                    PAGATOCONDIVISOMAB pc_new = new PAGATOCONDIVISOMAB()
                                    {
                                        IDMAB = mab_New.IDMAB,
                                        IDATTIVAZIONEMAB = am_new.IDATTIVAZIONEMAB,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        DATAINIZIOVALIDITA = old_pc.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = old_pc.DATAFINEVALIDITA,
                                        CONDIVISO = old_pc.CONDIVISO,
                                        PAGATO = old_pc.PAGATO,
                                        DATAAGGIORNAMENTO = old_pc.DATAAGGIORNAMENTO,
                                        NASCONDI = old_pc.NASCONDI,
                                        FK_IDPAGATOCONDIVISO = old_pc.FK_IDPAGATOCONDIVISO
                                    };
                                    db.PAGATOCONDIVISOMAB.Add(pc_new);
                                    old_pc.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile inserire il record relativo a PagatoCondivisoMAB nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                    }
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga canone per la richiesta maggiorazione abitazione.",
                                        "PAGATOCONDIVISOMAB", db,
                                        t.IDTRASFERIMENTO,
                                        pc_new.IDPAGATOCONDIVISO);


                                    #region associa PagatoCondivisoMAB a PercentualeCondivisione
                                    var PercCond_l = old_pc.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).ToList();
                                    if (PercCond_l?.Any() ?? false)
                                    {
                                        foreach (var PercCond in PercCond_l)
                                        {
                                            this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(pc_new.IDPAGATOCONDIVISO, PercCond.IDPERCCOND, db);
                                        }
                                    }
                                    #endregion

                                }
                                #endregion

                                #region documenti
                                ld_old = am_Old.DOCUMENTI.Where(x => x.MODIFICATO == false &&
                                                                    x.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare &&
                                                                    (
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione ||
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione ||
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore ||
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione ||
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Clausole_Contratto_Alloggio ||
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Contratto_Locazione ||
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione
                                                                    )
                                                                    )
                                                                    .ToList();

                                foreach (var d in ld_old)
                                {
                                    DOCUMENTI dNew = new DOCUMENTI()
                                    {
                                        IDTIPODOCUMENTO = d.IDTIPODOCUMENTO,
                                        NOMEDOCUMENTO = d.NOMEDOCUMENTO,
                                        ESTENSIONE = d.ESTENSIONE,
                                        FILEDOCUMENTO = d.FILEDOCUMENTO,
                                        DATAINSERIMENTO = d.DATAINSERIMENTO,
                                        MODIFICATO = d.MODIFICATO,
                                        FK_IDDOCUMENTO = d.FK_IDDOCUMENTO,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                    };
                                    am_new.DOCUMENTI.Add(dNew);
                                    d.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile inserire il record relativo a DOCUMENTI nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                    }

                                }
                                #endregion


                            }
                            else
                            {

                                #region attivazione
                                ATTIVAZIONEMAB am_new = CreaAttivazioneMAB_var(mab_Old.IDMAB, db);

                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                    "Inserimento di una nuova riga per il ciclo di attivazione relativo alla richiesta maggiorazione abitazione.",
                                    "ATTIVAZIONEMAB", db, t.IDTRASFERIMENTO,
                                    am_new.IDATTIVAZIONEMAB);
                                #endregion

                                #region PERIODOMAB
                                pmab_Old_l =
                                am_Old.PERIODOMAB.Where(
                               a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                foreach (var pmab_Old in pmab_Old_l)
                                {
                                    PERIODOMAB pmab_New = new PERIODOMAB()
                                    {
                                        IDMAB = pmab_Old.IDMAB,
                                        IDATTIVAZIONEMAB = am_new.IDATTIVAZIONEMAB,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        DATAINIZIOMAB = pmab_Old.DATAINIZIOMAB,
                                        DATAFINEMAB = pmab_Old.DATAFINEMAB,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        FK_IDPERIODOMAB = pmab_Old.FK_IDPERIODOMAB,
                                    };
                                    db.PERIODOMAB.Add(pmab_New);
                                    pmab_Old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile inserire il record relativo a PERIODOMAB nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                    }

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga PERIODOMAB.",
                                        "PERIODIOMAB", db,
                                        t.IDTRASFERIMENTO,
                                        pmab_New.IDPERIODOMAB);


                                    #region associa PERIODOMAB a percentuale MAB
                                    var percMAB_l = pmab_Old.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
                                    if (percMAB_l?.Any() ?? false)
                                    {
                                        foreach (var percMAB in percMAB_l)
                                        {
                                            this.Associa_PERIODOMAB_PercentualeMAB_var(pmab_New.IDPERIODOMAB, percMAB.IDPERCMAB, db);
                                        }
                                    }
                                    #endregion
                                }
                                #endregion

                                #region CANONEMAB
                                old_canone_l = am_Old.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare && a.NASCONDI == false).OrderBy(a => a.IDCANONE).ToList();
                                foreach (var old_canone in old_canone_l)
                                {
                                    CANONEMAB canone_new = new CANONEMAB()
                                    {
                                        IDMAB = old_canone.IDMAB,
                                        IDATTIVAZIONEMAB = am_new.IDATTIVAZIONEMAB,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        IDVALUTA = old_canone.IDVALUTA,
                                        DATAINIZIOVALIDITA = old_canone.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = old_canone.DATAFINEVALIDITA,
                                        IMPORTOCANONE = old_canone.IMPORTOCANONE,
                                        DATAAGGIORNAMENTO = old_canone.DATAAGGIORNAMENTO,
                                        NASCONDI = old_canone.NASCONDI,
                                        FK_IDCANONE = old_canone.FK_IDCANONE
                                    };
                                    db.CANONEMAB.Add(canone_new);
                                    old_canone.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile inserire il record relativo al canoneMAB nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                    }
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga canone per la richiesta maggiorazione abitazione.",
                                        "CANONEMAB", db,
                                        t.IDTRASFERIMENTO,
                                        canone_new.IDCANONE);


                                    #region associa canoneMAB a TFR
                                    var TFR_l = old_canone.TFR.Where(a => a.ANNULLATO == false).ToList();
                                    if (TFR_l?.Any() ?? false)
                                    {
                                        foreach (var TFR in TFR_l)
                                        {
                                            this.Associa_TFR_CanoneMAB_var(TFR.IDTFR, canone_new.IDCANONE, db);
                                        }
                                    }
                                    #endregion

                                }
                                #endregion

                                #region anticipoannualeMAB
                                old_aa_l = am_Old.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDANTICIPOANNUALEMAB).ToList();
                                foreach (var old_aa in old_aa_l)
                                {
                                    ANTICIPOANNUALEMAB aa_new = new ANTICIPOANNUALEMAB()
                                    {
                                        IDMAB = old_aa.IDMAB,
                                        IDATTIVAZIONEMAB = am_new.IDATTIVAZIONEMAB,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        ANTICIPOANNUALE = old_aa.ANTICIPOANNUALE,
                                        DATAAGGIORNAMENTO = old_aa.DATAAGGIORNAMENTO,
                                        FK_IDANTICIPOANNUALEMAB = old_aa.FK_IDANTICIPOANNUALEMAB
                                    };
                                    db.ANTICIPOANNUALEMAB.Add(aa_new);
                                    old_aa.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile inserire il record relativo a AnticipoAnnulale nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                    }

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga AnticipAnnuale per la richiesta maggiorazione abitazione.",
                                        "ANTICIPOANNUALEMAB", db,
                                        t.IDTRASFERIMENTO,
                                        aa_new.IDANTICIPOANNUALEMAB);
                                }
                                #endregion

                                #region documenti
                                ld_old = am_Old.DOCUMENTI.Where(x => x.MODIFICATO == false &&
                                                                    x.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare &&
                                                                    (
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione ||
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione ||
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore ||
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione ||
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Clausole_Contratto_Alloggio ||
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Contratto_Locazione ||
                                                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione
                                                                    )
                                                                    )
                                                                    .ToList();

                                foreach (var d in ld_old)
                                {
                                    DOCUMENTI dNew = new DOCUMENTI()
                                    {
                                        IDTIPODOCUMENTO = d.IDTIPODOCUMENTO,
                                        NOMEDOCUMENTO = d.NOMEDOCUMENTO,
                                        ESTENSIONE = d.ESTENSIONE,
                                        FILEDOCUMENTO = d.FILEDOCUMENTO,
                                        DATAINSERIMENTO = d.DATAINSERIMENTO,
                                        MODIFICATO = d.MODIFICATO,
                                        FK_IDDOCUMENTO = d.FK_IDDOCUMENTO,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                    };
                                    am_new.DOCUMENTI.Add(dNew);
                                    d.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile inserire il record relativo a DOCUMENTI nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                    }

                                }
                                #endregion

                                #region pagatocondivisoMAB
                                old_pc_l = am_Old.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare && a.NASCONDI == false).OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
                                foreach (var old_pc in old_pc_l)
                                {
                                    PAGATOCONDIVISOMAB pc_new = new PAGATOCONDIVISOMAB()
                                    {
                                        IDMAB = old_pc.IDMAB,
                                        IDATTIVAZIONEMAB = am_new.IDATTIVAZIONEMAB,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        DATAINIZIOVALIDITA = old_pc.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = old_pc.DATAFINEVALIDITA,
                                        CONDIVISO = old_pc.CONDIVISO,
                                        PAGATO = old_pc.PAGATO,
                                        DATAAGGIORNAMENTO = old_pc.DATAAGGIORNAMENTO,
                                        NASCONDI = old_pc.NASCONDI,
                                        FK_IDPAGATOCONDIVISO = old_pc.FK_IDPAGATOCONDIVISO
                                    };
                                    db.PAGATOCONDIVISOMAB.Add(pc_new);
                                    old_pc.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile inserire il record relativo a PagatoCondivisoMAB nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                    }
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga canone per la richiesta maggiorazione abitazione.",
                                        "PAGATOCONDIVISOMAB", db,
                                        t.IDTRASFERIMENTO,
                                        pc_new.IDPAGATOCONDIVISO);


                                    #region associa PagatoCondivisoMAB a PercentualeCondivisione
                                    var PercCond_l = old_pc.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).ToList();
                                    if (PercCond_l?.Any() ?? false)
                                    {
                                        foreach (var PercCond in PercCond_l)
                                        {
                                            this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(pc_new.IDPAGATOCONDIVISO, PercCond.IDPERCCOND, db);
                                        }
                                    }
                                    #endregion

                                }
                                #endregion
                            }

                            EmailTrasferimento.EmailAnnulla(t.IDTRASFERIMENTO,
                                                                Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioneAbitazione,
                                                                msg,
                                                                db);
                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                            {
                                dtce.AnnullaMessaggioEvento(t.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione, db);
                            }
                        }
                    }

                    db.Database.CurrentTransaction.Commit();
                }

                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }
        #endregion


        #region notifica 
        public void NotificaRichiestaMAB_var(decimal idAttivazioneMAB)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                        if (am?.IDATTIVAZIONEMAB > 0)
                        {
                            am.NOTIFICARICHIESTA = true;
                            am.DATANOTIFICARICHIESTA = DateTime.Now;
                            am.DATAAGGIORNAMENTO = DateTime.Now;
                            am.DATAVARIAZIONE = DateTime.Now;

                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione maggiorazione abitazione.");
                            }
                            else
                            {
                                #region imposto lo stato su DA_ATTIVARE
                                var mab = am.MAB; //.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).OrderBy(a => a.IDMAB).ToList();
                                //foreach (var mab in mabl)
                                //{
                                if (mab.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
                                {
                                    UpdateStatoMAB(mab.IDMAB, EnumStatoRecord.Da_Attivare, db);
                                }
                                //}

                                var pmabl = am.PERIODOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).OrderBy(a => a.IDPERIODOMAB).ToList();
                                foreach (var pmab in pmabl)
                                {
                                    UpdateStatoPeriodoMAB(pmab.IDPERIODOMAB, EnumStatoRecord.Da_Attivare, db);
                                }

                                var cmabl = am.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && a.NASCONDI==false).OrderBy(a => a.IDCANONE).ToList();
                                foreach (var cmab in cmabl)
                                {
                                    UpdateStatoCanoneMAB(cmab.IDCANONE, EnumStatoRecord.Da_Attivare, db);
                                }

                                var aal = am.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).OrderBy(a => a.IDANTICIPOANNUALEMAB).ToList();
                                foreach (var aa in aal)
                                {
                                    UpdateStatoAnticipoAnnualeMAB(aa.IDANTICIPOANNUALEMAB, EnumStatoRecord.Da_Attivare, db);
                                }

                                var pcmabl = am.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && a.NASCONDI == false).OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
                                foreach (var pcmab in pcmabl)
                                {
                                    UpdateStatoPagatoCondivisoMAB(pcmab.IDPAGATOCONDIVISO, EnumStatoRecord.Da_Attivare, db);
                                }

                                var dmabl = am.DOCUMENTI
                                            .Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && 
                                                            (
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione ||
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione ||
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore ||
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione ||
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Clausole_Contratto_Alloggio ||
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Contratto_Locazione ||
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione
                                                            )
                                                        )
                                            .OrderBy(a => a.IDDOCUMENTO).ToList();
                                foreach (var dmab in dmabl)
                                {
                                    this.UpdateStatoDocumentiMAB(dmab.IDDOCUMENTO, EnumStatoRecord.Da_Attivare, db);
                                }
                                #endregion

                                //elimina da CANONE e PAGATOCONDIVISO gli eventuali record con nascondi=true e stato=in lavorazione
                                cmabl = am.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && a.NASCONDI == true).ToList();
                                foreach(var cmab in cmabl)
                                {
                                    db.CANONEMAB.Remove(cmab);
                                    if (db.SaveChanges()<=0)
                                    {
                                        throw new Exception("Errore in fase di notifica MAB.");
                                    }
                                }
                                pcmabl = am.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && a.NASCONDI == true).ToList();
                                foreach (var pcmab in pcmabl)
                                {
                                    db.PAGATOCONDIVISOMAB.Remove(pcmab);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore in fase di notifica MAB.");
                                    }
                                }

                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    var dip = dtd.GetDipendenteByID(am.MAB.INDENNITA.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE);

                                    EmailTrasferimento.EmailNotifica(EnumChiamante.Maggiorazione_Abitazione,
                                                    am.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioneAbitazione,
                                                    string.Format(Resources.msgEmail.MessaggioNotificaMaggiorazioneAbitazione, dip.cognome + " " + dip.nome + " (" + dip.matricola + ")"),
                                                    db);
                                }

                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    CalendarioEventiModel cem = new CalendarioEventiModel()
                                    {
                                        idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione,
                                        idTrasferimento = am.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO,
                                        DataInizioEvento = DateTime.Now.Date,
                                        DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioneAbitazione)).Date,
                                    };

                                    dtce.InsertCalendarioEvento(ref cem, db);
                                }
                            }
                        }

                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        db.Database.CurrentTransaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public void InserisciMAB_var(MABViewModel mvm, decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        ATTIVAZIONEMAB amab = new ATTIVAZIONEMAB();
                        var att = GetAttivazioneAperta(idTrasferimento, db);


                        TrasferimentoModel tm = new TrasferimentoModel();

                        tm = dtt.GetTrasferimentoById(idTrasferimento);

                        #region MAB
                        MAB newmab = new MAB()
                        {
                            IDTRASFINDENNITA = idTrasferimento,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                            RINUNCIAMAB = false,
                            DATAAGGIORNAMENTO = DateTime.Now
                        };
                        db.MAB.Add(newmab);
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore in fase di creazione MAB");
                        }
                        #endregion

                        #region attivazione
                        if(att.IDATTIVAZIONEMAB>0==false)
                        {
                            att=CreaAttivazioneMAB_var(newmab.IDMAB, db);
                        }
                        #endregion

                        amab = att;

                        mvm.dataInizioMAB = mvm.ut_dataInizioMAB.Value;
                        mvm.dataFineMAB = (mvm.ut_dataFineMAB == null) ? tm.dataRientro.Value : mvm.ut_dataFineMAB.Value;
                        mvm.idTrasferimento = idTrasferimento;
                        mvm.idMAB = newmab.IDMAB;
                        mvm.idAttivazioneMAB = amab.IDATTIVAZIONEMAB;

                        #region periodo mab
                        PERIODOMAB new_pm = new PERIODOMAB()
                        {
                            IDMAB = mvm.idMAB,
                            IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                            DATAINIZIOMAB = mvm.dataInizioMAB,
                            DATAFINEMAB=mvm.dataFineMAB,
                            DATAAGGIORNAMENTO=DateTime.Now,
                            FK_IDPERIODOMAB=null
                        };
                        db.PERIODOMAB.Add(new_pm);
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore in fase di creazione PERIODOMAB");
                        }
                        #endregion

                        #region associo percentualeMAB
                        using (dtMaggiorazioneAbitazione dtmab = new dtMaggiorazioneAbitazione())
                        {
                            //dtmab.RimuoviAssociazione_PerMAB_PercentualeMAB(periodoMAB_old.IDPERIODOMAB, db);
                            //RimuoviAssociazionePeriodoMAB_PercentualeMAB_var(periodoMAB_old.IDPERIODOMAB, db);
                            var pmm = GetPeriodoMABModel(new_pm.IDMAB, db);
                            var lpercentualemab = dtmab.GetListaPercentualeMAB(pmm, tm, db);
                            foreach (var percentualemab in lpercentualemab)
                            {
                                dtmab.Associa_PerMAB_PercentualeMAB(new_pm.IDPERIODOMAB, percentualemab.IDPERCMAB, db);
                            }
                        }
                        #endregion

                        #region anticipo annuale
                        ANTICIPOANNUALEMAB new_aa = new ANTICIPOANNUALEMAB()
                        {
                            IDMAB = mvm.idMAB,
                            IDATTIVAZIONEMAB = mvm.idAttivazioneMAB,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                            ANTICIPOANNUALE = mvm.anticipoAnnuale,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            FK_IDANTICIPOANNUALEMAB = null
                        };
                        db.ANTICIPOANNUALEMAB.Add(new_aa);
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore in fase di creazione Anticipo Annuale MAB");
                        }
                        #endregion

                        #region associa maggiorazione annuale
                        var ma = GetMaggiorazioneAnnuale_var(mvm, db);
                        if (mvm.anticipoAnnuale)
                        { 
                            if (ma.IDMAGANNUALI > 0)
                            {
                                Associa_MAB_MaggiorazioniAnnuali_var(newmab.IDMAB, ma.IDMAGANNUALI, db);
                            }
                        }
                        #endregion

                            //mvm.idAttivazioneMAB = amab.IDATTIVAZIONEMAB;
                        mvm.dataAggiornamento = DateTime.Now;
                            //mvm.idMAB = newmab.IDMAB;

                        #region crea CANONE
                        var ultimaMab = GetUltimaMABAttiva(idTrasferimento, db);
                        var vm = GetUltimaValutaInseritaModel(ultimaMab.IDMAB, db);

                        CanoneMABModel cmabm = new CanoneMABModel();
                        cmabm.idValuta = vm.idValuta;
                        cmabm.DataInizioValidita = mvm.dataInizioMAB;
                        cmabm.DataFineValidita = mvm.dataFineMAB;
                        cmabm.IDMAB = mvm.idMAB;
                        cmabm.IDAttivazioneMAB = mvm.idAttivazioneMAB;
                        cmabm.ImportoCanone = 0;
                        cmabm.DataAggiornamento = DateTime.Now;
                        cmabm.nascondi = false;
                        SetCanoneMAB(ref cmabm, db); //associa anche TFR
                        #endregion


                        #region inserisce/aggiorna eventuale pagato condiviso
                        //mam = this.GetMaggiorazioneAbitazioneModel(amm);
                        PagatoCondivisoMABModel pcm = new PagatoCondivisoMABModel();
                        pcm.idMAB = mvm.idMAB;
                        pcm.idAttivazioneMAB = mvm.idAttivazioneMAB;
                        pcm.DataInizioValidita = mvm.dataInizioMAB;
                        pcm.DataFineValidita = mvm.dataFineMAB;
                        pcm.Condiviso = false;
                        pcm.Pagato = false;
                        pcm.DataAggiornamento = DateTime.Now;
                        pcm.Nascondi = false;
                        pcm.fk_IDPagatoCondiviso = null;

                        SetPagatoCondivisoMAB(ref pcm, db); //associa anche percentuale condivisione
                        #endregion
                       
                    }

                    db.Database.CurrentTransaction.Commit();

                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }











        //    //=======================================
        //    //using (ModelDBISE db = new ModelDBISE())
        //    //{
        //    //    db.Database.BeginTransaction();

            //    //    try
            //    //    {
            //    //        #region ATTIVAZIONE MAB
            //    //        var amm = this.GetAttivazioneMAB_var(idTrasferimento);
            //    //        if (amm != null && amm.idAttivazioneMAB > 0)
            //    //        {
            //    //            mvm.idAttivazioneMAB = amm.idAttivazioneMAB;
            //    //        }
            //    //        else
            //    //        {
            //    //            var am = this.CreaAttivazioneMAB_var(idTrasferimento, db);
            //    //            amm = new AttivazioneMABModel()
            //    //            {
            //    //                Annullato = am.ANNULLATO,
            //    //                Attivazione = am.ATTIVAZIONE,
            //    //                dataAggiornamento = am.DATAAGGIORNAMENTO,
            //    //                dataAttivazione = am.DATAATTIVAZIONE,
            //    //                dataNotificaRichiesta = am.DATANOTIFICARICHIESTA,
            //    //                dataVariazione = am.DATAVARIAZIONE,
            //    //                idAttivazioneMAB = am.IDATTIVAZIONEMAB,
            //    //                idTrasferimento = am.IDTRASFERIMENTO,
            //    //                notificaRichiesta = am.NOTIFICARICHIESTA
            //    //            };
            //    //        }
            //    //        #endregion

            //    //        mvm.dataAggiornamento = DateTime.Now;

            //    //        #region nuova MAB
            //    //        decimal new_idMAB = this.SetMaggiorazioneAbitazione_var(ref mvm, db, amm.idAttivazioneMAB);
            //    //        #endregion

            //    //        //DateTime dtIni = mvm.dataInizioMAB;
            //    //        DateTime dtFin = mvm.ut_dataFineMAB == null ? Utility.DataFineStop() : mvm.ut_dataFineMAB.Value;
            //    //        //mvm.dataFineMAB = dtFin;
            //    //        mvm.idMAB = new_idMAB;

            //    //        #region anticipo annuale
            //    //        //da fare
            //    //        //if (mvm.AnticipoAnnuale)
            //    //        //{

            //    //        //    var mann = this.GetMaggiorazioneAnnuale_var(mvm, db);

            //    //        //    if (mann.idMagAnnuali > 0)
            //    //        //    {
            //    //        //        mvm.AnticipoAnnuale = mann.annualita;
            //    //        //        //associa MAB a MaggiorazioniAnnuali se esiste
            //    //        //        this.Associa_MAB_MaggiorazioniAnnuali_var(new_idMAB, mann.idMagAnnuali, db);
            //    //        //    }
            //    //        //    else
            //    //        //    {
            //    //        //        mvm.AnticipoAnnuale = false;
            //    //        //    }

            //    //        //}
            //    //        #endregion

            //    //        #region associa MAB a tutte le percentuali MAB trovate
            //    //        var lista_perc = this.GetListaPercentualeMAB_var(idTrasferimento, db);
            //    //        if (lista_perc?.Any() ?? false)
            //    //        {
            //    //            foreach (var perc in lista_perc)
            //    //            {
            //    //                this.Associa_MAB_PercenualeMAB_var(new_idMAB, perc.IDPERCMAB, db);
            //    //            }
            //    //        }
            //    //        #endregion

            //    //        #region inserisci CANONE
            //    //        CANONEMAB c = this.SetCanoneMAB_var(mvm, db);
            //    //        #endregion

            //    //        #region associa canone MAB a TFR
            //    //        using (dtTFR dtt = new dtTFR())
            //    //        {
            //    //            using (dtTrasferimento dttrasf = new dtTrasferimento())
            //    //            {
            //    //                var t = dttrasf.GetTrasferimentoById(idTrasferimento);
            //    //                var ltfr = dtt.GetListaTfrByValuta_RangeDate(t.idUfficio, mvm.id_Valuta, dtIni, dtFin, db);

            //    //                if (ltfr?.Any() ?? false)
            //    //                {
            //    //                    foreach (var tfr in ltfr)
            //    //                    {
            //    //                        this.Associa_TFR_CanoneMAB_var(tfr.idTFR, c.IDCANONE, db);
            //    //                    }
            //    //                }
            //    //            }
            //    //        }
            //    //        #endregion

            //    //        #region inserisce eventuale pagato condiviso
            //    //        if (mvm.canone_condiviso)
            //    //        {
            //    //            PAGATOCONDIVISOMAB pc = this.SetPagatoCondivisoMAB_var(mvm, db);

            //    //            #region associa percentuale condivisione
            //    //            var lpercCond = this.GetListaPercentualeCondivisione_var(pc.DATAINIZIOVALIDITA, pc.DATAFINEVALIDITA, db);
            //    //            if (lpercCond?.Any() ?? false)
            //    //            {
            //    //                foreach (var percCond in lpercCond)
            //    //                {
            //    //                    this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(pc.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
            //    //                }
            //    //            }
            //    //            else
            //    //            {
            //    //                throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
            //    //            }
            //    //            #endregion

            //    //        }
            //    //        #endregion

            //    //        db.Database.CurrentTransaction.Commit();

            //    //    }
            //    //    catch (Exception ex)
            //    //    {
            //    //        db.Database.CurrentTransaction.Rollback();
            //    //        throw ex;
            //    //    }
            //    //}
            //}

        public void AggiornaMAB_var(MABViewModel mabvm, decimal idTrasferimento, decimal idMAB)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        using (dtTFR dtTFR = new dtTFR())
                        {
                            #region ATTIVAZIONE MAB
                            ATTIVAZIONEMAB amab = new ATTIVAZIONEMAB();
                            MABModel mabm = new MABModel();
                            MAB mab = new MAB();
                            TrasferimentoModel tm = new TrasferimentoModel();

                            tm = dtt.GetTrasferimentoById(idTrasferimento);

                            amab = GetAttivazioneAperta(idTrasferimento, db);

                            if (amab != null && amab.IDATTIVAZIONEMAB > 0)
                            {
                                mabvm.idAttivazioneMAB = amab.IDATTIVAZIONEMAB;                                
                            }
                            else
                            {
                                amab = CreaAttivazioneMAB_var(idMAB, db);
                                mabvm.idAttivazioneMAB = amab.IDATTIVAZIONEMAB;
                            }
                            #endregion

                            mabvm.dataAggiornamento = DateTime.Now;

                            DateTime dtIni = mabvm.dataInizioMAB;
                            DateTime dtFin = Utility.DataFineStop();
                            if (mabvm.ut_dataFineMAB != null)
                            {
                                dtFin = mabvm.ut_dataFineMAB.Value;
                            }
                            mabvm.dataFineMAB = dtFin;

                            mab = db.MAB.Find(idMAB);

                            var periodoMAB_old = GetPeriodoMAB(idMAB, db);


                            if (periodoMAB_old.DATAINIZIOMAB != dtIni)
                            {

                                if (periodoMAB_old.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
                                {
                                    #region se in lavorazione aggiorno dataini

                                    periodoMAB_old.DATAINIZIOMAB = dtIni;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la fase di aggiornamento data inizio MAB.");
                                    }
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica riga PERIODO MAB.", "PERIODOMAB", db, idTrasferimento, periodoMAB_old.IDPERIODOMAB);
                                    #endregion

                                    #region riassocio percentualeMAB
                                    using (dtMaggiorazioneAbitazione dtmab = new dtMaggiorazioneAbitazione())
                                    {
                                        dtmab.RimuoviAssociazione_PerMAB_PercentualeMAB(periodoMAB_old.IDPERIODOMAB, db);
                                        //RimuoviAssociazionePeriodoMAB_PercentualeMAB_var(periodoMAB_old.IDPERIODOMAB, db);
                                        var pmm = GetPeriodoMABModel(periodoMAB_old.IDMAB, db);
                                        var lpercentualemab = dtmab.GetListaPercentualeMAB(pmm, tm, db);
                                        foreach (var percentualemab in lpercentualemab)
                                        {
                                            dtmab.Associa_PerMAB_PercentualeMAB(periodoMAB_old.IDPERIODOMAB, percentualemab.IDPERCMAB, db);
                                        }
                                    }
                                    #endregion

                                }

                                #region allineo i periodi di CANONEMAB in base a datainizioMAB e riassocio TFR
                                //intercetto il periodo dove ricade datainizioMAB
                                var lcanoneMAB_curr = mab.CANONEMAB
                                                .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                            a.NASCONDI == false &&
                                                            a.DATAFINEVALIDITA > dtIni)
                                                .OrderByDescending(a => a.DATAFINEVALIDITA)
                                                .ToList();

                                if (lcanoneMAB_curr?.Any() ?? false)
                                {
                                    //nascondo il record
                                    var canoneMAB_curr = lcanoneMAB_curr.First();
                                    canoneMAB_curr.NASCONDI = true;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la cessazione MAB (allinemento data inizio CANONE).");
                                    }
                                    //creo nuovo periodo
                                    CANONEMAB new_canoneMAB = new CANONEMAB()
                                    {
                                        IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
                                        IDMAB = mab.IDMAB,
                                        DATAINIZIOVALIDITA = dtIni,
                                        DATAFINEVALIDITA = canoneMAB_curr.DATAFINEVALIDITA,
                                        IMPORTOCANONE = canoneMAB_curr.IMPORTOCANONE,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDVALUTA = canoneMAB_curr.IDVALUTA,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        FK_IDCANONE = canoneMAB_curr.FK_IDCANONE,
                                        NASCONDI = false
                                    };
                                    db.CANONEMAB.Add(new_canoneMAB);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la fase di aggiornamento MAB (replica canoneMAB).");
                                    }
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova riga CANONE MAB.", "CANONEMAB", db, idTrasferimento, new_canoneMAB.IDCANONE);

                                    //associo TFR
                                    var ltfr = dtTFR.GetListaTfrByValuta_RangeDate(tm, new_canoneMAB.IDVALUTA, new_canoneMAB.DATAINIZIOVALIDITA, new_canoneMAB.DATAFINEVALIDITA, db);

                                    if (ltfr?.Any() ?? false)
                                    {
                                        foreach (var tfr in ltfr)
                                        {
                                            Associa_TFR_CanoneMAB_var(tfr.idTFR, new_canoneMAB.IDCANONE, db);
                                        }
                                    }

                                    //nascondo tutti i canoni precedenti
                                    var lperiodicanoneMAB = mab.CANONEMAB.Where(a => 
                                                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && 
                                                            a.NASCONDI == false &&
                                                            a.DATAFINEVALIDITA <= dtIni)
                                                            .ToList();
                                    foreach (var periodicanoneMAB in lperiodicanoneMAB)
                                    {
                                        periodicanoneMAB.NASCONDI = true;
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore durante la cessazione MAB (allinemento data fine CANONE).");
                                        }

                                    }
                                }
                                #endregion


                                #region allineo i periodi PAGATOCONDIVISOMAB e riassocio percentuali
                                var lpagatocondivisoMAB_curr = mab.PAGATOCONDIVISOMAB
                                                            .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                            a.NASCONDI == false &&
                                                            a.DATAFINEVALIDITA > dtIni)
                                                            .OrderByDescending(a => a.DATAFINEVALIDITA)
                                                            .ToList();
                                if (lpagatocondivisoMAB_curr?.Any() ?? false)
                                {

                                    //nascondo il record
                                    var pagatocondivisoMAB_curr = lpagatocondivisoMAB_curr.First();
                                    pagatocondivisoMAB_curr.NASCONDI = true;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la cessazione MAB (allinemento data inizio PAGATOCONDIVISO).");
                                    }

                                    PAGATOCONDIVISOMAB new_pagatocondiviso = new PAGATOCONDIVISOMAB()
                                    {
                                        IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
                                        IDMAB = mab.IDMAB,
                                        DATAINIZIOVALIDITA = dtIni,
                                        DATAFINEVALIDITA = pagatocondivisoMAB_curr.DATAFINEVALIDITA,
                                        PAGATO = pagatocondivisoMAB_curr.PAGATO,
                                        CONDIVISO = pagatocondivisoMAB_curr.CONDIVISO,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        FK_IDPAGATOCONDIVISO = pagatocondivisoMAB_curr.FK_IDPAGATOCONDIVISO,
                                        NASCONDI = false
                                    };
                                    db.PAGATOCONDIVISOMAB.Add(new_pagatocondiviso);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la fase di cessazione MAB (allineamento datainizio pagatocondiviso MAB).");
                                    }
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova riga PAGATOCONDIVISOMAB.", "PAGATOCONDIVISOMAB", db, idTrasferimento, new_pagatocondiviso.IDPAGATOCONDIVISO);

                                    //nascondo tutti i pagatocondiviso precedenti
                                    var lperiodipagatocondivisoMAB = mab.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.NASCONDI == false &&
                                                                                  a.DATAFINEVALIDITA<=dtIni).ToList();
                                    foreach (var periodipagatocondivisoMAB in lperiodipagatocondivisoMAB)
                                    {
                                        periodipagatocondivisoMAB.NASCONDI = true;
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore durante la cessazione MAB (allinemento data inizio PAGATOCONDIVISOMAB).");
                                        }
                                    }


                                    if (new_pagatocondiviso.CONDIVISO)
                                    {
                                        var lpercCond = GetListaPercentualeCondivisione_var(new_pagatocondiviso.DATAINIZIOVALIDITA, new_pagatocondiviso.DATAFINEVALIDITA, db);
                                        if (lpercCond?.Any() ?? false)
                                        {
                                            //riassocio le percentuali
                                            foreach (var percCond in lpercCond)
                                            {
                                                this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(new_pagatocondiviso.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
                                        }
                                    }
                                }
                                #endregion
                            }


                            if (periodoMAB_old.DATAFINEMAB != dtFin)
                            {

                                if (periodoMAB_old.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
                                {
                                    #region se in lavorazione aggiorno datafine

                                    periodoMAB_old.DATAFINEMAB = dtFin;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la fase di aggiornamento data fine MAB.");
                                    }
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica riga PERIODO MAB.", "PERIODOMAB", db, idTrasferimento, periodoMAB_old.IDPERIODOMAB);
                                    #endregion

                                    #region riassocio percentualeMAB
                                    using (dtMaggiorazioneAbitazione dtmab = new dtMaggiorazioneAbitazione())
                                    {
                                        dtmab.RimuoviAssociazione_PerMAB_PercentualeMAB(periodoMAB_old.IDPERIODOMAB, db);
                                        //RimuoviAssociazionePeriodoMAB_PercentualeMAB_var(periodoMAB_old.IDPERIODOMAB, db);
                                        var pmm = GetPeriodoMABModel(periodoMAB_old.IDMAB, db);
                                        var lpercentualemab = dtmab.GetListaPercentualeMAB(pmm, tm, db);
                                        foreach (var percentualemab in lpercentualemab)
                                        {
                                            dtmab.Associa_PerMAB_PercentualeMAB(periodoMAB_old.IDPERIODOMAB, percentualemab.IDPERCMAB, db);
                                        }
                                    }
                                    #endregion

                                }
                                else
                                {
                                    #region se non in lavorazione replica PERIODOMAB
                                    if (periodoMAB_old.IDPERIODOMAB > 0)
                                    {
                                        PERIODOMAB new_periodoMAB = new PERIODOMAB()
                                        {
                                            IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
                                            IDMAB = mab.IDMAB,
                                            DATAINIZIOMAB = periodoMAB_old.DATAINIZIOMAB,
                                            DATAFINEMAB = dtFin,
                                            DATAAGGIORNAMENTO = DateTime.Now,
                                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                            FK_IDPERIODOMAB = periodoMAB_old.FK_IDPERIODOMAB
                                        };
                                        db.PERIODOMAB.Add(new_periodoMAB);
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore durante la fase di aggiornamento MAB (replica periodoMAB).");
                                        }
                                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova riga PERIODO MAB.", "PERIODOMAB", db, idTrasferimento, new_periodoMAB.IDPERIODOMAB);

                                        periodoMAB_old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore durante la fase di aggiornamento MAB (annullamento record periodoMAB ).");
                                        }

                                        #region associo percentualeMAB
                                        using (dtMaggiorazioneAbitazione dtmab = new dtMaggiorazioneAbitazione())
                                        {
                                            //dtmab.RimuoviAssociazione_PerMAB_PercentualeMAB(periodoMAB_old.IDPERIODOMAB, db);
                                            //RimuoviAssociazionePeriodoMAB_PercentualeMAB_var(periodoMAB_old.IDPERIODOMAB, db);
                                            var pmm = GetPeriodoMABModel(new_periodoMAB.IDMAB, db);
                                            var lpercentualemab = dtmab.GetListaPercentualeMAB(pmm, tm, db);
                                            foreach (var percentualemab in lpercentualemab)
                                            {
                                                dtmab.Associa_PerMAB_PercentualeMAB(new_periodoMAB.IDPERIODOMAB, percentualemab.IDPERCMAB, db);
                                            }
                                        }
                                        #endregion

                                    }
                                    #endregion
                                }

                                #region allineo i periodi di CANONEMAB in base a datafineMAB e riassocio TFR
                                //intercetto il periodo dove ricade datafineMAB
                                var lcanoneMAB_curr = mab.CANONEMAB
                                                .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                            a.NASCONDI == false &&
                                                            a.DATAINIZIOVALIDITA <= dtFin)
                                                .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                                if (lcanoneMAB_curr?.Any() ?? false)
                                {
                                    //nascondo il record
                                    var canoneMAB_curr = lcanoneMAB_curr.First();
                                    canoneMAB_curr.NASCONDI = true;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la cessazione MAB (allinemento data fine CANONE).");
                                    }
                                    //creo nuovo periodo
                                    CANONEMAB new_canoneMAB = new CANONEMAB()
                                    {
                                        IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
                                        IDMAB = mab.IDMAB,
                                        DATAINIZIOVALIDITA = canoneMAB_curr.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = dtFin,
                                        IMPORTOCANONE = canoneMAB_curr.IMPORTOCANONE,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDVALUTA = canoneMAB_curr.IDVALUTA,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        FK_IDCANONE = canoneMAB_curr.FK_IDCANONE,
                                        NASCONDI = false
                                    };
                                    db.CANONEMAB.Add(new_canoneMAB);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la fase di aggiornamento MAB (replica canoneMAB).");
                                    }
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova riga CANONE MAB.", "CANONEMAB", db, idTrasferimento, new_canoneMAB.IDCANONE);

                                    //associo TFR
                                    var ltfr = dtTFR.GetListaTfrByValuta_RangeDate(tm, new_canoneMAB.IDVALUTA, new_canoneMAB.DATAINIZIOVALIDITA, new_canoneMAB.DATAFINEVALIDITA, db);

                                    if (ltfr?.Any() ?? false)
                                    {
                                        foreach (var tfr in ltfr)
                                        {
                                            Associa_TFR_CanoneMAB_var(tfr.idTFR, new_canoneMAB.IDCANONE, db);
                                        }
                                    }

                                    //nascondo tutti i canoni successivi
                                    var lperiodicanoneMAB = mab.CANONEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.NASCONDI == false &&
                                                                                  a.DATAINIZIOVALIDITA > dtFin).ToList();
                                    foreach (var periodicanoneMAB in lperiodicanoneMAB)
                                    {
                                        periodicanoneMAB.NASCONDI = true;
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore durante la cessazione MAB (allinemento data fine CANONE).");
                                        }

                                    }
                                }
                                #endregion

                                #region allineo i periodi PAGATOCONDIVISOMAB e riassocio percentuali
                                var lpagatocondivisoMAB_curr = mab.PAGATOCONDIVISOMAB
                                                            .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                            a.NASCONDI == false &&
                                                            a.DATAINIZIOVALIDITA <= dtFin)
                                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                                            .ToList();
                                if (lpagatocondivisoMAB_curr?.Any() ?? false)
                                {

                                    //nascondo il record
                                    var pagatocondivisoMAB_curr = lpagatocondivisoMAB_curr.First();
                                    pagatocondivisoMAB_curr.NASCONDI = true;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la cessazione MAB (allinemento data fine PAGATOCONDIVISO).");
                                    }

                                    PAGATOCONDIVISOMAB new_pagatocondiviso = new PAGATOCONDIVISOMAB()
                                    {
                                        IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
                                        IDMAB = mab.IDMAB,
                                        DATAINIZIOVALIDITA = pagatocondivisoMAB_curr.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = dtFin,
                                        PAGATO = pagatocondivisoMAB_curr.PAGATO,
                                        CONDIVISO = pagatocondivisoMAB_curr.CONDIVISO,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        FK_IDPAGATOCONDIVISO = pagatocondivisoMAB_curr.FK_IDPAGATOCONDIVISO,
                                        NASCONDI = false
                                    };
                                    db.PAGATOCONDIVISOMAB.Add(new_pagatocondiviso);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la fase di cessazione MAB (allineamento datafine pagatocondiviso MAB).");
                                    }
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova riga PAGATOCONDIVISOMAB.", "PAGATOCONDIVISOMAB", db, idTrasferimento, new_pagatocondiviso.IDPAGATOCONDIVISO);

                                    //nascondo tutti i pagatocondiviso successivi
                                    var lperiodipagatocondivisoMAB = mab.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.NASCONDI == false &&
                                                                                  a.DATAINIZIOVALIDITA > dtFin).ToList();
                                    foreach (var periodipagatocondivisoMAB in lperiodipagatocondivisoMAB)
                                    {
                                        periodipagatocondivisoMAB.NASCONDI = true;
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore durante la cessazione MAB (allinemento data fine PAGATOCONDIVISOMAB).");
                                        }
                                    }


                                    if (new_pagatocondiviso.CONDIVISO)
                                    {
                                        var lpercCond = GetListaPercentualeCondivisione_var(new_pagatocondiviso.DATAINIZIOVALIDITA, new_pagatocondiviso.DATAFINEVALIDITA, db);
                                        if (lpercCond?.Any() ?? false)
                                        {
                                            //riassocio le percentuali
                                            foreach (var percCond in lpercCond)
                                            {
                                                this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(new_pagatocondiviso.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
                                        }
                                    }
                                }
                                #endregion
                            }

                            #region aggiorno anticipo annuale se necessario
                            var ant_ann = GetAnticipoAnnualeMAB(mabvm.idMAB, db);
                            if (mabvm.anticipoAnnuale != ant_ann.ANTICIPOANNUALE)
                            {
                                if (ant_ann.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
                                {
                                    ant_ann.ANTICIPOANNUALE = mabvm.anticipoAnnuale;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante l'aggiornamento dell'anticipo annuale MAB.");
                                    }
                                }
                                else
                                {
                                    var ant_ann_new = new ANTICIPOANNUALEMAB()
                                    {
                                        IDMAB=mabvm.idMAB,
                                        IDATTIVAZIONEMAB=amab.IDATTIVAZIONEMAB,
                                        IDSTATORECORD=(decimal)EnumStatoRecord.In_Lavorazione,
                                        ANTICIPOANNUALE= mabvm.anticipoAnnuale,
                                        DATAAGGIORNAMENTO=DateTime.Now,
                                        FK_IDANTICIPOANNUALEMAB=ant_ann.IDANTICIPOANNUALEMAB
                                    };
                                    db.ANTICIPOANNUALEMAB.Add(ant_ann_new);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante l'aggiornamento dell'anticipo annuale MAB (creazione nuovo record ANTICIPOANNUALEMAB.");
                                    }
                                }

                            }
                            #endregion

                        }
                    }

                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }


        public void TerminaMABbyDataFineTrasf(decimal idTrasferimento, DateTime dtFineTrasf, ModelDBISE db)
        {
            try
            {

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoById(idTrasferimento);
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    var lmab = t.INDENNITA.MAB
                                        .Where(a => 
                                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && 
                                            a.RINUNCIAMAB==false)
                                        .OrderBy(a => a.IDMAB)
                                        .ToList();
                    foreach(var mab in lmab)
                    {

                        var pmab = GetPeriodoMAB(mab.IDMAB, db);
                        if (pmab.DATAINIZIOMAB>dtFineTrasf)
                        {
                            #region annullo MAB
                            mab.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore durante l'annullamento MAB successive alla fine trasferimento.");
                            }
                            #endregion

                            #region annullo periodo mab
                            pmab.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore durante l'annullamento Periodo MAB successivo alla fine trasferimento.");
                            }
                            #endregion

                            #region annullo canoni associati attivi
                            var lcmab =mab.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDMAB).ToList();
                            foreach (var cmab in lcmab)
                            {
                                cmab.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante l'annullamento canoni MAB relativi a MAB successive alla fine trasferimento.");
                                }
                            }
                            #endregion

                            #region elimino eventuali canoni in lavorazione o da attivare
                            var lcmabNonAttivati = mab.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione ||
                                                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderByDescending(a => a.IDMAB).ToList();
                            foreach (var cmabNonAttivati in lcmabNonAttivati)
                            {
                                db.CANONEMAB.Remove(cmabNonAttivati);
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante la cancellazione canoni MAB (in lavorazione o da attivare) relativi a MAB successive alla fine trasferimento.");
                                }
                            }
                            #endregion

                            //#region annullo pagato condiviso associato
                            //pmab.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                            //if (db.SaveChanges() <= 0)
                            //{
                            //    throw new Exception("Errore durante l'annullamento Periodo MAB successivo alla fine trasferimento.");
                            //}
                            //#endregion

                            #region annullo pagato condiviso associati attivi
                            var lpcmab = mab.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDMAB).ToList();
                            foreach (var pcmab in lpcmab)
                            {
                                pcmab.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante l'annullamento PagatoCondiviso MAB relativi a MAB successive alla fine trasferimento.");
                                }
                            }
                            #endregion

                            #region elimino eventuali pagatocondiviso in lavorazione o da attivare
                            var lpcmabNonAttivati = mab.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione ||
                                                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderByDescending(a => a.IDMAB).ToList();
                            foreach (var pcmabNonAttivati in lpcmabNonAttivati)
                            {
                                db.PAGATOCONDIVISOMAB.Remove(pcmabNonAttivati);
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante la cancellazione Pagato Condiviso MAB (in lavorazione o da attivare) relativi a MAB successive alla fine trasferimento.");
                                }
                            }
                            #endregion

                            #region annullo documenti associati attivi
                            // da fare appena MAB => DOCUMENTI
                            var l_attmab = mab.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA && a.ATTIVAZIONE).ToList();
                            foreach (var attmab in l_attmab)
                            {
                                var ldoc = attmab.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                foreach (var doc in ldoc)
                                {
                                    doc.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante l'annullamento documenti relativi a MAB successive alla fine trasferimento.");
                                    }
                                }
                            }
                            #endregion

                            #region elimino documenti in lavorazione o da attivare
                            // da fare appena MAB => DOCUMENTI
                            foreach (var attmab in l_attmab)
                            {
                                var ldoc = attmab.DOCUMENTI.Where(a => 
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione ||
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                foreach (var doc in ldoc)
                                {
                                    db.DOCUMENTI.Remove(doc);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la cancellazione di documenti non attivati relativi a MAB successive alla fine trasferimento.");
                                    }
                                }
                            }
                            #endregion

                        }
                        else
                        {
                            if(pmab.DATAFINEMAB>dtFineTrasf)
                            {                           
                       
                                #region allineo i periodi PERIODOMAB e riassocio percentuali
                                var lperiodoMAB_curr = mab.PERIODOMAB
                                                            .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                                        a.DATAINIZIOMAB <= dtFineTrasf)
                                                            .OrderByDescending(a => a.DATAINIZIOMAB)
                                                            .ToList();
                                if (lperiodoMAB_curr?.Any() ?? false)
                                {
                                    //aggiorno data fine validita
                                    var periodoMAB_curr = lperiodoMAB_curr.First();
                                    periodoMAB_curr.DATAFINEMAB = dtFineTrasf;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la cessazione MAB.");
                                    }

                                    RimuoviAssociazionePeriodoMAB_PercentualeMAB_var(periodoMAB_curr.IDPERIODOMAB, db);

                                    var lpercMAB = GetListaPercentualeMAB_var(idTrasferimento, db);
                                    if (lpercMAB?.Any() ?? false)
                                    {
                                        //riassocio le percentuali
                                        foreach (var percMAB in lpercMAB)
                                        {
                                            Associa_PERIODOMAB_PercentualeMAB_var(periodoMAB_curr.IDPERIODOMAB, percMAB.IDPERCMAB, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Non è stata trovata la percentuale MAB per il periodo richiesto.");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Errore nella chiusura MAB. Nessun periodo MAB trovato.");
                                }
                                #endregion

                                #region allineo i periodi PAGATOCONDIVISOMAB e riassocio percentuali
                                var lpagatocondivisoMAB_curr = mab.PAGATOCONDIVISOMAB
                                                                    .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                                    a.NASCONDI == false &&
                                                                    a.DATAINIZIOVALIDITA <= dtFineTrasf)
                                                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                                                    .ToList();
                                if (lpagatocondivisoMAB_curr?.Any() ?? false)
                                {
                                    //aggiorno data fine validita
                                    var pagatocondivisoMAB_curr = lpagatocondivisoMAB_curr.First();
                                    pagatocondivisoMAB_curr.DATAFINEVALIDITA = dtFineTrasf;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la cessazione Pagato Condiviso MAB.");
                                    }


                                    //annullo tutti i pagatocondiviso successivi
                                    var lperiodipagatocondivisoMAB = mab.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.NASCONDI == false &&
                                                                          a.DATAINIZIOVALIDITA > dtFineTrasf).ToList();
                                    foreach (var periodipagatocondivisoMAB in lperiodipagatocondivisoMAB)
                                    {
                                        periodipagatocondivisoMAB.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore durante la cessazione Pagato Condiviso MAB (annullamento periodi successivi).");
                                        }
                                    }

                                    //riassocio le percentuali se necessario
                                    if (pagatocondivisoMAB_curr.CONDIVISO)
                                    {
                                        RimuoviAssociazionePagatoCondiviso_PercentualeCondivisione_var(pagatocondivisoMAB_curr.IDPAGATOCONDIVISO, db);

                                        var lpercCond = GetListaPercentualeCondivisione_var(pagatocondivisoMAB_curr.DATAINIZIOVALIDITA, pagatocondivisoMAB_curr.DATAFINEVALIDITA, db);
                                        if (lpercCond?.Any() ?? false)
                                        {
                                            foreach (var percCond in lpercCond)
                                            {
                                                this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(pagatocondivisoMAB_curr.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Errore nella chiusura MAB. Nessuna pagato Condiviso MAB trovato.");
                                }
                                #endregion

                                #region allineo i periodi di CANONEMAB in base a datafineMAB e riassocio TFR
                                //intercetto il periodo dove ricade datafineTrasferimento
                                var lcanoneMAB_curr = mab.CANONEMAB
                                        .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                    a.NASCONDI == false &&
                                                    a.DATAINIZIOVALIDITA <= dtFineTrasf)
                                        .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                        .ToList();

                                if (lcanoneMAB_curr?.Any() ?? false)
                                {

                                    var canoneMAB_curr = lcanoneMAB_curr.First();
                                    canoneMAB_curr.DATAFINEVALIDITA = dtFineTrasf;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la cessazione canone MAB.");
                                    }

                                    //riassocio TFR
                                    RimuoviAssociazioneCanoneMAB_TFR_var(canoneMAB_curr.IDCANONE, db);

                                    using (dtTFR dttfr = new dtTFR())
                                    {
                                        var ltfr = dttfr.GetListaTfrByValuta_RangeDate(tm, canoneMAB_curr.IDVALUTA, canoneMAB_curr.DATAINIZIOVALIDITA, canoneMAB_curr.DATAFINEVALIDITA, db);

                                        if (ltfr?.Any() ?? false)
                                        {
                                            foreach (var tfr in ltfr)
                                            {
                                                Associa_TFR_CanoneMAB_var(tfr.idTFR, canoneMAB_curr.IDCANONE, db);
                                            }
                                        }
                                    }

                                    //annullo tutti i canoni successivi
                                    var lperiodicanoneMAB = mab.CANONEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.NASCONDI == false &&
                                                                                  a.DATAINIZIOVALIDITA > dtFineTrasf).ToList();
                                    foreach (var periodicanoneMAB in lperiodicanoneMAB)
                                    {
                                        periodicanoneMAB.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore durante la cessazione canone MAB (annullamento canoni successivi).");
                                        }

                                    }
                                }
                                else
                                {
                                    throw new Exception("Errore nella chiusura MAB. Nessun canone MAB trovato.");
                                }
                                #endregion

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


        #region rimuovi associazioni

        public void RimuoviAssociazionePeriodoMAB_PercentualeMAB_var(decimal idPeriodoMAB, ModelDBISE db)
        {
            var pmab = db.PERIODOMAB.Find(idPeriodoMAB);
            var lpercMAB = pmab.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
            if (lpercMAB?.Any() ?? false)
            {
                foreach (var percMAB in lpercMAB)
                {
                    pmab.PERCENTUALEMAB.Remove(percMAB);
                }

                db.SaveChanges();
            }

        }


        public void RimuoviAssociazionePagatoCondiviso_PercentualeCondivisione_var(decimal idPagatoCondiviso, ModelDBISE db)
        {
            var pc = db.PAGATOCONDIVISOMAB.Find(idPagatoCondiviso);
            var lpercCond = pc.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).ToList();
            if (lpercCond?.Any() ?? false)
            {
                foreach (var percCond in lpercCond)
                {
                    pc.PERCENTUALECONDIVISIONE.Remove(percCond);
                }

                db.SaveChanges();
            }

        }

        public void RimuoviAssociazioneCanoneMAB_TFR_var(decimal idCanone, ModelDBISE db)
        {
            var c = db.CANONEMAB.Find(idCanone);
            var lTFR = c.TFR.Where(a => a.ANNULLATO == false).ToList();
            if (lTFR?.Any() ?? false)
            {
                foreach (var TFR in lTFR)
                {
                    c.TFR.Remove(TFR);
                }

                db.SaveChanges();
            }

        }
        #endregion

        #region associa

        public void Associa_MAB_MaggiorazioniAnnuali_var(decimal idMAB, decimal idMaggiorazioniAnnuali, ModelDBISE db)
        {
            try
            {
                var mab = db.MAB.Find(idMAB);
                var item = db.Entry<MAB>(mab);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.MAGGIORAZIONIANNUALI).Load();
                var ma = db.MAGGIORAZIONIANNUALI.Find(idMaggiorazioniAnnuali);
                mab.MAGGIORAZIONIANNUALI.Add(ma);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare MAB a MaggiorazioniAnnuali."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Associa_PERIODOMAB_PercentualeMAB_var(decimal idPeriodoMAB, decimal idPercMAB, ModelDBISE db)
        {
            try
            {
                var pmab = db.PERIODOMAB.Find(idPeriodoMAB);
                var item = db.Entry<PERIODOMAB>(pmab);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PERCENTUALEMAB).Load();
                var percmab = db.PERCENTUALEMAB.Find(idPercMAB);
                pmab.PERCENTUALEMAB.Add(percmab);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare PERIODOMAB a PercentualeMAB."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public void Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(decimal idPagatoCondiviso, decimal idPercCond, ModelDBISE db)
        {
            try
            {
                var pcmab = db.PAGATOCONDIVISOMAB.Find(idPagatoCondiviso);
                var item = db.Entry<PAGATOCONDIVISOMAB>(pcmab);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PERCENTUALECONDIVISIONE).Load();
                var pc = db.PERCENTUALECONDIVISIONE.Find(idPercCond);
                pcmab.PERCENTUALECONDIVISIONE.Add(pc);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare PagatoCondivisoMAB a PercentualeCondivisione."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Associa_TFR_CanoneMAB_var(decimal idTFR, decimal idCanoneMAB, ModelDBISE db)
        {
            try
            {
                var tfr = db.TFR.Find(idTFR);
                var item = db.Entry<TFR>(tfr);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.CANONEMAB).Load();
                var c = db.CANONEMAB.Find(idCanoneMAB);
                tfr.CANONEMAB.Add(c);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il canone MAB a TFR."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Associa_perMAB_PercenualeMAB_var(decimal idPeriodoMAB, decimal idPercMAB, ModelDBISE db)
        {
            try
            {
                var pm = db.PERIODOMAB.Find(idPeriodoMAB);
                var item = db.Entry<PERIODOMAB>(pm);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PERCENTUALEMAB).Load();
                var p = db.PERCENTUALEMAB.Find(idPercMAB);
                pm.PERCENTUALEMAB.Add(p);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare PERIODOMAB a PercentualeMAB."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        public List<CanoneMABModel> GetCanoneMABModel(decimal idMab, ModelDBISE db)
        {
            try
            {
                List<CanoneMABModel> lcmm = new List<CanoneMABModel>();

                var mab = db.MAB.Find(idMab);
                var t = mab.INDENNITA.TRASFERIMENTO;

                var lcm =
                        mab.CANONEMAB.Where(
                            a =>
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Nullo && 
                                a.NASCONDI==false && 
                                ((a.DATAINIZIOVALIDITA<=t.DATARIENTRO&&a.DATAFINEVALIDITA>=t.DATARIENTRO)||a.DATAFINEVALIDITA<t.DATARIENTRO))
                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                if (lcm?.Any() ?? false)
                {
                    foreach (var cm in lcm)
                    {
                        var cmm = new CanoneMABModel()
                        {
                            idCanone=cm.IDCANONE,
                            IDMAB = cm.IDMAB,
                            IDAttivazioneMAB = cm.IDATTIVAZIONEMAB,
                            idValuta=cm.IDVALUTA,
                            DataInizioValidita = cm.DATAINIZIOVALIDITA,
                            DataFineValidita = cm.DATAFINEVALIDITA>t.DATARIENTRO?t.DATARIENTRO:cm.DATAFINEVALIDITA,
                            ImportoCanone=cm.IMPORTOCANONE,
                            DataAggiornamento = cm.DATAAGGIORNAMENTO,
                            idStatoRecord = cm.IDSTATORECORD,
                            FK_IDCanone = cm.FK_IDCANONE
                        };
                        lcmm.Add(cmm);
                    }
                }
                else
                {
                    throw new Exception(string.Format("nessun Canone MAB trovato."));
                }


                return lcmm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PagatoCondivisoMABModel> GetPagatoCondivisoMABModel(decimal idMab, ModelDBISE db)
        {
            try
            {
                List<PagatoCondivisoMABModel> lpcmabm = new List<PagatoCondivisoMABModel>();

                var mab = db.MAB.Find(idMab);
                var t = mab.INDENNITA.TRASFERIMENTO;

                var lpcmab =
                        mab.PAGATOCONDIVISOMAB.Where(
                            a =>
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Nullo &&
                                ((a.DATAINIZIOVALIDITA<=t.DATARIENTRO && a.DATAFINEVALIDITA>=t.DATARIENTRO)||a.DATAFINEVALIDITA<t.DATARIENTRO) &&
                                a.NASCONDI == false)
                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                if (lpcmab?.Any() ?? false)
                {
                    foreach (var pcmab in lpcmab)
                    {
                        var pcmabm = new PagatoCondivisoMABModel()
                        {
                            idPagatoCondiviso = pcmab.IDPAGATOCONDIVISO,
                            idMAB = pcmab.IDMAB,
                            idAttivazioneMAB = pcmab.IDATTIVAZIONEMAB,
                            idStatoRecord = pcmab.IDSTATORECORD,
                            DataInizioValidita = pcmab.DATAINIZIOVALIDITA,
                            DataFineValidita = pcmab.DATAFINEVALIDITA>t.DATARIENTRO?t.DATARIENTRO:pcmab.DATAFINEVALIDITA,
                            Pagato = pcmab.PAGATO,
                            Condiviso=pcmab.CONDIVISO,
                            DataAggiornamento = pcmab.DATAAGGIORNAMENTO,
                            fk_IDPagatoCondiviso = pcmab.FK_IDPAGATOCONDIVISO,
                            Nascondi= pcmab.NASCONDI
                        };
                        lpcmabm.Add(pcmabm);
                    }
                }
                else
                {
                    throw new Exception(string.Format("nessun Pagato Condiviso MAB trovato."));
                }


                return lpcmabm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<CANONEMAB> GetCanoneMAB(decimal idMab, ModelDBISE db)
        {
            try
            {
                List<CANONEMAB> lcm = new List<CANONEMAB>();

                var mab = db.MAB.Find(idMab);

                var lcmab =
                        mab.CANONEMAB.Where(
                            a =>
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Nullo)
                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                if (lcmab?.Any() ?? false)
                {
                    lcm = lcmab;
                }
                else
                {
                    throw new Exception(string.Format("nessun Canone MAB trovato."));
                }
                return lcm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CANONEMAB GetUltimoCanoneMABInserito(decimal idMab, ModelDBISE db)
        {
            try
            {
                CANONEMAB cmab = new CANONEMAB();

                var mab = db.MAB.Find(idMab);

                var lcmab =
                        mab.CANONEMAB.Where(
                            a =>
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Nullo)
                            .OrderByDescending(a => a.IDCANONE)
                            .ToList();

                if (lcmab?.Any() ?? false)
                {
                    cmab = lcmab.First();
                }
                else
                {
                    throw new Exception(string.Format("Nessun Canone MAB trovato."));
                }
                return cmab;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PAGATOCONDIVISOMAB GetUltimoPagatoCondivisoMABInserito(decimal idMab, ModelDBISE db)
        {
            try
            {
                PAGATOCONDIVISOMAB pcmab = new PAGATOCONDIVISOMAB();

                var mab = db.MAB.Find(idMab);

                var lpcmab =
                        mab.PAGATOCONDIVISOMAB.Where(
                            a =>
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Nullo)
                            .OrderByDescending(a => a.IDPAGATOCONDIVISO)
                            .ToList();

                if (lpcmab?.Any() ?? false)
                {
                    pcmab = lpcmab.First();
                }
                else
                {
                    throw new Exception(string.Format("Nessun Pagato Condiviso MAB trovato."));
                }
                return pcmab;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region get documenti
        public List<DOCUMENTI> GetDocumentiMAB_var(decimal idAttivazioneMAB, ModelDBISE db)
        {
            try
            {


                DOCUMENTI d = new DOCUMENTI();
                List<DOCUMENTI> dl = new List<DOCUMENTI>();

                var a = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                dl = a.DOCUMENTI.Where(x => x.MODIFICATO == false &&
                                        (x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Clausole_Contratto_Alloggio ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Contratto_Locazione ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione)
                                        ).ToList();
                return dl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //        public List<DOCUMENTI> GetDocumentiMABbyTipoDoc_var(decimal idAttivazioneMAB, decimal idTipoDoc)
        //        {
        //            try
        //            {
        //                using (ModelDBISE db = new ModelDBISE())
        //                {

        //                    DOCUMENTI d = new DOCUMENTI();
        //                    List<DOCUMENTI> dl = new List<DOCUMENTI>();

        //                    var a = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

        //                    dl = a.DOCUMENTI.Where(x => x.MODIFICATO == false && x.IDTIPODOCUMENTO == idTipoDoc).ToList();
        //                    return dl;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }

        #endregion

        public void UpdateStatoMAB(decimal idMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                MAB m = db.MAB.Find(idMAB);
                if (m.IDMAB > 0)
                {
                    m.IDSTATORECORD = (decimal)stato;
                    m.DATAAGGIORNAMENTO = DateTime.Now;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggiornamento dello statorecord relativo a MAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica MAB", "MAB", db,
                            m.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, m.IDMAB);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatoPeriodoMAB(decimal idPeriodoMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                PERIODOMAB pm = db.PERIODOMAB.Find(idPeriodoMAB);
                if (pm.IDPERIODOMAB > 0)
                {
                    pm.IDSTATORECORD = (decimal)stato;
                    pm.DATAAGGIORNAMENTO = DateTime.Now;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggiornamento dello statorecord relativo a PERIODOMAB.");
                    }
                    else
                    {
                        var mab = db.MAB.Find(pm.IDMAB);
                        
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica PERIODOMAB", "PERIODOMAB", db,
                            mab.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, pm.IDPERIODOMAB);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatoCanoneMAB(decimal idCanoneMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                CANONEMAB cm = db.CANONEMAB.Find(idCanoneMAB);
                if (cm.IDCANONE > 0)
                {
                    cm.IDSTATORECORD = (decimal)stato;
                    cm.DATAAGGIORNAMENTO = DateTime.Now;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggiornamento dello statorecord relativo a CanoneMAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica CanoneMAB", "VARIAZIONIMAB", db,
                            cm.ATTIVAZIONEMAB.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, cm.IDCANONE);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatoAnticipoAnnualeMAB(decimal idAnticipoAnnualeMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                ANTICIPOANNUALEMAB aa = db.ANTICIPOANNUALEMAB.Find(idAnticipoAnnualeMAB);
                if (aa.IDANTICIPOANNUALEMAB > 0)
                {
                    aa.IDSTATORECORD = (decimal)stato;
                    aa.DATAAGGIORNAMENTO = DateTime.Now;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggiornamento dello statorecord relativo a AnticipAnnulaleMAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica AnticipoAnnualeMAB", "ANTICIPOANNUALE", db,
                            aa.ATTIVAZIONEMAB.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, aa.IDANTICIPOANNUALEMAB);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatoPagatoCondivisoMAB(decimal idPagatoCondivisoMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                PAGATOCONDIVISOMAB pcm = db.PAGATOCONDIVISOMAB.Find(idPagatoCondivisoMAB);
                if (pcm.IDPAGATOCONDIVISO > 0)
                {
                    pcm.IDSTATORECORD = (decimal)stato;
                    pcm.DATAAGGIORNAMENTO = DateTime.Now;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggiornamento dello statorecord relativo a PagatoCondivisoMAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica CanoneMAB", "VARIAZIONIMAB", db,
                            pcm.ATTIVAZIONEMAB.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, pcm.IDPAGATOCONDIVISO);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatoDocumentiMAB(decimal idDocumentoMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                DOCUMENTI dm = db.DOCUMENTI.Find(idDocumentoMAB);
                if (dm.IDDOCUMENTO > 0)
                {
                    dm.IDSTATORECORD = (decimal)stato;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggiornamento dello statorecord relativo a Documenti MAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica Documenti MAB", "DOCUMENTI", db,
                            dm.ATTIVAZIONEMAB.First().MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, dm.IDDOCUMENTO);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AnnullaModificheCanoneMAB(decimal idMAB)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();
                    try
                    {
                        var mab = db.MAB.Find(idMAB);
                        var lcmab = mab.CANONEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                        foreach (var cmab in lcmab)
                        {
                            if (cmab.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
                            {
                                //cmab.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                db.CANONEMAB.Remove(cmab);

                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore in fase di ripristino degli importi canone (cancellazione record in lavorazione)");
                                }
                            }
                            if (cmab.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && cmab.NASCONDI)
                            {
                                cmab.NASCONDI = false;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore in fase di ripristino degli importi canone (ripristino record attivo)");
                                }
                            }

                        }

                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        db.Database.CurrentTransaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AnnullaModifichePagatoCondivisoMAB(decimal idMAB)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();
                    try
                    {
                        var mab = db.MAB.Find(idMAB);
                        var lpcmab = mab.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                        foreach (var pcmab in lpcmab)
                        {
                            if (pcmab.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
                            {
                                db.PAGATOCONDIVISOMAB.Remove(pcmab);

                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore in fase di ripristino delle impostazioni condivisione del canone (cancellazione record in lavorazione)");
                                }
                            }

                            if (pcmab.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && pcmab.NASCONDI)
                            {
                                pcmab.NASCONDI = false;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore in fase di ripristino delle impostazioni di condivisione del canone (ripristino record attivo)");
                                }
                            }
                        }

                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        db.Database.CurrentTransaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AnnullaModificheMAB(decimal idMAB)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var mab = db.MAB.Find(idMAB);
                        if (mab != null && mab.IDMAB > 0)
                        {
                            //mab
                            if (mab.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
                            {
                                db.MAB.Remove(mab);
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Impossibile annullare le modifiche MAB.");
                                }
                            }
                            else
                            {
                                //periodo MAB
                                var lpmab = mab.PERIODOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                                foreach (var pmab in lpmab)
                                {
                                    //riattivo eventuale record da cui deriva
                                    if (pmab.FK_IDPERIODOMAB > 0)
                                    {
                                        var pmab_ori = db.PERIODOMAB.Find(pmab.FK_IDPERIODOMAB);
                                        pmab_ori.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Impossibile annullare le modifiche al Periodo MAB.");
                                        }
                                    }
                                    db.PERIODOMAB.Remove(pmab);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Impossibile annullare le modifiche al Periodo MAB.");
                                    }
                                }



                                //elimino eventuali modifiche canone
                                var lcmab = mab.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                                foreach (var cmab in lcmab)
                                {
                                    db.CANONEMAB.Remove(cmab);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Impossibile annullare le modifiche al Canone MAB.");
                                    }

                                    //ripristino canoni nascosti attivi
                                    var lcmab_attivi = mab.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                        a.NASCONDI).ToList();
                                    foreach (var cmab_attivi in lcmab_attivi)
                                    {
                                        cmab_attivi.NASCONDI = false;
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception(string.Format("Impossibile annullare le modifiche al Canone MAB."));
                                        }
                                    }

                                }

                                //elimino eventuali modifiche pagato condiviso
                                var lpcmab = mab.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                                foreach (var pcmab in lpcmab)
                                {
                                    db.PAGATOCONDIVISOMAB.Remove(pcmab);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Impossibile annullare le modifiche a pagato condiviso MAB.");
                                    }
                                }

                                //ripristino pagatocondiviso nascosti attivi
                                var lpcmab_attivi = mab.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                    a.NASCONDI).ToList();
                                foreach (var pcmab_attivi in lpcmab_attivi)
                                {
                                    pcmab_attivi.NASCONDI = false;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception(string.Format("Impossibile annullare le modifiche al Pagato Condiviso MAB."));
                                    }
                                }

                                //elimino eventuali documenti inseriti
                                var att = GetUltimaAttivazioneMABCorrente(idMAB, db);
                                var ldoc = att.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                                foreach (var doc in ldoc)
                                {
                                    att.DOCUMENTI.Remove(doc);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception(string.Format("Impossibile annullare i formulari MAB inseriti."));
                                    }
                                }

                            }

                        }

                        db.Database.CurrentTransaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        db.Database.CurrentTransaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}