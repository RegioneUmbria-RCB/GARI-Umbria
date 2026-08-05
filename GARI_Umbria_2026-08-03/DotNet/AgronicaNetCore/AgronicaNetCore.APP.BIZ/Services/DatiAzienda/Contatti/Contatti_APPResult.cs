using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Contatti
{
    public class Contatti_APPResult
    {
        public List<ContattoEntity> Contatti { get; set; } = new();

        public List<RisorseUmaneEntity> RisorseUmane { get; set; } = new();

        public List<ContattoEntity> Fornitori { get; set; } = new();

        public List<RisorseUmaneEntity> RisorseFornitori { get; set; } = new();

        public List<ContattoEntity> FornitoriMeteo { get; set; } = new();

        public List<RisorseUmaneEntity> RisorseFornitoriMeteo { get; set; } = new();

    }
}
