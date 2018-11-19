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


namespace NewISE.Models.DBModel.dtObj
{
    public class dtInvioFileFTP : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        private string host = null;
        private string user = null;
        private string pass = null;
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 2048;

        /// <summary>
        /// FlUpload
        /// </summary>
        /// <param name="idMeseAnnoElaborato">The idMeseAnnoElaborato<see cref="decimal"/></param>
        /// <param name="idTeorico">The idTeorico<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        public void FlUpload(decimal idAnnoMeseElaborato, decimal idTeorico, ModelDBISE db)
        {
            try
            {
                    //TEORICI teorico = db.TEORICI.Find(idTeorico);

                    var lTeorici =
                        db.TEORICI.Where(
                                a =>
                                    a.ANNULLATO == false && a.DIRETTO == true && a.ELABORATO == false &&
                                    a.IDMESEANNOELAB == idAnnoMeseElaborato && a.IDTEORICI == idTeorico &&
                                    a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe)
                            .OrderBy(a => a.ANNORIFERIMENTO)
                            .ThenBy(a => a.MESERIFERIMENTO)
                            .ToList();

                    if (lTeorici?.Any() ?? false)
                    {

                        // *****************************************************************************************

                        //// Create a string array with the lines of text
                        //string[] lines = { "First line", "Second line", "Third line"};

                        //// Set a variable to the My Documents path.
                        //string mydocpath =
                        //    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                        //// Write the string array to a new file named "WriteLines.txt".
                        //using (StreamWriter outputFile = new StreamWriter(Path.Combine(mydocpath, "FL052A-EEE.txt")))
                        //{
                        //    foreach (string line in lines)
                        //        outputFile.WriteLine(line);
                        //}

                    // *****************************************************************************************

                    ///* Create an FTP Request */
                    //ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                    ///* Log in to the FTP Server with the User Name and Password Provided */
                    //ftpRequest.Credentials = new NetworkCredential(user, pass);
                    ///* When in doubt, use these options */
                    //ftpRequest.UseBinary = true;
                    //ftpRequest.UsePassive = true;
                    //ftpRequest.KeepAlive = true;
                    ///* Specify the Type of FTP Request */
                    //ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                    ///* Establish Return Communication with the FTP Server */
                    //Stream ftpStream = ftpRequest.GetRequestStream();
                    ///* Open a File Stream to Read the File for Upload */
                    //FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                    ///* Buffer for the Downloaded Data */
                    //byte[] byteBuffer = new byte[bufferSize];
                    //int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                    ///* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
                    //try
                    //{
                    //    while (bytesSent != 0)
                    //    {
                    //        ftpStream.Write(byteBuffer, 0, bytesSent);
                    //        bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                    //    }
                    //}
                    //catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                    ///* Resource Cleanup */
                    //localFileStream.Close();
                    //ftpStream.Close();
                    //ftpRequest = null;




                    string server = "ftp://wm-egesia-test.ice.it/Example/"; //server path

                    string name = @"C:\Users\utente1\Desktop\FileStipendi"; //image path
                    //string Imagename = Path.GetFileName(name);

                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}{1}", server)));

                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    //request.Credentials = new NetworkCredential("username", "password");
                    Stream ftpStream = request.GetRequestStream();
                    FileStream fs = File.OpenRead(name);
                    byte[] buffer = new byte[1024];
                    int byteRead = 0;
                    do
                    {
                        byteRead = fs.Read(buffer, 0, 1024);
                        ftpStream.Write(buffer, 0, byteRead);
                    }
                    while (byteRead != 0);
                    fs.Close();
                    ftpStream.Close();

                }


                


                }
                catch (Exception ex)
                {
                    throw ex;
                }

        }


    }
}