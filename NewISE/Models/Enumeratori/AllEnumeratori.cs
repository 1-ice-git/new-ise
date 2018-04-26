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

    public enum EnumTrasportoEffetti
    {
        Partenza = 1,
        Rientro = 2
    }

    public enum EnumTipoAnticipi
    {
        Prima_sistemazione = 1
    }

    public enum EnumTipoMovimento
    {
        MeseCorrente_M = 1,
        Conguaglio_C = 2
    }

    public enum EnumVociCedolino
    {
        Sistemazione_Richiamo_Netto_086_383 = 1,
        Sistemazione_Lorda_086_380 = 2,
        Detrazione_086_384 = 3

    }

    public enum EnumVociContabili
    {
        Ind_Prima_Sist_IPS = 4
    }

    public enum EnumTipoAliquoteContributive
    {
        Detrazioni_DET = 1,
        Previdenziali_PREV = 2
    }
}