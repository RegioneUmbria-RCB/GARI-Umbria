using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Shared
{
    public class GridDto
    {
        public class Vista
        {
            public string IdVista;
            public string NomeUtente;
            public string GridId;
            public bool Predefinita;
            public string NomeVista;
            public string Stato;
            public string Colonne;
            public string FiltroJSON;
            public bool FlagPubblica;
        }

        public class SalvaVisteWrapper
        {
            public List<Vista> VisteNuove;
            public List<Vista> VisteModificate;
        }

        public class ChiaveVista
        {
            public string GridId;
            public string IdVista;
            public string NomeUtente;
        }

    }
}
