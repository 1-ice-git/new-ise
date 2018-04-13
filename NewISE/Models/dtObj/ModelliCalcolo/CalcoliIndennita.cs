using NewISE.EF;
using NewISE.Models.dtObj.Interfacce;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.Enum;

namespace NewISE.Models.dtObj.ModelliCalcolo
{
    public class CalcoliIndennita : Attribute
    {
        #region Proprietà private
        private decimal _indennitaDiBase = 0;
        private decimal _riduzioneIndennitaDiBase = 0;
        private decimal _coefficenteDiSede = 0;
        private decimal _percentualeDisagio = 0;
        private decimal _indennitaDiServizio = 0;
        private decimal _coefficenteRiduzione = 0;
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
        private decimal _indennitaSistemazioneAnticipabile = 0;
        private decimal _anticipoSistemazione = 0;
        private decimal _saldoSistemazione = 0;





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
        public decimal CoefficenteDiRiduzione => _coefficenteRiduzione;
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
        public decimal IndennitaSistemazioneAnticipabile => _indennitaSistemazioneAnticipabile;
        [ReadOnly(true)]
        public decimal AnticipoSistemazione => _anticipoSistemazione;
        [ReadOnly(true)]
        public decimal SaldoSistemazione => _saldoSistemazione;






        public CalcoliIndennita(decimal idTrasferimento, DateTime? dataCalcoloIndennita = null)
        {
            DateTime dt;


            using (ModelDBISE db = new ModelDBISE())
            {

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




                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }




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
                indServ = (((_indennitaDiBase * _coefficenteDiSede) +
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




        public void CalcoloIndennita(decimal idTrasferimento, DateTime? dataCalcoloIndennita = null)
        {
            DateTime? dt;
            RUOLODIPENDENTE ruoloDipendente = new RUOLODIPENDENTE();
            RUOLOUFFICIO ruoloUfficio = new RUOLOUFFICIO();
            DateTime dtDatiParametri;


            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    if (dataCalcoloIndennita.HasValue)
                    {
                        dt = dataCalcoloIndennita;
                    }
                    else
                    {
                        dt = DateTime.Now;
                    }

                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);

                    if (trasferimento.DATARIENTRO.HasValue)
                    {
                        if (trasferimento.DATARIENTRO.Value < dt.Value)
                        {
                            dtDatiParametri = trasferimento.DATARIENTRO.Value;
                        }
                        else
                        {
                            if (trasferimento.DATAPARTENZA > dt.Value)
                            {
                                dtDatiParametri = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtDatiParametri = dt.Value;
                            }
                        }
                    }
                    else
                    {
                        if (trasferimento.DATAPARTENZA > dt.Value)
                        {
                            dtDatiParametri = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtDatiParametri = dt.Value;
                        }
                    }

                    var indennita = trasferimento.INDENNITA;

                    var lrd =
                        trasferimento.RUOLODIPENDENTE.Where(
                            a =>
                                a.ANNULLATO == false && dtDatiParametri >= a.DATAINZIOVALIDITA &&
                                dtDatiParametri <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINZIOVALIDITA);

                    if (lrd?.Any() ?? false)
                    {
                        #region Ruolo dipendente
                        ruoloDipendente = lrd.First();
                        ruoloUfficio = ruoloDipendente.RUOLOUFFICIO;
                        #endregion

                        #region Indennita di base estera
                        RIDUZIONI riduzioniIB = new RIDUZIONI();

                        var lib =
                            indennita.INDENNITABASE.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    dtDatiParametri >= a.DATAINIZIOVALIDITA && dtDatiParametri <= a.DATAFINEVALIDITA)
                                .OrderByDescending(a => a.DATAINIZIOVALIDITA);

                        if (lib?.Any() ?? false)
                        {
                            var indennitaBase = lib.First();

                            var lr =
                                indennitaBase.RIDUZIONI.Where(
                                    a =>
                                        a.ANNULLATO == false && dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                        dtDatiParametri <= a.DATAFINEVALIDITA)
                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA);

                            if (lr?.Any() ?? false)
                            {
                                riduzioniIB = lr.First();
                            }

                            if (ruoloUfficio.IDRUOLO == (decimal)EnumRuoloUfficio.Dirigente || ruoloUfficio.IDRUOLO == (decimal)EnumRuoloUfficio.Responsabile)
                            {
                                decimal valRespIB = indennitaBase.VALORERESP;
                                decimal valRidIB = 0;

                                if (riduzioniIB?.IDRIDUZIONI > 0)
                                {
                                    valRidIB = riduzioniIB.PERCENTUALE;
                                }
                                if (valRidIB > 0)
                                {
                                    _indennitaDiBase = valRespIB * valRidIB / 100;
                                }
                                else
                                {
                                    _indennitaDiBase = valRespIB;
                                }
                            }
                            else
                            {
                                decimal valIB = indennitaBase.VALORE;
                                decimal valRidIB = 0;

                                if (riduzioniIB?.IDRIDUZIONI > 0)
                                {
                                    valRidIB = riduzioniIB.PERCENTUALE;
                                }
                                if (valRidIB > 0)
                                {
                                    _indennitaDiBase = valIB * valRidIB / 100;
                                }
                                else
                                {
                                    _indennitaDiBase = valIB;
                                }

                            }
                        }

                        #endregion

                        if (_indennitaDiBase > 0)
                        {
                            #region Indennità di servizio
                            var lcs =
                                indennita.COEFFICIENTESEDE.Where(
                                    a =>
                                        a.ANNULLATO == false && dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                        dtDatiParametri <= a.DATAFINEVALIDITA)
                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                            if (lcs?.Any() ?? false)
                            {
                                var coefficenteSede = lcs.First();
                                _coefficenteDiSede = coefficenteSede.VALORECOEFFICIENTE;

                                var lpd =
                                    indennita.PERCENTUALEDISAGIO.Where(
                                        a =>
                                            a.ANNULLATO == false && dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                            dtDatiParametri <= a.DATAFINEVALIDITA)
                                        .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                if (lpd?.Any() ?? false)
                                {
                                    var percentualeDisagio = lpd.First();
                                    _percentualeDisagio = percentualeDisagio.PERCENTUALE;

                                    _indennitaDiServizio = (((_indennitaDiBase * coefficenteSede.VALORECOEFFICIENTE) +
                                                             _indennitaDiBase) +
                                                            (((_indennitaDiBase * coefficenteSede.VALORECOEFFICIENTE) +
                                                              _indennitaDiBase) / 100 * percentualeDisagio.PERCENTUALE));
                                }
                            }
                            #endregion

                            if (_indennitaDiServizio > 0)
                            {
                                #region Maggiorazioni familiari
                                decimal valoreMF = 0;
                                var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                                var lattivazioneMF =
                                    mf.ATTIVAZIONIMAGFAM.Where(
                                        a =>
                                            a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                            a.ATTIVAZIONEMAGFAM == true)
                                        .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                                if (lattivazioneMF?.Any() ?? false)
                                {
                                    var lc =
                                        mf.CONIUGE.Where(
                                            a =>
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                                dtDatiParametri <= a.DATAFINEVALIDITA)
                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                    if (lc?.Any() ?? false)
                                    {
                                        var coniuge = lc.First();
                                        var lpmc =
                                            coniuge.PERCENTUALEMAGCONIUGE.Where(
                                                a =>
                                                    a.ANNULLATO == false &&
                                                    a.IDTIPOLOGIACONIUGE == coniuge.IDTIPOLOGIACONIUGE &&
                                                    dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                                    dtDatiParametri <= a.DATAFINEVALIDITA)
                                                .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
                                        if (lpmc?.Any() ?? false)
                                        {
                                            var percentualeMaggiorazioneConiuge = lpmc.First();

                                            _maggiorazioneConiuge = _indennitaDiServizio *
                                                                   percentualeMaggiorazioneConiuge.PERCENTUALECONIUGE /
                                                                   100;
                                        }
                                    }


                                }
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




        }






    }
}