namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS07-BL §Output data[]: Represents a single particella catastale row with conduzione data.
    /// Each <see cref="ImpreseXParticelle"/> row (period) generates one instance,
    /// so a physical cadastral parcel may appear multiple times — once per conduzione period.
    /// </summary>
    /// <param name="CodiceAzienda">PIVA of the azienda (<c>ixp.PIVA</c>).</param>
    /// <param name="CodiceCentro">Centro code as string (<c>ixp.Sa_Cod</c>).</param>
    /// <param name="CodiceIstat">6-character ISTAT code (<c>CONCAT(pc.PROV, pc.COM)</c>).</param>
    /// <param name="Foglio">Cadastral sheet number (<c>pc.FOGLIO</c>).</param>
    /// <param name="Sezione">Cadastral section (<c>pc.SEZIONE</c>).</param>
    /// <param name="Particella">Cadastral parcel number (<c>pc.NUMERO</c>).</param>
    /// <param name="Subalterno">Cadastral sub-parcel (<c>pc.SUBALTERNO</c>).</param>
    /// <param name="SuperficieCatastaleHa">
    /// Registered cadastral area in hectares, computed as
    /// <c>pc.ETTARI + pc.ARE/100 + pc.CENTIARE/10000</c>.
    /// </param>
    /// <param name="SuperficieCondottaHa">Effective operated area in hectares (<c>ixp.Sup_Condotta</c>).</param>
    /// <param name="DataInizioConduzione">Conduzione period start (<c>ixp.Validita_Inizio</c>).</param>
    /// <param name="DataFineConduzione">Conduzione period end (<c>ixp.Validita_Fine</c>); null if open-ended.</param>
    /// <param name="CodiceConduzione">Numeric tenure type code (<c>ixp.TitoloPossesso</c>).</param>
    /// <param name="DescrizioneConduzione">
    /// Human-readable tenure description derived from <c>ixp.TitoloPossesso</c>:
    /// 1=Proprietà, 2=Comodato d'uso, 3=Affitto con contratto,
    /// 4=Affitto senza contratto, 5=In conto terzi, else=Altro.
    /// </param>
    public record ParticellaCatastaleItem(
        string CodiceAzienda,
        string CodiceCentro,
        string CodiceIstat,
        string Foglio,
        string? Sezione,
        string Particella,
        string? Subalterno,
        decimal SuperficieCatastaleHa,
        decimal? SuperficieCondottaHa,
        DateTime DataInizioConduzione,
        DateTime? DataFineConduzione,
        int CodiceConduzione,
        string DescrizioneConduzione
    );
}
