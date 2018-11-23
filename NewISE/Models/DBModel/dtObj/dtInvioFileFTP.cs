using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtInvioFileFTP : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// FlUpload
        /// </summary>
        /// <param name="idDipendente">The idDipendente<see cref="decimal"/></param>
        /// <param name="idMeseAnnoElaborato">The idMeseAnnoElaborato<see cref="decimal"/></param>
        /// <param name="idTeorico">The idTeorico<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <param name="strLocalFilePath">C:\Users\UTENTE\Desktop</param>
        /// <param name="strFTPFilePath">ftp://wm-egesia-test.ice.it/FileStipendi</param>
        /// <param name="strUserName">utente1</param>
        /// <param name="strPassword">Mf3826@fat</param>


        public void FlUpload(decimal idDipendente, decimal idAnnoMeseElaborato, /*decimal idTeorico,*/ ModelDBISE db)
        //public void FlUpload(ModelDBISE db)
        {
            try
            {
                List<TRASFERIMENTO> lt = new List<TRASFERIMENTO>();
                
                var dip = db.DIPENDENTI.Find(idDipendente);
                
                lt = db.TRASFERIMENTO.Where(a =>
                                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                            a.IDDIPENDENTE == idDipendente)
                                        .ToList();

                foreach (var t in lt)
                {
                    var d = t.DIPENDENTI;
                    var nome = d.NOME;
                    var cognome = d.COGNOME;
                    var matricola = d.MATRICOLA;
                    var codiceufficio = t.UFFICI.CODICEUFFICIO;
                    var ufficio = t.UFFICI.DESCRIZIONEUFFICIO + " (" + codiceufficio + ")";
                    //decimal idVoci = 0;

                    var llivdip = t.INDENNITA.LIVELLIDIPENDENTI
                                                    .Where(a => a.ANNULLATO == false)
                                                    .ToList();

                    foreach (var livdip in llivdip)
                    {
                        var liv = livdip.LIVELLI;


                        //idVoci = (decimal)EnumVociCedolino.Detrazione_086_384;

                        var lTeorici = t.TEORICI.Where(a => a.IDMESEANNOELAB == idAnnoMeseElaborato &&
                                                            a.ANNULLATO == false &&
                                                            a.DIRETTO == false &&
                                                            a.ELABORATO == true &&
                                                            //a.IDVOCI == idVoci &&
                                                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe)
                                                .OrderBy(a => a.ANNORIFERIMENTO)
                                                .ThenBy(a => a.MESERIFERIMENTO).ToList();
                        

                        if (lTeorici?.Any() ?? false)
                        {
                            string pathtemp_idfile = System.Web.HttpContext.Current.Server.MapPath("\\Temp\\");
                            StreamWriter outputFile = new StreamWriter(Path.Combine(pathtemp_idfile, "FL052A-EEE.txt"));

                            foreach (var teorico in lTeorici)
                            {
                                var idMeseAnnoElaborato = teorico.MESEANNOELABORAZIONE.IDMESEANNOELAB;
                                var tm = teorico.TIPOMOVIMENTO;
                                var voce = teorico.VOCI;
                                var tl = teorico.VOCI.TIPOLIQUIDAZIONE;
                                var tv = teorico.VOCI.TIPOVOCE;
                                var tr = teorico.TRASFERIMENTO;
                                var NrDato = teorico.VOCI.CODICEVOCE;
                                var Scheda = "42";
                                var CodAzienda = "0001";
                                var CodMatr = String.Format("{0:000000}", dip.MATRICOLA);
                                var Filler1 = "  ";
                                var Filler2 = "00000000000000000000";
                                var Filler3 = "000000000000   ";
                                var CodCosto = "0000000000";
                                var CodGruppo = "0";

                                //var liv = teorico.ELABINDENNITA;
                                //var livello = liv.First();
                                var idlivello = liv.IDLIVELLO;

                                // IDLIVELLO == 41 -- Dirigente
                                if (liv.IDLIVELLO == 41)
                                {
                                    CodGruppo = "0001";
                                }
                                else
                                {
                                    CodGruppo = "0003";
                                }

                                var Valore = teorico.IMPORTO.ToString().PadLeft(10,'0').Replace(",","");
                                //var valore = teorico.IMPORTO;
                                //var Valore = valore.ToString().PadLeft(9, '0');

                                var CodFormula = teorico.VOCI.CODICEVOCE;

                                if (teorico.IMPORTO < 0)
                                {
                                    NrDato="5" + NrDato.Substring(5, 3);
                                }
                                else
                                {
                                    NrDato= "0" + NrDato.Substring(0, 3);
                                }

                                var codformula = CodFormula.Substring(4, 3);
                                
                                //string[] lines = { "First line", "Second line", "Third line", "Invio File Stipendi" };

                                outputFile.WriteLine(Scheda + CodAzienda + CodGruppo + CodMatr + Filler1 + NrDato + Filler2 + Valore + CodCosto + codformula + Filler3);

                                //TipoScheda = Scheda & CodAzienda & CodGruppo & CodMatr & Filler1 & NrDato & Filler2 & Valore & CodCosto & CodFormula & Filler3
                                //string[] lines = { Scheda + CodAzienda + CodGruppo + CodMatr + Filler1 + NrDato + Filler2 + Valore + CodCosto + codformula + Filler3 };

                                ////// Set a variable to the My Documents path.
                                ////string mydocpath =
                                ////    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                                //string pathtemp_idfile = System.Web.HttpContext.Current.Server.MapPath("\\Temp\\");
                                //using (StreamWriter outputFile = new StreamWriter(Path.Combine(pathtemp_idfile, "FL052A-EEE.txt")))
                                //{
                                //    foreach (string line in lines)
                                //        outputFile.WriteLine(line);
                                //}


                            }

                            outputFile.Close();

                            String strFTPFilePath = "ftp://vm-egesia-test.ice.it/Gepe_Svil/FILES/FL052A-EEE";

                            //String strUserName = "daniele";
                            //String strPassword = "fant$$$ini";

                            String strUserName = "gepe";
                            String strPassword = "gepe";

                            string strLocalFilePath = @pathtemp_idfile + "FL052A-EEE.txt";

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
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}