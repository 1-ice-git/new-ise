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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipodoc"></param>
        /// <param name="parentela">Utilizzato solo per i documenti di identità (per i viaggi)</param>
        /// <returns></returns>
        public IList<DocumentiModel> GetDocumentiByIdTable(decimal id, EnumTipoDoc tipodoc, EnumParentela parentela = EnumParentela.Richiedente)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                List<DOCUMENTI> ld = new List<DOCUMENTI>();

                switch (tipodoc)
                {
                    case EnumTipoDoc.CartaImbarco_Viaggi1:
                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                ld =
                                    db.CONIUGE.Find(id)
                                        .DOCUMENTI.Where(
                                            a => a.IDTIPODOCUMENTO == (decimal)tipodoc)
                                        .ToList();
                                break;
                            case EnumParentela.Figlio:
                                ld =
                                    db.FIGLI.Find(id)
                                        .DOCUMENTI.Where(
                                            a => a.IDTIPODOCUMENTO == (decimal)tipodoc)
                                        .ToList();
                                break;
                            case EnumParentela.Richiedente:
                                ld =
                                    db.TITOLIVIAGGIO.Find(id)
                                        .DOCUMENTI.Where(
                                            a => a.IDTIPODOCUMENTO == (decimal)tipodoc)
                                        .ToList();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }
                        break;
                    case EnumTipoDoc.TitoloViaggio_Viaggi1:
                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                ld =
                                    db.CONIUGE.Find(id)
                                        .DOCUMENTI.Where(
                                            a => a.IDTIPODOCUMENTO == (decimal)tipodoc)
                                        .ToList();
                                break;
                            case EnumParentela.Figlio:
                                ld =
                                    db.FIGLI.Find(id)
                                        .DOCUMENTI.Where(
                                            a => a.IDTIPODOCUMENTO == (decimal)tipodoc)
                                        .ToList();
                                break;
                            case EnumParentela.Richiedente:
                                ld =
                                    db.TITOLIVIAGGIO.Find(id)
                                        .DOCUMENTI.Where(
                                            a => a.IDTIPODOCUMENTO == (decimal)tipodoc)
                                        .ToList();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }
                        break;
                    case EnumTipoDoc.PrimaRataMab_MAB2:
                        break;
                    case EnumTipoDoc.DichiarazioneCostoLocazione_MAB2:
                        break;
                    case EnumTipoDoc.AttestazioneSpeseAbitazione_MAB2:
                        break;
                    case EnumTipoDoc.ClausoleContrattoAlloggio_MAB2:
                        break;
                    case EnumTipoDoc.CopiaContrattoLocazione_MAB2:
                        break;
                    case EnumTipoDoc.ContributoFissoOmnicomprensivo_TrasportoEffetti3:
                        break;
                    case EnumTipoDoc.AttestazioneTrasloco_TrasportoEffetti3:
                        break;
                    case EnumTipoDoc.DocumentoFamiliareConiuge_MaggiorazioniFamiliari4:
                        ld = db.CONIUGE.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                        break;
                    case EnumTipoDoc.DocumentoFamiliareFiglio_MaggiorazioniFamiliari4:
                        ld = db.FIGLI.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                        break;
                    case EnumTipoDoc.LetteraTrasferimento_Trasferimento5:
                        break;
                    case EnumTipoDoc.CartaIdentita_Viaggi1:
                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                ld = db.CONIUGE.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                break;
                            case EnumParentela.Figlio:
                                ld = db.FIGLI.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                break;
                            case EnumParentela.Richiedente:
                                //ld = db.PASSAPORTI.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.CartaIdentita_Viaggi1).ToList();
                                var p = db.PASSAPORTI.Find(id);
                                var ldoc =
                                    p.DOCUMENTI.Where(
                                        a => a.IDTIPODOCUMENTO == (decimal)tipodoc);
                                if (ldoc?.Any() ?? false)
                                {
                                    ld = ldoc.ToList();
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("tipodoc");
                }


                if (ld?.Any() ?? false)
                {
                    ldm.AddRange(from d in ld
                                 let f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO)
                                 select new DocumentiModel()
                                 {
                                     idDocumenti = d.IDDOCUMENTO,
                                     nomeDocumento = d.NOMEDOCUMENTO,
                                     estensione = d.ESTENSIONE,
                                     tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                                     dataInserimento = d.DATAINSERIMENTO,
                                     file = f
                                 });
                }


            }

            return ldm;
        }

        //public IList<DocumentiModel> GetDocumentiByIdFiglio(decimal idFiglio)
        //{
        //    List<DocumentiModel> ldm = new List<DocumentiModel>();

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        var ld = db.FIGLI.Find(idFiglio).DOCUMENTI.ToList();
        //        if (ld?.Any() ?? false)
        //        {
        //            ldm.AddRange(from d in ld
        //                         let f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO)
        //                         select new DocumentiModel()
        //                         {
        //                             idDocumenti = d.IDDOCUMENTO,
        //                             nomeDocumento = d.NOMEDOCUMENTO,
        //                             estensione = d.ESTENSIONE,
        //                             dataInserimento = d.DATAINSERIMENTO,
        //                             tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
        //                             file = f
        //                         });
        //        }

        //    }

        //    return ldm;
        //}

        public DocumentiModel GetDocumentoByIdPassaporto(decimal idPassaporto)
        {
            DocumentiModel dm = new DocumentiModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var p = db.PASSAPORTI.Find(idPassaporto);
                if (p != null && p.IDPASSAPORTO > 0)
                {
                    var ld = p.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.CartaIdentita_Viaggi1).ToList();
                    if (ld?.Any() ?? false)
                    {
                        var d = ld.First();
                        var f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO);
                        dm = new DocumentiModel()
                        {
                            idDocumenti = d.IDDOCUMENTO,
                            tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                            nomeDocumento = d.NOMEDOCUMENTO,
                            estensione = d.ESTENSIONE,
                            file = f,
                            dataInserimento = d.DATAINSERIMENTO
                        };
                    }
                }
            }

            return dm;
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



        public void AddDocumentoFromFiglio(ref DocumentiModel dm, decimal idFiglio, ModelDBISE db)
        {
            var f = db.FIGLI.Find(idFiglio);
            if (f.IDFIGLI != null && f.IDFIGLI > 0)
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                dm.file.InputStream.CopyTo(ms);

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();

                f.DOCUMENTI.Add(d);

                int i = db.SaveChanges();

                if (i > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                }
            }
        }



        public void AddDocumentoFromConiuge(ref DocumentiModel dm, decimal idConiuge, ModelDBISE db)
        {
            var c = db.CONIUGE.Find(idConiuge);

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

        public void AddDocumentoPassaportoFromRichiedente(ref DocumentiModel dm, decimal idTrasferimento, ModelDBISE db)
        {
            var t = db.TRASFERIMENTO.Find(idTrasferimento);
            if (t != null && t.IDTRASFERIMENTO > 0)
            {
                var p = t.PASSAPORTI;
                if (p != null && p.IDPASSAPORTO > 0)
                {
                    MemoryStream ms = new MemoryStream();
                    DOCUMENTI d = new DOCUMENTI();
                    dm.file.InputStream.CopyTo(ms);

                    d.NOMEDOCUMENTO = dm.nomeDocumento;
                    d.ESTENSIONE = dm.estensione;
                    d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
                    d.DATAINSERIMENTO = dm.dataInserimento;
                    d.FILEDOCUMENTO = ms.ToArray();

                    p.DOCUMENTI.Add(d);

                    if (db.SaveChanges() > 0)
                    {
                        dm.idDocumenti = d.IDDOCUMENTO;
                    }
                }
            }
        }


        public void AddDocumentoTitoloViaggioFromRichiedente(ref DocumentiModel dm, decimal idTrasferimento, ModelDBISE db)
        {
            var t = db.TRASFERIMENTO.Find(idTrasferimento);
            if (t != null && t.IDTRASFERIMENTO > 0)
            {
                var tv = t.TITOLIVIAGGIO;
                if (tv != null && tv.IDTITOLOVIAGGIO > 0)
                {
                    MemoryStream ms = new MemoryStream();
                    DOCUMENTI d = new DOCUMENTI();
                    dm.file.InputStream.CopyTo(ms);

                    d.NOMEDOCUMENTO = dm.nomeDocumento;
                    d.ESTENSIONE = dm.estensione;
                    d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
                    d.DATAINSERIMENTO = dm.dataInserimento;
                    d.FILEDOCUMENTO = ms.ToArray();

                    tv.DOCUMENTI.Add(d);

                    if (db.SaveChanges() > 0)
                    {
                        dm.idDocumenti = d.IDDOCUMENTO;
                    }
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