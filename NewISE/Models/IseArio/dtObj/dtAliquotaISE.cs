using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.IseArio.Model;
using Oracle.ManagedDataAccess.Client;

namespace NewISE.Models.IseArio.dtObj
{
    public class dtAliquotaISE : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Preleva l'aliquota ISE
        /// </summary>
        /// <param name="matricola">Matricola interessata</param>
        /// <param name="importo">Lordo previdenziale</param>
        /// <returns>Ritorna l'oggetto AliquotaIseModel</returns>
        public AliquotaIseModel GetAliquotaIse(int matricola, decimal importo)
        {

            string sqlStr = "SELECT T.Q_0401 COGNOME,\n" +
                            "       T.Q_0402 NOME,\n" +
                            "       NVL(ALIQUOTA_ISE(:matricola, :importo), 0) AS ALIQUOTA\n" +
                            "  FROM P_ANAGR T\n" +
                            " WHERE 1 = 1\n" +
                            "   AND T.NUM_IND = :matricola2";

            AliquotaIseModel ai = new AliquotaIseModel();

            try
            {
                using (Connessione conn = new Connessione())
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conn.OpenConnDBAzoto();
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.Add("matricola", OracleDbType.Int32).Value = matricola;
                        cmd.Parameters.Add("importo", OracleDbType.Decimal).Value = importo;
                        cmd.Parameters.Add("matricola2", OracleDbType.Int32).Value = matricola;

                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();

                                ai = new AliquotaIseModel()
                                {
                                    Matricola = matricola,
                                    Cognome = dr["COGNOME"].ToString(),
                                    Nome = dr["NOME"].ToString(),
                                    Aliquota = Convert.ToDecimal(dr["ALIQUOTA"])
                                };

                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ai;

        }


    }
}