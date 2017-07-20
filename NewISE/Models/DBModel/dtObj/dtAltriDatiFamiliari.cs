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



        public void SetAltriDatiFamiliariConiuge(AltriDatiFamModel adfm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ALTRIDATIFAM adf = new ALTRIDATIFAM()
                {
                    IDALTRIDATIFAM = adfm.idAltriDatiFam,
                    IDMAGGIORAZIONECONIUGE = adfm.idMaggiorazioneConiuge,
                    DATANASCITA = adfm.dataNascita.Value,
                    CAPNASCITA = adfm.capNascita,
                    COMUNENASCITA = adfm.comuneNascita,
                    PROVINCIANASCITA = adfm.provinciaNascita,
                    NAZIONALITA = adfm.nazionalita,
                    INDIRIZZORESIDENZA = adfm.indirizzoResidenza,
                    CAPRESIDENZA = adfm.capResidenza,
                    COMUNERESIDENZA = adfm.comuneResidenza,
                    PROVINCIARESIDENZA = adfm.provinciaResidenza,
                    DATAAGGIORNAMENTO = adfm.dataAggiornamento,
                    ANNULLATO = adfm.annullato
                };

                db.CONIUGE.Find(adfm.idMaggiorazioneConiuge).ALTRIDATIFAM.Add(adf);

                db.SaveChanges();
            }
        }

        public void SetAltriDatiFamiliariFigli(AltriDatiFamModel adfm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ALTRIDATIFAM adf = new ALTRIDATIFAM()
                {
                    IDALTRIDATIFAM = adfm.idAltriDatiFam,
                    IDFIGLI = adfm.idFigli,
                    DATANASCITA = adfm.dataNascita.Value,
                    CAPNASCITA = adfm.capNascita,
                    COMUNENASCITA = adfm.comuneNascita,
                    PROVINCIANASCITA = adfm.provinciaNascita,
                    NAZIONALITA = adfm.nazionalita,
                    INDIRIZZORESIDENZA = adfm.indirizzoResidenza,
                    CAPRESIDENZA = adfm.capResidenza,
                    COMUNERESIDENZA = adfm.comuneResidenza,
                    PROVINCIARESIDENZA = adfm.provinciaResidenza,
                    DATAAGGIORNAMENTO = adfm.dataAggiornamento,
                    ANNULLATO = adfm.annullato
                };

                db.FIGLI.Find(adfm.idFigli).ALTRIDATIFAM.Add(adf);

                db.SaveChanges();
            }
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

                var ladf = db.CONIUGE.Find(idMagConiuge).ALTRIDATIFAM.Where(a => a.ANNULLATO == false && a.IDMAGGIORAZIONECONIUGE == idMagConiuge).ToList();

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
                        dataAggiornamento = item.DATAAGGIORNAMENTO,
                        annullato = item.ANNULLATO,

                    };

                }


            }

            return adfm;
        }

        public AltriDatiFamModel GetAltriDatiFamiliari(decimal idAltriDatiFam)
        {
            AltriDatiFamModel adfm = new AltriDatiFamModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var adf = db.ALTRIDATIFAM.Find(idAltriDatiFam);

                if (adf != null && adf.IDALTRIDATIFAM > 0)
                {
                    adfm = new AltriDatiFamModel()
                    {
                        idAltriDatiFam = adf.IDALTRIDATIFAM,
                        idFigli = adf.IDFIGLI,
                        idMaggiorazioneConiuge = adf.IDMAGGIORAZIONECONIUGE,
                        dataNascita = adf.DATANASCITA,
                        capNascita = adf.CAPNASCITA,
                        comuneNascita = adf.COMUNENASCITA,
                        provinciaNascita = adf.PROVINCIANASCITA,
                        nazionalita = adf.NAZIONALITA,
                        indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                        capResidenza = adf.CAPRESIDENZA,
                        comuneResidenza = adf.COMUNERESIDENZA,
                        provinciaResidenza = adf.PROVINCIARESIDENZA,
                        dataAggiornamento = adf.DATAAGGIORNAMENTO,
                        annullato = adf.ANNULLATO,

                    };

                }

            }

            return adfm;
        }
    }
}