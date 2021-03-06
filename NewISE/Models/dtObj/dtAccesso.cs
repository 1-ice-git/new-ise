﻿using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj
{
    public class dtAccesso : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void SetAccesso(AccessoModel am)
        {

            using (ModelDBISE db = new ModelDBISE())
            {
                var a = new ACCESSI()
                {
                    IDDIPENDENTE = am.idDipendente,
                    DATAACCESSO = am.dataAccesso,
                    GUID = am.guid.ToString()
                };

                db.ACCESSI.Add(a);

                db.SaveChanges();

            }

        }



    }
}