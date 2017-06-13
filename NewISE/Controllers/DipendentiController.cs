using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class DipendentiController : Controller
    {
        // GET: Dipendenti
        public ActionResult Index()
        {
            bool admin = false;

            try
            {
                admin = Utility.Amministratore();

                ViewBag.Amministratore = admin;
            }
            catch (Exception)
            {

                return View("Error");
            }


            return View();
        }


        public ActionResult DipendentiGepe(string matricola = "")
        {
            var rMatricola = new List<SelectListItem>();
            var rNominativo = new List<SelectListItem>();
            bool admin = false;
            List<DipendentiModel> ldm = new List<DipendentiModel>();
            DipendentiModel dm = new DipendentiModel();
            AccountModel ac = new AccountModel();
            try
            {

                admin = Utility.Amministratore(out ac);

                using (dtDipendenti dtd = new dtDipendenti())
                {
                    if (!admin)
                    {
                        ldm.Add(dtd.GetDipendenteByMatricola(Convert.ToInt16(ac.utente)));
                    }
                    else if (matricola != string.Empty)
                    {
                        ldm.Add(dtd.GetDipendenteByMatricola(Convert.ToInt16(matricola)));
                    }
                    else
                    {
                        ldm = dtd.GetDipendenti().ToList();
                    }

                    if (ldm.Count > 0)
                    {
                        foreach (var item in ldm.OrderBy(a => a.matricola))
                        {
                            rMatricola.Add(new SelectListItem()
                            {
                                Text = item.matricola.ToString(),
                                Value = item.matricola.ToString(),
                                Selected = ldm.Count == 1 ? true : false
                            });
                        }

                        foreach (var item in ldm.OrderBy(a => a.Nominativo))
                        {
                            rNominativo.Add(new SelectListItem()
                            {
                                Text = item.Nominativo,
                                Value = item.matricola.ToString(),
                                Selected = ldm.Count == 1 ? true : false
                            });
                        }
                    }
                }

                dm = ldm.First();

                using (dtCDCGepe dtcdcg = new dtCDCGepe())
                {
                    dm.cdcGepe = dtcdcg.GetCDCGepe(dm.idDipendente);
                }

                using (dtLivelliDipendente dtpl = new dtLivelliDipendente())
                {
                    dm.livelloDipendenteValido = dtpl.GetLivelloDipendente(dm.idDipendente, DateTime.Now.Date);
                }


                ViewBag.ListDipendentiGepeMatricola = rMatricola;
                ViewBag.ListDipendentiGepeNominativo = rNominativo;
                ViewBag.Amministratore = admin;
                ViewBag.Matricola = dm.matricola;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }

            return PartialView(dm);
        }

        

    }



}