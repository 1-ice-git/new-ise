using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.Enumeratori;
using NewISE.Models.ViewModel;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtVoci : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Preleva tutte le voci gestite manualmente.
        /// </summary>
        /// <returns>Ritorna una lista di oggetti VociManualiModel.</returns>
        public IList<VociManualiModel> GetVociManuali()
        {
            List<VociManualiModel> lvmm = new List<VociManualiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lvm =
                    db.VOCI.Where(
                        a =>
                            a.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                            a.IDTIPOVOCE == (decimal)EnumTipoVoce.Manuale)
                        .OrderBy(a => a.CODICEVOCE)
                        .ThenBy(a => a.DESCRIZIONE)
                        .ToList();

                if (lvm?.Any() ?? false)
                {
                    lvmm = (from e in lvm
                            select new VociManualiModel()
                            {
                                idVoci = e.IDVOCI,
                                DescVoce = e.DESCRIZIONE + " (" + e.CODICEVOCE + ")"
                            }).ToList();
                }
            }

            return lvmm;
        }

    }
}