﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE.EF
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ModelDBISE : DbContext
    {
        public ModelDBISE()
            : base("name=ModelDBISE")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ACCESSI> ACCESSI { get; set; }
        public virtual DbSet<ALIQUOTECONTRIBUTIVE> ALIQUOTECONTRIBUTIVE { get; set; }
        public virtual DbSet<ALTRIDATIFAM> ALTRIDATIFAM { get; set; }
        public virtual DbSet<ANTICIPI> ANTICIPI { get; set; }
        public virtual DbSet<ANTICIPOSALDOTE> ANTICIPOSALDOTE { get; set; }
        public virtual DbSet<ATTIVAZIONEMAB> ATTIVAZIONEMAB { get; set; }
        public virtual DbSet<ATTIVAZIONETITOLIVIAGGIO> ATTIVAZIONETITOLIVIAGGIO { get; set; }
        public virtual DbSet<ATTIVAZIONIMAGFAM> ATTIVAZIONIMAGFAM { get; set; }
        public virtual DbSet<ATTIVAZIONIPASSAPORTI> ATTIVAZIONIPASSAPORTI { get; set; }
        public virtual DbSet<ATTIVAZIONIPROVSCOLASTICHE> ATTIVAZIONIPROVSCOLASTICHE { get; set; }
        public virtual DbSet<ATTIVAZIONIVIAGGICONGEDO> ATTIVAZIONIVIAGGICONGEDO { get; set; }
        public virtual DbSet<ATTIVITAANTICIPI> ATTIVITAANTICIPI { get; set; }
        public virtual DbSet<ATTIVITACRUD> ATTIVITACRUD { get; set; }
        public virtual DbSet<ATTIVITATEPARTENZA> ATTIVITATEPARTENZA { get; set; }
        public virtual DbSet<ATTIVITATERIENTRO> ATTIVITATERIENTRO { get; set; }
        public virtual DbSet<CALENDARIOEVENTI> CALENDARIOEVENTI { get; set; }
        public virtual DbSet<CANONEMAB> CANONEMAB { get; set; }
        public virtual DbSet<CDCGEPE> CDCGEPE { get; set; }
        public virtual DbSet<COEFFICIENTEINDRICHIAMO> COEFFICIENTEINDRICHIAMO { get; set; }
        public virtual DbSet<COEFFICIENTESEDE> COEFFICIENTESEDE { get; set; }
        public virtual DbSet<CONIUGE> CONIUGE { get; set; }
        public virtual DbSet<CONIUGEPASSAPORTO> CONIUGEPASSAPORTO { get; set; }
        public virtual DbSet<CONIUGETITOLIVIAGGIO> CONIUGETITOLIVIAGGIO { get; set; }
        public virtual DbSet<DESTINATARI> DESTINATARI { get; set; }
        public virtual DbSet<DIPENDENTI> DIPENDENTI { get; set; }
        public virtual DbSet<DOC_ATER> DOC_ATER { get; set; }
        public virtual DbSet<DOCUMENTI> DOCUMENTI { get; set; }
        public virtual DbSet<ELABINDENNITA> ELABINDENNITA { get; set; }
        public virtual DbSet<ELABINDSISTEMAZIONE> ELABINDSISTEMAZIONE { get; set; }
        public virtual DbSet<ELABMAB> ELABMAB { get; set; }
        public virtual DbSet<ELABTRASPEFFETTI> ELABTRASPEFFETTI { get; set; }
        public virtual DbSet<EMAILSECONDARIEDIP> EMAILSECONDARIEDIP { get; set; }
        public virtual DbSet<FASCIA_KM> FASCIA_KM { get; set; }
        public virtual DbSet<FASEPASSAPORTI> FASEPASSAPORTI { get; set; }
        public virtual DbSet<FIGLI> FIGLI { get; set; }
        public virtual DbSet<FIGLIPASSAPORTO> FIGLIPASSAPORTO { get; set; }
        public virtual DbSet<FIGLITITOLIVIAGGIO> FIGLITITOLIVIAGGIO { get; set; }
        public virtual DbSet<FLUSSICEDOLINO> FLUSSICEDOLINO { get; set; }
        public virtual DbSet<FUNZIONERIDUZIONE> FUNZIONERIDUZIONE { get; set; }
        public virtual DbSet<FUNZIONIEVENTI> FUNZIONIEVENTI { get; set; }
        public virtual DbSet<GRUPPO_FKM> GRUPPO_FKM { get; set; }
        public virtual DbSet<INDENNITA> INDENNITA { get; set; }
        public virtual DbSet<INDENNITABASE> INDENNITABASE { get; set; }
        public virtual DbSet<INDENNITAPRIMOSEGRETARIO> INDENNITAPRIMOSEGRETARIO { get; set; }
        public virtual DbSet<INDENNITASISTEMAZIONE> INDENNITASISTEMAZIONE { get; set; }
        public virtual DbSet<LIVELLI> LIVELLI { get; set; }
        public virtual DbSet<LIVELLIDIPENDENTI> LIVELLIDIPENDENTI { get; set; }
        public virtual DbSet<LOGATTIVITA> LOGATTIVITA { get; set; }
        public virtual DbSet<MAGGIORAZIONEABITAZIONE> MAGGIORAZIONEABITAZIONE { get; set; }
        public virtual DbSet<MAGGIORAZIONIANNUALI> MAGGIORAZIONIANNUALI { get; set; }
        public virtual DbSet<MAGGIORAZIONIFAMILIARI> MAGGIORAZIONIFAMILIARI { get; set; }
        public virtual DbSet<MODIFICHEMAGFAM> MODIFICHEMAGFAM { get; set; }
        public virtual DbSet<NETTO> NETTO { get; set; }
        public virtual DbSet<NORMACALCOLO> NORMACALCOLO { get; set; }
        public virtual DbSet<NOTIFICHE> NOTIFICHE { get; set; }
        public virtual DbSet<OA> OA { get; set; }
        public virtual DbSet<PAGATOCONDIVISOMAB> PAGATOCONDIVISOMAB { get; set; }
        public virtual DbSet<PASSAPORTI> PASSAPORTI { get; set; }
        public virtual DbSet<PASSAPORTORICHIEDENTE> PASSAPORTORICHIEDENTE { get; set; }
        public virtual DbSet<PENSIONE> PENSIONE { get; set; }
        public virtual DbSet<PERCENTUALEANTICIPOTE> PERCENTUALEANTICIPOTE { get; set; }
        public virtual DbSet<PERCENTUALECONDIVISIONE> PERCENTUALECONDIVISIONE { get; set; }
        public virtual DbSet<PERCENTUALEDISAGIO> PERCENTUALEDISAGIO { get; set; }
        public virtual DbSet<PERCENTUALEFKM> PERCENTUALEFKM { get; set; }
        public virtual DbSet<PERCENTUALEMAB> PERCENTUALEMAB { get; set; }
        public virtual DbSet<PERCENTUALEMAGCONIUGE> PERCENTUALEMAGCONIUGE { get; set; }
        public virtual DbSet<PERCENTUALEMAGFIGLI> PERCENTUALEMAGFIGLI { get; set; }
        public virtual DbSet<PRIMASITEMAZIONE> PRIMASITEMAZIONE { get; set; }
        public virtual DbSet<PROVVIDENZESCOLASTICHE> PROVVIDENZESCOLASTICHE { get; set; }
        public virtual DbSet<REGOLECALCOLO> REGOLECALCOLO { get; set; }
        public virtual DbSet<RICHIAMO> RICHIAMO { get; set; }
        public virtual DbSet<RIDUZIONI> RIDUZIONI { get; set; }
        public virtual DbSet<RINUNCIA_TE_P> RINUNCIA_TE_P { get; set; }
        public virtual DbSet<RINUNCIA_TE_R> RINUNCIA_TE_R { get; set; }
        public virtual DbSet<RINUNCIAANTICIPI> RINUNCIAANTICIPI { get; set; }
        public virtual DbSet<RINUNCIAMAB> RINUNCIAMAB { get; set; }
        public virtual DbSet<RINUNCIAMAGGIORAZIONIFAMILIARI> RINUNCIAMAGGIORAZIONIFAMILIARI { get; set; }
        public virtual DbSet<RUOLOACCESSO> RUOLOACCESSO { get; set; }
        public virtual DbSet<RUOLODIPENDENTE> RUOLODIPENDENTE { get; set; }
        public virtual DbSet<RUOLOUFFICIO> RUOLOUFFICIO { get; set; }
        public virtual DbSet<SELECTDOCVC> SELECTDOCVC { get; set; }
        public virtual DbSet<SOSPENSIONE> SOSPENSIONE { get; set; }
        public virtual DbSet<STATORECORD> STATORECORD { get; set; }
        public virtual DbSet<STATOTRASFERIMENTO> STATOTRASFERIMENTO { get; set; }
        public virtual DbSet<TEORICI> TEORICI { get; set; }
        public virtual DbSet<TEPARTENZA> TEPARTENZA { get; set; }
        public virtual DbSet<TERIENTRO> TERIENTRO { get; set; }
        public virtual DbSet<TFR> TFR { get; set; }
        public virtual DbSet<TIPOALIQUOTECONTRIBUTIVE> TIPOALIQUOTECONTRIBUTIVE { get; set; }
        public virtual DbSet<TIPOANTICIPOTRASPORTOEFFETTI> TIPOANTICIPOTRASPORTOEFFETTI { get; set; }
        public virtual DbSet<TIPODOCUMENTI> TIPODOCUMENTI { get; set; }
        public virtual DbSet<TIPOELABORAZIONE> TIPOELABORAZIONE { get; set; }
        public virtual DbSet<TIPOLIQUIDAZIONE> TIPOLIQUIDAZIONE { get; set; }
        public virtual DbSet<TIPOLOGIAANTICIPI> TIPOLOGIAANTICIPI { get; set; }
        public virtual DbSet<TIPOLOGIACOAN> TIPOLOGIACOAN { get; set; }
        public virtual DbSet<TIPOLOGIACONIUGE> TIPOLOGIACONIUGE { get; set; }
        public virtual DbSet<TIPOLOGIAFIGLIO> TIPOLOGIAFIGLIO { get; set; }
        public virtual DbSet<TIPOLOGIAMODIFICHE> TIPOLOGIAMODIFICHE { get; set; }
        public virtual DbSet<TIPOMOVIMENTO> TIPOMOVIMENTO { get; set; }
        public virtual DbSet<TIPOREGOLACALCOLO> TIPOREGOLACALCOLO { get; set; }
        public virtual DbSet<TIPOSOSPENSIONE> TIPOSOSPENSIONE { get; set; }
        public virtual DbSet<TIPOTRASFERIMENTO> TIPOTRASFERIMENTO { get; set; }
        public virtual DbSet<TIPOVOCE> TIPOVOCE { get; set; }
        public virtual DbSet<TITOLIVIAGGIO> TITOLIVIAGGIO { get; set; }
        public virtual DbSet<TITOLIVIAGGIORICHIEDENTE> TITOLIVIAGGIORICHIEDENTE { get; set; }
        public virtual DbSet<TRASFERIMENTO> TRASFERIMENTO { get; set; }
        public virtual DbSet<UFFICI> UFFICI { get; set; }
        public virtual DbSet<UTENTIAUTORIZZATI> UTENTIAUTORIZZATI { get; set; }
        public virtual DbSet<VALUTAUFFICIO> VALUTAUFFICIO { get; set; }
        public virtual DbSet<VALUTE> VALUTE { get; set; }
        public virtual DbSet<VARIAZIONIMAB> VARIAZIONIMAB { get; set; }
        public virtual DbSet<VIAGGICONGEDO> VIAGGICONGEDO { get; set; }
        public virtual DbSet<VOCI> VOCI { get; set; }
    }
}
