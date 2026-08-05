namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione non bloccante (warning) sollevata quando un impianto presenta
    /// <c>area_ha</c> pari a zero o null, rendendolo non idoneo per il calcolo CO2.
    /// L'impianto viene omesso dal payload; il flusso prosegue normalmente.
    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — Eccezioni, DataInconsistencyException.
    /// </summary>
    public class DataInconsistencyException : Exception
    {
        /// <summary>Identificativo dell'impianto con dato inconsistente (<c>PIVA|Sa_Cod|Appezza|ID_Reg|Progetto_Cod</c>).</summary>
        public string IdImpianto { get; }

        public DataInconsistencyException(string idImpianto)
            : base($"Impianto '{idImpianto}' omesso dal payload: area_ha è zero o null (dato inconsistente).")
        {
            IdImpianto = idImpianto;
        }

        public DataInconsistencyException(string idImpianto, Exception innerException)
            : base($"Impianto '{idImpianto}' omesso dal payload: area_ha è zero o null (dato inconsistente).", innerException)
        {
            IdImpianto = idImpianto;
        }
    }
}
