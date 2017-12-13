using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{

    public enum EnumParentela
    {
        Coniuge = 1,
        Figlio = 2,
        Richiedente = 3,
    }

    public class MaggiorazioniFamiliariModel
    {
        [Key]
        public decimal idMaggiorazioniFamiliari { get; set; }

        public TrasferimentoModel Trasferimento { get; set; }

        public IList<ConiugeModel> ListaConiuge { get; set; }
        public IList<FigliModel> ListaFigli { get; set; }
        public bool HasValue()
        {
            return idMaggiorazioniFamiliari > 0 ? true : false;
        }
    }
}