namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS08-BL §Output data[]: Represents a single appezzamento/impianto row for FS004,
    /// carrying agronomic, production, certification and relational fields.
    /// </summary>
    /// <param name="CodiceAzienda">PIVA of the azienda (<c>ese.Piva</c>).</param>
    /// <param name="CodiceCentro">Centro code (<c>ese.Sa_Cod</c>).</param>
    /// <param name="CodiceAppezzamento">Appezzamento code (<c>ese.Appezza</c>).</param>
    /// <param name="CodiceImpianto">Impianto (Id_Reg) code (<c>ese.Id_Reg</c>).</param>
    /// <param name="CodiceEsercizio">Progetto_Cod — also the keyset cursor value (<c>ese.Progetto_Cod</c>).</param>
    /// <param name="EsercizioInizioValidita">Esercizio validity start (<c>ese.Validita_Inizio</c>).</param>
    /// <param name="EsercizioFineValidita">Esercizio validity end (<c>ese.Validita_Fine</c>); null if open.</param>
    /// <param name="VincoloImpiantoCodice">Regulation code (<c>ese.Regolamento_Cod</c>).</param>
    /// <param name="VincoloImpiantoDescrizione">Regulation description from <c>Regolamenti</c> LEFT JOIN.</param>
    /// <param name="StatoEsercizioCodice">
    /// BI state code: <c>'2'</c> when <c>ese.StatoCod</c> ∈ {102,4,5,6,7,8,9,10,11,12,13,14,17},
    /// else <c>'1'</c>. Computed via SQL CASE (DS08-BL §Calcolo stato esercizio).
    /// </param>
    /// <param name="StatoEsercizioDescrizione">
    /// Human-readable state: <c>'parzialmente in produzione'</c> when <see cref="StatoEsercizioCodice"/> = '2',
    /// <c>'totalmente in produzione'</c> when '1'.
    /// </param>
    /// <param name="ProduzionePrevista">Production forecast in kg/Ha (<c>ese.produzione_prevista</c>); null if unset.</param>
    /// <param name="ProduzioneTotalePrevista">
    /// Total production forecast in kg, computed as <c>produzione_prevista × superficie</c>;
    /// null when either operand is null (DS08-BL §Calcolo produzione totale).
    /// </param>
    /// <param name="FlagSecondoRaccolto">Second-harvest flag (<c>ese.FlagSecondoRaccolto</c>).</param>
    /// <param name="RaccoltaPrevista">Expected harvest date (<c>ese.data_fine_prevista</c>); null if unset.</param>
    /// <param name="SpecieVegetaleCodice">Species code (<c>ese.Veg_Cod</c>).</param>
    /// <param name="SpecieVegetaleDescrizione">Species description from <c>SpecieVegetali</c> LEFT JOIN.</param>
    /// <param name="VarietaCodice">Variety code (<c>imp.CUL_COD</c>).</param>
    /// <param name="VarietaDescrizione">Variety description from <c>Cultivar</c> LEFT JOIN.</param>
    /// <param name="RaggruppamentoVarietaleCodice">Variety group code (<c>imp.GrVa_Cod_Veg</c>).</param>
    /// <param name="RaggruppamentoVarietaleDescrizione">Variety group description from <c>GruppoVarietale</c> LEFT JOIN.</param>
    /// <param name="GruppoVegetaleCodice">Macro vegetable group code from <c>SpecieVegetali.Gru_Cod</c>. <c>null</c> if not set.</param>
    /// <param name="VincoloImpiantoCodice">Crop regulation/purpose code (<c>imp.Grfi_cod</c>) — vincolo impianto.</param>
    /// <param name="VincoloImpiantoDescrizione">Crop regulation description from <c>GruppoFinalita</c> LEFT JOIN.</param>
    /// <param name="ContributoCodice">Disciplinary/contribution code (<c>ese.Disciplinare_Cod</c>). Null if unset.</param>
    /// <param name="FormaAllevamentoCodice">Training form code (<c>imp.foral_cod</c>).</param>
    /// <param name="PortinnestoCodice">Rootstock code (<c>imp.port_cod</c>).</param>
    /// <param name="ImpiantoInizioValidita">Impianto validity start (<c>imp.Validita_Inizio</c>).</param>
    /// <param name="Superficie">Impianto surface area in Ha (<c>imp.sup_imp</c>); null if unset.</param>
    /// <param name="NumeroPianteProduzione">Plants per hectare (<c>imp.P_HA</c>); null if unset.</param>
    /// <param name="DistanzaTraFileM">Row spacing in meters (<c>imp.TRA_FILA</c>); null if unset.</param>
    /// <param name="DistanzaSuFilaM">In-row spacing in meters (<c>imp.SU_FILA</c>); null if unset.</param>
    /// <param name="FlagIrrigazione">1 if <c>imp.Imp_Cod &gt; 0</c>, else 0.</param>
    /// <param name="FlagSerra">1 if <c>imp.Cop_Cod &gt; 0</c>, else 0.</param>
    /// <param name="NumeroAppezzamento">Descriptive name of the appezzamento (<c>app.APP_NOME</c>).</param>
    /// <param name="PartitaIvaFornitore">PIVA from <c>Imprese_Codici</c> id_cod=1324. Null if not present.</param>
    /// <param name="TecnicoResponsabile">Full name of the responsible technician from <c>Imprese_Codici</c> id_cod=1088 → <c>Contatti</c>.</param>
    /// <param name="MagazzinoConferimentoCodice">Warehouse code from <c>Imprese_Codici</c> id_cod=1317.</param>
    /// <param name="FlagFruizioneContributi">1 if <c>imp.flagContributo = 'S'</c>, else 0.</param>
    /// <param name="KgConferiti">Total kg delivered for the esercizio. 0 when no data is available.</param>
    /// <param name="SuperficieAbbattuta">
    /// Cumulative knocked-down surface in Ha from abbattimento movements
    /// (<c>lav_cod=170, cau_mov='2300'</c>). 0 when no abbattimento exists.
    /// </param>
    /// <param name="PercentualeMoria">
    /// Cumulative plant mortality percentage from technical damage surveys
    /// (<c>lav_cod=108, cau_mov='2200', ff_classe IN (28,29)</c>). 0 when no record exists.
    /// </param>
    /// <param name="ProduzioneStimataEffettiva">
    /// Estimated effective production in kg after damage adjustments (legacy §Stime_Produzione CASE formula).
    /// 0 when the impianto is inactive or not yet producing.
    /// </param>
    /// <param name="NoteDanni">
    /// Human-readable damage label (e.g. <c>"Abbattimento parziale e moria"</c>, <c>"Moria parziale"</c>,
    /// <c>"Abbattimento parziale"</c>, <c>"Impianto non attivo"</c>). Empty when no damage is recorded.
    /// </param>
    public record AppezzamentoItem(
        string CodiceAzienda,
        string CodiceCentro,
        string CodiceAppezzamento,
        string CodiceImpianto,
        string CodiceEsercizio,
        DateTime EsercizioInizioValidita,
        DateTime EsercizioFineValidita,
        string VincoloImpiantoCodice,
        string VincoloImpiantoDescrizione,
        string StatoEsercizioCodice,
        string StatoEsercizioDescrizione,
        decimal ProduzionePrevista,
        decimal ProduzioneTotalePrevista,
        int FlagSecondoRaccolto,
        DateTime? RaccoltaPrevista,
        string SpecieVegetaleCodice,
        string SpecieVegetaleDescrizione,
        string VarietaCodice,
        string VarietaDescrizione,
        string RaggruppamentoVarietaleCodice,
        string RaggruppamentoVarietaleDescrizione,
        string GruppoVegetaleCodice,
        string FinalitaProduttivaCodice,
        string FinalitaProduttivaDescrizione,
        string ContributoCodice,
        string ContributoDescrizione,
        string FormaAllevamentoCodice,
        int PortinnestoCodice,
        DateTime ImpiantoInizioValidita,
        DateTime? DataAbbattimento,
        decimal Superficie,
        decimal NumeroPianteProduzione,
        decimal DistanzaTraFileM,
        decimal DistanzaSuFilaM,
        int FlagIrrigazione,
        int FlagSerra,
        string NumeroAppezzamento,
        string PartitaIvaFornitore,
        string PartitaIvaConferimento,
        string TecnicoResponsabile,
        string TecnicoCf,
        string MagazzinoConferimentoCodice,
        string MagazzinoConferimentoDescrizione,
        string PartitaIvaAziendaLivello1,
        int FlagFruizioneContributi,
        decimal KgConferiti,
        string CertificazioneAziendaleCodice,
        string CertificazioneProdottoCodice,
        DateTime? DataUltimoRilievoProducuzionePrevista,
        decimal? ResaUltimoRilievoProduzionePrevistaKgHa,
        decimal SuperficieAbbattuta,
        decimal? PercentualeMoria,
        decimal ProduzioneStimataEffettiva,
        DateTime? DataRilievoDanni,
        decimal? ResaUltimoRilievoProduzionePrevistaKgTot,
        string CertificazioneCommercialeCodice,
        string ResiduoCodice,
        int StatoProduzione,
        string NoteDanni,
        string SpecieWmsCodice,
        string VarietaWmsCodice,
        string CodiceStagionalitaWms
    );
}
