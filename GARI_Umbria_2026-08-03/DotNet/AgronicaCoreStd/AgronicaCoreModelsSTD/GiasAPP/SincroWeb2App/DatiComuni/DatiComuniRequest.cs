using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni
{
    public class DatiComuniRequest
    {
        // anagrafiche metaschema
        public bool anagMetaschema = true;

        // impostazioni rilievi
        public bool anagPersonalizzataRilievoFasiFenologiche;
        public bool anagPersonalizzataRilievoIndiciMaturita;
        public bool anagPersonalizzataRilievoAvversita;
        public bool anagPersonalizzataRilievoDanniRaccolta;
        public bool anagPersonalizzataRilievoErbeInfestanti;


        // impostazioni prodotti
        public bool anagTrasformatiVegetali;
        public bool anagSementi;
        public bool anagFormulati;
        public bool anagFertilizzanti;
        public bool anagCodificaProdotti;
        public string specieProdotti;

        // impostazioni specie vegetali
        public string anagUtilizziTerreno;

        // impostazioni nazioni estero
        public string anagNazioni;

        // anagrafiche fornitori
        public bool anagFornitori;

        // impostazioni disciplinari
        public string anagDisciplinari;

        // gestione sincro in base ai permessi
        public string permessi;

        public string timestampUltimaSincronizzazione;
    }
}
