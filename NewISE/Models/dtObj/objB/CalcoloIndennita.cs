using NewISE.Models.DBModel;
using System;
using System.Linq;

namespace NewISE.Models.dtObj.objB
{
    public class CalcoloIndennita : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private decimal _indennitaBase = 0;
        private decimal _indennitaServizio = 0;

        public decimal indennitaVase
        {
            get
            {
                return _indennitaBase;
            }
        }

        public decimal indennitaServizio
        {
            get
            {
                return _indennitaServizio;
            }
        }

        public CalcoloIndennita(int matricola, DateTime dataDecorrenza)
        {
        }

        private decimal indennitaBase(int matricola, DateTime dataDecorrenza)
        {
            decimal ib = 0;
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    var ld = db.DIPENDENTI.Where(a => a.ABILITATO == true && a.MATRICOLA == matricola);
                    if (d.Count() == 1)
                    {
                        var lt = ld.First().TRASFERIMENTO.Where(a => a.ANNULLATO == false && (a.DATAPARTENZA <= dataDecorrenza && a.DATARIENTRO >= dataDecorrenza));
                        if (lt.Count() == 1)
                        {
                            var li = lt.First().INDENNITA.Where(a => a.ANNULLATO == false && (a.DATAINIZIO <= dataDecorrenza && a.DATAFINE >= dataDecorrenza));
                            if (li.Count() == 1)
                            {
                                var i = li.First();
                                decimal percentualeRiduzione = 0;

                                var ir = i.INDENNITABASE.INDBASE_RID;

                                



                            }
                            else
                            {
                                //TODO: creare una funzione per la creazione di un log su file.

                                throw new Exception("Errore nella funzione per il calcolo dell'indennita di base."
                            }
                        }
                        else
                        {
                            //TODO: creare una funzione per la creazione di un log su file.

                            throw new Exception("Errore nella funzione per il calcolo dell'indennita di base.");
                        }
                    }
                    else
                    {
                        //TODO: creare una funzione per la creazione di un log su file.

                        throw new Exception("Errore nella funzione per il calcolo dell'indennita di base.");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return ib;



            }
        }
    }
}