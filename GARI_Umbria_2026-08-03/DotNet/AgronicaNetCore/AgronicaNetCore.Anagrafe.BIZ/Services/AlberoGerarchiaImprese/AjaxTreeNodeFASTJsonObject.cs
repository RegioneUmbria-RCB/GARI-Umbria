namespace AgronicaNetCore.Anagrafe.BIZ.Services.AlberoGerarchiaImprese
{
    public class AjaxTreeNodeFASTJsonObject
    {

        public AjaxTreeNodeFASTJsonObject(string text, TipoNodo tipoNodo, string piva = "", string pivaPadre = "", string xCUAA = "", List<AjaxTreeNodeFASTJsonObject>? children = null)
        {
            Text = text;
            TipoNodo = tipoNodo;
            Piva = piva;
            PivaPadre = pivaPadre;
            CUAA = xCUAA;

            State = new AjaxTreeNodeFASTJsonObjectState();
            if (children is not null)
            {
                State.Opened = false;
                Children = children;
            }
        }

        public string Text { get; set; }
        public TipoNodo TipoNodo { get; set; }
        public string Piva { get; set; }
        public string PivaPadre { get; set; }
        public string CUAA { get; set; }
        public AjaxTreeNodeFASTJsonObjectState State { get; set; }
        public List<AjaxTreeNodeFASTJsonObject>? Children { get; set; }
    }
}
