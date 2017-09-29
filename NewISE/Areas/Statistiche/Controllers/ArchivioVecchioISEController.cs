﻿using Microsoft.Reporting.WebForms;
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
                t.Add(new SelectListItem() { Text = "Operazioni effettuate nel periodo", Value = "3" });
                t.Add(new SelectListItem() { Text = "Presenze dei livelli in servizio all'estero", Value = "4" });
                t.Add(new SelectListItem() { Text = "Spese diverse", Value = "5" });
                t.Add(new SelectListItem() { Text = "Spese di avvicendamento", Value = "6" });
                t.Add(new SelectListItem() { Text = "Storia del dipendente", Value = "7" });

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
  
        public ActionResult RptStoriaDipendente(string matricola)
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
                
                adp.Fill(ds6, ds6.STP_STORIA_DIP_ISESTOR.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report8.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet6", ds6.Tables[0]));

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
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

            //lcm = Dipendenti.GetAllDipendenti().ToList();

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
                                details.DATA_DECORRENZA = rdr["DATA_DECORRENZA"].ToString();
                                details.DATA_LETTERA = rdr["DATA_LETTERA"].ToString();
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

            //lcm = Dipendenti.GetAllSedi().ToList();

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



            //string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

            //OracleConnection cnn = new OracleConnection(cnnString);
            //OracleCommand cmd = new OracleCommand();
            //cmd.Connection = cnn;
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //cmd.CommandText = "ISE_STAMPA_ELENCO_TRASF";
            ////add any parameters the stored procedure might require
            //cnn.Open();
            //object o = cmd.ExecuteScalar();
            //cnn.Close();


            //SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]);
            //con.Open();



            //SqlCommand Cmd = new SqlCommand("usp_CheckEmailMobile", con);
            //Cmd.CommandType = CommandType.StoredProcedure;
            //Cmd.CommandText = "Registration";
            //Cmd.Parameters.AddWithValue("@Name", txtName.Text);
            //Cmd.Parameters.AddWithValue("@Email", txtEmailAddress.Text);
            //Cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
            //Cmd.Parameters.AddWithValue("@CountryCode", ddlCountryCode.Text);
            //Cmd.Parameters.AddWithValue("@Mobile", txtMobileNumber.Text);
            ////Cmd.Parameters.Add("@Result", DbType.Boolean);
            //SqlParameter sqlParam = new SqlParameter("@Result", DbType.Boolean);
            ////sqlParam.ParameterName = "@Result";
            ////sqlParam.DbType = DbType.Boolean;
            //sqlParam.Direction = ParameterDirection.Output;
            //Cmd.Parameters.Add(sqlParam);
            //Cmd.ExecuteNonQuery();
            //con.Close();
            //Response.Write(Cmd.Parameters["@Result"].Value);


            //bool result = false;
            //SqlCommand scCommand = new SqlCommand("usp_CheckEmailMobile", sqlCon);
            //scCommand.CommandType = CommandType.StoredProcedure;
            //scCommand.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = txtName.Text;
            //scCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = txtEmailAddress.Text;
            //scCommand.Parameters.Add("@Password ", SqlDbType.NVarChar, 50).Value = txtPassword.Text;
            //scCommand.Parameters.Add("@CountryCode", SqlDbType.VarChar.50).Value = ddlCountryCode.SelectedText;
            //scCommand.Parameters.Add("@Mobile", SqlDbType.NVarChar, 50).Value = txtMobileNumber.Text;
            //scCommand.Parameters.Add("@Result ", SqlDbType.Bit).Direction = ParameterDirection.Output;
            //try
            //{
            //    if (scCommand.Connection.State == ConnectionState.Closed)
            //    {
            //        scCommand.Connection.Open();
            //    }
            //    scCommand.ExecuteNonQuery();
            //    result = Convert.ToBoolean(scCommand.Parameters["@Result"].Value);


            //}
            //catch (Exception)
            //{

            //}
            //finally
            //{
            //    scCommand.Connection.Close();
            //    Response.Write(result);
            //}


            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {

                //OracleConnection cnn = new OracleConnection(cn);
                OracleCommand cmd1 = new OracleCommand();
                cmd1.Connection = cn;
                cmd1.CommandText = "ISE_STAMPA_ELENCO_TRASF";
                cmd1.CommandType = System.Data.CommandType.StoredProcedure;

                ////add any parameters the stored procedure might require
                //scCommand.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = txtName.Text;

                cmd1.Parameters.Add("@V_UTENTE", OracleDbType.Varchar2, 50).Value = V_UTENTE;
                cmd1.Parameters.Add("@V_DATA", OracleDbType.Varchar2, 50).Value = V_DATA;
                cmd1.Parameters.Add("@V_UFFICIO", OracleDbType.Varchar2, 50).Value = V_UFFICIO;

                //cmd1.Parameters.Add("@V_UTENTE", "fantomas");
                //cmd1.Parameters.Add("@V_DATA", "15/09/2017");
                //cmd1.Parameters.Add("@V_UFFICIO", "BUDAPEST");

                cn.Open();
                cmd1.ExecuteNonQuery();
                
                //String Sql = "Select distinct SEDE, VALUTA, MATRICOLA, NOMINATIVO, DT_TRASFERIMENTO, QUALIFICA,        CONIUGE, FIGLI, ISEP, CONTRIBUTO, USO, ISEP + CONTRIBUTO + USO TOTALE From ISE_STP_ELENCOTRASFERIMENTI WHERE UTENTE ='@V_UTENTE' Order By SEDE, NOMINATIVO";

                String Sql = "Select distinct SEDE, VALUTA, MATRICOLA, NOMINATIVO, DT_TRASFERIMENTO, QUALIFICA,        CONIUGE, FIGLI, ISEP, CONTRIBUTO, USO, ISEP + CONTRIBUTO + USO TOTALE From ISE_STP_ELENCOTRASFERIMENTI WHERE UTENTE ='"+ V_UTENTE + "' Order By SEDE, NOMINATIVO";

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
                                details.dataTrasferimento = rdr["DT_TRASFERIMENTO"].ToString();
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
        public ActionResult RptDislocazione(string V_UTENTE = "fantomas")
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
                //#region MyRegion

                //String Sql = "Select MATRICOLA, ";
                //Sql += "NOMINATIVO, ";
                //Sql += "SEDE, ";
                //Sql += "VALUTA, ";
                //Sql += "DESCRIZIONE, ";
                //Sql += "IMPORTO, ";
                //Sql += "TIPOIMPORTO, ";
                //Sql += "QUALIFICA, ";
                //Sql += "CODSEDE ";
                //Sql += "From ISE_STP_CONSUNTIVOCOSTI ";
                //Sql += "Order By SEDE, NOMINATIVO, DESCRIZIONE ";
                //#endregion

                String Sql = "Select distinct SEDE, VALUTA, MATRICOLA, NOMINATIVO, DT_TRASFERIMENTO, QUALIFICA,        CONIUGE, FIGLI, ISEP, CONTRIBUTO, USO, ISEP + CONTRIBUTO + USO TOTALE From ISE_STP_ELENCOTRASFERIMENTI WHERE UTENTE ='" + V_UTENTE + "' Order By SEDE, NOMINATIVO";

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                adp.Fill(ds15, ds15.V_ISE_STP_ELENCO_TRASF.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report19.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet15", ds15.Tables[0]));

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Consuntivo Costi
        public ActionResult ConsuntivoCosti()
        {
            // Chiamata Visual Basic

            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
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

                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Consuntivo_dei_costi> model = new List<Stp_Consuntivo_dei_costi>();
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
                    
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("ConsuntivoCosti", model);
            }
        }

        // Report Consuntivo Costi
        public ActionResult RptConsuntivoCosti()
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

                adp.Fill(ds9, ds9.V_ISE_STP_CONS_COSTI.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report12.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet9", ds9.Tables[0]));

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Consuntivo Costi CoAn
        public ActionResult ConsuntivoCostiCoAn(string codicecoan ="")
        {

            ViewBag.ListaCodiceCoan = new List<SelectListItem>();
            List<CodiceCoanModel> lcm = new List<CodiceCoanModel>();
            List<SelectListItem> lr = new List<SelectListItem>();
            List<Stp_Consuntivo_dei_costi_per_codice_Coan> lsd = new List<Stp_Consuntivo_dei_costi_per_codice_Coan>();
            List<Stp_Consuntivo_dei_costi_per_codice_Coan> model = new List<Stp_Consuntivo_dei_costi_per_codice_Coan>();

            //lcm = Dipendenti.GetAllCostiCoan().ToList();

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
                    lr.First().Selected = true;
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

            //using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            //{

            //    String Sql = "SELECT C.MATRICOLA, ";
            //    Sql += "C.NOMINATIVO, ";
            //    Sql += "C.SEDE, ";
            //    Sql += "C.VALUTA , ";
            //    Sql += "C.DESCRIZIONE, ";
            //    Sql += "SUM(C.IMPORTO) IMPORTO, ";
            //    Sql += "C.TIPOIMPORTO, ";
            //    Sql += "C.QUALIFICA, ";
            //    Sql += "C.CODSEDE, ";
            //    Sql += "DECODE(C.COAN, 'S', 'Serv. Ist.', C.COAN) COAN ";
            //    Sql += "FROM ISE_STP_CONSUNTIVOCOSTICOAN C ";
            //    Sql += "WHERE 1 = 1 ";
            //    Sql += "AND C.COAN = 'S' ";
            //    Sql += "GROUP BY C.MATRICOLA, ";
            //    Sql += "C.NOMINATIVO, ";
            //    Sql += "C.SEDE, ";
            //    Sql += "C.VALUTA, ";
            //    Sql += "C.DESCRIZIONE, ";
            //    Sql += "C.TIPOIMPORTO, ";
            //    Sql += "C.QUALIFICA, ";
            //    Sql += "C.CODSEDE, ";
            //    Sql += "C.COAN ";
            //    Sql += "ORDER BY COAN, NOMINATIVO, SEDE, DESCRIZIONE ";

            //    OracleCommand cmd = new OracleCommand(Sql, cn);
            //    cn.Open();
            //    OracleDataReader rdr = cmd.ExecuteReader();
            //    List<Stp_Consuntivo_dei_costi_per_codice_Coan> model = new List<Stp_Consuntivo_dei_costi_per_codice_Coan>();
            //    while (rdr.Read())
            //    {
            //        var details = new Stp_Consuntivo_dei_costi_per_codice_Coan();

            //        details.matricola = rdr["MATRICOLA"].ToString();
            //        details.nominativo = rdr["NOMINATIVO"].ToString();
            //        details.ufficio = rdr["SEDE"].ToString();
            //        details.valuta = rdr["VALUTA"].ToString();
            //        details.descrizione = rdr["DESCRIZIONE"].ToString();
            //        details.tipoimporto = rdr["TIPOIMPORTO"].ToString();
            //        details.livello = rdr["QUALIFICA"].ToString();
            //        details.codsede = rdr["CODSEDE"].ToString();
            //        details.euro = rdr["IMPORTO"].ToString();
            //        details.coan= rdr["COAN"].ToString();
            //        model.Add(details);
            //    }
            //    //return View("ViewName", model);
            //    return PartialView("ConsuntivoCostiCoAn", model);
            //}

            return PartialView("ConsuntivoCostiCoAn", model);

        }

        // Report Consuntivo Costi CoAn
        public ActionResult RptConsuntivoCostiCoAn()
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

                adp.Fill(ds10, ds10.V_ISE_STP_CONS_COSTI_COAN.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report13.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet10", ds10.Tables[0]));

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
        public ActionResult OpIndennitaEstera()
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
                Sql += "And(IES_DT_OPERAZIONE >= To_Date('01-gen-2017', 'DD-MON-RRRR') And ";
                Sql += "IES_DT_OPERAZIONE <= To_Date('25-set-2017', 'DD-MON-RRRR')) ";
                //Sql += "IES_DT_OPERAZIONE <= To_Date(SysDate)) ";
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
                    details.data_decorrenza = rdr["DATA_DECORRENZA"].ToString();
                    details.data_lettera = rdr["DATA_LETTERA"].ToString();
                    details.data_operazione = rdr["DATA_OPERAZIONE"].ToString();
                    details.indennita_personale = rdr["ISEP"].ToString();
                    details.sist_rientro_lorda = rdr["SISTEMAZIONE_RIENTRO"].ToString();
                    details.anticipo = rdr["ANTICIPO"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpIndennitaEstera", model);
            }
        }

        // Report Operazioni Effettuate - Indennità di Sede Estera
        public ActionResult RptOpIndennitaEstera()
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
                #region MyRegion

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
                Sql += "And(IES_DT_OPERAZIONE >= To_Date('01-gen-2017', 'DD-MON-RRRR') And ";
                Sql += "IES_DT_OPERAZIONE <= To_Date('25-set-2017', 'DD-MON-RRRR')) ";
                //Sql += "IES_DT_OPERAZIONE <= To_Date(SysDate)) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_COD_TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "IES_PROG_MOVIMENTO ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                adp.Fill(ds7, ds7.V_OP_EFFETTUATE_IND_ESTERA.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report9.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet7", ds7.Tables[0]));

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Operazioni Effettuate - Contributo Abitazione
        public ActionResult OpContributoAbitazione()
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
                Sql += "And(CON_DT_OPERAZIONE >= To_Date('01-gen-2000', 'DD-MON-RRRR') And ";
                Sql += "CON_DT_OPERAZIONE <= To_Date(SysDate)) ";
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
                    details.data_decorrenza = rdr["CON_DT_DECORRENZA"].ToString();
                    details.data_lettera = rdr["CON_DT_LETTERA"].ToString();
                    details.data_operazione = rdr["CON_DT_OPERAZIONE"].ToString();
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
        public ActionResult RptOpContributoAbitazione()
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
                Sql += "And(CON_DT_OPERAZIONE >= To_Date('01-gen-2000', 'DD-MON-RRRR') And ";
                Sql += "CON_DT_OPERAZIONE <= To_Date(SysDate)) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "CON_PROG_TRASFERIMENTO, ";
                Sql += "CON_DT_DECORRENZA, ";
                Sql += "CON_PROG_CONTRIBUTO_ABITAZIONE ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                adp.Fill(ds8, ds8.V_OP_EFFETTUATE_CONTR_ABITAZ.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report8.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet8", ds8.Tables[0]));

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Operazioni Effettuate - Uso Abitazione
        public ActionResult OpUsoAbitazione()
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
                Sql += "And(USO_DT_OPERAZIONE >= To_Date('01-gen-2000', 'DD-MON-RRRR') And ";
                Sql += "USO_DT_OPERAZIONE <= To_Date(SysDate)) ";
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
                    details.data_decorrenza = rdr["DATA_DECORRENZA"].ToString();
                    details.data_lettera = rdr["DATA_LETTERA"].ToString();
                    details.data_operazione = rdr["DATA_OPERAZIONE"].ToString();
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
        public ActionResult RptOpUsoAbitazione()
        {
            return View();
        }

        // Operazioni Effettuate - Canone Anticipato
        public ActionResult OpCanoneAnticipato()
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
                Sql += "And(CAN_DT_OPERAZIONE >= To_Date('01-gen-2000', 'DD-MON-RRRR') And ";
                Sql += "CAN_DT_OPERAZIONE <= To_Date('31-lug-2017', 'DD-MON-RRRR')) ";
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
                    details.data_decorrenza = rdr["DATA_DECORRENZA"].ToString();
                    details.data_lettera = rdr["DATA_LETTERA"].ToString();
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
        public ActionResult RptOpCanoneAnticipato()
        {
            return View();
        }

        // Operazioni Effettuate - Spese Diverse
        public ActionResult OpSpeseDiverse()
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                

                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "SPD_MATRICOLA, ";
                Sql += "SED_DESCRIZIONE, ";
                Sql += "TSP_DESCRIZIONE, ";
                Sql += "SPD_DT_DECORRENZA, ";
                Sql += "SPD_DT_OPERAZIONE, ";
                Sql += "SPD_IMPORTO_LIRE, ";
                Sql += "SPD_TIPO_MOVIMENTO, ";
                Sql += "SPD_PROG_SPESA, ";
                Sql += "SPD_PROG_TRASFERIMENTO, ";
                Sql += "SPD_TIPO_MOVIMENTO ";
                Sql += "From SPESEDIVERSE, SEDIESTERE, ANADIPE, TIPISPESE ";
                Sql += "Where SPD_COD_SEDE = SED_COD_SEDE ";
                Sql += "And SPD_COD_SPESA = TSP_COD_SPESA ";
                Sql += "And SPD_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(SPD_DT_OPERAZIONE >= To_Date('01-gen-2017', 'DD-MON-RRRR') And ";
                Sql += "SPD_DT_OPERAZIONE <= To_Date('31-lug-2017', 'DD-MON-RRRR')) ";
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
                    details.matricola = rdr["SPD_MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.sede = rdr["SED_DESCRIZIONE"].ToString();
                    //details.valuta = rdr["VALUTA"].ToString();
                    details.tipo_spesa = rdr["TSP_DESCRIZIONE"].ToString();
                    details.data_decorrenza = rdr["SPD_DT_DECORRENZA"].ToString();
                    details.data_operazione = rdr["SPD_DT_OPERAZIONE"].ToString();
                    details.importo_spesa = rdr["SPD_IMPORTO_LIRE"].ToString();
                    details.partenza_rientro = rdr["SPD_TIPO_MOVIMENTO"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpSpeseDiverse", model);
            }
        }

        // Report Operazioni Effettuate - Spese Diverse
        public ActionResult RptOpSpeseDiverse()
        {
            return View();
        }

        // Operazioni Effettuate - Maggiorazione Abitazione
        public ActionResult OpMaggiorazioneAbitazione()
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
                Sql += "AND(RMAB.MAB_RAT_DATARATA >= To_Date('01-gen-2017', 'DD-MON-RRRR') AND ";
                Sql += "RMAB.MAB_RAT_DATARATA <= To_Date(SysDate)) ";
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
                    details.data_decorrenza = rdr["DATADECORRENZA"].ToString();
                    details.data_lettera = rdr["DATALETTERA"].ToString();
                    details.data_operazione = rdr["DATAOPERAZIONE"].ToString();
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
        public ActionResult RptOpMaggiorazioneAbitazione()
        {
            return View();
        }

        // Presenze Livelli in servizio all' Estero
        public ActionResult PresenzeLivelli(string codicequalifica = "")
        {
            // Combo Qualifiche
            //Select Distinct IBS_DESCRIZIONE From INDENNITABASE Order By IBS_DESCRIZIONE

            ViewBag.ListaDipendentiEsteroLivello = new List<SelectListItem>();
            List<DipEsteroLivelloModel> lcm = new List<DipEsteroLivelloModel>();
            List<SelectListItem> lr = new List<SelectListItem>();
            List<Stp_Presenze_Livelli> lsd = new List<Stp_Presenze_Livelli>();
            List<Stp_Presenze_Livelli> model = new List<Stp_Presenze_Livelli>();

                     

            //lcm = Dipendenti.GetAllQualifiche().ToList();

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
                    lr.First().Selected = true;
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
                Sql += "And A.IES_DT_DECORRENZA <= To_Date('01-gen-2017', 'DD-MON-RRRR') ";
                Sql += "GROUP BY A.IES_MATRICOLA) MAXDATA ";
                Sql += "Where(IES_DT_TRASFERIMENTO <= To_Date('28-ago-2017', 'DD-MON-RRRR')) ";
                Sql += "And(TRA_DT_FIN_TRASFERIMENTO Is Null Or ";
                Sql += "To_Date(TRA_DT_FIN_TRASFERIMENTO) > ";
                Sql += "To_Date('01-gen-2017', 'DD-MON-RRRR')) ";
                Sql += "And MAXDATA.MATR(+) = IES_MATRICOLA ";
                Sql += "And(IES_DT_DECORRENZA > To_Date('01-gen-2017', 'DD-MON-RRRR') Or ";
                Sql += "IES_DT_DECORRENZA = MAXDATA.DATAM) ";
                Sql += "And IES_FLAG_RICALCOLATO Is Null ";
                Sql += "And IES_COD_QUALIFICA = IBS_COD_QUALIFICA ";
                Sql += "And IES_MATRICOLA = AND_MATRICOLA ";
                Sql += "And IES_COD_SEDE = SED_COD_SEDE ";
                Sql += "And IES_MATRICOLA = TRA_MATRICOLA ";
                Sql += "And IES_PROG_TRASFERIMENTO = TRA_PROG_TRASFERIMENTO ";
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
                                details.dt_Trasferimento = rdr["IES_DT_TRASFERIMENTO"].ToString();
                                details.dt_Rientro = rdr["TRA_DT_FIN_TRASFERIMENTO"].ToString();
                                details.dt_Decorrenza = rdr["IES_DT_DECORRENZA"].ToString();
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

        // Report Presenze Livelli
        public ActionResult RptPresenzeLivelli()
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

                String Sql = "Select distinct CODQUALIFICA, QUALIFICA, NOMINATIVO, MATRICOLA, SEDE, DT_TRASFERIMENTO, DT_RIENTRO, DT_DECORRENZA From ISE_STP_LIVELLIESTERI Order By QUALIFICA, NOMINATIVO";

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                adp.Fill(ds13, ds13.V_ISE_STP_LIVELLI_ESTERI.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report17.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet13", ds13.Tables[0]));

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Spese Diverse
        public ActionResult SpeseDiverse()
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {

                // Insert into ISE_STP_SPESEDIVERSE
                //String Sql = "Select DECODE(TIPO, 'P', 'SPESE DI TRASFERIMENTO', 'SPESE DI RIENTRO') TIPO, ";
                //Sql += "MATRICOLA, ";
                //Sql += "NOMINATIVO, ";
                //Sql += "CODLIVELLO, ";
                //Sql += "CODSEDE, ";
                //Sql += "SEDE, ";
                //Sql += "VALUTA, ";
                //Sql += "DT_TRASFERIMENTO, ";
                //Sql += "INITCAP(SPESA) SPESA, ";
                //Sql += "IMPORTOSPESA ";
                //Sql += "From ISE_STP_SPESEDIVERSE ";
                //Sql += "Order By TIPO Desc, NOMINATIVO, VALUTA ";


                //String Sql = "Select DECODE(TIPO,'P', 'SPESE DI TRASFERIMENTO', 'SPESE DI RIENTRO') TIPO, MATRICOLA, NOMINATIVO, CODLIVELLO, CODSEDE,        SEDE, VALUTA, DT_TRASFERIMENTO, INITCAP(SPESA) SPESA, IMPORTOSPESA From ISE_STP_SPESEDIVERSE Where UTENTE = 'uteadm' Order By TIPO Desc, NOMINATIVO, VALUTA ";

                // Query da modificare
                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "ANADIPE.AND_LIVELLO, ";
                Sql += "SED_DESCRIZIONE, ";
                Sql += "TSP_DESCRIZIONE, ";
                Sql += "SPESEDIVERSE.* ";
                Sql += "From ANADIPE, TIPISPESE, SPESEDIVERSE, SEDIESTERE ";
                Sql += "Where TSP_COD_SPESA = SPD_COD_SPESA ";
                Sql += "And SPD_MATRICOLA = AND_MATRICOLA ";
                Sql += "And SPD_COD_SEDE = SED_COD_SEDE ";
                Sql += "And (SPD_DT_DECORRENZA >= To_Date('01-gen-2017', 'DD-MON-RRRR') ";
                Sql += "And SPD_DT_DECORRENZA <= To_Date('25-set-2017', 'DD-MON-RRRR')) ";
  

                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Spese_diverse> model = new List<Stp_Spese_diverse>();
                while (rdr.Read())
                {
                    var details = new Stp_Spese_diverse();
                    //details.tipo = rdr["TIPO"].ToString();
                    details.matricola = rdr["MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.codlivello = rdr["CODLIVELLO"].ToString();
                    details.codsede = rdr["CODSEDE"].ToString();
                    details.sede = rdr["SEDE"].ToString();
                    details.valuta = rdr["VALUTA"].ToString();
                    details.dt_trasferiento = rdr["DT_TRASFERIMENTO"].ToString();
                    details.spesa = rdr["SPESA"].ToString();
                    details.importospesa = rdr["IMPORTOSPESA"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("SpeseDiverse", model);
            }
        }

        // Report Spese Diverse
        public ActionResult RptSpeseDiverse()
        {
            DataSet11 ds11 = new DataSet11();

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

                String Sql = "Select DECODE(TIPO, 'P', 'SPESE DI TRASFERIMENTO', 'SPESE DI RIENTRO') TIPO, ";
                Sql += "MATRICOLA, ";
                Sql += "NOMINATIVO, ";
                Sql += "CODLIVELLO, ";
                Sql += "CODSEDE, ";
                Sql += "SEDE, ";
                Sql += "VALUTA, ";
                Sql += "DT_TRASFERIMENTO, ";
                Sql += "INITCAP(SPESA) SPESA, ";
                Sql += "IMPORTOSPESA ";
                Sql += "From ISE_STP_SPESEDIVERSE ";
                Sql += "Order By TIPO Desc, NOMINATIVO, VALUTA ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                adp.Fill(ds11, ds11.V_ISE_STP_SPESE_DIVERSE.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report14.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet11", ds11.Tables[0]));

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Spese Avvicendamento
        public ActionResult SpeseAvvicendamento()
        {

            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {

                String Sql = "Select DECODE(TIPO, 'P', 'SPESE DI TRASFERIMENTO', 'SPESE DI RIENTRO') TIPO, ";
                Sql += "MATRICOLA, ";
                Sql += "NOMINATIVO, ";
                Sql += "CODLIVELLO, ";
                Sql += "CODSEDE, ";
                Sql += "SEDE, ";
                Sql += "VALUTA, ";
                Sql += "DT_TRASFERIMENTO, ";
                Sql += "INITCAP(SPESA) SPESA, ";
                Sql += "IMPORTOSPESA ";
                Sql += "From ISE_STP_CONSUNTIVOSPESEDIVERSE ";
                Sql += "Order By TIPO Desc, NOMINATIVO, VALUTA ";

                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Spese__di_avvicendamento> model = new List<Stp_Spese__di_avvicendamento>();
                while (rdr.Read())
                {
                    var details = new Stp_Spese__di_avvicendamento();
                    details.TIPO = rdr["TIPO"].ToString();
                    details.MATRICOLA = rdr["MATRICOLA"].ToString();
                    details.NOMINATIVO = rdr["NOMINATIVO"].ToString();
                    details.CODLIVELLO = rdr["CODLIVELLO"].ToString();
                    details.CODSEDE = rdr["CODSEDE"].ToString();
                    details.SEDE = rdr["SEDE"].ToString();
                    details.VALUTA = rdr["VALUTA"].ToString();
                    details.DT_TRASFERIMENTO = rdr["DT_TRASFERIMENTO"].ToString();
                    details.SPESA = rdr["SPESA"].ToString();
                    details.IMPORTOSPESA = rdr["IMPORTOSPESA"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("SpeseAvvicendamento", model);
            }
        }

        // Report Spese Avvicendamento
        public ActionResult RptSpeseAvvicendamento()
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

                String Sql = "Select DECODE(TIPO, 'P', 'SPESE DI TRASFERIMENTO', 'SPESE DI RIENTRO') TIPO, ";
                Sql += "MATRICOLA, ";
                Sql += "NOMINATIVO, ";
                Sql += "CODLIVELLO, ";
                Sql += "CODSEDE, ";
                Sql += "SEDE, ";
                Sql += "VALUTA, ";
                Sql += "DT_TRASFERIMENTO, ";
                Sql += "INITCAP(SPESA) SPESA, ";
                Sql += "IMPORTOSPESA ";
                Sql += "From ISE_STP_CONSUNTIVOSPESEDIVERSE ";
                Sql += "Order By TIPO Desc, NOMINATIVO, VALUTA ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                adp.Fill(ds12, ds12.V_ISE_STP_CONS_SPESE_DIVERSE.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report16.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet12", ds12.Tables[0]));

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }


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