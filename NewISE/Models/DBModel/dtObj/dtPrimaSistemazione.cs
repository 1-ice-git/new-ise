﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;
using Newtonsoft.Json.Schema;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPrimaSistemazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public PrimaSistemazioneModel GetPrimaSistemazioneBtIdTrasf(decimal idTrasferimento, ModelDBISE db)
        {
            PrimaSistemazioneModel psm = new PrimaSistemazioneModel();

            var t = db.TRASFERIMENTO.Find(idTrasferimento);

            var ps = t.PRIMASITEMAZIONE;

            psm = new PrimaSistemazioneModel()
            {
                idPrimaSistemazione = ps.IDPRIMASISTEMAZIONE,

            };

            return psm;
        }

        public PrimaSistemazioneModel GetPrimaSistemazioneByIdTrasf(decimal idTrasferimento)
        {
            PrimaSistemazioneModel psm = new PrimaSistemazioneModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                var ps = t.PRIMASITEMAZIONE;

                psm = new PrimaSistemazioneModel()
                {
                    idPrimaSistemazione = ps.IDPRIMASISTEMAZIONE

                };
            }

            return psm;
        }

        public PrimaSistemazioneModel GetPrimaSistemazione(decimal idPrimaSistemazione, ModelDBISE db)
        {
            PrimaSistemazioneModel psm = new PrimaSistemazioneModel();

            var ps = db.PRIMASITEMAZIONE.Find(idPrimaSistemazione);


            psm = new PrimaSistemazioneModel()
            {
                idPrimaSistemazione = ps.IDPRIMASISTEMAZIONE,

            };

            return psm;
        }





        public void InserisciPrimaSistemazione(PrimaSistemazioneModel psm, ModelDBISE db)
        {
            using (dtTrasferimento dttr = new dtTrasferimento())
            {
                var trm = dttr.GetTrasferimentoById(psm.idPrimaSistemazione, db);

                if (trm != null && trm.HasValue())
                {
                    using (dtIndennitaSistemazione dtis = new dtIndennitaSistemazione())
                    {
                        this.SetPrimaSistemazione(psm, db);

                        var lism = dtis.GetListIndennitaSistemazione((EnumTipoTrasferimento)trm.idTipoTrasferimento, trm.dataPartenza, db);

                        if (lism?.Any() ?? false)
                        {
                            foreach (var ism in lism)
                            {
                                this.AssociaIndennitaSistemazione(psm.idPrimaSistemazione, ism.idIndSist, db);
                            }
                        }else
                        {
                            throw new Exception("Indennita Prima Sistemazione non trovata.");
                        }
                    }
                }
                else
                {
                    throw new Exception("Nessun trasferimento rilevato per l'inserimento della prima sistemazione.");
                }
            }
        }

        public void AssociaIndennitaSistemazione(decimal idPrimaSitemazione, decimal idIndennitaSitemazione, ModelDBISE db)
        {
            var ps = db.PRIMASITEMAZIONE.Find(idPrimaSitemazione);

            var item = db.Entry<PRIMASITEMAZIONE>(ps);
            item.State = EntityState.Modified;
            item.Collection(a => a.INDENNITASISTEMAZIONE).Load();
            var indSist = db.INDENNITASISTEMAZIONE.Find(idIndennitaSitemazione);
            ps.INDENNITASISTEMAZIONE.Add(indSist);
            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Non è stato possibile associare l'indennità di sistemazione alla prima sistemazione.");
            }

        }

        public void SetPrimaSistemazione(PrimaSistemazioneModel psm, ModelDBISE db)
        {
            PRIMASITEMAZIONE ps = new PRIMASITEMAZIONE()
            {
                IDPRIMASISTEMAZIONE = psm.idPrimaSistemazione,

            };

            db.PRIMASITEMAZIONE.Add(ps);

            int i = db.SaveChanges();
            if (i <= 0)
            {
                throw new Exception("Errore nell'insenrimento della prima sistemazione.");
            }

        }


    }
}