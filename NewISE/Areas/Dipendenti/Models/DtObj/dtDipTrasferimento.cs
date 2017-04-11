using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Dipendenti.Models.DtObj
{
    public class dtDipTrasferimento : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public dipInfoTrasferimentoModel GetInfoTrasferimento(string matricola)
        {
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();
            TrasferimentoModel tm = new TrasferimentoModel();
            DateTime dtDatiParametri;
            
            try
            {
                using (dtTrasferimento dtt=new dtTrasferimento())
                {
                    tm = dtt.GetUltimoTrasferimentoByMatricola(matricola);

                    if (tm != null && tm.idTrasferimento > 0)
                    {
                        dit.statoTrasferimento = (EnumStatoTraferimento)tm.idStatoTrasferimento;
                        dit.UfficioDestinazione = tm.ufficio;
                        dit.Decorrenza = tm.dataPartenza;
                        dit.RuoloUfficio = tm.RuoloUfficio;

                        if (tm.dataRientro.HasValue)
                        {
                            dtDatiParametri = tm.dataRientro.Value;
                        }
                        else
                        {
                            dtDatiParametri = DateTime.Now.Date;
                        }

                        





                    }


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return dit;
        }


    }
}