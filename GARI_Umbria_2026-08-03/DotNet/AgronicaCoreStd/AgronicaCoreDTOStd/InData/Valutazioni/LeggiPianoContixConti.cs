using AgronicaCoreModelsSTD.valutazioni;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Valutazioni
{

    public class LeggiPianoContixConti
    {
        public string piva;
        public int pianoCod;
        public int contoCod;

        public LeggiPianoContixConti()
        {
            pianoCod = 0;
            contoCod = 0;
        }
    }




    //TREE


    public class TreeValutazionePianoContixConti

    {

        public string piva;

        public string id;

        public string tipo;

        public int codice;

        public string descrizione;

        public bool flag_collegato;


        public List<TreeValutazione_Item> TreeValutazione { get; set; }


        public TreeValutazionePianoContixConti()

        {

            id = "";
            codice = 0;

        }

    }
    public class TreeValutazione_Item

    {

        public string id;

        public string tipo;

        public int codice;

        public string descrizione;

        public int ordine;

        public bool flag_collegato;

        public bool flag_selezionato;



        public List<TreeValutazione_Item> children { get; set; }

        public TreeValutazione_Item()
        {
            id = "";
            codice = 0;
        }

        public void setSelectedByChildren()
        {
            int numFigli = children.Count;

            if (numFigli > 0 &&
                numFigli == children.Count(x => x.flag_selezionato == true))
            {
                flag_selezionato = true;
            }
        }

    }

}