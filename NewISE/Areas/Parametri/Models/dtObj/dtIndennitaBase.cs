using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtIndennitaBase : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<IndennitaBaseModel> getListIndennitaBase()
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (EntitiesDBISEPRO db = new EntitiesDBISEPRO())
                {
                    var lib = db.INDENNITABASE.ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                annullato = e.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                }
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<IndennitaBaseModel> getListIndennitaBase(decimal idLivello)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (EntitiesDBISEPRO db = new EntitiesDBISEPRO())
                {
                    var lib = db.INDENNITABASE.Where(a => a.IDLIVELLO == idLivello).ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                annullato = e.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                }
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<IndennitaBaseModel> getListIndennitaBase(bool escludiAnnullati = false)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (EntitiesDBISEPRO db = new EntitiesDBISEPRO())
                {
                    var lib = db.INDENNITABASE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                annullato = e.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                }
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<IndennitaBaseModel> getListIndennitaBase(decimal idLivello, bool escludiAnnullati = false)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (EntitiesDBISEPRO db = new EntitiesDBISEPRO())
                {
                    var lib = db.INDENNITABASE.Where(a => a.IDLIVELLO == idLivello && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                annullato = e.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                }
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public static EntityObject Clone(EntityObject entity)
        {
            var type = entity.GetType();
            var clone = Activator.CreateInstance(type);

            foreach (var property in type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.SetProperty))
            {
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>)) continue;
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>)) continue;
                if (property.PropertyType.IsSubclassOf(typeof(EntityObject))) continue;

                if (property.CanWrite)
                {
                    property.SetValue(clone, property.GetValue(entity, null), null);
                }
            }

            return (EntityObject)clone;
        }




        public void SetIndennitaDiBase(IndennitaBaseModel ibm)
        {
            List<INDENNITABASE> libNew = new List<INDENNITABASE>();

            INDENNITABASE ibNew = new INDENNITABASE();

            INDENNITABASE ibPrecedente = new INDENNITABASE();

            
            List<INDENNITABASE> lArchivioIB = new List<INDENNITABASE>();

            using (EntitiesDBISEPRO db = new EntitiesDBISEPRO())
            {
                try
                {
                    ibNew = new INDENNITABASE()
                    {
                        IDLIVELLO = ibm.idLivello,
                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                        DATAFINEVALIDITA = ibm.dataFineValidita,
                        VALORE = ibm.valore,
                        VALORERESP = ibm.valoreResponsabile,
                        ANNULLATO = ibm.annullato
                    };

                    db.Database.BeginTransaction();

                    var recordInteressati = db.INDENNITABASE.Where(a => a.ANNULLATO == false && a.IDLIVELLO == ibNew.IDLIVELLO)
                                                            .Where(a => a.DATAINIZIOVALIDITA >= ibNew.DATAINIZIOVALIDITA || a.DATAFINEVALIDITA >= ibNew.DATAINIZIOVALIDITA)
                                                            .Where(a => a.DATAINIZIOVALIDITA <= ibNew.DATAFINEVALIDITA || a.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
                                                            .ToList();

                    recordInteressati.ForEach(a => a.ANNULLATO = true);
                    db.SaveChanges();
                    

                    if (recordInteressati.Count > 0)
                    {
                        foreach (var item in recordInteressati)
                        {
                            INDENNITABASE ib = new INDENNITABASE()
                            {
                                IDLIVELLO = item.IDLIVELLO,
                                ANNULLATO = false,
                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                VALORE = item.VALORE,
                                VALORERESP = item.VALORERESP,

                            };

                            libNew.Add(ib);

                        }

                        libNew.Add(ibNew);

                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                        
                        for (int i = 0; i < libNew.Count; i++)
                        {
                            INDENNITABASE ib = libNew[i];   
                            if ((i + 1) < libNew.Count)
                            {
                                ib.DATAFINEVALIDITA = libNew[i + 1].DATAINIZIOVALIDITA.AddDays(-1);
                            }

                            db.INDENNITABASE.Add(ib);
                        }
                        db.SaveChanges();

                    }                    

                    db.Database.CurrentTransaction.Commit();
                    
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        //public bool VerificaElaborazioneIndennitabase(decimal idIndbase)
        //{
        //    using (EntitiesDBISEPRO db = new EntitiesDBISEPRO())
        //    {
        //        var n = db.INDENNITABASE.Where(a => a.IDINDENNITABASE == idIndbase && a.ANNULLATO == false)
        //                   .First().INDENNITA_INDBASE.Where(a => a.IDINDENNITABASE == idIndbase).First().INDENNITA.ELAB_CONT;

        //    }
        //}

        public void DelIndennitaDiBase(decimal idIndbase)
        {
            INDENNITABASE precedenteIB = new INDENNITABASE();
            INDENNITABASE successivaIB = new INDENNITABASE();
            INDENNITABASE delIB = new INDENNITABASE();

            using (EntitiesDBISEPRO db = new EntitiesDBISEPRO())
            {
                var lib = db.INDENNITABASE.Where(a => a.IDINDENNITABASE == idIndbase);

                if (lib.Count() > 0)
                {
                    delIB = lib.First();

                    //precedenteIB = db.INDENNITABASE.Where(a=>a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.DATAFINEVALIDITA == db.INDENNITABASE)

                    db.INDENNITABASE.Remove(db.INDENNITABASE.Find(idIndbase));
                    db.SaveChanges();
                }
            }
        }
    }
}