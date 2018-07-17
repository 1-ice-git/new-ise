using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;

using System.Linq;
using System.Web;
using NewISE.EF;

namespace NewISE.Models.DBModel.dtObj
{
    public class EvIndennita
    {
        private List<INDENNITABASE> evoluzioneIndennita;
        private List<COEFFICIENTESEDE> lCoefSede;

        public EvIndennita(List<INDENNITABASE> evoluzioneIndennita, List<COEFFICIENTESEDE> lCoefSede)
        {
            this.evoluzioneIndennita = evoluzioneIndennita;
            this.lCoefSede = lCoefSede;
        }

        public List<IndennitaBaseModel> EvoluzioneIndennita { get; set; }

        public List<CoefficientiSedeModel> EvoluzioneCoefficienteSede { get; set; }
    }
}