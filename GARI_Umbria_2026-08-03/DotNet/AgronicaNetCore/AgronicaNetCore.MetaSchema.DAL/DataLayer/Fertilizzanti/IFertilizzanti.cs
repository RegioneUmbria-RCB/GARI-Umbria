using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Fertilizzanti;

public interface IFertilizzanti
{
    /// <summary>
    /// Legge i fertilizzanti filtrati per i parametri indicati.
    /// TipoRichiesto:
    ///   0 = Tutti i fertilizzanti
    ///   1 = Trattamenti Antibutteratura
    ///   2 = Concimazione Fogliare
    ///   3 = Fertirrigazione
    ///   4 = Concimazione Organica
    ///   5 = Concimazione pieno Campo
    ///   6 = Ammendanti + Palabili + Liquami del PUA 2007
    ///   7 = Ammendanti + Palabili + Liquami del PAN 2012
    ///   8 = Ammendanti + Palabili + Liquami del PAN 2016
    /// Regolamento_Cod = -2 filtra solo i fertilizzanti biologici.
    /// </summary>
    Task<DataTable> LeggiAsync(
        int ferCod,
        string ferDes,
        int tipoRichiesto,
        int regolamentoCod,
        DateTime validitaInizio,
        DateTime validitaFine,
        bool includiTipologia,
        string statoCod,
        AgronicaCoreParametriServer objParametriServer
    );

    /// <summary>
    /// Legge i macro-elementi (N, P2O5, K2O, Cu) per una lista di codici fertilizzante.
    /// Colonne restituite: Fer_Cod, N, P2O5, K2O, Cu.
    /// </summary>
    Task<DataTable> LeggiMacroElementiAsync(
        List<int> ferCodList,
        AgronicaCoreParametriServer objParametriServer
    );
}
