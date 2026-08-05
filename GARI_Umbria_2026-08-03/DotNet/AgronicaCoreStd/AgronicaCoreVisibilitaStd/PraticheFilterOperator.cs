using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreVisibilitaStd
{
    /// <summary>
    /// Definisce l'operatore da applicare al filtro di visibilità pratiche rispetto a quello sulle imprese.
    /// </summary>
    public enum PraticheFilterOperator
    {
        /// <summary>
        /// Filtro esclusivo: un'impresa per essere in visibilità deve soddisfare 
        /// sia il filtro sulle imprese che quello sulle pratiche.
        /// </summary>
        And = 0,

        /// <summary>
        /// Filtro inclusivo: un'impresa è in visibilità se soddisfa il filtro
        /// sulle imprese oppure quello sulle pratiche.
        /// </summary>
        Or = 1
    }
}
