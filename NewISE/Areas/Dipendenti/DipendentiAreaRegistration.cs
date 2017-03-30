using System.Web.Mvc;

namespace NewISE.Areas.Dipendenti
{
    public class DipendentiAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Dipendenti";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Dipendenti_default",
                "Dipendenti/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Dipendenti",
                "Dipendenti/Dipendenti/Index"
            );
        }
    }
}