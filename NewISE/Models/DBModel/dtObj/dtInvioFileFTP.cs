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
                //TEORICI teorico = db.TEORICI.Find(idTeorico);

                var dip = db.DIPENDENTI.Find(idDipendente);
                
                //int fantone = Convert.ToInt32(idDipendente);
                //var Matricola = Convert.ToDecimal(dip.MATRICOLA);
                //var prova = dip.MATRICOLA.ToString();

                var lTeorici =
                    db.TEORICI.Where(
                            a =>
                                a.ANNULLATO == false && a.DIRETTO == false && a.ELABORATO == true &&
                                a.IDMESEANNOELAB == idAnnoMeseElaborato && /*a.IDTEORICI == idTeorico &&*/
                                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                                a.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE == idDipendente)
                        .OrderBy(a => a.ANNORIFERIMENTO)
                        .ThenBy(a => a.MESERIFERIMENTO)
                        .ToList();

                if (lTeorici?.Any() ?? false)
                {
                    foreach (var teorico in lTeorici)
                    {
                        var idMeseAnnoElaborato = teorico.MESEANNOELABORAZIONE.IDMESEANNOELAB;
                        var tm = teorico.TIPOMOVIMENTO;
                        var voce = teorico.VOCI;
                        var tl = teorico.VOCI.TIPOLIQUIDAZIONE;
                        var tv = teorico.VOCI.TIPOVOCE;
                        var tr = teorico.TRASFERIMENTO;
                        var dipendente = db.DIPENDENTI.Find(idDipendente).MATRICOLA;
                        //var dipendente = dip.MATRICOLA;
                        var Matricola = Convert.ToDecimal(dip.MATRICOLA);
                        var Scheda = "42";
                        var CodAzienda = "0001";
                        var CodGruppo = "0003";
                        var Filler1 = "  ";
                        var Filler2 = "00000000000000000000";
                        var Filler3 = "000000000000    ";
                        var CodCosto = "0000000000";
                        var Valore = teorico.IMPORTO;

                        //If pRs!stp_matricola < 6000 Then
                        //    CodGruppo = IIf(Dirigente(pRs!stp_matricola) = True, "0002", "0001")
                        //Else
                        //    CodGruppo = "0003"
                        //End If

                        //string[] lines = { "First line", "Second line", "Third line", "Invio File Stipendi" };

                        //TipoScheda = Scheda & CodAzienda & CodGruppo & CodMatr & Filler1 & NrDato & Filler2 & Valore & CodCosto & CodFormula & Filler3
                        string[] lines = { Scheda + CodAzienda + CodGruppo + dipendente + Filler1 + Filler2 + Valore + CodCosto + Filler3 };

                        //// Set a variable to the My Documents path.
                        //string mydocpath =
                        //    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                        string pathtemp_idfile = System.Web.HttpContext.Current.Server.MapPath("\\Temp\\");
                        using (StreamWriter outputFile = new StreamWriter(Path.Combine(pathtemp_idfile, "FL052A-EEE.txt")))
                        {
                            foreach (string line in lines)
                                outputFile.WriteLine(line);
                        }

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

            catch (Exception ex)
            {
                    throw ex;
            }



// Function TipoScheda(pVoce, pRs) As String

//   '----------------------------------------------------------------------------------------------
//   ' Questa function deve essere sempre allineata con la function VoceFormula del modulo Generale.
//   '----------------------------------------------------------------------------------------------
//   Dim Scheda, CodAzienda, CodGruppo, CodMatr, Filler1, Filler2 As String
//   Dim CodFormula, CodCosto, Filler3 As String
//   Scheda = "42"
//   CodAzienda = "0001"

//   If pRs!stp_matricola < 6000 Then
//      CodGruppo = IIf(Dirigente(pRs!stp_matricola) = True, "0002", "0001")
//   Else
//      CodGruppo = "0003"
//   End If

//   CodMatr = Format(pRs!stp_matricola, "000000")
//   Filler1 = "  "

//   If pRs!stp_importo < 0 Then


//'<Modified by: Project Administrator at 04/08/2010-09.55.54 on machine: ITROMC50424>
//      '<20100804-001-MAl>
//      ' Modificato il valore con cui vengono codificati i valori negativi nel file FL052A-EEE
//      ' per adattarla alla nuova versione di GePe XXL.
//      '</20100804-001-MAl>
//      ''''' NrDato = "1" & Mid(pRs!Stp_Codice, 1, 3)
//      NrDato = "5" & Mid(pRs!Stp_Codice, 1, 3)
//'</Modified by: Project Administrator at 04/08/2010-09.55.54 on machine: ITROMC50424>


//   Else
//      NrDato = "0" & Mid(pRs!Stp_Codice, 1, 3)
//   End If

//   Filler2 = "00000000000000000000"
//   Valore = Sostituisci_Carattere(Format(Round(Abs(pRs!stp_importo), "E"), "0000000.00"), ",", "")
//   CodCosto = "0000000000"
//   CodFormula = Mid(pRs!Stp_Codice, 4, 3)
//   Filler3 = "000000000000    "
//   TipoScheda = Scheda & CodAzienda & CodGruppo & CodMatr & Filler1 & NrDato & Filler2 & Valore & CodCosto & CodFormula & Filler3
//   ''' 20081216-001-AMa ***** Fine
//End Function

        }
    }
}