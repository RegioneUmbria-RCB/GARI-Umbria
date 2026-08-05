using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.profilazione
{
    public interface IUtente
    {
        string UserName { get; set; }
    }

    public interface IUtentePassword : IUtente
    {
        string Password { get; set; }
    }

    public interface IUtenteFinestraTemp: IUtente
    {
        IntervalloTemporale FinestraTemporale { get; set; }
    }

    public interface IUserConfig
    {
        int profile { get; set; }
    }
}
