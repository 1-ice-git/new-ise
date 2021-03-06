﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.Enumeratori
{
    public enum EnumTipoAnticipoTE
    {
        Partenza = 1,
        Rientro = 2
    }

    public enum EnumTipologiaCoan
    {
        Servizi_Istituzionali = 1,
        Servizi_Promozionali = 2
    }

    public enum EnumTipoTrasferimento
    {
        ItaliaEstero = 1,
        EsteroEstero = 2,
        EsteroEsteroStessaRegiona = 3
    }

    public enum EnumAttivitaCrud
    {
        Inserimento = 1,
        Eliminazione = 2,
        Modifica = 3,
        Annullato = 4

    }

    public enum EnumRuoloAccesso
    {
        SuperAmministratore = 1,
        Amministratore = 2,
        Utente = 3
    }

    public enum EnumStatoRecord
    {
        Nullo = 0,
        In_Lavorazione = 1,
        Da_Attivare = 2,
        Attivato = 3,
        Annullato = 4
    }

    public enum EnumStatoHome
    {
        Attivi = 1,
        Completati = 2,
        Scaduti = 3,
        Tutti = 4
    }

    public enum EnumFaseViaggioCongedo
    {
        Preventivi = 1,
        Documenti_di_Viaggio = 2
    }

    public enum EnumFaseRuoloDipendente
    {
        Dirigente = 1,
        Responsabile = 2,
        Collaboratore = 3,
        Assistente = 4
    }

    public enum EnumFasePassaporti
    {
        Richiesta_Passaporti = 1,
        Invio_Passaporti = 2,
        Completati = 3
    }

    public enum TipologiaCOAN
    {
        Servizi_Istituzionali = 1,
        Servizi_Promozionali = 2
    }

    public enum EnumTipologiaAnticipi
    {
        Prima_Sistemazione = 1
    }

    public enum EnumStatoTraferimento
    {
        Attivo = 1,
        Da_Attivare = 2,
        Non_Trasferito = 3,
        Terminato = 4,
        Annullato = 5
    }

    public enum EnumTipoSospensione
    {
        Idennita = 1,
        TipoSospensione2 = 2,
        TipoSospensione3 = 3
    }

    public enum EnumRuoloUfficio
    {
        Dirigente = 1,
        Responsabile = 2,
        Collaboratore = 3,
        Assistente = 4
    }

    public enum EnumTipologiaFiglio
    {
        Residente = 1,
        StudenteResidente = 2,
        StudenteNonResidente = 3,
        NonResidente = 4
    }

    public enum EnumTipologiaConiuge
    {
        Residente = 1,
        NonResidente_A_Carico = 2,
        NonResidente = 3
    }

    public enum EnumParentela
    {
        Coniuge = 1,
        Figlio = 2,
        Richiedente = 3,
    }

    public enum EnumTipoAnticipoSaldoTE
    {
        Anticipo = 1,
        Saldo = 2
    }

    public enum EnumChiamante
    {
        Maggiorazioni_Familiari = 1,
        Titoli_Viaggio = 2,
        Trasporto_Effetti = 3,
        Trasferimento = 4,
        Passaporti = 5,
        Variazione_Maggiorazioni_Familiari = 6,
        Maggiorazione_Abitazione = 7,
        Anticipi = 8,
        ProvvidenzeScolastiche = 9,
        Variazione_Maggiorazione_Abitazione = 10
    }

    public enum EnumTipoDoc
    {
        Carta_Imbarco = 1,
        Titolo_Viaggio = 2,
        Prima_Rata_Maggiorazione_abitazione = 3,
        MAB_Modulo2_Dichiarazione_Costo_Locazione = 4,
        Attestazione_Spese_Abitazione_Collaboratore = 5,
        Clausole_Contratto_Alloggio = 6,
        Copia_Contratto_Locazione = 7,
        Contributo_Fisso_Omnicomprensivo = 8,
        Attestazione_Trasloco = 9,
        Documento_Identita = 10,
        Lettera_Trasferimento = 11,
        Formulario_Maggiorazioni_Familiari = 12,
        Formulario_Titoli_Viaggio = 13,
        Copia_Ricevuta_Pagamento_Locazione = 14,
        MAB_Modulo4_Dichiarazione_Costo_Locazione = 15,
        Preventivo_Viaggio = 16,
        Passaporto = 17,
        Formulario_Provvidenze_Scolastiche = 18,
        Contributo_Fisso_Omnicomprensivo_Rientro = 19,
        Attestazione_Trasloco_Rientro = 20,
        Attestazione_Dip_Resp = 21
    }

    public enum EnumFunzioniEventi
    {
        RichiestaMaggiorazioniFamiliari = 1,
        RichiestaPassaporto = 2,
        RichiestaTitoliViaggio = 3,
        RichiestaTrasportoEffettiPartenza = 4,
        RichiestaTrasportoEffettiRientro = 5,
        RichiestaAnticipi = 6,
        RichiestaMaggiorazioneAbitazione = 7,
        AttivaTrasferimento = 8,
        InvioPassaporto = 9,
        RichiestaProvvidenzeScolastiche = 10,
        RichiestaViaggiCongedo = 11,
        TrasferimentoDaAttivare = 12
    }

    public enum EnumTipoRegolaCalcolo
    {
        IndennitaBase = 1,
        IndennitaDIServizio = 2,
        IndennitaPersonale = 3,
        MaggiorazioneConiuge = 4,
        MaggiorazioneFigli = 5,
        AnticipoIndennitaPrimaSistemazione = 6,
        IndennitaPrimaSistemazione = 7
    }
    public enum EnumTipoInserimento
    {
        Software = 1,
        Manuale = 2
    }
    public enum EnumTipoLiquidazione
    {
        Paghe = 1,
        Contabilità = 2
    }

    public enum EnumTipoVoce
    {
        Software = 1,
        Manuale = 2
    }

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
        Detrazione_086_384 = 3,
        Trasp_Mass_Partenza_Rientro_162_131 = 9,
        Rientro_Lordo_086_381 = 13,
        Polizza_sanitaria_AC_116_030 = 14,
        Viaggio_di_congedo_086_382 = 15

    }

    public enum EnumVociContabili
    {
        Ind_Sede_Estera = 8,
        Ind_Prima_Sist_IPS = 10,
        MAB = 11,
        Ind_Richiamo_IRI = 12,
        Trat_Magg_Figli_Su_ISE_ISE = 16,
        Prov_Scolastiche_PSC = 17,
        Viaggio_Trasferimento_TRA = 18,
        Trattenute_Varie_Su_ISE_ISE = 19

    }

    public enum EnumTipoAliquoteContributive
    {
        Detrazioni_DET = 1,
        Previdenziali_PREV = 2,
        ContributoAggiuntivo_CA = 3,
        MassimaleContributoAggiuntivo_MCA = 4

    }

    public enum EnumTabTitoloViaggio
    {
        Elaborazione = 1,
        Completati = 2
    }

    public enum EnumTipoCoefficienteRichiamo
    {
        CoefficienteMaggiorazione = 1,
        CoefficienteRichiamo = 2
    }

    public enum EnumStatoElaborazione
    {
        InCorso = 1,
        Terminata = 2,
        TerminataConErrori = 3,
        Schedulata = 4
    }

    public enum EnumTipoAmbiente
    {
        Reale = 1,
        Simulazione = 2
    }

}