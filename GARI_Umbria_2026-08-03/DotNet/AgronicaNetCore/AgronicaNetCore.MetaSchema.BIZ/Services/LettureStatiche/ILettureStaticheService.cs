using System.Data;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.LettureStatiche
{
    public interface ILettureStaticheService
    {
        Task<DataTable> LeggiMetodiDiProduzioneAsync();
    }
}
