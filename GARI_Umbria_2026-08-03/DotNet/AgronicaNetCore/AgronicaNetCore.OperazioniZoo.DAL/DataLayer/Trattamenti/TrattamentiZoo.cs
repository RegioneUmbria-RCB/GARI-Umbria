using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.Trattamenti
{
    public class TrattamentiZoo : BaseDALOperazioniZoo, ITrattamentiZoo
    {
        public TrattamentiZoo(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {

        }

        public async Task<bool> ScriviAgendaAsync(string Piva, int Sa_Cod, int Sta_Num, int Id_Agenda, DateTime inizio, DateTime fine, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Agenda (PIVA, Sa_Cod, Sta_Num, Id_Agenda, Lav_Cod, des_lib, Blocco_Flag, Blocco_Data, Blocco_Username, ")
                .AppendLine("    inviato, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, ")
                .AppendLine("    Stato_Export, Stato_Export_2, Tipo_Visibilita, ChkCoge_Manuale, Id_Attivita, Modulo, Audit_Cod, Raccoglitore_Cod, Split, Pratica_Cod, Origine) ");
            stbQuery.AppendLine("VALUES (@piva, @saCod, @staNum, @idAgenda, @lavCod, 'Cure e medicamenti', 0, @adInizio, '', ")
                .AppendLine("    0, GETDATE(), GETDATE(), @usernameOp, @usernameOp, @inizio, @fine, ")
                .AppendLine("    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, '') ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", Piva);
            expandoObj.TryAdd("@saCod", Sa_Cod);
            expandoObj.TryAdd("@staNum", Sta_Num);
            expandoObj.TryAdd("@idAgenda", Id_Agenda);
            expandoObj.TryAdd("@lavCod", LAV_COD.LAVCOD_CUREMEDICAMENTI_ANIMALI);
            expandoObj.TryAdd("@usernameOp", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@inizio", inizio);
            expandoObj.TryAdd("@fine", fine);
            expandoObj.TryAdd("@adInizio", CostantiPersonalizzate.AGRODATAINIZIO);

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> ScriviAgroLogAgendaAsync(int idAgenda, string piva, int saCod, DateTime inizio, int tipoOp, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Agronica_Log_Agenda (ID, Sa_Cod, SuperUser, Utente, Lav_Cod, Des_lib, Id_Servizio, Tipo_Operazione, Data_Ora_Lavorazione, Data_Ora_RegistrazioneLog) ");
            stbQuery.AppendLine("VALUES (@id, @saCod, @superUser, @utente, @lavCod, 'Cure e medicamenti', @idServizio, @tipoOp, @dataLav, GETDATE()) ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@id", idAgenda);
            expandoObj.TryAdd("@utente", piva);
            expandoObj.TryAdd("@saCod", saCod);
            expandoObj.TryAdd("@superUser", objParametriServer.PivaSuperUser);
            expandoObj.TryAdd("@lavCod", LAV_COD.LAVCOD_CUREMEDICAMENTI_ANIMALI);
            expandoObj.TryAdd("@idServizio", 5); //enum_Id_Servizio.GiasOnline
            expandoObj.TryAdd("@tipoOp", tipoOp);
            expandoObj.TryAdd("@dataLav", inizio);

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> ScriviMovimentiAsync(string piva, int idAgenda, DateTime inizio, DateTime registrazione, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Movimenti (PIVA, Sa_Cod, Id_Agenda, Id_Mov, Cod_RisUm, Cau_Mov, Mov_Desc, Data_Movimento, Scadenza, Doc_Numero, Num_Protocollo, ")
                .AppendLine("    inviato, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, ")
                .AppendLine("    Cod_IndirizzoRisUm, Cod_Destinazione, Cod_IndirizzoDestinazione, Mezzo, Cod_Vettore, Cod_IndirizzoVettore, Causale_Trasporto, ")
                .AppendLine("    Aspetto, Peso, Ora, Colli, Extra_Str, Extra_Int, Extra_Date, Tipo_Sconto, Doc_Numero_Des, Natura_Beni, Tara_Veicolo, Tara_Imballi, ")
                .AppendLine("    Tipo_Peso, Modalita, Username_Note, Scadenza_Extra, Doc_Numero_Sin, Progr_Protocollo, Progr_Registrazione, Data_Registrazione,  ")
                .AppendLine("    ChkLayOut_Bypass_Fatturato, ChkLayOut_Join_Prodotti, Cod_RisUm_Altro, ChkLayOut_Peso, ChkLayOut_Prezzo, ChkLayOut_Varietale, ")
                .AppendLine("    Disciplinare_PubblicoPrivato, Sezionale_Cod, Causale_Trasporto_Cod, ChkLayOut_Litri, Cod_RisUm_Aggiuntivo, Cod_Indirizzo_Aggiuntivo, ")
                .AppendLine("    ChkLayOut_Riscontrato, Doc_Numero_Visualizzato, TipoDocumento, Cod_Macchina_Lav, OraFine, Modalita_Applicazione) ");
            stbQuery.AppendLine("VALUES (")
                .AppendLine("");

            var expandoObj = new ExpandoObject();

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
