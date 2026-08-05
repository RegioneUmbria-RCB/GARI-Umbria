using System;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class UtenteColdiretti
    {
        public string COD_ANAGEN { get; set; }
        public string COD_TIPO_ANAG { get; set; }
        public DateTime? DAT_INIZIO { get; set; }
        public DateTime? DAT_FINE { get; set; }
        public Blocco_Data data { get; set; }

        private int isPersonaGiuridica = 0;
        public int IsPersonaGiuridica
        {
            get
            {
                if (isPersonaGiuridica == 0)
                {
                    if (COD_TIPO_ANAG == "G" && data.SGL_CODFIS.Length != 16)
                        isPersonaGiuridica = 1; // Persona Giuridica
                    else
                        isPersonaGiuridica = 2; // Prsona Fisica
                }
                return isPersonaGiuridica;
            }
        }

        public string GetUserCode()
        {
            if (IsPersonaGiuridica == 2) // è persona fisica
                return data.titolare.COD_ANAGEN;
            else
                return data.COD_ANAGEN;
        }

        public string Nome()
        {
            string result = "";
            if (IsPersonaGiuridica == 2) // è persona fisica
                result = data.titolare.DES_NOME;
            return result;
        }

        public string Cognome()
        {
            string result = "";
            if (IsPersonaGiuridica == 2) // è persona fisica
                result = data.titolare.DES_COGNOME;
            return result;
        }

        public string RagioneSociale()
        {
            string result = "";
            if (IsPersonaGiuridica == 1) // è persona giuridica
                result = data.DES_RAGIONE_SOCIALE;
            return result;
        }

    }

    public class Blocco_Data
    {
        public string COD_ANAGEN { get; set; }
        public string DES_RAGIONE_SOCIALE { get; set; }
        public string SGL_CODFIS { get; set; }
        public string SGL_PIVA { get; set; }
        public Blocco_Titolare titolare { get; set; }
    }

    public class Blocco_Titolare
    {
        public string COD_ANAGEN { get; set; }
        public string DES_NOME { get; set; }
        public string DES_COGNOME { get; set; }
        public string SGL_CODFIS { get; set; }
        public DateTime? DAT_NASCITA { get; set; }
        public string SGL_COMUNE_NASCITA { get; set; }
    }
}