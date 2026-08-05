using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.exceptions
{
    public enum ErroreGias_Tipo
    {
        Generico = 0,
        LoginFallito = 1,
        ConflittoPermessi = 2,
        ParticellaConLegami = 3,
        NonGestito = 999
    }
}
