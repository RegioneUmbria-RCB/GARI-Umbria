using System.Data;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Formulati;

public interface IFormulati
{
    /// <summary>
    /// Legge i formulati dalla banca dati inclusi i periodi di sospensione, le unità di misura dose
    /// e il flag IsTrappolaFormulato.
    /// </summary>
    /// <param name="tipo">Tipo formulato (numerico). Se valorizzato e numerico, filtra per ClassCod e calcola IsTrappolaFormulato.</param>
    /// <param name="statoCod">Codice stato per filtro ambito estero (es. "IT"). Se vuoto non applica il join.</param>
    /// <param name="vegCodList">Lista di codici specie vegetale. Se non vuota, filtra su FormulatixSpecieVegetalixNormative.</param>
    /// <param name="objParametriServer">Parametri server.</param>
    Task<DataTable> LeggiAsync(
        int tipoRichiesto,
        string statoCod,
        List<int> vegCodList,
        AgronicaCoreParametriServer objParametriServer,
        bool leggiAPP = false
    );
}
