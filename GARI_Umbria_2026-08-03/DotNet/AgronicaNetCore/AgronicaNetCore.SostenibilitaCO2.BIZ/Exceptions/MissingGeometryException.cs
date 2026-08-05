namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione bloccante sollevata quando la geometria poligonale di un appezzamento
    /// è assente o invalida in <c>GIS_ElementiGrafici</c> e il centroide azienda di fallback
    /// è anch'esso assente o non valorizzato.
    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — Eccezioni, Regola 2.
    /// </summary>
    public class MissingGeometryException : Exception
    {
        /// <summary>Identificativo dell'appezzamento privo di geometria (<c>PIVA|Sa_Cod|Appezza</c>).</summary>
        public string IdAppezzamento { get; }

        public MissingGeometryException(string idAppezzamento)
            : base($"Geometria assente o invalida per l'appezzamento '{idAppezzamento}'. Impossibile calcolare il centroide e il centroide azienda di fallback è assente.")
        {
            IdAppezzamento = idAppezzamento;
        }

        public MissingGeometryException(string idAppezzamento, Exception innerException)
            : base($"Geometria assente o invalida per l'appezzamento '{idAppezzamento}'. Impossibile calcolare il centroide e il centroide azienda di fallback è assente.", innerException)
        {
            IdAppezzamento = idAppezzamento;
        }
    }
}
