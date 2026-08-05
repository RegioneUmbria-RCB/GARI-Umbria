using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.AggregazionePayloadPerAzienda
{
    /// <summary>
    /// Contratto per l'aggregazione in-memory dei risultati di sostenibilità idrica
    /// e la creazione del payload JSON strutturato per la modalità "Per Azienda".
    /// <para>
    /// Il servizio somma le componenti H2O (fabbisogno, consumo, da meteo, delta) di tutti gli
    /// esercizi del perimetro aziendale-anno, pesandole per la rispettiva superficie, e produce
    /// un byte array UTF-8 del payload JSON conforme allo schema FS001 <c>/per_azienda_annuale</c>.
    /// </para>
    /// Riferimento spec: DS09-BL AggregazionePayloadPerAzienda.
    /// </summary>
    public interface IAggregazionePayloadPerAziendaService
    {
        /// <summary>
        /// Aggrega gli indicatori di sostenibilità idrica per tutti gli esercizi del perimetro
        /// aziendale e genera il payload JSON UTF-8 pronto per firma e persistenza.
        /// <para>
        /// Fase 1 — Aggregazione: <c>superficie_coltivata_ha = SUM(superficie_ha)</c>;
        /// ogni componente m³ = <c>SUM((valore_per_ha ?? 0) × superficie_ha)</c>.
        /// </para>
        /// <para>
        /// Fase 2 — Payload: costruisce la struttura JSON <c>{ aziende: [{ id_azienda, anni: [...] }] }</c>
        /// e la serializza come byte array UTF-8.
        /// </para>
        /// </summary>
        /// <param name="input">Metadati di calcolo e lista degli indicatori per esercizio.</param>
        /// <returns>
        /// JSON sotto forma di stringa
        /// </returns>
        /// <exception cref="ArgumentNullException">Se <paramref name="input"/> è null.</exception>
        /// <exception cref="ArgumentException">Se Filiera o Azienda sono vuote.</exception>
        /// <exception cref="Exceptions.InvalidAggregationException">
        /// Se la lista degli esercizi è vuota oppure la superficie totale è ≤ 0.
        /// </exception>
        /// <exception cref="Exceptions.PayloadGenerationException">
        /// Se la serializzazione JSON fallisce.
        /// </exception>
        string Aggrega(AggregazionePayloadPerAziendaInput input);
    }
}
