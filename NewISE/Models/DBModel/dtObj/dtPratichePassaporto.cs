using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using Newtonsoft.Json.Schema;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPratichePassaporto : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void SetPassaporto(decimal idPassaporto, ModelDBISE db)
        {
            PASSAPORTI p = new PASSAPORTI()
            {
                IDPASSAPORTO = idPassaporto,
                NOTIFICARICHIESTA = false,
                PRATICACONCLUSA = false
            };

            db.PASSAPORTI.Add(p);
            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nella fase d'inserimento dei dati per la gestione del passaporto.");
            }
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento dei dati di gestione del passaporto.", "Passaporti", db, idPassaporto, idPassaporto);
            }
        }

        public void SetPassaporto(decimal idPassaporto)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                PASSAPORTI p = new PASSAPORTI()
                {
                    IDPASSAPORTO = idPassaporto,
                    NOTIFICARICHIESTA = false,
                    PRATICACONCLUSA = false
                };

                db.PASSAPORTI.Add(p);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase d'inserimento dei dati per la gestione del passaporto.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento dei dati di gestione del passaporto.", "Passaporti", db, idPassaporto, idPassaporto);
                }
            }
        }

        public PassaportoModel GetPassaportoByID(decimal idPassaporto)
        {
            PassaportoModel pm = new PassaportoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var p = db.PASSAPORTI.Find(idPassaporto);

                pm = new PassaportoModel()
                {
                    idPassaporto = p.IDPASSAPORTO,
                    notificaRichiesta = p.NOTIFICARICHIESTA,
                    dataNotificaRichiesta = p.DATANOTIFICARICHIESTA,
                    praticaConclusa = p.PRATICACONCLUSA,
                    dataPraticaConclusa = p.DATAPRATICACONCLUSA,
                    //trasferimento = new TrasferimentoModel()
                    //{
                    //    idTrasferimento = p.TRASFERIMENTO.IDTRASFERIMENTO,
                    //    idTipoTrasferimento = p.TRASFERIMENTO.IDTIPOTRASFERIMENTO,
                    //    idUfficio = p.TRASFERIMENTO.IDUFFICIO,
                    //    idStatoTrasferimento = p.TRASFERIMENTO.IDSTATOTRASFERIMENTO,
                    //    idDipendente = p.TRASFERIMENTO.IDDIPENDENTE,
                    //    idTipoCoan = p.TRASFERIMENTO.IDTIPOCOAN,
                    //    dataPartenza = p.TRASFERIMENTO.DATAPARTENZA,
                    //    dataRientro = p.TRASFERIMENTO.DATARIENTRO,
                    //    coan = p.TRASFERIMENTO.COAN,
                    //    protocolloLettera = p.TRASFERIMENTO.PROTOCOLLOLETTERA,
                    //    dataLettera = p.TRASFERIMENTO.DATALETTERA,
                    //    notificaTrasferimento = p.TRASFERIMENTO.NOTIFICATRASFERIMENTO,
                    //    dataAggiornamento = p.TRASFERIMENTO.DATAAGGIORNAMENTO
                    //}
                };
            }

            return pm;
        }

        public IList<ElencoFamiliariModel> GetDipendentiRichiestaPassaporto(decimal idTrasferimento)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            TrasferimentoModel trm = new TrasferimentoModel();
            DipendentiModel dm = new DipendentiModel();
            MaggiorazioniFamiliariModel mf = new MaggiorazioniFamiliariModel();
            PassaportoModel pm = new PassaportoModel();

            using (dtDipendenti dtd = new dtDipendenti())
            {
                using (dtTrasferimento dttr = new dtTrasferimento())
                {
                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                    {
                        using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                        {
                            using (dtDocumenti dtdoc = new dtDocumenti())
                            {
                                trm = dttr.GetTrasferimentoById(idTrasferimento);

                                if (trm != null && trm.HasValue())
                                {
                                    dm = dtd.GetDipendenteByIDTrasf(trm.idTrasferimento);
                                    mf = dtmf.GetMaggiorazioniFamiliariByIDTrasf(trm.idTrasferimento);
                                    pm = dtpp.GetPassaportoByID(trm.idTrasferimento);
                                    ///la tabella passaporti è referenziata con la tabella trasferimento 1 a 1 pertanto l'id del trasferimento è anche l'id del passaporto.

                                    #region Passaporto richiedente

                                    if (dm != null && dm.idDipendente > 0)
                                    {
                                        ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                        {
                                            idMaggiorazioniFamiliari = 0,
                                            idFamiliare = dm.idDipendente,
                                            idPassaporto = pm.idPassaporto,
                                            Nominativo = dm.Nominativo,
                                            CodiceFiscale = string.Empty,
                                            dataInizio = trm.dataPartenza,
                                            dataFine = trm.dataRientro,
                                            parentela = EnumParentela.Richiedente,
                                            idAltriDati = 0,
                                            Documenti =
                                                dtdoc.GetDocumentiByIdTable(pm.idPassaporto,
                                                    EnumTipoDoc.CartaIdentita_Viaggi1, EnumParentela.Richiedente)
                                                    .ToList(),
                                        };

                                        lefm.Add(efm);
                                    }

                                    #endregion

                                    #region Passaporto familiari

                                    if (mf != null && mf.HasValue())
                                    {
                                        if (mf.attivazioneMaggiorazioni == true)
                                        {
                                            using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                                            {
                                                #region Coniuge

                                                using (dtConiuge dtc = new dtConiuge())
                                                {
                                                    var lcm =
                                                        dtc.GetListaConiugeByIdMagFam(mf.idMaggiorazioniFamiliari)
                                                            .ToList();
                                                    if (lcm?.Any() ?? false)
                                                    {
                                                        foreach (var cm in lcm)
                                                        {
                                                            ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                                            {
                                                                idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
                                                                idFamiliare = cm.idConiuge,
                                                                idPassaporto = pm.idPassaporto,
                                                                Nominativo = cm.nominativo,
                                                                CodiceFiscale = cm.codiceFiscale,
                                                                dataInizio = cm.dataInizio,
                                                                dataFine = cm.dataFine,
                                                                parentela = EnumParentela.Coniuge,
                                                                idAltriDati =
                                                                    dtadf.GetAlttriDatiFamiliariConiuge(cm.idConiuge)
                                                                        .idAltriDatiFam,
                                                                Documenti =
                                                                    dtdoc.GetDocumentiByIdTable(cm.idConiuge,
                                                                        EnumTipoDoc.CartaIdentita_Viaggi1,
                                                                        EnumParentela.Coniuge),
                                                            };

                                                            lefm.Add(efm);
                                                        }
                                                    }
                                                }

                                                #endregion

                                                #region Figli

                                                using (dtFigli dtf = new dtFigli())
                                                {
                                                    var lfm = dtf.GetListaFigli(mf.idMaggiorazioniFamiliari).ToList();
                                                    if (lfm?.Any() ?? false)
                                                    {
                                                        foreach (var fm in lfm)
                                                        {
                                                            ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                                            {
                                                                idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari,
                                                                idFamiliare = fm.idFigli,
                                                                idPassaporto = pm.idPassaporto,
                                                                Nominativo = fm.nominativo,
                                                                CodiceFiscale = fm.codiceFiscale,
                                                                dataInizio = fm.dataInizio,
                                                                dataFine = fm.dataFine,
                                                                parentela = EnumParentela.Figlio,
                                                                idAltriDati =
                                                                    dtadf.GetAlttriDatiFamiliariConiuge(fm.idFigli)
                                                                        .idAltriDatiFam,
                                                                Documenti =
                                                                    dtdoc.GetDocumentiByIdTable(fm.idFigli,
                                                                        EnumTipoDoc.CartaIdentita_Viaggi1,
                                                                        EnumParentela.Figlio),
                                                            };

                                                            lefm.Add(efm);
                                                        }
                                                    }
                                                }

                                                #endregion
                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }
                        }
                    }
                }
            }

            return lefm;
        }
    }
}