using AgronicaNetCore.Base.Base;
using DocumentFormat.OpenXml.VariantTypes;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using AgronicaNetCore.Base.Models;
using OutData.Varie;
using AgronicaNetCore.Base.DataLayer.Security;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.Utility.BIZ.Services
{
    public class EsportaAllegatoService : BaseService, IEsportaAllegatoService
    {
        private readonly ISecurityLayerDAL _securityLayer;

        public EsportaAllegatoService(IServiceProvider provider) : base(provider)
        {
            _securityLayer = _serviceProvider.GetRequiredService<ISecurityLayerDAL>();
        }

        public async Task<string?> EsportaExcelPath(DataTable data, string nomefile, 
            AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            return await EsportaExcelPath(data, nomefile, false, objParametriServer, objParametriSuperServer); 
        }

        public async Task<string?> EsportaExcelPath(DataTable data, string nomefile, bool applyFormating, 
            AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            try
            {
                string? pathFolder = await _securityLayer.LeggiConfigurazioneSitiScalareAsync("PathFileTemporanei", objParametriServer, objParametriSuperServer);

                if (string.IsNullOrEmpty(pathFolder))
                    throw new Exception("PathFileTemporanei in configurazione siti non impostato");

                if (!Directory.Exists(pathFolder))
                    Directory.CreateDirectory(pathFolder);
                var folder = new DirectoryInfo(pathFolder);

                if (string.IsNullOrEmpty(nomefile))
                    nomefile = "Esporta_Excel";
                nomefile = $"{nomefile}.xlsx";

                var pathFile = Path.Combine(folder.FullName, nomefile);
                if (File.Exists(pathFile))
                    File.Delete(pathFile);

                if (data != null)
                {
                    if (string.IsNullOrEmpty(data.TableName))
                        data.TableName = "Export";

                    var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add(data);

                    if (applyFormating)
                    {
                        foreach (DataColumn col in data.Columns)
                        {
                            string style = "@";
                            string exportFormat = col.ExtendedProperties["ExportFormat"]!.ToString()!.ToLower();

                            switch (exportFormat)
                            {
                                case "number":
                                    style = "0.00";
                                    break;
                                case "date":
                                    style = "dd/mm/yyyy";
                                    break;
                                default:
                                    style = "@";
                                    break;
                            }

                            worksheet.Column(col.ColumnName).Style.NumberFormat.SetFormat(style);
                        }
                    }

                    workbook.SaveAs(pathFile);

                    return await Task.FromResult(pathFile);
                }
                else
                    throw new Exception("Dati non presenti, impossibile creare il file Excel");
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<objAllegato?> ImpostaObjAllegato(string pathFile, AgronicaCoreParametri objParametriServer)
        {
            try
            {
                var objAllegato = new objAllegato()
                {
                    NomeFile = Path.GetFileNameWithoutExtension(pathFile),
                    Estensione = Path.GetExtension(pathFile),
                    File = File.ReadAllBytes(pathFile)
                };

                File.Delete(pathFile);
                return await Task.FromResult(objAllegato);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
