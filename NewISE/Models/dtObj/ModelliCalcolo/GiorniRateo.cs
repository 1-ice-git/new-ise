using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj.ModelliCalcolo
{
    public struct GiornoMeseAnno
    {
        public int Giorno { get; set; }
        public int Mese { get; set; }
        public int Anno { get; set; }
    }

    public class GiorniRateo : IDisposable
    {

        #region Proprietà pubbliche
        public int RateoGiorni => _rateoGiorni;
        public int CicliElaborazione => _cicliElaborazione;
        #endregion

        #region Proprietà private
        private int _rateoGiorni = 0;
        private int _cicliElaborazione = 0;
        #endregion



        #region Metodi pubblici
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }



        public GiorniRateo(DateTime dtInizio, DateTime dtFine, bool noGiorno = false)
        {
            int nGiorni = 0;
            int annoIni = dtInizio.Year;
            int annoFin = dtFine.Year;
            int giorno = 1;

            if (noGiorno)
            {
                giorno = 0;
            }

            for (int i = annoIni; i <= annoFin; i++)
            {
                if (i == annoIni && i == annoFin)
                {
                    int meseIni = dtInizio.Month;
                    int meseFin = dtFine.Month;

                    for (int j = meseIni; j <= meseFin; j++)
                    {
                        if (j == meseIni && j == meseFin)
                        {
                            int giornoIni = dtInizio.Day;
                            int giornoFin = dtFine.Day;

                            if (giornoIni == 1 && this.VerificaFineMese(giornoFin, meseFin, annoFin))
                            {
                                nGiorni = 30;
                            }
                            else
                            {
                                if (this.VerificaFineMese(giornoFin, meseFin, annoFin))
                                {
                                    if (giornoIni == 31)
                                    {
                                        nGiorni = 31 - giornoIni + 1;
                                    }
                                    else
                                    {
                                        nGiorni = 30 - giornoIni + giorno;
                                    }
                                }
                                else
                                {
                                    nGiorni = giornoFin - giornoIni + giorno;
                                }
                            }
                        }
                        else
                        {
                            int giornoIni = dtInizio.Day;
                            int giornoFin = dtFine.Day;

                            if (j == meseIni)
                            {
                                if (giornoIni == 1)
                                {
                                    nGiorni += 30;
                                }
                                else
                                {
                                    if (giornoIni == 31)
                                    {
                                        nGiorni += 31 - giornoIni + giorno;
                                    }
                                    else
                                    {
                                        nGiorni += 30 - giornoIni + giorno;
                                    }
                                }
                            }
                            else if (j > meseIni && j < meseFin)
                            {
                                nGiorni += 30;
                            }
                            else if (j > meseIni && j == meseFin)
                            {
                                if (this.VerificaFineMese(giornoFin, meseFin, annoFin))
                                {
                                    nGiorni += 30;
                                }
                                else
                                {
                                    nGiorni += giornoFin;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (i == annoIni)
                    {
                        int meseIni = dtInizio.Month;
                        int giornoIni = dtInizio.Day;

                        for (int j = meseIni; j <= 12; j++)
                        {
                            //int giornoFineMese = this.GetGiornoFineMese(j, i).Giorno;

                            if (j == meseIni)
                            {

                                if (giornoIni == 31)
                                {
                                    nGiorni += 31 - giornoIni + 1;
                                }
                                else
                                {
                                    nGiorni += 30 - giornoIni + 1;
                                }
                            }
                            else
                            {
                                nGiorni += 30;
                            }
                        }
                    }
                    else if (i > annoIni && i < annoFin)
                    {
                        for (int j = 1; j <= 12; j++)
                        {
                            nGiorni += 30;
                        }
                    }
                    else if (i > annoIni && i == annoFin)
                    {
                        int meseFin = dtFine.Month;
                        int giornoFin = dtFine.Day;

                        for (int j = 1; j <= meseFin; j++)
                        {
                            if (j < meseFin)
                            {
                                nGiorni += 30;
                            }
                            else if (j == meseFin)
                            {
                                if (this.VerificaFineMese(giornoFin, meseFin, annoFin))
                                {
                                    nGiorni += 30;
                                }
                                else
                                {
                                    nGiorni += giornoFin;
                                }
                            }
                        }
                    }
                }
            }

            _rateoGiorni = nGiorni;

            this.ElaboraCicliElaborazione(dtInizio, dtFine);
        }
        #endregion

        #region Metodi privati
        private GiornoMeseAnno GetGiornoFineMese(int mese, int anno)
        {
            GiornoMeseAnno gma = new GiornoMeseAnno();

            int giorno = 0;

            switch (mese)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    giorno = 31;
                    break;
                case 2:
                    if (DateTime.IsLeapYear(anno))
                    {
                        giorno = 29;
                    }
                    else
                    {
                        giorno = 28;
                    }
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    giorno = 30;
                    break;
            }

            gma.Giorno = giorno;
            gma.Mese = mese;
            gma.Anno = anno;

            return gma;
        }

        private bool VerificaFineMese(int giorno, int mese, int anno)
        {
            bool ret = false;

            switch (mese)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    if (giorno == 31)
                    {
                        ret = true;
                    }
                    break;
                case 2:
                    if (DateTime.IsLeapYear(anno))
                    {
                        if (giorno == 29)
                        {
                            ret = true;
                        }
                    }
                    else
                    {
                        if (giorno == 28)
                        {
                            ret = true;
                        }
                    }
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    if (giorno == 30)
                    {
                        ret = true;
                    }
                    break;
            }


            return ret;
        }


        private void ElaboraCicliElaborazione(DateTime dtInizio, DateTime dtFine)
        {
            int annoIni = dtInizio.Year;
            int annoFin = dtFine.Year;
            int cicli = 0;


            for (int i = annoIni; i <= annoFin; i++)
            {
                if (i == annoIni && i == annoFin)
                {
                    int meseIni = dtInizio.Month;
                    int meseFin = dtFine.Month;

                    for (int j = meseIni; j <= meseFin; j++)
                    {
                        cicli++;
                    }
                }
                else
                {
                    if (i == annoIni)
                    {
                        int meseIni = dtInizio.Month;

                        for (int j = meseIni; j <= 12; j++)
                        {
                            cicli++;
                        }

                    }
                    else if (i > annoIni && i < annoFin)
                    {
                        for (int j = 1; j <= 12; j++)
                        {
                            cicli++;
                        }
                    }
                    else if (i > annoIni && i == annoFin)
                    {
                        int meseFin = dtFine.Month;


                        for (int j = 1; j <= meseFin; j++)
                        {
                            cicli++;
                        }
                    }
                }
            }

            _cicliElaborazione = cicli;


        }
        #endregion
    }
}