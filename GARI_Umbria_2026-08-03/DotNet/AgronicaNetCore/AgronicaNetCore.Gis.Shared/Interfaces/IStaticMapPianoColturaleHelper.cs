using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Gis.Shared.Interfaces
{
    public interface IStaticMapPianoColturaleHelper
    {
        Task GeneraStaticMapDaSincroApp(
            DataTable dtImpianti,
            GeneraMappaStaticaInData staticMapCfg,
            AgronicaCoreParametri objParametriServer
        );
    }
}
