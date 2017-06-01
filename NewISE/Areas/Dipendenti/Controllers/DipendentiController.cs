using NewISE.Areas.Dipendenti.Models;
using NewISE.Areas.Dipendenti.Models.DtObj;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Dipendenti.Controllers
{
    public class DipendentiController : Controller
    {
        // GET: Dipendenti/Dipendenti
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DipendentiGepe(string matricola = "")
        {
            var rMatricola = new List<SelectListItem>();
            var rNominativo = new List<SelectListItem>();
            bool admin = false;
            List<DipendentiModel> ldm = new List<DipendentiModel>();
            DipendentiModel dm = new DipendentiModel();

            try
            {
                AccountModel ac = new AccountModel();
                ac = Utility.UtenteAutorizzato();
                if (ac != null)
                {
                    if (ac.idRuoloUtente == 1 || ac.idRuoloUtente == 2)
                    {
                        admin = true;
                    }
                    else
                    {
                        admin = false;
                    }
                }

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

                using (dtDipCDCGepe dtcdcg=new dtDipCDCGepe())
                {
                    dm.cdcGepe = dtcdcg.GetCDCGepe(dm.idDipendente);
                }


                using (dtDipLivelli dtpl=new dtDipLivelli())
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

        public ActionResult InfoTrasferimento(string matricola)
        {
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();

            try
            {
                using (dtDipTrasferimento dtdt = new dtDipTrasferimento())
                {
                    dit = dtdt.GetInfoTrasferimento(matricola);

                    if (dit.CDCDestinazione == string.Empty)
                    {
                        dit.statoTrasferimento = EnumStatoTraferimento.Non_Trasferito;
                        dit.UfficioDestinazione = new UfficiModel();
                        dit.RuoloUfficio = new RuoloUfficioModel();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }

            return PartialView(dit);
        }
    }
}