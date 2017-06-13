using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtDocumenti : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public DocumentiModel GetDocumento(decimal idDocumento, EntitiesDBISE db)
        {
            DocumentiModel dm = new DocumentiModel();

            var d = db.DOCUMENTI.Find(idDocumento);

            if (d != null && d.IDDOCUMENTO > 0)
            {
                
                var f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO);
                
                dm = new DocumentiModel()
                {
                    idDocumenti = d.IDDOCUMENTO,
                    NomeDocumento = d.NOMEDOCUMENTO,
                    Estensione = d.ESTENSIONE,
                    file = f
                };
            }


            return dm;
        }

        public DocumentiModel GetDocumentoByIdTrasferimento(decimal idTrasferimento, EntitiesDBISE db)
        {
            DocumentiModel dm = new DocumentiModel();

            var ld = db.TRASFERIMENTO.Find(idTrasferimento).DOCUMENTI;

            if (ld != null && ld.Count > 0)
            {
                var d = ld.First();
                var f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO);
                
                dm = new DocumentiModel()
                {
                    idDocumenti = d.IDDOCUMENTO,
                    NomeDocumento = d.NOMEDOCUMENTO,
                    Estensione = d.ESTENSIONE,
                    file = f
                };
            }


            return dm;
        }

        public DocumentiModel GetDocumentoByIdTrasferimento(decimal idTrasferimento)
        {
            DocumentiModel dm = new DocumentiModel();
            using (EntitiesDBISE db=new EntitiesDBISE())
            {
                var ld = db.TRASFERIMENTO.Find(idTrasferimento).DOCUMENTI;

                if (ld != null && ld.Count > 0)
                {
                    var d = ld.First();
                    var f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO);
                    
                    dm = new DocumentiModel()
                    {
                        idDocumenti = d.IDDOCUMENTO,
                        NomeDocumento = d.NOMEDOCUMENTO,
                        Estensione = d.ESTENSIONE,
                        file = f
                    };
                }
            }
            
            
            return dm;
        }

        public void SetDocumento(ref DocumentiModel dm, decimal idTrasferimento, EntitiesDBISE db)
        {
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();

            dm.file.InputStream.CopyTo(ms);

            var ld = db.TRASFERIMENTO.Find(idTrasferimento).DOCUMENTI;

            if (ld.Count > 0)
            {
                d = ld.First();
                d.NOMEDOCUMENTO = dm.NomeDocumento;
                d.ESTENSIONE = dm.Estensione;
                d.FILEDOCUMENTO = ms.ToArray();
                
                db.SaveChanges();

                dm.idDocumenti = d.IDDOCUMENTO;
            }
            else
            {
                d.NOMEDOCUMENTO = dm.NomeDocumento;
                d.ESTENSIONE = dm.Estensione;
                d.FILEDOCUMENTO = ms.ToArray();
                ld.Add(d);

                if(db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                }
            }
            

        }

    }
}