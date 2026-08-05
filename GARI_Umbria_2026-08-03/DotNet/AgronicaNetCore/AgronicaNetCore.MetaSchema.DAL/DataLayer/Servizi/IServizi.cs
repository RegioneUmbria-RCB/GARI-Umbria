using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreDTOStd.InData.Pratiche;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Servizi
{
    public interface IServizi
    {
        Task<DataTable> LeggiServiziEffettivamenteUsatiAsync(LeggiServizi_IN leggiServizi_IN, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiServizi_StatiAsync(LeggiServiziStati_IN leggiServiziStati_IN, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Restituisce il catalogo completo dei servizi/pratiche con supporto a filtro, ordinamento e paginazione.
        /// DS10 – API: Endpoint Recupero Catalogo Pratiche.
        /// </summary>
        Task<DataTable> LeggiCatalogoServiziAsync(LeggiCatalogoServizi_IN leggiCatalogoServizi_IN, AgronicaCoreParametriServer objParametriServer);
    }
}
