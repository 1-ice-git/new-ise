﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class MaggiorazioniFamiliariController : Controller
    {
        // GET: MaggiorazioniFamiliari
        public ActionResult MaggiorazioniFamiliari()
        {
            return PartialView();
        }
    }
}