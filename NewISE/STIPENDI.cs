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
    
    public partial class STIPENDI
    {
        public decimal IDELABORAZIONEMENSILE { get; set; }
        public decimal IDINDENNITA { get; set; }
        public decimal IDCONTABILITA { get; set; }
        public decimal IDVOCI { get; set; }
        public decimal MESEELABORAZIONE { get; set; }
        public decimal ANNOELABORAZIONE { get; set; }
        public decimal MESERIFERIMENTO { get; set; }
        public decimal ANNORIFERIMENTO { get; set; }
        public decimal GIORNI { get; set; }
        public decimal IMPORTO { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
        public decimal FLAGINVIOFLUSSI { get; set; }
        public Nullable<System.DateTime> DATAINVIOFLUSSI { get; set; }
        public decimal BLOCCAINVIOFLIE { get; set; }
    
        public virtual ELAB_CONT ELAB_CONT { get; set; }
        public virtual VOCI VOCI { get; set; }
    }
}