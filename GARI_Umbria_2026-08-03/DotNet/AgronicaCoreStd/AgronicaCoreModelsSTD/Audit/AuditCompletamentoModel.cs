using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Audit
{
    public class AuditCompletamentoModel
    {
        public int Audit_Tipo { get; set; }
        public string Audit_Nome { get; set; }
        public int Completate { get; set; }
        public string Coltura { get; set; }
        public int Non_Completate { get; set; }
    }
}
