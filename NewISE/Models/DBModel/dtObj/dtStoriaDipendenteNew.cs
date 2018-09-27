using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtStoriaDipendenteNew : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<StoriaDipendenteNewModel> GetStoriaDipendenteNew(decimal Nominativo, ModelDBISE db)
        {
            List<StoriaDipendenteNewModel> rim = new List<StoriaDipendenteNewModel>();
            
            var lDipendenti =
                   db.DIPENDENTI.Where(
                       a =>
                           a.MATRICOLA == Nominativo).ToList();



            if (lDipendenti?.Any() ?? false)
            {
                foreach (var t in lDipendenti)
                {

                        StoriaDipendenteNewModel ldvm = new StoriaDipendenteNewModel()
                        {
                            matricola = t.MATRICOLA,
                            nome = t.NOME,
                            cognome = t.COGNOME
                            
                        };

                        rim.Add(ldvm);
                    
                }
            }

            return rim;

        }



    }
}