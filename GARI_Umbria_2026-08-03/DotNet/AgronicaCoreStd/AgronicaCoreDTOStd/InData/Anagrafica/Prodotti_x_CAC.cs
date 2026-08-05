using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class Prodotti_x_CAC
    {
        public string Cau_Mov { get; set; }
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Id_Destinazione { get; set; }
        public string Elem_Cod { get; set; }
        public Boolean Flag_Negativo { get; set; }
        public string RicercaTesto { get; set; }
        public string RicercaTestoJArray { get; set; }
        public Boolean Flag_VisualizzaProCod { get; set; }
        public Boolean Flag_CaricaUdmCod { get; set; }
        public int Pro_Cod { get; set; }
        public int Udm_Cod { get; set; }
        public DateTime DataFiltroFormulati { get; set; }
        public int PUA_RegolamentoCod { get; set; }
        public int TipoRichiesto { get; set; }
        public Boolean Flag_LeggiGiacenze { get; set; }
        public Boolean Flag_FiltraRevocati { get; set; }
        public int RegolamentoCod_Operazioni { get; set; }
        public int Tipo_PuaRegolamento { get; set; }
        public Boolean Flag_IncludiNPK_Desc { get; set; }
        public Boolean Flag_IncludiClassificazione { get; set; }
        public Boolean Flag_QtaNoZero { get; set; }
        public string xFiltroAggiuntivo { get; set; }
        public Boolean Flag_Filtra_MateriePrime_Per_Piva { get; set; }
        public Boolean Flag_Filtra_MateriePrime_Pubblici { get; set; }
        public Boolean Flag_CodArticolo_In_Descrizione { get; set; }

    }
}
