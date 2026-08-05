namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS09-BL §Output data[]: Represents a single flattened appezzamento–particella
    /// mapping relation for FS005. One physical relation may produce multiple rows
    /// when a particella spans several appezzamento periods.
    /// </summary>
    /// <param name="CodiceEsercizio">
    /// <c>i.Progetto_Cod</c> — also the first part of the 3-key keyset cursor.
    /// </param>
    /// <param name="ParticellaCodice">
    /// <c>p.PART_COD</c> — second part of the 3-key cursor.
    /// </param>
    /// <param name="ValiditaInizioRelazione">
    /// <c>a.Validita_inizio</c> — third part of the 3-key cursor.
    /// </param>
    /// <param name="CodiceAzienda">PIVA of the azienda (<c>a.PIVA</c>).</param>
    /// <param name="CodiceCentro">Centro code (<c>a.sa_cod</c>).</param>
    /// <param name="CodiceAppezzamento">Appezzamento code (<c>a.APPEZZA</c>).</param>
    /// <param name="CodiceImpianto">Impianto (Id_Reg) recovered via JOIN on <c>i.Id_Reg</c>.</param>
    /// <param name="Provincia">Province code (<c>a.PROV</c>).</param>
    /// <param name="Comune">Municipality code (<c>a.com</c>).</param>
    /// <param name="Sezione">Cadastral section (<c>a.SEZIONE</c>); null if absent.</param>
    /// <param name="Foglio">Cadastral sheet number (<c>a.FOGLIO</c>).</param>
    /// <param name="Particella">Cadastral parcel number (<c>a.NUMERO</c>).</param>
    /// <param name="Subalterno">Cadastral sub-parcel (<c>a.SUBALTERNO</c>); null if absent.</param>
    /// <param name="CodiceIstat">6-character ISTAT code (<c>CONCAT(a.PROV, a.com)</c>).</param>
    /// <param name="ValiditaFineRelazione">Relation end date (<c>a.Validita_Fine</c>); null if open.</param>
    /// <param name="SuperficieAttribuitaHa">
    /// Portion of the parcella allocated to the appezzamento in Ha (<c>a.AREA</c>),
    /// 4 decimal places, no forced rounding. Exposed as-is even if the sum of attributed
    /// surfaces exceeds the cadastral surface (DS09-BL §Gestione superfici).
    /// </param>
    public record MappingAppezzamentiParticelleItem(
        string CodiceEsercizio,
        string ParticellaCodice,
        DateTime ValiditaInizioRelazione,
        string CodiceAzienda,
        string CodiceCentro,
        string CodiceAppezzamento,
        string CodiceImpianto,
        string Provincia,
        string Comune,
        string? Sezione,
        string Foglio,
        string Particella,
        string? Subalterno,
        string CodiceIstat,
        DateTime? ValiditaFineRelazione,
        decimal SuperficieAttribuitaHa
    );
}
