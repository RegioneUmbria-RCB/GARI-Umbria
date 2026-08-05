using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.exceptions
{
    public enum ErroreGias_Severity
    {
         Bloccante = 0, //'Errore in basso a dx rosso, l'utente non può procedere (sia errori gestiti che exceptions)
         Warning = 1, // 'Pop-up di n warning accodati con possibilità per l'utente di proseguire ("Si desidera proseguire?"   -   Annulla/Prosegui)
         Info = 2,
         WarningBloccante = 3 //'Pop-up di n errori accodati senza possibilità per l'utente di proseguire ("Non è possibile proseguire" - OK --> l'utente DEVE sistemare dei dati, NON può procedere altrimenti )
    }
}
