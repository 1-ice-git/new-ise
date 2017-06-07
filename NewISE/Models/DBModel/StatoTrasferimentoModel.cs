using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public enum EnumStatoTraferimento
    {
        Attivo = 1,
        Da_Attivare = 2,
        Non_Trasferito = 3,
        Terminato = 4
    }

    public class StatoTrasferimentoModel
    {
        [Key]
        public decimal idStatoTrasferimento { get; set; }
        [Required(ErrorMessage = "La descrizione è richiesta.")]
        [Display(Name = "Descrizione")]
        [StringLength(50, ErrorMessage = "Per la descrione sono richiesti un massimo di 50 caratteri.")]
        [DataType(DataType.Text)]
        [CustomValidation(typeof(dtStatoTrasferimento), "DescrizioneStatoTrasferimentoUnivoco", ErrorMessage = "La descrizione inserita è già presente, inserirne un altra.")]
        public string descrizioneStatoTrasferimento { get; set; }

        public static explicit operator EnumStatoTraferimento(StatoTrasferimentoModel v)
        {
            if (v.idStatoTrasferimento == 1)
            {
                return EnumStatoTraferimento.Attivo;
            }
            else if (v.idStatoTrasferimento == 2)
            {
                return EnumStatoTraferimento.Da_Attivare;
            }
            else if (v.idStatoTrasferimento == 3)
            {
                return EnumStatoTraferimento.Non_Trasferito;
            }
            else if (v.idStatoTrasferimento == 4)
            {
                return EnumStatoTraferimento.Terminato;
            }
            else
            {
                throw new Exception("Enumeratore stato trasferimento errato.");
            }
        }
    }
}