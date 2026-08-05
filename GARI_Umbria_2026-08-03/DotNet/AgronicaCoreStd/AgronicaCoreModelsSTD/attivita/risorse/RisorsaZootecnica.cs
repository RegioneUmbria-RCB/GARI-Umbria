using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    public class RisorsaZootecnica : Risorsa
    {
        public string descrizione { get; set; }

        /// <summary>
        /// (= GEN_COD)
        /// </summary>
        public Genere genere { get; set; }

        /// <summary>
        /// Specie Animale (= SPE_COD)
        /// </summary>
        public Specie specie { get; set; }

        /// <summary>
        /// (= IPRO_COD)
        /// </summary>
        public IndirizzoProduttivo indirizzoProd { get; set; }


        public RisorsaZootecnica()
        {
            genere = new Genere();
            specie = new Specie();
            indirizzoProd = new IndirizzoProduttivo();

            classType = costanti.ClassType.RisorsaZootecnica;
        }

        public RisorsaZootecnica(int genCod, int speCod, int indProdCod) {
            genere = new Genere(genCod);
            specie = new Specie(speCod);
            indirizzoProd = new IndirizzoProduttivo(indProdCod);
            classType = costanti.ClassType.RisorsaZootecnica;
        }

    }
}
