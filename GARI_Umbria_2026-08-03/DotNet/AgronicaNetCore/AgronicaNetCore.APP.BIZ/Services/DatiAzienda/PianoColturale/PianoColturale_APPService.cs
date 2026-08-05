using System.Data;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.CodiciAnagrafe;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Indirizzi;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.PianoColturale;
using AgronicaNetCore.Anagrafe.DAL.HelpersSTD;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services.ImpostazioniApp;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Gis.DAL.Helpers;
using AgronicaNetCore.Gis.Shared;
using AgronicaNetCore.Gis.Shared.Interfaces;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using static AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda.DatiAziendaEntity;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;
using AppezzamentoEntity = AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda.AppezzamentoEntity;
using DestinazioneUsoEntity = AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DestinazioneUsoEntity;
using EsercizioEntity = AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda.EsercizioEntity;
using GruppoFinalitaEntity = AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni.GruppoFinalitaEntity;
using ImpiantoEntity = AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda.ImpiantoEntity;
using SpecieEntity = AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni.SpecieEntity;
using VarietaEntity = AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni.VarietaEntity;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.PianoColturale
{
    public class PianoColturale_APPService : BaseServiceAppBIZ, IPianoColturale_APPService
    {
        private readonly IPianoColturale_APP _pianoColturale;
        private readonly IUtentiImpostazioni _utentiImpostazioni;
        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly ICodiciAnagrafe _codiciAnagrafe;
        private readonly IIndirizzi _indirizzi;
        private readonly IStaticMapPianoColturaleHelper _staticMapHelper;

        public PianoColturale_APPService(
            IServiceProvider provider,
            IPianoColturale_APP pianoColturale,
            IUtentiImpostazioni utentiImpostazioni,
            ISecurityLayerDAL securityLayerDal,
            ICodiciAnagrafe codiciAnagrafe,
            IIndirizzi indirizzi,
            IStaticMapPianoColturaleHelper staticMapHelper,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _pianoColturale = pianoColturale;
            _utentiImpostazioni = utentiImpostazioni;
            _securityLayerDal = securityLayerDal;
            _codiciAnagrafe = codiciAnagrafe;
            _indirizzi = indirizzi;
            _staticMapHelper = staticMapHelper;
        }

        public async Task<PianoColturaleEntity> LeggiPianoColturaleAsync(
            string piva,
            DateTime data,
            bool leggiCompleto,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            ImpostazioniAppModel? impostazioniApp = null,
            bool modalitaDemetra = false
        )
        {
            var pianoColturaleEntity = new PianoColturaleEntity();
            try
            {
                int sincroAnni = impostazioniApp?.SincroAnni ?? 0;
                if (leggiCompleto && sincroAnni > 0)
                {
                    int baseYear =
                        DateTime.Today.Month < 11 ? DateTime.Today.Year - 1 : DateTime.Today.Year;
                    data = new DateTime(baseYear - sincroAnni, 11, 1);
                }

                var configurazioni = await GetConfigurazioni(
                    objParametriUtenti,
                    objParametriServer
                );

                bool estraiStaticMap =
                    configurazioni.staticMapCfg.GeneraStaticMapDaSincroAPP
                    || configurazioni.staticMapCfg.StaticMapAttive;

                LogDebug(
                    "ReadAsync PianoColturale - piva: {Piva}, data: {Data}, leggiSoloAttivi: {LeggiSoloAttivi}, leggiAncheBloccati: {LeggiAncheBloccati}, estraiStaticMap: {EstraiStaticMap}, modalitaDemetra: {ModalitaDemetra}",
                    objParametriServer,
                    null,
                    piva,
                    data,
                    !leggiCompleto,
                    configurazioni.leggiAncheBloccati,
                    leggiCompleto,
                    modalitaDemetra
                );

                // Lettura dal DAL: il DataTable può essere usato per arricchire i dati dell'impianto
                // (es. cartografia, ageaIdColt, codiceImpianto, StaticMapBase64String) se non presenti nel modello ricco
                DataTable dtImpianti = await _pianoColturale.ReadAsync(
                    piva,
                    data,
                    objParametriServer,
                    leggiSoloAttivi: !leggiCompleto,
                    configurazioni.leggiAncheBloccati,
                    leggiStaticMap: leggiCompleto && estraiStaticMap,
                    modalitaDemetra,
                    leggiCartografia: leggiCompleto
                );

                if (configurazioni.staticMapCfg.GeneraStaticMapDaSincroAPP && leggiCompleto)
                    await _staticMapHelper.GeneraStaticMapDaSincroApp(
                        dtImpianti,
                        configurazioni.staticMapCfg,
                        objParametriServer
                    );

                var pianoColturaleObj = dtImpianti.AsEnumerable();
                if (pianoColturaleObj.Count() == 0)
                    return pianoColturaleEntity;

                DataTable? dtCodiciAppezzamenti = null;
                var codiciAppezzamenti =
                    new Dictionary<(string, int, int), List<CodiciAnagrafeValori>>();

                DataTable? dtCodiciImpianti = null;
                var codiciImpianti =
                    new Dictionary<(string, int, int, int), List<CodiciAnagrafeValori>>();

                DataTable? dtIndirizzi = null;
                var indirizzi = new Dictionary<(string, int, int), List<IndirizzoAssociato>>();

                if (leggiCompleto)
                {
                    dtCodiciAppezzamenti = await _codiciAnagrafe.LeggiCodiciAppezzamentiAsync(
                        new List<(string, int, int)>(
                            (
                                pianoColturaleObj
                                    .Select(x =>
                                        (
                                            x.Field<string>("PIVA"),
                                            x.Field<int>("SA_COD"),
                                            x.Field<int>("APPEZZA")
                                        )
                                    )
                                    .Distinct()
                                    .ToList()
                            )!
                        ),
                        new List<int>(),
                        objParametriServer,
                        escludiCodiciCliente: true
                    );

                    if (dtCodiciAppezzamenti?.Rows != null)
                        foreach (DataRow row in dtCodiciAppezzamenti.Rows)
                        {
                            var key = (
                                row.Field<string>("PIVA"),
                                row.Field<int>("SA_COD"),
                                row.Field<int>("APPEZZA")
                            );
                            if (!codiciAppezzamenti.ContainsKey(key!))
                            {
                                codiciAppezzamenti[key!] = new List<CodiciAnagrafeValori>();
                            }

                            codiciAppezzamenti[key!].Add(CodiciAnagrafeMapper.MapSTDFromRow(row));
                        }

                    dtCodiciImpianti = await _codiciAnagrafe.LeggiCodiciImpiantiAsync(
                        new List<(string, int, int, int)>(
                            (
                                pianoColturaleObj
                                    .Select(x =>
                                        (
                                            x.Field<string>("PIVA"),
                                            x.Field<int>("SA_COD"),
                                            x.Field<int>("APPEZZA"),
                                            x.Field<int>("ID_REG")
                                        )
                                    )
                                    .Distinct()
                                    .ToList()
                            )!
                        ),
                        new List<int>() { (int)Enum_CodiciAnagrafe.CodiceCatalogoAgeaDemetra },
                        objParametriServer,
                        escludiCodiciCliente: false
                    );

                    if (dtCodiciImpianti?.Rows != null)
                    {
                        foreach (DataRow row in dtCodiciImpianti.Rows)
                        {
                            var key = (
                                row.Field<string>("PIVA"),
                                row.Field<int>("SA_COD"),
                                row.Field<int>("APPEZZA"),
                                row.Field<int>("ID_REG")
                            );
                            if (!codiciImpianti.ContainsKey(key!))
                            {
                                codiciImpianti[key!] = new List<CodiciAnagrafeValori>();
                            }

                            codiciImpianti[key!].Add(CodiciAnagrafeMapper.MapSTDFromRow(row));
                        }
                    }

                    dtIndirizzi = await _indirizzi.LeggiIndirizziAppezzamentiAsync(
                        new List<(string, int, int)>(
                            (
                                pianoColturaleObj
                                    .Select(x =>
                                        (
                                            x.Field<string>("PIVA"),
                                            x.Field<int>("SA_COD"),
                                            x.Field<int>("APPEZZA")
                                        )
                                    )
                                    .Distinct()
                                    .ToList()
                            )!
                        ),
                        new List<int>(),
                        indirizzoCompleto: true,
                        objParametriServer
                    );

                    if (dtIndirizzi?.Rows != null)
                        foreach (DataRow row in dtIndirizzi.Rows)
                        {
                            var key = (
                                row.Field<string>("PIVA"),
                                row.Field<int>("SA_COD"),
                                row.Field<int>("APPEZZA")
                            );
                            if (!indirizzi.ContainsKey(key!))
                            {
                                indirizzi[key!] = new List<IndirizzoAssociato>();
                            }

                            indirizzi[key!].Add(IndirizzoMapper.MapSTDFromRow(row, true));
                        }
                }

                //sembra non venga usato lato app, per ora ignoriamo!
                // pianoColturaleEntity.centriAziendali = (
                //     from x in objects
                //     select new CentroAziendaleEntity()
                //     {
                //         codice = x.Field<int>("SA_COD"),
                //         descrizione = x.Field<string>("sa_nome"),
                //         partitaIva = x.Field<string>("PIVA"),
                //     }
                // )
                //     .GroupBy(x => new { x.codice })
                //     .Select(g => g.First())
                //     .ToList();

                pianoColturaleEntity.destinazioniUso = (
                    from x in pianoColturaleObj
                    where x.Field<int>("Veg_Cod") == 0
                    select new DestinazioneUsoEntity()
                    {
                        codice = x.Field<int>("id_Cod"),
                        descrizione = x.Field<string>("Codici_Anagrafe_Des"),
                    }
                )
                    .GroupBy(x => new { x.codice })
                    .Select(g => g.First())
                    .ToList();

                pianoColturaleEntity.specie = (
                    from x in pianoColturaleObj
                    where x.Field<int>("Veg_Cod") != 0
                    select new SpecieEntity()
                    {
                        codice = x.Field<int>("Veg_Cod"),
                        descrizione = x.Field<string>("Veg_Des"),
                    }
                )
                    .GroupBy(x => new { x.codice })
                    .Select(g => g.First())
                    .ToList();

                pianoColturaleEntity.varieta = (
                    from x in pianoColturaleObj
                    where x.Field<int>("Veg_Cod") != 0
                    select new VarietaEntity()
                    {
                        codice = x.Field<int>("CUL_COD"),
                        descrizione = x.Field<string>("Cul_Des"),
                        specieCod = x.Field<int>("Veg_Cod"),
                    }
                )
                    .GroupBy(x => new { x.codice })
                    .Select(g => g.First())
                    .ToList();

                pianoColturaleEntity.finalita = (
                    from x in pianoColturaleObj
                    where x.Field<int>("Veg_Cod") != 0
                    select new GruppoFinalitaEntity()
                    {
                        codice = x.Field<int>("Grfi_Cod"),
                        descrizione = x.Field<string>("Grfi_Des"),
                        specieCod = x.Field<int>("Veg_Cod"),
                    }
                )
                    .GroupBy(x => new { x.codice, x.specieCod })
                    .Select(g => g.First())
                    .ToList();

                pianoColturaleEntity.appezzamenti = (
                    from x in pianoColturaleObj
                    select new AppezzamentoEntity()
                    {
                        codice = x.Field<int>("APPEZZA"),
                        nome = x.Field<string>("APP_NOME"),
                        centroAziendaleCod = x.Field<int>("SA_COD"),
                        campoCod = x.Field<int>("Campo_Cod"),
                        partitaIva = x.Field<string>("PIVA"),
                        codiceAnagrafe = x.Field<string>("Codici_Anagrafe_Appezzamento"),
                        superficie = x.Field<double>("SUP_APP"),
                        inizioValidita = x.Field<DateTime>("Validita_Inizio_Appezza"),
                        fineValidita = x.Field<DateTime>("Validita_Fine_Appezza"),
                        blkFlag = Convert.ToInt32(x.Field<int>("Blk_Flag")) != 0,
                        blkInizioData = x.Field<DateTime>("Blk_Inizio_Data"),
                        blkInizioUsername = x.Field<string>("Blk_Inizio_Username"),
                        blkInizioNote = x.Field<string>("Blk_Inizio_Note"),
                        blkFineData = x.Field<DateTime>("Blk_Fine_Data"),
                        blkFineUsername = x.Field<string>("Blk_Fine_Username"),
                        blkFineNote = x.Field<string>("Blk_Fine_Note"),
                        indirizzi = leggiCompleto
                            ? JsonConvert.SerializeObject(
                                indirizzi!.GetValueOrDefault(
                                    (
                                        x.Field<string>("PIVA"),
                                        x.Field<int>("SA_COD"),
                                        x.Field<int>("APPEZZA")
                                    ),
                                    new List<IndirizzoAssociato>()
                                )
                            )
                            : "",
                        codici = leggiCompleto
                            ? JsonConvert.SerializeObject(
                                codiciAppezzamenti!.GetValueOrDefault(
                                    (
                                        x.Field<string>("PIVA"),
                                        x.Field<int>("SA_COD"),
                                        x.Field<int>("APPEZZA")
                                    ),
                                    new List<CodiciAnagrafeValori>()
                                )
                            )
                            : "",
                    }
                )
                    .GroupBy(x => new { x.codice, x.centroAziendaleCod })
                    .Select(g => g.First())
                    .ToList();

                pianoColturaleEntity.impianti = (
                    from x in pianoColturaleObj
                    select new ImpiantoEntity()
                    {
                        codice = x.Field<int>("ID_REG"),
                        appezzamentoCod = x.Field<int>("APPEZZA"),
                        centroAziendaleCod = x.Field<int>("SA_COD"),
                        partitaIva = x.Field<string>("PIVA"),
                        descrizione = x.Field<string>("Imp_Des"),
                        utilizzoTerrenoClassType =
                            x.Field<int>("Veg_Cod") == 0 ? "DestinazioneUso" : "Varieta",
                        utilizzoTerrenoCod =
                            x.Field<int>("Veg_Cod") == 0
                                ? Convert.ToInt32(x.Field<int>("id_Cod"))
                                : Convert.ToInt32(x.Field<int>("CUL_COD")),
                        gruppoFinalitaCod = x.Field<int>("Grfi_Cod"),
                        superficie = x.Field<double>("Sup_Imp"),
                        inizioValidita = x.Field<DateTime>("Validita_Inizio_Impianto"),
                        fineValidita = x.Field<DateTime>("Validita_Fine_Impianto"),
                        coverCrops = Convert.ToInt32(x.Field<int>("Cover")) != 0,
                        codiceCatalogoAgea = leggiCompleto
                            ? codiciImpianti!
                                .GetValueOrDefault(
                                    (
                                        x.Field<string>("PIVA"),
                                        x.Field<int>("SA_COD"),
                                        x.Field<int>("APPEZZA"),
                                        x.Field<int>("ID_REG")
                                    ),
                                    new List<CodiciAnagrafeValori>()
                                )
                                .FirstOrDefault(c =>
                                    c.codiceAnagrafe.codice
                                    == (int)Enum_CodiciAnagrafe.CodiceCatalogoAgeaDemetra
                                )
                                ?.valore
                                ?? ""
                            : "",
                        StaticMapBase64String =
                            leggiCompleto
                                ? x.Field<string>("StaticMapBase64String")
                                : "",
                        cartografia =
                            leggiCompleto ? x.Field<string>("Cartografia") : "",
                        ageaIdColt = leggiCompleto ? x.Field<string>("Agea_idColt") : "",
                        codiceImpianto = leggiCompleto
                            ? x.Field<string>("Codici_Anagrafe_Impianto")
                            : "",
                    }
                )
                    .GroupBy(x => new
                    {
                        x.codice,
                        x.appezzamentoCod,
                        x.centroAziendaleCod,
                    })
                    .Select(g => g.First())
                    .ToList();

                pianoColturaleEntity.esercizi = (
                    from x in pianoColturaleObj
                    select new EsercizioEntity()
                    {
                        codice = x.Field<int>("Progetto_Cod"),
                        impiantoCod = x.Field<int>("ID_REG"),
                        appezzamentoCod = x.Field<int>("APPEZZA"),
                        centroAziendaleCod = x.Field<int>("SA_COD"),
                        partitaIva = x.Field<string>("PIVA"),
                        descrizione = x.Field<string>("Progetto"),
                        inizioValidita = x.Field<DateTime>("Validita_Inizio_Distinta"),
                        fineValidita = x.Field<DateTime>("Validita_Fine_Distinta"),
                        resaPrevista = leggiCompleto ? x.Field<double>("produzione_prevista") : 0,
                    }
                )
                    .GroupBy(x => new { x.codice })
                    .Select(g => g.First())
                    .ToList();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return pianoColturaleEntity;
        }

        private async Task<(
            bool leggiAncheBloccati,
            GeneraMappaStaticaInData staticMapCfg
        )> GetConfigurazioni(
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            DataTable? dtImpostazioniUtente = await _utentiImpostazioni.ReadAsync(
                (int)
                    Enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI,
                (int)ModalitaLeggiImpostazioniUtente.SuperUser,
                objParametriUtenti,
                objParametriServer
            );
            bool leggiAncheBloccati = false;
            if (
                dtImpostazioniUtente != null
                && dtImpostazioniUtente.Rows.Count > 0
                && dtImpostazioniUtente.Rows[0][0] != DBNull.Value
            )
            {
                leggiAncheBloccati = Convert.ToInt32(dtImpostazioniUtente.Rows[0][2]) != 0;
            }

            GeneraMappaStaticaInData staticMapCfg = new GeneraMappaStaticaInData();
            var dtConfigSiti = await _securityLayerDal.LeggiConfigurazioneSitiAsync(
                "GIS_StaticMapCFG",
                objParametriServer
            );
            if (dtConfigSiti.Rows.Count > 0 && dtConfigSiti.Rows[0]["Valore"] != DBNull.Value)
            {
                var jSonStaticMapCfg = dtConfigSiti.Rows[0]["Valore"].ToString();
                if (!string.IsNullOrEmpty(jSonStaticMapCfg))
                {
                    staticMapCfg =
                        JsonConvert.DeserializeObject<GeneraMappaStaticaInData>(jSonStaticMapCfg)
                        ?? new GeneraMappaStaticaInData();
                }
            }

            return (leggiAncheBloccati, staticMapCfg);
        }
    }
}
