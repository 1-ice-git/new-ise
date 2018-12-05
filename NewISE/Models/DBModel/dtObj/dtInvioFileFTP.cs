using NewISE.EF;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtInvioFileFTP : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
      
        public void FlUpload(List<decimal> lTeorici, ModelDBISE db)
        {
            try
            {

                bool invia_file = false;
                string pathtemp_idfile = System.Web.HttpContext.Current.Server.MapPath("~");
                string strLocalFile = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPCedolinoNomeFileLocal"]);
                string strLocalFilePath = @pathtemp_idfile + strLocalFile;

                #region apre il file
                StreamWriter outputFile = new StreamWriter(Path.Combine(pathtemp_idfile, strLocalFile));
                #endregion

                if (lTeorici?.Any() ?? false)
                {
                    foreach (var teorici in lTeorici)
                    {
                        var teorici_row = db.TEORICI.Find(teorici);

                        #region verifica che la riga è relativa a PAGHE
                        if (
                                teorici_row.ANNULLATO == false &&
                                teorici_row.DIRETTO == false &&
                                teorici_row.ELABORATO == true &&
                                teorici_row.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe
                        )
                        {
                            #region imposta stringhe fisse
                            var Scheda = "42";
                            var CodAzienda = "0001";
                            var CodMatr = String.Format("{0:000000}", teorici_row.TRASFERIMENTO.DIPENDENTI.MATRICOLA);
                            var Filler1 = new String(' ', 2);  //Filler1 = "  ";
                            var Filler2 = new String('0', 20); //Filler2 = "00000000000000000000";
                            var Filler3 = new String('0', 12) + new String(' ', 4); //Filler3 = "000000000000    ";
                            var CodCosto = new String('0', 10);// CodCosto = "0000000000";
                            var CodGruppo = "0";
                            #endregion

                            invia_file = true;
                            decimal idLivello = 0;
                            bool livellotrovato = false;

                            #region verifica se il record è relativo a PRIMA SISTEMAZIONE
                            if (teorici_row?.IDINDSISTLORDA > 0)
                            {
                                idLivello = teorici_row.ELABINDSISTEMAZIONE.IDLIVELLO;
                                livellotrovato = true;
                            }
                            #endregion

                            #region verifica se il record è relativo a RICHIAMO
                            if (teorici_row?.IDELABINDRICHIAMO > 0)
                            {
                                idLivello = teorici_row.ELABINDRICHIAMO.IDLIVELLO;
                                livellotrovato = true;
                            }
                            #endregion

                            #region verifica se il record è relativo a TRASPORTO EFFETTI
                            if (teorici_row?.IDELABTRASPEFFETTI > 0)
                            {
                                idLivello = teorici_row.ELABTRASPEFFETTI.IDLIVELLO;
                                livellotrovato = true;
                            }
                            #endregion

                            #region verifica se il record è relativo a VOCI MANUALI
                            if (teorici_row?.IDAUTOVOCIMANUALI > 0)
                            {
                                livellotrovato = true;

                                var meserif = teorici_row.MESERIFERIMENTO;
                                var annorif = teorici_row.ANNORIFERIMENTO;
                                DateTime primogiornorif= Convert.ToDateTime("01/" + meserif + "/" + annorif);
                                DateTime ultimogiornorif= Utility.GetDtFineMese(primogiornorif);

                                idLivello = teorici_row.AUTOMATISMOVOCIMANUALI
                                                        .TRASFERIMENTO
                                                        .DIPENDENTI
                                                        .LIVELLIDIPENDENTI
                                                        .Where(a => a.ANNULLATO == false &&
                                                                    a.DATAINIZIOVALIDITA <= ultimogiornorif &&
                                                                    a.DATAFINEVALIDITA >= ultimogiornorif)
                                                        .ToList()
                                                        .First()
                                                        .IDLIVELLO;
                            }
                            #endregion

                            #region se il livello non viene trovato va in errore
                            if (livellotrovato == false)
                            {
                                throw new Exception(string.Format("Livello non trovato nella corrispondenza sulla tabella TEORICI (idteorici={0})", teorici));
                            }
                            #endregion

                            var l_livDirigente = db.LIVELLI.Where(a => a.LIVELLO == "D").OrderByDescending(a=>a.IDLIVELLO).ToList();

                            #region se non esiste il livello DIRIGENTE va in errore
                            if (!(l_livDirigente?.Any() ?? false))
                            {
                                throw new Exception("Non esiste il livello DIRIGENTE (D) nella tabella LIVELLI");
                            }
                            #endregion

                            #region imposta codice gruppo
                            if (idLivello == l_livDirigente.First().IDLIVELLO) // Dirigente
                            {
                                CodGruppo = "0002";
                            }
                            else
                            {
                                CodGruppo = "0001";
                            }

                            #endregion

                            #region imposta importo
                            var Importo = Math.Abs(Math.Round(teorici_row.IMPORTO, 2));
                            var importosenzavirgola = Importo.ToString().Replace(",", "");
                            var importoPadded = importosenzavirgola.PadLeft(9, '0');
                            var Valore = importoPadded;
                            //var Valore = Importo.ToString().PadLeft(10, '0').Replace(",", "");

                            #endregion
                            if (Importo == 0)
                            {
                                var val = 0;
                                var val1 = val.ToString().PadLeft(10, '0').Replace(",", "");
                            }

                            var CodFormula = teorici_row.VOCI.CODICEVOCE;
                            string NrDato = CodFormula;

                            #region imposta NrDato
                            if (teorici_row.IMPORTO < 0)
                            {
                                NrDato = "5" + NrDato.Substring(0, 3);
                            }
                            else
                            {
                                NrDato = "0" + NrDato.Substring(0, 3);
                            }
                            #endregion

                            var codformula = CodFormula.Substring(4, 3);

                            outputFile.WriteLine(Scheda + CodAzienda + CodGruppo + CodMatr + Filler1 + NrDato + Filler2 + Valore + CodCosto + codformula + Filler3);
                        }
                        #endregion 
                    }
                }

                #region chiude il file
                outputFile.Close();
                #endregion

                #region se necessario invia il file
                if (invia_file)
                {
                    string strFTPFilePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPCedolinoPath"]);
                    string strUserName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPCedolinoUser"]);
                    string strPassword = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPCedolinoPassword"]);

                    //Create a FTP Request Object and Specfiy a Complete Path
                    FtpWebRequest reqObj = (FtpWebRequest)WebRequest.Create(strFTPFilePath);

                    //Call A FileUpload Method of FTP Request Object
                    reqObj.Method = WebRequestMethods.Ftp.UploadFile;

                    //If you want to access Resourse Protected,give UserName and PWD
                    reqObj.Credentials = new NetworkCredential(strUserName, strPassword);

                    // Copy the contents of the file to the byte array.
                    byte[] fileContents = File.ReadAllBytes(strLocalFilePath);
                    reqObj.ContentLength = fileContents.Length;

                    //Upload File to FTPServer
                    Stream requestStream = reqObj.GetRequestStream();
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();
                    FtpWebResponse response = (FtpWebResponse)reqObj.GetResponse();
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}