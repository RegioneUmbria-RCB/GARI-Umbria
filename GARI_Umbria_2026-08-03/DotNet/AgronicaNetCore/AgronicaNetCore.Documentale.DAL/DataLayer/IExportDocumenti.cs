using AgronicaNetCore.Base.Models;
using OutData.DataExchange;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Documentale.DAL.DataLayer
{
    public interface IExportDocumenti
    {
        Task<DataTable> LeggiDocumentiExportAsync(string CUAA, int Tipologia_Cod, DateTime DataRiferimento, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiMetadatiExportAsync(List <int> IdDocumentList, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiDocumentiAnalisiPDCExportAsync(string CUAA, DateTime DataRiferimento, AgronicaCoreParametriServer objParametriServer);
        Task<bool> AggiornaPubblicazioneAnalisiPDCAsync(int IdAnalisi, int Stato, AgronicaCoreParametriServer objParametriServer);
        Task<bool> AggiornaPubblicazioneAnalisiPDCAsync(string CUAA, AgronicaCoreParametriServer objParametriServer);
        Task<bool> ScriviExportDocumentiAnalisiPDCAsync(string CUAA, int IdAnalisi, int IdDocumento, int Stato, AgronicaCoreParametriServer objParametriServer);
        Task<bool> AggiornaExportDocumentiAnalisiPDCAsync(string CUAA, List<int> IdDocumenti, int Stato, AgronicaCoreParametriServer objParametriServer);
    }
}



