//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE
{
    using System;
    using System.Collections.Generic;
    
    public partial class ALTRIDATIFAM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ALTRIDATIFAM()
        {
            this.MAGFAM_ALTRIDATIFAM = new HashSet<MAGFAM_ALTRIDATIFAM>();
        }
    
        public decimal IDALTRIDATIFAM { get; set; }
        public System.DateTime DATANASCITA { get; set; }
        public string COMUNENASCITA { get; set; }
        public string PROVINCIANASCITA { get; set; }
        public string NAZIONALITA { get; set; }
        public string INDIRIZZORESIDENZA { get; set; }
        public string COMUNERESIDENZA { get; set; }
        public string PROVINCIARESIDENZA { get; set; }
        public string CAP { get; set; }
        public decimal RESIDENTE { get; set; }
        public decimal ULTERIOREMAGCONIUGENONRES { get; set; }
        public decimal STUDENTE { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGFAM_ALTRIDATIFAM> MAGFAM_ALTRIDATIFAM { get; set; }
    }
}