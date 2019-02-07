using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public enum EnumTipoTabella
    {
        AttivazioneMaggiorazioniFamiliari = 0,
        Coniuge = 1,
        Figli = 2,
        Pensione = 3,
        AltriDatiFamiliari = 4,
        Documenti = 5,
        PercentualeMaggiorazioneConiuge = 6,
        AttivazioniPassaporti = 7,
        PercentualiMaggiorazioneFigli = 8,
        IndennitaPrimoSegretario = 9,
        Trasferimento = 10,
        MaggiorazioniFamiliari = 11,
        RinunciaMaggiorazioniFamiliari = 12
    }
}