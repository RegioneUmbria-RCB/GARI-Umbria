using AgronicaNetCore.Anagrafe.BIZ.Services.PianoColturale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgronicaNetCore.Anagrafe.DAL.DataLayer;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaCoreModelsSTD.anagrafiche;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.DettagliMateriePrime;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.DettagliMateriePrime
{
    public class DettagliMateriePrimeService : BaseServiceAnagrafeBIZ, IDettagliMateriePrimeService
    {
        private readonly IDettagliMateriePrime _dettagliMateriaPrimaDAL;

        public DettagliMateriePrimeService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _dettagliMateriaPrimaDAL = provider.GetRequiredService<IDettagliMateriePrime>();
        }

        public async Task<MateriePrime?> GetMateriaPrimaByCodAsync(int matCod, AgronicaCoreParametriServer objParametriServer)
        {
            // 1. Il BIZ chiama il DAL per ottenere i dati grezzi
            DataTable dtDettaglio = await _dettagliMateriaPrimaDAL.GetDettagliMateriaPrimaAsync(matCod, objParametriServer);

            if (dtDettaglio == null || dtDettaglio.Rows.Count == 0)
            {
                return null;
            }

            // 2. Il BIZ esegue la mappatura da DataRow a oggetto DTO
            var materiaPrimaMappata = MapDataRowToModel(dtDettaglio.Rows[0]);

            return materiaPrimaMappata;
        }

        /// <summary>
        /// Metodo helper privato per la mappatura.
        /// </summary>
        private MateriePrime MapDataRowToModel(DataRow row)
        {
            return new MateriePrime
            {
                partitaIva = row["Piva"] != DBNull.Value ? row["Piva"].ToString() : string.Empty,

                codice = Convert.ToInt32(row["Mat_Cod"]),

                cod_articolo = row["Cod_Articolo"] != DBNull.Value ? row["Cod_Articolo"].ToString() : string.Empty,

                descrizione = row["Mat_Des"] != DBNull.Value ? row["Mat_Des"].ToString() : string.Empty,

                categoria_prodotto = Convert.ToInt32(row["Elem_Cod"]),

                // Date
                data_Creazione = row["data_creazione"] != DBNull.Value ? Convert.ToDateTime(row["data_creazione"]) : DateTime.MinValue,
                data_Modifica = row["data_modifica"] != DBNull.Value ? Convert.ToDateTime(row["data_modifica"]) : DateTime.MinValue,

                centroPK = new CentroAziendale.PK
                {
                    codice = Convert.ToInt32(row["Sa_Cod"]), 
                    partitaIva = row["Piva"] != DBNull.Value ? row["Piva"].ToString() : string.Empty
                },

                validita = new IntervalloTemporale
                {
                    inizio = row["validita_inizio"] != DBNull.Value ? Convert.ToDateTime(row["validita_inizio"]) : new DateTime(1900, 1, 1),
                    fine = row["validita_fine"] != DBNull.Value ? Convert.ToDateTime(row["validita_fine"]) : new DateTime(2100, 12, 31)
                },

                visibilitaPubblica = (Convert.ToInt32(row["Sa_Cod"]) == -1)
            };
        }
    }
}
