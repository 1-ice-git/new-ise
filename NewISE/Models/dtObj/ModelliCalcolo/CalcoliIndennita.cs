using NewISE.EF;
using NewISE.Models.dtObj.Interfacce;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Web;
using NewISE.Models.DBModel;

using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;

namespace NewISE.Models.dtObj.ModelliCalcolo
{
    //public class DatiCalcoloTrasportoEffetti
    //{
    //    public decimal percentualeFK { get; set; }
    //    public decimal percentualeAnticipoTE { get; set; }
    //    public decimal percentualeSaldoTE { get; set; }
    //    public decimal AnticipoTE { get; set; }
    //    public decimal SaldoTE { get; set; }


    //}


    public class PrimaSistemazione
    {

    }


    public class DatiFigli
    {
        public decimal indennitaPrimoSegretario { get; set; }
        public decimal percentualeMaggiorazioniFligli { get; set; }
    }
    public class CalcoliIndennita : Attribute, IDisposable
    {
        #region Proprietà private
        private decimal _indennitaDiBase = 0;
        private decimal _riduzioneIndennitaDiBase = 0;
        private decimal _coefficienteDiSede = 0;
        private decimal _percentualeDisagio = 0;
        private decimal _indennitaDiServizio = 0;
        private decimal _percentualeMaggiorazioneConiuge = 0;
        private decimal _maggiorazioneConiuge = 0;
        private decimal _maggiorazioneConiugeRichiamo = 0;
        private decimal _pensioneConiuge = 0;
        private decimal _maggiorazioneConiugeMenoPensione = 0;
        private decimal _percentualeMaggiorazioniFigli = 0;
        private decimal _indennitaPrimoSegretario = 0;
        private decimal _indennitaServizioPrimoSegretario = 0;
        private decimal _maggiorazioneFigli = 0;
        private decimal _maggiorazioneFigliRichiamo = 0;
        private decimal _maggiorazioniFimailiri = 0;
        private decimal _indennitaPersonale = 0;
        private decimal _indennitaPersonaleInValuta = 0;
        private decimal _coefficienteIndennitaSistemazione = 0;
        private decimal _indennitaSistemazione = 0;
        private decimal _percentualeRiduzionePrimaSistemazione = 0;
        private decimal _indennitaSistemazioneAnticipabile = 0;
        private decimal _percentualeFKMPartenza = 0;
        private decimal _anticipoContributoOmnicomprensivoPartenza = 0;
        private decimal _percentualeAnticipoTEPartenza = 0;
        private decimal _percentualeSaldoTEPartenza = 0;
        private decimal _saldoContributoOmnicomprensivoPartenza = 0;

        private decimal _percentualeFKMRientro = 0;
        private decimal _anticipoContributoOmnicomprensivoRientro = 0;
        private decimal _percentualeAnticipoTERientro = 0;
        private decimal _percentualeSaldoTERientro = 0;
        private decimal _saldoContributoOmnicomprensivoRientro = 0;
        private decimal _totaleContributoOmnicomprensivoPartenza = 0;
        private decimal _totaleContributoOmnicomprensivoRientro = 0;

        private decimal _coefficenteIndennitaRichiamo = 0;
        private decimal _coefficenteMaggiorazioneRichiamo = 0;
        private decimal _indennitaRichiamoLordo = 0;
        private decimal _indennitaRichiamoNetto = 0;
        //private int _giorniSospensione = 0;
        //private decimal _importoAbbattimentoSospensione = 0;

        private decimal _percentualeMAB = 0;
        private decimal _canoneMab = 0;
        private decimal _tassoCambio = 0;
        private VALUTE _valutaMab = new VALUTE();
        private bool _anticipoAnnualeMAB = false;
        private bool _condivisioneMAB = false;
        private bool _pagatoMAB = false;
        private decimal _percentualeCondivisione = 0;
        private decimal _canoneInEuro = 0;
        private decimal _importoMABMaxMensile = 0;
        private decimal _importoMABMensile = 0;

        private decimal _percentualeRiduzioneRichiamo = 0;



        private DateTime _dtDatiParametri;
        private TRASFERIMENTO _trasferimento = new TRASFERIMENTO();
        private INDENNITA _indennita = new INDENNITA();

        private RUOLODIPENDENTE _ruoloDipendente = new RUOLODIPENDENTE();
        private RUOLOUFFICIO _ruoloUfficio = new RUOLOUFFICIO();

        private LIVELLI _livello = new LIVELLI();

        private List<DatiFigli> _lDatiFigli = new List<DatiFigli>();

        private FASCIA_KM _fasciaKMPartenza = new FASCIA_KM();
        private FASCIA_KM _fasciaKMRientro = new FASCIA_KM();

        #endregion


        #region Proprietà pubbliche
        [ReadOnly(true)]
        public FASCIA_KM FasciaKM_P => _fasciaKMPartenza;
        [ReadOnly(true)]
        public FASCIA_KM FasciaKM_R => _fasciaKMRientro;
        [ReadOnly(true)]
        public RUOLODIPENDENTE RuoloDipendente => _ruoloDipendente;
        [ReadOnly(true)]
        public RUOLOUFFICIO RuoloUfficio => _ruoloUfficio;

        [ReadOnly(true)]
        public decimal IndennitaDiBase => _indennitaDiBase;
        [ReadOnly(true)]
        public decimal CoefficienteDiSede => _coefficienteDiSede;
        [ReadOnly(true)]
        public decimal PercentualeDisagio => _percentualeDisagio;
        [ReadOnly(true)]
        public decimal IndennitaDiServizio => _indennitaDiServizio;
        [ReadOnly(true)]
        public decimal PercentualeMaggiorazioneConiuge => _percentualeMaggiorazioneConiuge;
        [ReadOnly(true)]
        public decimal PercentualeMaggiorazioneFigli => _percentualeMaggiorazioniFigli;
        [ReadOnly(true)]
        public decimal MaggiorazioneConiuge => _maggiorazioneConiuge;
        [ReadOnly(true)]
        public decimal MaggiorazioneConiugeRichiamo => _maggiorazioneConiugeRichiamo;
        [ReadOnly(true)]
        public decimal PensioneConiuge => _pensioneConiuge;
        [ReadOnly(true)]
        public decimal MaggiorazioneConiugeMenoPensione => _maggiorazioneConiugeMenoPensione;
        //[ReadOnly(true)]
        //public decimal PercentualeMaggiorazioneFigli => _percentualeMaggiorazioniFigli;
        [ReadOnly(true)]
        public decimal IndennitaPrimoSegretario => _indennitaPrimoSegretario;
        [ReadOnly(true)]
        public decimal IndennitaServizioPrimoSegretario => _indennitaServizioPrimoSegretario;
        [ReadOnly(true)]
        public decimal MaggiorazioneFigli => _maggiorazioneFigli;
        [ReadOnly(true)]
        public decimal MaggiorazioneFigliRichiamo => _maggiorazioneFigliRichiamo;
        [ReadOnly(true)]
        public decimal MaggiorazioniFamiliari => _maggiorazioniFimailiri;
        [ReadOnly(true)]
        public decimal IndennitaPersonale => _indennitaPersonale;
        [ReadOnly(true)]
        public decimal IndennitaPersonaleInValuta => _indennitaPersonaleInValuta;
        [ReadOnly(true)]
        public decimal CoefficienteIndennitaSistemazione => _coefficienteIndennitaSistemazione;
        [ReadOnly(true)]
        public decimal IndennitaSistemazioneLorda => _indennitaSistemazione;
        [ReadOnly(true)]
        public decimal PercentualeRiduzionePrimaSistemazione => _percentualeRiduzionePrimaSistemazione;
        [ReadOnly(true)]
        public decimal IndennitaSistemazioneAnticipabileLorda => _indennitaSistemazioneAnticipabile;
        [ReadOnly(true)]
        public decimal PercentualeFKMPartenza => _percentualeFKMPartenza;
        [ReadOnly(true)]
        public decimal AnticipoContributoOmnicomprensivoPartenza => _anticipoContributoOmnicomprensivoPartenza;
        [ReadOnly(true)]
        public decimal PercentualeAnticipoTEPartenza => _percentualeAnticipoTEPartenza;
        [ReadOnly(true)]
        public decimal PercentualeSaldoTEPartenza => _percentualeSaldoTEPartenza;
        [ReadOnly(true)]
        public decimal SaldoContributoOmnicomprensivoPartenza => _saldoContributoOmnicomprensivoPartenza;
        [ReadOnly(true)]
        public decimal TotaleContributoOmnicomprensivoPartenza => _totaleContributoOmnicomprensivoPartenza;
        [ReadOnly(true)]
        public decimal TotaleContributoOmnicomprensivoRientro => _totaleContributoOmnicomprensivoRientro;


        [ReadOnly(true)]
        public decimal PercentualeFKMRientro => _percentualeFKMRientro;
        [ReadOnly(true)]
        public decimal AnticipoContributoOmnicomprensivoRientro => _anticipoContributoOmnicomprensivoRientro;
        [ReadOnly(true)]
        public decimal PercentualeAnticipoTERientro => _percentualeAnticipoTERientro;
        [ReadOnly(true)]
        public decimal PercentualeSaldoTERientro => _percentualeSaldoTERientro;
        [ReadOnly(true)]
        public decimal SaldoContributoOmnicomprensivoRientro => _saldoContributoOmnicomprensivoRientro;

        [ReadOnly(true)]
        public decimal CoefficenteIndennitaRichiamo => _coefficenteIndennitaRichiamo;
        [ReadOnly(true)]
        public decimal CoefficenteMaggiorazioneRichiamo => _coefficenteMaggiorazioneRichiamo;
        [ReadOnly(true)]
        public decimal IndennitaRichiamoLordo => _indennitaRichiamoLordo;
        [ReadOnly(true)]
        public decimal IndennitaRichiamoNetto => _indennitaRichiamoNetto;
        [ReadOnly(true)]
        public decimal PercentualeMAB => _percentualeMAB;
        [ReadOnly(true)]
        public decimal CanoneMAB => _canoneMab;
        [ReadOnly(true)]
        public decimal TassoCambio => _tassoCambio;
        [ReadOnly(true)]
        public VALUTE ValutaMAB => _valutaMab;
        [ReadOnly(true)]
        public bool AnticipoAnnualeMAB => _anticipoAnnualeMAB;
        [ReadOnly(true)]
        public bool CondivisioneMAB => _condivisioneMAB;
        [ReadOnly(true)]
        public bool PagatoMab => _pagatoMAB;
        [ReadOnly(true)]
        public decimal PercentualeCondivisione => _percentualeCondivisione;
        [ReadOnly(true)]
        public decimal CanoneMABEuro => _canoneInEuro;
        [ReadOnly(true)]
        public decimal ImportoMABMaxMensile => _importoMABMaxMensile;
        [ReadOnly(true)]
        public decimal ImportoMABMensile => _importoMABMensile;
        [ReadOnly(true)]
        public decimal PercentualeRiduzioneRichiamo => _percentualeRiduzioneRichiamo;
        //[ReadOnly(true)]
        //public int GiorniSospensione => _giorniSospensione;

        //[ReadOnly(true)]
        //public decimal ImportoAbbattimentoSospensione => _importoAbbattimentoSospensione;


        [ReadOnly(true)]
        public LIVELLI Livello => _livello;
        [ReadOnly(true)]
        public IList<DatiFigli> lDatiFigli => _lDatiFigli;
        //[ReadOnly(true)]
        //public DatiCalcoloTrasportoEffetti DatiCalcoloTrasportoEffetti => _datiCalcoloTrasportoEffetti;
        #endregion


        public CalcoliIndennita(decimal idTrasferimento, DateTime? dataCalcoloIndennita, ModelDBISE db)
        {

            try
            {
                this.Elaborazioni(idTrasferimento, dataCalcoloIndennita, db);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public CalcoliIndennita(decimal idTrasferimento, DateTime? dataCalcoloIndennita = null)
        {
            //DateTime dt;


            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    this.Elaborazioni(idTrasferimento, dataCalcoloIndennita, db);

                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }




        #region Metodi per il calcolo degli importi
        /// <summary>
        /// Istanzia l'indennità
        /// </summary>
        /// <param name="trasf"></param>
        public void Indennita()
        {
            _indennita = _trasferimento.INDENNITA;

        }

        private void Elaborazioni(decimal idTrasferimento, DateTime? dataCalcoloIndennita, ModelDBISE db)
        {
            DateTime dt;

            try
            {
                dt = dataCalcoloIndennita ?? DateTime.Now;

                _trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);

                if (_trasferimento.DATARIENTRO != Utility.DataFineStop())
                {
                    if (_trasferimento.DATARIENTRO < dt)
                    {
                        _dtDatiParametri = _trasferimento.DATARIENTRO;
                    }
                    else
                    {
                        if (_trasferimento.DATAPARTENZA > dt)
                        {
                            _dtDatiParametri = _trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            _dtDatiParametri = dt;
                        }
                    }
                }
                else
                {
                    if (_trasferimento.DATAPARTENZA > dt)
                    {
                        _dtDatiParametri = _trasferimento.DATAPARTENZA;
                    }
                    else
                    {
                        _dtDatiParametri = dt;
                    }
                }

                this.Indennita();
                this.RuoloDipendente_Ufficio();
                this.PrelevaIndennitaDiBase();
                this.PrelevaCoefficenteDiSede();
                this.PrelevaPercentualeDisagio();
                this.CalcolaIndennitaDiServizio();
                this.CalcolaMaggiorazioneFamiliare();
                this.CalcolaIndennitaPersonale();
                this.CalcolaIndennitaPersonaleInValuta();
                this.CalcolaPrimaSistemazione();
                this.CalcolaContributoOmniComprensivoPartenza();
                this.CalcolaRichiamo();
                this.CalcolaContributoOmniComprensivoRientro();
                this.CalcolaMab();


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void PrelevaPagatoCondiviso(MAB mab)
        {
            var lpcMab =
                mab.PAGATOCONDIVISOMAB.Where(
                    a =>
                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                        _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                        _dtDatiParametri <= a.DATAFINEVALIDITA)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                    .ToList();
            if (lpcMab?.Any() ?? false)
            {
                var pcMab = lpcMab.First();
                _condivisioneMAB = pcMab.CONDIVISO;
                _pagatoMAB = pcMab.PAGATO;
            }
        }

        private void PrelevaAnticipoAnnualeMab(MAB mab)
        {
            var lAntAnnMab =
                mab.ANTICIPOANNUALEMAB.Where(
                    a =>
                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                    .OrderByDescending(a => a.IDANTICIPOANNUALEMAB)
                    .ToList();
            if (lAntAnnMab?.Any() ?? false)
            {
                var aamab = lAntAnnMab.First();

                _anticipoAnnualeMAB = aamab.ANTICIPOANNUALE;

            }
        }

        private void PrelevaTFRMAB(CANONEMAB cmab)
        {
            var ltfr =
                cmab.TFR.Where(
                    a =>
                        a.ANNULLATO == false && a.IDVALUTA == cmab.IDVALUTA && _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                        _dtDatiParametri <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
            if (ltfr?.Any() ?? false)
            {
                var tfr = ltfr.First();

                _tassoCambio = tfr.TASSOCAMBIO;

            }
        }

        private void PrelevaCanoneMab(MAB mab)
        {
            var lcmab =
                mab.CANONEMAB.Where(
                    a =>
                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                        _dtDatiParametri >= a.DATAINIZIOVALIDITA && _dtDatiParametri <= a.DATAFINEVALIDITA)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                    .ToList();
            if (lcmab?.Any() ?? false)
            {
                var cmab = lcmab.First();

                _canoneMab = cmab.IMPORTOCANONE;
                _valutaMab = cmab.VALUTE;

                this.PrelevaTFRMAB(cmab);

            }
        }

        private void PrelevaDatiMab()
        {

            //var magAbitazione = _indennita.MAGGIORAZIONEABITAZIONE;

            //var lmab = magAbitazione.MAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();

            //var lmabp =
            //    lmab.Where(
            //        a =>
            //            a.PERIODOMAB.Any(
            //                b =>
            //                    b.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
            //                    _dtDatiParametri >= b.DATAINIZIOMAB && _dtDatiParametri <= b.DATAFINEMAB)).ToList();




            var lmab =
                _indennita.MAB.Where(
                    a =>
                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                        a.RINUNCIAMAB == false &&
                        a.PERIODOMAB.Any(
                            b =>
                                b.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                _dtDatiParametri >= b.DATAINIZIOMAB &&
                                _dtDatiParametri <= b.DATAFINEMAB))
                    .ToList();

            //var lmab = _indennita.MAGGIORAZIONEABITAZIONE.MAB.Where(a=>a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && a.ATTIVAZIONEMAB.ANNULLATO == false && a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true && a.ATTIVAZIONEMAB.ATTIVAZIONE == true).order

            if (lmab?.Any() ?? false)
            {
                #region Percentuale MAB
                var mab = lmab.First();
                var perMab =
                    mab.PERIODOMAB.Where(
                        a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                        .OrderByDescending(a => a.IDPERIODOMAB)
                        .First();

                var lpmab =
                    perMab.PERCENTUALEMAB.Where(
                        a =>
                            a.ANNULLATO == false && a.IDUFFICIO == _trasferimento.IDUFFICIO &&
                            a.IDLIVELLO == _livello.IDLIVELLO &&
                            _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                            _dtDatiParametri <= a.DATAFINEVALIDITA)
                        .ToList();

                if (lpmab?.Any() ?? false)
                {
                    var pmab = lpmab.First();

                    if (_ruoloUfficio.IDRUOLO == (decimal)EnumRuoloUfficio.Dirigente ||
                        _ruoloUfficio.IDRUOLO == (decimal)EnumRuoloUfficio.Responsabile)
                    {
                        _percentualeMAB = pmab.PERCENTUALERESPONSABILE;
                    }
                    else
                    {
                        _percentualeMAB = pmab.PERCENTUALE;
                    }



                }
                #endregion

                this.PrelevaCanoneMab(mab);
                this.PrelevaAnticipoAnnualeMab(mab);
                this.PrelevaPagatoCondiviso(mab);
            }

        }





        private void CalcolaMab()
        {
            this.PrelevaDatiMab();

            _importoMABMaxMensile = (_percentualeMAB / 100) * _indennitaPersonale;

            if (_canoneMab > 0)
            {

                if (_tassoCambio > 0)
                {
                    _canoneInEuro = _canoneMab / _tassoCambio;
                }
                else
                {
                    throw new Exception("Impossibile effettuare il calcolo con tasso valuta a zero.");
                }

                if (_importoMABMaxMensile > _canoneInEuro)
                {
                    _importoMABMensile = _canoneInEuro;
                }
                else
                {
                    _importoMABMensile = _importoMABMaxMensile;
                }
            }
            else
            {
                _importoMABMensile = _importoMABMaxMensile;
            }

            if (_condivisioneMAB)
            {
                if (PagatoMab)
                {
                    _importoMABMaxMensile = (_importoMABMaxMensile * (_percentualeCondivisione / 100)) + _importoMABMaxMensile;
                    _importoMABMensile = (_importoMABMensile * (_percentualeCondivisione / 100)) + _importoMABMensile;
                }
                else
                {
                    _importoMABMensile = 0;
                }
            }



        }

        private void RuoloDipendente_Ufficio()
        {
            var lrd =
                _trasferimento.RUOLODIPENDENTE.Where(
                    a =>
                        a.ANNULLATO == false && _dtDatiParametri >= a.DATAINZIOVALIDITA &&
                        _dtDatiParametri <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINZIOVALIDITA).ToList();

            if (lrd?.Any() ?? false)
            {
                _ruoloDipendente = lrd.First();
                _ruoloUfficio = _ruoloDipendente.RUOLOUFFICIO;

            }
        }

        private void PrelevaIndennitaDiBase()
        {
            RIDUZIONI riduzioniIB = new RIDUZIONI();
            LIVELLIDIPENDENTI livDip = new LIVELLIDIPENDENTI();

            var lLivDip =
                _indennita.LIVELLIDIPENDENTI.Where(
                    a =>
                        a.ANNULLATO == false && _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                        _dtDatiParametri <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
            if (lLivDip?.Any() ?? false)
            {
                livDip = lLivDip.First();
                _livello = livDip.LIVELLI;

                var lib =
                _indennita.INDENNITABASE.Where(
                    a =>
                        a.ANNULLATO == false &&
                        a.IDLIVELLO == livDip.IDLIVELLO &&
                        _dtDatiParametri >= a.DATAINIZIOVALIDITA && _dtDatiParametri <= a.DATAFINEVALIDITA)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA);

                if (lib?.Any() ?? false)
                {
                    var indennitaBase = lib.First();

                    var lr =
                        indennitaBase.RIDUZIONI.Where(
                            a =>
                                a.ANNULLATO == false && _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                _dtDatiParametri <= a.DATAFINEVALIDITA)
                            .OrderByDescending(a => a.DATAINIZIOVALIDITA);

                    if (lr?.Any() ?? false)
                    {
                        riduzioniIB = lr.First();
                    }

                    if (_ruoloUfficio.IDRUOLO == (decimal)EnumRuoloUfficio.Dirigente || _ruoloUfficio.IDRUOLO == (decimal)EnumRuoloUfficio.Responsabile)
                    {
                        decimal valRespIB = indennitaBase.VALORERESP;
                        //decimal valRidIB = 0;

                        if (riduzioniIB?.IDRIDUZIONI > 0)
                        {
                            _riduzioneIndennitaDiBase = riduzioniIB.PERCENTUALE;
                        }
                        if (_riduzioneIndennitaDiBase > 0)
                        {
                            _indennitaDiBase = valRespIB - (valRespIB * (_riduzioneIndennitaDiBase / 100));
                        }
                        else
                        {
                            _indennitaDiBase = valRespIB;
                        }
                    }
                    else
                    {
                        decimal valIB = indennitaBase.VALORE;
                        //decimal valRidIB = 0;

                        if (riduzioniIB?.IDRIDUZIONI > 0)
                        {
                            _riduzioneIndennitaDiBase = riduzioniIB.PERCENTUALE;
                        }
                        if (_riduzioneIndennitaDiBase > 0)
                        {
                            _indennitaDiBase = valIB - (valIB * (_riduzioneIndennitaDiBase / 100));
                        }
                        else
                        {
                            _indennitaDiBase = valIB;
                        }

                    }
                }
            }



        }

        private void PrelevaCoefficenteDiSede()
        {
            var lcs =
                _indennita.COEFFICIENTESEDE.Where(
                    a =>
                        a.ANNULLATO == false && _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                        _dtDatiParametri <= a.DATAFINEVALIDITA)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

            if (lcs?.Any() ?? false)
            {
                var coefficenteSede = lcs.First();
                _coefficienteDiSede = coefficenteSede.VALORECOEFFICIENTE;
            }
        }

        private void PrelevaPercentualeDisagio()
        {
            var lpd =
                _indennita.PERCENTUALEDISAGIO.Where(
                    a =>
                        a.ANNULLATO == false && _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                        _dtDatiParametri <= a.DATAFINEVALIDITA)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

            if (lpd?.Any() ?? false)
            {
                var percentualeDisagio = lpd.First();
                _percentualeDisagio = percentualeDisagio.PERCENTUALE;


            }
        }

        private void CalcolaIndennitaDiServizio()
        {

            if (_indennitaDiBase > 0)
            {
                var indServ = (((_indennitaDiBase * _coefficienteDiSede) +
                                _indennitaDiBase) +
                               (((_indennitaDiBase * _coefficienteDiSede) +
                                 _indennitaDiBase) * (_percentualeDisagio / 100)));

                _indennitaDiServizio = Math.Round(indServ, 8);
            }
        }

        private void CalcolaMaggiorazioneFamiliare()
        {
            var mf = _trasferimento.MAGGIORAZIONIFAMILIARI;

            var lattivazioneMF =
                mf.ATTIVAZIONIMAGFAM.Where(
                    a =>
                        a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                        a.ATTIVAZIONEMAGFAM == true)
                    .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

            if (lattivazioneMF?.Any() ?? false)
            {
                #region Maggiorazione coniuge

                var lc =
                    mf.CONIUGE.Where(
                        a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                            _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                            _dtDatiParametri <= a.DATAFINEVALIDITA)
                        .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                if (lc?.Any() ?? false)
                {
                    var coniuge = lc.First();

                    var lpmc =
                        coniuge.PERCENTUALEMAGCONIUGE.Where(
                            a =>
                                a.ANNULLATO == false &&
                                a.IDTIPOLOGIACONIUGE == coniuge.IDTIPOLOGIACONIUGE &&
                                _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                _dtDatiParametri <= a.DATAFINEVALIDITA)
                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                    if (lpmc?.Any() ?? false)
                    {
                        var percentualeMaggiorazioneConiuge = lpmc.First();

                        _percentualeMaggiorazioneConiuge = percentualeMaggiorazioneConiuge.PERCENTUALECONIUGE;

                        _maggiorazioneConiuge = _indennitaDiServizio *
                                                _percentualeMaggiorazioneConiuge /
                                                100;
                    }

                    var lpensioni =
                        coniuge.PENSIONE.Where(
                            a =>
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                _dtDatiParametri >= a.DATAINIZIO &&
                                _dtDatiParametri <= a.DATAFINE)
                            .OrderByDescending(a => a.DATAINIZIO)
                            .ToList();


                    if (lpensioni?.Any() ?? false)
                    {
                        var pens = lpensioni.First();
                        _pensioneConiuge = pens.IMPORTOPENSIONE;

                        if (_pensioneConiuge >= _maggiorazioneConiuge)
                        {
                            _maggiorazioneConiugeMenoPensione = 0;
                        }
                        else
                        {
                            _maggiorazioneConiugeMenoPensione = MaggiorazioneConiuge - _pensioneConiuge;
                        }
                    }
                    else
                    {
                        _maggiorazioneConiugeMenoPensione = MaggiorazioneConiuge;
                    }
                }
                #endregion

                #region Maggiorazioni figli

                var lf =
                    mf.FIGLI.Where(
                        a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                            _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                            _dtDatiParametri <= a.DATAFINEVALIDITA)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                if (lf?.Any() ?? false)
                {
                    foreach (var f in lf)
                    {
                        DatiFigli datiFigli = new DatiFigli();

                        var lpmf =
                            f.PERCENTUALEMAGFIGLI.Where(
                                a =>
                                    a.ANNULLATO == false && _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                    _dtDatiParametri <= a.DATAFINEVALIDITA && a.IDTIPOLOGIAFIGLIO == f.IDTIPOLOGIAFIGLIO)
                                .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lpmf?.Any() ?? false)
                        {
                            var pmf = lpmf.First();
                            _percentualeMaggiorazioniFigli = pmf.PERCENTUALEFIGLI;

                            var lips =
                                f.INDENNITAPRIMOSEGRETARIO.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                        _dtDatiParametri <= a.DATAFINEVALIDITA)
                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                            if (lips?.Any() ?? false)
                            {
                                var ips = lips.First();
                                _indennitaPrimoSegretario = ips.INDENNITA;

                                _indennitaServizioPrimoSegretario = (((_indennitaPrimoSegretario *
                                                                       _coefficienteDiSede) +
                                                                      _indennitaPrimoSegretario) +
                                                                     (((_indennitaPrimoSegretario *
                                                                        _coefficienteDiSede) +
                                                                       _indennitaPrimoSegretario)
                                                                      * (_percentualeDisagio / 100)));

                                _maggiorazioneFigli += _indennitaServizioPrimoSegretario *
                                                       _percentualeMaggiorazioniFigli / 100;


                                datiFigli.indennitaPrimoSegretario = _indennitaPrimoSegretario;
                                datiFigli.percentualeMaggiorazioniFligli = _percentualeMaggiorazioniFigli;

                                _lDatiFigli.Add(datiFigli);

                            }



                        }


                    }


                }
                #endregion

                _maggiorazioniFimailiri = Math.Round(_maggiorazioneConiugeMenoPensione + _maggiorazioneFigli, 8);
            }

        }


        private Decimal CalcolaMaggiorazioneFamiliareRichiamo(decimal indennitaServizioRichiamo)
        {
            //decimal maggiorazioneConiuge = 0;
            decimal maggiorazioneConiugeMenoPensione = 0;
            decimal ImppercentualeMaggiorazioneConiuge = 0;
            decimal pensioneConiuge = 0;
            decimal percentualeMaggiorazioniFigli = 0;
            decimal indennitaPrimoSegretario = 0;
            decimal indennitaServizioPrimoSegretario = 0;
            //decimal maggiorazioneFigli = 0;
            List<DatiFigli> lDatiFigli = new List<DatiFigli>();
            decimal maggiorazioniFimailiri = 0;

            var mf = _trasferimento.MAGGIORAZIONIFAMILIARI;

            var lattivazioneMF =
                mf.ATTIVAZIONIMAGFAM.Where(
                    a =>
                        a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                        a.ATTIVAZIONEMAGFAM == true)
                    .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

            if (lattivazioneMF?.Any() ?? false)
            {
                #region Maggiorazione coniuge

                var lc =
                    mf.CONIUGE.Where(
                        a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                            _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                            _dtDatiParametri <= a.DATAFINEVALIDITA)
                        .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                if (lc?.Any() ?? false)
                {
                    var coniuge = lc.First();

                    var lpmc =
                        coniuge.PERCENTUALEMAGCONIUGE.Where(
                            a =>
                                a.ANNULLATO == false &&
                                a.IDTIPOLOGIACONIUGE == coniuge.IDTIPOLOGIACONIUGE &&
                                _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                _dtDatiParametri <= a.DATAFINEVALIDITA)
                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                    if (lpmc?.Any() ?? false)
                    {
                        var percentualeMaggiorazioneConiuge = lpmc.First();

                        ImppercentualeMaggiorazioneConiuge = percentualeMaggiorazioneConiuge.PERCENTUALECONIUGE;

                        _maggiorazioneConiugeRichiamo = indennitaServizioRichiamo *
                                                        ImppercentualeMaggiorazioneConiuge /
                                                        100;
                    }

                    var lpensioni =
                        coniuge.PENSIONE.Where(
                            a =>
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                _dtDatiParametri >= a.DATAINIZIO &&
                                _dtDatiParametri <= a.DATAFINE)
                            .OrderByDescending(a => a.DATAINIZIO)
                            .ToList();


                    if (lpensioni?.Any() ?? false)
                    {
                        var pens = lpensioni.First();
                        pensioneConiuge = pens.IMPORTOPENSIONE;

                        if (pensioneConiuge >= _maggiorazioneConiugeRichiamo)
                        {
                            maggiorazioneConiugeMenoPensione = 0;
                        }
                        else
                        {
                            maggiorazioneConiugeMenoPensione = _maggiorazioneConiugeRichiamo - pensioneConiuge;
                        }

                    }
                    else
                    {
                        maggiorazioneConiugeMenoPensione = _maggiorazioneConiugeRichiamo;
                    }
                }
                #endregion

                #region Maggiorazioni figli

                var lf =
                    mf.FIGLI.Where(
                        a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                            _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                            _dtDatiParametri <= a.DATAFINEVALIDITA)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                if (lf?.Any() ?? false)
                {
                    foreach (var f in lf)
                    {
                        DatiFigli datiFigli = new DatiFigli();

                        var lpmf =
                            f.PERCENTUALEMAGFIGLI.Where(
                                a =>
                                    a.ANNULLATO == false && _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                    _dtDatiParametri <= a.DATAFINEVALIDITA && a.IDTIPOLOGIAFIGLIO == f.IDTIPOLOGIAFIGLIO)
                                .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lpmf?.Any() ?? false)
                        {
                            var pmf = lpmf.First();
                            percentualeMaggiorazioniFigli = pmf.PERCENTUALEFIGLI;

                            var lips =
                                f.INDENNITAPRIMOSEGRETARIO.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                        _dtDatiParametri <= a.DATAFINEVALIDITA)
                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                            if (lips?.Any() ?? false)
                            {
                                var ips = lips.First();
                                indennitaPrimoSegretario = ips.INDENNITA;

                                #region Modifica della formula per il calcolo delle maggiorazioni figli
                                ///Modificato il calcolo delle maggiorazioni figli dato da un richiamo.
                                /// La formula sottostante ci è stata data dalla Velardi.
                                ///indennità base lorda 770,55 * 6,562(coefficiente di maggiorazione richiamo) = 5056.35
                                ///770.55 + 5056,35 = 5.826,90
                                ///5826,90 * 12,5 % = 728,36(maggiorazione di famiglia per un figlio corretta)


                                //indennitaServizioPrimoSegretario = (((indennitaPrimoSegretario *
                                //                                      _coefficenteMaggiorazioneRichiamo) +
                                //                                      indennitaPrimoSegretario) +
                                //                                     (((_indennitaPrimoSegretario *
                                //                                        _coefficenteMaggiorazioneRichiamo) +
                                //                                       indennitaPrimoSegretario)
                                //                                      * (_percentualeDisagio / 100))); 
                                indennitaServizioPrimoSegretario = (indennitaPrimoSegretario * _coefficenteMaggiorazioneRichiamo) + indennitaPrimoSegretario;
                                #endregion


                                _maggiorazioneFigliRichiamo += indennitaServizioPrimoSegretario *
                                                               percentualeMaggiorazioniFigli / 100;


                                datiFigli.indennitaPrimoSegretario = indennitaPrimoSegretario;
                                datiFigli.percentualeMaggiorazioniFligli = percentualeMaggiorazioniFigli;

                                lDatiFigli.Add(datiFigli);

                            }



                        }


                    }


                }
                #endregion

                maggiorazioniFimailiri = Math.Round(maggiorazioneConiugeMenoPensione + _maggiorazioneFigliRichiamo, 8);


            }

            return maggiorazioniFimailiri;

        }


        private void CalcolaIndennitaPersonale()
        {
            _indennitaPersonale = Math.Round(_indennitaDiServizio + _maggiorazioniFimailiri, 8);

        }


        private void CalcolaIndennitaPersonaleInValuta()
        {

            var ltfr =
                _indennita.TFR.Where(
                    a =>
                        a.ANNULLATO == false && _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                        _dtDatiParametri <= a.DATAFINEVALIDITA).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

            if (ltfr?.Any() ?? false)
            {
                var tfr = ltfr.Last();

                decimal tassoCambio = tfr.TASSOCAMBIO;

                _indennitaPersonale = Math.Round(_indennitaDiServizio + _maggiorazioniFimailiri, 8);

                if (tassoCambio > 0)
                {
                    _indennitaPersonaleInValuta = _indennitaPersonale / tassoCambio;
                }
                else
                {
                    _indennitaPersonaleInValuta = 0;
                }


            }




        }


        private void CalcolaRichiamo()
        {

            decimal maggiorazioniFamiliariRichiamo = 0;

            var lRichiamo =
                _trasferimento.RICHIAMO.Where(
                    a => a.ANNULLATO == false && a.DATARICHIAMO < Convert.ToDateTime("31/12/9999"))
                    .OrderByDescending(a => a.IDRICHIAMO)
                    .ToList();

            if (lRichiamo?.Any() ?? false)
            {
                if (_trasferimento.DATARIENTRO >= _dtDatiParametri)
                {
                    var richiamo = lRichiamo.First();
                    DateTime dataRientro = _trasferimento.DATARIENTRO;

                    var lcmr =
                        richiamo.COEFFICIENTEINDRICHIAMO.Where(
                                a =>
                                    a.ANNULLATO == false && dataRientro >= a.DATAINIZIOVALIDITA &&
                                    dataRientro <= a.DATAFINEVALIDITA && a.IDTIPOCOEFFICIENTERICHIAMO ==
                                    (decimal)EnumTipoCoefficienteRichiamo.CoefficienteMaggiorazione)
                            .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                    if (lcmr?.Any() ?? false)
                    {
                        RIDUZIONI riduzione = new RIDUZIONI();

                        var cmr = lcmr.First();

                        var lrid =
                            cmr.RIDUZIONI.Where(
                                a =>
                                    a.ANNULLATO == false && dataRientro >= a.DATAINIZIOVALIDITA &&
                                    dataRientro <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lrid?.Any() ?? false)
                        {
                            riduzione = lrid.First();
                            _percentualeRiduzioneRichiamo = riduzione.PERCENTUALE;
                        }

                        _coefficenteMaggiorazioneRichiamo = cmr.COEFFICIENTERICHIAMO;

                        var lcr =
                            richiamo.COEFFICIENTEINDRICHIAMO.Where(
                                    a =>
                                        a.ANNULLATO == false && dataRientro >= a.DATAINIZIOVALIDITA &&
                                        dataRientro <= a.DATAFINEVALIDITA && a.IDTIPOCOEFFICIENTERICHIAMO ==
                                        (decimal)EnumTipoCoefficienteRichiamo.CoefficienteRichiamo)
                                .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                .ToList();

                        if (lcr?.Any() ?? false)
                        {
                            var cr = lcr.First();

                            _coefficenteIndennitaRichiamo = cr.COEFFICIENTERICHIAMO;


                            var maggiorazione = _indennitaDiBase * _coefficenteMaggiorazioneRichiamo;

                            var comodo1 = maggiorazione + _indennitaDiBase;

                            maggiorazioniFamiliariRichiamo = this.CalcolaMaggiorazioneFamiliareRichiamo(comodo1);

                            var comodo2 = (comodo1 * _coefficenteIndennitaRichiamo) + maggiorazioniFamiliariRichiamo;



                            if (_percentualeRiduzioneRichiamo > 0)
                            {
                                _indennitaRichiamoLordo = Math.Round((comodo2 * (_percentualeRiduzioneRichiamo / 100)), 8);
                            }
                            else
                            {
                                _indennitaRichiamoLordo = Math.Round(comodo2, 8);
                            }

                        }
                        else
                        {
                            throw new Exception("Il coefficiente di richiamo non è presente.");
                        }

                    }
                    else
                    {
                        throw new Exception("Il coefficiente di maggiorazione per il richiamo non è presente.");
                    }
                }
            }
        }


        private void CalcolaPrimaSistemazione()
        {
            RIDUZIONI riduzioniPS = new RIDUZIONI();

            var primaSistemazione = _trasferimento.PRIMASITEMAZIONE;

            var lis =
                primaSistemazione.INDENNITASISTEMAZIONE.Where(
                    a =>
                        a.ANNULLATO == false && a.IDTIPOTRASFERIMENTO == _trasferimento.IDTIPOTRASFERIMENTO &&
                        _dtDatiParametri >= a.DATAINIZIOVALIDITA && _dtDatiParametri <= a.DATAFINEVALIDITA)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                    .ToList();

            if (lis?.Any() ?? false)
            {
                var indSist = lis.First();

                _coefficienteIndennitaSistemazione = indSist.COEFFICIENTE;

                var lr =
                    indSist.RIDUZIONI.Where(
                        a =>
                            a.ANNULLATO == false && _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                            _dtDatiParametri <= a.DATAFINEVALIDITA)
                        .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                if (lr?.Any() ?? false)
                {
                    riduzioniPS = lr.First();
                    _percentualeRiduzionePrimaSistemazione = riduzioniPS.PERCENTUALE;
                }

                if (_percentualeRiduzionePrimaSistemazione > 0)
                {
                    _indennitaSistemazione = Math.Round((_coefficienteIndennitaSistemazione * (_percentualeRiduzionePrimaSistemazione / 100)) * _indennitaPersonale, 8);
                    _indennitaSistemazioneAnticipabile = Math.Round((_coefficienteIndennitaSistemazione * (_percentualeRiduzionePrimaSistemazione / 100)) * _indennitaDiServizio, 8);
                }
                else
                {
                    _indennitaSistemazione = Math.Round(_coefficienteIndennitaSistemazione * _indennitaPersonale, 8);
                    _indennitaSistemazioneAnticipabile = Math.Round(_coefficienteIndennitaSistemazione * _indennitaDiServizio, 8);
                }
            }
        }



        private void CalcolaContributoOmniComprensivoPartenza()
        {
            var ps = _trasferimento.PRIMASITEMAZIONE;
            var lps =
                ps.PERCENTUALEFKM.Where(
                    a =>
                        a.ANNULLATO == false && _trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                        _trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

            if (lps?.Any() ?? false)
            {
                var pfkm = lps.First();
                _fasciaKMPartenza = pfkm.FASCIA_KM;
                _percentualeFKMPartenza = pfkm.COEFFICIENTEKM;

                var tePartenza = _trasferimento.TEPARTENZA;

                if (tePartenza.ATTIVITATEPARTENZA.Any(a => a.ANNULLATO == false && a.RICHIESTATRASPORTOEFFETTI == true && a.ATTIVAZIONETRASPORTOEFFETTI == true))
                {
                    var lpa =
                        tePartenza.PERCENTUALEANTICIPOTE.Where(
                            a =>
                                a.ANNULLATO == false && a.IDTIPOANTICIPOTE == (decimal)EnumTrasportoEffetti.Partenza &&
                                _trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                _trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                            .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                            .ToList();
                    if (lpa?.Any() ?? false)
                    {
                        var pa = lpa.First();
                        _percentualeAnticipoTEPartenza = pa.PERCENTUALE;
                        _percentualeSaldoTEPartenza = 100 - _percentualeAnticipoTEPartenza;

                        _totaleContributoOmnicomprensivoPartenza = Math.Round(_indennitaSistemazione * (_percentualeFKMPartenza / 100), 8);

                        _anticipoContributoOmnicomprensivoPartenza = Math.Round(_totaleContributoOmnicomprensivoPartenza * (_percentualeAnticipoTEPartenza / 100), 8);

                        _saldoContributoOmnicomprensivoPartenza = Math.Round(_totaleContributoOmnicomprensivoPartenza * (_percentualeSaldoTEPartenza / 100), 8);

                    }
                }




            }


        }

        private void CalcolaContributoOmniComprensivoRientro()
        {
            var lric = _trasferimento.RICHIAMO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.DATARICHIAMO);

            if (lric?.Any() ?? false)
            {
                var ric = lric.First();
                DateTime dataRientro = _trasferimento.DATARIENTRO;

                var lpfk =
                ric.PERCENTUALEFKM.Where(
                    a =>
                        a.ANNULLATO == false && dataRientro >= a.DATAINIZIOVALIDITA &&
                        dataRientro <= a.DATAFINEVALIDITA)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                if (lpfk?.Any() ?? false)
                {
                    var pfkm = lpfk.First();
                    _fasciaKMRientro = pfkm.FASCIA_KM;
                    _percentualeFKMRientro = pfkm.COEFFICIENTEKM;

                    var teRientro = _trasferimento.TERIENTRO;

                    if (teRientro.ATTIVITATERIENTRO.Any(a => a.ANNULLATO == false && a.RICHIESTATRASPORTOEFFETTI == true && a.ATTIVAZIONETRASPORTOEFFETTI == true))
                    {
                        var lpa =
                            teRientro.PERCENTUALEANTICIPOTE.Where(
                                a =>
                                    a.ANNULLATO == false && a.IDTIPOANTICIPOTE == (decimal)EnumTrasportoEffetti.Rientro &&
                                    dataRientro >= a.DATAINIZIOVALIDITA &&
                                    dataRientro <= a.DATAFINEVALIDITA)
                                .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                .ToList();

                        if (lpa?.Any() ?? false)
                        {
                            var pa = lpa.First();
                            _percentualeAnticipoTERientro = pa.PERCENTUALE;
                            _percentualeSaldoTERientro = 100 - _percentualeAnticipoTERientro;

                            _totaleContributoOmnicomprensivoRientro = _indennitaRichiamoLordo * (_percentualeFKMRientro / 100);

                            _anticipoContributoOmnicomprensivoRientro = Math.Round((_totaleContributoOmnicomprensivoRientro * (_percentualeAnticipoTERientro / 100)), 8);

                            _saldoContributoOmnicomprensivoRientro = Math.Round((_totaleContributoOmnicomprensivoRientro * (_percentualeSaldoTERientro / 100)), 8);

                        }
                    }


                }





            }


        }


        #endregion









        /// <summary>
        /// Preleva l'importo dell'anticipo.
        /// </summary>
        /// <param name="percentualeAnticipo"></param>
        /// <returns></returns>
        public decimal AnticipoPrimaSistemazione(decimal percentualeAnticipo)
        {
            return Math.Round(_indennitaSistemazioneAnticipabile * percentualeAnticipo / 100, 8);
        }

        /// <summary>
        /// Elabora l'anticipo della prima sistemazione ed elabora la percentuale richiesta.
        /// </summary>
        /// <param name="indennitaDiBase"></param>
        /// <param name="coefficenteDiSede"></param>
        /// <param name="percentualeDiDisagio"></param>
        /// <param name="percentualeRiduzione"></param>
        /// <param name="coefficenteIndSistemazione"></param>
        /// <returns></returns>
        public static void ElaboraPrimaSistemazione(decimal indennitaDiBase, decimal coefficenteDiSede, decimal percentualeDiDisagio, decimal percentualeRiduzione, decimal coefficenteIndSistemazione, decimal percentualeMagConiuge, decimal pensioneConiuge, ICollection<ELABDATIFIGLI> ledf, out decimal indPrimaSistemazioneAnticipabile, out decimal indPrimaSistemazioneUnicaSoluzione, out decimal maggiorazioniFamiliari)
        {
            decimal indServ = 0;
            //decimal maggiorazioniFamiliari = 0;
            decimal maggiorazioneConiuge = 0;
            decimal maggiorazioneConiugeMenoPensione = 0;
            decimal maggiorazioniFigli = 0;



            indServ = (((indennitaDiBase * coefficenteDiSede) +
                        indennitaDiBase) +
                       (((indennitaDiBase * coefficenteDiSede) +
                         indennitaDiBase) * (percentualeDiDisagio / 100)));

            maggiorazioneConiuge = indServ * percentualeMagConiuge / 100;


            if (pensioneConiuge < maggiorazioneConiuge)
            {
                maggiorazioneConiugeMenoPensione = maggiorazioneConiuge - pensioneConiuge;
            }

            if (ledf?.Any() ?? false)
            {
                foreach (ELABDATIFIGLI edf in ledf)
                {
                    decimal indServPS = (((edf.INDENNITAPRIMOSEGRETARIO * coefficenteDiSede) +
                                          edf.INDENNITAPRIMOSEGRETARIO) +
                                         (((edf.INDENNITAPRIMOSEGRETARIO * coefficenteDiSede) +
                                           edf.INDENNITAPRIMOSEGRETARIO) * (percentualeDiDisagio / 100)));

                    maggiorazioniFigli += indServPS * edf.PERCENTUALEMAGGIORAZIONEFIGLI / 100;
                }
            }

            maggiorazioniFamiliari = maggiorazioneConiugeMenoPensione + maggiorazioniFigli;

            if (percentualeRiduzione > 0)
            {
                indPrimaSistemazioneAnticipabile = Math.Round((coefficenteIndSistemazione * indServ) * percentualeRiduzione, 8);
                indPrimaSistemazioneUnicaSoluzione = Math.Round((coefficenteIndSistemazione * (indServ + maggiorazioniFamiliari)) * percentualeRiduzione, 8);

            }
            else
            {
                indPrimaSistemazioneAnticipabile = Math.Round(coefficenteIndSistemazione * indServ, 8);
                indPrimaSistemazioneUnicaSoluzione = Math.Round((coefficenteIndSistemazione * (indServ + maggiorazioniFamiliari)), 8);
            }



        }

        public decimal RateoIndennitaPersonale(int giorniRateo)
        {
            decimal ret = 0;

            ret = Math.Round((_indennitaPersonale / 30) * giorniRateo, 8);

            return ret;

        }

        public void CalcolaGiorniSospensione(DateTime dtIni, DateTime dtFin, int giorniRateoIndPers, out int oGiorniSospensione, out decimal oImportoAbbattimentoSospensione)
        {
            oGiorniSospensione = 0;
            oImportoAbbattimentoSospensione = 0;
            decimal _impSosp = 0;

            var lSosp =
                _trasferimento.SOSPENSIONE.Where(
                    a =>
                        a.ANNULLATO == false && a.IDTIPOSOSPENSIONE == (decimal)EnumTipoSospensione.Idennita &&
                        dtIni <= a.DATAFINE && dtFin >= a.DATAINIZIO)
                    .OrderByDescending(a => a.DATAINIZIO)
                    .ToList();

            if (lSosp?.Any() ?? false)
            {
                int gs = 0;
                decimal impSosp = 0;

                foreach (var sosp in lSosp)
                {
                    using (GiorniRateo gr = new GiorniRateo(sosp.DATAINIZIO, sosp.DATAFINE))
                    {
                        gs = gr.RateoGiorni;
                        impSosp = (_indennitaPersonale / giorniRateoIndPers) * gs;
                    }
                    oGiorniSospensione += gs;
                    _impSosp += impSosp;
                }

                oImportoAbbattimentoSospensione = Math.Round(_impSosp, 8);

            }
        }







        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}