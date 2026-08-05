using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Zone;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Zone
{
    public class ZoneService : BaseServiceAnagrafeBIZ, IZoneService
    {
        private readonly IZone _zoneDal;
        private readonly IAgro_Sequence _sequenceDal;

        public ZoneService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _zoneDal = _serviceProvider.GetRequiredService<IZone>();
            _sequenceDal = _serviceProvider.GetRequiredService<IAgro_Sequence>();
        }

        public async Task<DataTable> LeggiZoneAsync(int Zona_Cod, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable dt;

            try
            {
                dt = await _zoneDal.LeggiZoneAsync(Zona_Cod, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return dt;
        }

        //Lavez - 01/10/2024 - Esempio di apertura connessione\transazione , scrittura multiple e chiusura finale 

        //public async Task<bool> ScriviZone(Test_In input, AgronicaCoreParametri objParametri)
        //{
        //    bool result = false;
        //    try
        //    {
        //        OpenConnection(objParametri,false);
        //        foreach (var particella in input.particelle)
        //        {
        //            //OpenTransaction(objParametri);
        //            particella.Part_Cod = _sequenceDal.NuovoId_Tabella("ParticelleCatastali", CounterBaseTopCode.BaseCode, CounterBaseTopCode.TopCode, objParametri);

        //            result = _zoneDal.ScriviParticella(particella, objParametri);
        //            if (result == false)
        //                throw new Exception("Errore scrittura particella");
        //            //CloseTransaction(objParametri);
        //        }
                                   
        //        //CloseTransaction(objParametri);
        //        //OpenTransaction(objParametri);
        //        foreach (var zona in input.zone)
        //        {                   
        //            result = _zoneDal.ScriviZona(zona, objParametri);
        //            if (result == false)
        //                throw new Exception("Errore scrittura zone");
        //        }
        //        result = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        CloseTransaction(objParametri, true);
        //        result = false;
        //        LogError(ex.Message, objParametri, ex);
        //        throw;
        //    }finally
        //    {
        //        CloseConnection(objParametri);
        //    }

        //    return result;
        //}
    }
}
