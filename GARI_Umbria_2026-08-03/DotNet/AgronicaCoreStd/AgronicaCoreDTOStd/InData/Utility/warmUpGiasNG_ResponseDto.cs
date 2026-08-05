using InData;
using System;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Utility
{
    public class warmUpGiasNG_ResponseDto
    {
        public AgronicaCoreParametri_NG objP_super_server;
        public AgronicaCoreParametri_NG objP_server;
        public AgronicaCoreParametri_NG objP_utenti;
        public Parametri_ObjParametriAgenda_NG objParametri_Agenda;
        public Utente_Permessi utente;
        public AgronicaLink_NG link;
        public VariabiliInSessione_NG VariabiliInSessione;
        public List<Imprese_Impostazioni> impresa_impostazioni;
        public int Lingua_Cod;
    }
}
