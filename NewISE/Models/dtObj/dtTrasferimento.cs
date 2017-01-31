using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj
{
    public class dtTrasferimento : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void InsTrasferimento(TrasferimentoModel ptrasf)
        {
            try
            {
                TRASFERIMENTO trasf = new TRASFERIMENTO();

                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    trasf.IDTRASFERIMENTO = ptrasf.idTrasferimento;
                    trasf.IDTIPOTRASFERIMENTO = ptrasf.idTipoTrasferimento;
                    trasf.IDUFFICIO = ptrasf.idUfficio;
                    trasf.IDSTATOTRASFERIMENTO = ptrasf.idStatoTrasferimento;
                    trasf.IDRUOLO = ptrasf.idRuolo;
                    trasf.DATAPARTENZA = ptrasf.dataPartenza;
                    trasf.DATARIENTRO = ptrasf.dataRientro;
                    trasf.COAN = ptrasf.coan;
                    trasf.PROTOCOLLOLETTERA = ptrasf.protocolloLettera;
                    trasf.DATALETTERA = ptrasf.dataLettera;
                    trasf.ANNULLATO = ptrasf.annullato;

                    db.TRASFERIMENTO.Add(trasf);

                    db.SaveChanges();


                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        // read-only instance property
        public string ReadTrasferimento;
        public string Trasferimento
        {
            get
            {
                return ReadTrasferimento;
            }
        }


    }
}