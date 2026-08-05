using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Classe implementata per poter utilizzare AgronicaCoreGestioneRichieste.ParametriAgronicaAuditPUA, 
    /// sucessivamente è necessario Serializzarla in stringa e deserializzarla nel tipo AgronicaCoreGestioneRichieste.ParametriAgronicaAuditPUA
    /// </summary>
    public class ObjParams_AuditPUA
    {
        /// <summary>
        /// Assolutamente da lasciare come prima property 
        /// poichè gestisce la sessione nella classe a lvl inferiore
        /// </summary>
        public bool Sessione { get; set; }
        public string Piva { get; set; }
        public int Tipo_Audit { get; set; }
        public string username_codfisc { get; set; }
        public int programmazione_cod { get; set; }
        public int tipo_operazione { get; set; }
        public int sito_origine { get; set; }
        public int Regolamento_Cod { get; set; }
        public int Audit_Cod { get; set; }
        public string LinkAgronicaAgenda2010 { get; set; }
    }
}
