using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.dtObj.Interfacce;

namespace NewISE.Models.dtObj
{
    public class dtRicalcoloParametri : IDisposable, IricalcoloParametri
    {
        public void AssociaConiuge_PMC(decimal idPercMagConiuge, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaFiglio_PMF(decimal idPercFiflio, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaFigli_IPS(decimal idPrimoSegretario, ModelDBISE db)
        {

            //var ps = db.PRIMASITEMAZIONE.Find(idPrimaSitemazione);

            //var item = db.Entry<PRIMASITEMAZIONE>(ps);
            //item.State = EntityState.Modified;
            //item.Collection(a => a.INDENNITASISTEMAZIONE).Load();
            //var indSist = db.INDENNITASISTEMAZIONE.Find(idIndennitaSitemazione);
            //ps.INDENNITASISTEMAZIONE.Add(indSist);
            //int i = db.SaveChanges();

            //if (i <= 0)
            //{
            //    throw new Exception("Non è stato possibile associare l'indennità di sistemazione alla prima sistemazione.");
            //}


            try
            {
                var ps = db.INDENNITAPRIMOSEGRETARIO.Find(idPrimoSegretario);
                var item = db.Entry<INDENNITAPRIMOSEGRETARIO>(ps);
                item.State = EntityState.Modified;
                item.Collection(a => a.FIGLI).Load();
                //var f = db.FIGLI
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaIndenita_PD(decimal idPercDisagio, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaIndennitaBase_IB(decimal idIndBase, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaIndennita_CS(decimal idCoefficenteSede, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaMAB_PMAB(decimal idPerceMAB, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaMaggiorazioniAbitazione_MA(decimal idMagAnnuali, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaPagatoCondiviso(decimal idPercentualeCondivisione, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaPercentualeAnticipoTEP(decimal idPercentualeAnticipoTEP, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaPercentualeAnticipoTER(decimal idPercentualeAnticipoTEP, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaPrimaSistemazione_IS(decimal idIndSistemazione, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaPrimaSistemazione_PKM(decimal idPercKM, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaRichiamo_CR(decimal idCoeffRichiamo, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaRichiamo_PKM(decimal idPercKM, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaRiduzioniIB(decimal idIndBase, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaRiduzioni_CR(decimal idCoeffRichiamo, ModelDBISE db)
        {
            throw new NotImplementedException();
        }

        public void AssociaRiduzioni_IS(decimal idIndSistemazione, ModelDBISE db)
        {
            throw new NotImplementedException();
        }



        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


    }
}