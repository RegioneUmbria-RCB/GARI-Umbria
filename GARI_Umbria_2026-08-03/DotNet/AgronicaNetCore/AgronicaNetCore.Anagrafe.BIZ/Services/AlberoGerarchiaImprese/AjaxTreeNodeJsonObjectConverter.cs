namespace AgronicaNetCore.Anagrafe.BIZ.Services.AlberoGerarchiaImprese
{
    public static class AjaxTreeNodeJsonObjectConverter
    {
        public static void AjaxTreeNodeJsonObject_KendoHierarchical(List<AjaxTreeNodeFASTJsonObject> nodes, KendoHierarchicalDataSource kendoHierarchical)
        {
            foreach (var node in nodes)
            {
                AjaxTreeNodeJsonObject_KendoHierarchical(node, kendoHierarchical, null);
            }
        }

        private static void AjaxTreeNodeJsonObject_KendoHierarchical(AjaxTreeNodeFASTJsonObject node, KendoHierarchicalDataSource kendoHierarchical, List<KendoHierarchicalDataSource>? listaAppendi)
        {
            CopiaTreenodeKendoHierarchical(node, kendoHierarchical);

            if (listaAppendi is not null)
            {
                listaAppendi.Add(kendoHierarchical);
            }

            if (node.Children is not null && node.Children.Any())
            {
                if (kendoHierarchical.Items is null)
                {
                    kendoHierarchical.Items = new List<KendoHierarchicalDataSource>();
                }

                foreach (var child in node.Children)
                {
                    AjaxTreeNodeJsonObject_KendoHierarchical(child, new KendoHierarchicalDataSource(), kendoHierarchical.Items);
                }
            }
        }

        private static void CopiaTreenodeKendoHierarchical(AjaxTreeNodeFASTJsonObject node, KendoHierarchicalDataSource kendoHierarchical)
        {
            kendoHierarchical.TipoNodo = node.TipoNodo;
            kendoHierarchical.Piva = node.Piva;
            kendoHierarchical.PivaPadre = node.PivaPadre;
            kendoHierarchical.CUAA = node.CUAA;
            kendoHierarchical.Text = node.Text;
            kendoHierarchical.Expanded = node.State.Opened;
        }
    }
}
