﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.dtObj.Interfacce;
using NewISE.Models.DBModel;
using NewISE.Models.Tools;

namespace NewISE.Models.dtObj
{

    public class dtRicalcoloParametri : IDisposable, IricalcoloParametri
    {
        public void AssociaCanoneMAB_TFR(decimal idTFR, ModelDBISE db)
        {
            try
            {
                var tfr = db.TFR.Find(idTFR);
                var item = db.Entry<TFR>(tfr);
                item.State = EntityState.Modified;
                item.Collection(a => a.CANONEMAB).Load();

                var lCanoneMAB =
                    db.CANONEMAB.Where(
                        a =>
                            a.ANNULLATO == false && a.DATAFINEVALIDITA >= tfr.DATAINIZIOVALIDITA &&
                            a.DATAINIZIOVALIDITA <= tfr.DATAFINEVALIDITA).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                foreach (var cmab in lCanoneMAB)
                {
                    var nCmab = tfr.CANONEMAB.Count(a => a.ANNULLATO == false && a.IDCANONE == cmab.IDCANONE);
                    if (nCmab <= 0)
                    {
                        tfr.CANONEMAB.Add(cmab);

                        var t = cmab.ATTIVAZIONEMAB.TRASFERIMENTO;

                        Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, tfr.DATAINIZIOVALIDITA, db);
                    }
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di asscoiazione del canome MAB al TFR.");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaCoefficienteRichiamo_Riduzioni(decimal idRiduzioni, ModelDBISE db)
        {
            try
            {
                var riduzioni = db.RIDUZIONI.Find(idRiduzioni);
                var item = db.Entry<RIDUZIONI>(riduzioni);
                item.State = EntityState.Modified;
                item.Collection(a => a.COEFFICIENTEINDRICHIAMO).Load();

                var lcir =
                    db.COEFFICIENTEINDRICHIAMO.Where(
                        a =>
                            a.ANNULLATO == false && a.DATAFINEVALIDITA >= riduzioni.DATAINIZIOVALIDITA &&
                            a.DATAINIZIOVALIDITA <= riduzioni.DATAFINEVALIDITA)
                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                        .ToList();

                foreach (var cir in lcir)
                {
                    var nConta =
                        riduzioni.COEFFICIENTEINDRICHIAMO.Count(
                            a => a.ANNULLATO == false && a.IDCOEFINDRICHIAMO == cir.IDCOEFINDRICHIAMO);

                    if (nConta <= 0)
                    {
                        riduzioni.COEFFICIENTEINDRICHIAMO.Add(cir);
                    }
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione del coefficente di richiamo alle riduzioni.");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaConiuge_PMC(decimal idPercMagConiuge, ModelDBISE db)
        {
            try
            {
                var pmc = db.PERCENTUALEMAGCONIUGE.Find(idPercMagConiuge);
                var item = db.Entry<PERCENTUALEMAGCONIUGE>(pmc);
                item.State = EntityState.Modified;
                item.Collection(a => a.CONIUGE).Load();

                //var lc = db.CONIUGE

            }
            catch (Exception ex)
            {

                throw ex;
            }




        }

        public void AssociaFiglio_PMF(decimal idPercFiglio, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaFigli_IPS(decimal idPrimoSegretario, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaIndenita_PD(decimal idPercDisagio, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaIndennitaBase_IB(decimal idIndBase, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaIndennitaBase_Riduzioni(decimal idRiduzioni, ModelDBISE db)
        {
            try
            {
                var r = db.RIDUZIONI.Find(idRiduzioni);
                var item = db.Entry<RIDUZIONI>(r);
                item.State = EntityState.Modified;
                item.Collection(a => a.INDENNITABASE).Load();

                var lib =
                    db.INDENNITABASE.Where(
                        a =>
                            a.ANNULLATO == false && a.DATAFINEVALIDITA >= r.DATAINIZIOVALIDITA &&
                            a.DATAINIZIOVALIDITA <= r.DATAFINEVALIDITA).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                foreach (INDENNITABASE ib in lib)
                {
                    var nConta =
                        r.INDENNITABASE.Count(a => a.ANNULLATO == false && a.IDINDENNITABASE == ib.IDINDENNITABASE);

                    if (nConta <= 0)
                    {
                        r.INDENNITABASE.Add(ib);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaIndennitaSistemazione_Riduzioni(decimal idRiduzioni, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaIndennita_CS(decimal idCoefficenteSede, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaIndennita_TFR(decimal idTFR, ModelDBISE db)
        {
            try
            {
                var tfr = db.TFR.Find(idTFR);
                var item = db.Entry<TFR>(tfr);
                item.State = EntityState.Modified;
                item.Collection(a => a.INDENNITA).Load();

                var lTrsferimento =
                    db.TRASFERIMENTO.Where(
                        a =>
                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                            a.DATARIENTRO >= tfr.DATAINIZIOVALIDITA && a.DATAPARTENZA <= tfr.DATAFINEVALIDITA)
                        .OrderBy(a => a.DATAPARTENZA)
                        .ToList();

                foreach (var t in lTrsferimento)
                {
                    var indennita = t.INDENNITA;

                    var nCont = tfr.INDENNITA.Count(a => a.IDTRASFINDENNITA == t.INDENNITA.IDTRASFINDENNITA);
                    if (nCont <= 0)
                    {
                        tfr.INDENNITA.Add(indennita);
                    }
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di asscoiazione dell'indennità al TFR.");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaMAB_PMAB(decimal idPerceMAB, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaMaggiorazioniAbitazione_MA(decimal idMagAnnuali, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaPagatoCondivisoMAB(decimal idPercentualeCondivisione, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaPercentualeAnticipoTEP(decimal idPercentualeAnticipoTEP, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaPercentualeAnticipoTER(decimal idPercentualeAnticipoTEP, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaPrimaSistemazione_IS(decimal idIndSistemazione, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaPrimaSistemazione_PKM(decimal idPercKM, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaRichiamo_CR(decimal idCoeffRichiamo, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaRichiamo_PKM(decimal idPercKM, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaRiduzioniIB(decimal idIndBase, ModelDBISE db)
        {

        }

        public void AssociaRiduzioni_CR(decimal idCoeffRichiamo, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaRiduzioni_IS(decimal idIndSistemazione, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


    }
}