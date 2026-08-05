using System.Data;
using System.Linq;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Contatti;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Contatti
{
    public class Contatti_APPService : BaseServiceAppBIZ, IContatti_APPService
    {
        private readonly IContatti_APP _contattiApp;

        public Contatti_APPService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _contattiApp = _serviceProvider.GetRequiredService<IContatti_APP>();
        }

        public async Task<Contatti_APPResult> LeggiAsync(
            Contatti_APPRequest request,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            request ??= new Contatti_APPRequest();
            var piva = request.Piva;
            var allRows = await ReadRowsAsync(piva, request, objParametriServer);
            var result = new Contatti_APPResult();

            if (request.IncludeContatti)
            {
                var contattiRows = ApplyContattiFilters(allRows, piva, objParametriServer, request.ModalitaDemetra).ToList();
                result.Contatti = MapContatti(contattiRows);
                result.RisorseUmane = MapRisorseUmane(contattiRows);

                var lavoratoriMovimentatiRows = await ReadLavoratoriMovimentatiAsync(
                    piva,
                    objParametriServer
                );

                var nuoviLavoratori = MapContattiFornitoriMovimentati(lavoratoriMovimentatiRows);
                var existingLavoratoriKeys = result
                    .Contatti.Select(c => (c.codice, c.partitaIva))
                    .ToHashSet();
                result.Contatti.AddRange(
                    nuoviLavoratori.Where(c =>
                        !existingLavoratoriKeys.Contains((c.codice, c.partitaIva))
                    )
                );

                var nuoveRisorseLavoratori = MapRisorseUmane(lavoratoriMovimentatiRows);
                var existingRisorseLavoratoriKeys = result
                    .RisorseUmane.Select(r => (r.codice, r.rapportoContabileCod))
                    .ToHashSet();
                result.RisorseUmane.AddRange(
                    nuoveRisorseLavoratori.Where(r =>
                        !existingRisorseLavoratoriKeys.Contains((r.codice, r.rapportoContabileCod))
                    )
                );
            }

            if (request.IncludeFornitoriMeteo)
            {
                var fornitoriMeteoRows = ApplyFornitoriMeteoFilters(allRows, piva).ToList();
                result.FornitoriMeteo.AddRange(MapContatti(fornitoriMeteoRows));
                result.RisorseFornitoriMeteo.AddRange(MapRisorseUmane(fornitoriMeteoRows));
            }

            if (request.IncludeFornitori)
            {
                var fornitoriNormaliRows = ApplyFornitoriNormaliFilters(
                        allRows,
                        piva,
                        objParametriServer,
                        request.ModalitaDemetra
                    )
                    .ToList();
                result.Fornitori.AddRange(MapContatti(fornitoriNormaliRows));
                result.RisorseFornitori.AddRange(MapRisorseUmane(fornitoriNormaliRows));

                var fornitoriMovimentatiRows = await ReadFornitoriMovimentatiAsync(
                    piva,
                    objParametriServer
                );

                var nuoviContatti = MapContattiFornitoriMovimentati(fornitoriMovimentatiRows);
                var existingContattiKeys = result
                    .Fornitori.Select(c => (c.codice, c.partitaIva))
                    .ToHashSet();
                result.Fornitori.AddRange(
                    nuoviContatti.Where(c =>
                        !existingContattiKeys.Contains((c.codice, c.partitaIva))
                    )
                );

                var nuoveRisorse = MapRisorseUmane(fornitoriMovimentatiRows);
                var existingRisorseKeys = result
                    .RisorseFornitori.Select(r => (r.codice, r.rapportoContabileCod))
                    .ToHashSet();
                result.RisorseFornitori.AddRange(
                    nuoveRisorse.Where(r =>
                        !existingRisorseKeys.Contains((r.codice, r.rapportoContabileCod))
                    )
                );
            }

            return result;
        }

        private async Task<List<DataRow>> ReadRowsAsync(
            string? piva,
            Contatti_APPRequest request,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            try
            {
                var dt = await _contattiApp.ReadAsync(
                    piva,
                    request.IncludeContatti,
                    request.IncludeFornitoriMeteo,
                    request.IncludeFornitori,
                    objParametriServer
                );
                return dt.AsEnumerable().ToList();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        private async Task<List<DataRow>> ReadFornitoriMovimentatiAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            try
            {
                var dt = await _contattiApp.ContattiFornitoriMovimentati_APPAsync(
                    piva,
                    objParametriServer
                );
                return dt.AsEnumerable().ToList();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        private async Task<List<DataRow>> ReadLavoratoriMovimentatiAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            try
            {
                var dt = await _contattiApp.ContattiLavoratoriMovimentati_APPAsync(
                    piva,
                    objParametriServer
                );
                return dt.AsEnumerable().ToList();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        private static List<ContattoEntity> MapContattiFornitoriMovimentati(
            IEnumerable<DataRow> rows
        )
        {
            var result = new List<ContattoEntity>();

            foreach (var row in rows)
            {
                result.Add(
                    new ContattoEntity
                    {
                        codice = row.Field<string>("Cod_Contatto") ?? string.Empty,
                        partitaIva = row.Field<string>("Piva") ?? string.Empty,
                        centroAziendaleCod = row.Field<int?>("Sa_Cod") ?? 0,
                        codiceFiscale = row.Field<string>("Cod_Contatto") ?? string.Empty,
                        nome = row.Field<string>("Nome") ?? string.Empty,
                        cognome = row.Field<string>("Cognome") ?? string.Empty,
                        nomeBreve = BuildNomeBreve(row),
                        ragioneSociale = row.Field<string>("Rag_Soc") ?? string.Empty,
                        isPublic = row.Field<int?>("Sa_Cod") == -1,
                        badge = row.Field<string>("NrBadge") ?? string.Empty,
                        validitaFrom = row.Field<DateTime?>("Validita_Inizio") ?? DateTime.MinValue,
                        validitaTo = row.Field<DateTime?>("Validita_Fine") ?? DateTime.MinValue,
                        dataNascita = row.Field<DateTime?>("Data_Nascita"),
                        sesso = row.Field<string>("Sesso") ?? string.Empty,
                    }
                );
            }

            return result;
        }

        private static List<ContattoEntity> MapContatti(IEnumerable<DataRow> rows)
        {
            var result = new List<ContattoEntity>();
            var seen = new HashSet<(string, string)>();

            foreach (var row in rows)
            {
                var entity = new ContattoEntity
                {
                    codice = row.Field<string>("Cod_Contatto") ?? string.Empty,
                    partitaIva = row.Field<string>("Piva") ?? string.Empty,
                    centroAziendaleCod = row.Field<int?>("Sa_Cod") ?? 0,
                    codiceFiscale = row.Field<string>("Cod_Contatto") ?? string.Empty,
                    nome = row.Field<string>("Nome") ?? string.Empty,
                    cognome = row.Field<string>("Cognome") ?? string.Empty,
                    nomeBreve = BuildNomeBreve(row),
                    ragioneSociale = row.Field<string>("rag_soc") ?? string.Empty,
                    isPublic = row.Field<int?>("Sa_Cod") == -1,
                    badge = row.Field<string>("NrBadge") ?? string.Empty,
                    validitaFrom = row.Field<DateTime?>("Validita_Inizio") ?? DateTime.MinValue,
                    validitaTo = row.Field<DateTime?>("Validita_Fine") ?? DateTime.MinValue,
                    dataScadenzaPatentino = row.Field<DateTime?>("Data_Scadenza_Patentino"),
                    dataRilascioPatentino = row.Field<DateTime?>("Data_Rilascio_Patentino"),
                    nrPatentino = row.Field<string>("nrPatentino") ?? string.Empty,
                    dataNascita = row.Field<DateTime?>("Data_Nascita"),
                    sesso = row.Field<string>("Sesso") ?? string.Empty,
                };

                if (seen.Add((entity.codice, entity.partitaIva)))
                    result.Add(entity);
            }

            return result;
        }

        private static List<RisorseUmaneEntity> MapRisorseUmane(IEnumerable<DataRow> rows)
        {
            var result = new List<RisorseUmaneEntity>();
            var seen = new HashSet<(int, string)>();

            foreach (var row in rows)
            {
                var item = new RisorseUmaneEntity
                {
                    codice = row.Field<int?>("Cod_RisUm") ?? 0,
                    validitaFrom = row.Field<DateTime?>("Validita_Inizio") ?? DateTime.MinValue,
                    validitaTo = row.Field<DateTime?>("Validita_Fine") ?? DateTime.MinValue,
                    settore = string.Empty,
                    attivita = string.Empty,
                    rapportoContabileCod = row.Field<int?>("Cod_Rapporto") ?? 0,
                    codiceContatto = row.Field<string>("Cod_Contatto") ?? string.Empty,
                    partitaIvaContatto = row.Field<string>("Piva") ?? string.Empty,
                };

                if (item.codice == 0)
                    continue;

                if (seen.Add((item.codice, item.partitaIvaContatto)))
                    result.Add(item);
            }

            return result;
        }

        private static IEnumerable<DataRow> ApplyContattiFilters(
            IEnumerable<DataRow> rows,
            string? piva,
            AgronicaCoreParametriServer objParametriServer,
            bool modalitaDemetra
        )
        {
            return rows.Where(row =>
                StringEquals(
                    GetString(row, "RapportoContabile_Piva"),
                    objParametriServer.PivaSuperUser?.Trim()
                )
                && IsContattiRapportoAmmesso(row)
                && MatchesContattiVisibility(row, piva, modalitaDemetra)
            );
        }

        private static IEnumerable<DataRow> ApplyFornitoriMeteoFilters(
            IEnumerable<DataRow> rows,
            string? piva
        )
        {
            return rows.Where(row =>
                GetInt(row, "RapportoContabile_Cod_Rapporto")
                    == (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Fornitore
                && GetInt(row, "Sa_Cod") != -1
            );
        }

        private static IEnumerable<DataRow> ApplyFornitoriNormaliFilters(
            IEnumerable<DataRow> rows,
            string? piva,
            AgronicaCoreParametriServer objParametriServer,
            bool modalitaDemetra
        )
        {
            return rows.Where(row =>
                StringEquals(
                    GetString(row, "RapportoContabile_Piva"),
                    objParametriServer.PivaSuperUser?.Trim()
                )
                && IsFornitoreNormaleRapportoAmmesso(row)
                && MatchesContattiVisibility(row, piva, modalitaDemetra)
            );
        }

        private static bool IsContattiRapportoAmmesso(DataRow row)
        {
            var rapporto = GetInt(row, "RapportoContabile_Cod_Rapporto");
            return rapporto
                    == (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Legale_Rappresentante
                || rapporto == (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Dipendente
                || rapporto == (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Tecnico
                || rapporto == (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Terzista
                || GetInt(row, "RapportoContabile_Dipendente") == 1
                || GetInt(row, "RapportoContabile_Legale") == 1
                || GetInt(row, "RapportoContabile_Terzista") == 1;
        }

        private static bool IsFornitoreNormaleRapportoAmmesso(DataRow row)
        {
            var rapporto = GetInt(row, "RapportoContabile_Cod_Rapporto");
            return rapporto == (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Vivaio
                || rapporto
                    == (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Fornitore_Agrofarmaci
                || rapporto
                    == (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Fornitore
                || rapporto
                    == (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Conferente;
        }

        private static bool MatchesContattiVisibility(DataRow row, string? piva, bool modalitaDemetra)
        {
            var partitaIva = GetString(row, "Piva");
            var saCod = GetInt(row, "Sa_Cod");

            if (saCod == -1)
                return !modalitaDemetra;

            return StringEquals(partitaIva, piva);
        }

        private static int GetInt(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) ? row.Field<int?>(columnName) ?? 0 : 0;
        }

        private static string GetString(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName)
                ? row.Field<string>(columnName) ?? string.Empty
                : string.Empty;
        }

        private static bool StringEquals(string left, string? right)
        {
            return string.Equals(left, right ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildNomeBreve(DataRow row)
        {
            var nome = row.Field<string>("Nome") ?? string.Empty;
            var cognome = row.Field<string>("Cognome") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cognome))
            {
                return row.Field<string>("rag_soc") ?? string.Empty;
            }

            return $"{nome} {cognome}";
        }
    }
}
