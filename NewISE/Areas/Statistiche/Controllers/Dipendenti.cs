using Microsoft.Reporting.WebForms;
using NewISE.Areas.Statistiche.Models;
using NewISE.Areas.Statistiche.Models.dtObj;
using NewISE.Areas.Statistiche.RPTDataSet;
using NewISE.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class Dipendenti
    {
        public static IList<CategoryModel> GetAllDipendenti()
        {
            List<CategoryModel> lcm = new List<CategoryModel>();

            try
            {
                
                using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
                {
                    string query = string.Format(" Select Distinct AND_MATRICOLA MATRICOLA, AND_COGNOME ||' '|| AND_NOME NOMINATIVO From ANADIPE, TRASFERIMENTO Where AND_MATRICOLA = TRA_MATRICOLA Order By NOMINATIVO");
                    {
                        using (OracleCommand cmd1 = new OracleCommand(query, cn))
                        {
                            cmd1.Connection.Open();
                            // Ampoooo vuoto
                            CategoryModel cm1 = new CategoryModel()
                            {
                                matricola = "",
                                nominativo = ""
                            };
                            lcm.Add(cm1);

                            using (OracleDataReader dr = cmd1.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                if (dr.HasRows)
                                {
                                    while (dr.Read())
                                    {
                                        CategoryModel cm = new CategoryModel()
                                        {
                                            matricola = dr["MATRICOLA"].ToString(),
                                            nominativo = dr["NOMINATIVO"].ToString()
                                        };

                                        lcm.Add(cm);

                                    }
                                }
                            }

                        }
                    }




                }

                return lcm;


            }
            catch (Exception ex)
            {

                throw ex;
            }
            

        }

        // Combo Sedi Estere
        // Select Distinct SED_DESCRIZIONE, SED_COD_SEDE   From SEDIESTERE  Order By SED_DESCRIZIONE
        public static IList<DipEsteroModel> GetAllSedi()
        {
            List<DipEsteroModel> lcm = new List<DipEsteroModel>();

            try
            {

                using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
                {
                    string query = string.Format("Select Distinct SED_DESCRIZIONE, SED_COD_SEDE   From SEDIESTERE  Order By SED_DESCRIZIONE");
                    {
                        using (OracleCommand cmd1 = new OracleCommand(query, cn))
                        {
                            cmd1.Connection.Open();

                            using (OracleDataReader dr = cmd1.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                if (dr.HasRows)
                                {
                                    while (dr.Read())
                                    {
                                        DipEsteroModel cm = new DipEsteroModel()
                                        {
                                             codicesede = dr["SED_COD_SEDE"].ToString(),
                                             descrizione = dr["SED_DESCRIZIONE"].ToString()
                                        };

                                        lcm.Add(cm);

                                    }
                                }
                            }

                        }
                    }
                    
                }

                return lcm;


            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        // Combo Qualifiche
        // Select Distinct IBS_DESCRIZIONE   From INDENNITABASE  Order By IBS_DESCRIZIONE
        public static IList<DipEsteroLivelloModel> GetAllQualifiche()
        {
            List<DipEsteroLivelloModel> lcm = new List<DipEsteroLivelloModel>();

            try
            {

                using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
                {
                    string query = string.Format("Select Distinct IBS_COD_QUALIFICA AS COD_QUALIFICA, IBS_DESCRIZIONE From INDENNITABASE Order By IBS_DESCRIZIONE");
                    {
                        using (OracleCommand cmd1 = new OracleCommand(query, cn))
                        {
                            cmd1.Connection.Open();
                            // Ampoooo vuoto
                            DipEsteroLivelloModel cm1 = new DipEsteroLivelloModel()
                            {
                                codicequalifica = "",
                                qualifica = ""
                            };
                            lcm.Add(cm1);


                            using (OracleDataReader dr = cmd1.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                if (dr.HasRows)
                                {
                                    while (dr.Read())
                                    {
                                        DipEsteroLivelloModel cm = new DipEsteroLivelloModel()
                                        {
                                             codicequalifica = dr["COD_QUALIFICA"].ToString(),
                                             qualifica = dr["IBS_DESCRIZIONE"].ToString()
                                        };

                                        lcm.Add(cm);

                                    }
                                }
                            }

                        }
                    }

                }

                return lcm;


            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public static IList<CodiceCoanModel> GetAllCostiCoan()
        {
            List<CodiceCoanModel> lcm = new List<CodiceCoanModel>();

            try
            {

                using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
                {
                    string query = string.Format("SELECT DISTINCT DECODE(T.PRO_COAN, 'S', 'Serv. Ist.', T.PRO_COAN) COAN    FROM SERVIZIPROMOZIONALI T   WHERE T.PRO_COAN <> 'S'   ORDER BY COAN");
                    {
                        using (OracleCommand cmd1 = new OracleCommand(query, cn))
                        {
                            cmd1.Connection.Open();

                            using (OracleDataReader dr = cmd1.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                if (dr.HasRows)
                                {
                                    while (dr.Read())
                                    {
                                        CodiceCoanModel cm = new CodiceCoanModel()
                                        {
                                            codicecoan = dr["COAN"].ToString()
                                        };

                                        lcm.Add(cm);

                                    }
                                }
                            }

                        }
                    }

                }

                return lcm;


            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

    }
}