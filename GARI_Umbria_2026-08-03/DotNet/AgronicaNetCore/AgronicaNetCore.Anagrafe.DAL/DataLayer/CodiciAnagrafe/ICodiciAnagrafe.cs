using System.Data;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.CodiciAnagrafe
{
    public interface ICodiciAnagrafe
    {
        Task<List<CodiceAnagrafeBase>> LeggiCodiciUsatixEntitaUsatixEntitaAsync(
            int entitaLetturaCodici,
            int idBudget,
            AgronicaCoreParametriServer objParametriServer
        );

        Task<DataTable> LeggiCodiciUsatixEntitaDestinazioniDUsoAsync(
            AgronicaCoreParametriServer objParametriServer
        );

        Task<DataTable> LeggiCodiciImpresaAsync(
            List<string> chiaviAzienda,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
        );

        Task<DataTable> LeggiCodiciCentroAsync(
            List<(string, int)> chiaviCentro,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
        );

        Task<DataTable> LeggiCodiciAppezzamentiAsync(
            List<(string, int, int)> chiaviAppezzamento,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
        );
        Task<DataTable> LeggiCodiciImpiantiAsync(
            List<(string, int, int, int)> chiaviImpianto,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
        );
        Task<DataTable> LeggiCodiciEserciziAsync(
            List<(string, int, int, int, int)> chiaviEsercizio,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
        );
        Task<DataTable> LeggiCodiciContattiAsync(
            List<(string, int)> chiaviContatto,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
        );
        Task<DataTable> LeggiCodiciFabbricatiAsync(
            List<(string, int, int)> chiaviFabbricato,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
        );

        Task<DataTable> LeggiCodiciDestinazioniUsoAsync(List<(string, int, int, int)> chiaviImpianto, AgronicaCoreParametriServer objParametriServer);
    }
}
