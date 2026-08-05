using AgronicaNetCore.Base.Base;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.LettureStatiche
{
    public class LettureStatiche : Serilog_Base, ILettureStatiche
    {
        public LettureStatiche(IServiceProvider provider) : base(provider) {}

        public DataTable LeggiMetodiDiProduzione()
        {
            var metodiDiProduzione = new DataTable("Metodi di produzione");

            DataColumn value = new DataColumn("Value");
            value.DataType = typeof(int);
            metodiDiProduzione.Columns.Add(value);

            DataColumn descrizione = new DataColumn("Descrizione");
            descrizione.DataType = typeof(string);
            metodiDiProduzione.Columns.Add(descrizione);

            //DataRow nonImpostato = metodiDiProduzione.NewRow();
            //nonImpostato["Value"] = 0;
            //nonImpostato["Descrizione"] = "";
            //metodiDiProduzione.Rows.Add(nonImpostato);

            DataRow convenzionale = metodiDiProduzione.NewRow();
            convenzionale["Value"] = 1;
            convenzionale["Descrizione"] = "Convenzionale";
            metodiDiProduzione.Rows.Add(convenzionale);

            DataRow inConversione = metodiDiProduzione.NewRow();
            inConversione["Value"] = 2;
            inConversione["Descrizione"] = "InConversione";
            metodiDiProduzione.Rows.Add(inConversione);

            DataRow biologico = metodiDiProduzione.NewRow();
            biologico["Value"] = 3;
            biologico["Descrizione"] = "Biologico";
            metodiDiProduzione.Rows.Add(biologico);

            return metodiDiProduzione;
        }
    }
}
