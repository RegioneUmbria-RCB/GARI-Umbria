using System.Data;
using System.Linq;
using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.CodiciAnagrafe;
using AgronicaNetCore.Anagrafe.DAL.HelpersSTD;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.CodiciAnagrafe
{
    public class CodiciAnagrafeService : BaseServiceAnagrafeBIZ, ICodiciAnagrafeService
    {
        private readonly ICodiciAnagrafe _codiciAnagrafeDal;

        public CodiciAnagrafeService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _codiciAnagrafeDal = _serviceProvider.GetRequiredService<ICodiciAnagrafe>();
        }

        public async Task<List<CodiceAnagrafeBase>> LeggiCodiciUsatixEntitaAsync(
            LeggiCodiciUsatixEntitaAnagrafe_IN LeggiCodiciUsatixEntitaAnagrafe,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            List<CodiceAnagrafeBase> codiciAnagrafe;
            try
            {
                codiciAnagrafe = await _codiciAnagrafeDal.LeggiCodiciUsatixEntitaUsatixEntitaAsync(
                    LeggiCodiciUsatixEntitaAnagrafe.entita,
                    LeggiCodiciUsatixEntitaAnagrafe.idBudget,
                    objParametriServer
                );
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return codiciAnagrafe;
        }

        public async Task<DataTable> LeggiCodiciUsatixEntitaDestinazioniDUsoAsync(
            AgronicaCoreParametriServer objParametriServer
        )
        {
            DataTable dt;
            try
            {
                dt = await _codiciAnagrafeDal.LeggiCodiciUsatixEntitaDestinazioniDUsoAsync(
                    objParametriServer
                );
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return dt;
        }

        public async Task<List<CodiciAnagrafeValori>> LeggiTuttiCodiciAnagrafexEntita(
            LeggiCodiciAnagrafe parametri,
            List<int> idCods,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            DataTable dt;

            switch (parametri.ElementoAnagrafico)
            {
                case EntitaAlberoImprese.Impresa:
                    dt = await _codiciAnagrafeDal.LeggiCodiciImpresaAsync(
                        parametri.ChiaviImprese.Select(k => k.partitaIva).ToList(),
                        idCods,
                        objParametriServer,
                        escludiCodiciCliente: parametri.EscludiCodiciCliente
                    );
                    break;
                case EntitaAlberoImprese.Centro:
                    dt = await _codiciAnagrafeDal.LeggiCodiciCentroAsync(
                        parametri
                            .ChiaviCentriAziendali.Select(k => (k.partitaIva, k.codice))
                            .ToList(),
                        idCods,
                        objParametriServer,
                        escludiCodiciCliente: parametri.EscludiCodiciCliente
                    );
                    break;
                case EntitaAlberoImprese.Campo:
                    // TODO: chiamare DAL Campi_Codici
                    dt = new DataTable();
                    break;
                case EntitaAlberoImprese.Appezzamento:
                    dt = await _codiciAnagrafeDal.LeggiCodiciAppezzamentiAsync(
                        parametri
                            .ChiaviAppezzamenti.Select(k =>
                                (
                                    k.centroAziendalePK.partitaIva,
                                    k.centroAziendalePK.codice,
                                    k.codice
                                )
                            )
                            .ToList(),
                        idCods,
                        objParametriServer,
                        escludiCodiciCliente: parametri.EscludiCodiciCliente
                    );
                    break;
                case EntitaAlberoImprese.Impianto:
                    dt = await _codiciAnagrafeDal.LeggiCodiciImpiantiAsync(
                        parametri
                            .ChiaviImpianti.Select(k =>
                                (
                                    k.appezzamentoPK.centroAziendalePK.partitaIva,
                                    k.appezzamentoPK.centroAziendalePK.codice,
                                    k.appezzamentoPK.codice,
                                    k.codice
                                )
                            )
                            .ToList(),
                        idCods,
                        objParametriServer,
                        escludiCodiciCliente: parametri.EscludiCodiciCliente
                    );
                    break;
                case EntitaAlberoImprese.Fabbricato:
                    dt = await _codiciAnagrafeDal.LeggiCodiciFabbricatiAsync(
                        parametri
                            .ChiaviFabbricati.Select(k =>
                                (
                                    k.centroAziendalePK.partitaIva,
                                    k.centroAziendalePK.codice,
                                    k.codice
                                )
                            )
                            .ToList(),
                        idCods,
                        objParametriServer,
                        escludiCodiciCliente: parametri.EscludiCodiciCliente
                    );
                    break;
                case EntitaAlberoImprese.Distinta:
                    dt = await _codiciAnagrafeDal.LeggiCodiciEserciziAsync(
                        parametri
                            .ChiaviEsercizi.Select(k =>
                                (
                                    k.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                    k.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                    k.impiantoPK.appezzamentoPK.codice,
                                    k.impiantoPK.codice,
                                    k.codice
                                )
                            )
                            .ToList(),
                        idCods,
                        objParametriServer,
                        escludiCodiciCliente: parametri.EscludiCodiciCliente
                    );
                    break;
                case EntitaAlberoImprese.Contatto:
                    dt = await _codiciAnagrafeDal.LeggiCodiciContattiAsync(
                        parametri
                            .ChiaviContatti.Select(k =>
                                (
                                    k.partitaIva,
                                    int.TryParse(k.codice, out var codContatto) ? codContatto : 0
                                )
                            )
                            .ToList(),
                        idCods,
                        objParametriServer,
                        escludiCodiciCliente: parametri.EscludiCodiciCliente
                    );
                    break;
                default:
                    throw new NotSupportedException(
                        $"Lettura CodiciAnagrafeValori su elemento anagrafico {parametri.ElementoAnagrafico} non gestito"
                    );
            }

            return CodiciAnagrafeMapper.MapSTDFromDataTable(dt);
        }
    }
}
