﻿using NewISE.EF;
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
        public IList<RiepiloVociModel> GetRiepilogoVoci(decimal idTrasferimento)
        {
            List<RiepiloVociModel> lrvm = new List<RiepiloVociModel>();
            

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    if (t != null && t.IDTRASFERIMENTO > 0)
                    {

                        var ps = t.PRIMASITEMAZIONE;
                        var ind = t.INDENNITA;
                        var tep = t.TEPARTENZA;
                        var ter = t.TERIENTRO;
                        
                        var lTeorici =
                            db.TEORICI.Where(
                                a =>
                                    a.ANNULLATO == false && a.ELABORATO == true && 
                                    (a.ELABINDSISTEMAZIONE.IDPRIMASISTEMAZIONE == ps.IDPRIMASISTEMAZIONE) ||
                                    a.ELABINDENNITA.Any(b => b.ANNULLATO == false && b.IDTRASFINDENNITA == ind.IDTRASFINDENNITA) ||
                                    a.ELABMAB.Any(b => b.ANNULLATO == false && b.IDTRASFINDENNITA == ind.IDTRASFINDENNITA) ||
                                    a.ELABTRASPEFFETTI.IDTEPARTENZA.Value == tep.IDTEPARTENZA ||
                                    a.ELABTRASPEFFETTI.IDTERIENTRO.Value == ter.IDTERIENTRO)
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
                               
                                RiepiloVociModel rv = new RiepiloVociModel()
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