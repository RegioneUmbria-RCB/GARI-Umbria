using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.Enums;
using AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL;
using AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleWS;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune
{
    public class LetturaDatiTabellaComuneAppFactory : BaseServiceAppBIZ
    {
        private readonly ILoggingService _loggingService;

        public LetturaDatiTabellaComuneAppFactory(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
        }

        public ILetturaTabellaComuneApp Create(TabellaComune tabella) =>
            tabella switch
            {
                // SQL-based
                TabellaComune.Specie =>
                    _serviceProvider.GetRequiredService<SpecieVegetaliApp>(),
                TabellaComune.Varieta => _serviceProvider.GetRequiredService<VarietaApp>(),
                TabellaComune.Finalita => _serviceProvider.GetRequiredService<FinalitaApp>(),
                TabellaComune.Lavorazioni => _serviceProvider.GetRequiredService<OperazioniApp>(),
                TabellaComune.OperazioniCombinazioni =>
                    _serviceProvider.GetRequiredService<OperazioniCombinazioniApp>(),
                //TabellaComune.OperazioniCausali => //non sono dati comuni, tabella di db_Server
                //    _serviceProvider.GetRequiredService<OperazioniCausaliApp>(),
                TabellaComune.Avversita => _serviceProvider.GetRequiredService<AvversitaApp>(),
                TabellaComune.GruppiAvversita =>
                    _serviceProvider.GetRequiredService<GruppiAvversitaApp>(),
                TabellaComune.GruppiAvversitaAttive =>
                    _serviceProvider.GetRequiredService<GruppiAvversitaAttiveApp>(),
                TabellaComune.InfestantiAttive =>
                    _serviceProvider.GetRequiredService<InfestantiAttiveApp>(),
                TabellaComune.AvversitaSpecie =>
                    _serviceProvider.GetRequiredService<AvversitaSpecieApp>(),
                TabellaComune.CategorieUnitaMisura =>
                    _serviceProvider.GetRequiredService<CategorieUnitaMisuraApp>(),
                TabellaComune.TipiMacchine =>
                    _serviceProvider.GetRequiredService<TipiMacchineApp>(),
                TabellaComune.Nazioni => _serviceProvider.GetRequiredService<NazioniApp>(),
                TabellaComune.Regioni => _serviceProvider.GetRequiredService<RegioniApp>(),
                TabellaComune.Province => _serviceProvider.GetRequiredService<ProvinceApp>(),
                TabellaComune.Comuni => _serviceProvider.GetRequiredService<ComuniApp>(),
                TabellaComune.SpecieZootecniche =>
                    _serviceProvider.GetRequiredService<SpecieZootecnicheApp>(),
                TabellaComune.DestinazioneUso =>
                    _serviceProvider.GetRequiredService<DestinazioniUsoApp>(),

                // WS-based
                TabellaComune.MisureAvversita =>
                    _serviceProvider.GetRequiredService<MisureAvversitaApp>(),
                TabellaComune.MisureAvversitaPersonalizzate =>
                    _serviceProvider.GetRequiredService<MisureAvversitaPersonalizzateApp>(),
                TabellaComune.MisureDanni => _serviceProvider.GetRequiredService<MisureDanniApp>(),
                TabellaComune.MisureDanniPersonalizzate =>
                    _serviceProvider.GetRequiredService<MisureDanniPersonalizzateApp>(),
                TabellaComune.IndiciMaturita =>
                    _serviceProvider.GetRequiredService<IndiciMaturitaApp>(),
                TabellaComune.IndiciMaturitaSpecieVegetali =>
                    _serviceProvider.GetRequiredService<IndiciMaturitaSpecieVegetaliApp>(),
                TabellaComune.MisureIndiciMaturita =>
                    _serviceProvider.GetRequiredService<MisureIndiciMaturitaApp>(),
                TabellaComune.IndiciMaturitaPersonalizzate =>
                    _serviceProvider.GetRequiredService<IndiciMaturitaPersonalizzateApp>(),
                TabellaComune.IndiciMaturitaSpecieVegetaliPersonalizzate =>
                    _serviceProvider.GetRequiredService<IndiciMaturitaSpecieVegetaliPersonalizzateApp>(),
                TabellaComune.MisureIndiciMaturitaPersonalizzate =>
                    _serviceProvider.GetRequiredService<MisureIndiciMaturitaPersonalizzateApp>(),
                TabellaComune.SpecieVegetaliStadiCrescita =>
                    _serviceProvider.GetRequiredService<SpecieVegetaliStadiCrescitaApp>(),
                TabellaComune.SpecieVegetaliStadiCrescitaPersonalizzati =>
                    _serviceProvider.GetRequiredService<SpecieVegetaliStadiCrescitaPersonalizzatiApp>(),
                TabellaComune.Disciplinari =>
                    _serviceProvider.GetRequiredService<DisciplinariApp>(),
                TabellaComune.ImpiantiIrrigazioni =>
                    _serviceProvider.GetRequiredService<ImpiantiIrrigazioniApp>(),
                TabellaComune.RapportiContabili =>
                    _serviceProvider.GetRequiredService<RapportiContabiliApp>(),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(tabella),
                    tabella,
                    $"Tabella comune non supportata: {tabella}"
                ),
            };
    }
}

