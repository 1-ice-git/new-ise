using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtAltriDatiFamiliari : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public AltriDatiFamModel GetAltriDatiFamiliariFiglio(decimal idFiglio)
        {
            AltriDatiFamModel adfm = new AltriDatiFamModel();

            using (ModelDBISE db = new ModelDBISE())
            {

                var ladf = db.ALTRIDATIFAM.Where(a => a.ANNULLATO == false && a.IDFIGLI == idFiglio).ToList();

                if (ladf != null && ladf.Count > 0)
                {

                    var item = ladf.First();

                    adfm = new AltriDatiFamModel()
                    {
                        idAltriDatiFam = item.IDALTRIDATIFAM,
                        idFigli = item.IDFIGLI,
                        idMaggiorazioneConiuge = item.IDMAGGIORAZIONECONIUGE,
                        dataNascita = item.DATANASCITA,
                        capNascita = item.CAPNASCITA,
                        comuneNascita = item.COMUNENASCITA,
                        provinciaNascita = item.PROVINCIANASCITA,
                        nazionalita = item.NAZIONALITA,
                        indirizzoResidenza = item.INDIRIZZORESIDENZA,
                        capResidenza = item.CAPRESIDENZA,
                        comuneResidenza = item.COMUNERESIDENZA,
                        provinciaResidenza = item.PROVINCIARESIDENZA,
                        residente = item.RESIDENTE,
                        studente = item.STUDENTE,
                        ulterioreMagConiuge = item.ULTERIOREMAGCONIUGE,
                        dataAggiornamento = item.DATAAGGIORNAMENTO,
                        annullato = item.ANNULLATO,

                    };


                }


            }

            return adfm;
        }

        public AltriDatiFamModel GetAltriDatiFamiliariConiuge(decimal idMagConiuge)
        {
            AltriDatiFamModel adfm = new AltriDatiFamModel();

            using (ModelDBISE db = new ModelDBISE())
            {

                var ladf = db.ALTRIDATIFAM.Where(a => a.ANNULLATO == false && a.IDMAGGIORAZIONECONIUGE == idMagConiuge).ToList();

                if (ladf != null && ladf.Count > 0)
                {

                    var item = ladf.First();

                    adfm = new AltriDatiFamModel()
                    {
                        idAltriDatiFam = item.IDALTRIDATIFAM,
                        idFigli = item.IDFIGLI,
                        idMaggiorazioneConiuge = item.IDMAGGIORAZIONECONIUGE,
                        dataNascita = item.DATANASCITA,
                        capNascita = item.CAPNASCITA,
                        comuneNascita = item.COMUNENASCITA,
                        provinciaNascita = item.PROVINCIANASCITA,
                        nazionalita = item.NAZIONALITA,
                        indirizzoResidenza = item.INDIRIZZORESIDENZA,
                        capResidenza = item.CAPRESIDENZA,
                        comuneResidenza = item.COMUNERESIDENZA,
                        provinciaResidenza = item.PROVINCIARESIDENZA,
                        residente = item.RESIDENTE,
                        studente = item.STUDENTE,
                        ulterioreMagConiuge = item.ULTERIOREMAGCONIUGE,
                        dataAggiornamento = item.DATAAGGIORNAMENTO,
                        annullato = item.ANNULLATO,

                    };


                }


            }

            return adfm;
        }
    }
}