using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Anagrafica
{
    public class ScriviMagazzini
    {
        public List<Fabbricato> magazzini;

        public ScriviMagazzini(List<Fabbricato> listMagazzini)
        {
            magazzini = listMagazzini;
        }
    }
}
