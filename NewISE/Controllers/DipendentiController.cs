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
                    else if (matricola != string.Empty && admin == false)
                    {
                        ldm.Add(dtd.GetDipendenteByMatricola(Convert.ToInt16(matricola)));
                    }
                    else
                    {
                        ldm = dtd.GetDipendenti().ToList();
                    }

                    if (ldm.Count > 0)
                    {
                        foreach (var item in ldm)
                        {
                            rMatricola.Add(new SelectListItem()
                            {
                                Text = item.matricola.ToString(),
                                Value = item.matricola.ToString()
                            });

                            rNominativo.Add(new SelectListItem()
                            {
                                Text = item.Nominativo + " (" + item.matricola.ToString() + ")",
                                Value = item.matricola.ToString()
                            });

                        }

                        rMatricola.Insert(0, new SelectListItem() { Text = "", Value = "" });



                        rNominativo.Insert(0, new SelectListItem() { Text = "", Value = "" });

                        if (matricola == string.Empty)
                        {
                            rMatricola.First().Selected = true;
                            rNominativo.First().Selected = true;
                        }
                        else
                        {
                            foreach (var item in rMatricola)
                            {
                                if (matricola == item.Value)
                                {
                                    item.Selected = true;
                                }
                            }

                            foreach (var item in rNominativo)
                            {
                                if (matricola == item.Value)
                                {
                                    item.Selected = true;
                                }
                            }
                        }
                    }
                }


                if (matricola != string.Empty)
                {
                    dm = ldm.Where(a => a.matricola == Convert.ToInt16(matricola)).First();

                    using (dtCDCGepe dtcdcg = new dtCDCGepe())
                    {
                        dm.cdcGepe = dtcdcg.GetCDCGepe(dm.idDipendente);
                    }

                    using (dtLivelliDipendente dtpl = new dtLivelliDipendente())
                    {
                        dm.livelloDipendenteValido = dtpl.GetLivelloDipendente(dm.idDipendente, DateTime.Now.Date);
                    }
                }


                //ViewBag.ListDipendentiGepeMatricola = rMatricola.OrderBy(a=>a.Text);
                ViewBag.ListDipendentiGepeNominativo = rNominativo.OrderBy(a => a.Text);
                ViewBag.Amministratore = admin;
                ViewBag.Matricola = dm.matricola;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(dm);
        }



    }



}