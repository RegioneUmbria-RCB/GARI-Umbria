namespace AgronicaCoreModelsSTD.attivita
{
    public class ComplianceResponse
    {
        public int IdTestata { get; set; }

        /// <summary>
        /// The execution error in case the request has failed (status -1)
        /// </summary>
        public string Error { get; set; } = "";
    }
}
