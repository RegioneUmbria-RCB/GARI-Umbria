using AgronicaNetCore.Base.Constants;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.AlberoGerarchiaImprese
{
    public class KendoHierarchicalDataSource : IDisposable
    {
        public string Text { get; set; }
        public TipoNodo TipoNodo { get; set; }
        public string Piva { get; set; }
        public string PivaPadre { get; set; }
        public string CUAA { get; set; }
        public bool Expanded { get; set; }
        public List<KendoHierarchicalDataSource> Items { get; set; }

        public void Dispose()
        {
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    item.Dispose();
                }
                Items.Clear();
                Items = null;
            }
            
        }
    }
}
