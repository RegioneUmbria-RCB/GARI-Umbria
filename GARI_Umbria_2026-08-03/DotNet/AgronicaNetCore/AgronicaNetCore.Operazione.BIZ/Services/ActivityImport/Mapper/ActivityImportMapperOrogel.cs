using AgronicaCoreDTOStd.InData.ActivityImport;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaDataProvider6.Extensions;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Operazione.BIZ.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Operazione.BIZ.Services.ActivityImport.Mapper
{
    public class ActivityImportMapperOrogel<T> : ActivityImportMapperTemplate<T> where T : IActivityImportData
    {
        public ActivityImportMapperOrogel(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) {}

        protected override void SetOrigin()
        {
            _codiceAnagrafeCliente = (int)Enum_Anagrafe_CodiciCliente.Orogel;
            _origin = Enum_Origine.ConferimentoWMS;
        }

        protected override async Task SetActivityBaseData(int raccoglitore)
        {
            if (base._base.tipo_operazione != LAV_COD.LAVCOD_RACCOLTA)
            {
                throw new NotImplementedException(base._localizer.GetString("AttivitaNonGestita", base._base.tipo_operazione).Value);
            }
            await base.SetActivityBaseData(raccoglitore);
            _activity.descrizione = "Raccolta per Conferimento";
        }

        protected override async Task<Impianto.PK> CheckPlantValid(IActivityImportPlant plant, string piva)
        {
            string[] keys = plant.plot_id.Split('_');
            Impianto.PK key = new Impianto.PK(
                int.Parse(keys[3].ToString() ?? "0"),
                int.Parse(keys[2].ToString() ?? "0"),
                int.Parse(keys[1].ToString() ?? "0"),
                keys[0].ToString() ?? ""
            );
            int progettoCod = int.Parse(keys[4].ToString() ?? "-1");

            DataTable? results = await _impianti.LeggiAsync(_server, key.piva, key.saCod, key.appezza, key.codice, progettoCod);
            if (results == null || results.Rows.Count == 0)
                throw new InvalidOperationException(_localizer.GetString("ImpiantoNonEsistente", plant.plot_id).Value);
            else if (results.Rows.Count > 1)
                throw new DuplicateNameException(_localizer.GetString("ImpiantoNonUnivoco", plant.plot_id).Value);

            DataRow plotRow = results.Rows[0];
            DateTime inizio = DateTime.Parse(plotRow["Validita_Inizio"].ToString() ?? CostantiPersonalizzate.AGRODATAINIZIO);
            DateTime fine = DateTime.Parse(plotRow["Validita_Fine"].ToString() ?? CostantiPersonalizzate.AGRODATAFINE);

            if (!_activity.inizio.IsInRange(inizio, fine))
            {
                throw new ArgumentOutOfRangeException(_localizer.GetString(
                    "ImpiantoNonValidoInData", plant.plot_id, key.toString("-"), _activity.inizio.ToString("dd/MM/yyyy")
                ).Value);
            }

            return key;
        }

        // IActivityImportData does not provide a usable .raccolti field
        protected override void SetHarvests() { }

        // IActivityImportData does not provide a usable .irrigazioni field
        protected override void SetIrrigations() { }

        // IActivityImportData does not provide a usable .macchine field
        protected override void SetMachines() { }

        // IActivityImportData does not provide a usable .operatori field
        protected override void SetOperators() { }

    }
}
