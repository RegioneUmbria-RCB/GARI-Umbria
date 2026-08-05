using System.Text.Json;
using AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Services.Security;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    public class OrogelMultiAnnoService : BaseServiceAnagrafeBIZ, IOrogelMultiAnnoService
    {
        private const string ConfigKey = "OrogelBI_SwitchArchivioDinamico";
        private readonly ISecurityService _securityService;
        private readonly ISecurityLayerDAL _securityLayerDAL;

        public OrogelMultiAnnoService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer,
            ISecurityService securityService,
            ISecurityLayerDAL securityLayerDAL
        )
            : base(provider, localizer)
        {
            _securityService = securityService;
            _securityLayerDAL = securityLayerDAL;
        }

        public async Task<OrogelRoutingResult> ValidaEInstradaArchivioAsync(
            int anno,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            if (anno < 1000 || anno > 9999)
                throw new InvalidYearFormatException(anno);

            int currentYear = DateTime.Now.Year;
            if (anno < 2000 || anno > currentYear)
                throw new YearOutOfRangeException(anno, currentYear);

            AgronicaCoreParametriSuperServer objParametriSuperServer =
                _securityService.GetAgronicaCoreParametri();

            System.Data.DataTable configTable;
            try
            {
                configTable = await _securityLayerDAL.LeggiConfigurazioneSitiAsync(
                    ConfigKey,
                    objParametriSuperServer
                );
            }
            catch (Exception ex)
            {
                throw new ConfigurationLoadFailedException(
                    $"Impossibile caricare la configurazione '{ConfigKey}'.",
                    ex
                );
            }

            if (configTable == null || configTable.Rows.Count == 0)
                throw new ConfigurationLoadFailedException(
                    $"La configurazione '{ConfigKey}' non ha restituito risultati."
                );

            Dictionary<string, int> mapping;
            try
            {
                string jsonValore = configTable.Rows[0]["Valore"].ToString()!;
                mapping =
                    JsonSerializer.Deserialize<Dictionary<string, int>>(jsonValore)
                    ?? throw new ConfigurationLoadFailedException(
                        $"La deserializzazione della configurazione '{ConfigKey}' ha restituito null."
                    );
            }
            catch (JsonException ex)
            {
                throw new ConfigurationLoadFailedException(
                    $"Formato JSON non valido nella configurazione '{ConfigKey}'.",
                    ex
                );
            }

            string annoKey = anno.ToString();
            if (!mapping.ContainsKey(annoKey))
                throw new ArchiveNotAvailableException(anno, mapping.Keys);

            int idDbServer = mapping[annoKey];

            AgronicaCoreParametriTriple? objParametriTriple =
                _securityService.GetAgronicaCoreParametriTriple(idDbServer);
            if (objParametriTriple == null)
                throw new DatabaseParametersNotFoundException(idDbServer);

            return new OrogelRoutingResult(anno, idDbServer, objParametriTriple);
        }
    }
}
