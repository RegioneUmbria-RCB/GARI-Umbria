namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione bloccante sollevata quando un appezzamento del perimetro non ha
    /// alcun impianto valido associato dopo il filtro <c>area_ha &gt; 0</c>.
    /// Indica un'inconsistenza nei dati GIAS o criteri di filtro DS01-BL troppo restrittivi.
    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — Eccezioni, Regola 9.
    /// </summary>
    public class IncompletePerimeterException : Exception
    {
        /// <summary>Identificativo dell'appezzamento privo di impianti (<c>PIVA|Sa_Cod|Appezza</c>).</summary>
        public string IdAppezzamento { get; }

        public IncompletePerimeterException(string idAppezzamento)
            : base($"Appezzamento '{idAppezzamento}' nel perimetro non ha impianti validi associati. Verificare i dati GIAS o i criteri di filtro DS01-BL.")
        {
            IdAppezzamento = idAppezzamento;
        }

        public IncompletePerimeterException(string idAppezzamento, Exception innerException)
            : base($"Appezzamento '{idAppezzamento}' nel perimetro non ha impianti validi associati. Verificare i dati GIAS o i criteri di filtro DS01-BL.", innerException)
        {
            IdAppezzamento = idAppezzamento;
        }
    }
}
