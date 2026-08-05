using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Base.DataLayer.ConfigurazioneElasticSearch
{
    public interface IConfigurazioneElasticSearchDAL
    {
        [Obsolete("Metodo che effettua lettura sincrona")]
        Models.ConfigurazioneElasticSearch LeggiConfigurazione(AgronicaCoreParametriServer objParametriServer);
    }
}
