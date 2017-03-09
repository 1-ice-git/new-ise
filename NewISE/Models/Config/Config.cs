using NewISE.Models.Config.s_admin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace NewISE.Models.Config
{
    public class Config : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public sAdmin SuperAmministratore()
        {
            sAdmin sa = new sAdmin();
        
            try
            {
                //C:\Users\yoravas\documents\visual studio 2015\Projects\NewISE\NewISE\Models\Config\s_admin\s_admin.json
                using (StreamReader sr = new StreamReader(VirtualPathProvider.OpenFile("/Models/Config/s_admin/s_admin.json")))
                {
                    string content = sr.ReadToEnd();
                    sa = JsonConvert.DeserializeObject<sAdmin>(content);
                }

                
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return sa;

        }
    }
}