
using NewISE.EF;
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


        public void RimuoviAssociaCoefficenteSede_Indennita(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            var i = db.INDENNITA.Find(idTrasferimento);

            var item = db.Entry<INDENNITA>(i);

            item.State = System.Data.Entity.EntityState.Modified;

            item.Collection(a => a.COEFFICIENTESEDE).Load();


            var e = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).ToList();

            if (e != null && e.Count > 0)
            {
                foreach (var it in e)
                {
                    i.COEFFICIENTESEDE.Remove(it);
                }
            }

            

            db.SaveChanges();
        }


        public CoefficientiSedeModel GetCoefficenteSedeByIdTrasf(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            CoefficientiSedeModel csm = new CoefficientiSedeModel();

            var lcs = db.INDENNITA.Find(idTrasferimento)
                                  .COEFFICIENTESEDE.Where(a => a.ANNULLATO == false && 
                                                          dt >= a.DATAINIZIOVALIDITA && 
                                                          dt <= a.DATAFINEVALIDITA)
                                                   .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                                   .ToList();

            if (lcs != null && lcs.Count > 0)
            {
                var cs = lcs.First();

                csm = new CoefficientiSedeModel()
                {
                    idCoefficientiSede = cs.IDCOEFFICIENTESEDE,
                    idUfficio = cs.IDUFFICIO,
                    dataInizioValidita = cs.DATAINIZIOVALIDITA,
                    dataFineValidita = cs.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : cs.DATAFINEVALIDITA,
                    valore = cs.VALORECOEFFICIENTE,
                    dataAggiornamento = cs.DATAAGGIORNAMENTO,
                    annullato = cs.ANNULLATO,
                    Ufficio = new UfficiModel()
                    {
                        idUfficio = cs.UFFICI.IDUFFICIO,
                        idValuta = cs.UFFICI.IDVALUTA,
                        codiceUfficio = cs.UFFICI.CODICEUFFICIO,
                        descUfficio = cs.UFFICI.DESCRIZIONEUFFICIO,
                        pagatoValutaUfficio = cs.UFFICI.PAGATOVALUTAUFFICIO
                    }
                };
            }

            return csm;
        }

        public CoefficientiSedeModel GetCoefficenteSede(decimal idCoefficenteSede, ModelDBISE db)
        {
            CoefficientiSedeModel csm = new CoefficientiSedeModel();

            var cs = db.COEFFICIENTESEDE.Find(idCoefficenteSede);

            if (cs!= null && cs.IDCOEFFICIENTESEDE > 0)
            {
                csm = new CoefficientiSedeModel()
                {
                    idCoefficientiSede = cs.IDCOEFFICIENTESEDE,
                    idUfficio = cs.IDUFFICIO,
                    dataInizioValidita = cs.DATAINIZIOVALIDITA,
                    dataFineValidita = cs.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : cs.DATAFINEVALIDITA,
                    valore = cs.VALORECOEFFICIENTE,
                    dataAggiornamento = cs.DATAAGGIORNAMENTO,
                    annullato = cs.ANNULLATO,
                    Ufficio = new UfficiModel()
                    {
                        idUfficio = cs.UFFICI.IDUFFICIO,
                        idValuta = cs.UFFICI.IDVALUTA,
                        codiceUfficio = cs.UFFICI.CODICEUFFICIO,
                        descUfficio = cs.UFFICI.DESCRIZIONEUFFICIO,
                        pagatoValutaUfficio = cs.UFFICI.PAGATOVALUTAUFFICIO
                    }
                };
            }

            return csm;
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
                    dataFineValidita = cs.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : cs.DATAFINEVALIDITA,
                    valore = cs.VALORECOEFFICIENTE,
                    dataAggiornamento = cs.DATAAGGIORNAMENTO,
                    annullato = cs.ANNULLATO
                };
            }

            return csm;
        }


    }
}