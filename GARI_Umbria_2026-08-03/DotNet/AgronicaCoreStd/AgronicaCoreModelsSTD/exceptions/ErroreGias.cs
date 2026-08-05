using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.exceptions
{
    public class ErroreGias
    {
        public ErroreGias_Severity severity;
        public string messaggio;
        public string ex;
        public ErroreGias_Tipo tipo;
    }
}
