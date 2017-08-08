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

        public void AssociaRuoloDipendente_Indennita(decimal idTrasferimento, decimal id, ModelDBISE db)
        {

            try
            {
                var i = db.INDENNITA.Find(idTrasferimento);

                var item = db.Entry<INDENNITA>(i);

                item.State = System.Data.Entity.EntityState.Modified;



                item.Collection(a => a.RUOLODIPENDENTE).Load();

                var e = db.RUOLODIPENDENTE.Find(id);

                i.RUOLODIPENDENTE.Add(e);

                db.SaveChanges();


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public void RimuoviAssociaRuoloDipendente_Indennita(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {

            var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
            var lrd = i.RUOLODIPENDENTE.Where(a => a.ANNULLATO == false && dt >= a.DATAINZIOVALIDITA && dt <= a.DATAFINEVALIDITA).ToList();

            foreach (var item in lrd)
            {
                i.RUOLODIPENDENTE.Remove(item);
            }
            db.SaveChanges();


        }


        public RuoloDipendenteModel GetRuoloDipendente(decimal idRuolo, DateTime dataIni)
        {
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            using (ModelDBISE db = new ModelDBISE())
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
        public RuoloDipendenteModel GetRuoloDipendente(decimal idRuolo, DateTime dataIni, ModelDBISE db)
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

        public RuoloDipendenteModel GetRuoloDipendenteByIdTrasferimento(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            var t = db.TRASFERIMENTO.Find(idTrasferimento);

            if (t != null && t.IDTRASFERIMENTO > 0)
            {
                var i = t.INDENNITA;

                var lrd = i.RUOLODIPENDENTE.Where(a => a.ANNULLATO == false && dt >= a.DATAINZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINZIOVALIDITA).ToList();

                if (lrd != null && lrd.Count > 0)
                {
                    var rd = lrd.First();
                    rdm = new RuoloDipendenteModel()
                    {
                        idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                        idRuolo = rd.IDRUOLO,
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
                    var i = t.INDENNITA;

                    var lrd = i.RUOLODIPENDENTE.Where(a => a.ANNULLATO == false && dt >= a.DATAINZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINZIOVALIDITA).ToList();

                    if (lrd != null && lrd.Count > 0)
                    {
                        var rd = lrd.First();
                        rdm = new RuoloDipendenteModel()
                        {
                            idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                            idRuolo = rd.IDRUOLO,
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





        public void SetRuoloDipendente(ref RuoloDipendenteModel rdm, ModelDBISE db)
        {
            RUOLODIPENDENTE rd;

            rd = new RUOLODIPENDENTE()
            {
                IDRUOLO = rdm.idRuolo,
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
                rd.DATAINZIOVALIDITA = rdm.dataInizioValidita;
                rd.DATAFINEVALIDITA = rdm.dataFineValidita.HasValue == true ? rdm.dataFineValidita.Value : Utility.DataFineStop();
                rd.DATAAGGIORNAMENTO = rdm.dataAggiornamento;
                rd.ANNULLATO = rdm.annullato;

                db.SaveChanges();
            }



        }


    }
}