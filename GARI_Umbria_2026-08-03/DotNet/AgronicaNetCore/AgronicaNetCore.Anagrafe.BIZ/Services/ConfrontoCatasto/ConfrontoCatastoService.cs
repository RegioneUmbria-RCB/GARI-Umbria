using AgronicaCoreDTOStd.Identity;
using AgronicaCoreDTOStd.InData.ConfrontoCatasto;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impianti;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Programmazione;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.ConfrontoCatasto
{
    public class ConfrontoCatastoService : BaseServiceAnagrafeBIZ, IConfrontoCatastoService
    {
        public ConfrontoCatastoService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
        }

        public async Task<DataTable?> GetConfrontoCatastoAsync(ParametriTipoConfronto tipoConfronto, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;
            try
            {
                switch ((enum_TipoConfrontoCatasto)tipoConfronto.tipoConfronto)
                {
                    case enum_TipoConfrontoCatasto.Planning_PianoColturale:
                        res = await ElaboraConfrontoPlanning_PianoColturaleAsync(tipoConfronto, objParametriServer);
                        break;
                    case enum_TipoConfrontoCatasto.PianoColturale_Planning:
                        break;

                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }
            return res;
        }

        private async Task<DataTable?> ElaboraConfrontoPlanning_PianoColturaleAsync(ParametriTipoConfronto parametriConfronto, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                var pianocolturaleDal = _serviceProvider.GetRequiredService<IImpianti>();
                var programmazioneDal = _serviceProvider.GetRequiredService<IProgrammazione>();

                var DT_Ori = await programmazioneDal.GetPlanningPerConfrontoCatastoAsync(parametriConfronto.partitaIva,
                                                                              parametriConfronto.programmazioneCod,
                                                                              objParametriServer,
                                                                              true,
                                                                              parametriConfronto.showCatasto,
                                                                              parametriConfronto.showVarieta
                                                                              );

                var DT_Act = await pianocolturaleDal.GetPianoColturalePerConfrontoCatastoAsync(parametriConfronto.partitaIva,
                                                                                    parametriConfronto.dataInizio,
                                                                                    parametriConfronto.dataFine,
                                                                                    objParametriServer,
                                                                                    false,
                                                                                    parametriConfronto.showCatasto,
                                                                                    parametriConfronto.showVarieta
                                                                              );

                if (DT_Ori == null)
                    throw new Exception("Dati planning non valorizzati. verificare lettura");
                if (DT_Act == null)
                    throw new Exception("Dati piano colturale non valorizzati. verificare lettura");

                DT_Ori.Merge(DT_Act);

                if (parametriConfronto.showCatasto)
                {
                    DT_Ori = DT_Ori.AsEnumerable()
                   .OrderBy(r => r.Field<string>("Prov_Des"))
                   .ThenBy(r => r.Field<string>("Com_Des"))
                   .ThenBy(r => r.Field<string>("Sezione"))
                   .ThenBy(r => r.Field<int>("Foglio"))
                   .ThenBy(r => r.Field<int>("Numero"))
                   .ThenBy(r => r.Field<string>("Subalterno"))
                   .CopyToDataTable();
                }

                return DT_Ori;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                return null;
            }

        }
    }
}
