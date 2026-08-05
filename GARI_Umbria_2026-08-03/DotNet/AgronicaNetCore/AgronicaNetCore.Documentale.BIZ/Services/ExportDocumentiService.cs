using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreModelsSTD.analisi;
using AgronicaCoreModelsSTD.pianiDiCampionamento;
using AgronicaDataProvider6.Providers;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Documentale.BIZ.Resources;
using AgronicaNetCore.Documentale.DAL.DataLayer;
using AgronicaNetCore.Utility.DAL.DataLayer.ObjectUtility;
using InData.DataExchange;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OutData.DataExchange;
using Serilog.Context;
using System.Data;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.Documentale.BIZ.Services
{
    public class ExportDocumentiService : BaseServiceExportDocumentiBIZ, IExportDocumentiService

    {
        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly ICompressioneDecompressione _compressioneDecompressione;
        private readonly IExportDocumenti _exportDocumenti;
        private readonly IExportDocumenti _exportDocumentiMetadati;
        private readonly ILogger<ExportDocumentiService> _logger;

        public ExportDocumentiService(ILogger<ExportDocumentiService> logger, IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _securityLayerDal = _serviceProvider.GetRequiredService<ISecurityLayerDAL>();
            _compressioneDecompressione = _serviceProvider.GetRequiredService<ICompressioneDecompressione>();
            _exportDocumenti = _serviceProvider.GetRequiredService<IExportDocumenti>();
            _exportDocumentiMetadati = _serviceProvider.GetRequiredService<IExportDocumenti>();
            _logger = logger;
        }

        public async Task<string> GetDocumentiExportAsync(ExportDocumenti_In ExportDocumenti_IN, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            JsonSerializerSettings tzh = new() { DateFormatString = "yyyy-MM-ddT00:00:00Z" };
            List<ExportDocumenti_Out> DocumentiExport = new();

            string datiCompressi = "";

            try
            {
                if (string.IsNullOrEmpty(ExportDocumenti_IN.CUAA))
                {
                    throw new Exception("CUAA Non valorizzato");
                }
                

                DataTable resultDocumenti = await _exportDocumenti.LeggiDocumentiExportAsync(ExportDocumenti_IN.CUAA, ExportDocumenti_IN.Id_Tipologia, ExportDocumenti_IN.DataOra_Rif_Documenti, objParametriServer);
                resultDocumenti = await LeggiAllegatoDaFileSystemAsync(resultDocumenti, objParametriServer, objParametriSuperServer);

                if (resultDocumenti.Rows.Count > 0)
                {

                    List<int> IdDocumentList = resultDocumenti.AsEnumerable().Select(r => r.Field<int>("ID_Elenco")).Distinct().ToList();
                    DataTable resultMD = await _exportDocumentiMetadati.LeggiMetadatiExportAsync(IdDocumentList, objParametriServer);

                    foreach (DataRow row in resultDocumenti.Rows)
                    {
                        bool cancellato = Convert.ToBoolean((int)row["Cancellato"]);

                        if (!cancellato && row["File_Allegato_DB"] != DBNull.Value) {

                            ExportDocumenti_Out Documento = new();

                            Documento.Id_Documento = (int)row["ID_Elenco"];
                            Documento.Id_Tipologia = (int)row["ID_Tipologia"];

                       
                            Documento.Cancellato = cancellato;

                            if (!cancellato)
                            {
                                Documento.Descrizione = row["Descrizione"].ToString();
                                Documento.Data_Scadenza = DateTime.Parse(row["Data_Scadenza"].ToString()!);
                                Documento.FileName = row["Allegati_Documenti_NomeFile"].ToString();
                                Documento.FileByte = (byte[])row["File_Allegato_DB"];

                                List<Metadati> Metadati = new();

                                if (resultMD != null && resultMD.Rows.Count > 0)
                                {

                                    DataTable resultMDCurrent = resultMD.AsEnumerable().Where(r => r.Field<int>("ID_Elenco") == Documento.Id_Documento).CopyToDataTable();


                                    if (resultMDCurrent != null && resultMDCurrent.Rows.Count > 0)
                                    {

                                        foreach (DataRow rowMD in resultMDCurrent.Rows)
                                        {
                                            Metadati Metadato = new();

                                            Metadato.Chiave = rowMD["TitoloIndice"].ToString();
                                            Metadato.Valore = rowMD["Valore_Des"].ToString();

                                            Metadati.Add(Metadato);

                                        }

                                        Documento.Metadati = Metadati;
                                    }
                                }

                                DocumentiExport.Add(Documento);
                            }

                        }
                        
                    }
                    
                }
                datiCompressi = _compressioneDecompressione.CompressioneBase64(1, JsonConvert.SerializeObject(DocumentiExport, tzh));
            }

            catch (Exception ex)
            {
                datiCompressi = "";
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception(ex.Message);
            }

            return datiCompressi;
        }

        public async Task<string> GetDocumentiAnalisiPDCExportAsync(ExportDocumenti_In ExportDocumenti_IN, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            JsonSerializerSettings tzh = new() { DateFormatString = "yyyy-MM-ddT00:00:00Z" };
            List<ExportDocumenti_Out> DocumentiExport = new();

            string datiCompressi = "";

            try
            {
                if (string.IsNullOrEmpty(ExportDocumenti_IN.CUAA))
                {
                    throw new Exception("CUAA Non valorizzato");
                }

                DataTable resultDocumenti = await _exportDocumenti.LeggiDocumentiAnalisiPDCExportAsync(ExportDocumenti_IN.CUAA, ExportDocumenti_IN.DataOra_Rif_Documenti, objParametriServer);
                resultDocumenti = await LeggiAllegatoDaFileSystemAsync(resultDocumenti, objParametriServer, objParametriSuperServer);

                if (resultDocumenti.Rows.Count > 0)
                {

                    foreach (DataRow row in resultDocumenti.Rows)
                    {
                        int idAnalisi = (int)row["ID_Analisi"];
                        int idDocumento = (int)row["ID_Documento"];
                        bool cancellato = Convert.ToBoolean((int)row["Cancellato"]);
                        int statoPubblicazione = (int)row["Stato_Pubblicazione"];
                        int stato = statoPubblicazione;

                        if (cancellato || statoPubblicazione == (int)Enum_PDC_Stato_Pubblicazione.Da_Rimuovere)
                        {
                            ExportDocumenti_Out Documento = new();
                            Documento.Id_Documento = idDocumento;
                            Documento.Id_Tipologia = CostantiPersonalizzate.TIPOLOGIA_ANALISI_PDC;
                            Documento.Cancellato = true;
                            DocumentiExport.Add(Documento);
                            stato = (int)Enum_PDC_Stato_Pubblicazione.Non_Pubblicata;
                        } 
                        else if (!cancellato && row["File_Allegato_DB"] != DBNull.Value)
                        {
                            ExportDocumenti_Out Documento = new();
                            Documento.Id_Documento = idDocumento;
                            Documento.Id_Tipologia = CostantiPersonalizzate.TIPOLOGIA_ANALISI_PDC;
                            Documento.Cancellato = false;
                            Documento.Descrizione = row["Descrizione"].ToString();
                            Documento.Data_Scadenza = DateTime.Parse(row["Data_Scadenza"].ToString()!);
                            Documento.FileName = row["Allegati_Documenti_NomeFile"].ToString();
                            Documento.FileByte = (byte[])row["File_Allegato_DB"];
                            Documento.Metadati = LeggiMetadatiAnalisiPDC(row);
                            DocumentiExport.Add(Documento);
                            stato = (int)Enum_PDC_Stato_Pubblicazione.Pubblicata;
                        }

                        if (stato != statoPubblicazione)
                        {
                            if (!cancellato) await _exportDocumenti.AggiornaPubblicazioneAnalisiPDCAsync(idAnalisi, stato, objParametriServer);
                            await _exportDocumenti.ScriviExportDocumentiAnalisiPDCAsync(ExportDocumenti_IN.CUAA, idAnalisi, idDocumento, stato, objParametriServer);
                        }
                    }
                }

                datiCompressi = _compressioneDecompressione.CompressioneBase64(1, JsonConvert.SerializeObject(DocumentiExport, tzh));
            }

            catch (Exception ex)
            {
                datiCompressi = "";
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception(ex.Message);
            }

            return datiCompressi;
        }

        private List<Metadati> LeggiMetadatiAnalisiPDC(DataRow row)
        {
            List<Metadati> metadati = new();

            metadati.Add(new Metadati
            {
                Chiave = "Codice Analisi",
                Valore = row["Codice_Analisi"].ToString()
            });

            metadati.Add(new Metadati
            {
                Chiave = "Data Fine Analisi",
                Valore = row["Data_Fine_Analisi"].ToString()
            });

            metadati.Add(new Metadati
            {
                Chiave = "Specie",
                Valore = row["Specie"].ToString()
            });

            metadati.Add(new Metadati
            {
                Chiave = "Varietà",
                Valore = row["Varieta"].ToString()
            });

            return metadati;
        }

        public async Task<DataTable> LeggiAllegatoDaFileSystemAsync(DataTable dt, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            string percorsoAllegatyRepository = await GetGestioneAllegatiRepository(objParametriServer, objParametriSuperServer);
            bool PercorsoImpostato = false;

            //Aggiungere controllo sull'esistenza del file
            //se non esiste --> response KO con id_elenco problematico
            //rompiamo tutto oppure no? verificare con scatto

            if (dt.Rows.Count > 0)
            {
                bool bFS = true;

                foreach (DataRow row in dt.Rows)
                {

                    if (!Convert.ToBoolean((int)row["Cancellato"]))
                    {

                        // Controllo Tipo Salvataggio
                        if (row["File_Allegato_DB"] != DBNull.Value)
                        {
                            if (row["File_Allegato_DB"].ToString() == "System.Byte[]")
                            {
                                // Salvataggio su DB
                                bFS = false;
                            }
                        }

                        if (bFS)
                        {
                            if (!string.IsNullOrEmpty(row["Sottocartella"].ToString()) && !PercorsoImpostato)
                            {
                                percorsoAllegatyRepository = Path.Combine(percorsoAllegatyRepository,
                                           row["Sottocartella"].ToString()!);

                                PercorsoImpostato = true;
                            }

                            string fileName = Path.Combine(percorsoAllegatyRepository, row["Allegati_Documenti_NomeFile"].ToString()!);

                            if (File.Exists(fileName)) { 

                                // Lettura file su file system
                                byte[] fileByteArray = File.ReadAllBytes(fileName);

                                // Aggiorna la colonna con il contenuto del file
                                row["File_Allegato_DB"] = fileByteArray;

                            }
                            else
                            {
                                using (LogContext.PushProperty("LogPath", Path.Combine(objParametriServer.LogDirectory, "ExportDocumenti","LogInformation")))
                                {
                                    _logger.LogInformation($"Il seguente file {fileName} non esiste!. id_elenco: {row["ID_Elenco"].ToString()} ");
                                }
                                row["File_Allegato_DB"] = null;
                            }


                        }
                    }

                }
            }

            return dt;
        }

        private async Task<string> GetGestioneAllegatiRepository(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            return await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync("GestioneAllegati_Repository", objParametriServer, objParametriSuperServer);
        }


        public async Task<string> SincroDocumentiAnalisiPDCExportAsync(SincroExportDocumenti_In SincroExportDocumenti_IN, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            string result = "";

            try
            {
                if (string.IsNullOrEmpty(SincroExportDocumenti_IN.CUAA))
                {
                    throw new Exception("CUAA Non valorizzato");
                }

                if (SincroExportDocumenti_IN.Id_Documenti == null || SincroExportDocumenti_IN.Id_Documenti.Count == 0)
                {
                    throw new Exception("Lista documenti non passata");
                }

                string CUAA = SincroExportDocumenti_IN.CUAA;
                List<int> idDocumenti = SincroExportDocumenti_IN.Id_Documenti;                
                int stato = (int)Enum_PDC_Stato_Pubblicazione.Pubblicata;
                if (SincroExportDocumenti_IN.Cancellato) stato = (int)Enum_PDC_Stato_Pubblicazione.Non_Pubblicata;
                await _exportDocumenti.AggiornaExportDocumentiAnalisiPDCAsync(CUAA, idDocumenti, stato, objParametriServer);
                await _exportDocumenti.AggiornaPubblicazioneAnalisiPDCAsync(CUAA, objParametriServer);
                
                result = "OK";
            }

            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }


    }
}
