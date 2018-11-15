using NewISE.EF;
using NewISE.Models.Enumeratori;
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


        public void AssociaDocumentoConiuge(decimal idConiuge, decimal idDocumento, ModelDBISE db)
        {
            try
            {
                var c = db.CONIUGE.Find(idConiuge);
                var item = db.Entry<CONIUGE>(c);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                c.DOCUMENTI.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il documento per il coniuge. {0}", c.COGNOME + " " + c.NOME));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaDocumentoFiglio(decimal idFiglio, decimal idDocumento, ModelDBISE db)
        {
            try
            {
                var f = db.FIGLI.Find(idFiglio);
                var item = db.Entry<FIGLI>(f);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                f.DOCUMENTI.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il documento per il figlio. {0}", f.COGNOME + " " + f.NOME));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DocumentiModel GetFormularioTitoliViaggio(decimal idTitoloViaggio)
        {
            DocumentiModel dm = new DocumentiModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                //var tv = db.TITOLIVIAGGIO.Find(idTitoloViaggio);
                //var ld = tv.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Titoli_Viaggio)
                //        .OrderByDescending(a => a.IDDOCUMENTO);
                //if (ld?.Any() ?? false)
                //{
                //    var d = ld.First();

                //    dm = this.GetDocumento(d.IDDOCUMENTO, db);
                //}
            }

            return dm;
        }

        public IList<DocumentiModel> GetFormulariAttivazioneMagFam(decimal idAttivazioneMagFam)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);


                if (amf?.IDATTIVAZIONEMAGFAM > 0)
                {

                    var ld =
                        amf.DOCUMENTI.Where(
                            a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari)
                            .OrderByDescending(a => a.DATAINSERIMENTO);
                    if (ld?.Any() ?? false)
                    {
                        ldm.AddRange(ld.Select(d => this.GetDocumento(d.IDDOCUMENTO, db)));
                    }


                }


            }

            return ldm;

        }

        public IList<DocumentiModel> GetFormulariAttivazioneProvvScol(decimal idTrasfProvScolastiche)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var amf = db.ATTIVAZIONIPROVSCOLASTICHE.Find(idTrasfProvScolastiche);


                if (amf?.IDTRASFPROVSCOLASTICHE > 0)
                {

                    var ld =
                        amf.DOCUMENTI.Where(
                            a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Provvidenze_Scolastiche)
                            .OrderByDescending(a => a.DATAINSERIMENTO);
                    if (ld?.Any() ?? false)
                    {
                        ldm.AddRange(ld.Select(d => this.GetDocumento(d.IDDOCUMENTO, db)));
                    }


                }


            }

            return ldm;

        }


        public IList<DocumentiModel> GetDocumentiIdentitaRichiedentePassaporto(decimal idPassaportoRichiedente)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var rp = db.PASSAPORTORICHIEDENTE.Find(idPassaportoRichiedente);

                var ld = rp.DOCUMENTI.Where(
                    a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)
                    .OrderByDescending(a => a.DATAINSERIMENTO);

                if (ld?.Any() ?? false)
                {
                    ldm.AddRange(from d in ld
                                 let file = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO)
                                 select new DocumentiModel()
                                 {
                                     idDocumenti = d.IDDOCUMENTO,
                                     nomeDocumento = d.NOMEDOCUMENTO,
                                     estensione = d.ESTENSIONE,
                                     tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                                     dataInserimento = d.DATAINSERIMENTO,
                                     file = file
                                 });
                }
            }

            return ldm;
        }


        public IList<DocumentiModel> GetDocumentiIdentitaFiglioPassaporto(decimal idFiglioPassaporto)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var fp = db.FIGLIPASSAPORTO.Find(idFiglioPassaporto);
                var f = fp.FIGLI
                        .First(a=>a.ATTIVAZIONIMAGFAM
                                .Where(b => b.ANNULLATO == false && 
                                            b.RICHIESTAATTIVAZIONE == true)
                                .Any());
                var ld =
                    f.DOCUMENTI.Where(
                        a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)
                        .OrderByDescending(a => a.DATAINSERIMENTO);

                if (ld?.Any() ?? false)
                {
                    ldm.AddRange(from d in ld
                                 let file = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO)
                                 select new DocumentiModel()
                                 {
                                     idDocumenti = d.IDDOCUMENTO,
                                     nomeDocumento = d.NOMEDOCUMENTO,
                                     estensione = d.ESTENSIONE,
                                     tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                                     dataInserimento = d.DATAINSERIMENTO,
                                     file = file
                                 });
                }
            }

            return ldm;
        }


        public IList<DocumentiModel> GetDocumentiIdentitaConiugePassaporto(decimal idConiugePassaporto)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var cp = db.CONIUGEPASSAPORTO.Find(idConiugePassaporto);
                var c = cp.CONIUGE
                             .First(a => a.ATTIVAZIONIMAGFAM
                                .Where(b => b.ANNULLATO == false &&
                                            b.RICHIESTAATTIVAZIONE == true)
                                .Any());
                var ld =
                    c.DOCUMENTI.Where(
                        a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)
                        .OrderByDescending(a => a.DATAINSERIMENTO);

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


        public IList<DocumentiModel> GetDocumentiIdentitaConiuge(decimal idConiuge, decimal idAttivazioneMagFam)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {

                var c = db.CONIUGE.Find(idConiuge);

                var ld =
                    c.DOCUMENTI.Where(
                        a =>
                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                            a.ATTIVAZIONIMAGFAM.Any(b => b.IDATTIVAZIONEMAGFAM == idAttivazioneMagFam))
                        .OrderByDescending(a => a.IDDOCUMENTO);


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

        public IList<DocumentiModel> GetDocumentiIdentitaFigli(decimal idFiglio, decimal idAttivitaMagFam)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var fi = db.FIGLI.Find(idFiglio);
                var ld =
                    fi.DOCUMENTI.Where(
                        a =>
                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                            a.ATTIVAZIONIMAGFAM.Any(b => b.IDATTIVAZIONEMAGFAM == idAttivitaMagFam))
                        .OrderByDescending(a => a.IDTIPODOCUMENTO);

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


        public IList<DocumentiModel> GetDocumentiByIdFamiliarePassaporto(decimal idFamiliarePassaporto, EnumTipoDoc tipodoc, EnumParentela parentela = EnumParentela.Richiedente)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                List<DOCUMENTI> ld = new List<DOCUMENTI>();


                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        var cp = db.CONIUGEPASSAPORTO.Find(idFamiliarePassaporto);
                        var c = cp.CONIUGE.First();
                        ld = c.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == Convert.ToDecimal(tipodoc)).ToList();
                        break;
                    case EnumParentela.Figlio:
                        var fp = db.FIGLIPASSAPORTO.Find(idFamiliarePassaporto);
                        var f = fp.FIGLI.First();
                        ld = f.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == Convert.ToDecimal(tipodoc)).ToList();
                        break;
                    case EnumParentela.Richiedente:
                        var rp = db.PASSAPORTORICHIEDENTE.Find(idFamiliarePassaporto);
                        ld = rp.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == Convert.ToDecimal(tipodoc)).ToList();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("parentela");
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipodoc"></param>
        /// <param name="parentela"></param>
        /// <returns></returns>
        public IList<DocumentiModel> GetDocumentiByIdTable(decimal id, EnumTipoDoc tipodoc, EnumParentela parentela = EnumParentela.Richiedente, decimal idAttivazioneMagFam = 0)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                List<DOCUMENTI> ld = new List<DOCUMENTI>();
                //ATTIVAZIONIMAGFAM amf = new ATTIVAZIONIMAGFAM();

                switch (tipodoc)
                {
                    case EnumTipoDoc.Carta_Imbarco:
                    case EnumTipoDoc.Titolo_Viaggio:
                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                var c = db.CONIUGE.Find(id);
                                if (c != null && c.IDCONIUGE > 0)
                                {
                                    ld = c.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                }
                                break;
                            case EnumParentela.Figlio:
                                var f = db.FIGLI.Find(id);
                                if (f != null && f.IDFIGLI > 0)
                                {
                                    ld = f.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                }
                                break;
                            case EnumParentela.Richiedente:
                                var tv = db.TITOLIVIAGGIO.Find(id);
                                if (tv != null && tv.IDTITOLOVIAGGIO > 0)
                                {
                                    //ld = tv.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }
                        break;
                    case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                        break;
                    case EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore:
                        break;
                    case EnumTipoDoc.Clausole_Contratto_Alloggio:
                        break;
                    case EnumTipoDoc.Copia_Contratto_Locazione:
                        break;
                    case EnumTipoDoc.Contributo_Fisso_Omnicomprensivo:
                        break;
                    case EnumTipoDoc.Attestazione_Trasloco:
                        break;
                    case EnumTipoDoc.Documento_Identita:
                        if (idAttivazioneMagFam > 0)
                        {
                            switch (parentela)
                            {
                                case EnumParentela.Coniuge:
                                    var c = db.CONIUGE.Find(id);
                                    ld =
                                        c.DOCUMENTI.Where(
                                            a => a.IDTIPODOCUMENTO == (decimal)tipodoc &&
                                                 a.ATTIVAZIONIMAGFAM.Any(
                                                     b =>
                                                         b.ANNULLATO == false &&
                                                         b.IDATTIVAZIONEMAGFAM == idAttivazioneMagFam))
                                            .OrderByDescending(a => a.IDDOCUMENTO)
                                            .ToList();


                                    break;
                                case EnumParentela.Figlio:
                                    var f = db.FIGLI.Find(id);
                                    ld =
                                        f.DOCUMENTI.Where(
                                            a =>
                                                a.IDTIPODOCUMENTO == (decimal)tipodoc &&
                                                a.ATTIVAZIONIMAGFAM.Any(
                                                    b =>
                                                        b.ANNULLATO == false &&
                                                        b.IDATTIVAZIONEMAGFAM == idAttivazioneMagFam))
                                            .OrderByDescending(a => a.IDDOCUMENTO)
                                            .ToList();

                                    break;
                                case EnumParentela.Richiedente:
                                    ld = db.PASSAPORTORICHIEDENTE.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("parentela");
                            }
                        }
                        else
                        {
                            switch (parentela)
                            {
                                case EnumParentela.Coniuge:
                                    ld = db.CONIUGE.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                    break;
                                case EnumParentela.Figlio:
                                    ld = db.FIGLI.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();

                                    break;
                                case EnumParentela.Richiedente:
                                    ld = db.PASSAPORTORICHIEDENTE.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("parentela");
                            }
                        }

                        break;
                    case EnumTipoDoc.Lettera_Trasferimento:
                        break;
                    case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:

                        ld = db.ATTIVAZIONIMAGFAM.Find(id)
                            .DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc)
                            .OrderByDescending(a => a.DATAINSERIMENTO).ToList();

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

        public List<TIPODOCUMENTI> GetElencoTipoDocumentiMAB(ModelDBISE db)
        {

                List<TIPODOCUMENTI> ltd = db.TIPODOCUMENTI.Where(a =>
                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione ||
                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione ||
                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore ||
                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione ||
                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Clausole_Contratto_Alloggio ||
                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Contratto_Locazione ||
                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione)
                                .OrderBy(a=>a.IDTIPODOCUMENTO)
                                .ToList();
            return ltd;
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

        //public DocumentiModel GetDatiDocumentoByIdTrasferimento(decimal idTrasferimento)
        //{
        //    DocumentiModel dm = new DocumentiModel();
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        var ld = db.TRASFERIMENTO.Find(idTrasferimento).DOCUMENTI;

        //        if (ld != null && ld.Count > 0)
        //        {
        //            var d = ld.First();
        //            //HttpPostedFileBase f;

        //            //f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO, d.NOMEDOCUMENTO + d.ESTENSIONE, "application/pdf");

        //            dm = new DocumentiModel()
        //            {
        //                idDocumenti = d.IDDOCUMENTO,
        //                nomeDocumento = d.NOMEDOCUMENTO,
        //                estensione = d.ESTENSIONE,
        //                tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
        //                dataInserimento = d.DATAINSERIMENTO,
        //                //file = f
        //            };
        //        }
        //    }

        //    return dm;
        //}

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
            d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

            db.DOCUMENTI.Add(d);

            if (db.SaveChanges() > 0)
            {
                dm.idDocumenti = d.IDDOCUMENTO;
                TRASFERIMENTO t = new TRASFERIMENTO();

                switch ((EnumTipoDoc)d.IDTIPODOCUMENTO)
                {
                    case EnumTipoDoc.Carta_Imbarco:
                    case EnumTipoDoc.Titolo_Viaggio:
                    case EnumTipoDoc.Formulario_Titoli_Viaggio:
                        //t = d.TITOLIVIAGGIO.OrderByDescending(a => a.IDTITOLOVIAGGIO).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                    case EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore:
                    case EnumTipoDoc.Clausole_Contratto_Alloggio:
                    case EnumTipoDoc.Copia_Contratto_Locazione:
                        //t = d.MAGGIORAZIONEABITAZIONE.OrderByDescending(a => a.IDMAB).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Contributo_Fisso_Omnicomprensivo:
                        //t = d.TRASPORTOEFFETTI.OrderByDescending(a => a.IDTRASPORTOEFFETTI).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Attestazione_Trasloco:
                        break;
                    case EnumTipoDoc.Documento_Identita:
                        //t = d.PASSAPORTI.OrderByDescending(a => a.IDPASSAPORTI).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Lettera_Trasferimento:
                        t = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO).First();
                        break;
                    case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:
                        t = d.ATTIVAZIONIMAGFAM.First().MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
                        break;
                    default:
                        t = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO).First();
                        break;

                }


                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
            }

        }


        public void SetFormularioTitoliViaggio(ref DocumentiModel dm, decimal idTitoloViaggio, ModelDBISE db)
        {
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();

            dm.file.InputStream.CopyTo(ms);

            var tv = db.TITOLIVIAGGIO.Find(idTitoloViaggio);
            //var ld = tv.DOCUMENTI;

            //if (ld?.Any() ?? false)
            //{
            //    d = ld.First();
            //    d.NOMEDOCUMENTO = dm.nomeDocumento;
            //    d.ESTENSIONE = dm.estensione;
            //    d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.Formulario_Titoli_Viaggio;
            //    d.DATAINSERIMENTO = dm.dataInserimento;
            //    d.FILEDOCUMENTO = ms.ToArray();

            //    if (db.SaveChanges() > 0)
            //    {
            //        dm.idDocumenti = d.IDDOCUMENTO;
            //        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Inserimento di una nuovo documento (formulario titoli viaggio).", "Documenti", db, tv.IDTRASFERIMENTO, dm.idDocumenti);
            //    }
            //}
            //else
            //{
            //    d.NOMEDOCUMENTO = dm.nomeDocumento;
            //    d.ESTENSIONE = dm.estensione;
            //    d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.Formulario_Titoli_Viaggio;
            //    d.DATAINSERIMENTO = dm.dataInserimento;
            //    d.FILEDOCUMENTO = ms.ToArray();
            //    ld.Add(d);

            //    if (db.SaveChanges() > 0)
            //    {
            //        dm.idDocumenti = d.IDDOCUMENTO;
            //        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (formulario titoli viaggio).", "Documenti", db, tv.IDTRASFERIMENTO, dm.idDocumenti);
            //    }
            //}


        }

        public void SetFormularioMaggiorazioniFamiliari(ref DocumentiModel dm, decimal idAttivazioneMagFam, ModelDBISE db)
        {
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();

            dm.file.InputStream.CopyTo(ms);
            var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

            if (amf?.IDATTIVAZIONEMAGFAM > 0)
            {
                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.FK_IDDOCUMENTO = null;
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;
                amf.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    var mf = amf.MAGGIORAZIONIFAMILIARI;

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (formulario maggiorazioni familiari).", "Documenti", db, mf.TRASFERIMENTO.IDTRASFERIMENTO, dm.idDocumenti);
                }
                else
                {
                    throw new Exception("Errore nella fase di inserimento del formulario maggiorazioni familiari.");
                }
            }
            else
            {
                throw new Exception("Errore nella fase di inserimento del formulario maggiorazioni familiari.");
            }




        }


        public void SetFormularioProvvidenzeScolastiche(ref DocumentiModel dm, decimal idProvScolastiche, ModelDBISE db)
        {
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();

            dm.file.InputStream.CopyTo(ms);
            var amf = db.ATTIVAZIONIPROVSCOLASTICHE.Find(idProvScolastiche);

            if (amf?.IDPROVSCOLASTICHE > 0)
            {
                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.Formulario_Provvidenze_Scolastiche;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.FK_IDDOCUMENTO = null;
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;
                amf.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    var mf = amf.PROVVIDENZESCOLASTICHE;

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (formulario provvidenze scolastiche).", "Documenti", db, mf.TRASFERIMENTO.IDTRASFERIMENTO, dm.idDocumenti);
                }
                else
                {
                    throw new Exception("Errore nella fase di inserimento del formulario provvidenze scolastiche.");
                }
            }
            else
            {
                throw new Exception("Errore nella fase di inserimento del formulario provvidenze scolastiche.");
            }




        }


        public void SetLetteraTrasferimento(ref DocumentiModel dm, decimal idTrasferimento, ModelDBISE db)
        {
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();

            dm.file.InputStream.CopyTo(ms);

            var ld = db.TRASFERIMENTO.Find(idTrasferimento).DOCUMENTI;

            if (ld?.Any() ?? false)
            {
                d = ld.First();
                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.Lettera_Trasferimento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Inserimento di una nuovo documento (lettera di trasferimento).", "Documenti", db, idTrasferimento, dm.idDocumenti);
                }
            }
            else
            {
                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.Lettera_Trasferimento;
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
            var t = f.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

            if (f.IDFIGLI > 0)
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                dm.file.InputStream.CopyTo(ms);

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.FK_IDDOCUMENTO = dm.fk_iddocumento;
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

                f.DOCUMENTI.Add(d);

                int i = db.SaveChanges();

                if (i > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
                }
            }
        }



        public void AddDocumentoFromConiuge(ref DocumentiModel dm, decimal idConiuge, ModelDBISE db)
        {
            var c = db.CONIUGE.Find(idConiuge);
            var t = c.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
            var amf = c.ATTIVAZIONIMAGFAM.OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First(a => a.ANNULLATO == false);

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
                d.FK_IDDOCUMENTO = dm.fk_iddocumento;
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

                c.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                    {
                        dtamf.AssociaDocumentoAttivazione(amf.IDATTIVAZIONEMAGFAM, d.IDDOCUMENTO, db);
                    }

                }
            }


        }

        public void AddDocumentoPassaportoFromRichiedente(ref DocumentiModel dm, decimal idPassaportoRichiedente, ModelDBISE db)
        {
            var pr = db.PASSAPORTORICHIEDENTE.Find(idPassaportoRichiedente);

            if (pr?.IDPASSAPORTORICHIEDENTE > 0)
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                dm.file.InputStream.CopyTo(ms);

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.FK_IDDOCUMENTO = dm.fk_iddocumento;
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

                pr.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    var t = pr.PASSAPORTI.TRASFERIMENTO;
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
                }
            }


        }


        public void AddDocumentoTitoloViaggioFromRichiedente(ref DocumentiModel dm, decimal idTrasferimento, ModelDBISE db)
        {
            var t = db.TRASFERIMENTO.Find(idTrasferimento);
        }


        public void DeleteDocumentoPassaporto(decimal idDocumento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var d = db.DOCUMENTI.Find(idDocumento);
                var lpr = d.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false);

                if (lpr?.Any() ?? false)
                {
                    var pr = lpr.First();

                    var t = pr.PASSAPORTI.TRASFERIMENTO;

                    db.DOCUMENTI.Remove(d);

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception(string.Format("Non è stato possibile effettuare l'eliminazione del documento ({0}).", d.NOMEDOCUMENTO + d.ESTENSIONE));
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione di un documento (" + ((EnumTipoDoc)d.IDTIPODOCUMENTO).ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, d.IDDOCUMENTO);
                    }
                }
                else
                {
                    throw new Exception(string.Format("Non è stato possibile effettuare l'eliminazione del documento ({0}).", d.NOMEDOCUMENTO + d.ESTENSIONE));
                }



            }
        }

        public void DeleteDocumento(decimal idDocumento, EnumChiamante chiamante)
        {
            TRASFERIMENTO t = new TRASFERIMENTO();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var d = db.DOCUMENTI.Find(idDocumento);

                    switch ((EnumTipoDoc)d.IDTIPODOCUMENTO)
                    {
                        case EnumTipoDoc.Carta_Imbarco:
                        case EnumTipoDoc.Titolo_Viaggio:
                        case EnumTipoDoc.Formulario_Titoli_Viaggio:
                            //t = d.TITOLIVIAGGIO.OrderByDescending(a => a.IDTITOLOVIAGGIO).First().TRASFERIMENTO;
                            break;
                        case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                        case EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore:
                        case EnumTipoDoc.Clausole_Contratto_Alloggio:
                        case EnumTipoDoc.Copia_Contratto_Locazione:
                            //t = d.MAGGIORAZIONEABITAZIONE.OrderByDescending(a => a.IDMAB).First().TRASFERIMENTO;
                            break;
                        case EnumTipoDoc.Contributo_Fisso_Omnicomprensivo:
                            //t = d.TRASPORTOEFFETTI.OrderByDescending(a => a.IDTRASPORTOEFFETTI).First().TRASFERIMENTO;
                            break;
                        case EnumTipoDoc.Attestazione_Trasloco:
                            break;
                        case EnumTipoDoc.Lettera_Trasferimento:
                            t = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO).First();
                            break;
                        case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:
                            t = d.ATTIVAZIONIMAGFAM.First().MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
                            break;
                        case EnumTipoDoc.Documento_Identita:

                            switch (chiamante)
                            {
                                case EnumChiamante.Maggiorazioni_Familiari:
                                case EnumChiamante.Variazione_Maggiorazioni_Familiari:
                                    var lc = d.CONIUGE;
                                    if (lc?.Any() ?? false)
                                    {
                                        t = lc.First().MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
                                    }
                                    else
                                    {
                                        var lf = d.FIGLI;
                                        if (lf?.Any() ?? false)
                                        {
                                            t = lf.First().MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
                                        }
                                    }
                                    break;
                                case EnumChiamante.Titoli_Viaggio:
                                    break;
                                case EnumChiamante.Trasporto_Effetti:
                                    break;
                                case EnumChiamante.Trasferimento:
                                    break;
                                case EnumChiamante.Passaporti:
                                    //t =
                                    //    d.PASSAPORTI.OrderByDescending(a => a.IDPASSAPORTI)
                                    //        .First()
                                    //        .TRASFERIMENTO;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("chiamante");
                            }


                            break;
                        default:
                            t = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO).First();
                            break;

                    }


                    if (d != null && d.IDDOCUMENTO > 0)
                    {
                        db.DOCUMENTI.Remove(d);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Non è stato possibile effettuare l'eliminazione del documento ({0}).", d.NOMEDOCUMENTO + d.ESTENSIONE));
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione di un documento (" + ((EnumTipoDoc)d.IDTIPODOCUMENTO).ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, d.IDDOCUMENTO);
                        }
                    }
                }



            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void DeleteDocumento(decimal idDocumento, EnumChiamante chiamante, ModelDBISE db)
        {
            TRASFERIMENTO t = new TRASFERIMENTO();

            try
            {

                var d = db.DOCUMENTI.Find(idDocumento);

                switch ((EnumTipoDoc)d.IDTIPODOCUMENTO)
                {
                    case EnumTipoDoc.Carta_Imbarco:
                    case EnumTipoDoc.Titolo_Viaggio:
                    case EnumTipoDoc.Formulario_Titoli_Viaggio:
                        //t = d.TITOLIVIAGGIO.OrderByDescending(a => a.IDTITOLOVIAGGIO).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                    case EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore:
                    case EnumTipoDoc.Clausole_Contratto_Alloggio:
                    case EnumTipoDoc.Copia_Contratto_Locazione:
                    case EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione:
                    case EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione:
                    case EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione:
                        t = d.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).First().MAB.INDENNITA.TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Contributo_Fisso_Omnicomprensivo:
                        //t = d.TRASPORTOEFFETTI.OrderByDescending(a => a.IDTRASPORTOEFFETTI).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Attestazione_Trasloco:
                        break;
                    case EnumTipoDoc.Lettera_Trasferimento:
                        t = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO).First();
                        break;
                    case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:
                        t = d.ATTIVAZIONIMAGFAM.First().MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Formulario_Provvidenze_Scolastiche:
                        t = d.ATTIVAZIONIPROVSCOLASTICHE.First().PROVVIDENZESCOLASTICHE.TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Documento_Identita:
                        switch (chiamante)
                        {
                            case EnumChiamante.Maggiorazioni_Familiari:
                                var lc = d.CONIUGE;
                                if (lc?.Any() ?? false)
                                {
                                    t = lc.First().MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
                                }
                                else
                                {
                                    var lf = d.FIGLI;
                                    if (lf?.Any() ?? false)
                                    {
                                        t = lf.First().MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
                                    }
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("chiamante");
                        }


                        break;
                    default:
                        t = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO).First();
                        break;
                }


                if (d != null && d.IDDOCUMENTO > 0)
                {
                    db.DOCUMENTI.Remove(d);

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception(string.Format("Non è stato possibile effettuare l'eliminazione del documento ({0}).", d.NOMEDOCUMENTO + d.ESTENSIONE));
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione di un documento (" + (GetDescrizioneTipoDocumento((EnumTipoDoc)d.IDTIPODOCUMENTO,db)).ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, d.IDDOCUMENTO);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public IList<VariazioneDocumentiModel> GetFormulariMaggiorazioniFamiliariVariazione(decimal idMaggiorazioniFamiliari)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                var lamf =
                    mf.ATTIVAZIONIMAGFAM.Where(
                        a => (a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true) || a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONEMAGFAM);

                var t = mf.TRASFERIMENTO;
                var statoTrasf = t.IDSTATOTRASFERIMENTO;

                var i = 1;
                var coloresfondo = "";
                var coloretesto = "";

                if (lamf?.Any() ?? false)
                {
                    foreach (var e in lamf)
                    {
                        var ld =
                            e.DOCUMENTI.Where(
                                a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                .OrderByDescending(a => a.DATAINSERIMENTO);

                        bool modificabile = false;
                        if (e.RICHIESTAATTIVAZIONE == false &&
                            e.ATTIVAZIONEMAGFAM == false &&
                            statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                            statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                        {
                            modificabile = true;
                            coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                            coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                        }
                        else
                        {
                            if (i % 2 == 0)
                            {
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoDispari;
                            }
                            else
                            {
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoPari;
                            }
                            coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                        }
                        foreach (var doc in ld)
                        {
                            var amf = new VariazioneDocumentiModel()
                            {
                                dataInserimento = doc.DATAINSERIMENTO,
                                estensione = doc.ESTENSIONE,
                                idDocumenti = doc.IDDOCUMENTO,
                                nomeDocumento = doc.NOMEDOCUMENTO,
                                Modificabile = modificabile,
                                IdAttivazione = e.IDATTIVAZIONEMAGFAM,
                                DataAggiornamento = e.DATAAGGIORNAMENTO,
                                ColoreSfondo = coloresfondo,
                                ColoreTesto = coloretesto,
                                progressivo = i
                            };

                            ldm.Add(amf);
                        }

                        i++;

                    }

                }
            }

            return ldm;

        }

        public IList<VariazioneDocumentiModel> GetFormulariMaggiorazioniAbitazioneVariazione(decimal idMab, ModelDBISE db)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
            {
                var mab = db.MAB.Find(idMab);

                var lamabm = dtvma.GetListaAttivazioniMABconDocumentiModel(idMab,db).OrderBy(a => a.idAttivazioneMAB).ToList(); 

                var statoTrasf = mab.INDENNITA.TRASFERIMENTO.IDSTATOTRASFERIMENTO;

                var i = 1;
                var coloresfondo = "";
                var coloretesto = "";

                if (lamabm?.Any() ?? false)
                {
                    foreach (var amabm in lamabm)
                    {
                        var amab = db.ATTIVAZIONEMAB.Find(amabm.idAttivazioneMAB);

                        var ld =
                            amab.DOCUMENTI.Where(
                                a => (
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione ||
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione ||
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore ||
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione ||
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Clausole_Contratto_Alloggio ||
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Contratto_Locazione ||
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione
                                    ) && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                .OrderByDescending(a => a.DATAINSERIMENTO);

                        bool modificabile = false;
                        if (amab.NOTIFICARICHIESTA == false &&
                            amab.ATTIVAZIONE == false &&
                            statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                            statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                        {
                            modificabile = true;
                            coloresfondo = Resources.VariazioneMABColori.AttivazioniMABAbilitate_Sfondo;
                            coloretesto = Resources.VariazioneMABColori.AttivazioniMABAbilitate_Testo;
                        }
                        else
                        {
                            if (amab.NOTIFICARICHIESTA == true &&
                                amab.ATTIVAZIONE == false &&
                                statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                                statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                            {
                                coloresfondo = Resources.VariazioneMABColori.AttivazioniMABAbilitate_Sfondo;
                                coloretesto = Resources.VariazioneMABColori.AttivazioniMABAbilitate_Testo;
                            }
                            else
                            {
                                if (i % 2 == 0)
                                {
                                    coloresfondo = Resources.VariazioneMABColori.AttivazioniMABDisabilitate_SfondoDispari;
                                }
                                else
                                {
                                    coloresfondo = Resources.VariazioneMABColori.AttivazioniMABDisabilitate_SfondoPari;
                                }
                                coloretesto = Resources.VariazioneMABColori.AttivazioniMABDisabilitate_Testo;
                            }
                        }
                        foreach (var doc in ld)
                        {
                            var vdm = new VariazioneDocumentiModel()
                            {
                                dataInserimento = doc.DATAINSERIMENTO,
                                estensione = doc.ESTENSIONE,
                                idDocumenti = doc.IDDOCUMENTO,
                                nomeDocumento = doc.NOMEDOCUMENTO,
                                Modificabile = modificabile,
                                IdAttivazione = amab.IDATTIVAZIONEMAB,
                                DataAggiornamento = amab.DATAAGGIORNAMENTO,
                                ColoreSfondo = coloresfondo,
                                ColoreTesto = coloretesto,
                                progressivo = i,
                                tipoDocumento = (EnumTipoDoc)doc.IDTIPODOCUMENTO,
                                DescrizioneTipoDocumento = GetDescrizioneTipoDocumento((EnumTipoDoc)doc.IDTIPODOCUMENTO, db)                                  
                            };
                            ldm.Add(vdm);
                        }

                        i++;

                    }
                }
            }

            return ldm;

        }

        public IList<VariazioneDocumentiModel> GetFormulariMaggiorazioniAbitazioneVariazioneByIdAttivazione(decimal idMab, decimal idAttivazione)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var mab = db.MAB.Find(idMab);

                    var lamabm = dtvma.GetListaAttivazioniMABconDocumentiModel(idMab,db).OrderBy(a=>a.idAttivazioneMAB).ToList();

                    var t = mab.INDENNITA.TRASFERIMENTO;
                    var statoTrasf = t.IDSTATOTRASFERIMENTO;

                    var i = 1;
                    var coloresfondo = "";
                    var coloretesto = "";

                    if (lamabm?.Any() ?? false)
                    {
                        foreach (var amabm in lamabm)
                        {
                            if (amabm.idAttivazioneMAB == idAttivazione)
                            {
                                var amab = db.ATTIVAZIONEMAB.Find(amabm.idAttivazioneMAB);

                                var ld =
                                    amab.DOCUMENTI.Where(
                                        a => (
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione ||
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione ||
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore ||
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione ||
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Clausole_Contratto_Alloggio ||
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Contratto_Locazione ||
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione
                                            ) && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                        .OrderByDescending(a => a.DATAINSERIMENTO);

                                bool modificabile = false;
                                if (amab.NOTIFICARICHIESTA == false &&
                                    amab.ATTIVAZIONE == false &&
                                    statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                                    statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                                {
                                    modificabile = true;
                                    coloresfondo = Resources.VariazioneMABColori.AttivazioniMABAbilitate_Sfondo;
                                    coloretesto = Resources.VariazioneMABColori.AttivazioniMABAbilitate_Testo;
                                }
                                else
                                {
                                    if (amab.NOTIFICARICHIESTA == true &&
                                        amab.ATTIVAZIONE == false &&
                                        statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                                        statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                                    {
                                        coloresfondo = Resources.VariazioneMABColori.AttivazioniMABAbilitate_Sfondo;
                                        coloretesto = Resources.VariazioneMABColori.AttivazioniMABAbilitate_Testo;
                                    }
                                    else
                                    {
                                        if (i % 2 == 0)
                                        {
                                            coloresfondo = Resources.VariazioneMABColori.AttivazioniMABDisabilitate_SfondoDispari;
                                        }
                                        else
                                        {
                                            coloresfondo = Resources.VariazioneMABColori.AttivazioniMABDisabilitate_SfondoPari;
                                        }
                                        coloretesto = Resources.VariazioneMABColori.AttivazioniMABDisabilitate_Testo;
                                    }
                                }
                                foreach (var doc in ld)
                                {
                                    var vdm = new VariazioneDocumentiModel()
                                    {
                                        dataInserimento = doc.DATAINSERIMENTO,
                                        estensione = doc.ESTENSIONE,
                                        idDocumenti = doc.IDDOCUMENTO,
                                        nomeDocumento = doc.NOMEDOCUMENTO,
                                        Modificabile = modificabile,
                                        IdAttivazione = amab.IDATTIVAZIONEMAB,
                                        DataAggiornamento = amab.DATAAGGIORNAMENTO,
                                        ColoreSfondo = coloresfondo,
                                        ColoreTesto = coloretesto,
                                        progressivo = i,
                                        tipoDocumento = (EnumTipoDoc)doc.IDTIPODOCUMENTO,
                                        DescrizioneTipoDocumento = GetDescrizioneTipoDocumento((EnumTipoDoc)doc.IDTIPODOCUMENTO, db)
                                    };
                                    ldm.Add(vdm);
                                }
                            }
                            i++;
                        }
                    }
                }
            }

            return ldm;

        }


        public IList<VariazioneDocumentiModel> GetFormulariProvvidenzeScolasticheVariazione(decimal idTrasfProvScolastiche)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche);

                //var lamf =
                //    mf.ATTIVAZIONIPROVSCOLASTICHE.Where(
                //        a => (a.NOTIFICARICHIESTA == true && a.ATTIVARICHIESTA == true) || a.ANNULLATO == false).OrderBy(a => a.IDPROVSCOLASTICHE);

                var lamf =
                    mf.ATTIVAZIONIPROVSCOLASTICHE.Where(
                        a => (a.NOTIFICARICHIESTA == true && a.ATTIVARICHIESTA == true) || a.ANNULLATO == false).OrderBy(a => a.IDPROVSCOLASTICHE);

                var t = mf.TRASFERIMENTO;
                var statoTrasf = t.IDSTATOTRASFERIMENTO;

                var i = 1;
                var coloresfondo = "";
                var coloretesto = "";

                if (lamf?.Any() ?? false)
                {
                    foreach (var e in lamf)
                    {
                        var ld =
                            e.DOCUMENTI.Where(
                                a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Provvidenze_Scolastiche)
                                .OrderByDescending(a => a.DATAINSERIMENTO);

                        bool modificabile = false;


                        //if (e.NOTIFICARICHIESTA == false &&
                        //    e.ATTIVARICHIESTA == false &&
                        //    statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                        //    statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                        //{
                        //    modificabile = true;
                        //    coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                        //    coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                        //}
                        //else
                        //{
                        //    if (i % 2 == 0)
                        //    {
                        //        coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoDispari;
                        //    }
                        //    else
                        //    {
                        //        coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoPari;
                        //    }
                        //    coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                        //}



                        //if (Condition1)
                        //{
                        //    // Condition1 is true.
                        //}
                        //else if (Condition2)
                        //{
                        //    // Condition1 is false and Condition2 is true.
                        //}
                        //else if (Condition3)
                        //{
                        //    if (Condition4)
                        //    {
                        //        // Condition1 and Condition2 are false. Condition3 and Condition4 are true.
                        //    }
                        //    else
                        //    {
                        //        // Condition1, Condition2, and Condition4 are false. Condition3 is true.
                        //    }
                        //}
                        //else
                        //{
                        //    // Condition1, Condition2, and Condition3 are false.
                        //}


                        // Condition1 is true.
                        if (e.NOTIFICARICHIESTA == false && e.ATTIVARICHIESTA == false &&
                            statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                            statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                        {
                            modificabile = true;
                            coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                            coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                        }
                        // Condition1 is false and Condition2 is true.
                        else if (e.NOTIFICARICHIESTA == true && e.ATTIVARICHIESTA == false &&
                            statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                            statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                        {
                            modificabile = false;
                            coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                            coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                        }

                        else if (e.NOTIFICARICHIESTA == true && e.ATTIVARICHIESTA == true &&
                            statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                            statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                        {
                            modificabile = false;
                            coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoDispari;
                            coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                        }
                        else
                        {
                            if (i % 2 == 0)
                            {
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoDispari;
                            }
                            else
                            {
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoPari;
                            }
                            coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                        }


                        foreach (var doc in ld)
                        {
                            var amf = new VariazioneDocumentiModel()
                            {
                                dataInserimento = doc.DATAINSERIMENTO,
                                estensione = doc.ESTENSIONE,
                                idDocumenti = doc.IDDOCUMENTO,
                                nomeDocumento = doc.NOMEDOCUMENTO,
                                Modificabile = modificabile,
                                IdAttivazione = e.IDPROVSCOLASTICHE,
                                DataAggiornamento = e.DATAAGGIORNAMENTO,
                                ColoreSfondo = coloresfondo,
                                ColoreTesto = coloretesto,
                                progressivo = i
                            };

                            ldm.Add(amf);
                        }

                        i++;

                    }

                }
            }

            return ldm;

        }
        public VariazioneDocumentiModel GetVariazioneDocumento(decimal idDocumento, ModelDBISE db)
        {
            VariazioneDocumentiModel dm = new VariazioneDocumentiModel();

            var d = db.DOCUMENTI.Find(idDocumento);

            if (d != null && d.IDDOCUMENTO > 0)
            {
                var f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO);

                dm = new VariazioneDocumentiModel()
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

        public string GetDescrizioneTipoDocumento(EnumTipoDoc TipoDocumento, ModelDBISE db)
        {
            TIPODOCUMENTI td = new TIPODOCUMENTI();
            string descrizioneTipoDoc;

            td = db.TIPODOCUMENTI.Find((decimal)TipoDocumento);

            if (td != null && td.IDTIPODOCUMENTO > 0)
            {
                descrizioneTipoDoc = td.DESCRIZIONE;
            }else
            {
                throw new Exception(string.Format("Nessuna corrispondenza trovata nella descrizione del tipo documento ({0}).",TipoDocumento.ToString()));
            }

            return descrizioneTipoDoc;
        }

        public IList<VariazioneDocumentiModel> GetFormulariMaggiorazioniFamiliariVariazioneByIdAttivazione(decimal idMaggiorazioniFamiliari, decimal idAttivazione)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);
                var t = mf.TRASFERIMENTO;
                var statoTrasf = t.IDSTATOTRASFERIMENTO;

                //var lamf =
                //    mf.ATTIVAZIONIMAGFAM.Where(
                //        a => ((a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true) || a.ANNULLATO == false) && a.IDATTIVAZIONEMAGFAM == idAttivazione)
                //        .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);
                var lamf =
                    mf.ATTIVAZIONIMAGFAM.Where(
                        a => ((a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true) || a.ANNULLATO == false))
                        .OrderBy(a => a.IDATTIVAZIONEMAGFAM);
                var i = 1;
                var coloretesto = "";
                var coloresfondo = "";

                if (lamf?.Any() ?? false)
                {

                    foreach (var e in lamf)
                    {
                        if (e.IDATTIVAZIONEMAGFAM == idAttivazione)
                        {
                            var ld =
                            e.DOCUMENTI.Where(
                                a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari)
                                .OrderByDescending(a => a.DATAINSERIMENTO);

                            bool modificabile = false;
                            if (e.RICHIESTAATTIVAZIONE == false &&
                                e.ATTIVAZIONEMAGFAM == false &&
                                statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                                statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                            {
                                modificabile = true;
                                coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                            }
                            else
                            {
                                coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoPari;
                            }

                            foreach (var doc in ld)
                            {
                                var amf = new VariazioneDocumentiModel()
                                {
                                    dataInserimento = doc.DATAINSERIMENTO,
                                    estensione = doc.ESTENSIONE,
                                    idDocumenti = doc.IDDOCUMENTO,
                                    nomeDocumento = doc.NOMEDOCUMENTO,
                                    Modificabile = modificabile,
                                    IdAttivazione = e.IDATTIVAZIONEMAGFAM,
                                    DataAggiornamento = e.DATAAGGIORNAMENTO,
                                    ColoreTesto = coloretesto,
                                    ColoreSfondo = coloresfondo,
                                    progressivo = i
                                };

                                ldm.Add(amf);
                            }
                        }

                        i++;
                    }

                }
            }
            return ldm;
        }

        public IList<VariazioneDocumentiModel> GetFormulariProvvidenzeScolasticheByIdAttivazione(decimal idTrasfProvScolastiche, decimal idAttivazione)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche);
                var t = mf.TRASFERIMENTO;
                var statoTrasf = t.IDSTATOTRASFERIMENTO;

                //var lamf =
                //    mf.ATTIVAZIONIMAGFAM.Where(
                //        a => ((a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true) || a.ANNULLATO == false) && a.IDATTIVAZIONEMAGFAM == idAttivazione)
                //        .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);
                var lamf =
                    mf.ATTIVAZIONIPROVSCOLASTICHE.Where(
                        a => ((a.NOTIFICARICHIESTA == true && a.ATTIVARICHIESTA == true) || a.ANNULLATO == false))
                        .OrderBy(a => a.IDPROVSCOLASTICHE);
                var i = 1;
                var coloretesto = "";
                var coloresfondo = "";

                if (lamf?.Any() ?? false)
                {

                    foreach (var e in lamf)
                    {
                        if (e.IDPROVSCOLASTICHE == idAttivazione)
                        {
                            var ld =
                            e.DOCUMENTI.Where(
                                a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Provvidenze_Scolastiche)
                                .OrderByDescending(a => a.DATAINSERIMENTO);

                            bool modificabile = false;

                            //if (e.NOTIFICARICHIESTA == false &&
                            //    e.ATTIVARICHIESTA == false &&
                            //    statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                            //    statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                            //{
                            //    modificabile = true;
                            //    coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                            //    coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                            //}
                            //else
                            //{
                            //    coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                            //    coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoPari;
                            //}

                            // Condition1 is true.
                            if (e.NOTIFICARICHIESTA == false && e.ATTIVARICHIESTA == false &&
                                statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                                statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                            {
                                modificabile = true;
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                                coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                            }
                            // Condition1 is false and Condition2 is true.
                            else if (e.NOTIFICARICHIESTA == true && e.ATTIVARICHIESTA == false &&
                                statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                                statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                            {
                                modificabile = false;
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                                coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                            }

                            else if (e.NOTIFICARICHIESTA == true && e.ATTIVARICHIESTA == true &&
                                statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                                statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                            {
                                modificabile = false;
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoDispari;
                                coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                            }
                            else
                            {
                                if (i % 2 == 0)
                                {
                                    coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoDispari;
                                }
                                else
                                {
                                    coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoPari;
                                }
                                coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                            }


                            foreach (var doc in ld)
                            {
                                var amf = new VariazioneDocumentiModel()
                                {
                                    dataInserimento = doc.DATAINSERIMENTO,
                                    estensione = doc.ESTENSIONE,
                                    idDocumenti = doc.IDDOCUMENTO,
                                    nomeDocumento = doc.NOMEDOCUMENTO,
                                    Modificabile = modificabile,
                                    IdAttivazione = e.IDPROVSCOLASTICHE,
                                    DataAggiornamento = e.DATAAGGIORNAMENTO,
                                    ColoreTesto = coloretesto,
                                    ColoreSfondo = coloresfondo,
                                    progressivo = i
                                };

                                ldm.Add(amf);
                            }
                        }

                        i++;
                    }

                }
            }
            return ldm;
        }


        public IList<VariazioneDocumentiModel> GetFormulariProvvidenzeScolasticheByIdAttivazioneVariazione(decimal idTrasfProvScolastiche, decimal idProvScolastiche)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche);
                var t = mf.TRASFERIMENTO;
                var statoTrasf = t.IDSTATOTRASFERIMENTO;

                ////var lamf =
                ////    mf.ATTIVAZIONIMAGFAM.Where(
                ////        a => ((a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true) || a.ANNULLATO == false) && a.IDATTIVAZIONEMAGFAM == idAttivazione)
                ////        .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);
                //var lamf =
                //    mf.ATTIVAZIONIPROVSCOLASTICHE.Where(
                //        a => ((a.NOTIFICARICHIESTA == true && a.ATTIVARICHIESTA == true) || a.ANNULLATO == false))
                //        .OrderBy(a => a.IDPROVSCOLASTICHE);

                var ps = db.ATTIVAZIONIPROVSCOLASTICHE.Find(idProvScolastiche);
                var i = 1;
                var coloretesto = "";
                var coloresfondo = "";

                var ld = ps.DOCUMENTI.Where(
                               a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Provvidenze_Scolastiche && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                               .OrderBy(a => a.DATAINSERIMENTO);

                bool modificabile = false;

                //var ld =
                //            e.DOCUMENTI.Where(
                //                a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari)
                //                .OrderByDescending(a => a.DATAINSERIMENTO);

                //bool modificabile = false;
                //if (e.RICHIESTAATTIVAZIONE == false &&
                //    e.ATTIVAZIONEMAGFAM == false &&
                //    statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                //    statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                //{
                //    modificabile = true;
                //    coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                //    coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                //}
                //else
                //{
                //    coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                //    coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoPari;
                //}



                if (ps.NOTIFICARICHIESTA == false && ps.ATTIVARICHIESTA == false &&
                    statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                    statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                {
                    modificabile = true;
                    coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                    coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                }
                else
                {
                    coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                    coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                }

                //else if (ps.NOTIFICARICHIESTA == true && ps.ATTIVARICHIESTA == false &&
                //    statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                //    statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                //{
                //    modificabile = true;
                //    coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                //    coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                //}

                //else if (ps.NOTIFICARICHIESTA == true && ps.ATTIVARICHIESTA == true &&
                //    statoTrasf != (decimal)EnumStatoTraferimento.Annullato &&
                //    statoTrasf != (decimal)EnumStatoTraferimento.Terminato)
                //{
                //    modificabile = false;
                //    coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoDispari;
                //    coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                //}
                //else
                //{
                //    if (i % 2 == 0)
                //    {
                //        coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoDispari;
                //    }
                //    else
                //    {
                //        coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                //        coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                //    }

                    
                //}


                foreach (var doc in ld)
                {
                    var amf = new VariazioneDocumentiModel()
                    {
                        dataInserimento = doc.DATAINSERIMENTO,
                        estensione = doc.ESTENSIONE,
                        idDocumenti = doc.IDDOCUMENTO,
                        nomeDocumento = doc.NOMEDOCUMENTO,
                        Modificabile = modificabile,
                        IdAttivazione = ps.IDPROVSCOLASTICHE,
                        DataAggiornamento = ps.DATAAGGIORNAMENTO,
                        ColoreTesto = coloretesto,
                        ColoreSfondo = coloresfondo,
                        progressivo = i
                    };

                    ldm.Add(amf);
                }


                i++;



            }
            return ldm;
        }
        public string GetDescrizioneTipoDocumentoByIdTipoDocumento(decimal idTipoDocumento)
        {
            string DescTipoDoc = "";

            using (ModelDBISE db = new ModelDBISE())
            {
                DescTipoDoc = db.TIPODOCUMENTI.Find(idTipoDocumento).DESCRIZIONE;
            }

            return DescTipoDoc;
        }


    }
}