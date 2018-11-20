using NewISE.EF;
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
        /// <param name="idMeseAnnoElaborato">The idMeseAnnoElaborato<see cref="decimal"/></param>
        /// <param name="idTeorico">The idTeorico<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <param name="strLocalFilePath">C:\Users\UTENTE\Desktop</param>
        /// <param name="strFTPFilePath">ftp://wm-egesia-test.ice.it/FileStipendi</param>
        /// <param name="strUserName">utente1</param>
        /// <param name="strPassword">Mf3826@fat</param>


        public void FlUpload(decimal idAnnoMeseElaborato, decimal idTeorico, ModelDBISE db)
        //public void FlUpload(ModelDBISE db)
        {
            try
            {
                //TEORICI teorico = db.TEORICI.Find(idTeorico);

                //var lTeorici =
                //    db.TEORICI.Where(
                //            a =>
                //                a.ANNULLATO == false && a.DIRETTO == true && a.ELABORATO == true &&
                //                a.IDMESEANNOELAB == idAnnoMeseElaborato && a.IDTEORICI == idTeorico &&
                //                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe)
                //        .OrderBy(a => a.ANNORIFERIMENTO)
                //        .ThenBy(a => a.MESERIFERIMENTO)
                //        .ToList();

                var lTeorici =
                    db.TEORICI
                        .ToList();


                if (lTeorici?.Any() ?? false)
                {
                    foreach (var teorico in lTeorici)
                    {


                        var idMeseAnnoElaborato = teorico.MESEANNOELABORAZIONE.IDMESEANNOELAB;

                        //string[] lines = { "First line", "Second line", "Third line", "Invio File Stipendi" };
                        string[] lines = { idMeseAnnoElaborato.ToString(), "Second line", "Third line", "Invio File Stipendi" };

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


        }
    }
}