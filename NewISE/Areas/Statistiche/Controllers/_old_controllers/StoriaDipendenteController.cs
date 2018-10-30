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
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class StoriaDipendenteController : Controller
    {
        // GET: Statistiche/StoriaDipendente
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult StoriaDipendente(string matricola="")
        {
            ViewBag.ListaCategorie = new List<SelectListItem>();
            List<CategoryModel> lcm = new List<CategoryModel>();
            List<SelectListItem> lr = new List<SelectListItem>();
            List<Stp_Storia_Dipendente> lsd = new List<Stp_Storia_Dipendente>();
            List<Stp_Storia_Dipendente> model = new List<Stp_Storia_Dipendente>();

            lcm = Dipendenti.GetAllDipendenti().ToList();

            if (lcm != null && lcm.Any())
            {
                foreach (var item in lcm)
                {
                    SelectListItem r = new SelectListItem()
                    {
                        Text = item.nominativo,
                        Value = item.matricola
                    };

                    lr.Add(r);

                }
                if (matricola == string.Empty)
                {
                    lr.First().Selected = true;
                    matricola = lr.First().Value;
                }
                else
                {
                    var lvr = lr.Where(a => a.Value == matricola);
                    if (lvr != null && lvr.Count() > 0)
                    {
                        var r = lvr.First();
                        r.Selected = true;

                    }
                }


                ViewBag.ListaCategorie = lr;
            }


            //return PartialView("StoriaDipendente", lsd);



            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {

                //String Sql = "SELECT DECODE (rec,1, z.livello, '') livello, DECODE(rec, 1, z.TIPO_MOVIMENTO, '') tipo_movimento, DECODE(rec, 1, z.IES_DT_DECORRENZA, '') data_decorrenza, DECODE(rec, 1, z.IES_DT_LETTERA, '') data_lettera, DECODE(rec, 1, z.COEFFICIENTE_DI_SEDE, '') coef_sede, DECODE(rec, 1, z.IES_PERC_DISAGIO, '') perc_disagio, DECODE(rec, 1, z.IES_PERC_ABBATTIMENTO, '') perc_spettante, DECODE(rec, 1, z.IES_PERC_CONIUGE, '') perc_Coniuge, DECODE(rec, 1, z.IES_PENSIONE, '') pensione, DECODE(rec, 1, z.IES_FIGLI, '') n_figli, DECODE(rec, 1, z.CAMBIO, '') TFR, z.descr, z.indennita, z.IES_COD_TIPO_MOVIMENTO ord2, z.IES_PROG_MOVIMENTO ord3, z.IES_DT_DECORRENZA ord1 FROM (SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, SED_DESCRIZIONE                       AS SEDE, VAL_DESCRIZIONE                       AS VALUTA,IBS_DESCRIZIONE                       AS LIVELLO, TMO_DESCRIZIONE_MOVIMENTO             AS TIPO_MOVIMENTO, IES_DT_DECORRENZA, IES_DT_LETTERA, ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, IES_PERC_DISAGIO, IES_PERC_ABBATTIMENTO, IES_PERC_CONIUGE, IES_PENSIONE, IES_FIGLI, ABS(IES_CAMBIO) AS CAMBIO, '1-Base  ' descr, IES_INDEN_BASE AS Indennita, IES_COD_TIPO_MOVIMENTO, IES_PROG_MOVIMENTO, AND_COGNOME || ' '|| AND_NOME NOMINATIVO, 1 rec FROM INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, INDENNITABASE, ANADIPE WHERE IES_COD_SEDE = SED_COD_SEDE AND IES_COD_VALUTA = VAL_COD_VALUTA AND IES_COD_QUALIFICA = IBS_COD_QUALIFICA AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO AND IES_MATRICOLA = AND_MATRICOLA AND IES_MATRICOLA = 3192 AND IES_FLAG_RICALCOLATO IS NULL UNION SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, SED_DESCRIZIONE                      AS SEDE, VAL_DESCRIZIONE                      AS VALUTA, IBS_DESCRIZIONE                      AS LIVELLO, TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, IES_DT_DECORRENZA, IES_DT_LETTERA, ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, IES_PERC_DISAGIO, IES_PERC_ABBATTIMENTO, IES_PERC_CONIUGE, IES_PENSIONE, IES_FIGLI, ABS(IES_CAMBIO) AS CAMBIO, '2-Pers.  ' descr, DECODE(IES_FLAG_VALUTA, 'E', IES_INDEN_PERS * IES_CAMBIO, IES_INDEN_PERS / IES_CAMBIO) AS Indennita, IES_COD_TIPO_MOVIMENTO, IES_PROG_MOVIMENTO, AND_COGNOME || ' ' || AND_NOME NOMINATIVO, 2 rec FROM INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, INDENNITABASE, ANADIPE WHERE IES_COD_SEDE = SED_COD_SEDE AND IES_COD_VALUTA = VAL_COD_VALUTA AND IES_COD_QUALIFICA = IBS_COD_QUALIFICA AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO AND IES_MATRICOLA = AND_MATRICOLA AND IES_MATRICOLA = 3192 AND IES_FLAG_RICALCOLATO  IS NULL UNION SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, SED_DESCRIZIONE                      AS SEDE, VAL_DESCRIZIONE                      AS VALUTA, IBS_DESCRIZIONE                      AS LIVELLO, TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, IES_DT_DECORRENZA, IES_DT_LETTERA, ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, IES_PERC_DISAGIO, IES_PERC_ABBATTIMENTO, IES_PERC_CONIUGE, IES_PENSIONE, IES_FIGLI, ABS(IES_CAMBIO) AS CAMBIO, '4-Sist/Rien.  ' descr, IES_INDEN_SIS_RIE AS Indennita, IES_COD_TIPO_MOVIMENTO, IES_PROG_MOVIMENTO, AND_COGNOME || ' ' || AND_NOME NOMINATIVO, 4 rec FROM INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, INDENNITABASE, ANADIPE WHERE IES_COD_SEDE = SED_COD_SEDE AND IES_COD_VALUTA = VAL_COD_VALUTA AND IES_COD_QUALIFICA = IBS_COD_QUALIFICA AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO AND IES_MATRICOLA = AND_MATRICOLA AND IES_MATRICOLA = 3192 AND IES_FLAG_RICALCOLATO  IS NULL UNION SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, SED_DESCRIZIONE                      AS SEDE, VAL_DESCRIZIONE                      AS VALUTA, IBS_DESCRIZIONE                      AS LIVELLO, TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, IES_DT_DECORRENZA, IES_DT_LETTERA, ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, IES_PERC_DISAGIO, IES_PERC_ABBATTIMENTO, IES_PERC_CONIUGE,IES_PENSIONE, IES_FIGLI, ABS(IES_CAMBIO) AS CAMBIO, '3-Anticipo  ' descr, IES_ANTICIPO AS Indennita, IES_COD_TIPO_MOVIMENTO, IES_PROG_MOVIMENTO, AND_COGNOME || ' ' || AND_NOME NOMINATIVO, 3 rec FROM INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, INDENNITABASE, ANADIPE WHERE IES_COD_SEDE = SED_COD_SEDE AND IES_COD_VALUTA = VAL_COD_VALUTA AND IES_COD_QUALIFICA = IBS_COD_QUALIFICA AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO AND IES_MATRICOLA = AND_MATRICOLA AND IES_MATRICOLA = 3192 AND IES_FLAG_RICALCOLATO  IS NULL UNION SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, SED_DESCRIZIONE                      AS SEDE, VAL_DESCRIZIONE                      AS VALUTA, IBS_DESCRIZIONE                      AS LIVELLO, TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, IES_DT_DECORRENZA, IES_DT_LETTERA, ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, IES_PERC_DISAGIO, IES_PERC_ABBATTIMENTO, IES_PERC_CONIUGE, IES_PENSIONE, IES_FIGLI, ABS(IES_CAMBIO) AS CAMBIO, '5-Sist.Netta  ' descr, DECODE(IES_FLAG_VALUTA, 'E', IES_INDEN_SIS_RIE_NETTA * IES_CAMBIO, IES_INDEN_SIS_RIE_NETTA / IES_CAMBIO) AS Indennita, IES_COD_TIPO_MOVIMENTO, IES_PROG_MOVIMENTO, AND_COGNOME || ' ' || AND_NOME NOMINATIVO, 5 rec FROM INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, INDENNITABASE, ANADIPE WHERE IES_COD_SEDE = SED_COD_SEDE AND IES_COD_VALUTA = VAL_COD_VALUTA AND IES_COD_QUALIFICA = IBS_COD_QUALIFICA AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO AND IES_MATRICOLA = AND_MATRICOLA AND IES_MATRICOLA = 3192 AND IES_FLAG_RICALCOLATO  IS NULL ) z ORDER BY z.IES_DT_DECORRENZA, z.IES_COD_TIPO_MOVIMENTO, z.IES_PROG_MOVIMENTO, z.descr";
                #region Query gigius

                String Sql = "SELECT DECODE (rec,1, z.nominativo, '') nominativo, ";
                Sql += "DECODE (rec,1, z.ies_matricola, '') matricola, ";
                Sql += "DECODE (rec,1, z.livello, '') livello, ";
                Sql += "DECODE(rec, 1, z.TIPO_MOVIMENTO, '') tipo_movimento, ";
                Sql += "DECODE(rec, 1, z.IES_DT_DECORRENZA, '') data_decorrenza, ";
                Sql += "DECODE(rec, 1, z.IES_DT_LETTERA, '') data_lettera, ";
                Sql += "DECODE(rec, 1, z.COEFFICIENTE_DI_SEDE, '') coef_sede, ";
                Sql += "DECODE(rec, 1, z.IES_PERC_DISAGIO, '') perc_disagio, ";
                Sql += "DECODE(rec, 1, z.IES_PERC_ABBATTIMENTO, '') perc_spettante, ";
                Sql += "DECODE(rec, 1, z.IES_PERC_CONIUGE, '') perc_Coniuge, ";
                Sql += "DECODE(rec, 1, z.IES_PENSIONE, '') pensione, ";
                Sql += "DECODE(rec, 1, z.IES_FIGLI, '') n_figli, ";
                Sql += "DECODE(rec, 1, z.CAMBIO, '') TFR, ";
                Sql += "z.descr, ";
                Sql += "ROUND(z.indennita,2) indennita, ";
                Sql += "z.IES_COD_TIPO_MOVIMENTO ord2, ";
                Sql += "z.IES_PROG_MOVIMENTO ord3, ";
                Sql += "z.IES_DT_DECORRENZA ord1 ";
                Sql += "FROM ";
                Sql += "(SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
                Sql += "SED_DESCRIZIONE                       AS SEDE, ";
                Sql += "VAL_DESCRIZIONE                       AS VALUTA, ";
                Sql += "IBS_DESCRIZIONE                       AS LIVELLO, ";
                Sql += "TMO_DESCRIZIONE_MOVIMENTO             AS TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "IES_DT_LETTERA, ";
                Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
                Sql += "IES_PERC_DISAGIO, ";
                Sql += "IES_PERC_ABBATTIMENTO, ";
                Sql += "IES_PERC_CONIUGE, ";
                Sql += "IES_PENSIONE, ";
                Sql += "IES_FIGLI, ";
                Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
                Sql += "'1-Base  ' descr, ";
                Sql += "IES_INDEN_BASE AS Indennita, ";
                Sql += "IES_COD_TIPO_MOVIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO, ";
                Sql += "IES_MATRICOLA, ";
                Sql += "AND_COGNOME ";
                Sql += "|| ' ' ";
                Sql += "|| AND_NOME NOMINATIVO, ";
                Sql += "1 rec ";
                Sql += "FROM INDESTERA, ";
                Sql += "TIPOMOVIMENTO, ";
                Sql += "SEDIESTERE, ";
                Sql += "VALUTE, ";
                Sql += "INDENNITABASE, ";
                Sql += "ANADIPE ";
                Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
                Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
                Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
                Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
                Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
                //Sql += "AND IES_MATRICOLA = " + matricola + " ";
                Sql += "AND IES_MATRICOLA = :matr ";
                Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
                Sql += "UNION ";
                Sql += "SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
                Sql += "SED_DESCRIZIONE                      AS SEDE, ";
                Sql += "VAL_DESCRIZIONE                      AS VALUTA, ";
                Sql += "IBS_DESCRIZIONE                      AS LIVELLO, ";
                Sql += "TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "IES_DT_LETTERA, ";
                Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
                Sql += "IES_PERC_DISAGIO, ";
                Sql += "IES_PERC_ABBATTIMENTO, ";
                Sql += "IES_PERC_CONIUGE, ";
                Sql += "IES_PENSIONE, ";
                Sql += "IES_FIGLI, ";
                Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
                Sql += "'2-Pers.  ' descr, ";
                Sql += "DECODE(IES_FLAG_VALUTA, 'E', IES_INDEN_PERS * IES_CAMBIO, IES_INDEN_PERS / IES_CAMBIO) AS Indennita, ";
                Sql += "IES_COD_TIPO_MOVIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO, ";
                Sql += "IES_MATRICOLA, ";
                Sql += "AND_COGNOME ";
                Sql += "|| ' ' ";
                Sql += "|| AND_NOME NOMINATIVO, ";
                Sql += "2 rec ";
                Sql += "FROM INDESTERA, ";
                Sql += "TIPOMOVIMENTO, ";
                Sql += "SEDIESTERE, ";
                Sql += "VALUTE, ";
                Sql += "INDENNITABASE, ";
                Sql += "ANADIPE ";
                Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
                Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
                Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
                Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
                Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
                //Sql += "AND IES_MATRICOLA = " + matricola + " ";
                Sql += "AND IES_MATRICOLA = :matr ";
                Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
                Sql += "UNION ";
                Sql += "SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
                Sql += "SED_DESCRIZIONE                      AS SEDE, ";
                Sql += "VAL_DESCRIZIONE                      AS VALUTA, ";
                Sql += "IBS_DESCRIZIONE                      AS LIVELLO, ";
                Sql += "TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "IES_DT_LETTERA, ";
                Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
                Sql += "IES_PERC_DISAGIO, ";
                Sql += "IES_PERC_ABBATTIMENTO, ";
                Sql += "IES_PERC_CONIUGE, ";
                Sql += "IES_PENSIONE, ";
                Sql += "IES_FIGLI, ";
                Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
                Sql += "'4-Sist/Rien.  ' descr, ";
                Sql += "IES_INDEN_SIS_RIE AS Indennita, ";
                Sql += "IES_COD_TIPO_MOVIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO, ";
                Sql += "IES_MATRICOLA, ";
                Sql += "AND_COGNOME ";
                Sql += "|| ' ' ";
                Sql += "|| AND_NOME NOMINATIVO, ";
                Sql += "4 rec ";
                Sql += "FROM INDESTERA, ";
                Sql += "TIPOMOVIMENTO, ";
                Sql += "SEDIESTERE, ";
                Sql += "VALUTE, ";
                Sql += "INDENNITABASE, ";
                Sql += "ANADIPE ";
                Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
                Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
                Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
                Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
                Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
                //Sql += "AND IES_MATRICOLA = " + matricola + " ";
                Sql += "AND IES_MATRICOLA = :matr ";
                Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
                Sql += "UNION ";
                Sql += "SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
                Sql += "SED_DESCRIZIONE                      AS SEDE, ";
                Sql += "VAL_DESCRIZIONE                      AS VALUTA, ";
                Sql += "IBS_DESCRIZIONE                      AS LIVELLO, ";
                Sql += "TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "IES_DT_LETTERA, ";
                Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
                Sql += "IES_PERC_DISAGIO, ";
                Sql += "IES_PERC_ABBATTIMENTO, ";
                Sql += "IES_PERC_CONIUGE, ";
                Sql += "IES_PENSIONE, ";
                Sql += "IES_FIGLI, ";
                Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
                Sql += "'3-Anticipo  ' descr, ";
                Sql += "IES_ANTICIPO AS Indennita, ";
                Sql += "IES_COD_TIPO_MOVIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO, ";
                Sql += "IES_MATRICOLA, ";
                Sql += "AND_COGNOME ";
                Sql += "|| ' ' ";
                Sql += "|| AND_NOME NOMINATIVO, ";
                Sql += "3 rec ";
                Sql += "FROM INDESTERA, ";
                Sql += "TIPOMOVIMENTO, ";
                Sql += "SEDIESTERE, ";
                Sql += "VALUTE, ";
                Sql += "INDENNITABASE, ";
                Sql += "ANADIPE ";
                Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
                Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
                Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
                Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
                Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
                //Sql += "AND IES_MATRICOLA = " + matricola + " ";
                Sql += "AND IES_MATRICOLA = :matr ";
                Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
                Sql += "UNION ";
                Sql += "SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
                Sql += "SED_DESCRIZIONE                      AS SEDE, ";
                Sql += "VAL_DESCRIZIONE                      AS VALUTA, ";
                Sql += "IBS_DESCRIZIONE                      AS LIVELLO, ";
                Sql += "TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "IES_DT_LETTERA, ";
                Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
                Sql += "IES_PERC_DISAGIO, ";
                Sql += "IES_PERC_ABBATTIMENTO, ";
                Sql += "IES_PERC_CONIUGE, ";
                Sql += "IES_PENSIONE, ";
                Sql += "IES_FIGLI, ";
                Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
                Sql += "'5-Sist.Netta  ' descr, ";
                Sql += "DECODE(IES_FLAG_VALUTA, 'E', IES_INDEN_SIS_RIE_NETTA * IES_CAMBIO, IES_INDEN_SIS_RIE_NETTA / IES_CAMBIO) AS Indennita, ";
                Sql += "IES_COD_TIPO_MOVIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO, ";
                Sql += "IES_MATRICOLA, ";
                Sql += "AND_COGNOME ";
                Sql += "|| ' ' ";
                Sql += "|| AND_NOME NOMINATIVO, ";
                Sql += "5 rec ";
                Sql += "FROM INDESTERA, ";
                Sql += "TIPOMOVIMENTO, ";
                Sql += "SEDIESTERE, ";
                Sql += "VALUTE, ";
                Sql += "INDENNITABASE, ";
                Sql += "ANADIPE ";
                Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
                Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
                Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
                Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
                Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
                //Sql += "AND IES_MATRICOLA = " + matricola + " ";
                Sql += "AND IES_MATRICOLA = :matr ";
                Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
                Sql += ") z ";
                Sql += "ORDER BY z.nominativo, ";
                Sql += "z.ies_matricola, ";
                Sql += "z.IES_DT_DECORRENZA, ";
                Sql += "z.IES_COD_TIPO_MOVIMENTO, ";
                Sql += "z.IES_PROG_MOVIMENTO, ";
                Sql += "z.descr ";
                #endregion

                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = Sql;
                    cmd.Parameters.Add("matr", matricola);
                    cmd.Connection.Open();

                    using (OracleDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                var details = new Stp_Storia_Dipendente();
                                details.NOMINATIVO = rdr["NOMINATIVO"].ToString();
                                details.MATRICOLA = rdr["MATRICOLA"].ToString();
                                details.LIVELLO = rdr["LIVELLO"].ToString();
                                details.TIPO_MOVIMENTO = rdr["TIPO_MOVIMENTO"].ToString();
                                details.DATA_DECORRENZA = rdr["DATA_DECORRENZA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_DECORRENZA"]).ToString("dd/MM/yyyy");
                                //details.DATA_DECORRENZA = Convert.ToDateTime(rdr["DATA_DECORRENZA"]).ToString("dd/MM/yyyy");
                                //details.DATA_DECORRENZA = rdr["DATA_DECORRENZA"].ToString();
                                //details.DATA_LETTERA = rdr["DATA_LETTERA"].ToString();
                                details.DATA_LETTERA = rdr["DATA_LETTERA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_LETTERA"]).ToString("dd/MM/yyyy");
                                //details.DATA_LETTERA = Convert.ToDateTime(rdr["DATA_LETTERA"]).ToString("dd/MM/yyyy");

                                details.COEF_SEDE = rdr["COEF_SEDE"].ToString();
                                details.PERC_DISAGIO = rdr["PERC_DISAGIO"].ToString();
                                details.PERC_SPETTANTE = rdr["PERC_SPETTANTE"].ToString();
                                details.PERC_CONIUGE = rdr["PERC_CONIUGE"].ToString();
                                details.PENSIONE = rdr["PENSIONE"].ToString();
                                details.N_FIGLI = rdr["N_FIGLI"].ToString();
                                details.TFR = rdr["TFR"].ToString();
                                details.DESCR = rdr["DESCR"].ToString();
                                details.INDENNITA = rdr["INDENNITA"].ToString();
                                details.ORD1 = rdr["ord1"].ToString();
                                details.ORD2 = rdr["ord2"].ToString();
                                details.ORD3 = rdr["ord3"].ToString();
                                model.Add(details);
                            }
                        }
                    }

                }

                return PartialView("StoriaDipendente", model);
            }

        }
        //public ActionResult RptStoriaDipendente(string matricola = "", string V_DATA = "")
        //{
        //    DataSet6 ds6 = new DataSet6();
        //    try
        //    {

        //        ReportViewer reportViewer = new ReportViewer();
        //        reportViewer.ProcessingMode = ProcessingMode.Local;
        //        reportViewer.SizeToReportContent = true;
        //        reportViewer.Width = Unit.Percentage(100);
        //        reportViewer.Height = Unit.Percentage(100);


        //        var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

        //        OracleConnection conx = new OracleConnection(connectionString);
        //        #region MyRegion

        //        String Sql = "SELECT DECODE (rec,1, z.nominativo, '') nominativo, ";
        //        Sql += "DECODE (rec,1, z.ies_matricola, '') matricola, ";
        //        Sql += "DECODE (rec,1, z.livello, '') livello, ";
        //        Sql += "DECODE(rec, 1, z.TIPO_MOVIMENTO, '') tipo_movimento, ";
        //        Sql += "DECODE(rec, 1, z.IES_DT_DECORRENZA, '') data_decorrenza, ";
        //        Sql += "DECODE(rec, 1, z.IES_DT_LETTERA, '') data_lettera, ";
        //        Sql += "DECODE(rec, 1, z.COEFFICIENTE_DI_SEDE, '') coef_sede, ";
        //        Sql += "DECODE(rec, 1, z.IES_PERC_DISAGIO, '') perc_disagio, ";
        //        Sql += "DECODE(rec, 1, z.IES_PERC_ABBATTIMENTO, '') perc_spettante, ";
        //        Sql += "DECODE(rec, 1, z.IES_PERC_CONIUGE, '') perc_Coniuge, ";
        //        Sql += "DECODE(rec, 1, z.IES_PENSIONE, '') pensione, ";
        //        Sql += "DECODE(rec, 1, z.IES_FIGLI, '') n_figli, ";
        //        Sql += "DECODE(rec, 1, z.CAMBIO, '') TFR, ";
        //        Sql += "z.descr, ";
        //        Sql += "ROUND(z.indennita,2) indennita, ";
        //        Sql += "z.IES_COD_TIPO_MOVIMENTO ord2, ";
        //        Sql += "z.IES_PROG_MOVIMENTO ord3, ";
        //        Sql += "z.IES_DT_DECORRENZA ord1 ";
        //        Sql += "FROM ";
        //        Sql += "(SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
        //        Sql += "SED_DESCRIZIONE                       AS SEDE, ";
        //        Sql += "VAL_DESCRIZIONE                       AS VALUTA, ";
        //        Sql += "IBS_DESCRIZIONE                       AS LIVELLO, ";
        //        Sql += "TMO_DESCRIZIONE_MOVIMENTO             AS TIPO_MOVIMENTO, ";
        //        Sql += "IES_DT_DECORRENZA, ";
        //        Sql += "IES_DT_LETTERA, ";
        //        Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
        //        Sql += "IES_PERC_DISAGIO, ";
        //        Sql += "IES_PERC_ABBATTIMENTO, ";
        //        Sql += "IES_PERC_CONIUGE, ";
        //        Sql += "IES_PENSIONE, ";
        //        Sql += "IES_FIGLI, ";
        //        Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
        //        Sql += "'1-Base  ' descr, ";
        //        Sql += "IES_INDEN_BASE AS Indennita, ";
        //        Sql += "IES_COD_TIPO_MOVIMENTO, ";
        //        Sql += "IES_PROG_MOVIMENTO, ";
        //        Sql += "IES_MATRICOLA, ";
        //        Sql += "AND_COGNOME ";
        //        Sql += "|| ' ' ";
        //        Sql += "|| AND_NOME NOMINATIVO, ";
        //        Sql += "1 rec ";
        //        Sql += "FROM INDESTERA, ";
        //        Sql += "TIPOMOVIMENTO, ";
        //        Sql += "SEDIESTERE, ";
        //        Sql += "VALUTE, ";
        //        Sql += "INDENNITABASE, ";
        //        Sql += "ANADIPE ";
        //        Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
        //        Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
        //        Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
        //        Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
        //        Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
        //        Sql += "AND IES_MATRICOLA = " + matricola + " ";
        //        Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
        //        Sql += "UNION ";
        //        Sql += "SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
        //        Sql += "SED_DESCRIZIONE                      AS SEDE, ";
        //        Sql += "VAL_DESCRIZIONE                      AS VALUTA, ";
        //        Sql += "IBS_DESCRIZIONE                      AS LIVELLO, ";
        //        Sql += "TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, ";
        //        Sql += "IES_DT_DECORRENZA, ";
        //        Sql += "IES_DT_LETTERA, ";
        //        Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
        //        Sql += "IES_PERC_DISAGIO, ";
        //        Sql += "IES_PERC_ABBATTIMENTO, ";
        //        Sql += "IES_PERC_CONIUGE, ";
        //        Sql += "IES_PENSIONE, ";
        //        Sql += "IES_FIGLI, ";
        //        Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
        //        Sql += "'2-Pers.  ' descr, ";
        //        Sql += "DECODE(IES_FLAG_VALUTA, 'E', IES_INDEN_PERS * IES_CAMBIO, IES_INDEN_PERS / IES_CAMBIO) AS Indennita, ";
        //        Sql += "IES_COD_TIPO_MOVIMENTO, ";
        //        Sql += "IES_PROG_MOVIMENTO, ";
        //        Sql += "IES_MATRICOLA, ";
        //        Sql += "AND_COGNOME ";
        //        Sql += "|| ' ' ";
        //        Sql += "|| AND_NOME NOMINATIVO, ";
        //        Sql += "2 rec ";
        //        Sql += "FROM INDESTERA, ";
        //        Sql += "TIPOMOVIMENTO, ";
        //        Sql += "SEDIESTERE, ";
        //        Sql += "VALUTE, ";
        //        Sql += "INDENNITABASE, ";
        //        Sql += "ANADIPE ";
        //        Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
        //        Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
        //        Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
        //        Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
        //        Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
        //        Sql += "AND IES_MATRICOLA = " + matricola + " ";
        //        Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
        //        Sql += "UNION ";
        //        Sql += "SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
        //        Sql += "SED_DESCRIZIONE                      AS SEDE, ";
        //        Sql += "VAL_DESCRIZIONE                      AS VALUTA, ";
        //        Sql += "IBS_DESCRIZIONE                      AS LIVELLO, ";
        //        Sql += "TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, ";
        //        Sql += "IES_DT_DECORRENZA, ";
        //        Sql += "IES_DT_LETTERA, ";
        //        Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
        //        Sql += "IES_PERC_DISAGIO, ";
        //        Sql += "IES_PERC_ABBATTIMENTO, ";
        //        Sql += "IES_PERC_CONIUGE, ";
        //        Sql += "IES_PENSIONE, ";
        //        Sql += "IES_FIGLI, ";
        //        Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
        //        Sql += "'4-Sist/Rien.  ' descr, ";
        //        Sql += "IES_INDEN_SIS_RIE AS Indennita, ";
        //        Sql += "IES_COD_TIPO_MOVIMENTO, ";
        //        Sql += "IES_PROG_MOVIMENTO, ";
        //        Sql += "IES_MATRICOLA, ";
        //        Sql += "AND_COGNOME ";
        //        Sql += "|| ' ' ";
        //        Sql += "|| AND_NOME NOMINATIVO, ";
        //        Sql += "4 rec ";
        //        Sql += "FROM INDESTERA, ";
        //        Sql += "TIPOMOVIMENTO, ";
        //        Sql += "SEDIESTERE, ";
        //        Sql += "VALUTE, ";
        //        Sql += "INDENNITABASE, ";
        //        Sql += "ANADIPE ";
        //        Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
        //        Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
        //        Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
        //        Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
        //        Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
        //        Sql += "AND IES_MATRICOLA = " + matricola + " ";
        //        Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
        //        Sql += "UNION ";
        //        Sql += "SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
        //        Sql += "SED_DESCRIZIONE                      AS SEDE, ";
        //        Sql += "VAL_DESCRIZIONE                      AS VALUTA, ";
        //        Sql += "IBS_DESCRIZIONE                      AS LIVELLO, ";
        //        Sql += "TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, ";
        //        Sql += "IES_DT_DECORRENZA, ";
        //        Sql += "IES_DT_LETTERA, ";
        //        Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
        //        Sql += "IES_PERC_DISAGIO, ";
        //        Sql += "IES_PERC_ABBATTIMENTO, ";
        //        Sql += "IES_PERC_CONIUGE, ";
        //        Sql += "IES_PENSIONE, ";
        //        Sql += "IES_FIGLI, ";
        //        Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
        //        Sql += "'3-Anticipo  ' descr, ";
        //        Sql += "IES_ANTICIPO AS Indennita, ";
        //        Sql += "IES_COD_TIPO_MOVIMENTO, ";
        //        Sql += "IES_PROG_MOVIMENTO, ";
        //        Sql += "IES_MATRICOLA, ";
        //        Sql += "AND_COGNOME ";
        //        Sql += "|| ' ' ";
        //        Sql += "|| AND_NOME NOMINATIVO, ";
        //        Sql += "3 rec ";
        //        Sql += "FROM INDESTERA, ";
        //        Sql += "TIPOMOVIMENTO, ";
        //        Sql += "SEDIESTERE, ";
        //        Sql += "VALUTE, ";
        //        Sql += "INDENNITABASE, ";
        //        Sql += "ANADIPE ";
        //        Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
        //        Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
        //        Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
        //        Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
        //        Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
        //        Sql += "AND IES_MATRICOLA = " + matricola + " ";
        //        Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
        //        Sql += "UNION ";
        //        Sql += "SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
        //        Sql += "SED_DESCRIZIONE                      AS SEDE, ";
        //        Sql += "VAL_DESCRIZIONE                      AS VALUTA, ";
        //        Sql += "IBS_DESCRIZIONE                      AS LIVELLO, ";
        //        Sql += "TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, ";
        //        Sql += "IES_DT_DECORRENZA, ";
        //        Sql += "IES_DT_LETTERA, ";
        //        Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
        //        Sql += "IES_PERC_DISAGIO, ";
        //        Sql += "IES_PERC_ABBATTIMENTO, ";
        //        Sql += "IES_PERC_CONIUGE, ";
        //        Sql += "IES_PENSIONE, ";
        //        Sql += "IES_FIGLI, ";
        //        Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
        //        Sql += "'5-Sist.Netta  ' descr, ";
        //        Sql += "DECODE(IES_FLAG_VALUTA, 'E', IES_INDEN_SIS_RIE_NETTA * IES_CAMBIO, IES_INDEN_SIS_RIE_NETTA / IES_CAMBIO) AS Indennita, ";
        //        Sql += "IES_COD_TIPO_MOVIMENTO, ";
        //        Sql += "IES_PROG_MOVIMENTO, ";
        //        Sql += "IES_MATRICOLA, ";
        //        Sql += "AND_COGNOME ";
        //        Sql += "|| ' ' ";
        //        Sql += "|| AND_NOME NOMINATIVO, ";
        //        Sql += "5 rec ";
        //        Sql += "FROM INDESTERA, ";
        //        Sql += "TIPOMOVIMENTO, ";
        //        Sql += "SEDIESTERE, ";
        //        Sql += "VALUTE, ";
        //        Sql += "INDENNITABASE, ";
        //        Sql += "ANADIPE ";
        //        Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
        //        Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
        //        Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
        //        Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
        //        Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
        //        Sql += "AND IES_MATRICOLA = " + matricola + " ";
        //        Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
        //        Sql += ") z ";
        //        Sql += "ORDER BY z.nominativo, ";
        //        Sql += "z.ies_matricola, ";
        //        Sql += "z.IES_DT_DECORRENZA, ";
        //        Sql += "z.IES_COD_TIPO_MOVIMENTO, ";
        //        Sql += "z.IES_PROG_MOVIMENTO, ";
        //        Sql += "z.descr ";
        //        #endregion

        //        OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

        //        //adp.Fill(ds6, ds6.STP_STORIA_DIP_ISESTOR.TableName);
        //        adp.Fill(ds6, ds6.DataTable6.TableName);

        //        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptStoriaDipendente.rdlc";
        //        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet6", ds6.Tables[0]));

        //        ReportParameter[] parameterValues = new ReportParameter[]
        //        {
        //            //new ReportParameter ("fromDate","Test"),
        //            new ReportParameter ("fromDate",V_DATA),
        //            new ReportParameter ("toDate","Test2")
        //        };

        //        reportViewer.LocalReport.SetParameters(parameterValues);
        //        reportViewer.LocalReport.Refresh();

        //        ViewBag.ReportViewer = reportViewer;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //    return View();
        //}
    }
}