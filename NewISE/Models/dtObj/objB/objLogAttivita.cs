using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj.objB
{
    public enum enumAttivita
    {
        Inserimento = 1,
        Eliminazione = 2,
        Modifica = 3
    }


    public class objLogAttivita : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Log(enumAttivita attivita, string descrizioneAttivita, string tabellaCoinvolta = "", decimal? idtabellaCoinvolta = null, decimal? idTrasferimento = null)
        {
            AccountModel ac = new AccountModel();
            LogAttivitaModel lam = new LogAttivitaModel();

            using (dtLogAttivita dtl = new dtLogAttivita())
            {
                try
                {
                    ac = Utility.UtenteAutorizzato();

                    if (ac != null && ac.idDipendente > 0)
                    {
                        lam.idDipendente = ac.idDipendente;
                        lam.idTrasferimento = idTrasferimento;
                        lam.idAttivitaCrud = (decimal)attivita;
                        lam.dataOperazione = DateTime.Now;
                        lam.descAttivitaSvolta = descrizioneAttivita;
                        lam.tabellaCoinvolta = tabellaCoinvolta;
                        lam.idTabellaCoinvolta = idtabellaCoinvolta;

                        dtl.SetLogAttivita(lam);
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