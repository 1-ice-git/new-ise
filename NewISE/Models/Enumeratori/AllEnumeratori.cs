using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.Enumeratori
{
    public enum EnumCicloAttivazione
    {
        Notifica = 1,
        Conferma = 2,
        Annulla = 3
    }

    public enum EnumFunzioniRiduzione
    {
        Indennita_Base = 1,
        Indennita_Sistemazione = 2,
        Coefficente_Richiamo = 3
    }
}