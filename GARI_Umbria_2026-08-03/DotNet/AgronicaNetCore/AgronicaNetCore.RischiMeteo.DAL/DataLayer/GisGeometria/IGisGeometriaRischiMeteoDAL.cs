using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.RischiMeteo.DAL.DataLayer.GisGeometria
{
    /// <summary>
    /// Contratto per il recupero della geometria (poligono e centroide WKT) di un appezzamento
    /// dalle tabelle GIS GIAS, per la costruzione del payload M2.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Mapping geoImpianto.
    /// </summary>
    public interface IGisGeometriaRischiMeteoDAL
    {
        /// <summary>
        /// Recupera il poligono WKT dal campo <c>Poligono_GeoEntity_WKT</c> in <c>GIS_ElementiGrafici</c>
        /// tramite join con <c>GIS_Entita</c>.
        /// </summary>
        //Task<string?> GetPoligonoWktAsync(
        //    string piva, int saCod, int appezza,
        //    AgronicaCoreParametriServer objParametriServer);

        ///// <summary>
        ///// Recupera il centroide WKT dal campo <c>Centroide_GeoEntity_WKT</c> in
        ///// <c>GIS_ElementiGrafici_Clustering</c> tramite join con <c>GIS_ElementiGrafici</c>
        ///// e <c>GIS_Entita</c>.
        ///// </summary>
        //Task<string?> GetCentroideWktAsync(string piva, int saCod, int appezza, AgronicaCoreParametriServer objParametriServer);

        ///// <summary>
        ///// Recupera il centroide WKT dal campo <c>Centroide_GeoEntity_WKT</c> in
        ///// <c>GIS_ElementiGrafici_Clustering</c> tramite join con <c>GIS_ElementiGrafici</c>
        ///// e <c>GIS_Entita</c>.
        ///// </summary>
        //Task<string?> GetCentroideWktAsync(string poligonoWkt);
    }
}
