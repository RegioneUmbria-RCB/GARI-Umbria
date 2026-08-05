using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Data;

namespace OutData.Zoo
{
    public class InterventoZoo
    {
        public int Id_Intervento { get; set; }
        public string Descrizione { get; set; }
        public int Ordine { get; set; }

        public List<ProtocolloFromIntervento> Protocolli { get; set; }

        public InterventoZoo()
        {
            Protocolli = new List<ProtocolloFromIntervento>();
        }

        public InterventoZoo(int idIntervento, string descrizione, int ordine, List<ProtocolloFromIntervento> protocolli = null)
        {
            Id_Intervento = idIntervento;
            Descrizione = descrizione;
            Ordine = ordine;
            Protocolli = protocolli ?? new List<ProtocolloFromIntervento>();
        }

        public InterventoZoo(DataRow drIntervento)
        {
            Id_Intervento = (int)drIntervento["Id_Intervento"];
            Descrizione = (string)drIntervento["Intervento_Des"];
            Ordine = (int)drIntervento["Ordine"];
            Protocolli = new List<ProtocolloFromIntervento>();
        }

        public bool IsValid()
        {
            return Id_Intervento != 0 && !string.IsNullOrEmpty(Descrizione) && Protocolli.Count > 0;
        }
    }

    public class ProtocolloFromIntervento
    {
        public int Id_Protocollo { get; set; }
        public int? Id_Protocollo_Alt { get; set; }


        public ProtocolloFromIntervento() {}
        
        public ProtocolloFromIntervento(int idProtocollo) => Id_Protocollo = idProtocollo;

        public ProtocolloFromIntervento(DataRow drProtocollo)
        {
            Id_Protocollo = (int)drProtocollo["Id_Protocollo"];
            Id_Protocollo_Alt = drProtocollo["Id_Protocollo_Alt"] == DBNull.Value ? null : (int?)drProtocollo["Id_Protocollo_Alt"];
        }
    }
}
