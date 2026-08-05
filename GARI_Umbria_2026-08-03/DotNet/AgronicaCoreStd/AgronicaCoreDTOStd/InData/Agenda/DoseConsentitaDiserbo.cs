using System;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class DoseConsentitaDiserbo
    {
        public decimal D_HA_Max_Diserbo { get; set; }

        public string lbl_qta_residua { get; set; }

        public int Udm_Radice_HA { get; set; }

        public string Lbl_Dose_Consigliata { get; set; }

        public string Lbl_Dose_Consigliata2 { get; set; }

        public decimal percAbbDaApplicare { get; set; }

        public string principiAttiviPercAbb { get; set; }

        public bool Div_DettaglioDoseConsentita { get; set; }

    }
}