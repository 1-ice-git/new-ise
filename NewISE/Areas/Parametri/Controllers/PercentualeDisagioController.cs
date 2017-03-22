using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NewISE;

namespace NewISE.Areas.Parametri.Controllers
{
    public class PercentualeDisagioController : Controller
    {
        private EntitiesDBISEPRO db = new EntitiesDBISEPRO();

        // GET: Parametri/PercentualeDisagio
        public ActionResult Index()
        {
            var pERCENTUALEDISAGIO = db.PERCENTUALEDISAGIO.Include(p => p.UFFICI);
            return View(pERCENTUALEDISAGIO.ToList());
            
        }

        // GET: Parametri/PercentualeDisagio/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PERCENTUALEDISAGIO pERCENTUALEDISAGIO = db.PERCENTUALEDISAGIO.Find(id);
            if (pERCENTUALEDISAGIO == null)
            {
                return HttpNotFound();
            }
            return View(pERCENTUALEDISAGIO);
        }

        // GET: Parametri/PercentualeDisagio/Create
        public ActionResult Create()
        {
            ViewBag.IDUFFICIO = new SelectList(db.UFFICI, "IDUFFICIO", "CODICEUFFICIO");
            return View();
        }

        // POST: Parametri/PercentualeDisagio/Create
        // Per proteggere da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per ulteriori dettagli, vedere http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDPERCENTUALEDISAGIO,IDUFFICIO,DATAINIZIOVALIDITA,DATAFINEVALIDITA,PERCENTUALE,ANNULLATO")] PERCENTUALEDISAGIO pERCENTUALEDISAGIO)
        {
            if (ModelState.IsValid)
            {
                db.PERCENTUALEDISAGIO.Add(pERCENTUALEDISAGIO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDUFFICIO = new SelectList(db.UFFICI, "IDUFFICIO", "CODICEUFFICIO", pERCENTUALEDISAGIO.IDUFFICIO);
            return View(pERCENTUALEDISAGIO);
        }

        // GET: Parametri/PercentualeDisagio/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PERCENTUALEDISAGIO pERCENTUALEDISAGIO = db.PERCENTUALEDISAGIO.Find(id);
            if (pERCENTUALEDISAGIO == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDUFFICIO = new SelectList(db.UFFICI, "IDUFFICIO", "CODICEUFFICIO", pERCENTUALEDISAGIO.IDUFFICIO);
            return View(pERCENTUALEDISAGIO);
        }

        // POST: Parametri/PercentualeDisagio/Edit/5
        // Per proteggere da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per ulteriori dettagli, vedere http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDPERCENTUALEDISAGIO,IDUFFICIO,DATAINIZIOVALIDITA,DATAFINEVALIDITA,PERCENTUALE,ANNULLATO")] PERCENTUALEDISAGIO pERCENTUALEDISAGIO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pERCENTUALEDISAGIO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDUFFICIO = new SelectList(db.UFFICI, "IDUFFICIO", "CODICEUFFICIO", pERCENTUALEDISAGIO.IDUFFICIO);
            return View(pERCENTUALEDISAGIO);
        }

        // GET: Parametri/PercentualeDisagio/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PERCENTUALEDISAGIO pERCENTUALEDISAGIO = db.PERCENTUALEDISAGIO.Find(id);
            if (pERCENTUALEDISAGIO == null)
            {
                return HttpNotFound();
            }
            return View(pERCENTUALEDISAGIO);
        }

        // POST: Parametri/PercentualeDisagio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            PERCENTUALEDISAGIO pERCENTUALEDISAGIO = db.PERCENTUALEDISAGIO.Find(id);
            db.PERCENTUALEDISAGIO.Remove(pERCENTUALEDISAGIO);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
