using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class MaggiorazioniFamiliariController : Controller
    {

        public ActionResult ElencoFamiliari(decimal idTrasferimento)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tr = dtt.GetSoloTrasferimentoById(idTrasferimento);

                    using (dtMaggiorazioniFigli dtmf = new dtMaggiorazioniFigli())
                    {
                        MaggiorazioniFigliModel mf = dtmf.GetMaggiorazioneFigli(tr.idTrasferimento, tr.dataPartenza);

                        using (dtFigli dtf = new dtFigli())
                        {
                            mf.LFigli = dtf.GetListaFigli(mf.idMaggiorazioneFigli);

                            using (dtDocumenti dtd = new dtDocumenti())
                            {
                                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                                {
                                    foreach (var item in mf.LFigli)
                                    {
                                        ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                        {
                                            id = item.idMaggiorazioneFigli,
                                            idTrasferimento = idTrasferimento,
                                            idFamiliare = item.idFigli,
                                            Nominativo = item.cognome + " " + item.nome,
                                            CodiceFiscale = item.codiceFiscale,
                                            dataInizio = item.MaggiorazioniFigli.dataInizioValidita,
                                            dataFine = item.MaggiorazioniFigli.dataFineValidita,
                                            parentela = EnumParentela.Figlio,
                                            idAltriDati = dtadf.GetAltriDatiFamiliariFiglio(item.idFigli).idAltriDatiFam,
                                            Documento = dtd.GetDocumentoByIdFiglio(idFiglio: item.idFigli),


                                        };

                                        lefm.Add(efm);
                                    }
                                }

                            }
                        }

                    }

                    using (dtMaggiorazioneConiuge dtmc = new dtMaggiorazioneConiuge())
                    {
                        MaggiorazioneConiugeModel mcm = dtmc.GetMaggiorazioneConiuge(tr.idTrasferimento, tr.dataPartenza);
                        using (dtConiuge dtc = new dtConiuge())
                        {
                            mcm.Coniuge = dtc.GetConiuge(mcm.idMaggiorazioneConiuge);
                            using (dtDocumenti dtd = new dtDocumenti())
                            {
                                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                                {
                                    ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                    {
                                        id = mcm.idMaggiorazioneConiuge,
                                        idTrasferimento = idTrasferimento,
                                        idFamiliare = mcm.idMaggiorazioneConiuge,
                                        Nominativo = mcm.Coniuge.cognome + " " + mcm.Coniuge.nome,
                                        CodiceFiscale = mcm.Coniuge.codiceFiscale,
                                        dataInizio = mcm.dataInizioValidita,
                                        dataFine = mcm.dataFineValidita,
                                        parentela = EnumParentela.Coniuge,
                                        idAltriDati = dtadf.GetAltriDatiFamiliariConiuge(mcm.idMaggiorazioneConiuge).idAltriDatiFam,
                                        Documento = dtd.GetDocumentoByIdMagConiuge(idMagConiuge: mcm.idMaggiorazioneConiuge),


                                    };

                                    lefm.Add(efm);
                                }

                            }

                        }

                    }



                }
                return PartialView(lefm);

            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }

        }

        [HttpPost]
        public ActionResult NuovoFamiliare(decimal idTrasferimento)
        {
            return PartialView();
        }




    }
}