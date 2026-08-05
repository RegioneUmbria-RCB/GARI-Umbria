using System.Collections.Generic;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.profilazione;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class LeggiAziendexUtenti
    {
    }

    public class ImpresexUtentiVisibilita : ImpresaDto
    {
        public string Username { get; set; }
        public string DettagliUtente { get; set; }
        public string Gruppo_Des { get; set; }
        public string Cuaa { get; set; }
    }
}
