using System;

namespace OutData.Zoo
{
    /// <summary>
    /// Risultato del processo di export generato da DS07-API.
    /// Contiene i bytes del file in memoria, il content-type e il nome file proposto.
    /// Vedere DS07-API, sezione "Risposte / 200".
    /// </summary>
    public class PesatureExportFileResult
    {
        /// <summary>Contenuto del file generato in memoria.</summary>
        public byte[] Content { get; set; } = new byte[0];

        /// <summary>MIME type del file (text/csv oppure application/vnd.openxmlformats-officedocument.spreadsheetml.sheet).</summary>
        public string ContentType { get; set; } = "text/csv";

        /// <summary>Nome file da proporre nel Content-Disposition.</summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>Numero di record dati nel file (escluso header e metadata footer).</summary>
        public int RecordCount { get; set; }

        /// <summary>Dimensione in byte del file generato.</summary>
        public long FileSizeBytes { get; set; }
    }

    /// <summary>
    /// Riga aggregata per export DS07 con view_mode=aggregated.
    /// Vedere DS07-API, sezione "Risposte / 200 - Aggregated".
    /// </summary>
    public class PesatureExportRigaAggregata
    {
        public string Gruppo { get; set; } = string.Empty;
        public string GruppoDes { get; set; } = string.Empty;
        public int NumeroAnimali { get; set; }
        public int NumeroPesate { get; set; }
        public double ScostamentoMedioPct { get; set; }
        public double ScostamentoMaxPct { get; set; }
        public double ScostamentoMinPct { get; set; }
        public int AlertCount { get; set; }
        public DateTime UltimoAggiornamento { get; set; }
    }
}
