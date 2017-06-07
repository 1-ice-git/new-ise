using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Areas.Dipendenti.Models.DtObj
{
    public class dtDipTrasferimento : IDisposable
    {
        public static ValidationResult VerificaRequiredCoan(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var tr = context.ObjectInstance as dipTrasferimentoModel;

            if (tr != null)
            {
                if (tr.idTipoCoan == Convert.ToDecimal(EnumTipologiaCoan.Servizi_Promozionali))
                {
                    if (tr.coan != null && tr.coan != string.Empty && tr.coan.Length == 10)
                    {
                        vr = ValidationResult.Success;
                    }
                    else
                    {
                        vr = new ValidationResult("Il CO.AN. è richiesto e deve essere composto da 10 caratteri.");
                    }
                }
                else if (tr.idTipoCoan == Convert.ToDecimal(EnumTipologiaCoan.Servizi_Istituzionali))
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = new ValidationResult("Il CO.AN. è richiesto e deve essere composto da 10 caratteri.");
            }

            return vr;
        }

        public static ValidationResult VerificaRequiredDataLettera(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var tr = context.ObjectInstance as dipTrasferimentoModel;

            if (tr != null)
            {
                if ((tr.protocolloLettera != null && tr.protocolloLettera.Trim() != string.Empty) || tr.documento != null)
                {
                    if (tr.dataLettera.HasValue)
                    {
                        vr = ValidationResult.Success;
                    }
                    else
                    {
                        vr = new ValidationResult("La data della lettera è richiesta.");
                    }
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }

            return vr;
        }

        public static ValidationResult VerificaRequiredDocumentoLettera(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var tr = context.ObjectInstance as dipTrasferimentoModel;

            if (tr != null)
            {
                if (tr.dataLettera.HasValue || (tr.protocolloLettera != null && tr.protocolloLettera.Trim() != string.Empty))
                {
                    if (tr.documento != null)
                    {
                        vr = ValidationResult.Success;
                    }
                    else
                    {
                        vr = new ValidationResult("L'opzione allega lettera di trasferimento è richiesta.");
                    }
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = ValidationResult.Success;
            }

            return vr;
        }

        public static ValidationResult VerificaRequiredProtocolloLettera(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var tr = context.ObjectInstance as dipTrasferimentoModel;

            if (tr != null)
            {
                if (tr.dataLettera.HasValue || tr.documento != null)
                {
                    if (tr.protocolloLettera != null && tr.protocolloLettera.Trim() != string.Empty)
                    {
                        vr = ValidationResult.Success;
                    }
                    else
                    {
                        vr = new ValidationResult("Il Protocollo della lettera è richiesto.");
                    }
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = ValidationResult.Success;
            }

            return vr;
        }

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
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    tm = dtt.GetUltimoTrasferimentoByMatricola(matricola);

                    if (tm != null && tm.idTrasferimento > 0)
                    {
                        dit.statoTrasferimento = (EnumStatoTraferimento)tm.idStatoTrasferimento;
                        dit.UfficioDestinazione = tm.Ufficio;
                        dit.Decorrenza = tm.dataPartenza;
                        if (tm.dataRientro.HasValue)
                        {
                            dtDatiParametri = tm.dataRientro.Value;
                        }
                        else
                        {
                            dtDatiParametri = DateTime.Now.Date;
                        }

                        using (dtDipRuoloUfficio dtru = new dtDipRuoloUfficio())
                        {
                            RuoloUfficioModel rum = new RuoloUfficioModel();
                            rum = dtru.GetRuoloDipendente(tm.idTrasferimento, dtDatiParametri).RuoloUfficio;

                            dit.RuoloUfficio = rum;
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

        public void SetTrasferimento(TrasferimentoModel trm, EntitiesDBISE db)
        {
            TRASFERIMENTO tr;

            tr = new TRASFERIMENTO()
            {
                IDTIPOTRASFERIMENTO = trm.idTipoTrasferimento,
                IDUFFICIO = trm.idUfficio,
                IDSTATOTRASFERIMENTO = trm.idStatoTrasferimento,
                IDDIPENDENTE = trm.idDipendente,
                IDTIPOCOAN = trm.idTipoCoan,
                DATAPARTENZA = trm.dataPartenza,
                DATARIENTRO = trm.dataRientro,
                COAN = trm.coan,
                PROTOCOLLOLETTERA = trm.protocolloLettera,
                DATALETTERA = trm.dataLettera,
                DATAAGGIORNAMENTO = trm.dataAggiornamento,
                ANNULLATO = trm.annullato
            };

            db.TRASFERIMENTO.Add(tr);

            db.SaveChanges();
        }
    }
}