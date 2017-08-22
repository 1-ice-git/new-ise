using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public IList<DocumentiModel> GetDocumentiByIdConiuge(decimal idConiuge)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ld = db.CONIUGE.Find(idConiuge).DOCUMENTI.ToList();
                if (ld != null && ld.Count > 0)
                {

                    foreach (var d in ld)
                    {


                        var f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO);
                        var dm = new DocumentiModel()
                        {
                            idDocumenti = d.IDDOCUMENTO,
                            nomeDocumento = d.NOMEDOCUMENTO,
                            estensione = d.ESTENSIONE,
                            tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                            dataInserimento = d.DATAINSERIMENTO,
                            file = f
                        };

                        ldm.Add(dm);
                    }



                }


            }

            return ldm;
        }

        public IList<DocumentiModel> GetDocumentiByIdFiglio(decimal idFiglio)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ld = db.FIGLI.Find(idFiglio).DOCUMENTI.ToList();
                if (ld?.Any() ?? false)
                {
                    ldm.AddRange(from d in ld
                                 let f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO)
                                 select new DocumentiModel()
                                 {
                                     idDocumenti = d.IDDOCUMENTO,
                                     nomeDocumento = d.NOMEDOCUMENTO,
                                     estensione = d.ESTENSIONE,
                                     dataInserimento = d.DATAINSERIMENTO,
                                     tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                                     file = f
                                 });
                }

            }

            return ldm;
        }



        public DocumentiModel GetDocumento(decimal idDocumento, ModelDBISE db)
        {
            DocumentiModel dm = new DocumentiModel();

            var d = db.DOCUMENTI.Find(idDocumento);

            if (d != null && d.IDDOCUMENTO > 0)
            {
                var f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO);

                dm = new DocumentiModel()
                {
                    idDocumenti = d.IDDOCUMENTO,
                    nomeDocumento = d.NOMEDOCUMENTO,
                    estensione = d.ESTENSIONE,
                    tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                    dataInserimento = d.DATAINSERIMENTO,
                    file = f
                };
            }

            return dm;
        }

        /// <summary>
        /// Verifica la presenza della lettera di trasferimento. Può esistere solo una lettera di trasferimento.
        /// </summary>
        /// <param name="idTrasferimento"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool HasLetteraTrasferimento(decimal idTrasferimento, ModelDBISE db)
        {
            bool ret = false;

            var ld = db.TRASFERIMENTO.Find(idTrasferimento).DOCUMENTI;

            if (ld != null && ld.Count > 0)
            {
                ret = true;
            }
            else
            {
                ret = false;
            }

            return ret;
        }

        public void RimuoviLetteraTrasferimento(decimal idTrasferimento, ModelDBISE db)
        {
            var ld = db.TRASFERIMENTO.Find(idTrasferimento).DOCUMENTI;
            if (ld != null && ld.Count > 0)
            {
                foreach (var e in ld)
                {
                    db.TRASFERIMENTO.Find(idTrasferimento).DOCUMENTI.Remove(e);
                    Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione della lettera di trasferimento.", "Documenti", db, idTrasferimento, e.IDDOCUMENTO);
                }

                db.SaveChanges();
            }
        }

        public DocumentiModel GetDocumentoByIdTrasferimento(decimal idTrasferimento, ModelDBISE db)
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
                    nomeDocumento = d.NOMEDOCUMENTO,
                    estensione = d.ESTENSIONE,
                    tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                    dataInserimento = d.DATAINSERIMENTO,
                    file = f
                };
            }

            return dm;
        }

        public DocumentiModel GetDocumentoByIdTrasferimento(decimal idTrasferimento)
        {
            DocumentiModel dm = new DocumentiModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                var ld = db.TRASFERIMENTO.Find(idTrasferimento).DOCUMENTI;

                if (ld != null && ld.Count > 0)
                {
                    var d = ld.First();
                    HttpPostedFileBase f;

                    f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO, d.NOMEDOCUMENTO + d.ESTENSIONE, "application/pdf");

                    dm = new DocumentiModel()
                    {
                        idDocumenti = d.IDDOCUMENTO,
                        nomeDocumento = d.NOMEDOCUMENTO,
                        estensione = d.ESTENSIONE,
                        tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                        dataInserimento = d.DATAINSERIMENTO,
                        file = f
                    };
                }
            }

            return dm;
        }

        public DocumentiModel GetDatiDocumentoByIdTrasferimento(decimal idTrasferimento)
        {
            DocumentiModel dm = new DocumentiModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                var ld = db.TRASFERIMENTO.Find(idTrasferimento).DOCUMENTI;

                if (ld != null && ld.Count > 0)
                {
                    var d = ld.First();
                    //HttpPostedFileBase f;

                    //f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO, d.NOMEDOCUMENTO + d.ESTENSIONE, "application/pdf");

                    dm = new DocumentiModel()
                    {
                        idDocumenti = d.IDDOCUMENTO,
                        nomeDocumento = d.NOMEDOCUMENTO,
                        estensione = d.ESTENSIONE,
                        tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                        dataInserimento = d.DATAINSERIMENTO,
                        //file = f
                    };
                }
            }

            return dm;
        }

        public DocumentiModel GetDatiDocumentoById(decimal idDocumento)
        {
            DocumentiModel dm = new DocumentiModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                var d = db.DOCUMENTI.Find(idDocumento);

                if (d != null && d.IDDOCUMENTO > 0)
                {

                    dm = new DocumentiModel()
                    {
                        idDocumenti = d.IDDOCUMENTO,
                        nomeDocumento = d.NOMEDOCUMENTO,
                        estensione = d.ESTENSIONE,
                        tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                        dataInserimento = d.DATAINSERIMENTO,
                        //file = f
                    };
                }
            }

            return dm;
        }

        public byte[] GetDocumentoByteById(decimal idDocumento)
        {
            byte[] blob = null;

            using (ModelDBISE db = new ModelDBISE())
            {
                var d = db.DOCUMENTI.Find(idDocumento);

                if (d != null && d.IDDOCUMENTO > 0)
                {
                    blob = d.FILEDOCUMENTO;
                }
            }

            return blob;
        }

        public byte[] GetDocumentoByteById(decimal idDocumento, ModelDBISE db)
        {
            byte[] blob = null;

            var d = db.DOCUMENTI.Find(idDocumento);

            if (d != null && d.IDDOCUMENTO > 0)
            {
                blob = d.FILEDOCUMENTO;
            }

            return blob;
        }

        public void SetDocumento(ref DocumentiModel dm, ModelDBISE db)
        {
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();
            dm.file.InputStream.CopyTo(ms);

            d.NOMEDOCUMENTO = dm.nomeDocumento;
            d.ESTENSIONE = dm.estensione;
            d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
            d.DATAINSERIMENTO = dm.dataInserimento;
            d.FILEDOCUMENTO = ms.ToArray();

            db.DOCUMENTI.Add(d);

            if (db.SaveChanges() > 0)
            {
                dm.idDocumenti = d.IDDOCUMENTO;
            }


        }

        public void SetLetteraTrasferimento(ref DocumentiModel dm, decimal idTrasferimento, ModelDBISE db)
        {
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();

            dm.file.InputStream.CopyTo(ms);

            var ld = db.TRASFERIMENTO.Find(idTrasferimento).DOCUMENTI;

            if (ld.Count > 0)
            {
                d = ld.First();
                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.LetteraTrasferimento_Trasferimento5;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (lettera di trasferimento).", "Documenti", db, idTrasferimento, dm.idDocumenti);
                }


            }
            else
            {
                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.LetteraTrasferimento_Trasferimento5;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                ld.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (lettera di trasferimento).", "Documenti", db, idTrasferimento, dm.idDocumenti);
                }
            }
        }

        public void AddDocumentoMagFamConiuge(ref DocumentiModel dm, decimal idMaggiorazioneConiuge, ModelDBISE db)
        {
            var c = db.CONIUGE.Find(idMaggiorazioneConiuge);

            if (c != null && c.IDCONIUGE > 0)
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                dm.file.InputStream.CopyTo(ms);

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();

                c.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                }
            }


        }

        public void DeleteDocumento(decimal idDocumento)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var d = db.DOCUMENTI.Find(idDocumento);

                    if (d != null && d.IDDOCUMENTO > 0)
                    {
                        db.DOCUMENTI.Remove(d);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Non è stato possibile effettuare l'eliminazione del documento ({0}).", d.NOMEDOCUMENTO + d.ESTENSIONE));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



    }
}