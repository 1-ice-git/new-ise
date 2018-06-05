
using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtCoefficenteSede : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void AssociaCoefficenteSede_Indennita(decimal idTrasferimento, decimal id, ModelDBISE db)
        {

            try
            {
                var i = db.INDENNITA.Find(idTrasferimento);

                var item = db.Entry<INDENNITA>(i);

                item.State = System.Data.Entity.EntityState.Modified;

                item.Collection(a => a.COEFFICIENTESEDE).Load();



                var e = db.COEFFICIENTESEDE.Find(id);

                i.COEFFICIENTESEDE.Add(e);

                db.SaveChanges();


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void RimuoviCoefficientiSede_Indennita(decimal idTrasferimento, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
            var lcs =
                i.COEFFICIENTESEDE.Where(
                    a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= dtIni && a.DATAINIZIOVALIDITA <= dtFin)
                    .OrderBy(a => a.DATAINIZIOVALIDITA);
            if (lcs?.Any() ?? false)
            {
                foreach (var cs in lcs)
                {
                    i.COEFFICIENTESEDE.Remove(cs);
                }

                db.SaveChanges();
            }
        }

        public void RimuoviAssociaCoefficenteSede_Indennita(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            //var i = db.INDENNITA.Find(idTrasferimento);

            //var item = db.Entry<INDENNITA>(i);

            //item.State = System.Data.Entity.EntityState.Modified;

            //item.Collection(a => a.COEFFICIENTESEDE).Load();

            //var n = i.COEFFICIENTESEDE.ToList().RemoveAll(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA);

            //if (n > 0)
            //    db.SaveChanges();

            var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
            var lit = i.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).ToList();

            foreach (var item in lit)
            {
                i.COEFFICIENTESEDE.Remove(item);
            }
            db.SaveChanges();

        }


        public CoefficientiSedeModel GetCoefficenteSedeByIdTrasferimento(decimal idTrasferimento)
        {

            CoefficientiSedeModel csm = new CoefficientiSedeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lrd = db.INDENNITA.Find(idTrasferimento).COEFFICIENTESEDE.Where(a => a.ANNULLATO == false);

                var cs = lrd.First();
                if (lrd?.Any() ?? false)
                {
                    csm = new CoefficientiSedeModel()
                    {
                        idCoefficientiSede = cs.IDCOEFFICIENTESEDE,
                        idUfficio = cs.IDUFFICIO,
                        dataInizioValidita = cs.DATAINIZIOVALIDITA,
                        dataFineValidita = cs.DATAFINEVALIDITA,
                        dataAggiornamento = cs.DATAAGGIORNAMENTO,
                        annullato = cs.ANNULLATO,
                        
                    };
                }
            }
            return csm;
        }

        

        public CoefficientiSedeModel GetCoefficenteSede(decimal idCoefficenteSede, ModelDBISE db)
        {
            CoefficientiSedeModel csm = new CoefficientiSedeModel();

            var cs = db.COEFFICIENTESEDE.Find(idCoefficenteSede);

            if (cs != null && cs.IDCOEFFICIENTESEDE > 0)
            {
                csm = new CoefficientiSedeModel()
                {
                    idCoefficientiSede = cs.IDCOEFFICIENTESEDE,
                    idUfficio = cs.IDUFFICIO,
                    dataInizioValidita = cs.DATAINIZIOVALIDITA,
                    dataFineValidita = cs.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : cs.DATAFINEVALIDITA,
                    valore = cs.VALORECOEFFICIENTE,
                    dataAggiornamento = cs.DATAAGGIORNAMENTO,
                    annullato = cs.ANNULLATO,
                    Ufficio = new UfficiModel()
                    {
                        idUfficio = cs.UFFICI.IDUFFICIO,
                        codiceUfficio = cs.UFFICI.CODICEUFFICIO,
                        descUfficio = cs.UFFICI.DESCRIZIONEUFFICIO,
                        pagatoValutaUfficio = cs.UFFICI.PAGATOVALUTAUFFICIO
                    }
                };
            }

            return csm;
        }

        public IList<CoefficientiSedeModel> GetCoefficenteSedeIndennitaByRangeDate(decimal idUfficio, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            List<CoefficientiSedeModel> lcsm = new List<CoefficientiSedeModel>();

            var u = db.UFFICI.Find(idUfficio);

            var lcs =
                u.COEFFICIENTESEDE.Where(
                    a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= dtIni && a.DATAINIZIOVALIDITA <= dtFin)
                    .OrderBy(a => a.DATAINIZIOVALIDITA);

            if (lcs?.Any() ?? false)
            {
                foreach (var cs in lcs)
                {
                    var csm = new CoefficientiSedeModel()
                    {
                        idCoefficientiSede = cs.IDCOEFFICIENTESEDE,
                        idUfficio = cs.IDUFFICIO,
                        dataInizioValidita = cs.DATAINIZIOVALIDITA,
                        dataFineValidita =
                            cs.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : cs.DATAFINEVALIDITA,
                        valore = cs.VALORECOEFFICIENTE,
                        dataAggiornamento = cs.DATAAGGIORNAMENTO,
                        annullato = cs.ANNULLATO
                    };

                    lcsm.Add(csm);
                }
            }

            return lcsm;
        }


        public CoefficientiSedeModel GetCoefficenteSedeValido(decimal idUfficio, DateTime dt, ModelDBISE db)
        {
            CoefficientiSedeModel csm = new CoefficientiSedeModel();

            var lcs = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false &&
                                                a.IDUFFICIO == idUfficio &&
                                                dt >= a.DATAINIZIOVALIDITA &&
                                                dt <= a.DATAFINEVALIDITA)
                                         .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                         .ToList();

            if (lcs != null && lcs.Count > 0)
            {
                COEFFICIENTESEDE cs = lcs.First();

                csm = new CoefficientiSedeModel()
                {
                    idCoefficientiSede = cs.IDCOEFFICIENTESEDE,
                    idUfficio = cs.IDUFFICIO,
                    dataInizioValidita = cs.DATAINIZIOVALIDITA,
                    dataFineValidita = cs.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : cs.DATAFINEVALIDITA,
                    valore = cs.VALORECOEFFICIENTE,
                    dataAggiornamento = cs.DATAAGGIORNAMENTO,
                    annullato = cs.ANNULLATO
                };
            }

            return csm;
        }


    }
}