namespace NewISE.POCO
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelDBISE : DbContext
    {
        public ModelDBISE()
            : base("name=ModelDBISE")
        {
        }

        public virtual DbSet<ACCESSI> ACCESSI { get; set; }
        public virtual DbSet<ALIQUOTECONTRIBUTIVE> ALIQUOTECONTRIBUTIVE { get; set; }
        public virtual DbSet<ALTRIDATIFAM> ALTRIDATIFAM { get; set; }
        public virtual DbSet<ANTICIPI> ANTICIPI { get; set; }
        public virtual DbSet<ATTIVITACRUD> ATTIVITACRUD { get; set; }
        public virtual DbSet<BIGLIETTI> BIGLIETTI { get; set; }
        public virtual DbSet<BIGLIETTI_DOCUMENTI> BIGLIETTI_DOCUMENTI { get; set; }
        public virtual DbSet<CDCGEPE> CDCGEPE { get; set; }
        public virtual DbSet<COEFFICIENTEFKM> COEFFICIENTEFKM { get; set; }
        public virtual DbSet<COEFFICIENTEINDRICHIAMO> COEFFICIENTEINDRICHIAMO { get; set; }
        public virtual DbSet<COEFFICIENTESEDE> COEFFICIENTESEDE { get; set; }
        public virtual DbSet<CONIUGE> CONIUGE { get; set; }
        public virtual DbSet<CONTABILITA> CONTABILITA { get; set; }
        public virtual DbSet<DEFFASCIACHILOMETRICA> DEFFASCIACHILOMETRICA { get; set; }
        public virtual DbSet<DIPENDENTI> DIPENDENTI { get; set; }
        public virtual DbSet<DOCUMENTI> DOCUMENTI { get; set; }
        public virtual DbSet<ELAB_CONT> ELAB_CONT { get; set; }
        public virtual DbSet<FIGLI> FIGLI { get; set; }
        public virtual DbSet<GRUPPIDOCUMENTI> GRUPPIDOCUMENTI { get; set; }
        public virtual DbSet<INDENNITA> INDENNITA { get; set; }
        public virtual DbSet<INDENNITABASE> INDENNITABASE { get; set; }
        public virtual DbSet<INDENNITAPRIMOSEGRETARIO> INDENNITAPRIMOSEGRETARIO { get; set; }
        public virtual DbSet<INDENNITASISTEMAZIONE> INDENNITASISTEMAZIONE { get; set; }
        public virtual DbSet<LIVELLI> LIVELLI { get; set; }
        public virtual DbSet<LIVELLIDIPENDENTI> LIVELLIDIPENDENTI { get; set; }
        public virtual DbSet<LOGATTIVITA> LOGATTIVITA { get; set; }
        public virtual DbSet<MAB_ALIQCONTR> MAB_ALIQCONTR { get; set; }
        public virtual DbSet<MAB_DOC> MAB_DOC { get; set; }
        public virtual DbSet<MAGFAM_DOC> MAGFAM_DOC { get; set; }
        public virtual DbSet<MAGGIORAZIONEABITAZIONE> MAGGIORAZIONEABITAZIONE { get; set; }
        public virtual DbSet<MAGGIORAZIONECONIUGE> MAGGIORAZIONECONIUGE { get; set; }
        public virtual DbSet<MAGGIORAZIONEFIGLI> MAGGIORAZIONEFIGLI { get; set; }
        public virtual DbSet<MAGGIORAZIONIANNUALI> MAGGIORAZIONIANNUALI { get; set; }
        public virtual DbSet<NORMACALCOLO> NORMACALCOLO { get; set; }
        public virtual DbSet<NOTIFICARICHIESTAMAGFAM> NOTIFICARICHIESTAMAGFAM { get; set; }
        public virtual DbSet<OA> OA { get; set; }
        public virtual DbSet<PASSAPORTI> PASSAPORTI { get; set; }
        public virtual DbSet<PENSIONECONIUGE> PENSIONECONIUGE { get; set; }
        public virtual DbSet<PERCENTUALEDISAGIO> PERCENTUALEDISAGIO { get; set; }
        public virtual DbSet<PERCENTUALEMAB> PERCENTUALEMAB { get; set; }
        public virtual DbSet<PERCENTUALEMAGCONIUGE> PERCENTUALEMAGCONIUGE { get; set; }
        public virtual DbSet<PERCENTUALEMAGFIGLI> PERCENTUALEMAGFIGLI { get; set; }
        public virtual DbSet<PRIMASITEMAZIONE> PRIMASITEMAZIONE { get; set; }
        public virtual DbSet<REGOLECALCOLO> REGOLECALCOLO { get; set; }
        public virtual DbSet<RICHIAMO> RICHIAMO { get; set; }
        public virtual DbSet<RIDUZIONI> RIDUZIONI { get; set; }
        public virtual DbSet<RUOLOACCESSO> RUOLOACCESSO { get; set; }
        public virtual DbSet<RUOLODIPENDENTE> RUOLODIPENDENTE { get; set; }
        public virtual DbSet<RUOLOUFFICIO> RUOLOUFFICIO { get; set; }
        public virtual DbSet<SOSPENSIONE> SOSPENSIONE { get; set; }
        public virtual DbSet<STATOTRASFERIMENTO> STATOTRASFERIMENTO { get; set; }
        public virtual DbSet<STIPENDI> STIPENDI { get; set; }
        public virtual DbSet<TEORICI> TEORICI { get; set; }
        public virtual DbSet<TFR> TFR { get; set; }
        public virtual DbSet<TIPOALIQUOTECONTRIBUTIVE> TIPOALIQUOTECONTRIBUTIVE { get; set; }
        public virtual DbSet<TIPODOCUMENTI> TIPODOCUMENTI { get; set; }
        public virtual DbSet<TIPOELABORAZIONE> TIPOELABORAZIONE { get; set; }
        public virtual DbSet<TIPOLIQUIDAZIONE> TIPOLIQUIDAZIONE { get; set; }
        public virtual DbSet<TIPOLOGIACOAN> TIPOLOGIACOAN { get; set; }
        public virtual DbSet<TIPOLOGIACONIUGE> TIPOLOGIACONIUGE { get; set; }
        public virtual DbSet<TIPOLOGIAFIGLIO> TIPOLOGIAFIGLIO { get; set; }
        public virtual DbSet<TIPOMOVIMENTO> TIPOMOVIMENTO { get; set; }
        public virtual DbSet<TIPOREGOLACALCOLO> TIPOREGOLACALCOLO { get; set; }
        public virtual DbSet<TIPOSOSPENSIONE> TIPOSOSPENSIONE { get; set; }
        public virtual DbSet<TIPOTRASFERIMENTO> TIPOTRASFERIMENTO { get; set; }
        public virtual DbSet<TIPOVOCE> TIPOVOCE { get; set; }
        public virtual DbSet<TRASFERIMENTO> TRASFERIMENTO { get; set; }
        public virtual DbSet<TRASPEFFETTIRIEN_COEFFIFKM> TRASPEFFETTIRIEN_COEFFIFKM { get; set; }
        public virtual DbSet<TRASPEFFETTIRIENTRO_DOC> TRASPEFFETTIRIENTRO_DOC { get; set; }
        public virtual DbSet<TRASPEFFETTISIST_DOC> TRASPEFFETTISIST_DOC { get; set; }
        public virtual DbSet<TRASPORTOEFFETTIRIENTRO> TRASPORTOEFFETTIRIENTRO { get; set; }
        public virtual DbSet<TRASPORTOEFFETTISIST> TRASPORTOEFFETTISIST { get; set; }
        public virtual DbSet<UFFICI> UFFICI { get; set; }
        public virtual DbSet<UTENTIAUTORIZZATI> UTENTIAUTORIZZATI { get; set; }
        public virtual DbSet<VALUTE> VALUTE { get; set; }
        public virtual DbSet<VOCI> VOCI { get; set; }
        public virtual DbSet<RUOLODIPENDENTE_OLD> RUOLODIPENDENTE_OLD { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ACCESSI>()
                .Property(e => e.IDACCESSO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ACCESSI>()
                .Property(e => e.IDUTENTELOGGATO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ACCESSI>()
                .Property(e => e.GUID)
                .IsUnicode(false);

            modelBuilder.Entity<ALIQUOTECONTRIBUTIVE>()
                .Property(e => e.IDALIQCONTR)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ALIQUOTECONTRIBUTIVE>()
                .Property(e => e.IDTIPOCONTRIBUTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ALIQUOTECONTRIBUTIVE>()
                .Property(e => e.ALIQUOTA)
                .HasPrecision(18, 8);

            modelBuilder.Entity<ALIQUOTECONTRIBUTIVE>()
                .HasOptional(e => e.MAB_ALIQCONTR)
                .WithRequired(e => e.ALIQUOTECONTRIBUTIVE)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ALTRIDATIFAM>()
                .Property(e => e.IDALTRIDATIFAM)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ALTRIDATIFAM>()
                .Property(e => e.IDFIGLI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ALTRIDATIFAM>()
                .Property(e => e.IDCONIUGE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ALTRIDATIFAM>()
                .Property(e => e.COMUNENASCITA)
                .IsUnicode(false);

            modelBuilder.Entity<ALTRIDATIFAM>()
                .Property(e => e.PROVINCIANASCITA)
                .IsUnicode(false);

            modelBuilder.Entity<ALTRIDATIFAM>()
                .Property(e => e.NAZIONALITA)
                .IsUnicode(false);

            modelBuilder.Entity<ALTRIDATIFAM>()
                .Property(e => e.INDIRIZZORESIDENZA)
                .IsUnicode(false);

            modelBuilder.Entity<ALTRIDATIFAM>()
                .Property(e => e.COMUNERESIDENZA)
                .IsUnicode(false);

            modelBuilder.Entity<ALTRIDATIFAM>()
                .Property(e => e.PROVINCIARESIDENZA)
                .IsUnicode(false);

            modelBuilder.Entity<ALTRIDATIFAM>()
                .Property(e => e.CAP)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<ALTRIDATIFAM>()
                .Property(e => e.STUDENTE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ANTICIPI>()
                .Property(e => e.IDANTICIPO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ANTICIPI>()
                .Property(e => e.IDPRIMASISTEMAZIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ANTICIPI>()
                .Property(e => e.IMPORTOANTICIPO)
                .HasPrecision(18, 8);

            modelBuilder.Entity<ANTICIPI>()
                .Property(e => e.NOTIFICARICHIESTA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ANTICIPI>()
                .Property(e => e.PRATICACONCLUSA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ANTICIPI>()
                .HasMany(e => e.ELAB_CONT)
                .WithOptional(e => e.ANTICIPI)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ATTIVITACRUD>()
                .Property(e => e.IDATTIVITACRUD)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ATTIVITACRUD>()
                .Property(e => e.DESCRIZIONEATTIVITA)
                .IsUnicode(false);

            modelBuilder.Entity<BIGLIETTI>()
                .Property(e => e.IDBIGLIETTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<BIGLIETTI>()
                .Property(e => e.NOTIFICARICHIESTA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<BIGLIETTI>()
                .Property(e => e.PERSONALE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<BIGLIETTI>()
                .Property(e => e.PRATICACONCLUSA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<BIGLIETTI_DOCUMENTI>()
                .Property(e => e.IDBIGLIETTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<BIGLIETTI_DOCUMENTI>()
                .Property(e => e.IDDOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<BIGLIETTI_DOCUMENTI>()
                .Property(e => e.IDTIPODOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<CDCGEPE>()
                .Property(e => e.IDDIPENDENTE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<CDCGEPE>()
                .Property(e => e.CODICECDC)
                .IsUnicode(false);

            modelBuilder.Entity<CDCGEPE>()
                .Property(e => e.DESCCDC)
                .IsUnicode(false);

            modelBuilder.Entity<COEFFICIENTEFKM>()
                .Property(e => e.IDCFKM)
                .HasPrecision(38, 0);

            modelBuilder.Entity<COEFFICIENTEFKM>()
                .Property(e => e.IDDEFKM)
                .HasPrecision(38, 0);

            modelBuilder.Entity<COEFFICIENTEFKM>()
                .Property(e => e.COEFFICIENTEKM)
                .HasPrecision(18, 8);

            modelBuilder.Entity<COEFFICIENTEFKM>()
                .HasMany(e => e.UFFICI)
                .WithMany(e => e.COEFFICIENTEFKM)
                .Map(m => m.ToTable("FASCIACHILOMETRICA", "ISEPRO").MapLeftKey("IDCFKM").MapRightKey("IDUFFICIO"));

            modelBuilder.Entity<COEFFICIENTEINDRICHIAMO>()
                .Property(e => e.IDCOEFINDRICHIAMO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<COEFFICIENTEINDRICHIAMO>()
                .Property(e => e.COEFFICIENTERICHIAMO)
                .HasPrecision(18, 8);

            modelBuilder.Entity<COEFFICIENTEINDRICHIAMO>()
                .Property(e => e.COEFFICIENTEINDBASE)
                .HasPrecision(10, 8);

            modelBuilder.Entity<COEFFICIENTESEDE>()
                .Property(e => e.IDCOEFFICIENTESEDE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<COEFFICIENTESEDE>()
                .Property(e => e.IDUFFICIO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<COEFFICIENTESEDE>()
                .Property(e => e.VALORECOEFFICIENTE)
                .HasPrecision(18, 8);

            modelBuilder.Entity<COEFFICIENTESEDE>()
                .HasMany(e => e.INDENNITA)
                .WithMany(e => e.COEFFICIENTESEDE)
                .Map(m => m.ToTable("IND_COEFFICENTESEDE", "ISEPRO").MapLeftKey("IDCOEFFICIENTESEDE").MapRightKey("IDTRASFINDENNITA"));

            modelBuilder.Entity<CONIUGE>()
                .Property(e => e.IDCONIUGE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<CONIUGE>()
                .Property(e => e.IDMAGGIORAZIONECONIUGE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<CONIUGE>()
                .Property(e => e.NOME)
                .IsUnicode(false);

            modelBuilder.Entity<CONIUGE>()
                .Property(e => e.COGNOME)
                .IsUnicode(false);

            modelBuilder.Entity<CONIUGE>()
                .Property(e => e.CODICEFISCALE)
                .IsUnicode(false);

            modelBuilder.Entity<CONIUGE>()
                .HasMany(e => e.ALTRIDATIFAM)
                .WithOptional(e => e.CONIUGE)
                .WillCascadeOnDelete();

            modelBuilder.Entity<CONTABILITA>()
                .Property(e => e.IDCONTABILITA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<CONTABILITA>()
                .Property(e => e.IDTEORICI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<CONTABILITA>()
                .Property(e => e.ANNOELABORAZIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<CONTABILITA>()
                .Property(e => e.MESEELABORAZIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<CONTABILITA>()
                .Property(e => e.ANNORIFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<CONTABILITA>()
                .Property(e => e.MESERIFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<CONTABILITA>()
                .Property(e => e.GIORNI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<CONTABILITA>()
                .Property(e => e.IMPORTO)
                .HasPrecision(18, 8);

            modelBuilder.Entity<CONTABILITA>()
                .Property(e => e.FLAGINVIOOA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<CONTABILITA>()
                .HasOptional(e => e.OA)
                .WithRequired(e => e.CONTABILITA)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DEFFASCIACHILOMETRICA>()
                .Property(e => e.IDDEFKM)
                .HasPrecision(38, 0);

            modelBuilder.Entity<DEFFASCIACHILOMETRICA>()
                .Property(e => e.KM)
                .IsUnicode(false);

            modelBuilder.Entity<DIPENDENTI>()
                .Property(e => e.IDDIPENDENTE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<DIPENDENTI>()
                .Property(e => e.NOME)
                .IsUnicode(false);

            modelBuilder.Entity<DIPENDENTI>()
                .Property(e => e.COGNOME)
                .IsUnicode(false);

            modelBuilder.Entity<DIPENDENTI>()
                .Property(e => e.INDIRIZZO)
                .IsUnicode(false);

            modelBuilder.Entity<DIPENDENTI>()
                .Property(e => e.CAP)
                .IsUnicode(false);

            modelBuilder.Entity<DIPENDENTI>()
                .Property(e => e.CITTA)
                .IsUnicode(false);

            modelBuilder.Entity<DIPENDENTI>()
                .Property(e => e.PROVINCIA)
                .IsUnicode(false);

            modelBuilder.Entity<DIPENDENTI>()
                .Property(e => e.EMAIL)
                .IsUnicode(false);

            modelBuilder.Entity<DIPENDENTI>()
                .Property(e => e.TELEFONO)
                .IsUnicode(false);

            modelBuilder.Entity<DIPENDENTI>()
                .Property(e => e.FAX)
                .IsUnicode(false);

            modelBuilder.Entity<DIPENDENTI>()
                .HasOptional(e => e.CDCGEPE)
                .WithRequired(e => e.DIPENDENTI)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DOCUMENTI>()
                .Property(e => e.IDDOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<DOCUMENTI>()
                .Property(e => e.NOMEDOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<DOCUMENTI>()
                .Property(e => e.ESTENSIONE)
                .IsUnicode(false);

            modelBuilder.Entity<DOCUMENTI>()
                .HasOptional(e => e.MAGFAM_DOC)
                .WithRequired(e => e.DOCUMENTI)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DOCUMENTI>()
                .HasMany(e => e.NORMACALCOLO)
                .WithMany(e => e.DOCUMENTI)
                .Map(m => m.ToTable("DOCUMENTO_NORMACALCOLO", "ISEPRO").MapLeftKey("IDDOCUMENTO").MapRightKey("IDNORMACALCOLO"));

            modelBuilder.Entity<DOCUMENTI>()
                .HasMany(e => e.PASSAPORTI)
                .WithMany(e => e.DOCUMENTI)
                .Map(m => m.ToTable("PASSAPORTI_DOCUMENTI", "ISEPRO").MapLeftKey("IDDOCUMENTO").MapRightKey("IDPASSAPORTO"));

            modelBuilder.Entity<DOCUMENTI>()
                .HasMany(e => e.TRASFERIMENTO)
                .WithMany(e => e.DOCUMENTI)
                .Map(m => m.ToTable("TRASF_DOC", "ISEPRO").MapLeftKey("IDDOCUMENTO").MapRightKey("IDTRASFERIMENTO"));

            modelBuilder.Entity<ELAB_CONT>()
                .Property(e => e.IDELABCONT)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ELAB_CONT>()
                .Property(e => e.IDINDENNITA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ELAB_CONT>()
                .Property(e => e.IDANTICIPO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ELAB_CONT>()
                .Property(e => e.IDRICHIAMO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<ELAB_CONT>()
                .Property(e => e.IDPRIMASISTEMAZIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<FIGLI>()
                .Property(e => e.IDFIGLI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<FIGLI>()
                .Property(e => e.IDMAGGIORAZIONEFIGLI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<FIGLI>()
                .Property(e => e.NOME)
                .IsUnicode(false);

            modelBuilder.Entity<FIGLI>()
                .Property(e => e.COGNOME)
                .IsUnicode(false);

            modelBuilder.Entity<FIGLI>()
                .Property(e => e.CODICEFISCALE)
                .IsUnicode(false);

            modelBuilder.Entity<FIGLI>()
                .HasMany(e => e.ALTRIDATIFAM)
                .WithOptional(e => e.FIGLI)
                .WillCascadeOnDelete();

            modelBuilder.Entity<GRUPPIDOCUMENTI>()
                .Property(e => e.IDGRUPPODOC)
                .HasPrecision(38, 0);

            modelBuilder.Entity<GRUPPIDOCUMENTI>()
                .Property(e => e.DESCGRUPPO)
                .IsUnicode(false);

            modelBuilder.Entity<INDENNITA>()
                .Property(e => e.IDTRASFINDENNITA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<INDENNITA>()
                .HasMany(e => e.INDENNITABASE)
                .WithMany(e => e.INDENNITA)
                .Map(m => m.ToTable("INDENNITA_INDENNITABASE", "ISEPRO").MapLeftKey("IDTRASFINDENNITA").MapRightKey("IDINDENNITABASE"));

            modelBuilder.Entity<INDENNITA>()
                .HasMany(e => e.LIVELLIDIPENDENTI)
                .WithMany(e => e.INDENNITA)
                .Map(m => m.ToTable("INDENNITA_LIVELLIDIPENDENTI", "ISEPRO").MapLeftKey("IDTRASFINDENNITA").MapRightKey("IDLIVDIPENDENTE"));

            modelBuilder.Entity<INDENNITA>()
                .HasMany(e => e.MAGGIORAZIONECONIUGE)
                .WithMany(e => e.INDENNITA)
                .Map(m => m.ToTable("INDENNITA_MAGCONIUGE", "ISEPRO").MapLeftKey("IDTRASFINDENNITA").MapRightKey("IDMAGGIORAZIONECONIUGE"));

            modelBuilder.Entity<INDENNITA>()
                .HasMany(e => e.MAGGIORAZIONEFIGLI)
                .WithMany(e => e.INDENNITA)
                .Map(m => m.ToTable("INDENNITA_MAGFIGLI", "ISEPRO").MapLeftKey("IDTRASFINDENNITA").MapRightKey("IDMAGGIORAZIONEFIGLI"));

            modelBuilder.Entity<INDENNITA>()
                .HasMany(e => e.PERCENTUALEDISAGIO)
                .WithMany(e => e.INDENNITA)
                .Map(m => m.ToTable("INDENNITA_PERCDISAGIO", "ISEPRO").MapLeftKey("IDTRASFINDENNITA").MapRightKey("IDPERCENTUALEDISAGIO"));

            modelBuilder.Entity<INDENNITA>()
                .HasMany(e => e.RUOLODIPENDENTE)
                .WithMany(e => e.INDENNITA)
                .Map(m => m.ToTable("INDENNITA_RUOLODIPENDENTE", "ISEPRO").MapLeftKey("IDTRASFINDENNITA").MapRightKey("IDRUOLODIPENDENTE"));

            modelBuilder.Entity<INDENNITA>()
                .HasMany(e => e.TFR)
                .WithMany(e => e.INDENNITA)
                .Map(m => m.ToTable("INDENNITA_TFR", "ISEPRO").MapLeftKey("IDTRASFINDENNITA").MapRightKey("IDTFR"));

            modelBuilder.Entity<INDENNITABASE>()
                .Property(e => e.IDINDENNITABASE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<INDENNITABASE>()
                .Property(e => e.IDLIVELLO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<INDENNITABASE>()
                .Property(e => e.IDRIDUZIONI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<INDENNITABASE>()
                .Property(e => e.VALORE)
                .HasPrecision(18, 8);

            modelBuilder.Entity<INDENNITABASE>()
                .Property(e => e.VALORERESP)
                .HasPrecision(18, 8);

            modelBuilder.Entity<INDENNITAPRIMOSEGRETARIO>()
                .Property(e => e.IDINDPRIMOSEGR)
                .HasPrecision(38, 0);

            modelBuilder.Entity<INDENNITAPRIMOSEGRETARIO>()
                .Property(e => e.INDENNITA)
                .HasPrecision(18, 8);

            modelBuilder.Entity<INDENNITASISTEMAZIONE>()
                .Property(e => e.IDINDSIST)
                .HasPrecision(38, 0);

            modelBuilder.Entity<INDENNITASISTEMAZIONE>()
                .Property(e => e.IDTIPOTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<INDENNITASISTEMAZIONE>()
                .Property(e => e.COEFFICIENTE)
                .HasPrecision(18, 8);

            modelBuilder.Entity<LIVELLI>()
                .Property(e => e.IDLIVELLO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<LIVELLI>()
                .Property(e => e.LIVELLO)
                .IsUnicode(false);

            modelBuilder.Entity<LIVELLIDIPENDENTI>()
                .Property(e => e.IDLIVDIPENDENTE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<LIVELLIDIPENDENTI>()
                .Property(e => e.IDDIPENDENTE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<LIVELLIDIPENDENTI>()
                .Property(e => e.IDLIVELLO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<LOGATTIVITA>()
                .Property(e => e.IDLOG)
                .HasPrecision(38, 0);

            modelBuilder.Entity<LOGATTIVITA>()
                .Property(e => e.IDUTENTELOGGATO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<LOGATTIVITA>()
                .Property(e => e.IDTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<LOGATTIVITA>()
                .Property(e => e.IDATTIVITACRUD)
                .HasPrecision(38, 0);

            modelBuilder.Entity<LOGATTIVITA>()
                .Property(e => e.DESCATTIVITASVOLTA)
                .IsUnicode(false);

            modelBuilder.Entity<LOGATTIVITA>()
                .Property(e => e.TABELLACOINVOLTA)
                .IsUnicode(false);

            modelBuilder.Entity<LOGATTIVITA>()
                .Property(e => e.IDTABELLACOINVOLTA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAB_ALIQCONTR>()
                .Property(e => e.IDALIQCONTR)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAB_DOC>()
                .Property(e => e.IDMAB)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAB_DOC>()
                .Property(e => e.IDDOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAB_DOC>()
                .Property(e => e.IDTIPODOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGFAM_DOC>()
                .Property(e => e.IDDOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGFAM_DOC>()
                .Property(e => e.IDMAGGIORAZIONECONIUGE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGFAM_DOC>()
                .Property(e => e.IDMAGGIORAZIONEFIGLI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONEABITAZIONE>()
                .Property(e => e.IDMAB)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONEABITAZIONE>()
                .Property(e => e.IDPERCMAB)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONEABITAZIONE>()
                .Property(e => e.IDTFR)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONEABITAZIONE>()
                .Property(e => e.IDMAGANNUALI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONEABITAZIONE>()
                .Property(e => e.IDTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONEABITAZIONE>()
                .Property(e => e.CANONE)
                .HasPrecision(18, 8);

            modelBuilder.Entity<MAGGIORAZIONEABITAZIONE>()
                .Property(e => e.IDRUOLODIPENDENTE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONEABITAZIONE>()
                .Property(e => e.IDLIVDIPENDENTE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONECONIUGE>()
                .Property(e => e.IDMAGGIORAZIONECONIUGE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONECONIUGE>()
                .Property(e => e.IDTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONECONIUGE>()
                .Property(e => e.IDPERCMAGCONIUGE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONECONIUGE>()
                .Property(e => e.IDPENSIONECONIUGE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONECONIUGE>()
                .HasMany(e => e.MAGFAM_DOC)
                .WithOptional(e => e.MAGGIORAZIONECONIUGE)
                .WillCascadeOnDelete();

            modelBuilder.Entity<MAGGIORAZIONEFIGLI>()
                .Property(e => e.IDMAGGIORAZIONEFIGLI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONEFIGLI>()
                .Property(e => e.IDPERCMAGFIGLI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONEFIGLI>()
                .Property(e => e.IDTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONEFIGLI>()
                .Property(e => e.IDINDPRIMOSEGR)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONEFIGLI>()
                .HasMany(e => e.MAGFAM_DOC)
                .WithOptional(e => e.MAGGIORAZIONEFIGLI)
                .WillCascadeOnDelete();

            modelBuilder.Entity<MAGGIORAZIONIANNUALI>()
                .Property(e => e.IDMAGANNUALI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<MAGGIORAZIONIANNUALI>()
                .Property(e => e.IDUFFICIO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<NORMACALCOLO>()
                .Property(e => e.IDNORMACALCOLO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<NORMACALCOLO>()
                .Property(e => e.RIFERIMENTONORMATIVO)
                .IsUnicode(false);

            modelBuilder.Entity<NORMACALCOLO>()
                .HasMany(e => e.REGOLECALCOLO)
                .WithOptional(e => e.NORMACALCOLO)
                .WillCascadeOnDelete();

            modelBuilder.Entity<NOTIFICARICHIESTAMAGFAM>()
                .Property(e => e.IDNOTRICMAGFAM)
                .HasPrecision(38, 0);

            modelBuilder.Entity<NOTIFICARICHIESTAMAGFAM>()
                .Property(e => e.IDTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<OA>()
                .Property(e => e.CTB_ID_RECORD)
                .HasPrecision(38, 0);

            modelBuilder.Entity<OA>()
                .Property(e => e.CTB_QUALIFICA)
                .IsUnicode(false);

            modelBuilder.Entity<OA>()
                .Property(e => e.CTB_COD_SEDE)
                .IsUnicode(false);

            modelBuilder.Entity<OA>()
                .Property(e => e.CTB_TIPO_VOCE)
                .IsUnicode(false);

            modelBuilder.Entity<OA>()
                .Property(e => e.CTB_TIPO_MOVIMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<OA>()
                .Property(e => e.CTB_DESCRIZIONE)
                .IsUnicode(false);

            modelBuilder.Entity<OA>()
                .Property(e => e.CTB_COAN)
                .IsUnicode(false);

            modelBuilder.Entity<OA>()
                .Property(e => e.CTB_NUM_DOC)
                .IsUnicode(false);

            modelBuilder.Entity<OA>()
                .Property(e => e.CTB_IMPORTO)
                .HasPrecision(18, 8);

            modelBuilder.Entity<OA>()
                .Property(e => e.CTB_OPER_99)
                .IsUnicode(false);

            modelBuilder.Entity<PASSAPORTI>()
                .Property(e => e.IDPASSAPORTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PASSAPORTI>()
                .Property(e => e.NOTIFICARICHIESTA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PASSAPORTI>()
                .Property(e => e.PRATICACONCLUSA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PENSIONECONIUGE>()
                .Property(e => e.IDPENSIONECONIUGE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PENSIONECONIUGE>()
                .Property(e => e.IMPORTOPENSIONE)
                .HasPrecision(18, 8);

            modelBuilder.Entity<PENSIONECONIUGE>()
                .HasMany(e => e.MAGGIORAZIONECONIUGE)
                .WithOptional(e => e.PENSIONECONIUGE)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PERCENTUALEDISAGIO>()
                .Property(e => e.IDPERCENTUALEDISAGIO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PERCENTUALEDISAGIO>()
                .Property(e => e.IDUFFICIO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PERCENTUALEDISAGIO>()
                .Property(e => e.PERCENTUALE)
                .HasPrecision(11, 8);

            modelBuilder.Entity<PERCENTUALEMAB>()
                .Property(e => e.IDPERCMAB)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PERCENTUALEMAB>()
                .Property(e => e.IDUFFICIO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PERCENTUALEMAB>()
                .Property(e => e.IDLIVELLO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PERCENTUALEMAB>()
                .Property(e => e.PERCENTUALE)
                .HasPrecision(11, 8);

            modelBuilder.Entity<PERCENTUALEMAB>()
                .Property(e => e.PERCENTUALERESPONSABILE)
                .HasPrecision(18, 8);

            modelBuilder.Entity<PERCENTUALEMAGCONIUGE>()
                .Property(e => e.IDPERCMAGCONIUGE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PERCENTUALEMAGCONIUGE>()
                .Property(e => e.IDTIPOLOGIACONIUGE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PERCENTUALEMAGCONIUGE>()
                .Property(e => e.PERCENTUALECONIUGE)
                .HasPrecision(11, 8);

            modelBuilder.Entity<PERCENTUALEMAGFIGLI>()
                .Property(e => e.IDPERCMAGFIGLI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PERCENTUALEMAGFIGLI>()
                .Property(e => e.IDTIPOLOGIAFIGLIO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PERCENTUALEMAGFIGLI>()
                .Property(e => e.PERCENTUALEFIGLI)
                .HasPrecision(11, 8);

            modelBuilder.Entity<PRIMASITEMAZIONE>()
                .Property(e => e.IDPRIMASISTEMAZIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PRIMASITEMAZIONE>()
                .Property(e => e.IDTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PRIMASITEMAZIONE>()
                .Property(e => e.IDINDSIST)
                .HasPrecision(38, 0);

            modelBuilder.Entity<PRIMASITEMAZIONE>()
                .HasMany(e => e.ELAB_CONT)
                .WithOptional(e => e.PRIMASITEMAZIONE)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PRIMASITEMAZIONE>()
                .HasMany(e => e.TRASPORTOEFFETTISIST)
                .WithOptional(e => e.PRIMASITEMAZIONE)
                .WillCascadeOnDelete();

            modelBuilder.Entity<REGOLECALCOLO>()
                .Property(e => e.IDREGOLA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<REGOLECALCOLO>()
                .Property(e => e.IDTIPOREGOLACALCOLO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<REGOLECALCOLO>()
                .Property(e => e.IDNORMACALCOLO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<REGOLECALCOLO>()
                .Property(e => e.FORMULAREGOLACALCOLO)
                .IsUnicode(false);

            modelBuilder.Entity<RICHIAMO>()
                .Property(e => e.IDRICHIAMO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<RICHIAMO>()
                .Property(e => e.IDTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<RICHIAMO>()
                .Property(e => e.IDCOEFINDRICHIAMO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<RICHIAMO>()
                .HasMany(e => e.ELAB_CONT)
                .WithOptional(e => e.RICHIAMO)
                .WillCascadeOnDelete();

            modelBuilder.Entity<RICHIAMO>()
                .HasMany(e => e.TRASPORTOEFFETTIRIENTRO)
                .WithOptional(e => e.RICHIAMO)
                .WillCascadeOnDelete();

            modelBuilder.Entity<RIDUZIONI>()
                .Property(e => e.IDRIDUZIONI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<RIDUZIONI>()
                .Property(e => e.IDREGOLA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<RIDUZIONI>()
                .Property(e => e.PERCENTUALE)
                .HasPrecision(11, 8);

            modelBuilder.Entity<RUOLOACCESSO>()
                .Property(e => e.IDRUOLOACCESSO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<RUOLOACCESSO>()
                .Property(e => e.DESCRUOLO)
                .IsUnicode(false);

            modelBuilder.Entity<RUOLOACCESSO>()
                .HasMany(e => e.UTENTIAUTORIZZATI)
                .WithRequired(e => e.RUOLOACCESSO)
                .HasForeignKey(e => e.IDRUOLOUTENTE);

            modelBuilder.Entity<RUOLODIPENDENTE>()
                .Property(e => e.IDRUOLODIPENDENTE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<RUOLODIPENDENTE>()
                .Property(e => e.IDRUOLO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<RUOLOUFFICIO>()
                .Property(e => e.IDRUOLO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<RUOLOUFFICIO>()
                .Property(e => e.DESCRUOLO)
                .IsUnicode(false);

            modelBuilder.Entity<SOSPENSIONE>()
                .Property(e => e.IDSOSPENSIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<SOSPENSIONE>()
                .Property(e => e.IDTIPOSOSPENSIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<SOSPENSIONE>()
                .Property(e => e.IDINDENNITA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<SOSPENSIONE>()
                .Property(e => e.IDTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<STATOTRASFERIMENTO>()
                .Property(e => e.IDSTATOTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<STATOTRASFERIMENTO>()
                .Property(e => e.DESCRIZIONE)
                .IsUnicode(false);

            modelBuilder.Entity<STIPENDI>()
                .Property(e => e.IDELABORAZIONEMENSILE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<STIPENDI>()
                .Property(e => e.IDTEORICI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<STIPENDI>()
                .Property(e => e.MESEELABORAZIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<STIPENDI>()
                .Property(e => e.ANNOELABORAZIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<STIPENDI>()
                .Property(e => e.MESERIFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<STIPENDI>()
                .Property(e => e.ANNORIFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<STIPENDI>()
                .Property(e => e.GIORNI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<STIPENDI>()
                .Property(e => e.IMPORTO)
                .HasPrecision(18, 8);

            modelBuilder.Entity<STIPENDI>()
                .Property(e => e.BLOCCAINVIOFLIE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<STIPENDI>()
                .Property(e => e.DATAINVIOGEPE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TEORICI>()
                .Property(e => e.IDTEORICI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TEORICI>()
                .Property(e => e.IDELABCONT)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TEORICI>()
                .Property(e => e.IDVOCI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TEORICI>()
                .Property(e => e.IDTIPOMOVIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TEORICI>()
                .Property(e => e.IMPORTO)
                .HasPrecision(18, 8);

            modelBuilder.Entity<TEORICI>()
                .HasMany(e => e.STIPENDI)
                .WithRequired(e => e.TEORICI)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TFR>()
                .Property(e => e.IDTFR)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TFR>()
                .Property(e => e.IDVALUTA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TFR>()
                .Property(e => e.TASSOCAMBIO)
                .HasPrecision(18, 8);

            modelBuilder.Entity<TIPOALIQUOTECONTRIBUTIVE>()
                .Property(e => e.IDTIPOALIQCONTR)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPOALIQUOTECONTRIBUTIVE>()
                .Property(e => e.CODICE)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOALIQUOTECONTRIBUTIVE>()
                .Property(e => e.DESCRIZIONE)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOALIQUOTECONTRIBUTIVE>()
                .HasMany(e => e.ALIQUOTECONTRIBUTIVE)
                .WithRequired(e => e.TIPOALIQUOTECONTRIBUTIVE)
                .HasForeignKey(e => e.IDTIPOCONTRIBUTO);

            modelBuilder.Entity<TIPODOCUMENTI>()
                .Property(e => e.IDTIPODOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPODOCUMENTI>()
                .Property(e => e.IDGRUPPODOC)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPODOCUMENTI>()
                .Property(e => e.DESCRIZIONE)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOELABORAZIONE>()
                .Property(e => e.IDTIPOELABORAZIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPOELABORAZIONE>()
                .Property(e => e.DESCRIZIONE)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOELABORAZIONE>()
                .HasMany(e => e.TIPOVOCE)
                .WithRequired(e => e.TIPOELABORAZIONE)
                .HasForeignKey(e => e.IDTIPOELABOPRAZIONE);

            modelBuilder.Entity<TIPOLIQUIDAZIONE>()
                .Property(e => e.IDTIPOLIQUIDAZIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPOLIQUIDAZIONE>()
                .Property(e => e.DESCRIZIONE)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOLOGIACOAN>()
                .Property(e => e.IDTIPOCOAN)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPOLOGIACOAN>()
                .Property(e => e.DESCRIZIONE)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOLOGIACONIUGE>()
                .Property(e => e.IDTIPOLOGIACONIUGE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPOLOGIACONIUGE>()
                .Property(e => e.TIPOLOGIACONIUGE1)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOLOGIAFIGLIO>()
                .Property(e => e.IDTIPOLOGIAFIGLIO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPOLOGIAFIGLIO>()
                .Property(e => e.TIPOLOGIAFIGLIO1)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOMOVIMENTO>()
                .Property(e => e.IDTIPOMOVIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPOMOVIMENTO>()
                .Property(e => e.TIPOMOVIMENTO1)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOMOVIMENTO>()
                .Property(e => e.DESCMOVIMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOREGOLACALCOLO>()
                .Property(e => e.IDTIPOREGOLACALCOLO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPOREGOLACALCOLO>()
                .Property(e => e.DESCRIZIONEREGOLA)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOREGOLACALCOLO>()
                .HasMany(e => e.REGOLECALCOLO)
                .WithOptional(e => e.TIPOREGOLACALCOLO)
                .WillCascadeOnDelete();

            modelBuilder.Entity<TIPOSOSPENSIONE>()
                .Property(e => e.IDTIPOSOSPENSIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPOSOSPENSIONE>()
                .Property(e => e.DESCRIZIONE)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOTRASFERIMENTO>()
                .Property(e => e.IDTIPOTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPOTRASFERIMENTO>()
                .Property(e => e.TIPOTRASFERIMENTO1)
                .IsUnicode(false);

            modelBuilder.Entity<TIPOVOCE>()
                .Property(e => e.IDTIPOVOCE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPOVOCE>()
                .Property(e => e.IDTIPOELABOPRAZIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TIPOVOCE>()
                .Property(e => e.DESCRIZIONE)
                .IsUnicode(false);

            modelBuilder.Entity<TRASFERIMENTO>()
                .Property(e => e.IDTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASFERIMENTO>()
                .Property(e => e.IDTIPOTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASFERIMENTO>()
                .Property(e => e.IDUFFICIO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASFERIMENTO>()
                .Property(e => e.IDSTATOTRASFERIMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASFERIMENTO>()
                .Property(e => e.IDDIPENDENTE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASFERIMENTO>()
                .Property(e => e.IDTIPOCOAN)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASFERIMENTO>()
                .Property(e => e.COAN)
                .IsUnicode(false);

            modelBuilder.Entity<TRASFERIMENTO>()
                .Property(e => e.PROTOCOLLOLETTERA)
                .IsUnicode(false);

            modelBuilder.Entity<TRASFERIMENTO>()
                .HasOptional(e => e.INDENNITA)
                .WithRequired(e => e.TRASFERIMENTO)
                .WillCascadeOnDelete();

            modelBuilder.Entity<TRASFERIMENTO>()
                .HasMany(e => e.LOGATTIVITA)
                .WithOptional(e => e.TRASFERIMENTO)
                .WillCascadeOnDelete();

            modelBuilder.Entity<TRASPEFFETTIRIEN_COEFFIFKM>()
                .Property(e => e.IDTRASPORTOEFFETTISIST)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPEFFETTIRIEN_COEFFIFKM>()
                .Property(e => e.IDCFKM)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPEFFETTIRIENTRO_DOC>()
                .Property(e => e.IDTRASPORTOEFFETTISIST)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPEFFETTIRIENTRO_DOC>()
                .Property(e => e.IDDOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPEFFETTIRIENTRO_DOC>()
                .Property(e => e.IDTIPODOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPEFFETTISIST_DOC>()
                .Property(e => e.IDTRASPORTOEFFETTISIST)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPEFFETTISIST_DOC>()
                .Property(e => e.IDDOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPEFFETTISIST_DOC>()
                .Property(e => e.IDTIPODOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPORTOEFFETTIRIENTRO>()
                .Property(e => e.IDTRASPORTOEFFETTISIST)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPORTOEFFETTIRIENTRO>()
                .Property(e => e.IDRICHIAMO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPORTOEFFETTISIST>()
                .Property(e => e.IDTRASPORTOEFFETTISIST)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPORTOEFFETTISIST>()
                .Property(e => e.IDCFKM)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPORTOEFFETTISIST>()
                .Property(e => e.IDPRIMASISTEMAZIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<TRASPORTOEFFETTISIST>()
                .HasOptional(e => e.TRASPORTOEFFETTIRIENTRO)
                .WithRequired(e => e.TRASPORTOEFFETTISIST)
                .WillCascadeOnDelete();

            modelBuilder.Entity<UFFICI>()
                .Property(e => e.IDUFFICIO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<UFFICI>()
                .Property(e => e.CODICEUFFICIO)
                .IsUnicode(false);

            modelBuilder.Entity<UFFICI>()
                .Property(e => e.DESCRIZIONEUFFICIO)
                .IsUnicode(false);

            modelBuilder.Entity<UFFICI>()
                .Property(e => e.IDVALUTA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<UTENTIAUTORIZZATI>()
                .Property(e => e.IDUTENTEAUTORIZZATO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<UTENTIAUTORIZZATI>()
                .Property(e => e.IDRUOLOUTENTE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<UTENTIAUTORIZZATI>()
                .Property(e => e.UTENTE)
                .IsUnicode(false);

            modelBuilder.Entity<UTENTIAUTORIZZATI>()
                .HasMany(e => e.ACCESSI)
                .WithRequired(e => e.UTENTIAUTORIZZATI)
                .HasForeignKey(e => e.IDUTENTELOGGATO);

            modelBuilder.Entity<UTENTIAUTORIZZATI>()
                .HasMany(e => e.LOGATTIVITA)
                .WithRequired(e => e.UTENTIAUTORIZZATI)
                .HasForeignKey(e => e.IDUTENTELOGGATO);

            modelBuilder.Entity<VALUTE>()
                .Property(e => e.IDVALUTA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<VALUTE>()
                .Property(e => e.DESCRIZIONEVALUTA)
                .IsUnicode(false);

            modelBuilder.Entity<VOCI>()
                .Property(e => e.IDVOCI)
                .HasPrecision(38, 0);

            modelBuilder.Entity<VOCI>()
                .Property(e => e.IDTIPOLIQUIDAZIONE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<VOCI>()
                .Property(e => e.IDTIPOVOCE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<VOCI>()
                .Property(e => e.CODICEVOCE)
                .IsUnicode(false);

            modelBuilder.Entity<VOCI>()
                .Property(e => e.DESCRIZIONE)
                .IsUnicode(false);

            modelBuilder.Entity<RUOLODIPENDENTE_OLD>()
                .Property(e => e.IDRUOLODIPENDENTE)
                .HasPrecision(38, 0);

            modelBuilder.Entity<RUOLODIPENDENTE_OLD>()
                .Property(e => e.IDRUOLO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<RUOLODIPENDENTE_OLD>()
                .Property(e => e.IDTRASFERIMENTO)
                .HasPrecision(38, 0);
        }
    }
}
