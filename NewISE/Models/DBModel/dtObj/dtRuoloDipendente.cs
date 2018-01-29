using NewISE.EF;
using NewISE.Models.dtObj;
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





        public IList<RuoloDipendenteModel> GetRuoliDipendenteIndennitaByRangeDate(decimal idRuolo, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            List<RuoloDipendenteModel> lrdm = new List<RuoloDipendenteModel>();

            var r = db.RUOLOUFFICIO.Find(idRuolo);

            var lrd =
                r.RUOLODIPENDENTE.Where(
                    a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= dtIni && a.DATAINZIOVALIDITA <= dtFin)
                    .OrderBy(a => a.DATAINZIOVALIDITA);

            if (lrd?.Any() ?? false)
            {
                foreach (var rd in lrd)
                {
                    var rdm = new RuoloDipendenteModel()
                    {
                        idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                        idRuolo = rd.IDRUOLO,
                        idTrasferimento = rd.IDTRASFERIMENTO,
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

                    lrdm.Add(rdm);
                }
            }

            return lrdm;

        }

        public RuoloDipendenteModel GetRuoloDipendente(decimal idTrasferimento, decimal idRuolo, DateTime dataIni, ModelDBISE db)
        {
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            var lrd = db.RUOLODIPENDENTE.Where(a => a.IDRUOLO == idRuolo &&
                                                    a.IDTRASFERIMENTO == idTrasferimento &&
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
                    idTrasferimento = rd.IDTRASFERIMENTO,
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

        public RuoloDipendenteModel GetRuoloDipendenteByIdTrasferimento(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            var t = db.TRASFERIMENTO.Find(idTrasferimento);

            if (t != null && t.IDTRASFERIMENTO > 0)
            {
                var lrd =
                    t.RUOLODIPENDENTE.Where(
                        a => a.ANNULLATO == false && dt >= a.DATAINZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                        .OrderByDescending(a => a.DATAINZIOVALIDITA)
                        .ToList();

                if (lrd?.Any() ?? false)
                {
                    var rd = lrd.First();

                    rdm = new RuoloDipendenteModel()
                    {
                        idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                        idRuolo = rd.IDRUOLO,
                        idTrasferimento = rd.IDTRASFERIMENTO,
                        dataInizioValidita = rd.DATAINZIOVALIDITA,
                        dataFineValidita = rd.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : rd.DATAFINEVALIDITA,
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

        public RuoloDipendenteModel GetRuoloDipendenteByIdTrasferimento(decimal idTrasferimento, DateTime dt)
        {
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                if (t != null && t.IDTRASFERIMENTO > 0)
                {
                    var lrd =
                        t.RUOLODIPENDENTE.Where(
                            a => a.ANNULLATO == false && dt >= a.DATAINZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                            .OrderByDescending(a => a.DATAINZIOVALIDITA)
                            .ToList();


                    if (lrd != null && lrd.Count > 0)
                    {
                        var rd = lrd.First();
                        rdm = new RuoloDipendenteModel()
                        {
                            idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                            idRuolo = rd.IDRUOLO,
                            idTrasferimento = rd.IDTRASFERIMENTO,
                            dataInizioValidita = rd.DATAINZIOVALIDITA,
                            dataFineValidita = rd.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : rd.DATAFINEVALIDITA,
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
            }

            return rdm;

        }



        public void SetNuovoRuoloDipendente(ref RuoloDipendenteModel rdm, ModelDBISE db)
        {
            decimal idTrasferimento = rdm.idTrasferimento;
            DateTime dtIni = rdm.dataInizioValidita;
            //DateTime dtFin = rdm.dataFineValidita;

            var lrd = db.RUOLODIPENDENTE.Where(a => a.ANNULLATO == false && a.IDTRASFERIMENTO == idTrasferimento);

            if (lrd?.Any() ?? false)
            {

            }
        }



        public void SetRuoloDipendente(ref RuoloDipendenteModel rdm, ModelDBISE db)
        {
            RUOLODIPENDENTE rd;

            rd = new RUOLODIPENDENTE()
            {
                IDRUOLO = rdm.idRuolo,
                IDTRASFERIMENTO = rdm.idTrasferimento,
                DATAINZIOVALIDITA = rdm.dataInizioValidita,
                DATAFINEVALIDITA = rdm.dataFineValidita.HasValue == true ? rdm.dataFineValidita.Value : Utility.DataFineStop(),
                DATAAGGIORNAMENTO = rdm.dataAggiornamento,
                ANNULLATO = rdm.annullato

            };

            db.RUOLODIPENDENTE.Add(rd);

            if (db.SaveChanges() > 0)
            {
                rdm.idRuoloDipendente = rd.IDRUOLODIPENDENTE;
            }



        }

        public void EditRuoloDipendente(RuoloDipendenteModel rdm, ModelDBISE db)
        {
            RUOLODIPENDENTE rd = db.RUOLODIPENDENTE.Find(rdm.idRuoloDipendente);

            if (rd != null && rd.IDRUOLODIPENDENTE > 0)
            {
                rd.IDRUOLO = rdm.idRuolo;
                rd.IDTRASFERIMENTO = rdm.idTrasferimento,
                rd.DATAINZIOVALIDITA = rdm.dataInizioValidita;
                rd.DATAFINEVALIDITA = rdm.dataFineValidita.HasValue == true ? rdm.dataFineValidita.Value : Utility.DataFineStop();
                rd.DATAAGGIORNAMENTO = rdm.dataAggiornamento;
                rd.ANNULLATO = rdm.annullato;

                db.SaveChanges();
            }



        }


    }
}