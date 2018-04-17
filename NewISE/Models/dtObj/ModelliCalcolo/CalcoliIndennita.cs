using NewISE.EF;
using NewISE.Models.dtObj.Interfacce;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.Enum;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.dtObj.ModelliCalcolo
{
    public class CalcoliIndennita : Attribute, IDisposable
    {
        #region Proprietà private
        private decimal _indennitaDiBase = 0;
        private decimal _riduzioneIndennitaDiBase = 0;
        private decimal _coefficenteDiSede = 0;
        private decimal _percentualeDisagio = 0;
        private decimal _indennitaDiServizio = 0;
        private decimal _percentualeMaggiorazioneConiuge = 0;
        private decimal _maggiorazioneConiuge = 0;
        private decimal _pensioneConiuge = 0;
        private decimal _maggiorazioneConiugeMenoPensione = 0;
        private decimal _percentualeMaggiorazioniFigli = 0;
        private decimal _indennitaPrimoSegretario = 0;
        private decimal _indennitaServizioPrimoSegretario = 0;
        private decimal _maggiorazioneFigli = 0;
        private decimal _maggiorazioniFimailiri = 0;
        private decimal _indennitaPersonale = 0;
        private decimal _indennitaSistemazione = 0;
        private decimal _percentualeRiduzionePrimaSistemazione = 0;
        private decimal _indennitaSistemazioneAnticipabile = 0;
        private decimal _percentualeFKMPartenza = 0;
        private decimal _anticipoContributoOmnicomprensivoPartenza = 0;
        private decimal _percentualeAnticipoTEPartenza = 0;
        private decimal _percentualeSaldoTEPartenza = 0;
        private decimal _saldoContributoOmnicomprensivoPartenza = 0;




        private DateTime _dtDatiParametri;
        private TRASFERIMENTO _trasferimento = new TRASFERIMENTO();
        private INDENNITA _indennita = new INDENNITA();

        private RUOLODIPENDENTE _ruoloDipendente = new RUOLODIPENDENTE();
        private RUOLOUFFICIO _ruoloUfficio = new RUOLOUFFICIO();

        #endregion


        [ReadOnly(true)]
        public decimal IndennitaDiBase => _indennitaDiBase;
        [ReadOnly(true)]
        public decimal CoefficenteDiSede => _coefficenteDiSede;
        [ReadOnly(true)]
        public decimal PercentualeDisagio => _percentualeDisagio;
        [ReadOnly(true)]
        public decimal IndennitaDiServizio => _indennitaDiServizio;
        [ReadOnly(true)]
        public decimal PercentualeMAggiorazioneConiuge => _percentualeMaggiorazioneConiuge;
        [ReadOnly(true)]
        public decimal MaggiorazioneConiuge => _maggiorazioneConiuge;
        [ReadOnly(true)]
        public decimal PensioneConiuge => _pensioneConiuge;
        [ReadOnly(true)]
        public decimal MaggiorazioneConiugeMenoPensione => _maggiorazioneConiugeMenoPensione;
        [ReadOnly(true)]
        public decimal PercentualeMaggiorazioneFigli => _percentualeMaggiorazioniFigli;
        [ReadOnly(true)]
        public decimal IndennitaPrimoSegretario => _indennitaPrimoSegretario;
        [ReadOnly(true)]
        public decimal IndennitaServizioPrimoSegretario => _indennitaServizioPrimoSegretario;
        [ReadOnly(true)]
        public decimal MaggiorazioneFigli => _maggiorazioneFigli;
        [ReadOnly(true)]
        public decimal MaggiorazioniFamiliari => _maggiorazioniFimailiri;
        [ReadOnly(true)]
        public decimal IndennitaPersonale => _indennitaPersonale;
        [ReadOnly(true)]
        public decimal IndennitaSistemazione => _indennitaSistemazione;
        [ReadOnly(true)]
        public decimal PercentualeRiduzionePrimaSistemazione => _percentualeRiduzionePrimaSistemazione;
        [ReadOnly(true)]
        public decimal IndennitaSistemazioneAnticipabile => _indennitaSistemazioneAnticipabile;
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


        public CalcoliIndennita(decimal idTrasferimento, DateTime? dataCalcoloIndennita = null)
        {
            DateTime dt;


            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    if (dataCalcoloIndennita.HasValue)
                    {
                        dt = dataCalcoloIndennita.Value;
                    }
                    else
                    {
                        dt = DateTime.Now;
                    }

                    _trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);

                    if (_trasferimento.DATARIENTRO.HasValue)
                    {
                        if (_trasferimento.DATARIENTRO.Value < dt)
                        {
                            _dtDatiParametri = _trasferimento.DATARIENTRO.Value;
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
                    this.CalcolaPrimaSistemazione();
                    this.CalcolaContributoOmniComprensivoPartenza();


                }
                catch (Exception ex)
                {

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

            var lib =
                _indennita.INDENNITABASE.Where(
                    a =>
                        a.ANNULLATO == false &&
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
                        _indennitaDiBase = valRespIB * _riduzioneIndennitaDiBase / 100;
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
                        _indennitaDiBase = valIB * _riduzioneIndennitaDiBase / 100;
                    }
                    else
                    {
                        _indennitaDiBase = valIB;
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
                _coefficenteDiSede = coefficenteSede.VALORECOEFFICIENTE;
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
                var indServ = (((_indennitaDiBase * _coefficenteDiSede) +
                                _indennitaDiBase) +
                               (((_indennitaDiBase * _coefficenteDiSede) +
                                 _indennitaDiBase) / 100 * _percentualeDisagio));

                _indennitaDiServizio = indServ;
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
                        var lpmf =
                            f.PERCENTUALEMAGFIGLI.Where(
                                a =>
                                    a.ANNULLATO == false && _dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                    _dtDatiParametri <= a.DATAFINEVALIDITA)
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
                                                                       _coefficenteDiSede) +
                                                                      _indennitaPrimoSegretario) +
                                                                     (((_indennitaPrimoSegretario *
                                                                        _coefficenteDiSede) +
                                                                       _indennitaPrimoSegretario) /
                                                                      100 * _coefficenteDiSede));

                                _maggiorazioneFigli = _indennitaServizioPrimoSegretario *
                                                      _percentualeMaggiorazioniFigli / 100;




                            }



                        }


                    }


                }
                #endregion

                _maggiorazioniFimailiri = _maggiorazioneConiugeMenoPensione + _maggiorazioneFigli;
            }

        }

        private void CalcolaIndennitaPersonale()
        {
            _indennitaPersonale = _indennitaDiServizio + _maggiorazioniFimailiri;
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
                    _indennitaSistemazione = (indSist.COEFFICIENTE * _percentualeRiduzionePrimaSistemazione) * _indennitaPersonale;
                    _indennitaSistemazioneAnticipabile = (indSist.COEFFICIENTE * _percentualeRiduzionePrimaSistemazione) * _indennitaDiServizio;
                }
                else
                {
                    _indennitaSistemazione = indSist.COEFFICIENTE * _indennitaPersonale;
                    _indennitaSistemazioneAnticipabile = indSist.COEFFICIENTE * _indennitaDiServizio;
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
                _percentualeFKMPartenza = pfkm.COEFFICIENTEKM;

                var lpa =
                    _trasferimento.TEPARTENZA.PERCENTUALEANTICIPOTE.Where(
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

                    _anticipoContributoOmnicomprensivoPartenza = (_indennitaSistemazione * (_percentualeFKMPartenza / 100) *
                                                                  (_percentualeAnticipoTEPartenza / 100));

                    _saldoContributoOmnicomprensivoPartenza = (_indennitaSistemazione * (_percentualeFKMPartenza / 100) *
                                                                  (_percentualeSaldoTEPartenza / 100));

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
            return _indennitaSistemazioneAnticipabile * percentualeAnticipo / 100;
        }




        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}