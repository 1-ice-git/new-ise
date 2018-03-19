using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewISE.EF;

namespace NewISE.Models.dtObj.Interfacce
{
    interface IricalcoloParametri
    {
        #region Coefficiente di richiamo
        /// <summary>
        /// Associa il coefficente di richiamo alla tabella richiamo.
        /// </summary>
        /// <param name="idCoeffRichiamo"></param>
        /// <param name="db"></param>
        void AssociaRichiamo_CR(decimal idCoeffRichiamo, ModelDBISE db);
        /// <summary>
        /// Asscia il coefficente di richiamo alla tabella riduzioni.
        /// </summary>
        /// <param name="idCoeffRichiamo"></param>
        /// <param name="db"></param>
        void AssociaRiduzioni_CR(decimal idCoeffRichiamo, ModelDBISE db);
        #endregion

        #region Coefficiente di sede
        /// <summary>
        /// Associa il coefficiente di sede alla tabella indennità
        /// </summary>
        /// <param name="idCoefficenteSede"></param>
        /// <param name="db"></param>
        void AssociaIndennita_CS(decimal idCoefficenteSede, ModelDBISE db);
        #endregion

        #region Indennità di base
        /// <summary>
        /// Associa l'indennità di base alla tabella indennità.
        /// </summary>
        /// <param name="idIndBase"></param>
        /// <param name="db"></param>
        void AssociaIndennitaBase_IB(decimal idIndBase, ModelDBISE db);
        /// <summary>
        /// Associa l'indennità di base alla tabella riduzioni.
        /// </summary>
        /// <param name="idIndBase"></param>
        /// <param name="db"></param>
        void AssociaRiduzioniIB(decimal idIndBase, ModelDBISE db);
        #endregion

        #region Indennità di primo segretario
        /// <summary>
        /// Associa l'indennità di primo segretario alla tabella figli.
        /// </summary>
        /// <param name="idPrimoSegretario"></param>
        /// <param name="db"></param>
        void AssociaFigli_IPS(decimal idPrimoSegretario, ModelDBISE db);
        #endregion

        #region Indennita di sistemazione
        /// <summary>
        /// Associa l'indennità di sistemnazione alla tabella PrimaSistemazione.
        /// </summary>
        /// <param name="idIndSistemazione"></param>
        /// <param name="db"></param>
        void AssociaPrimaSistemazione_IS(decimal idIndSistemazione, ModelDBISE db);
        /// <summary>
        /// Associa l'indennità di prima sistemazione alla tabella Riduzioni.
        /// </summary>
        /// <param name="idIndSistemazione"></param>
        /// <param name="db"></param>
        void AssociaRiduzioni_IS(decimal idIndSistemazione, ModelDBISE db);

        #endregion

        #region Maggiorazioni annuali
        /// <summary>
        /// Associa le maggiorazioni annuali alla tabella MaggiorazioneAbitazione.
        /// </summary>
        /// <param name="idMagAnnuali"></param>
        /// <param name="db"></param>
        void AssociaMaggiorazioniAbitazione_MA(decimal idMagAnnuali, ModelDBISE db);
        #endregion

        #region Percentuale anticio trasporto effetti
        /// <summary>
        /// Associa la percentuale di anticipo trasporto effetti alla tabella TEPartenza.
        /// </summary>
        /// <param name="idPercentualeAnticipoTEP"></param>
        /// <param name="db"></param>
        void AssociaPercentualeAnticipoTEP(decimal idPercentualeAnticipoTEP, ModelDBISE db);
        /// <summary>
        /// Associa la percentuale di anticipo trasporto effetti alla tabella TERientro.
        /// </summary>
        /// <param name="idPercentualeAnticipoTEP"></param>
        /// <param name="db"></param>
        void AssociaPercentualeAnticipoTER(decimal idPercentualeAnticipoTEP, ModelDBISE db);
        #endregion

        #region Percentuale condivisione

        /// <summary>
        /// Associa la percentuale di condivisione alla maggiorazione abitazione tramite la tabella PagatoCondivisoMAB.
        /// </summary>
        /// <param name="idPercentualeCondivisione"></param>
        /// <param name="db"></param>
        void AssociaPagatoCondivisoMAB(decimal idPercentualeCondivisione, ModelDBISE db);
        #endregion

        #region Percentuale chilometrica
        /// <summary>
        /// Associa la percentuale di fascia chilometrica alla tabella PrimaSistemazione.
        /// </summary>
        /// <param name="idPercKM"></param>
        /// <param name="db"></param>
        void AssociaPrimaSistemazione_PKM(decimal idPercKM, ModelDBISE db);

        /// <summary>
        /// Associa la percentuale di fascia chilometrica alla tabella Richiamo.
        /// </summary>
        /// <param name="idPercKM"></param>
        /// <param name="db"></param>
        void AssociaRichiamo_PKM(decimal idPercKM, ModelDBISE db);
        #endregion

        #region Percentuale di disagio
        /// <summary>
        /// Associa la percentuale di disagio alla tabella Indennita.
        /// </summary>
        /// <param name="idPercDisagio"></param>
        /// <param name="db"></param>
        void AssociaIndenita_PD(decimal idPercDisagio, ModelDBISE db);

        #endregion

        #region PercentualeMAB
        /// <summary>
        /// Associa la percentuale MAB alla tabella MAB.
        /// </summary>
        /// <param name="idPerceMAB"></param>
        /// <param name="db"></param>
        void AssociaMAB_PMAB(decimal idPerceMAB, ModelDBISE db);
        #endregion

        #region Percentuale maggiorazione coniuge
        /// <summary>
        /// Associa la percentuale di maggiorazione coniuge alla tabella Coniuge.
        /// </summary>
        /// <param name="idPercMagConiuge"></param>
        /// <param name="db"></param>
        void AssociaConiuge_PMC(decimal idPercMagConiuge, ModelDBISE db);

        #endregion

        #region Percentuale maggiorazione figli
        /// <summary>
        /// Associa la percentuale di maggiorazione figli alla tabella figli.
        /// </summary>
        /// <param name="idPercFiglio"></param>
        /// <param name="db"></param>
        void AssociaFiglio_PMF(decimal idPercFiglio, ModelDBISE db);

        #endregion

        #region Riduzioni
        /// <summary>
        /// Associa le riduzioni alla tabella Indennitabase.
        /// </summary>
        /// <param name="idRiduzioni"></param>
        /// <param name="db"></param>
        void AssociaIndennitaBase_Riduzioni(decimal idRiduzioni, ModelDBISE db);
        /// <summary>
        /// Associa le riduzioni alla tabella CoefficenteIndRichiamo.
        /// </summary>
        /// <param name="idRiduzioni"></param>
        /// <param name="db"></param>
        void AssociaCoefficienteRichiamo_Riduzioni(decimal idRiduzioni, ModelDBISE db);
        /// <summary>
        /// Associa le riduzioni alla tabella IndennitaSistemazione.
        /// </summary>
        /// <param name="idRiduzioni"></param>
        /// <param name="db"></param>
        void AssociaIndennitaSistemazione_Riduzioni(decimal idRiduzioni, ModelDBISE db);

        #endregion

        #region TFR
        /// <summary>
        /// Associa il tfr alla tabella Indennita.
        /// </summary>
        /// <param name="idTFR"></param>
        /// <param name="db"></param>
        void AssociaIndennita_TFR(decimal idTFR, ModelDBISE db);
        /// <summary>
        /// Associa il tfr alla tabella CanoneMAB.
        /// </summary>
        /// <param name="idTFR"></param>
        /// <param name="db"></param>
        void AssociaCanoneMAB_TFR(decimal idTFR, ModelDBISE db);

        #endregion



    }
}
