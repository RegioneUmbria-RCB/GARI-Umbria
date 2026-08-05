using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Resources;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.WidgetNutrizione.Models;
using AgronicaNetCore.Operazione.DAL.DataLayer.RilievoFasiFenologiche;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.FasiFenologicheRegistrate
{
    /// <summary>
    /// Implementazione BIZ per l'estrazione cronologica delle fasi fenologiche registrate
    /// su un appezzamento nell'intervallo di validità indicato.
    /// Restituisce una lista di <see cref="FaseFenologicaCorrenteDto"/> con codice BBCH,
    /// descrizione e data di ogni fase.
    /// </summary>
    /// <remarks>
    /// Design Specification: DS20-BL GetPhenologicalPhasesChronological — Dettaglio Procedurale.
    /// DS21-API POST /api/fasi-fenologiche-registrate — Processing Logic.
    /// </remarks>
    public sealed class GetPhenologicalPhasesChronologicalService : BaseDSSNutrizioneBIZService, IGetPhenologicalPhasesChronologicalService
    {
        private readonly IRilievoFasiFenologiche _rilievoFasiFenologicheDAL;

        public GetPhenologicalPhasesChronologicalService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _rilievoFasiFenologicheDAL = provider.GetRequiredService<IRilievoFasiFenologiche>();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<FaseFenologicaCorrenteDto>> EseguiAsync(
            FasiFenologicheRequest request,
            AgronicaCoreParametriServer objParametriServer,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            if (objParametriServer is null)
                throw new ArgumentNullException(nameof(objParametriServer));

            var appezzamento = request.Appezzamento;

            // Design Specification: DS20-BL — Query Movimenti Fenologici; Ordinamento e Deduplica.
            DataTable dt = await _rilievoFasiFenologicheDAL.LeggiAsync(
                appezzamento.Piva,
                appezzamento.SaCod,
                appezzamento.Appezza,
                appezzamento.IdReg,
                request.ValiditaInizio,
                request.ValiditaFine,
                objParametriServer);

            // Design Specification: DS20-BL — Composizione Output.
            // DS21-API — Se totale_fasi === 0, restituire lista vuota (non è errore).
            var lista = new List<FaseFenologicaCorrenteDto>(dt.Rows.Count);

            foreach (DataRow r in dt.Rows)
            {
                var idAgenda = r.Field<int>("Id_Agenda");
                var idMov = r.Field<int>("Id_Mov");
                var idMovDet = r.Field<int>("Id_Mov_Det");
                var codBbch         = r.Field<string>("CodBbch") ?? string.Empty;
                var descrizioneBbch = r.Field<string>("DescrizioneBbch") ?? string.Empty;
                var dataFase   = r.Field<DateTime>("Validita_Inizio");

                lista.Add(new FaseFenologicaCorrenteDto
                {
                    IdAgenda       = idAgenda,
                    IdMov          = idMov,
                    IdMovDet       = idMovDet,
                    BbchCod         = codBbch,
                    BbchDescrizione = descrizioneBbch,
                    DataFase        = dataFase.ToString("yyyy-MM-dd")
                });
            }

            return lista;
        }
    }
}
