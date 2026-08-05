using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.ImpiantoFasiFenologiche;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.FasiFenologiche.Persistenza
{
    /// <summary>
    /// Implementazione del servizio di persistenza delle fasi fenologiche.
    /// Applica le regole GIAS su colonne standard, bulk insert e atomicità transazionale.
    /// </summary>
    /// <remarks>
    /// Design Specification: PersistenzaFasiFenologiche - Regole di Business.
    /// </remarks>
    public sealed class PersistenzaFasiFenologicheService: BaseService, IPersistenzaFasiFenologicheService
    {
        private const int SogliaBulkInsert = 50;

        private readonly IImpiantoFasiFenologiche _impiantoFasiFenologiche;

        public PersistenzaFasiFenologicheService(IServiceProvider provider) : base(provider)
        {
            _impiantoFasiFenologiche = provider.GetRequiredService<IImpiantoFasiFenologiche>();
        }

        /// <inheritdoc />
        public async Task<bool> SalvaAsync(
            PersistenzaFasiFenologicheInput input,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));

            // Persistenza atomica: bulk o singola, all'interno di una transazione gestita dal DAL
            try
            {
                if (input.FasiFenologiche.Count > SogliaBulkInsert)
                {
                    try
                    {
                        await _impiantoFasiFenologiche.EseguiBulkInsertAsync(
                                                            input.Impianto,
                                                            input.FasiFenologiche,
                                                            input.MetadataAcquisizione,
                                                            objParametriServer);
                    }
                    catch (Exception ex) when (ex is not BulkInsertException)
                    {
                        throw new BulkInsertException(ex.Message, ex);
                    }
                }
                else
                {
                    await _impiantoFasiFenologiche.InserisciFasiAsync(
                                                                        input.Impianto,
                                                                        input.FasiFenologiche,
                                                                        input.MetadataAcquisizione,
                                                                        objParametriServer);
                }
            }
            catch (BulkInsertException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TransactionRollbackException(ex.Message, ex);
            }

            return true;
        }

    }
}
