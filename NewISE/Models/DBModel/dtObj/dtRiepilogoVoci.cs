using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRiepilogoVoci : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public IList<RiepilogoVociModel> GetRiepilogoVoci(decimal idTrasferimento)
        {
            List<RiepilogoVociModel> lrvm = new List<RiepilogoVociModel>();

            List<MeseAnnoElaborazioneModel> lmaem = new List<MeseAnnoElaborazioneModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);

                    if (t != null && t.IDTRASFERIMENTO > 0)
                    {
                        var ps = t.PRIMASITEMAZIONE;
                        var ind = t.INDENNITA;
                        var tep = t.TEPARTENZA;
                        var ter = t.TERIENTRO;
                     
                        DateTime DataLimiteRiepilogoVoci = Convert.ToDateTime(System.Configuration.ConfigurationManager.AppSettings["DataLimiteRiepilogoVoci"]);

                        decimal idMeseAnnoElaborazioneLimite = 0;
                        using (CalcoloMeseAnnoElaborazione cmem = new CalcoloMeseAnnoElaborazione(db))
                        {
                            lmaem = cmem.Mae.Where(a=>
                                                    a.mese>= Convert.ToDecimal(DataLimiteRiepilogoVoci.Month) &&
                                                    a.anno >= Convert.ToDecimal(DataLimiteRiepilogoVoci.Year))
                                            .OrderBy(a=>a.idMeseAnnoElab)
                                            .ToList();
                            if(lmaem?.Any()??false)
                            {
                                idMeseAnnoElaborazioneLimite = lmaem.First().idMeseAnnoElab;
                            }
                        }

                        var lTeorici =
                               trasferimento.TEORICI.Where(
                                         a =>
                                            a.ANNULLATO == false && 
                                            a.ELABORATO == true && 
                                            a.IDMESEANNOELAB>=idMeseAnnoElaborazioneLimite)
                                        .OrderBy(a => a.ANNORIFERIMENTO)
                                        .ThenBy(a => a.MESERIFERIMENTO)
                                        .ToList();

                        if (lTeorici?.Any() ?? false)
                        {
                            foreach (var teorico in lTeorici)
                            {
                                var idMeseAnnoElaborato = teorico.MESEANNOELABORAZIONE.IDMESEANNOELAB;
                                var tm = teorico.TIPOMOVIMENTO;
                                var voce = teorico.VOCI;
                                var tl = teorico.VOCI.TIPOLIQUIDAZIONE;
                                var tv = teorico.VOCI.TIPOVOCE;
                               
                                RiepilogoVociModel rv = new RiepilogoVociModel()
                                {
                                    idMeseAnnoElaborato = teorico.MESEANNOELABORAZIONE.IDMESEANNOELAB,
                                    dataOperazione = teorico.DATAOPERAZIONE,
                                    importo = teorico.IMPORTO,
                                    descrizione = teorico.VOCI.DESCRIZIONE,
                                    TipoMovimento = new TipoMovimentoModel()
                                    {
                                        idTipoMovimento = tm.IDTIPOMOVIMENTO,
                                        TipoMovimento = tm.TIPOMOVIMENTO1,
                                        DescMovimento = tm.DESCMOVIMENTO
                                    },
                                    TipoLiquidazione = new TipoLiquidazioneModel()
                                    {
                                        idTipoLiquidazione = tl.IDTIPOLIQUIDAZIONE,
                                        descrizione = tl.DESCRIZIONE
                                    },
                                    TipoVoce = new TipoVoceModel()
                                    {
                                        idTipoVoce = tv.IDTIPOVOCE,
                                        descrizione = tv.DESCRIZIONE
                                    },
                                    Voci = new VociModel()
                                    {
                                        idVoci = voce.IDVOCI,
                                        codiceVoce = voce.CODICEVOCE,
                                        descrizione = voce.DESCRIZIONE,
                                        TipoLiquidazione = new TipoLiquidazioneModel()
                                        {
                                            idTipoLiquidazione = tl.IDTIPOLIQUIDAZIONE,
                                            descrizione = tl.DESCRIZIONE
                                        },
                                        TipoVoce = new TipoVoceModel()
                                        {
                                            idTipoVoce = tv.IDTIPOVOCE,
                                            descrizione = tv.DESCRIZIONE
                                        }
                                    },
                                    meseRiferimento = teorico.MESERIFERIMENTO,
                                    annoRiferimento = teorico.ANNORIFERIMENTO,
                                    annomeseRiferimento = teorico.ANNORIFERIMENTO + teorico.MESERIFERIMENTO,
                                    MeseAnnoElaborato = teorico.MESEANNOELABORAZIONE.MESE.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + teorico.MESEANNOELABORAZIONE.ANNO.ToString(),                                 
                                    GiornoMeseAnnoElaborato=Convert.ToDateTime("01/"+ teorico.MESEANNOELABORAZIONE.MESE.ToString()+"/"+ teorico.MESEANNOELABORAZIONE.ANNO.ToString()),
                                    giorni = 0,
                                    Importo = teorico.IMPORTO,
                                    Elaborato = teorico.ELABORATO
                                };
                                lrvm.Add(rv);
                            }
                        }
                    }

                    db.Database.CurrentTransaction.Commit();

                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }

            }

            return lrvm;

        }

    }
}