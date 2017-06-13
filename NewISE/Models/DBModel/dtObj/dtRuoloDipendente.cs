﻿using NewISE.Models.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRuoloDipendente : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public RuoloDipendenteModel GetRuoloDipendente(decimal idRuolo, DateTime dataIni)
        {
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            using (EntitiesDBISE db=new EntitiesDBISE())
            {
                var lrd = db.RUOLODIPENDENTE.Where(a => a.IDRUOLO == idRuolo &&
                                               dataIni >= a.DATAINZIOVALIDITA &&
                                               dataIni <= a.DATAFINEVALIDITA &&
                                               a.ANNULLATO == false)
                                       .OrderByDescending(a => a.DATAINZIOVALIDITA).ToList();


                if (lrd != null && lrd.Count > 0)
                {
                    var rd = lrd.First();

                    rdm = new RuoloDipendenteModel()
                    {
                        idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                        idRuolo = rd.IDRUOLO,
                        dataInizioValidita = rd.DATAINZIOVALIDITA,
                        dataFineValidita = rd.DATAFINEVALIDITA,
                        dataAggiornamento = rd.DATAAGGIORNAMENTO,
                        annullato = rd.ANNULLATO,
                        RuoloUfficio = new RuoloUfficioModel()
                        {
                            idRuoloUfficio = rd.RUOLOUFFICIO.IDRUOLO,
                            DescrizioneRuolo = rd.RUOLOUFFICIO.DESCRUOLO
                        }
                    };


                }
            }
                       

            return rdm;

        }
        public RuoloDipendenteModel GetRuoloDipendente(decimal idRuolo, DateTime dataIni, EntitiesDBISE db)
        {
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            var lrd = db.RUOLODIPENDENTE.Where(a => a.IDRUOLO == idRuolo &&
                                               dataIni >= a.DATAINZIOVALIDITA &&
                                               dataIni <= a.DATAFINEVALIDITA &&
                                               a.ANNULLATO == false)
                                       .OrderByDescending(a => a.DATAINZIOVALIDITA).ToList();


            if (lrd != null && lrd.Count > 0)
            {
                var rd = lrd.First();

                rdm = new RuoloDipendenteModel()
                {
                    idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                    idRuolo = rd.IDRUOLO,
                    dataInizioValidita = rd.DATAINZIOVALIDITA,
                    dataFineValidita = rd.DATAFINEVALIDITA,
                    dataAggiornamento = rd.DATAAGGIORNAMENTO,
                    annullato = rd.ANNULLATO,
                    RuoloUfficio = new RuoloUfficioModel()
                    {
                        idRuoloUfficio = rd.RUOLOUFFICIO.IDRUOLO,
                        DescrizioneRuolo = rd.RUOLOUFFICIO.DESCRUOLO
                    }
                };
                

            }
            
            return rdm;

        }

        public void SetRuoloDipendente(ref RuoloDipendenteModel rdm, EntitiesDBISE db)
        {
            RUOLODIPENDENTE rd;

            rd = new RUOLODIPENDENTE()
            {
                IDRUOLO = rdm.idRuolo,
                DATAINZIOVALIDITA = rdm.dataInizioValidita,
                DATAFINEVALIDITA = rdm.dataFineValidita.HasValue == true ? rdm.dataFineValidita.Value : Convert.ToDateTime("31/12/9999"),
                DATAAGGIORNAMENTO = rdm.dataAggiornamento,
                ANNULLATO = rdm.annullato

            };

            db.RUOLODIPENDENTE.Add(rd);

            if(db.SaveChanges() > 0)
            {
                rdm.idRuoloDipendente = rd.IDRUOLODIPENDENTE;
            }
            


        }

        public void EditRuoloDipendente(RuoloDipendenteModel rdm, EntitiesDBISE db)
        {
            RUOLODIPENDENTE rd = db.RUOLODIPENDENTE.Find(rdm.idRuoloDipendente);

            if (rd != null && rd.IDRUOLODIPENDENTE > 0)
            {
                rd.IDRUOLO = rdm.idRuolo;
                rd.DATAINZIOVALIDITA = rdm.dataInizioValidita;
                rd.DATAFINEVALIDITA = rdm.dataFineValidita.HasValue == true ? rdm.dataFineValidita.Value : Convert.ToDateTime("31/12/9999");
                rd.DATAAGGIORNAMENTO = rdm.dataAggiornamento;
                rd.ANNULLATO = rdm.annullato;

                db.SaveChanges();
            }



        }


    }
}