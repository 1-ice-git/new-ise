using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtTrasferimento : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public TrasferimentoModel GetUltimoTrasferimentoByMatricola(int matricola)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            try
            {
                using (EntitiesDBISE db=new EntitiesDBISE())
                {
                    var lt = db.DIPENDENTI.Where(a => a.MATRICOLA == matricola).First().TRASFERIMENTO.Where(a=>a.ANNULLATO == false);

                    var lt1 = lt.Where(a=>a.IDTRASFERIMENTO == lt.Max(b=>b.IDTRASFERIMENTO));

                    if (lt1 != null || lt1.Count() > 0)
                    {
                        var t = lt1.First();

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
                            ufficio = new UfficiModel()
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
                                abilitato = t.DIPENDENTI.ABILITATO
                            }

                        
                            
                        };
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