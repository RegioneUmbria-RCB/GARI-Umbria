namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class AnagraficaWrapper<T>
    {
        public string codice { get; set; }

        public string codice_esterno { get; set; }

        public T elemento_anagrafico { get; set; }
    }
}
