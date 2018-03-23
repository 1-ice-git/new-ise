using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.dtObj.Interfacce;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.Enum;
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
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && a.DATAFINEVALIDITA >= tfr.DATAINIZIOVALIDITA &&
                            a.DATAINIZIOVALIDITA <= tfr.DATAFINEVALIDITA).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                foreach (var cmab in lCanoneMAB)
                {
                    var nCmab = tfr.CANONEMAB.Count(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato == false && a.IDCANONE == cmab.IDCANONE);
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
                    throw new Exception("Errore nella fase di associazione del canome MAB al TFR.");
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

                        var lr = cir.RICHIAMO;

                        foreach (var r in lr)
                        {
                            var t = r.TRASFERIMENTO;

                            Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, riduzioni.DATAINIZIOVALIDITA, db);
                        }

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

                var lc =
                    db.CONIUGE.Where(
                        a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                            a.ATTIVAZIONIMAGFAM.Where(
                                b =>
                                    b.ANNULLATO == false && b.RICHIESTAATTIVAZIONE == true &&
                                    b.ATTIVAZIONEMAGFAM == true).Any() && a.DATAFINEVALIDITA >= pmc.DATAINIZIOVALIDITA &&
                            a.DATAINIZIOVALIDITA <= pmc.DATAFINEVALIDITA).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                foreach (var c in lc)
                {
                    var nConta =
                        pmc.CONIUGE.Count(
                            a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && a.IDCONIUGE == c.IDCONIUGE);

                    if (nConta <= 0)
                    {
                        pmc.CONIUGE.Add(c);

                        var t = c.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

                        Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, pmc.DATAINIZIOVALIDITA, db);
                    }
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione dell'indennità al TFR.");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void AssociaFiglio_PMF(decimal idPercFiglio, ModelDBISE db)
        {
            try
            {
                var pmf = db.PERCENTUALEMAGFIGLI.Find(idPercFiglio);
                var item = db.Entry<PERCENTUALEMAGFIGLI>(pmf);
                item.State = EntityState.Modified;
                item.Collection(a => a.FIGLI).Load();

                var lf = db.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                             a.ATTIVAZIONIMAGFAM.Where(
                                                 b =>
                                                     b.ANNULLATO == false && b.RICHIESTAATTIVAZIONE == true &&
                                                     b.ATTIVAZIONEMAGFAM == true).Any() &&
                                             a.DATAFINEVALIDITA >= pmf.DATAINIZIOVALIDITA &&
                                             a.DATAINIZIOVALIDITA <= pmf.DATAFINEVALIDITA)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();

                foreach (var f in lf)
                {
                    var nConta =
                        pmf.FIGLI.Count(
                            a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && a.IDFIGLI == f.IDFIGLI);
                    if (nConta <= 0)
                    {
                        pmf.FIGLI.Add(f);
                        var t = f.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

                        Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, pmf.DATAINIZIOVALIDITA, db);
                    }
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione della percentuale figli alla tabella figli.");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaFigli_IPS(decimal idPrimoSegretario, ModelDBISE db)
        {
            try
            {
                var ips = db.INDENNITAPRIMOSEGRETARIO.Find(idPrimoSegretario);
                var item = db.Entry<INDENNITAPRIMOSEGRETARIO>(ips);
                item.State = EntityState.Modified;
                item.Collection(a => a.FIGLI).Load();

                var lf = db.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                             a.ATTIVAZIONIMAGFAM.Where(
                                                 b =>
                                                     b.ANNULLATO == false && b.RICHIESTAATTIVAZIONE == true &&
                                                     b.ATTIVAZIONEMAGFAM == true).Any() &&
                                             a.DATAFINEVALIDITA >= ips.DATAINIZIOVALIDITA &&
                                             a.DATAINIZIOVALIDITA <= ips.DATAFINEVALIDITA)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();

                foreach (var f in lf)
                {
                    var nConta =
                        ips.FIGLI.Count(
                            a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && a.IDFIGLI == f.IDFIGLI);

                    if (nConta <= 0)
                    {
                        ips.FIGLI.Add(f);
                        var t = f.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
                        Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, ips.DATAINIZIOVALIDITA, db);
                    }
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione dell'indennità di primo segretario alla tabella figli.");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaIndenita_PD(decimal idPercDisagio, ModelDBISE db)
        {
            try
            {
                var pd = db.PERCENTUALEDISAGIO.Find(idPercDisagio);
                var item = db.Entry<PERCENTUALEDISAGIO>(pd);
                item.State = EntityState.Modified;
                item.Collection(a => a.INDENNITA).Load();

                var lTrsferimento =
                    db.TRASFERIMENTO.Where(
                        a =>
                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                            a.DATARIENTRO >= pd.DATAINIZIOVALIDITA && a.DATAPARTENZA <= pd.DATAFINEVALIDITA)
                        .OrderBy(a => a.DATAPARTENZA)
                        .ToList();

                foreach (var t in lTrsferimento)
                {
                    var indennita = t.INDENNITA;

                    var nCont = pd.INDENNITA.Count(a => a.IDTRASFINDENNITA == t.INDENNITA.IDTRASFINDENNITA);
                    if (nCont <= 0)
                    {
                        pd.INDENNITA.Add(indennita);

                        Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, pd.DATAINIZIOVALIDITA, db);
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

        public void AssociaIndennitaBase_IB(decimal idIndBase, ModelDBISE db)
        {
            try
            {
                var ib = db.INDENNITABASE.Find(idIndBase);
                var item = db.Entry<INDENNITABASE>(ib);
                item.State = EntityState.Modified;
                item.Collection(a => a.INDENNITA).Load();

                var lTrsferimento =
                    db.TRASFERIMENTO.Where(
                        a =>
                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                            a.DATARIENTRO >= ib.DATAINIZIOVALIDITA && a.DATAPARTENZA <= ib.DATAFINEVALIDITA)
                        .OrderBy(a => a.DATAPARTENZA)
                        .ToList();

                foreach (var t in lTrsferimento)
                {
                    var indennita = t.INDENNITA;

                    var nCont = ib.INDENNITA.Count(a => a.IDTRASFINDENNITA == t.INDENNITA.IDTRASFINDENNITA);
                    if (nCont <= 0)
                    {
                        ib.INDENNITA.Add(indennita);

                        Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, ib.DATAINIZIOVALIDITA, db);
                    }
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione dell'indennità di base alla tabella Indennita.");
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
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

                        var t =
                            ib.INDENNITA.Where(
                                a => a.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato);

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

                        Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, tfr.DATAINIZIOVALIDITA, db);
                    }
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione dell'indennità al TFR.");
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