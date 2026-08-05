using AgronicaNetCore.Anagrafe.DAL.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgronicaNetCore.Anagrafe.DAL.DataLayer;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.DettagliMateriePrime
{
    public class DettagliMateriePrime : BaseDALAnagrafe, IDettagliMateriePrime
    {
        public DettagliMateriePrime(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> GetDettagliMateriaPrimaAsync(int matCod, AgronicaCoreParametriServer objParametriServer)
        {
            // Ho elencato le colonne della tabella che corrispondono alle proprietà del modello MateriePrime.
            const string query = @"
                SELECT 
                    Piva,
                    Sa_Cod,
                    Elem_Cod,
                    Mat_Cod,
                    Cod_Articolo,
                    Mat_Des,
                    validita_inizio,
                    validita_fine,
                    data_creazione,
                    data_modifica
                FROM 
                    Materie_Prime 
                WHERE 
                    Mat_Cod = @MatCod";

            var sqlParams = new Dictionary<string, object>
            {
                { "@MatCod", matCod }
            };

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(query, sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
