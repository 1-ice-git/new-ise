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

        public DocumentiModel GetFormularioTitoliViaggio(decimal idTitoloViaggio)
        {
            DocumentiModel dm = new DocumentiModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                var tv = db.TITOLIVIAGGIO.Find(idTitoloViaggio);
                var ld =
                    tv.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Titoli_Viaggio)
                        .OrderByDescending(a => a.IDDOCUMENTO);
                if (ld?.Any() ?? false)
                {
                    var d = ld.First();

                    dm = this.GetDocumento(d.IDDOCUMENTO, db);
                }
            }

            return dm;
        }

        public IList<DocumentiModel> GetFormulariMaggiorazioniFamiliari(decimal idMaggiorazioniFamiliari)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);
                var ld =
                    mf.DOCUMENTI.Where(
                        a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari)
                        .OrderByDescending(a => a.IDDOCUMENTO);
                if (ld?.Any() ?? false)
                {
                    ldm.AddRange(ld.Select(d => this.GetDocumento(d.IDDOCUMENTO, db)));
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
        public IList<DocumentiModel> GetDocumentiByIdTable(decimal id, EnumTipoDoc tipodoc, EnumParentela parentela = EnumParentela.Richiedente)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                List<DOCUMENTI> ld = new List<DOCUMENTI>();

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
                                    ld = tv.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }
                        break;
                    case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                        break;
                    case EnumTipoDoc.Dichiarazione_Costo_Locazione:
                        break;
                    case EnumTipoDoc.Attestazione_Spese_Abitazione:
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

                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                ld = db.CONIUGE.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                break;
                            case EnumParentela.Figlio:
                                ld = db.FIGLI.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                break;
                            case EnumParentela.Richiedente:
                                ld = db.PASSAPORTI.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }
                        break;
                    case EnumTipoDoc.Lettera_Trasferimento:
                        break;
                    case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:
                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                ld = db.CONIUGE.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                break;
                            case EnumParentela.Figlio:
                                ld = db.FIGLI.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                break;
                            case EnumParentela.Richiedente:
                                ld = db.MAGGIORAZIONIFAMILIARI.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
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
                TRASFERIMENTO t = new TRASFERIMENTO();

                switch ((EnumTipoDoc)d.IDTIPODOCUMENTO)
                {
                    case EnumTipoDoc.Carta_Imbarco:
                    case EnumTipoDoc.Titolo_Viaggio:
                    case EnumTipoDoc.Formulario_Titoli_Viaggio:
                        t = d.TITOLIVIAGGIO.OrderByDescending(a => a.IDTITOLOVIAGGIO).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                    case EnumTipoDoc.Dichiarazione_Costo_Locazione:
                    case EnumTipoDoc.Attestazione_Spese_Abitazione:
                    case EnumTipoDoc.Clausole_Contratto_Alloggio:
                    case EnumTipoDoc.Copia_Contratto_Locazione:
                        t = d.MAGGIORAZIONEABITAZIONE.OrderByDescending(a => a.IDMAB).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Contributo_Fisso_Omnicomprensivo:
                        t = d.TRASPORTOEFFETTI.OrderByDescending(a => a.IDTRASPORTOEFFETTI).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Attestazione_Trasloco:
                        break;
                    case EnumTipoDoc.Documento_Identita:
                        t = d.PASSAPORTI.OrderByDescending(a => a.IDPASSAPORTI).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Lettera_Trasferimento:
                        t = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO).First();
                        break;
                    case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:
                        t =
                            d.MAGGIORAZIONIFAMILIARI.OrderByDescending(a => a.IDMAGGIORAZIONIFAMILIARI)
                                .First()
                                .TRASFERIMENTO;
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
            var ld = tv.DOCUMENTI;

            if (ld?.Any() ?? false)
            {
                d = ld.First();
                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.Formulario_Titoli_Viaggio;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Inserimento di una nuovo documento (formulario titoli viaggio).", "Documenti", db, tv.IDTRASFERIMENTO, dm.idDocumenti);
                }
            }
            else
            {
                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.Formulario_Titoli_Viaggio;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                ld.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (formulario titoli viaggio).", "Documenti", db, tv.IDTRASFERIMENTO, dm.idDocumenti);
                }
            }


        }

        public void SetFormularioMaggiorazioniFamiliari(ref DocumentiModel dm, decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();

            dm.file.InputStream.CopyTo(ms);

            var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

            d.NOMEDOCUMENTO = dm.nomeDocumento;
            d.ESTENSIONE = dm.estensione;
            d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari;
            d.DATAINSERIMENTO = dm.dataInserimento;
            d.FILEDOCUMENTO = ms.ToArray();
            mf.DOCUMENTI.Add(d);

            if (db.SaveChanges() > 0)
            {
                dm.idDocumenti = d.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (formulario maggiorazioni familiari).", "Documenti", db, mf.TRASFERIMENTO.IDTRASFERIMENTO, dm.idDocumenti);
            }
            else
            {
                throw new Exception("Errore nella fase di inserimento del formulario maggiorazioni familiari.");
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
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
                }
            }
        }



        public void AddDocumentoFromConiuge(ref DocumentiModel dm, decimal idConiuge, ModelDBISE db)
        {
            var c = db.CONIUGE.Find(idConiuge);
            var t = c.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

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
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
                }
            }


        }

        public void AddDocumentoPassaportoFromRichiedente(ref DocumentiModel dm, decimal idPassaporto, ModelDBISE db)
        {
            var p = db.PASSAPORTI.Find(idPassaporto);

            if (p != null && p.IDPASSAPORTI > 0)
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
                    var t = p.TRASFERIMENTO;
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
                }
            }


        }


        public void AddDocumentoTitoloViaggioFromRichiedente(ref DocumentiModel dm, decimal idTrasferimento, ModelDBISE db)
        {
            var t = db.TRASFERIMENTO.Find(idTrasferimento);
            if (t != null && t.IDTRASFERIMENTO > 0)
            {
                var tv = t.TITOLIVIAGGIO.OrderBy(a => a.IDTITOLOVIAGGIO).First();
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
                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
                    }
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
                            t = d.TITOLIVIAGGIO.OrderByDescending(a => a.IDTITOLOVIAGGIO).First().TRASFERIMENTO;
                            break;
                        case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                        case EnumTipoDoc.Dichiarazione_Costo_Locazione:
                        case EnumTipoDoc.Attestazione_Spese_Abitazione:
                        case EnumTipoDoc.Clausole_Contratto_Alloggio:
                        case EnumTipoDoc.Copia_Contratto_Locazione:
                            t = d.MAGGIORAZIONEABITAZIONE.OrderByDescending(a => a.IDMAB).First().TRASFERIMENTO;
                            break;
                        case EnumTipoDoc.Contributo_Fisso_Omnicomprensivo:
                            t = d.TRASPORTOEFFETTI.OrderByDescending(a => a.IDTRASPORTOEFFETTI).First().TRASFERIMENTO;
                            break;
                        case EnumTipoDoc.Attestazione_Trasloco:
                            break;
                        case EnumTipoDoc.Lettera_Trasferimento:
                            t = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO).First();
                            break;
                        case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:
                            t =
                                d.MAGGIORAZIONIFAMILIARI.OrderByDescending(a => a.IDMAGGIORAZIONIFAMILIARI)
                                    .First()
                                    .TRASFERIMENTO;
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
                                case EnumChiamante.Titoli_Viaggio:
                                    break;
                                case EnumChiamante.Trasporto_Effetti:
                                    break;
                                case EnumChiamante.Trasferimento:
                                    break;
                                case EnumChiamante.Passaporti:
                                    t =
                                        d.PASSAPORTI.OrderByDescending(a => a.IDPASSAPORTI)
                                            .First()
                                            .TRASFERIMENTO;
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
                        t = d.TITOLIVIAGGIO.OrderByDescending(a => a.IDTITOLOVIAGGIO).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                    case EnumTipoDoc.Dichiarazione_Costo_Locazione:
                    case EnumTipoDoc.Attestazione_Spese_Abitazione:
                    case EnumTipoDoc.Clausole_Contratto_Alloggio:
                    case EnumTipoDoc.Copia_Contratto_Locazione:
                        t = d.MAGGIORAZIONEABITAZIONE.OrderByDescending(a => a.IDMAB).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Contributo_Fisso_Omnicomprensivo:
                        t = d.TRASPORTOEFFETTI.OrderByDescending(a => a.IDTRASPORTOEFFETTI).First().TRASFERIMENTO;
                        break;
                    case EnumTipoDoc.Attestazione_Trasloco:
                        break;
                    case EnumTipoDoc.Lettera_Trasferimento:
                        t = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO).First();
                        break;
                    case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:
                        t =
                            d.MAGGIORAZIONIFAMILIARI.OrderByDescending(a => a.IDMAGGIORAZIONIFAMILIARI)
                                .First()
                                .TRASFERIMENTO;
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
                        Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione di un documento (" + ((EnumTipoDoc)d.IDTIPODOCUMENTO).ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, d.IDDOCUMENTO);
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