using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public enum EnumTipologiaCoan
    {
        Servizi_Istituzionali = 1,
        Servizi_Promozionali = 2
    }

    public class dtTrasferimento : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static ValidationResult VerificaRequiredCoan(string v, ValidationContext context)
        {

            ValidationResult vr = ValidationResult.Success;

            var tr = context.ObjectInstance as TrasferimentoModel;

            
            if (tr.idTipoCoan == Convert.ToDecimal(EnumTipologiaCoan.Servizi_Promozionali))
            {
                if (tr.coan != string.Empty && tr.coan.Length == 10)
                {
                    vr= ValidationResult.Success;
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

            return vr;
            
        }

        public TrasferimentoModel GetUltimoTrasferimentoByMatricola(string matricola)
        {
            TrasferimentoModel tm = new TrasferimentoModel();
            int matr = Convert.ToInt16(matricola);
            try
            {
                using (EntitiesDBISE db=new EntitiesDBISE())
                {
                    
                    var ldp = db.DIPENDENTI.Where(a => a.MATRICOLA == matr);

                    if (ldp != null && ldp.Count() > 0)
                    {
                        var lt = ldp.First().TRASFERIMENTO.Where(a => a.ANNULLATO == false).ToList();                       

                        if (lt != null || lt.Count() > 0)
                        {
                            var t = lt.First();

                            tm = new TrasferimentoModel()
                            {
                                idTrasferimento = t.IDTRASFERIMENTO,
                                idTipoTrasferimento = t.IDTIPOTRASFERIMENTO,
                                idUfficio = t.IDUFFICIO,
                                idStatoTrasferimento = t.IDSTATOTRASFERIMENTO,
                                idRuolo = t.IDRUOLO,
                                idDipendente = t.IDDIPENDENTE,
                                dataPartenza = t.DATAPARTENZA,
                                dataRientro = t.DATARIENTRO,
                                coan = t.COAN,
                                protocolloLettera = t.PROTOCOLLOLETTERA,
                                dataLettera = t.DATALETTERA,
                                dataAggiornamento = t.DATAAGGIORNAMENTO,
                                annullato = t.ANNULLATO,
                                RuoloUfficio = new RuoloUfficioModel()
                                {
                                    idRuoloUfficio = t.RUOLOUFFICIO.IDRUOLO,
                                    DescrizioneRuolo = t.RUOLOUFFICIO.DESCRUOLO
                                },
                                StatoTrasferimento = new StatoTrasferimentoModel()
                                {
                                    idStatoTrasferimento = t.STATOTRASFERIMENTO.IDSTATOTRASFERIMENTO,
                                    descrizioneStatoTrasferimento = t.STATOTRASFERIMENTO.DESCRIZIONE
                                },
                                TipoTrasferimento = new TipoTrasferimentoModel()
                                {
                                    idTipoTrasferimento = t.TIPOTRASFERIMENTO.IDTIPOTRASFERIMENTO,
                                    tipologiaTrasferimento = t.TIPOTRASFERIMENTO.TIPOTRASFERIMENTO1
                                },
                                Ufficio = new UfficiModel()
                                {
                                    idUfficio = t.UFFICI.IDUFFICIO,
                                    codiceUfficio = t.UFFICI.CODICEUFFICIO,
                                    DescUfficio = t.UFFICI.DESCRIZIONEUFFICIO
                                },
                                Dipendente = new DipendentiModel()
                                {
                                    idDipendente = t.DIPENDENTI.IDDIPENDENTE,
                                    matricola = t.DIPENDENTI.MATRICOLA,
                                    nome = t.DIPENDENTI.NOME,
                                    cognome = t.DIPENDENTI.COGNOME,
                                    dataAssunzione = t.DIPENDENTI.DATAASSUNZIONE,
                                    dataCessazione = t.DIPENDENTI.DATACESSAZIONE,
                                    indirizzo = t.DIPENDENTI.INDIRIZZO,
                                    cap = t.DIPENDENTI.CAP,
                                    citta = t.DIPENDENTI.CITTA,
                                    provincia = t.DIPENDENTI.PROVINCIA,
                                    email = t.DIPENDENTI.EMAIL,
                                    telefono = t.DIPENDENTI.TELEFONO,
                                    fax = t.DIPENDENTI.FAX,
                                    abilitato = t.DIPENDENTI.ABILITATO,
                                    dataInizioRicalcoli = t.DIPENDENTI.DATAINIZIORICALCOLI
                                }
                            };
                        }
                        
                    }
                   

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return tm;

        }
    }
}