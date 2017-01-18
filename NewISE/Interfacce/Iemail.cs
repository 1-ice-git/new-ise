using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NewISE.Interfacce
{
    interface Iemail
    {
        /// <summary>
        /// Funzione generica per l'invio dei messaggi e-mail comprensivo di eventuali allegati.
        /// </summary>
        /// <param name="msgMail">Modello per l'invio dei messaggi compreso di eventuali allegati.</param>
        /// <returns>Vero se la/le mail viene/vengono inviata/e, falso se il contrario.</returns>
        bool sendMail(ModelloMsgMail msgMail);
    }
}
