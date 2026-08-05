namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP
{
    public interface ILetturaTabellaComuneApp
    {
        Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters);
    }
}
