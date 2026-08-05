using AgronicaCoreModelsSTD.profilazione;
using AgronicaCoreModelsSTD.utente;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.AgronicaCoreUtentiBIZ
{
    public class SalvaImpostazioni_AziendeCentri
    {
        // public AgronicaCoreModelsSTD.profilazione.Impostazione[] impostazioni { get; set; }
        public Utente_Impostazioni[] impostazioni { get; set; }
        public ImpresaDto[] imprese { get; set; }
    }
}
