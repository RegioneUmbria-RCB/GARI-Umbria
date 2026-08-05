using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.SmartTractors_HubIoT
{

    public class WorkOrderKeyBase
    {
        public int Entita_Origine { get; set; }
        public string PivaSuperUser { get; set; }
        public string Piva { get; set; }
        public int sa_cod { get; set; }
        public int appezza { get; set; }
        public int id_reg { get; set; }
        public int Stato { get; set; }
        public int RegolaElaborazione { get; set; }
        public string workerOrderId { get; set; }

        public WorkOrderKeyBase(int origine)
        {
            Entita_Origine = origine;
        }
    }

    public class WorkOrderKeyRicettaOperazione : WorkOrderKeyBase
    {
        public WorkOrderKeyRicettaOperazione() : base(1) { }
        public int Ricetta_Cod { get; set; }
        public int Ricetta_Operazione_Cod { get; set; }
        public int Mac_Cod { get; set; }
        public DateTime Data_Ricetta { get; set; } = new DateTime(1900, 01, 01);
    }

    public class WorderOrderKeyOperazionePianificata : WorkOrderKeyBase
    {
        public WorderOrderKeyOperazionePianificata() : base(2) { }
        public int Id_Agenda { get; set; }
        public int id_mov_det { get; set; } = 0;
        public int Mac_Cod { get; set; }
        public DateTime Data_Operazione { get; set; } = new DateTime(1900, 01, 01);
    }

    public class WorkOrderKeyConsiglioIrriguo : WorkOrderKeyBase
    {
        public WorkOrderKeyConsiglioIrriguo() : base(3) { }
    }

    public class WorkOrderKey : WorkOrderKeyBase
    {
        public WorkOrderKey() : base(0){}
        public int Id_documento { get; set; }
        public int id_operazione_documento { get; set; } = 0;
        public int Mac_Cod { get; set; }
        public DateTime Data_Registrazione { get; set; } = new DateTime(1900, 01, 01);

    }

    #region "From agenda to hubiot"
    public class RicettaOperazione2WorkOrderKey
    {
        public int Entita_Origine { get; set; } = 1;
        public string PivaSuperUser { get; set; } 
        public int Ricetta { get; set; }
        public int RicettaOperazione { get; set; }
        public int MacCod { get; set; }
    }

    public class OperazionePianificata2WorkOrderKey
    {
        public int Entita_Origine { get; set; } = 1;
        public string PivaSuperUser { get; set; }
        public int id_agenda { get; set; }
        public int id_mov_det { get; set; }
        public int MacCod { get; set; }
    }
    #endregion

}
