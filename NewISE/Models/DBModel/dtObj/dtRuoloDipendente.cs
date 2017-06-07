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

        public RuoloDipendenteModel GetRuoloDipendente(decimal idTrasferimento, DateTime dataIni, EntitiesDBISE db)
        {
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            var rd = db.RUOLODIPENDENTE.Where(a => a.IDTRASFERIMENTO == idTrasferimento &&
                                               dataIni >= a.DATAINZIOVALIDITA &&
                                               dataIni <= a.DATAFINEVALIDITA &&
                                               a.ANNULLATO == false)
                                       .OrderBy(a => a.DATAINZIOVALIDITA).Last();


            if (rd != null && rd.IDRUOLODIPENDENTE > 0)
            {
                
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

        public void SetRuoloDipendente(RuoloDipendenteModel rdm, EntitiesDBISE db)
        {
            RUOLODIPENDENTE rd;

            rd = new RUOLODIPENDENTE()
            {
                IDRUOLO = rdm.idRuolo,
                IDTRASFERIMENTO = rdm.idTrasferimento,
                DATAINZIOVALIDITA = rdm.dataInizioValidita,
                DATAFINEVALIDITA = rdm.dataFineValidita.HasValue == true ? rdm.dataFineValidita.Value : Convert.ToDateTime("31/12/9999"),
                DATAAGGIORNAMENTO = rdm.dataAggiornamento,
                ANNULLATO = rdm.annullato

            };

            db.SaveChanges();




        }

        public void EditRuoloDipendente(RuoloDipendenteModel rdm, EntitiesDBISE db)
        {
            RUOLODIPENDENTE rd = db.RUOLODIPENDENTE.Find(rdm.idRuoloDipendente);

            if (rd != null && rd.IDRUOLODIPENDENTE > 0)
            {
                rd.IDRUOLO = rdm.idRuolo;
                rd.IDTRASFERIMENTO = rdm.idTrasferimento;
                rd.DATAINZIOVALIDITA = rdm.dataInizioValidita;
                rd.DATAFINEVALIDITA = rdm.dataFineValidita.HasValue == true ? rdm.dataFineValidita.Value : Convert.ToDateTime("31/12/9999");
                rd.DATAAGGIORNAMENTO = rdm.dataAggiornamento;
                rd.ANNULLATO = rdm.annullato;

                db.SaveChanges();
            }



        }


    }
}