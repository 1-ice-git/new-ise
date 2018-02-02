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
    public class ArchivioVecchioISEController : Controller
    {
        // GET: Statistiche/ArchivioVecchioISE
        public ActionResult Index()
        {

            var t = new List<SelectListItem>();

            try
            {
                t.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
                t.Add(new SelectListItem() { Text = "Consuntivo dei Costi", Value = "0" });
                t.Add(new SelectListItem() { Text = "Consuntivo dei Costi per Codice Co.An.", Value = "1" });
                t.Add(new SelectListItem() { Text = "Dislocazione dei dipendenti all'estero", Value = "2" });
                //t.Add(new SelectListItem() { Text = "Operazioni effettuate nel periodo", Value = "3" });
                t.Add(new SelectListItem() { Text = "Presenze dei livelli in servizio all'estero", Value = "4" });
                t.Add(new SelectListItem() { Text = "Spese diverse", Value = "5" });
                t.Add(new SelectListItem() { Text = "Spese di avvicendamento", Value = "6" });
                t.Add(new SelectListItem() { Text = "Storia del dipendente", Value = "7" });
                t.Add(new SelectListItem() { Text = "Operazioni effettuate - Indennità di Sede Estera", Value = "8" });
                t.Add(new SelectListItem() { Text = "Operazioni Effettuate - Contributo Abitazione", Value = "9" });
                t.Add(new SelectListItem() { Text = "Operazioni Effettuate - Uso Abitazione", Value = "10" });
                t.Add(new SelectListItem() { Text = "Operazioni Effettuate - Canone Anticipato", Value = "11" });
                t.Add(new SelectListItem() { Text = "Operazioni Effettuate - Spese Diverse", Value = "12" });
                t.Add(new SelectListItem() { Text = "Operazioni Effettuate - Maggiorazione Abitazione", Value = "13" });
                

                ViewBag.VecchioIse = t;
                return PartialView();

            }
            catch (Exception ex)
            {
                return View("Error");

            }



        }

        // Storia del Dipendente
        //public ActionResult StoriaDipendente(string matricola)
        //{
        //    try
        //    {
        //        //using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
        //        //{
                    
        //        //    //String Sql = "SELECT DECODE (rec,1, z.livello, '') livello, DECODE(rec, 1, z.TIPO_MOVIMENTO, '') tipo_movimento, DECODE(rec, 1, z.IES_DT_DECORRENZA, '') data_decorrenza, DECODE(rec, 1, z.IES_DT_LETTERA, '') data_lettera, DECODE(rec, 1, z.COEFFICIENTE_DI_SEDE, '') coef_sede, DECODE(rec, 1, z.IES_PERC_DISAGIO, '') perc_disagio, DECODE(rec, 1, z.IES_PERC_ABBATTIMENTO, '') perc_spettante, DECODE(rec, 1, z.IES_PERC_CONIUGE, '') perc_Coniuge, DECODE(rec, 1, z.IES_PENSIONE, '') pensione, DECODE(rec, 1, z.IES_FIGLI, '') n_figli, DECODE(rec, 1, z.CAMBIO, '') TFR, z.descr, z.indennita, z.IES_COD_TIPO_MOVIMENTO ord2, z.IES_PROG_MOVIMENTO ord3, z.IES_DT_DECORRENZA ord1 FROM (SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, SED_DESCRIZIONE                       AS SEDE, VAL_DESCRIZIONE                       AS VALUTA,IBS_DESCRIZIONE                       AS LIVELLO, TMO_DESCRIZIONE_MOVIMENTO             AS TIPO_MOVIMENTO, IES_DT_DECORRENZA, IES_DT_LETTERA, ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, IES_PERC_DISAGIO, IES_PERC_ABBATTIMENTO, IES_PERC_CONIUGE, IES_PENSIONE, IES_FIGLI, ABS(IES_CAMBIO) AS CAMBIO, '1-Base  ' descr, IES_INDEN_BASE AS Indennita, IES_COD_TIPO_MOVIMENTO, IES_PROG_MOVIMENTO, AND_COGNOME || ' '|| AND_NOME NOMINATIVO, 1 rec FROM INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, INDENNITABASE, ANADIPE WHERE IES_COD_SEDE = SED_COD_SEDE AND IES_COD_VALUTA = VAL_COD_VALUTA AND IES_COD_QUALIFICA = IBS_COD_QUALIFICA AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO AND IES_MATRICOLA = AND_MATRICOLA AND IES_MATRICOLA = 3192 AND IES_FLAG_RICALCOLATO IS NULL UNION SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, SED_DESCRIZIONE                      AS SEDE, VAL_DESCRIZIONE                      AS VALUTA, IBS_DESCRIZIONE                      AS LIVELLO, TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, IES_DT_DECORRENZA, IES_DT_LETTERA, ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, IES_PERC_DISAGIO, IES_PERC_ABBATTIMENTO, IES_PERC_CONIUGE, IES_PENSIONE, IES_FIGLI, ABS(IES_CAMBIO) AS CAMBIO, '2-Pers.  ' descr, DECODE(IES_FLAG_VALUTA, 'E', IES_INDEN_PERS * IES_CAMBIO, IES_INDEN_PERS / IES_CAMBIO) AS Indennita, IES_COD_TIPO_MOVIMENTO, IES_PROG_MOVIMENTO, AND_COGNOME || ' ' || AND_NOME NOMINATIVO, 2 rec FROM INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, INDENNITABASE, ANADIPE WHERE IES_COD_SEDE = SED_COD_SEDE AND IES_COD_VALUTA = VAL_COD_VALUTA AND IES_COD_QUALIFICA = IBS_COD_QUALIFICA AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO AND IES_MATRICOLA = AND_MATRICOLA AND IES_MATRICOLA = 3192 AND IES_FLAG_RICALCOLATO  IS NULL UNION SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, SED_DESCRIZIONE                      AS SEDE, VAL_DESCRIZIONE                      AS VALUTA, IBS_DESCRIZIONE                      AS LIVELLO, TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, IES_DT_DECORRENZA, IES_DT_LETTERA, ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, IES_PERC_DISAGIO, IES_PERC_ABBATTIMENTO, IES_PERC_CONIUGE, IES_PENSIONE, IES_FIGLI, ABS(IES_CAMBIO) AS CAMBIO, '4-Sist/Rien.  ' descr, IES_INDEN_SIS_RIE AS Indennita, IES_COD_TIPO_MOVIMENTO, IES_PROG_MOVIMENTO, AND_COGNOME || ' ' || AND_NOME NOMINATIVO, 4 rec FROM INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, INDENNITABASE, ANADIPE WHERE IES_COD_SEDE = SED_COD_SEDE AND IES_COD_VALUTA = VAL_COD_VALUTA AND IES_COD_QUALIFICA = IBS_COD_QUALIFICA AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO AND IES_MATRICOLA = AND_MATRICOLA AND IES_MATRICOLA = 3192 AND IES_FLAG_RICALCOLATO  IS NULL UNION SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, SED_DESCRIZIONE                      AS SEDE, VAL_DESCRIZIONE                      AS VALUTA, IBS_DESCRIZIONE                      AS LIVELLO, TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, IES_DT_DECORRENZA, IES_DT_LETTERA, ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, IES_PERC_DISAGIO, IES_PERC_ABBATTIMENTO, IES_PERC_CONIUGE,IES_PENSIONE, IES_FIGLI, ABS(IES_CAMBIO) AS CAMBIO, '3-Anticipo  ' descr, IES_ANTICIPO AS Indennita, IES_COD_TIPO_MOVIMENTO, IES_PROG_MOVIMENTO, AND_COGNOME || ' ' || AND_NOME NOMINATIVO, 3 rec FROM INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, INDENNITABASE, ANADIPE WHERE IES_COD_SEDE = SED_COD_SEDE AND IES_COD_VALUTA = VAL_COD_VALUTA AND IES_COD_QUALIFICA = IBS_COD_QUALIFICA AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO AND IES_MATRICOLA = AND_MATRICOLA AND IES_MATRICOLA = 3192 AND IES_FLAG_RICALCOLATO  IS NULL UNION SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, SED_DESCRIZIONE                      AS SEDE, VAL_DESCRIZIONE                      AS VALUTA, IBS_DESCRIZIONE                      AS LIVELLO, TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, IES_DT_DECORRENZA, IES_DT_LETTERA, ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, IES_PERC_DISAGIO, IES_PERC_ABBATTIMENTO, IES_PERC_CONIUGE, IES_PENSIONE, IES_FIGLI, ABS(IES_CAMBIO) AS CAMBIO, '5-Sist.Netta  ' descr, DECODE(IES_FLAG_VALUTA, 'E', IES_INDEN_SIS_RIE_NETTA * IES_CAMBIO, IES_INDEN_SIS_RIE_NETTA / IES_CAMBIO) AS Indennita, IES_COD_TIPO_MOVIMENTO, IES_PROG_MOVIMENTO, AND_COGNOME || ' ' || AND_NOME NOMINATIVO, 5 rec FROM INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, INDENNITABASE, ANADIPE WHERE IES_COD_SEDE = SED_COD_SEDE AND IES_COD_VALUTA = VAL_COD_VALUTA AND IES_COD_QUALIFICA = IBS_COD_QUALIFICA AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO AND IES_MATRICOLA = AND_MATRICOLA AND IES_MATRICOLA = 3192 AND IES_FLAG_RICALCOLATO  IS NULL ) z ORDER BY z.IES_DT_DECORRENZA, z.IES_COD_TIPO_MOVIMENTO, z.IES_PROG_MOVIMENTO, z.descr";
                    
        //        //    String Sql = "SELECT DECODE (rec,1, z.nominativo, '') nominativo, ";
        //        //    Sql += "DECODE (rec,1, z.ies_matricola, '') matricola, ";
        //        //    Sql += "DECODE (rec,1, z.livello, '') livello, ";
        //        //    Sql += "DECODE(rec, 1, z.TIPO_MOVIMENTO, '') tipo_movimento, ";
        //        //    Sql += "DECODE(rec, 1, z.IES_DT_DECORRENZA, '') data_decorrenza, ";
        //        //    Sql += "DECODE(rec, 1, z.IES_DT_LETTERA, '') data_lettera, ";
        //        //    Sql += "DECODE(rec, 1, z.COEFFICIENTE_DI_SEDE, '') coef_sede, ";
        //        //    Sql += "DECODE(rec, 1, z.IES_PERC_DISAGIO, '') perc_disagio, ";
        //        //    Sql += "DECODE(rec, 1, z.IES_PERC_ABBATTIMENTO, '') perc_spettante, ";
        //        //    Sql += "DECODE(rec, 1, z.IES_PERC_CONIUGE, '') perc_Coniuge, ";
        //        //    Sql += "DECODE(rec, 1, z.IES_PENSIONE, '') pensione, ";
        //        //    Sql += "DECODE(rec, 1, z.IES_FIGLI, '') n_figli, ";
        //        //    Sql += "DECODE(rec, 1, z.CAMBIO, '') TFR, ";
        //        //    Sql += "z.descr, ";
        //        //    Sql += "ROUND(z.indennita,2) indennita, ";
        //        //    Sql += "z.IES_COD_TIPO_MOVIMENTO ord2, ";
        //        //    Sql += "z.IES_PROG_MOVIMENTO ord3, ";
        //        //    Sql += "z.IES_DT_DECORRENZA ord1 ";
        //        //    Sql += "FROM ";
        //        //    Sql += "(SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
        //        //    Sql += "SED_DESCRIZIONE                       AS SEDE, ";
        //        //    Sql += "VAL_DESCRIZIONE                       AS VALUTA, ";
        //        //    Sql += "IBS_DESCRIZIONE                       AS LIVELLO, ";
        //        //    Sql += "TMO_DESCRIZIONE_MOVIMENTO             AS TIPO_MOVIMENTO, ";
        //        //    Sql += "IES_DT_DECORRENZA, ";
        //        //    Sql += "IES_DT_LETTERA, ";
        //        //    Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
        //        //    Sql += "IES_PERC_DISAGIO, ";
        //        //    Sql += "IES_PERC_ABBATTIMENTO, ";
        //        //    Sql += "IES_PERC_CONIUGE, ";
        //        //    Sql += "IES_PENSIONE, ";
        //        //    Sql += "IES_FIGLI, ";
        //        //    Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
        //        //    Sql += "'1-Base  ' descr, ";
        //        //    Sql += "IES_INDEN_BASE AS Indennita, ";
        //        //    Sql += "IES_COD_TIPO_MOVIMENTO, ";
        //        //    Sql += "IES_PROG_MOVIMENTO, ";
        //        //    Sql += "IES_MATRICOLA, ";
        //        //    Sql += "AND_COGNOME ";
        //        //    Sql += "|| ' ' ";
        //        //    Sql += "|| AND_NOME NOMINATIVO, ";
        //        //    Sql += "1 rec ";
        //        //    Sql += "FROM INDESTERA, ";
        //        //    Sql += "TIPOMOVIMENTO, ";
        //        //    Sql += "SEDIESTERE, ";
        //        //    Sql += "VALUTE, ";
        //        //    Sql += "INDENNITABASE, ";
        //        //    Sql += "ANADIPE ";
        //        //    Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
        //        //    Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
        //        //    Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
        //        //    Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
        //        //    Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
        //        //    Sql += "AND IES_MATRICOLA = " + matricola + " ";
        //        //    Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
        //        //    Sql += "UNION ";
        //        //    Sql += "SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
        //        //    Sql += "SED_DESCRIZIONE                      AS SEDE, ";
        //        //    Sql += "VAL_DESCRIZIONE                      AS VALUTA, ";
        //        //    Sql += "IBS_DESCRIZIONE                      AS LIVELLO, ";
        //        //    Sql += "TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, ";
        //        //    Sql += "IES_DT_DECORRENZA, ";
        //        //    Sql += "IES_DT_LETTERA, ";
        //        //    Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
        //        //    Sql += "IES_PERC_DISAGIO, ";
        //        //    Sql += "IES_PERC_ABBATTIMENTO, ";
        //        //    Sql += "IES_PERC_CONIUGE, ";
        //        //    Sql += "IES_PENSIONE, ";
        //        //    Sql += "IES_FIGLI, ";
        //        //    Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
        //        //    Sql += "'2-Pers.  ' descr, ";
        //        //    Sql += "DECODE(IES_FLAG_VALUTA, 'E', IES_INDEN_PERS * IES_CAMBIO, IES_INDEN_PERS / IES_CAMBIO) AS Indennita, ";
        //        //    Sql += "IES_COD_TIPO_MOVIMENTO, ";
        //        //    Sql += "IES_PROG_MOVIMENTO, ";
        //        //    Sql += "IES_MATRICOLA, ";
        //        //    Sql += "AND_COGNOME ";
        //        //    Sql += "|| ' ' ";
        //        //    Sql += "|| AND_NOME NOMINATIVO, ";
        //        //    Sql += "2 rec ";
        //        //    Sql += "FROM INDESTERA, ";
        //        //    Sql += "TIPOMOVIMENTO, ";
        //        //    Sql += "SEDIESTERE, ";
        //        //    Sql += "VALUTE, ";
        //        //    Sql += "INDENNITABASE, ";
        //        //    Sql += "ANADIPE ";
        //        //    Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
        //        //    Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
        //        //    Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
        //        //    Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
        //        //    Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
        //        //    Sql += "AND IES_MATRICOLA = " + matricola + " ";
        //        //    Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
        //        //    Sql += "UNION ";
        //        //    Sql += "SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
        //        //    Sql += "SED_DESCRIZIONE                      AS SEDE, ";
        //        //    Sql += "VAL_DESCRIZIONE                      AS VALUTA, ";
        //        //    Sql += "IBS_DESCRIZIONE                      AS LIVELLO, ";
        //        //    Sql += "TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, ";
        //        //    Sql += "IES_DT_DECORRENZA, ";
        //        //    Sql += "IES_DT_LETTERA, ";
        //        //    Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
        //        //    Sql += "IES_PERC_DISAGIO, ";
        //        //    Sql += "IES_PERC_ABBATTIMENTO, ";
        //        //    Sql += "IES_PERC_CONIUGE, ";
        //        //    Sql += "IES_PENSIONE, ";
        //        //    Sql += "IES_FIGLI, ";
        //        //    Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
        //        //    Sql += "'4-Sist/Rien.  ' descr, ";
        //        //    Sql += "IES_INDEN_SIS_RIE AS Indennita, ";
        //        //    Sql += "IES_COD_TIPO_MOVIMENTO, ";
        //        //    Sql += "IES_PROG_MOVIMENTO, ";
        //        //    Sql += "IES_MATRICOLA, ";
        //        //    Sql += "AND_COGNOME ";
        //        //    Sql += "|| ' ' ";
        //        //    Sql += "|| AND_NOME NOMINATIVO, ";
        //        //    Sql += "4 rec ";
        //        //    Sql += "FROM INDESTERA, ";
        //        //    Sql += "TIPOMOVIMENTO, ";
        //        //    Sql += "SEDIESTERE, ";
        //        //    Sql += "VALUTE, ";
        //        //    Sql += "INDENNITABASE, ";
        //        //    Sql += "ANADIPE ";
        //        //    Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
        //        //    Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
        //        //    Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
        //        //    Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
        //        //    Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
        //        //    Sql += "AND IES_MATRICOLA = " + matricola + " ";
        //        //    Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
        //        //    Sql += "UNION ";
        //        //    Sql += "SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
        //        //    Sql += "SED_DESCRIZIONE                      AS SEDE, ";
        //        //    Sql += "VAL_DESCRIZIONE                      AS VALUTA, ";
        //        //    Sql += "IBS_DESCRIZIONE                      AS LIVELLO, ";
        //        //    Sql += "TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, ";
        //        //    Sql += "IES_DT_DECORRENZA, ";
        //        //    Sql += "IES_DT_LETTERA, ";
        //        //    Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
        //        //    Sql += "IES_PERC_DISAGIO, ";
        //        //    Sql += "IES_PERC_ABBATTIMENTO, ";
        //        //    Sql += "IES_PERC_CONIUGE, ";
        //        //    Sql += "IES_PENSIONE, ";
        //        //    Sql += "IES_FIGLI, ";
        //        //    Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
        //        //    Sql += "'3-Anticipo  ' descr, ";
        //        //    Sql += "IES_ANTICIPO AS Indennita, ";
        //        //    Sql += "IES_COD_TIPO_MOVIMENTO, ";
        //        //    Sql += "IES_PROG_MOVIMENTO, ";
        //        //    Sql += "IES_MATRICOLA, ";
        //        //    Sql += "AND_COGNOME ";
        //        //    Sql += "|| ' ' ";
        //        //    Sql += "|| AND_NOME NOMINATIVO, ";
        //        //    Sql += "3 rec ";
        //        //    Sql += "FROM INDESTERA, ";
        //        //    Sql += "TIPOMOVIMENTO, ";
        //        //    Sql += "SEDIESTERE, ";
        //        //    Sql += "VALUTE, ";
        //        //    Sql += "INDENNITABASE, ";
        //        //    Sql += "ANADIPE ";
        //        //    Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
        //        //    Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
        //        //    Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
        //        //    Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
        //        //    Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
        //        //    Sql += "AND IES_MATRICOLA = " + matricola + " ";
        //        //    Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
        //        //    Sql += "UNION ";
        //        //    Sql += "SELECT DISTINCT IES_PROG_TRASFERIMENTO AS TIPOLOGIA_TRASFERIMENTO, ";
        //        //    Sql += "SED_DESCRIZIONE                      AS SEDE, ";
        //        //    Sql += "VAL_DESCRIZIONE                      AS VALUTA, ";
        //        //    Sql += "IBS_DESCRIZIONE                      AS LIVELLO, ";
        //        //    Sql += "TMO_DESCRIZIONE_MOVIMENTO            AS TIPO_MOVIMENTO, ";
        //        //    Sql += "IES_DT_DECORRENZA, ";
        //        //    Sql += "IES_DT_LETTERA, ";
        //        //    Sql += "ABS(IES_COEFFICIENTE_SEDE) AS COEFFICIENTE_DI_SEDE, ";
        //        //    Sql += "IES_PERC_DISAGIO, ";
        //        //    Sql += "IES_PERC_ABBATTIMENTO, ";
        //        //    Sql += "IES_PERC_CONIUGE, ";
        //        //    Sql += "IES_PENSIONE, ";
        //        //    Sql += "IES_FIGLI, ";
        //        //    Sql += "ABS(IES_CAMBIO) AS CAMBIO, ";
        //        //    Sql += "'5-Sist.Netta  ' descr, ";
        //        //    Sql += "DECODE(IES_FLAG_VALUTA, 'E', IES_INDEN_SIS_RIE_NETTA * IES_CAMBIO, IES_INDEN_SIS_RIE_NETTA / IES_CAMBIO) AS Indennita, ";
        //        //    Sql += "IES_COD_TIPO_MOVIMENTO, ";
        //        //    Sql += "IES_PROG_MOVIMENTO, ";
        //        //    Sql += "IES_MATRICOLA, ";
        //        //    Sql += "AND_COGNOME ";
        //        //    Sql += "|| ' ' ";
        //        //    Sql += "|| AND_NOME NOMINATIVO, ";
        //        //    Sql += "5 rec ";
        //        //    Sql += "FROM INDESTERA, ";
        //        //    Sql += "TIPOMOVIMENTO, ";
        //        //    Sql += "SEDIESTERE, ";
        //        //    Sql += "VALUTE, ";
        //        //    Sql += "INDENNITABASE, ";
        //        //    Sql += "ANADIPE ";
        //        //    Sql += "WHERE IES_COD_SEDE         = SED_COD_SEDE ";
        //        //    Sql += "AND IES_COD_VALUTA         = VAL_COD_VALUTA ";
        //        //    Sql += "AND IES_COD_QUALIFICA      = IBS_COD_QUALIFICA ";
        //        //    Sql += "AND IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
        //        //    Sql += "AND IES_MATRICOLA          = AND_MATRICOLA ";
        //        //    Sql += "AND IES_MATRICOLA = " + matricola + " ";
        //        //    Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
        //        //    Sql += ") z ";
        //        //    Sql += "ORDER BY z.nominativo, ";
        //        //    Sql += "z.ies_matricola, ";
        //        //    Sql += "z.IES_DT_DECORRENZA, ";
        //        //    Sql += "z.IES_COD_TIPO_MOVIMENTO, ";
        //        //    Sql += "z.IES_PROG_MOVIMENTO, ";
        //        //    Sql += "z.descr ";

        //        //    OracleCommand cmd = new OracleCommand(Sql, cn);
        //        //    cn.Open();
        //        //    OracleDataReader rdr = cmd.ExecuteReader();
        //        //    List<Stp_Storia_Dipendente> model = new List<Stp_Storia_Dipendente>();
        //        //    while (rdr.Read())
        //        //    {
        //        //        var details = new Stp_Storia_Dipendente();
        //        //        details.NOMINATIVO = rdr["NOMINATIVO"].ToString();
        //        //        details.MATRICOLA = rdr["MATRICOLA"].ToString();
        //        //        details.LIVELLO = rdr["LIVELLO"].ToString();
        //        //        details.TIPO_MOVIMENTO = rdr["TIPO_MOVIMENTO"].ToString();
        //        //        details.DATA_DECORRENZA = rdr["DATA_DECORRENZA"].ToString();
        //        //        details.DATA_LETTERA = rdr["DATA_LETTERA"].ToString();
        //        //        details.COEF_SEDE = rdr["COEF_SEDE"].ToString();
        //        //        details.PERC_DISAGIO = rdr["PERC_DISAGIO"].ToString();
        //        //        details.PERC_SPETTANTE = rdr["PERC_SPETTANTE"].ToString();
        //        //        details.PERC_CONIUGE = rdr["PERC_CONIUGE"].ToString();
        //        //        details.PENSIONE = rdr["PENSIONE"].ToString();
        //        //        details.N_FIGLI = rdr["N_FIGLI"].ToString();
        //        //        details.TFR = rdr["TFR"].ToString();
        //        //        details.DESCR = rdr["DESCR"].ToString();
        //        //        details.INDENNITA = rdr["INDENNITA"].ToString();
        //        //        details.ORD1 = rdr["ord1"].ToString();
        //        //        details.ORD2 = rdr["ord2"].ToString();
        //        //        details.ORD3 = rdr["ord3"].ToString();
        //        //        model.Add(details);
        //        //    }



        //        //    //List<SelectListItem> li = new List<SelectListItem>();
        //        //    //string constr = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

        //        //    //using (OracleConnection con = new OracleConnection(constr))
        //        //    //{
        //        //    //    string query = "Select Distinct AND_MATRICOLA, AND_COGNOME ||' '|| AND_NOME NOMINATIVO From ANADIPE, TRASFERIMENTO Where AND_MATRICOLA = TRA_MATRICOLA Order By NOMINATIVO";
        //        //    //    OracleCommand cmd1 = new OracleCommand(query, con);
        //        //    //    con.Open();
        //        //    //    OracleDataReader sdr = cmd1.ExecuteReader();
        //        //    //    {
        //        //    //        while (sdr.Read())
        //        //    //        {
        //        //    //            li.Add(new SelectListItem
        //        //    //            {
        //        //    //                Text = sdr["NOMINATIVO"].ToString(),
        //        //    //                Value = sdr["AND_MATRICOLA"].ToString()

        //        //    //            });
        //        //    //        }

        //        //    //        ViewData["Stampe"] = li;

        //        //    //    }

        //        //    //    con.Close();
        //        //    //}

        //        //    return PartialView("StoriaDipendente", model);
        //        //}
                
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }


        //}
  
        public ActionResult RptStoriaDipendente(string matricola = "", string V_DATA="")
        {
            DataSet6 ds6 = new DataSet6();
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);
                
                
                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                #region MyRegion

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
                Sql += "AND IES_MATRICOLA = " + matricola + " ";
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
                Sql += "AND IES_MATRICOLA = " + matricola + " ";
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
                Sql += "AND IES_MATRICOLA = " + matricola + " ";
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
                Sql += "AND IES_MATRICOLA = " + matricola + " ";
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
                Sql += "AND IES_MATRICOLA = " + matricola + " ";
                Sql += "AND IES_FLAG_RICALCOLATO IS NULL ";
                Sql += ") z ";
                Sql += "ORDER BY z.nominativo, ";
                Sql += "z.ies_matricola, ";
                Sql += "z.IES_DT_DECORRENZA, ";
                Sql += "z.IES_COD_TIPO_MOVIMENTO, ";
                Sql += "z.IES_PROG_MOVIMENTO, ";
                Sql += "z.descr "; 
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);
                
                //adp.Fill(ds6, ds6.STP_STORIA_DIP_ISESTOR.TableName);
                adp.Fill(ds6, ds6.DataTable6.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptStoriaDipendente.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet6", ds6.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                {
                    //new ReportParameter ("fromDate","Test"),
                    new ReportParameter ("fromDate",V_DATA),
                    new ReportParameter ("toDate","Test2")
                };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();
        }
        
        public ActionResult BindWithViewBag()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Select Category", Value = "0", Selected = true });

            items.Add(new SelectListItem { Text = "Beverages", Value = "1" });

            items.Add(new SelectListItem { Text = "Condiments", Value = "2" });

            items.Add(new SelectListItem { Text = "Confections", Value = "3" });

            items.Add(new SelectListItem { Text = "Dairy Products", Value = "4" });

            items.Add(new SelectListItem { Text = "Grains/Cereals", Value = "5" });

            items.Add(new SelectListItem { Text = "Meat/Poultry", Value = "6" });

            items.Add(new SelectListItem { Text = "Produce", Value = "7" });

            items.Add(new SelectListItem { Text = "Seafood", Value = "8" });

            ViewBag.CategoryType = items;

            return View();
        }
        
        public ActionResult BindWithDbValues(string matricola = "")
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

        // Dislocazione dei Dipendenti all'Estero
        public ActionResult Dislocazione(string codicesede = "", string V_UTENTE = "", string V_DATA = "", string V_UFFICIO = "")
        {
            ViewBag.ListaDislocazioneDipEstero = new List<SelectListItem>();
            List<DipEsteroModel> lcm = new List<DipEsteroModel>();
            List<SelectListItem> lr = new List<SelectListItem>();
            List<Stp_Dislocazione_dipendenti> lsd = new List<Stp_Dislocazione_dipendenti>();
            List<Stp_Dislocazione_dipendenti> model = new List<Stp_Dislocazione_dipendenti>();

            lcm = Dipendenti.GetAllSedi().ToList();

            if (lcm != null && lcm.Any())
            {
                foreach (var item in lcm)
                {
                    SelectListItem r = new SelectListItem()
                    {
                        Text = item.descrizione,
                        Value = item.codicesede
                    };

                    lr.Add(r);

                }
                if (codicesede == string.Empty)
                {
                    //lr.First().Selected = true;
                    lr.First().Value = "";
                    codicesede = lr.First().Value;
                }
                else
                {
                    var lvr = lr.Where(a => a.Value == codicesede);
                    if (lvr != null && lvr.Count() > 0)
                    {
                        var r = lvr.First();
                        r.Selected = true;

                    }
                }


                ViewBag.ListaDislocazioneDipEstero = lr;
            }

            // Chiamata alla pagina Visual Basic -- Stampa_Elenco_Trasferimenti()
            //StampeISE.frmStampe xx = new frmStampe();
            //xx.Stampa_Consuntivo_Costi_x_Coan("E135A054B1", "01/03/2017", "01/03/2018", "fantomas");


            //StampeISE.Class1 xx = new Class1();
            //xx.Stampa_Elenco_Trasferimenti();
            
            
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                
                OracleCommand cmd1 = new OracleCommand();
                cmd1.Connection = cn;
                cmd1.CommandText = "ISE_STAMPA_ELENCO_TRASF";
                cmd1.CommandType = System.Data.CommandType.StoredProcedure;
                
                cmd1.Parameters.Add("@V_UTENTE", OracleDbType.Varchar2, 50).Value = V_UTENTE;
                cmd1.Parameters.Add("@V_DATA", OracleDbType.Varchar2, 50).Value = V_DATA;
                cmd1.Parameters.Add("@V_UFFICIO", OracleDbType.Varchar2, 50).Value = codicesede;
                
                cn.Open();
                cmd1.ExecuteNonQuery();
                
                //String Sql = "Select distinct SEDE, VALUTA, MATRICOLA, NOMINATIVO, DT_TRASFERIMENTO, QUALIFICA, CONIUGE, FIGLI, ISEP, CONTRIBUTO, USO, ISEP + CONTRIBUTO + USO TOTALE From ISE_STP_ELENCOTRASFERIMENTI, SEDIESTERE WHERE SEDIESTERE.SED_COD_SEDE = '" + codicesede + "' AND ISE_STP_ELENCOTRASFERIMENTI.SEDE = SEDIESTERE.SED_DESCRIZIONE Order By SEDE, NOMINATIVO";

                String Sql = "Select SEDE,VALUTA, ";
                Sql += "MATRICOLA, ";
                Sql += "NOMINATIVO, ";
                Sql += "DT_TRASFERIMENTO, ";
                Sql += "QUALIFICA, ";
                Sql += "CONIUGE, ";
                Sql += "FIGLI, ";
                Sql += "ISEP, ";
                Sql += "CONTRIBUTO, ";
                Sql += "USO, ";
                Sql += "ISEP +CONTRIBUTO + USO TOTALE ";
                Sql += "From ISE_STP_ELENCOTRASFERIMENTI ";
                Sql += "Where UTENTE = '" + V_UTENTE + "' ";
                Sql += "Order By SEDE, NOMINATIVO";



                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = Sql;
                    //cmd.Parameters.Add("codsede", codicesede);
                    //cmd.Connection.Open();

                    using (OracleDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                var details = new Stp_Dislocazione_dipendenti();
                                details.sede = rdr["SEDE"].ToString();
                                details.valuta = rdr["VALUTA"].ToString();
                                details.matricola = rdr["MATRICOLA"].ToString();
                                details.nominativo = rdr["NOMINATIVO"].ToString();
                                details.dataTrasferimento = Convert.ToDateTime(rdr["DT_TRASFERIMENTO"]).ToString("dd/MM/yyyy");
                                details.qualifica = rdr["QUALIFICA"].ToString();
                                details.coniuge = rdr["CONIUGE"].ToString();
                                details.figli = rdr["FIGLI"].ToString();
                                details.isep = rdr["ISEP"].ToString();
                                details.contributo = rdr["CONTRIBUTO"].ToString();
                                details.uso = rdr["USO"].ToString();
                                details.totale = rdr["TOTALE"].ToString();
                                model.Add(details);
                            }
                        }
                    }

                }

                return PartialView("DislocazioneDipEstero", model);
            }

           

        }

        // Report Dislocazione dei Dipendenti all'Estero
        public ActionResult RptDislocazione(string codicesede = "", string V_UTENTE = "", string V_DATA = "")
        {
            DataSet15 ds15 = new DataSet15();

            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);

                OracleCommand cmd1 = new OracleCommand();
                cmd1.Connection = conx;
                cmd1.CommandText = "ISE_STAMPA_ELENCO_TRASF";
                cmd1.CommandType = System.Data.CommandType.StoredProcedure;

                cmd1.Parameters.Add("@V_UTENTE", OracleDbType.Varchar2, 50).Value = V_UTENTE;
                cmd1.Parameters.Add("@V_DATA", OracleDbType.Varchar2, 50).Value = V_DATA;
                cmd1.Parameters.Add("@V_UFFICIO", OracleDbType.Varchar2, 50).Value = codicesede;

                conx.Open();
                cmd1.ExecuteNonQuery();

                //String Sql = "Select distinct SEDE, VALUTA, MATRICOLA, NOMINATIVO, DT_TRASFERIMENTO, QUALIFICA, CONIUGE, FIGLI, ISEP, CONTRIBUTO, USO, ISEP + CONTRIBUTO + USO TOTALE From ISE_STP_ELENCOTRASFERIMENTI, SEDIESTERE WHERE SEDIESTERE.SED_COD_SEDE = '" + codicesede + "' AND ISE_STP_ELENCOTRASFERIMENTI.SEDE = SEDIESTERE.SED_DESCRIZIONE Order By SEDE, NOMINATIVO";

                String Sql = "Select SEDE,VALUTA, ";
                Sql += "MATRICOLA, ";
                Sql += "NOMINATIVO, ";
                Sql += "DT_TRASFERIMENTO, ";
                Sql += "QUALIFICA, ";
                Sql += "CONIUGE, ";
                Sql += "FIGLI, ";
                Sql += "ISEP, ";
                Sql += "CONTRIBUTO, ";
                Sql += "USO, ";
                Sql += "ISEP +CONTRIBUTO + USO TOTALE ";
                Sql += "From ISE_STP_ELENCOTRASFERIMENTI ";
                Sql += "Where UTENTE = '" + V_UTENTE + "' ";
                Sql += "Order By SEDE, NOMINATIVO";



                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds15, ds15.V_ISE_STP_ELENCO_TRASF.TableName);
                adp.Fill(ds15, ds15.DataTable15.TableName);
                

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report19.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet15", ds15.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                {   
                    new ReportParameter ("fromDate",V_DATA)
                };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Consuntivo Costi
        public ActionResult ConsuntivoCosti(string V_DATA = "", string V_DATA1 = "", string V_UTENTE = "")
        {

            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {  
                List<Stp_Consuntivo_dei_costi> model = new List<Stp_Consuntivo_dei_costi>();

                if (V_DATA != string.Empty && V_DATA1 != string.Empty)
                {
                    OracleCommand cmd1 = new OracleCommand();
                    cmd1.Connection = cn;
                                        
                    cmd1.CommandText = "ISE_Consuntivo_Costi.CONSUNTIVO_COSTI_MAIN";
                    cmd1.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd1.Parameters.Add("@P_DATA_INI", OracleDbType.Date).Value = Convert.ToDateTime(V_DATA);
                    cmd1.Parameters.Add("@P_DATA_FIN", OracleDbType.Date).Value = Convert.ToDateTime(V_DATA1);
                    cmd1.Parameters.Add("@P_USER", OracleDbType.Varchar2, 50).Value = "fantomas";

                    cn.Open();
                    cmd1.ExecuteNonQuery();

                    String Sql = "SELECT * FROM ISE_STP_CONSUNTIVOCOSTI2";

                    OracleCommand cmd = new OracleCommand(Sql, cn);
                    OracleDataReader rdr = cmd.ExecuteReader();
                    
                    while (rdr.Read())
                    {
                        var details = new Stp_Consuntivo_dei_costi();

                        details.matricola = rdr["MATRICOLA"].ToString();
                        details.nominativo = rdr["NOMINATIVO"].ToString();
                        details.sede = rdr["SEDE"].ToString();
                        details.valuta = rdr["VALUTA"].ToString();
                        details.descrizione = rdr["DESCRIZIONE"].ToString();
                        details.importo = rdr["IMPORTO"].ToString();
                        details.tipoImporto = rdr["TIPOIMPORTO"].ToString();
                        details.qualifica = rdr["QUALIFICA"].ToString();
                        details.codsede = rdr["CODSEDE"].ToString();
                        details.utente = rdr["UTENTE"].ToString();

                        model.Add(details);
                    }
                    //cn.Close();
                }

                return PartialView("ConsuntivoCosti", model);
            }
        }

        // Report Consuntivo Costi
        public ActionResult RptConsuntivoCosti(string V_DATA = "", string V_DATA1 = "")
        {
            DataSet9 ds9 = new DataSet9();
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                #region MyRegion

                String Sql = "Select MATRICOLA, ";
                Sql += "NOMINATIVO, ";
                Sql += "SEDE, ";
                Sql += "VALUTA, ";
                Sql += "DESCRIZIONE, ";
                Sql += "IMPORTO, ";
                Sql += "TIPOIMPORTO, ";
                Sql += "QUALIFICA, ";
                Sql += "CODSEDE ";
                Sql += "From ISE_STP_CONSUNTIVOCOSTI ";
                Sql += "Order By SEDE, NOMINATIVO, DESCRIZIONE ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds9, ds9.V_ISE_STP_CONS_COSTI.TableName);
                adp.Fill(ds9, ds9.DataTable9.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report12.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet9", ds9.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                   {    
                        new ReportParameter ("fromDate",V_DATA),
                        new ReportParameter ("toDate",V_DATA1)
                   };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Consuntivo Costi CoAn
        public ActionResult ConsuntivoCostiCoAn(string codicecoan ="", string V_DATA = "", string V_DATA1 = "")
        {

            ViewBag.ListaCodiceCoan = new List<SelectListItem>();
            List<CodiceCoanModel> lcm = new List<CodiceCoanModel>();
            List<SelectListItem> lr = new List<SelectListItem>();
            List<Stp_Consuntivo_dei_costi_per_codice_Coan> lsd = new List<Stp_Consuntivo_dei_costi_per_codice_Coan>();
            List<Stp_Consuntivo_dei_costi_per_codice_Coan> model = new List<Stp_Consuntivo_dei_costi_per_codice_Coan>();

            lcm = Dipendenti.GetAllCostiCoan().ToList();

            if (lcm != null && lcm.Any())
            {
                foreach (var item in lcm)
                {
                    SelectListItem r = new SelectListItem()
                    {
                        Text = item.codicecoan,
                        Value = item.codicecoan
                    };

                    lr.Add(r);

                }
                if (codicecoan == string.Empty)
                {
                    //lr.First().Selected = true;
                    lr.First().Value = "";
                    codicecoan = lr.First().Value;
                }
                else
                {
                    var lvr = lr.Where(a => a.Value == codicecoan);
                    if (lvr != null && lvr.Count() > 0)
                    {
                        var r = lvr.First();
                        r.Selected = true;

                    }
                }


                ViewBag.ListaCodiceCoan = lr;
            }

            // Chiamare Sub Stampa_Consuntivo_Costi_x_Coan 

            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {

                String Sql = "SELECT C.MATRICOLA, ";
                Sql += "C.NOMINATIVO, ";
                Sql += "C.SEDE, ";
                Sql += "C.VALUTA , ";
                Sql += "C.DESCRIZIONE, ";
                Sql += "SUM(C.IMPORTO) IMPORTO, ";
                Sql += "C.TIPOIMPORTO, ";
                Sql += "C.QUALIFICA, ";
                Sql += "C.CODSEDE, ";
                Sql += "DECODE(C.COAN, 'S', 'Serv. Ist.', C.COAN) COAN ";
                Sql += "FROM ISE_STP_CONSUNTIVOCOSTICOAN C ";
                Sql += "WHERE 1 = 1 ";
                Sql += "AND C.COAN = 'S' ";
                Sql += "GROUP BY C.MATRICOLA, ";
                Sql += "C.NOMINATIVO, ";
                Sql += "C.SEDE, ";
                Sql += "C.VALUTA, ";
                Sql += "C.DESCRIZIONE, ";
                Sql += "C.TIPOIMPORTO, ";
                Sql += "C.QUALIFICA, ";
                Sql += "C.CODSEDE, ";
                Sql += "C.COAN ";
                Sql += "ORDER BY COAN, NOMINATIVO, SEDE, DESCRIZIONE ";

                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                //List<Stp_Consuntivo_dei_costi_per_codice_Coan> model = new List<Stp_Consuntivo_dei_costi_per_codice_Coan>();
                while (rdr.Read())
                {
                    var details = new Stp_Consuntivo_dei_costi_per_codice_Coan();

                    details.matricola = rdr["MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.livello = rdr["QUALIFICA"].ToString();
                    details.ufficio = rdr["SEDE"].ToString();
                    details.descrizione = rdr["DESCRIZIONE"].ToString();
                    details.valuta = rdr["VALUTA"].ToString();
                    details.importo = rdr["IMPORTO"].ToString();

                    model.Add(details);
                }
            
                return PartialView("ConsuntivoCostiCoAn", model);
            }
            
        }

        // Report Consuntivo Costi CoAn
        public ActionResult RptConsuntivoCostiCoAn(string codicecoan = "", string V_DATA = "", string V_DATA1 = "")
        {
            DataSet10 ds10 = new DataSet10();
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                #region MyRegion

                String Sql = "SELECT C.MATRICOLA, ";
                Sql += "C.NOMINATIVO, ";
                Sql += "C.SEDE, ";
                Sql += "C.VALUTA , ";
                Sql += "C.DESCRIZIONE, ";
                Sql += "SUM(C.IMPORTO) IMPORTO, ";
                Sql += "C.TIPOIMPORTO, ";
                Sql += "C.QUALIFICA, ";
                Sql += "C.CODSEDE, ";
                Sql += "DECODE(C.COAN, 'S', 'Serv. Ist.', C.COAN) COAN ";
                Sql += "FROM ISE_STP_CONSUNTIVOCOSTICOAN C ";
                Sql += "WHERE 1 = 1 ";
                Sql += "AND C.COAN = 'S' ";
                Sql += "GROUP BY C.MATRICOLA, ";
                Sql += "C.NOMINATIVO, ";
                Sql += "C.SEDE, ";
                Sql += "C.VALUTA, ";
                Sql += "C.DESCRIZIONE, ";
                Sql += "C.TIPOIMPORTO, ";
                Sql += "C.QUALIFICA, ";
                Sql += "C.CODSEDE, ";
                Sql += "C.COAN ";
                Sql += "ORDER BY COAN, NOMINATIVO, SEDE, DESCRIZIONE ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds10, ds10.V_ISE_STP_CONS_COSTI_COAN.TableName);
                adp.Fill(ds10, ds10.DataTable10.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report13.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet10", ds10.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                   {
                        new ReportParameter ("xcodicecoan",codicecoan),
                        new ReportParameter ("fromDate",V_DATA),
                        new ReportParameter ("toDate",V_DATA1)
                   };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();


                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Operazioni Effettuate
        public ActionResult OperazioniEffettuate()
        {
            var t = new List<SelectListItem>();

            try
            {
                t.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
                t.Add(new SelectListItem() { Text = "Indennità di Sede Estera", Value = "0" });
                t.Add(new SelectListItem() { Text = "Contributo Abitazione", Value = "1" });
                t.Add(new SelectListItem() { Text = "Uso Abitazione", Value = "2" });
                t.Add(new SelectListItem() { Text = "Canone Anticipato", Value = "3" });
                t.Add(new SelectListItem() { Text = "Spese Diverse", Value = "4" });
                t.Add(new SelectListItem() { Text = "Maggiorazione Abitazione", Value = "5" });

                ViewBag.OperazioniEffettuate = t;
                return PartialView("OperazioniEffettuate");

            }
            catch (Exception ex)
            {
                return View("Error");

            }
        }

        // Operazioni Effettuate - Indennità di Sede Estera
        public ActionResult OpIndennitaEstera(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "IES_MATRICOLA MATRICOLA, ";
                Sql += "IES_COD_QUALIFICA QUALIFICA, ";
                Sql += "SED_DESCRIZIONE SEDE, ";
                Sql += "VAL_DESCRIZIONE VALUTA, ";
                Sql += "TMO_DESCRIZIONE_MOVIMENTO TIPO_MOVIMENTO, ";
                Sql += "IES_COD_TIPO_MOVIMENTO CODICE_TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA DATA_DECORRENZA, ";
                Sql += "IES_DT_LETTERA DATA_LETTERA, ";
                Sql += "IES_DT_OPERAZIONE DATA_OPERAZIONE, ";
                Sql += "decode(IES_flag_valuta, ";
                Sql += "'E', ";
                Sql += "IES_INDEN_PERS * IES_CAMBIO, ";
                Sql += "IES_INDEN_PERS / IES_CAMBIO) ISEP, ";
                Sql += "IES_INDEN_SIS_RIE SISTEMAZIONE_RIENTRO, ";
                Sql += "IES_ANTICIPO ANTICIPO, ";
                Sql += "decode(IES_flag_valuta, ";
                Sql += "'E', ";
                Sql += "IES_INDEN_SIS_RIE_NETTA * IES_CAMBIO, ";
                Sql += "IES_INDEN_SIS_RIE_NETTA / IES_CAMBIO) SISRIENETTA, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO ";
                Sql += "From INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where IES_COD_SEDE = SED_COD_SEDE ";
                Sql += "And IES_COD_VALUTA = VAL_COD_VALUTA ";
                Sql += "And IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
                Sql += "And IES_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(IES_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY')  ";
                Sql += "And IES_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_COD_TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "IES_PROG_MOVIMENTO ";

           
                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Op_Indennita_Estera> model = new List<Stp_Op_Indennita_Estera>();
                while (rdr.Read())
                {
                    var details = new Stp_Op_Indennita_Estera();
                    details.matricola = rdr["MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.qualifica = rdr["QUALIFICA"].ToString();
                    details.sede = rdr["SEDE"].ToString();
                    details.valuta = rdr["VALUTA"].ToString();
                    details.tipo_movimento = rdr["TIPO_MOVIMENTO"].ToString();
                    details.codice_tipo_movimento = rdr["CODICE_TIPO_MOVIMENTO"].ToString();
                    details.data_decorrenza = Convert.ToDateTime(rdr["DATA_DECORRENZA"]).ToString("dd/MM/yyyy");
                    details.data_lettera = Convert.ToDateTime(rdr["DATA_LETTERA"]).ToString("dd/MM/yyyy");
                    details.data_operazione = Convert.ToDateTime(rdr["DATA_OPERAZIONE"]).ToString("dd/MM/yyyy");
                    details.indennita_personale = rdr["ISEP"].ToString();
                    details.sist_rientro_lorda = rdr["SISTEMAZIONE_RIENTRO"].ToString();
                    details.anticipo = rdr["ANTICIPO"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpIndennitaEstera", model);
                // return PartialView("PartialViewOpIndennitaEstera", model);
            }
        }

        // Report Operazioni Effettuate - Indennità di Sede Estera
        public ActionResult RptOpIndennitaEstera(string V_DATA = "", string V_DATA1 = "")
        {
            DataSet7 ds7 = new DataSet7();
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                #region RptOpIndennitaEstera

                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "IES_MATRICOLA MATRICOLA, ";
                Sql += "IES_COD_QUALIFICA QUALIFICA, ";
                Sql += "SED_DESCRIZIONE SEDE, ";
                Sql += "VAL_DESCRIZIONE VALUTA, ";
                Sql += "TMO_DESCRIZIONE_MOVIMENTO TIPO_MOVIMENTO, ";
                Sql += "IES_COD_TIPO_MOVIMENTO CODICE_TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA DATA_DECORRENZA, ";
                Sql += "IES_DT_LETTERA DATA_LETTERA, ";
                Sql += "IES_DT_OPERAZIONE DATA_OPERAZIONE, ";
                Sql += "decode(IES_flag_valuta, ";
                Sql += "'E', ";
                Sql += "IES_INDEN_PERS * IES_CAMBIO, ";
                Sql += "IES_INDEN_PERS / IES_CAMBIO) ISEP, ";
                Sql += "IES_INDEN_SIS_RIE SISTEMAZIONE_RIENTRO, ";
                Sql += "IES_ANTICIPO ANTICIPO, ";
                Sql += "decode(IES_flag_valuta, ";
                Sql += "'E', ";
                Sql += "IES_INDEN_SIS_RIE_NETTA * IES_CAMBIO, ";
                Sql += "IES_INDEN_SIS_RIE_NETTA / IES_CAMBIO) SISRIENETTA, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO ";
                Sql += "From INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where IES_COD_SEDE = SED_COD_SEDE ";
                Sql += "And IES_COD_VALUTA = VAL_COD_VALUTA ";
                Sql += "And IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
                Sql += "And IES_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(IES_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY')  ";
                Sql += "And IES_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_COD_TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "IES_PROG_MOVIMENTO ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds7, ds7.V_OP_EFFETTUATE_IND_ESTERA.TableName);
                adp.Fill(ds7, ds7.DataTable7.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report22.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet7", ds7.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                   {
                        new ReportParameter ("fromDate",V_DATA),
                        new ReportParameter ("toDate",V_DATA1)
                   };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();


                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Operazioni Effettuate - Contributo Abitazione
        public ActionResult OpContributoAbitazione(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "CON_MATRICOLA, ";
                Sql += "SED_DESCRIZIONE, ";
                Sql += "VAL_DESCRIZIONE, ";
                Sql += "CON_DT_DECORRENZA, ";
                Sql += "CON_DT_LETTERA, ";
                Sql += "CON_DT_OPERAZIONE, ";
                Sql += "CON_CONTRIBUTO_VALUTA, ";
                Sql += "CON_CANONE, ";
                Sql += "CON_VALUTA_UFFICIALE, ";
                Sql += "CON_PERCENTUALE, ";
                Sql += "CON_CONTRIBUTO_LIRE, ";
                Sql += "CON_PROG_CONTRIBUTO_ABITAZIONE, ";
                Sql += "CON_PROG_TRASFERIMENTO ";
                Sql += "From CONTRIBUTOABITAZIONE, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where CON_COD_SEDE = SED_COD_SEDE ";
                Sql += "And CON_VALUTA_UFFICIALE = VAL_COD_VALUTA ";
                Sql += "And CON_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(CON_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "And CON_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "CON_PROG_TRASFERIMENTO, ";
                Sql += "CON_DT_DECORRENZA, ";
                Sql += "CON_PROG_CONTRIBUTO_ABITAZIONE ";

                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Op_Contributo_Abitazione> model = new List<Stp_Op_Contributo_Abitazione>();
                while (rdr.Read())
                {
                    var details = new Stp_Op_Contributo_Abitazione();
                    details.matricola = rdr["CON_MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.sede = rdr["SED_DESCRIZIONE"].ToString();
                    details.valuta = rdr["VAL_DESCRIZIONE"].ToString();
                    //if (rdr["CON_DT_DECORRENZA"] != DBNull.Value)
                    details.data_decorrenza = rdr["CON_DT_DECORRENZA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["CON_DT_DECORRENZA"]).ToString("dd/MM/yyyy");
                    //details.data_decorrenza = rdr["CON_DT_DECORRENZA"].ToString();
                    //if (rdr["CON_DT_LETTERA"] != DBNull.Value)
                    //details.data_lettera = Convert.ToDateTime("" + rdr["CON_DT_LETTERA"]).ToString("dd/MM/yyyy");
                    details.data_operazione = rdr["CON_DT_OPERAZIONE"] == DBNull.Value ? null : Convert.ToDateTime(rdr["CON_DT_OPERAZIONE"]).ToString("dd/MM/yyyy");
                    //details.data_operazione = rdr["CON_DT_OPERAZIONE"].ToString();
                    details.contributo_valuta = rdr["CON_CONTRIBUTO_VALUTA"].ToString();
                    details.contributo_L_E = rdr["CON_CONTRIBUTO_LIRE"].ToString();
                    details.canone = rdr["CON_CANONE"].ToString();
                    details.percentuale_applicata = rdr["CON_PERCENTUALE"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpContributoAbitazione", model);
            }
        }
       
        // Report Operazioni Effettuate - Contributo Abitazione
        public ActionResult RptOpContributoAbitazione(string V_DATA = "", string V_DATA1 = "")
        {
            DataSet8 ds8 = new DataSet8();
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                #region MyRegion

                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "CON_MATRICOLA, ";
                Sql += "SED_DESCRIZIONE, ";
                Sql += "VAL_DESCRIZIONE, ";
                Sql += "CON_DT_DECORRENZA, ";
                Sql += "CON_DT_LETTERA, ";
                Sql += "CON_DT_OPERAZIONE, ";
                Sql += "CON_CONTRIBUTO_VALUTA, ";
                Sql += "CON_CANONE, ";
                Sql += "CON_VALUTA_UFFICIALE, ";
                Sql += "CON_PERCENTUALE, ";
                Sql += "CON_CONTRIBUTO_LIRE, ";
                Sql += "CON_PROG_CONTRIBUTO_ABITAZIONE, ";
                Sql += "CON_PROG_TRASFERIMENTO ";
                Sql += "From CONTRIBUTOABITAZIONE, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where CON_COD_SEDE = SED_COD_SEDE ";
                Sql += "And CON_VALUTA_UFFICIALE = VAL_COD_VALUTA ";
                Sql += "And CON_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(CON_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "And CON_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "CON_PROG_TRASFERIMENTO, ";
                Sql += "CON_DT_DECORRENZA, ";
                Sql += "CON_PROG_CONTRIBUTO_ABITAZIONE ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds8, ds8.V_OP_EFFETTUATE_CONTR_ABITAZ.TableName);
                adp.Fill(ds8, ds8.DataTable8.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report23.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet8", ds8.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
               {
                    new ReportParameter ("fromDate",V_DATA),
                    new ReportParameter ("toDate",V_DATA1)
               };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Operazioni Effettuate - Uso Abitazione
        public ActionResult OpUsoAbitazione(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                
                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "USO_MATRICOLA AS MATRICOLA, ";
                Sql += "SED_DESCRIZIONE AS SEDE, ";
                Sql += "VAL_DESCRIZIONE AS VALUTA, ";
                Sql += "USO_DT_DECORRENZA AS DATA_DECORRENZA, ";
                Sql += "USO_DT_LETTERA AS DATA_LETTERA, ";
                Sql += "USO_DT_OPERAZIONE AS DATA_OPERAZIONE, ";
                Sql += "USO_CANONE_VALUTA AS CANONE_VALUTA, ";
                Sql += "(USO_CANONE_VALUTA / USO_CAMBIO_VALUTA_CANONE) CANONE, ";
                Sql += "USO_IMPONIBILE_PREV AS IMPONIBILE_PREVIDENZIALE, ";
                Sql += "USO_PROG_USO_ABITAZIONE, ";
                Sql += "USO_PROG_TRASFERIMENTO ";
                Sql += "From USOABITAZIONE, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where USO_COD_SEDE = SED_COD_SEDE ";
                Sql += "And USO_VALUTA_CANONE = VAL_COD_VALUTA ";
                Sql += "And USO_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(USO_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "And USO_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "USO_PROG_TRASFERIMENTO, ";
                Sql += "USO_DT_DECORRENZA, ";
                Sql += "USO_PROG_USO_ABITAZIONE ";


                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Op_Uso_Abitazione> model = new List<Stp_Op_Uso_Abitazione>();
                while (rdr.Read())
                {
                    var details = new Stp_Op_Uso_Abitazione();
                    details.matricola = rdr["MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.sede = rdr["SEDE"].ToString();
                    details.valuta = rdr["VALUTA"].ToString();
                    //details.data_decorrenza = Convert.ToDateTime(rdr["DATA_DECORRENZA"]).ToString("dd/MM/yyyy");
                    //details.data_lettera = Convert.ToDateTime(rdr["DATA_LETTERA"]).ToString("dd/MM/yyyy");
                    //details.data_operazione = Convert.ToDateTime(rdr["DATA_OPERAZIONE"]).ToString("dd/MM/yyyy");
                    details.data_decorrenza = rdr["DATA_DECORRENZA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_DECORRENZA"]).ToString("dd/MM/yyyy");
                    //details.data_decorrenza = rdr["DATA_DECORRENZA"].ToString();
                    //details.data_lettera = rdr["DATA_LETTERA"].ToString();
                    details.data_lettera = rdr["DATA_LETTERA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_LETTERA"]).ToString("dd/MM/yyyy");
                    //details.data_operazione = rdr["DATA_OPERAZIONE"].ToString();
                    details.data_operazione = rdr["DATA_OPERAZIONE"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_OPERAZIONE"]).ToString("dd/MM/yyyy");
                    details.canone_in_valuta = rdr["CANONE_VALUTA"].ToString();
                    //details.canone_in_euro = rdr["CANONE"].ToString();
                    details.imponibile_previdenziale = rdr["IMPONIBILE_PREVIDENZIALE"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpUsoAbitazione", model);
            }
        }

        // Report Operazioni Effettuate - Uso Abitazione
        public ActionResult RptOpUsoAbitazione(string V_DATA = "", string V_DATA1 = "")
        {
            DataSet1 ds1 = new DataSet1();
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                #region MyRegion

                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "USO_MATRICOLA AS MATRICOLA, ";
                Sql += "SED_DESCRIZIONE AS SEDE, ";
                Sql += "VAL_DESCRIZIONE AS VALUTA, ";
                Sql += "USO_DT_DECORRENZA AS DATA_DECORRENZA, ";
                Sql += "USO_DT_LETTERA AS DATA_LETTERA, ";
                Sql += "USO_DT_OPERAZIONE AS DATA_OPERAZIONE, ";
                Sql += "USO_CANONE_VALUTA AS CANONE_VALUTA, ";
                Sql += "ROUND (USO_CANONE_VALUTA / USO_CAMBIO_VALUTA_CANONE, 6) CANONE, ";
                Sql += "USO_IMPONIBILE_PREV AS IMPONIBILE_PREVIDENZIALE, ";
                Sql += "USO_PROG_USO_ABITAZIONE, ";
                Sql += "USO_PROG_TRASFERIMENTO ";
                Sql += "From USOABITAZIONE, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where USO_COD_SEDE = SED_COD_SEDE ";
                Sql += "And USO_VALUTA_CANONE = VAL_COD_VALUTA ";
                Sql += "And USO_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(USO_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "And USO_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "USO_PROG_TRASFERIMENTO, ";
                Sql += "USO_DT_DECORRENZA, ";
                Sql += "USO_PROG_USO_ABITAZIONE ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds1, ds1.V_OP_EFFETTUATE_USO_ABITAZ.TableName);
                adp.Fill(ds1, ds1.DataTable1.TableName);


                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report35.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", ds1.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
               {
                    new ReportParameter ("fromDate",V_DATA),
                    new ReportParameter ("toDate",V_DATA1)
               };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Operazioni Effettuate - Canone Anticipato
        public ActionResult OpCanoneAnticipato(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                
                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME AS NOMINATIVO, ";
                Sql += "CAN_MATRICOLA AS MATRICOLA, ";
                Sql += "SED_DESCRIZIONE AS SEDE, ";
                Sql += "VAL_DESCRIZIONE AS VALUTA, ";
                Sql += "CAN_DT_DECORRENZA AS DATA_DECORRENZA, ";
                Sql += "CAN_DT_LETTERA AS DATA_LETTERA, ";
                Sql += "CAN_DT_OPERAZIONE AS DATA_OPERAZIONE, ";
                Sql += "CAN_CANONE_ANNUO_VALUTA, ";
                Sql += "DECODE(CAN_CAMBIO_VALUTA_CANONE,0,0, ";
                Sql += "CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE) CANONE, ";
                Sql += "- (DECODE(CAN_CAMBIO_VALUTA_CANONE,0,0, ";
                Sql += "CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE) / CAN_N_MESI) AS QUOTA_MENS, ";
                Sql += "CAN_PROG_TRASFERIMENTO, ";
                Sql += "CAN_PROG_CAN_ABITAZIONE ";
                Sql += "From CANONEANNUO, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where CAN_COD_SEDE = SED_COD_SEDE ";
                Sql += "And CAN_VALUTA_CANONE = VAL_COD_VALUTA ";
                Sql += "And CAN_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(CAN_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "And CAN_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "CAN_PROG_TRASFERIMENTO, ";
                Sql += "CAN_DT_DECORRENZA, ";
                Sql += "CAN_PROG_CAN_ABITAZIONE ";


                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Op_Canone_Anticipato> model = new List<Stp_Op_Canone_Anticipato>();
                while (rdr.Read())
                {
                    var details = new Stp_Op_Canone_Anticipato();
                    details.matricola = rdr["MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.sede = rdr["SEDE"].ToString();
                    details.valuta = rdr["VALUTA"].ToString();
                    details.data_decorrenza = rdr["DATA_DECORRENZA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_DECORRENZA"]).ToString("dd/MM/yyyy");
                    //details.data_decorrenza = rdr["DATA_DECORRENZA"].ToString();
                    //details.data_lettera = rdr["DATA_LETTERA"].ToString();
                    details.data_lettera = rdr["DATA_LETTERA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_LETTERA"]).ToString("dd/MM/yyyy");
                    details.data_operazione = rdr["DATA_OPERAZIONE"].ToString();
                    details.anticipo_valuta = rdr["CANONE"].ToString();
                    details.anticipo_euro = rdr["CANONE"].ToString();
                    //details.quota_mensile = rdr["QUOTA_MENS"].ToString();
                     model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpCanoneAnticipato", model);

                
            }
        }

        // Report Operazioni Effettuate - Canone Anticipato
        public ActionResult RptOpCanoneAnticipato(string V_DATA = "", string V_DATA1 = "")
        {
            //DataSet14 ds14 = new DataSet14();
            DataSet22 ds22 = new DataSet22();
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                #region MyRegion

                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME AS NOMINATIVO, ";
                Sql += "CAN_MATRICOLA AS MATRICOLA, ";
                Sql += "SED_DESCRIZIONE AS SEDE, ";
                Sql += "VAL_DESCRIZIONE AS VALUTA, ";
                Sql += "CAN_DT_DECORRENZA AS DATA_DECORRENZA, ";
                Sql += "CAN_DT_LETTERA AS DATA_LETTERA, ";
                Sql += "CAN_DT_OPERAZIONE AS DATA_OPERAZIONE, ";
                Sql += "CAN_CANONE_ANNUO_VALUTA, ";
                Sql += "DECODE(CAN_CAMBIO_VALUTA_CANONE,0,0, ";
                Sql += "CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE) CANONE, ";
                Sql += "- ROUND((DECODE(CAN_CAMBIO_VALUTA_CANONE,0,0, ";
                Sql += "CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE) / CAN_N_MESI),6) AS QUOTA_MENS, ";
                Sql += "CAN_PROG_TRASFERIMENTO, ";
                Sql += "CAN_PROG_CAN_ABITAZIONE ";
                Sql += "From CANONEANNUO, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where CAN_COD_SEDE = SED_COD_SEDE ";
                Sql += "And CAN_VALUTA_CANONE = VAL_COD_VALUTA ";
                Sql += "And CAN_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(CAN_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "And CAN_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "CAN_PROG_TRASFERIMENTO, ";
                Sql += "CAN_DT_DECORRENZA, ";
                Sql += "CAN_PROG_CAN_ABITAZIONE ";
                #endregion


                //String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                //Sql += "CAN_MATRICOLA AS MATRICOLA, ";
                //Sql += "SED_DESCRIZIONE AS SEDE, ";
                //Sql += "VAL_DESCRIZIONE AS VALUTA, ";
                //Sql += "CAN_DT_DECORRENZA AS DATA_DECORRENZA, "; 
                //Sql += "CAN_DT_LETTERA AS DATA_DECORRENZA, ";
                //Sql += "CAN_DT_OPERAZIONE AS DATA_OPERAZIONE, ";
                //Sql += "CAN_CANONE_ANNUO_VALUTA, ";
                //Sql += "DECODE(CAN_CAMBIO_VALUTA_CANONE, ";
                //Sql += "0, ";
                //Sql += "0, ";
                //Sql += "CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE) CANONE, ";
                //Sql += "- (DECODE(CAN_CAMBIO_VALUTA_CANONE, ";
                //Sql += "0, ";
                //Sql += "0, ";
                //Sql += "CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE) / ";
                //Sql += "CAN_N_MESI) QUOTA_MENS, ";
                //Sql += "CAN_PROG_TRASFERIMENTO, ";
                //Sql += "CAN_PROG_CAN_ABITAZIONE ";
                //Sql += "From CANONEANNUO, SEDIESTERE, VALUTE, ANADIPE ";
                //Sql += "Where CAN_COD_SEDE = SED_COD_SEDE ";
                //Sql += "And CAN_VALUTA_CANONE = VAL_COD_VALUTA ";
                //Sql += "And CAN_MATRICOLA = AND_MATRICOLA ";
                //Sql += "And(CAN_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') And ";
                //Sql += "CAN_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                //Sql += "Order By NOMINATIVO, ";
                //Sql += "CAN_PROG_TRASFERIMENTO, ";
                //Sql += "CAN_DT_DECORRENZA, ";
                //Sql += "CAN_PROG_CAN_ABITAZIONE ";



                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds14, ds14.V_OP_EFFETTUATE_CANONE_ANTI.TableName);
                adp.Fill(ds22, ds22.DataTable22.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report35.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet22", ds22.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                {
                    new ReportParameter ("fromDate",V_DATA),
                    new ReportParameter ("toDate",V_DATA1)
                };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();
        }

        // Operazioni Effettuate - Spese Diverse
        public ActionResult OpSpeseDiverse(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                
                String Sql = "Select Distinct ANADIPE.AND_COGNOME || ' ' || ANADIPE.AND_NOME AS NOMINATIVO,";
                Sql += "SPESEDIVERSE.SPD_MATRICOLA AS MATRICOLA, ";
                Sql += "SEDIESTERE.SED_DESCRIZIONE AS DESCRIZIONE, ";
                Sql += "TIPISPESE.TSP_DESCRIZIONE, ";
                Sql += "SPESEDIVERSE.SPD_DT_DECORRENZA, ";
                Sql += "SPESEDIVERSE.SPD_DT_OPERAZIONE, ";
                Sql += "SPESEDIVERSE.SPD_IMPORTO_LIRE, ";
                Sql += "SPESEDIVERSE.SPD_TIPO_MOVIMENTO, ";
                Sql += "SPESEDIVERSE.SPD_PROG_SPESA, ";
                Sql += "SPESEDIVERSE.SPD_PROG_TRASFERIMENTO ";
                Sql += "From SPESEDIVERSE, SEDIESTERE, ANADIPE, TIPISPESE ";
                Sql += "Where SPD_COD_SEDE = SED_COD_SEDE ";
                Sql += "And SPESEDIVERSE.SPD_COD_SPESA = TIPISPESE.TSP_COD_SPESA ";
                Sql += "And SPESEDIVERSE.SPD_MATRICOLA = ANADIPE.AND_MATRICOLA ";
                Sql += "And(SPD_DT_OPERAZIONE >= To_Date('" + V_DATA + "', 'DD-MM-YYYY') ";
                Sql += "And SPD_DT_OPERAZIONE <= To_Date('" + V_DATA1 + "', 'DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "SPD_PROG_TRASFERIMENTO, ";
                Sql += "SPD_DT_DECORRENZA, ";
                Sql += "SPD_PROG_SPESA ";

                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Op_Spese_Diverse> model = new List<Stp_Op_Spese_Diverse>();
                while (rdr.Read())
                {
                    var details = new Stp_Op_Spese_Diverse();
                    details.NOMINATIVO = rdr["NOMINATIVO"].ToString();
                    details.MATRICOLA = rdr["MATRICOLA"].ToString();
                    details.DESCRIZIONE = rdr["DESCRIZIONE"].ToString();
                    details.TSP_DESCRIZIONE = rdr["TSP_DESCRIZIONE"].ToString();
                    //details.SPD_DT_DECORRENZA = rdr["SPD_DT_DECORRENZA"].ToString();
                    //details.SPD_DT_OPERAZIONE = rdr["SPD_DT_OPERAZIONE"].ToString();
                    details.SPD_DT_DECORRENZA = Convert.ToDateTime(rdr["SPD_DT_DECORRENZA"]).ToString("dd/MM/yyyy");
                    details.SPD_DT_OPERAZIONE = Convert.ToDateTime(rdr["SPD_DT_OPERAZIONE"]).ToString("dd/MM/yyyy");
                    details.SPD_IMPORTO_LIRE = rdr["SPD_IMPORTO_LIRE"].ToString();
                    details.SPD_TIPO_MOVIMENTO = rdr["SPD_TIPO_MOVIMENTO"].ToString();
                    details.SPD_PROG_SPESA = rdr["SPD_PROG_SPESA"].ToString();
                    details.SPD_PROG_TRASFERIMENTO = rdr["SPD_PROG_TRASFERIMENTO"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpSpeseDiverse", model);
            }
        }

        // Report Operazioni Effettuate - Spese Diverse
        public ActionResult RptOpSpeseDiverse(string V_DATA = "", string V_DATA1 = "")
        {
            DataSet17 ds17 = new DataSet17();
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                #region MyRegion

                String Sql = "Select Distinct ANADIPE.AND_COGNOME || ' ' || ANADIPE.AND_NOME AS NOMINATIVO,";
                Sql += "SPESEDIVERSE.SPD_MATRICOLA AS MATRICOLA, ";
                Sql += "SEDIESTERE.SED_DESCRIZIONE AS DESCRIZIONE, ";
                Sql += "TIPISPESE.TSP_DESCRIZIONE, ";
                Sql += "SPESEDIVERSE.SPD_DT_DECORRENZA, ";
                Sql += "SPESEDIVERSE.SPD_DT_OPERAZIONE, ";
                Sql += "SPESEDIVERSE.SPD_IMPORTO_LIRE, ";
                Sql += "SPESEDIVERSE.SPD_TIPO_MOVIMENTO, ";
                Sql += "SPESEDIVERSE.SPD_PROG_SPESA, ";
                Sql += "SPESEDIVERSE.SPD_PROG_TRASFERIMENTO ";
                Sql += "From SPESEDIVERSE, SEDIESTERE, ANADIPE, TIPISPESE ";
                Sql += "Where SPD_COD_SEDE = SED_COD_SEDE ";
                Sql += "And SPESEDIVERSE.SPD_COD_SPESA = TIPISPESE.TSP_COD_SPESA ";
                Sql += "And SPESEDIVERSE.SPD_MATRICOLA = ANADIPE.AND_MATRICOLA ";
                Sql += "And(SPD_DT_OPERAZIONE >= To_Date('" + V_DATA + "', 'DD-MM-YYYY') ";
                Sql += "And SPD_DT_OPERAZIONE <= To_Date('" + V_DATA1 + "', 'DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "SPD_PROG_TRASFERIMENTO, ";
                Sql += "SPD_DT_DECORRENZA, ";
                Sql += "SPD_PROG_SPESA ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds17, ds17.V_OP_EFFETTUATE_SPESE_DIVERSE.TableName);
                adp.Fill(ds17, ds17.DataTable17.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report32.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet17", ds17.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                {
                    new ReportParameter ("fromDate",V_DATA),
                    new ReportParameter ("toDate",V_DATA1)
                };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();
        }

        // Operazioni Effettuate - Maggiorazione Abitazione
        public ActionResult OpMaggiorazioneAbitazione(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                String Sql = "SELECT DISTINCT MAB.IES_MATRICOLA MATRICOLA, ";
                Sql += "A.AND_COGNOME || ' ' || A.AND_NOME NOMINATIVO, ";
                Sql += "S.SED_COD_SEDE CodSede, ";
                Sql += "S.SED_DESCRIZIONE SEDE, ";
                Sql += "(MAB.VAL_ID_VALUTACANONE + MAB.VAL_ID_VALUTAUFFICIALE) VALUTA, ";
                Sql += "MAB.MAB_DT_DATADECORRENZA DATADECORRENZA, ";
                Sql += "MAB.MAB_DT_LETTERA DATALETTERA, ";
                Sql += "MAB.MAB_DT_OPERAZIONE DATAOPERAZIONE, ";
                Sql += "MAB.MAB_CANONELOCAZIONE CANONE, ";
                Sql += "RMAB.MAB_RAT_MAGINVIATA RATAINVIATA, ";
                Sql += "P.MAB_PAR_PERCENTUALE PERCENTUALE, ";
                Sql += "MAB.IES_PROG_TRASFERIMENTO ";
                Sql += "FROM MAG_ABITAZIONE MAB, ";
                Sql += "RATEIZZAZIONECONTMAB RMAB, ";
                Sql += "ANADIPE A, ";
                Sql += "SEDIESTERE S, ";
                Sql += "PARAMETRIMAB P ";
                Sql += "WHERE 1 = 1 ";
                Sql += "AND RMAB.MAB_ID = MAB.MAB_ID ";
                Sql += "AND A.AND_MATRICOLA = MAB.IES_MATRICOLA ";
                Sql += "AND MAB.IES_COD_SEDE = S.SED_COD_SEDE ";
                Sql += "AND P.MAB_PAR_ID = MAB.MAB_PAR_ID ";
                Sql += "AND MAB.MAB_FLAG_ANNULLATO = 0 ";
                Sql += "AND RMAB.MAB_RAT_FLAG_ANNULLATO = 0 ";
                Sql += "AND MAB.MAB_CESSAZIONE = 0 ";
                Sql += "AND(RMAB.MAB_RAT_DATARATA >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "AND RMAB.MAB_RAT_DATARATA <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "ORDER BY NOMINATIVO, MAB.IES_PROG_TRASFERIMENTO, MAB.MAB_DT_DATADECORRENZA ";


                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Op_Magg_Abitaz> model = new List<Stp_Op_Magg_Abitaz>();
                while (rdr.Read())
                {
                    var details = new Stp_Op_Magg_Abitaz();
                    details.matricola = rdr["MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.codice_sede = rdr["CodSede"].ToString();
                    details.sede = rdr["SEDE"].ToString();
                    details.valuta = rdr["VALUTA"].ToString();
                    //if (rdr["DATADECORRENZA"] != DBNull.Value)
                    details.data_decorrenza = Convert.ToDateTime(rdr["DATADECORRENZA"]).ToString("dd/MM/yyyy");
                    details.data_lettera = Convert.ToDateTime(rdr["DATALETTERA"]).ToString("dd/MM/yyyy");
                    details.data_operazione = Convert.ToDateTime(rdr["DATAOPERAZIONE"]).ToString("dd/MM/yyyy");
                    details.canone = rdr["CANONE"].ToString();
                    details.importo = rdr["RATAINVIATA"].ToString();
                    details.percentuale_applicata = rdr["PERCENTUALE"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpMaggiorazioneAbitazione", model);
            }
        }

        // Report Operazioni Effettuate - Maggiorazione Abitazione
        public ActionResult RptOpMaggiorazioneAbitazione(string V_DATA = "", string V_DATA1 = "")
        {
            DataSet3 ds3 = new DataSet3();
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                #region MyRegion

                String Sql = "SELECT DISTINCT MAB.IES_MATRICOLA MATRICOLA, ";
                Sql += "A.AND_COGNOME || ' ' || A.AND_NOME NOMINATIVO, ";
                Sql += "S.SED_COD_SEDE CodSede, ";
                Sql += "S.SED_DESCRIZIONE SEDE, ";
                Sql += "(MAB.VAL_ID_VALUTACANONE + MAB.VAL_ID_VALUTAUFFICIALE) VALUTA, ";
                Sql += "MAB.MAB_DT_DATADECORRENZA DATADECORRENZA, ";
                Sql += "MAB.MAB_DT_LETTERA DATALETTERA, ";
                Sql += "MAB.MAB_DT_OPERAZIONE DATAOPERAZIONE, ";
                Sql += "MAB.MAB_CANONELOCAZIONE CANONE, ";
                Sql += "RMAB.MAB_RAT_MAGINVIATA RATAINVIATA, ";
                Sql += "P.MAB_PAR_PERCENTUALE PERCENTUALE, ";
                Sql += "MAB.IES_PROG_TRASFERIMENTO ";
                Sql += "FROM MAG_ABITAZIONE MAB, ";
                Sql += "RATEIZZAZIONECONTMAB RMAB, ";
                Sql += "ANADIPE A, ";
                Sql += "SEDIESTERE S, ";
                Sql += "PARAMETRIMAB P ";
                Sql += "WHERE 1 = 1 ";
                Sql += "AND RMAB.MAB_ID = MAB.MAB_ID ";
                Sql += "AND A.AND_MATRICOLA = MAB.IES_MATRICOLA ";
                Sql += "AND MAB.IES_COD_SEDE = S.SED_COD_SEDE ";
                Sql += "AND P.MAB_PAR_ID = MAB.MAB_PAR_ID ";
                Sql += "AND MAB.MAB_FLAG_ANNULLATO = 0 ";
                Sql += "AND RMAB.MAB_RAT_FLAG_ANNULLATO = 0 ";
                Sql += "AND MAB.MAB_CESSAZIONE = 0 ";
                Sql += "AND(RMAB.MAB_RAT_DATARATA >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "AND RMAB.MAB_RAT_DATARATA <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "ORDER BY NOMINATIVO, MAB.IES_PROG_TRASFERIMENTO, MAB.MAB_DT_DATADECORRENZA ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds3, ds3.V_OP_EFFETTUATE_MAGG_ABITAZ.TableName);
                adp.Fill(ds3, ds3.DataTable3.TableName);


                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report26.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet3", ds3.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                {
                    new ReportParameter ("fromDate",V_DATA),
                    new ReportParameter ("toDate",V_DATA1)
                };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();
        }

        // Presenze Livelli in servizio all' Estero
        public ActionResult PresenzeLivelli(string codicequalifica = "", string V_DATA= "", string V_DATA1= "")
        {
            // Combo Qualifiche
            //Select Distinct IBS_DESCRIZIONE From INDENNITABASE Order By IBS_DESCRIZIONE

            ViewBag.ListaDipendentiEsteroLivello = new List<SelectListItem>();
            List<DipEsteroLivelloModel> lcm = new List<DipEsteroLivelloModel>();
            List<SelectListItem> lr = new List<SelectListItem>();
            List<Stp_Presenze_Livelli> lsd = new List<Stp_Presenze_Livelli>();
            List<Stp_Presenze_Livelli> model = new List<Stp_Presenze_Livelli>();
            
            lcm = Dipendenti.GetAllQualifiche().ToList();

            if (lcm != null && lcm.Any())
            {
                foreach (var item in lcm)
                {
                    SelectListItem r = new SelectListItem()
                    {
                        Text = item.qualifica,
                        Value = item.codicequalifica
                    };

                    lr.Add(r);

                }
                if (codicequalifica == string.Empty)
                {

                    //lr.First().Selected = true;
                    lr.First().Value = "";
                    codicequalifica = lr.First().Value;
                }
                else
                {
                    var lvr = lr.Where(a => a.Value == codicequalifica);
                    if (lvr != null && lvr.Count() > 0)
                    {
                        var r = lvr.First();
                        r.Selected = true;

                    }
                }


                ViewBag.ListaDipendentiEsteroLivello = lr;
            }

            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                // Insert into ISE_STP_LIVELLIESTERI
                //String Sql = "Select distinct CODQUALIFICA, QUALIFICA, NOMINATIVO, MATRICOLA, SEDE, DT_TRASFERIMENTO, DT_RIENTRO, DT_DECORRENZA From ISE_STP_LIVELLIESTERI Order By QUALIFICA, NOMINATIVO";
                //string Data_Oracle = VDATA;

                if (V_DATA == "")
                {
                    V_DATA = "01/01/" + DateTime.Now.Year;
                }
            
                
                String Sql = "Select Distinct IES_COD_QUALIFICA, ";
                Sql += "IBS_DESCRIZIONE, ";
                Sql += "IES_MATRICOLA, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "SED_DESCRIZIONE, ";
                Sql += "IES_DT_TRASFERIMENTO, ";
                Sql += "TRA_DT_FIN_TRASFERIMENTO, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO ";
                Sql += "From INDESTERA T1, ";
                Sql += "SEDIESTERE, ";
                Sql += "INDENNITABASE, ";
                Sql += "ANADIPE, ";
                Sql += "TRASFERIMENTO, ";
                Sql += "(Select A.IES_MATRICOLA MATR, Max(A.IES_DT_DECORRENZA) DATAM ";
                Sql += "From INDESTERA A, INDESTERA B ";
                Sql += "Where A.IES_MATRICOLA = B.IES_MATRICOLA ";
                Sql += "And A.IES_PROG_TRASFERIMENTO = B.IES_PROG_TRASFERIMENTO ";
                Sql += "And A.IES_DT_DECORRENZA <= To_Date ('" + V_DATA + "', 'DD-MM-YYYY')  ";
                Sql += "GROUP BY A.IES_MATRICOLA) MAXDATA ";
                Sql += "Where(IES_DT_TRASFERIMENTO <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "And (TRA_DT_FIN_TRASFERIMENTO Is Null Or ";
                Sql += "To_Date (TRA_DT_FIN_TRASFERIMENTO) > ";
                Sql += "To_Date( '" + V_DATA + "', 'DD-MM-YYYY')) ";
                Sql += "And MAXDATA.MATR(+) = IES_MATRICOLA ";
                Sql += "And (IES_DT_DECORRENZA > To_Date('" + V_DATA + "', 'DD-MM-YYYY') Or ";
                Sql += "IES_DT_DECORRENZA = MAXDATA.DATAM) ";
                Sql += "And IES_FLAG_RICALCOLATO Is Null ";
                Sql += "And IES_COD_QUALIFICA = IBS_COD_QUALIFICA ";
                Sql += "And IES_MATRICOLA = AND_MATRICOLA ";
                Sql += "And IES_COD_SEDE = SED_COD_SEDE ";
                Sql += "And IES_MATRICOLA = TRA_MATRICOLA ";
                Sql += "And IES_PROG_TRASFERIMENTO = TRA_PROG_TRASFERIMENTO ";
                if (codicequalifica != "")
                    Sql += "And IES_COD_QUALIFICA = :codqualifica ";
                //Sql += "And IBS_DESCRIZIONE = 'DIRIGENTE' ";
                Sql += "Order By IES_MATRICOLA, ";
                Sql += "IES_PROG_TRASFERIMENTO Desc, ";
                Sql += "IES_DT_DECORRENZA Desc, ";
                Sql += "IES_PROG_MOVIMENTO Desc ";

                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = Sql;
                    cmd.Parameters.Add("codqualifica", codicequalifica);
                    cmd.Connection.Open();

                    using (OracleDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                var details = new Stp_Presenze_Livelli();
                                details.codQualifica = rdr["IES_COD_QUALIFICA"].ToString();
                                details.qualifica = rdr["IBS_DESCRIZIONE"].ToString();
                                details.nominativo = rdr["NOMINATIVO"].ToString();
                                details.matricola = rdr["IES_MATRICOLA"].ToString();
                                details.sede = rdr["SED_DESCRIZIONE"].ToString();
                                details.dt_Trasferimento = Convert.ToDateTime(rdr["IES_DT_TRASFERIMENTO"]).ToString("dd/MM/yyyy");
                                details.dt_Rientro = rdr["TRA_DT_FIN_TRASFERIMENTO"].ToString();
                                details.dt_Decorrenza = Convert.ToDateTime(rdr["IES_DT_DECORRENZA"]).ToString("dd/MM/yyyy");
                                details.progr_trasferimento = rdr["IES_PROG_TRASFERIMENTO"].ToString();
                                details.progr_movimento = rdr["IES_PROG_MOVIMENTO"].ToString();
                                model.Add(details);
                            }
                        }
                    }

                }

                return PartialView("PresenzeLivelli", model);
            }

            
        }

        // Report Presenze Livelli in servizio all' Estero
        public ActionResult RptPresenzeLivelli(string codicequalifica="", string V_DATA="", string V_DATA1="")
        {
            DataSet13 ds13 = new DataSet13();

            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);

                //String Sql = "Select distinct CODQUALIFICA, QUALIFICA, NOMINATIVO, MATRICOLA, SEDE, DT_TRASFERIMENTO, DT_RIENTRO, DT_DECORRENZA From ISE_STP_LIVELLIESTERI Order By QUALIFICA, NOMINATIVO";

                #region MyRegion
                String Sql = "Select Distinct IES_COD_QUALIFICA, ";
                Sql += "IBS_DESCRIZIONE, ";
                Sql += "IES_MATRICOLA, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "SED_DESCRIZIONE, ";
                Sql += "IES_DT_TRASFERIMENTO, ";
                Sql += "TRA_DT_FIN_TRASFERIMENTO, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO ";
                Sql += "From INDESTERA T1, ";
                Sql += "SEDIESTERE, ";
                Sql += "INDENNITABASE, ";
                Sql += "ANADIPE, ";
                Sql += "TRASFERIMENTO, ";
                Sql += "(Select A.IES_MATRICOLA MATR, Max(A.IES_DT_DECORRENZA) DATAM ";
                Sql += "From INDESTERA A, INDESTERA B ";
                Sql += "Where A.IES_MATRICOLA = B.IES_MATRICOLA ";
                Sql += "And A.IES_PROG_TRASFERIMENTO = B.IES_PROG_TRASFERIMENTO ";
                Sql += "And A.IES_DT_DECORRENZA <= To_Date ('" + V_DATA + "', 'DD-MM-YYYY')  ";
                Sql += "GROUP BY A.IES_MATRICOLA) MAXDATA ";
                Sql += "Where(IES_DT_TRASFERIMENTO <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "And (TRA_DT_FIN_TRASFERIMENTO Is Null Or ";
                Sql += "To_Date (TRA_DT_FIN_TRASFERIMENTO) > ";
                Sql += "To_Date( '" + V_DATA + "', 'DD-MM-YYYY')) ";
                Sql += "And MAXDATA.MATR(+) = IES_MATRICOLA ";
                Sql += "And (IES_DT_DECORRENZA > To_Date('" + V_DATA + "', 'DD-MM-YYYY') Or ";
                Sql += "IES_DT_DECORRENZA = MAXDATA.DATAM) ";
                Sql += "And IES_FLAG_RICALCOLATO Is Null ";
                Sql += "And IES_COD_QUALIFICA = IBS_COD_QUALIFICA ";
                Sql += "And IES_MATRICOLA = AND_MATRICOLA ";
                Sql += "And IES_COD_SEDE = SED_COD_SEDE ";
                Sql += "And IES_MATRICOLA = TRA_MATRICOLA ";
                Sql += "And IES_PROG_TRASFERIMENTO = TRA_PROG_TRASFERIMENTO ";
                if (codicequalifica != "")
                    //Sql += "And IES_COD_QUALIFICA = :codicequalifica ";
                Sql += "And IES_COD_QUALIFICA = '" + codicequalifica + "' ";
                //Sql += "And IBS_DESCRIZIONE = 'DIRIGENTE' ";
                Sql += "Order By IES_MATRICOLA, ";
                Sql += "IES_PROG_TRASFERIMENTO Desc, ";
                Sql += "IES_DT_DECORRENZA Desc, ";
                Sql += "IES_PROG_MOVIMENTO Desc ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds13, ds13.V_PRESENZE_LIVELLI.TableName);
                adp.Fill(ds13, ds13.DataTable13.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report20.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet13", ds13.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                   {
                        new ReportParameter ("fromDate",V_DATA),
                        new ReportParameter ("toDate",V_DATA1)
                   };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();
        }

        // Spese Diverse
        public ActionResult SpeseDiverse(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                
                //String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                //Sql += "ANADIPE.AND_LIVELLO LIVELLO, ";
                //Sql += "SED_DESCRIZIONE SEDE, ";
                //Sql += "TSP_DESCRIZIONE, ";
                //Sql += "SPESEDIVERSE.* ";
                //Sql += "From ANADIPE, TIPISPESE, SPESEDIVERSE, SEDIESTERE ";
                //Sql += "Where TSP_COD_SPESA = SPD_COD_SPESA ";
                //Sql += "And SPD_MATRICOLA = AND_MATRICOLA ";
                //Sql += "And SPD_COD_SEDE = SED_COD_SEDE ";
                //Sql += "And (SPD_DT_DECORRENZA >= To_Date ('" + V_DATA + "', 'DD-MM-YYYY')  ";
                //Sql += "And SPD_DT_DECORRENZA <= To_Date ('" + V_DATA1 + "', 'DD-MM-YYYY'))  ";

                String Sql = "SELECT DISTINCT ANADIPE.AND_MATRICOLA AS MATRICOLA, ";
                Sql += "ANADIPE.AND_COGNOME || ' ' || ANADIPE.AND_NOME  AS NOMINATIVO, ";
                Sql += "ANADIPE.AND_LIVELLO AS LIVELLO, ";
                Sql += "SEDIESTERE.SED_COD_SEDE AS CODICE_SEDE, ";
                Sql += "SEDIESTERE.SED_DESCRIZIONE AS DESCRIZIONE_SEDE, ";
                Sql += "SPESEDIVERSE.SPD_DT_DECORRENZA AS DATA, ";
                Sql += "TIPISPESE.TSP_DESCRIZIONE AS VOCE_DI_SPESA, ";
                Sql += "SPESEDIVERSE.SPD_IMPORTO_VALUTA AS IMPORTO_VALUTA ";
                Sql += "FROM ANADIPE, ";
                Sql += "TIPISPESE, ";
                Sql += "SPESEDIVERSE, ";
                Sql += "SEDIESTERE ";
                Sql += "WHERE TSP_COD_SPESA = SPD_COD_SPESA ";
                Sql += "AND SPD_MATRICOLA = AND_MATRICOLA ";
                Sql += "AND SPD_COD_SEDE = SED_COD_SEDE ";
                Sql += "AND(SPD_DT_DECORRENZA >= To_Date ('" + V_DATA + "', 'DD-MM-YYYY') ";
                Sql += "AND SPD_DT_DECORRENZA <= To_Date ('" + V_DATA1 + "', 'DD-MM-YYYY')) ";

                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Spese_diverse> model = new List<Stp_Spese_diverse>();
                while (rdr.Read())
                {
                    var details = new Stp_Spese_diverse();
                        details.MATRICOLA = rdr["MATRICOLA"].ToString();
                        details.NOMINATIVO = rdr["NOMINATIVO"].ToString();
                        details.LIVELLO = rdr["LIVELLO"].ToString();
                        details.CODICE_SEDE = rdr["CODICE_SEDE"].ToString();
                        details.DESCRIZIONE_SEDE = rdr["DESCRIZIONE_SEDE"].ToString();
                        details.DATA = Convert.ToDateTime(rdr["DATA"]).ToString("dd/mm/yyyy");
                        details.VOCE_DI_SPESA = rdr["VOCE_DI_SPESA"].ToString();
                        details.IMPORTO_VALUTA = rdr["IMPORTO_VALUTA"].ToString();
                        model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("SpeseDiverse", model);
            }
        }

        // Report Spese Diverse
        public ActionResult RptSpeseDiverse(string V_DATA = "", string V_DATA1 = "")
        {
            DataSet18 ds18 = new DataSet18();

            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);


                //String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                //Sql += "ANADIPE.AND_LIVELLO LIVELLO, ";
                //Sql += "SED_DESCRIZIONE SEDE, ";
                //Sql += "TSP_DESCRIZIONE, ";
                //Sql += "SPESEDIVERSE.* ";
                //Sql += "From ANADIPE, TIPISPESE, SPESEDIVERSE, SEDIESTERE ";
                //Sql += "Where TSP_COD_SPESA = SPD_COD_SPESA ";
                //Sql += "And SPD_MATRICOLA = AND_MATRICOLA ";
                //Sql += "And SPD_COD_SEDE = SED_COD_SEDE ";
                //Sql += "And (SPD_DT_DECORRENZA >= To_Date ('" + V_DATA + "', 'DD-MM-YYYY')  ";
                //Sql += "And SPD_DT_DECORRENZA <= To_Date ('" + V_DATA1 + "', 'DD-MM-YYYY'))  ";

                #region MyRegion
                String Sql = "SELECT DISTINCT ANADIPE.AND_MATRICOLA AS MATRICOLA, ";
                Sql += "ANADIPE.AND_COGNOME || ' ' || ANADIPE.AND_NOME  AS NOMINATIVO, ";
                Sql += "ANADIPE.AND_LIVELLO AS LIVELLO, ";
                Sql += "SEDIESTERE.SED_COD_SEDE AS CODICE_SEDE, ";
                Sql += "SEDIESTERE.SED_DESCRIZIONE AS DESCRIZIONE_SEDE, ";
                Sql += "SPESEDIVERSE.SPD_DT_DECORRENZA AS DATA, ";
                Sql += "TIPISPESE.TSP_DESCRIZIONE AS VOCE_DI_SPESA, ";
                Sql += "SPESEDIVERSE.SPD_IMPORTO_VALUTA AS IMPORTO_VALUTA ";
                Sql += "FROM ANADIPE, ";
                Sql += "TIPISPESE, ";
                Sql += "SPESEDIVERSE, ";
                Sql += "SEDIESTERE ";
                Sql += "WHERE TSP_COD_SPESA = SPD_COD_SPESA ";
                Sql += "AND SPD_MATRICOLA = AND_MATRICOLA ";
                Sql += "AND SPD_COD_SEDE = SED_COD_SEDE ";
                Sql += "AND(SPD_DT_DECORRENZA >= To_Date ('" + V_DATA + "', 'DD-MM-YYYY') ";
                Sql += "AND SPD_DT_DECORRENZA <= To_Date ('" + V_DATA1 + "', 'DD-MM-YYYY')) ";
                Sql += "ORDER BY NOMINATIVO ";
                #endregion

                //string sql = "";
                //sql += "SELECT * FROM TABLE WHERE NAME='JOHN SMITH'";
                //OdbcDataAdapter adptr = new OdbcDataAdapter(sql, _connection);
                //DataSet ds = new DataSet();
                //adptr.Fill(ds);
                //return ds;

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);
                adp.Fill(ds18, ds18.DataTable1.TableName);
                

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report33.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet18", ds18.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                {
                    new ReportParameter ("fromDate",V_DATA),
                    new ReportParameter ("toDate",V_DATA1)
                };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();
        }

        // Spese Avvicendamento
        public ActionResult SpeseAvvicendamento(string V_DATA = "", string V_DATA1 = "")
        {

            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
               
                String Sql = "SELECT DISTINCT SPESEDIVERSE.SPD_MATRICOLA AS MATRICOLA, ";
                Sql += "ANADIPE.AND_COGNOME || ' ' || ANADIPE.AND_NOME AS NOMINATIVO, ";
                Sql += "ANADIPE.AND_LIVELLO AS LIVELLO, ";
                Sql += "SPESEDIVERSE.SPD_COD_SEDE AS CODICE_SEDE, ";
                Sql += "SEDIESTERE.SED_DESCRIZIONE AS DESCRIZIONE_SEDE, ";
                Sql += "SPESEDIVERSE.SPD_DT_DECORRENZA AS DATA, ";
                Sql += "TIPISPESE.TSP_DESCRIZIONE AS SPESA, ";
                Sql += "SPESEDIVERSE.SPD_IMPORTO_VALUTA AS INDENITA_IN_VALUTA ";
                //Sql += "SPESEDIVERSE.* ";
                Sql += "FROM ANADIPE, ";
                Sql += "TIPISPESE, ";
                Sql += "SPESEDIVERSE, ";
                Sql += "SEDIESTERE ";
                Sql += "WHERE TSP_COD_SPESA = SPD_COD_SPESA ";
                Sql += "AND SPD_MATRICOLA = AND_MATRICOLA ";
                Sql += "AND SPD_COD_SEDE = SED_COD_SEDE ";
                Sql += "AND(SPD_DT_DECORRENZA >= To_Date('" + V_DATA + "', 'DD-MM-YYYY')";
                Sql += "AND SPD_DT_DECORRENZA <= To_Date('" + V_DATA1 + "', 'DD-MM-YYYY')) ";
                Sql += "AND TSP_COD_SPESA NOT IN (1) ";

                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Spese__di_avvicendamento> model = new List<Stp_Spese__di_avvicendamento>();
                while (rdr.Read())
                {
                    var details = new Stp_Spese__di_avvicendamento();
                    details.MATRICOLA = rdr["MATRICOLA"].ToString();
                    details.NOMINATIVO = rdr["NOMINATIVO"].ToString();
                    details.LIVELLO = rdr["LIVELLO"].ToString();
                    details.CODICE_SEDE = rdr["CODICE_SEDE"].ToString();
                    details.DESCRIZIONE_SEDE = rdr["DESCRIZIONE_SEDE"].ToString();
                    //details.DATA = rdr["DATA"].ToString();
                    details.DATA = Convert.ToDateTime(rdr["DATA"]).ToString("dd/MM/yyyy");
                    details.SPESA = rdr["SPESA"].ToString();
                    details.INDENITA_IN_VALUTA = rdr["INDENITA_IN_VALUTA"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("SpeseAvvicendamento", model);
            }
        }

        // Report Spese Avvicendamento
        public ActionResult RptSpeseAvvicendamento(string V_DATA = "", string V_DATA1 = "")
        {
            DataSet12 ds12 = new DataSet12();
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                #region MyRegion


                String Sql = "SELECT DISTINCT SPESEDIVERSE.SPD_MATRICOLA AS MATRICOLA, ";
                Sql += "ANADIPE.AND_COGNOME || ' ' || ANADIPE.AND_NOME AS NOMINATIVO, ";
                Sql += "ANADIPE.AND_LIVELLO AS LIVELLO, ";
                Sql += "SPESEDIVERSE.SPD_COD_SEDE AS CODICE_SEDE, ";
                Sql += "SEDIESTERE.SED_DESCRIZIONE AS DESCRIZIONE_SEDE, ";
                Sql += "SPESEDIVERSE.SPD_DT_DECORRENZA AS DATA, ";
                Sql += "TIPISPESE.TSP_DESCRIZIONE AS SPESA, ";
                Sql += "SPESEDIVERSE.SPD_IMPORTO_VALUTA AS INDENITA_IN_VALUTA ";
                //Sql += "--SPESEDIVERSE.* ";
                Sql += "FROM ANADIPE, ";
                Sql += "TIPISPESE, ";
                Sql += "SPESEDIVERSE, ";
                Sql += "SEDIESTERE ";
                Sql += "WHERE TSP_COD_SPESA = SPD_COD_SPESA ";
                Sql += "AND SPD_MATRICOLA = AND_MATRICOLA ";
                Sql += "AND SPD_COD_SEDE = SED_COD_SEDE ";
                Sql += "AND(SPD_DT_DECORRENZA >= To_Date('" + V_DATA + "', 'DD-MM-YYYY')";
                Sql += "AND SPD_DT_DECORRENZA <= To_Date('" + V_DATA1 + "', 'DD-MM-YYYY')) ";
                Sql += "AND TSP_COD_SPESA NOT IN (1) ";
                Sql += "ORDER BY NOMINATIVO ASC ";
                #endregion


                // query originale

                //Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO,
                //       ANADIPE.AND_LIVELLO,
                //       SED_DESCRIZIONE,
                //       TSP_DESCRIZIONE,
                //       SPESEDIVERSE.*
                //From ANADIPE, TIPISPESE, SPESEDIVERSE, SEDIESTERE
                //Where TSP_COD_SPESA = SPD_COD_SPESA
                //And SPD_MATRICOLA = AND_MATRICOLA
                //And SPD_COD_SEDE = SED_COD_SEDE
                //And(SPD_DT_DECORRENZA >= To_Date('01-gen-2017', 'DD-MON-RRRR') And
                //   SPD_DT_DECORRENZA <= To_Date('31-dic-2017', 'DD-MON-RRRR'))
                //   And TSP_COD_SPESA Not In (1)
                //ORDER BY NOMINATIVO ASC


                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds12, ds12.V_ISE_STP_CONS_SPESE_AVVICE.TableName);
                adp.Fill(ds12, ds12.DataTable12.TableName);


                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report31.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet12", ds12.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                {
                    new ReportParameter ("fromDate",V_DATA),
                    new ReportParameter ("toDate",V_DATA1)
                };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();
        }
        
        // Stampa Livelli
        public ActionResult StampaLivelli()
        {

            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;
            OracleConnection cnn = new OracleConnection(cnnString);
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "ISE_STAMPA_ELENCO_LIVELLI";
            //add any parameters the stored procedure might require
            cmd.Parameters.Add(new OracleParameter("@V_UTENTE", "fantomas"));
            cmd.Parameters.Add(new OracleParameter("@V_DATA", "31-AGO-2017"));
            
            cnn.Open();
            //object o = cmd.ExecuteScalar();

            
            using (OracleDataReader rdr = cmd.ExecuteReader())
            {
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var details = new Stp_Dislocazione_dipendenti();
                        details.sede = rdr["SEDE"].ToString();
                        details.valuta = rdr["VALUTA"].ToString();
                        details.matricola = rdr["MATRICOLA"].ToString();
                        details.nominativo = rdr["NOMINATIVO"].ToString();
                        details.dataTrasferimento = rdr["DT_TRASFERIMENTO"].ToString();
                        details.qualifica = rdr["QUALIFICA"].ToString();
                        details.coniuge = rdr["CONIUGE"].ToString();
                        details.figli = rdr["FIGLI"].ToString();
                        details.isep = rdr["ISEP"].ToString();
                        details.contributo = rdr["CONTRIBUTO"].ToString();
                        details.uso = rdr["USO"].ToString();
                        details.totale = rdr["TOTALE"].ToString();
                        //model.Add(details);
                    }
                }
            }


            cnn.Close();
            return View();
        }


    }
    
}