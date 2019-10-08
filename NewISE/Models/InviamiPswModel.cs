using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class InviamiPswModel
    {
        [Required(ErrorMessage = @"La matricola è richiesta.")]
        public string matricola { get; set; }
    }
}