using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;

namespace NewISE.Models.DBModel.dtObj
{
    public enum EnumTipoTrasferimento
    {
        SedeEstero = 1,
        EsteroEstero = 2,
        EsteroEsteroStessaRegiona = 3
    }
    public class dtIndennitaSistemazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPrimaSistemazione"></param>
        /// <param name="tipoTrasf"></param>
        /// <param name="dt"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public IndennitaSistemazioneModel GetIndennitaSistemazione(Decimal idPrimaSistemazione, EnumTipoTrasferimento tipoTrasf, DateTime dt, ModelDBISE db)
        {

            IndennitaSistemazioneModel ism = new IndennitaSistemazioneModel();

            var ps = db.PRIMASITEMAZIONE.Find(idPrimaSistemazione);

            if (ps != null && ps.IDPRIMASISTEMAZIONE > 0)
            {
                var lIndSist =
                    ps.INDENNITASISTEMAZIONE.Where(
                        a =>
                            a.ANNULLATO == false && a.IDTIPOTRASFERIMENTO == (decimal)tipoTrasf &&
                            dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                        .OrderByDescending(a => a.DATAFINEVALIDITA);

                if (lIndSist?.Any() ?? false)
                {
                    var indSist = lIndSist.First();
                    ism = new IndennitaSistemazioneModel()
                    {
                        idIndSist = indSist.IDINDSIST,
                        idTipoTrasferimento = indSist.IDTIPOTRASFERIMENTO,
                        idRiduzioni = indSist.IDRIDUZIONI,
                        dataInizioValidita = indSist.DATAINIZIOVALIDITA,
                        dataFineValidita = indSist.DATAFINEVALIDITA,
                        coefficiente = indSist.COEFFICIENTE,
                        dataAggiornamento = indSist.DATAAGGIORNAMENTO,
                        annullato = indSist.ANNULLATO
                    };
                }
            }


            return ism;
        }

    }
}