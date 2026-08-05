using AgronicaCoreDTOStd.OutData;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Data;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni
{
    public interface IUtentiImpostazioni
    {
        Task<DataTable> Read_JoinWithFiltroMonoAsync(TipiEnumerativi.Enum_Impostazioni_Utenti Impostazione_Cod, int User1_SuperUser2, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> Read_User_Then_SuperUserAsync(int cod, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> ReadAsync(int cod, int User1_SuperUser2, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);

        Task<DataInizioEFine> CropYearAsync(DateTime dataRiferimento, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
        Task<string> ImpostazioneValore1_from_ImpostazioneCod(Enum_Impostazioni_Utenti Impostazione_Cod, int User1_SuperUser2, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);

        Task<string> LeggiConDefault(Enum_Impostazioni_Utenti impostazioneCod, int Username_1Utente_o_2SuperUser, string valoreDefault, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
    }
}
