using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtElaborazioni : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void InviaAnticipoPrimaSistemazione(decimal idAttivitaAnticipi, ModelDBISE db)
        {
            try
            {
                var aa = db.ATTIVITAANTICIPI.Find(idAttivitaAnticipi);
                var ps = aa.PRIMASITEMAZIONE;
                var t = ps.TRASFERIMENTO;
                var ra = aa.RINUNCIAANTICIPI;

                if (ra.RINUNCIAANT == false)
                {
                    var lanticipi =
                        aa.ANTICIPI.Where(
                            a =>
                                a.ANNULLATO == false &&
                                a.IDTIPOLOGIAANTICIPI == (decimal)EnumTipoAnticipi.Prima_sistemazione)
                            .ToList();

                    if (lanticipi?.Any() ?? false)
                    {
                        var anticipi = lanticipi.First();
                        using (CalcoliIndennita ci = new CalcoliIndennita(t.IDTRASFERIMENTO, t.DATAPARTENZA))
                        {
                            //decimal importoAnticipo = ci.AnticipoPrimaSistemazione(anticipi.PERCENTUALEANTICIPO);
                            ELABINDSISTEMAZIONE eis = new ELABINDSISTEMAZIONE()
                            {
                                IDPRIMASISTEMAZIONE = ps.IDPRIMASISTEMAZIONE,
                                INDENNITABASE = ci.IndennitaDiBase,
                                COEFFICENTESEDE = ci.CoefficienteDiSede,
                                PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                                COEFFICENTEINDSIST = ci.CoefficienteIndennitaSistemazione,
                                PERCENTUALERIDUZIONE = ci.PercentualeRiduzionePrimaSistemazione,
                                PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                                PENSIONECONIUGE = ci.PensioneConiuge,
                                INDENNITAPRIMOSEGRETARIO = ci.IndennitaPrimoSegretario,
                                PERCENTUALEMAGFIGLIO = ci.PercentualeMaggiorazioneFigli,
                                ANTICIPO = true,
                                SALDO = false,
                                UNICASOLUZIONE = false,
                                PERCANTSALDOUNISOL = anticipi.PERCENTUALEANTICIPO,
                                DATAOPERAZIONE = DateTime.Now,
                                ELABORATO = false,
                                ANNULLATO = false
                            };


                            var leis =
                                db.ELABINDSISTEMAZIONE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDPRIMASISTEMAZIONE == ps.IDPRIMASISTEMAZIONE &&
                                        a.ANTICIPO == true)
                                    .OrderByDescending(a => a.IDINDSISTLORDA)
                                    .ToList();

                            if (leis?.Any() == false)
                            {
                                var eisOld = leis.First();
                                eis.FK_IDINDSISTLORDA = eisOld.IDINDSISTLORDA;

                            }

                            db.ELABINDSISTEMAZIONE.Add(eis);

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore nella fase d'inderimento dell'anticipo di prima sistemazione.");
                            }

                            var importoAnticipo = CalcoliIndennita.ElaboraAnticipoPrimaSistemazione(eis.INDENNITABASE,
                                eis.COEFFICENTESEDE, eis.PERCENTUALEDISAGIO, eis.PERCENTUALERIDUZIONE,
                                eis.COEFFICENTEINDSIST, eis.PERCANTSALDOUNISOL);

                            TEORICI teorici = new TEORICI()
                            {

                            };


                        }
                    }


                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void InserisciTeoriciPrimaSistemazione(ELABINDSISTEMAZIONE eis, ModelDBISE db)
        {

        }





    }
}