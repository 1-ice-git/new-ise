using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.dtObj.Interfacce;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.Enum;
using NewISE.Models.Tools;
using System.Linq.Dynamic;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.dtObj
{

    public class DtRicalcoloParametri : IDisposable, IricalcoloParametri
    {
        public void AssociaCanoneMAB_TFR(decimal idTFR, ModelDBISE db)
        {
            try
            {
                var tfr = db.TFR.Find(idTFR);
                var item = db.Entry<TFR>(tfr);


                var lCanoneMAB =
                    db.CANONEMAB.Where(
                        a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                            a.DATAFINEVALIDITA >= tfr.DATAINIZIOVALIDITA &&
                            a.DATAINIZIOVALIDITA <= tfr.DATAFINEVALIDITA && a.IDVALUTA == tfr.IDVALUTA)
                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                        .ToList();

                if (lCanoneMAB?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.CANONEMAB).Load();

                    foreach (var cmab in lCanoneMAB)
                    {
                        var nCmab = tfr.CANONEMAB.Count(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && a.IDCANONE == cmab.IDCANONE);
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


                if (riduzioni.IDFUNZIONERIDUZIONE == (decimal)EnumFunzioniRiduzione.Coefficente_Richiamo)
                {
                    var item = db.Entry<RIDUZIONI>(riduzioni);


                    var lcir =
                        db.COEFFICIENTEINDRICHIAMO.Where(
                            a =>
                                a.ANNULLATO == false && a.DATAFINEVALIDITA >= riduzioni.DATAINIZIOVALIDITA &&
                                a.DATAINIZIOVALIDITA <= riduzioni.DATAFINEVALIDITA)
                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                    if (lcir?.Any() ?? false)
                    {
                        item.State = EntityState.Modified;
                        item.Collection(a => a.COEFFICIENTEINDRICHIAMO).Load();

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


                var lc =
                    db.CONIUGE.Where(
                        a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                            a.ATTIVAZIONIMAGFAM.Where(
                                b =>
                                    b.ANNULLATO == false && b.RICHIESTAATTIVAZIONE == true &&
                                    b.ATTIVAZIONEMAGFAM == true).Any() && a.DATAFINEVALIDITA >= pmc.DATAINIZIOVALIDITA &&
                            a.DATAINIZIOVALIDITA <= pmc.DATAFINEVALIDITA).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                if (lc?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.CONIUGE).Load();

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


                var lf = db.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                             a.ATTIVAZIONIMAGFAM.Where(
                                                 b =>
                                                     b.ANNULLATO == false && b.RICHIESTAATTIVAZIONE == true &&
                                                     b.ATTIVAZIONEMAGFAM == true).Any() &&
                                             a.DATAFINEVALIDITA >= pmf.DATAINIZIOVALIDITA &&
                                             a.DATAINIZIOVALIDITA <= pmf.DATAFINEVALIDITA)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();

                if (lf?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.FIGLI).Load();

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


                var lf = db.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                             a.ATTIVAZIONIMAGFAM.Where(
                                                 b =>
                                                     b.ANNULLATO == false && b.RICHIESTAATTIVAZIONE == true &&
                                                     b.ATTIVAZIONEMAGFAM == true).Any() &&
                                             a.DATAFINEVALIDITA >= ips.DATAINIZIOVALIDITA &&
                                             a.DATAINIZIOVALIDITA <= ips.DATAFINEVALIDITA)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();

                if (lf?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.FIGLI).Load();

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


                var lTrsferimento =
                    db.TRASFERIMENTO.Where(
                        a =>
                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                            a.DATARIENTRO >= pd.DATAINIZIOVALIDITA && a.DATAPARTENZA <= pd.DATAFINEVALIDITA)
                        .OrderBy(a => a.DATAPARTENZA)
                        .ToList();

                if (lTrsferimento?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.INDENNITA).Load();

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


                var lTrsferimento =
                    db.TRASFERIMENTO.Where(
                        a =>
                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                            a.DATARIENTRO >= ib.DATAINIZIOVALIDITA && a.DATAPARTENZA <= ib.DATAFINEVALIDITA &&
                            a.DIPENDENTI.LIVELLIDIPENDENTI.Where(
                                b =>
                                    b.ANNULLATO == false && b.IDLIVELLO == ib.IDLIVELLO &&
                                    b.DATAFINEVALIDITA >= ib.DATAINIZIOVALIDITA &&
                                    b.DATAINIZIOVALIDITA <= ib.DATAFINEVALIDITA).Any())
                        .OrderBy(a => a.DATAPARTENZA)
                        .ToList();

                if (lTrsferimento?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.INDENNITA).Load();

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

                if (r.IDFUNZIONERIDUZIONE == (decimal)EnumFunzioniRiduzione.Indennita_Base)
                {
                    var item = db.Entry<RIDUZIONI>(r);


                    var lib =
                        db.INDENNITABASE.Where(
                            a =>
                                a.ANNULLATO == false && a.DATAFINEVALIDITA >= r.DATAINIZIOVALIDITA &&
                                a.DATAINIZIOVALIDITA <= r.DATAFINEVALIDITA).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();



                    if (lib?.Any() ?? false)
                    {
                        item.State = EntityState.Modified;
                        item.Collection(a => a.INDENNITABASE).Load();

                        foreach (INDENNITABASE ib in lib)
                        {
                            var nConta =
                                r.INDENNITABASE.Count(a => a.ANNULLATO == false && a.IDINDENNITABASE == ib.IDINDENNITABASE);

                            if (nConta <= 0)
                            {
                                r.INDENNITABASE.Add(ib);

                                var li =
                                    ib.INDENNITA.Where(
                                        a =>
                                            a.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                            a.TRASFERIMENTO.DATARIENTRO >= r.DATAINIZIOVALIDITA &&
                                            a.TRASFERIMENTO.DATAPARTENZA <= r.DATAFINEVALIDITA).ToList();

                                foreach (var i in li)
                                {
                                    var t = i.TRASFERIMENTO;

                                    Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, r.DATAINIZIOVALIDITA, db);

                                }

                            }
                        }


                        int j = db.SaveChanges();

                        if (j <= 0)
                        {
                            throw new Exception("Errore nella fase di associazione dell'indennità di base alla tabella Riduzioni.");
                        }
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
            try
            {
                var r = db.RIDUZIONI.Find(idRiduzioni);

                if (r.IDFUNZIONERIDUZIONE == (decimal)EnumFunzioniRiduzione.Indennita_Sistemazione)
                {
                    var item = db.Entry<RIDUZIONI>(r);

                    var lis =
                        db.INDENNITASISTEMAZIONE.Where(
                            a =>
                                a.ANNULLATO == false && a.DATAFINEVALIDITA >= r.DATAINIZIOVALIDITA &&
                                a.DATAINIZIOVALIDITA <= r.DATAFINEVALIDITA).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                    if (lis?.Any() ?? false)
                    {
                        item.State = EntityState.Modified;
                        item.Collection(a => a.INDENNITASISTEMAZIONE).Load();

                        foreach (INDENNITASISTEMAZIONE isist in lis)
                        {
                            var nConta =
                                r.INDENNITASISTEMAZIONE.Count(a => a.ANNULLATO == false && a.IDINDSIST == isist.IDINDSIST);

                            if (nConta <= 0)
                            {
                                r.INDENNITASISTEMAZIONE.Add(isist);

                                var li =
                                    isist.PRIMASITEMAZIONE.Where(
                                        a =>
                                            a.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                            a.TRASFERIMENTO.DATARIENTRO >= r.DATAINIZIOVALIDITA &&
                                            a.TRASFERIMENTO.DATAPARTENZA <= r.DATAFINEVALIDITA).ToList();

                                foreach (var i in li)
                                {
                                    var t = i.TRASFERIMENTO;

                                    Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, r.DATAINIZIOVALIDITA, db);

                                }

                            }
                        }

                        int j = db.SaveChanges();

                        if (j <= 0)
                        {
                            throw new Exception("Errore nella fase di associazione dell'indennità di sistemazione alla tabella Riduzioni.");
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaIndennita_CS(decimal idCoefficenteSede, ModelDBISE db)
        {
            try
            {
                var cs = db.COEFFICIENTESEDE.Find(idCoefficenteSede);
                var item = db.Entry<COEFFICIENTESEDE>(cs);


                var lTrsferimento =
                    db.TRASFERIMENTO.Where(
                        a =>
                            a.IDUFFICIO == cs.IDUFFICIO &&
                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                            a.DATARIENTRO >= cs.DATAINIZIOVALIDITA && a.DATAPARTENZA <= cs.DATAFINEVALIDITA)
                        .OrderBy(a => a.DATAPARTENZA)
                        .ToList();

                if (lTrsferimento?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.INDENNITA).Load();

                    foreach (var t in lTrsferimento)
                    {
                        var indennita = t.INDENNITA;

                        var nCont = cs.INDENNITA.Count(a => a.IDTRASFINDENNITA == t.INDENNITA.IDTRASFINDENNITA);
                        if (nCont <= 0)
                        {
                            cs.INDENNITA.Add(indennita);

                            Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, cs.DATAINIZIOVALIDITA, db);
                        }
                    }

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Errore nella fase di associazione dell'indennità al coefficiente di sede.");
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        public void AssociaIndennita_TFR(decimal idTFR, ModelDBISE db)
        {
            try
            {
                var tfr = db.TFR.Find(idTFR);
                var item = db.Entry<TFR>(tfr);


                var lTrsferimento =
                    db.TRASFERIMENTO.Where(
                        a =>
                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                            a.DATARIENTRO >= tfr.DATAINIZIOVALIDITA && a.DATAPARTENZA <= tfr.DATAFINEVALIDITA)
                        .OrderBy(a => a.DATAPARTENZA)
                        .ToList();

                if (lTrsferimento?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.INDENNITA).Load();

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

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaMAB_VMAB(decimal idPerceMAB, ModelDBISE db)
        {
            try
            {
                var pm = db.PERCENTUALEMAB.Find(idPerceMAB);
                var item = db.Entry<PERCENTUALEMAB>(pm);


                var lvmab =
                    db.VARIAZIONIMAB.Where(
                        a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                            a.DATAFINEMAB >= pm.DATAINIZIOVALIDITA && a.DATAINIZIOMAB <= pm.DATAFINEVALIDITA)
                        .OrderBy(a => a.DATAINIZIOMAB)
                        .ToList();

                if (lvmab?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.VARIAZIONIMAB).Load();

                    foreach (var vmab in lvmab)
                    {
                        var nConta =
                            pm.VARIAZIONIMAB.Count(
                                a =>
                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                    a.IDVARIAZIONIMAB == vmab.IDVARIAZIONIMAB);

                        if (nConta <= 0)
                        {
                            pm.VARIAZIONIMAB.Add(vmab);

                            var t = vmab.MAGGIORAZIONEABITAZIONE.TRASFERIMENTO;

                            Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, pm.DATAINIZIOVALIDITA, db);
                        }

                    }

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Errore nella fase di associazione della percentuale di maggiorazione abitazione sulla tabella VariazioneMAB.");
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaMaggiorazioniAbitazione_MA(decimal idMagAnnuali, ModelDBISE db)
        {
            try
            {
                var ma = db.MAGGIORAZIONIANNUALI.Find(idMagAnnuali);
                var item = db.Entry<MAGGIORAZIONIANNUALI>(ma);


                var lmab =
                    db.MAGGIORAZIONEABITAZIONE.Where(
                        a =>
                            a.TRASFERIMENTO.IDUFFICIO == ma.IDUFFICIO &&
                            a.VARIAZIONIMAB.Where(
                                b =>
                                    b.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                    b.DATAFINEMAB >= ma.DATAINIZIOVALIDITA && b.DATAINIZIOMAB <= ma.DATAFINEVALIDITA)
                                .Any()).OrderBy(a => a.IDMAB).ToList();

                if (lmab?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.MAGGIORAZIONEABITAZIONE).Load();

                    foreach (var mab in lmab)
                    {
                        var nConta =
                            ma.MAGGIORAZIONEABITAZIONE.Count(
                                a =>
                                    a.IDMAB == mab.IDMAB &&
                                    a.VARIAZIONIMAB.Any(b => b.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                             b.DATAFINEMAB >= ma.DATAINIZIOVALIDITA &&
                                                             b.DATAINIZIOMAB <= ma.DATAFINEVALIDITA));

                        if (nConta <= 0)
                        {
                            ma.MAGGIORAZIONEABITAZIONE.Add(mab);

                            var t = mab.TRASFERIMENTO;

                            Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, ma.DATAINIZIOVALIDITA, db);
                        }

                    }

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Errore nella fase di associazione delle maggiorazioni annuali sulla tabella MaggiorazioneAbitazione.");
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaPagatoCondivisoMAB(decimal idPercentualeCondivisione, ModelDBISE db)
        {
            try
            {
                var pc = db.PERCENTUALECONDIVISIONE.Find(idPercentualeCondivisione);
                var item = db.Entry<PERCENTUALECONDIVISIONE>(pc);


                var lpc =
                    db.PAGATOCONDIVISOMAB.Where(
                        a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                            a.DATAFINEVALIDITA >= pc.DATAINIZIOVALIDITA && a.DATAINIZIOVALIDITA <= pc.DATAFINEVALIDITA)
                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                        .ToList();

                if (lpc?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.PAGATOCONDIVISOMAB).Load();

                    foreach (var pgc in lpc)
                    {
                        var nConta =
                            pc.PAGATOCONDIVISOMAB.Count(
                                a =>
                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                    a.IDPAGATOCONDIVISO == pgc.IDPAGATOCONDIVISO);


                        if (nConta <= 0)
                        {
                            pc.PAGATOCONDIVISOMAB.Add(pgc);

                            var t = pgc.MAGGIORAZIONEABITAZIONE.TRASFERIMENTO;
                            Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, pc.DATAINIZIOVALIDITA, db);

                        }

                    }

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Errore nella fase di associazione della percentuale di condivisione alla tabella PagatoConvisoMAB.");
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaPercentualeAnticipoTEP(decimal idPercentualeAnticipoTEP, ModelDBISE db)
        {
            try
            {
                var patep = db.PERCENTUALEANTICIPOTE.Find(idPercentualeAnticipoTEP);
                var item = db.Entry<PERCENTUALEANTICIPOTE>(patep);


                var ltep =
                    db.TEPARTENZA.Where(
                        a =>
                            a.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                            a.TRASFERIMENTO.DATARIENTRO >= patep.DATAINIZIOVALIDITA &&
                            a.TRASFERIMENTO.DATAPARTENZA <= patep.DATAFINEVALIDITA)
                        .OrderBy(a => a.TRASFERIMENTO.DATAPARTENZA)
                        .ToList();

                if (ltep?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.TEPARTENZA).Load();

                    foreach (var tep in ltep)
                    {
                        var nConta =
                            patep.TEPARTENZA.Count(
                                a =>
                                    a.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                    a.IDTEPARTENZA == tep.IDTEPARTENZA);

                        if (nConta <= 0)
                        {
                            patep.TEPARTENZA.Add(tep);

                            var t = tep.TRASFERIMENTO;
                            Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, patep.DATAINIZIOVALIDITA, db);
                        }

                    }

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Errore nella fase di associazione della percentuale di anticipo trasporto effetti fase partenza alla tabella TEPartenza.");
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaPercentualeAnticipoTER(decimal idPercentualeAnticipoTER, ModelDBISE db)
        {
            try
            {
                var pater = db.PERCENTUALEANTICIPOTE.Find(idPercentualeAnticipoTER);
                var item = db.Entry<PERCENTUALEANTICIPOTE>(pater);

                var lter =
                    db.TERIENTRO.Where(
                        a =>
                            a.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                            a.TRASFERIMENTO.DATARIENTRO >= pater.DATAINIZIOVALIDITA &&
                            a.TRASFERIMENTO.DATAPARTENZA <= pater.DATAFINEVALIDITA)
                        .OrderBy(a => a.TRASFERIMENTO.DATAPARTENZA)
                        .ToList();

                if (lter?.Any() ?? false)
                {
                    item.State = EntityState.Modified;
                    item.Collection(a => a.TEPARTENZA).Load();

                    foreach (var ter in lter)
                    {
                        var nConta =
                            pater.TERIENTRO.Count(
                                a =>
                                    a.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                    a.IDTERIENTRO == ter.IDTERIENTRO);

                        if (nConta <= 0)
                        {
                            pater.TERIENTRO.Add(ter);

                            var t = ter.TRASFERIMENTO;
                            Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, pater.DATAINIZIOVALIDITA, db);
                        }

                    }

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Errore nella fase di associazione della percentuale di anticipo trasporto effetti fase rientro alla tabella TERientro.");
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaPrimaSistemazione_IS(decimal idIndSistemazione, ModelDBISE db)
        {
            var indSist = db.INDENNITASISTEMAZIONE.Find(idIndSistemazione);
            var item = db.Entry<INDENNITASISTEMAZIONE>(indSist);

            var lps =
                db.PRIMASITEMAZIONE.Where(
                    a =>
                        (a.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                         a.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Da_Attivare ||
                         a.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) &&
                        a.TRASFERIMENTO.DATARIENTRO >= indSist.DATAINIZIOVALIDITA &&
                        a.TRASFERIMENTO.DATAPARTENZA <= indSist.DATAFINEVALIDITA &&
                        a.TRASFERIMENTO.IDTIPOTRASFERIMENTO == indSist.IDTIPOTRASFERIMENTO)
                    .OrderBy(a => a.TRASFERIMENTO.DATAPARTENZA)
                    .ToList();

            if (lps?.Any() ?? false)
            {
                item.State = EntityState.Modified;
                item.Collection(a => a.PRIMASITEMAZIONE).Load();

                foreach (var ps in lps)
                {
                    var nConta =
                        indSist.PRIMASITEMAZIONE.Count(
                            a =>
                                a.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                a.TRASFERIMENTO.IDTIPOTRASFERIMENTO == ps.TRASFERIMENTO.IDTIPOTRASFERIMENTO &&
                                a.IDPRIMASISTEMAZIONE == ps.IDPRIMASISTEMAZIONE);

                    if (nConta <= 0)
                    {
                        indSist.PRIMASITEMAZIONE.Add(ps);
                        var t = ps.TRASFERIMENTO;
                        Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, indSist.DATAINIZIOVALIDITA, db);
                    }
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione dell'indennità di sistemazione alla tabella PRIMASITEMAZIONE.");
                }

            }

        }

        public void AssociaPrimaSistemazione_PKM(decimal idPercKM, ModelDBISE db)
        {
            var pfkm = db.PERCENTUALEFKM.Find(idPercKM);
            var item = db.Entry<PERCENTUALEFKM>(pfkm);

            var lps = db.PRIMASITEMAZIONE.Where(
                a =>
                    (a.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                     a.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Da_Attivare ||
                     a.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) &&
                    a.TRASFERIMENTO.DATARIENTRO >= pfkm.DATAINIZIOVALIDITA &&
                    a.TRASFERIMENTO.DATAPARTENZA <= pfkm.DATAFINEVALIDITA &&
                    a.PERCENTUALEFKM.Any(b => b.IDFKM == pfkm.IDFKM))
                .OrderBy(a => a.TRASFERIMENTO.DATAPARTENZA)
                .ToList();

            if (lps?.Any() ?? false)
            {
                item.State = EntityState.Modified;
                item.Collection(a => a.PRIMASITEMAZIONE).Load();

                foreach (var ps in lps)
                {
                    var nConta =
                        pfkm.PRIMASITEMAZIONE.Count(
                            a =>
                                a.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                a.TRASFERIMENTO.IDTIPOTRASFERIMENTO == ps.TRASFERIMENTO.IDTIPOTRASFERIMENTO &&
                                a.PERCENTUALEFKM.Any(b => b.IDFKM == pfkm.IDFKM) &&
                                a.IDPRIMASISTEMAZIONE == ps.IDPRIMASISTEMAZIONE);

                    if (nConta <= 0)
                    {
                        pfkm.PRIMASITEMAZIONE.Add(ps);
                        var t = ps.TRASFERIMENTO;
                        Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, pfkm.DATAINIZIOVALIDITA, db);
                    }
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione della fascia chilometrica alla tabella PRIMASITEMAZIONE.");
                }

            }


        }

        public void AssociaRichiamo_CR(decimal idCoeffRichiamo, ModelDBISE db)
        {
            var cr = db.COEFFICIENTEINDRICHIAMO.Find(idCoeffRichiamo);
            var item = db.Entry<COEFFICIENTEINDRICHIAMO>(cr);


            var lr =
                db.RICHIAMO.Where(
                    a =>
                        a.ANNULLATO == false &&
                        a.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato &&
                        a.DATARIENTRO >= cr.DATAINIZIOVALIDITA).OrderBy(a => a.DATARIENTRO).ToList();

            if (lr?.Any() ?? false)
            {
                item.State = EntityState.Modified;
                item.Collection(a => a.RICHIAMO).Load();

                foreach (var r in lr)
                {
                    var nConta =
                        cr.RICHIAMO.Count(
                            a =>
                                a.ANNULLATO == false &&
                                a.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato && a.IDRICHIAMO == r.IDRICHIAMO);

                    if (nConta <= 0)
                    {
                        cr.RICHIAMO.Add(r);
                        var t = r.TRASFERIMENTO;
                        Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, cr.DATAINIZIOVALIDITA, db);
                    }


                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione del coefficente di richiamo alla tabella Richiamo.");
                }
            }


        }

        public void AssociaRichiamo_PKM(decimal idPercKM, ModelDBISE db)
        {
            var pfkm = db.PERCENTUALEFKM.Find(idPercKM);
            var item = db.Entry<PERCENTUALEFKM>(pfkm);

            var lr =
                db.RICHIAMO.Where(
                    a =>
                        a.ANNULLATO == false &&
                        a.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato &&
                        a.TRASFERIMENTO.PRIMASITEMAZIONE.PERCENTUALEFKM.Any(b => b.IDFKM == pfkm.IDFKM) &&
                        a.DATARIENTRO >= pfkm.DATAINIZIOVALIDITA).OrderBy(a => a.DATARIENTRO).ToList();

            if (lr?.Any() ?? false)
            {
                item.State = EntityState.Modified;
                item.Collection(a => a.RICHIAMO).Load();

                foreach (var r in lr)
                {
                    var nConta =
                        pfkm.RICHIAMO.Count(
                            a =>
                                a.ANNULLATO == false &&
                                a.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato && a.IDRICHIAMO == r.IDRICHIAMO);

                    if (nConta <= 0)
                    {
                        pfkm.RICHIAMO.Add(r);
                        var t = r.TRASFERIMENTO;
                        Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, pfkm.DATAINIZIOVALIDITA, db);
                    }


                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione della fascia chilometrica alla tabella Richiamo.");
                }
            }


        }

        public void AssociaRiduzioniIB(decimal idIndBase, ModelDBISE db)
        {
            var ib = db.INDENNITABASE.Find(idIndBase);
            var item = db.Entry<INDENNITABASE>(ib);

            var lr =
                db.RIDUZIONI.Where(
                    a =>
                        a.ANNULLATO == false && a.DATAFINEVALIDITA >= ib.DATAINIZIOVALIDITA &&
                        a.DATAINIZIOVALIDITA <= ib.DATAFINEVALIDITA &&
                        a.IDFUNZIONERIDUZIONE == (decimal)EnumFunzioniRiduzione.Indennita_Base)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();

            if (lr?.Any() ?? false)
            {
                item.State = EntityState.Modified;
                item.Collection(a => a.RIDUZIONI).Load();

                foreach (var r in lr)
                {
                    var nConta = ib.RIDUZIONI.Count(a => a.ANNULLATO == false && a.IDRIDUZIONI == r.IDRIDUZIONI);

                    if (nConta <= 0)
                    {
                        ib.RIDUZIONI.Add(r);

                        var li =
                            ib.INDENNITA.Where(
                                a =>
                                    a.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                                    a.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato)
                                .OrderBy(a => a.TRASFERIMENTO.DATAPARTENZA)
                                .ToList();

                        foreach (var ind in li)
                        {
                            var t = ind.TRASFERIMENTO;
                            Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, ib.DATAINIZIOVALIDITA, db);
                        }

                    }
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione dell'indennità di base alla tabella Riduzioni.");
                }
            }

        }

        public void AssociaRiduzioni_CR(decimal idCoeffRichiamo, ModelDBISE db)
        {
            var cr = db.COEFFICIENTEINDRICHIAMO.Find(idCoeffRichiamo);
            var item = db.Entry<COEFFICIENTEINDRICHIAMO>(cr);

            var lr =
                db.RIDUZIONI.Where(
                    a =>
                        a.ANNULLATO == false && a.DATAFINEVALIDITA >= cr.DATAINIZIOVALIDITA &&
                        a.DATAINIZIOVALIDITA <= cr.DATAFINEVALIDITA &&
                        a.IDFUNZIONERIDUZIONE == (decimal)EnumFunzioniRiduzione.Coefficente_Richiamo)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();

            if (lr?.Any() ?? false)
            {
                item.State = EntityState.Modified;
                item.Collection(a => a.RIDUZIONI).Load();

                foreach (var r in lr)
                {
                    var nConta = cr.RIDUZIONI.Count(a => a.ANNULLATO == false && a.IDRIDUZIONI == r.IDRIDUZIONI);

                    if (nConta <= 0)
                    {
                        cr.RIDUZIONI.Add(r);

                        var lric = cr.RICHIAMO.Where(a => a.ANNULLATO == false).ToList();

                        foreach (var ric in lric)
                        {
                            var t = ric.TRASFERIMENTO;
                            Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, cr.DATAINIZIOVALIDITA, db);
                        }

                    }

                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione del coefficente di richiamo alla tabella Riduzioni.");
                }


            }

        }

        public void AssociaRiduzioni_IS(decimal idIndSistemazione, ModelDBISE db)
        {
            var indSist = db.INDENNITASISTEMAZIONE.Find(idIndSistemazione);
            var item = db.Entry<INDENNITASISTEMAZIONE>(indSist);

            var lr =
                db.RIDUZIONI.Where(
                    a =>
                        a.ANNULLATO == false && a.DATAFINEVALIDITA >= indSist.DATAINIZIOVALIDITA &&
                        a.DATAINIZIOVALIDITA <= indSist.DATAFINEVALIDITA &&
                        a.IDFUNZIONERIDUZIONE == (decimal)EnumFunzioniRiduzione.Indennita_Sistemazione)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();

            if (lr?.Any() ?? false)
            {
                item.State = EntityState.Modified;
                item.Collection(a => a.RIDUZIONI).Load();

                foreach (var r in lr)
                {
                    var nConta = indSist.RIDUZIONI.Count(a => a.ANNULLATO == false && a.IDRIDUZIONI == r.IDRIDUZIONI);

                    if (nConta <= 0)
                    {
                        indSist.RIDUZIONI.Add(r);

                        var lps = indSist.PRIMASITEMAZIONE;

                        foreach (var ps in lps)
                        {
                            var t = ps.TRASFERIMENTO;
                            Utility.DataInizioRicalcoliDipendente(t.IDTRASFERIMENTO, indSist.DATAINIZIOVALIDITA, db);
                        }

                    }

                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di associazione dell'indennità di prima sistemazione alla tabella Riduzioni.");
                }

            }





        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        //public void Dipendenti()
        //{
        //    DateTime dtNow = Convert.ToDateTime("01/04/2018");

        //    using (ModelDBISE db = new ModelDBISE())
        //    {

        //        var ld =
        //            db.DIPENDENTI.Where(
        //                a =>
        //                    a.ABILITATO == true &&
        //                    a.UTENTIAUTORIZZATI.Where(
        //                        b => b.IDRUOLOUTENTE == (decimal)EnumRuoloAccesso.Utente).Any() &&
        //                    a.TRASFERIMENTO.Where(
        //                        c =>
        //                            (c.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
        //                             c.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) &&
        //                            dtNow >= c.DATAPARTENZA &&
        //                            dtNow <= (c.DATARIENTRO.HasValue == true ? c.DATARIENTRO.Value : new DateTime(9999, 12, 31))).Any()).ToList();


        //        foreach (var d in ld)
        //        {

        //        }



        //    }
        //}

    }
}