using AgronicaCoreDTOStd.InData.Pratiche;
using AgronicaCoreVisibilitaStd;
using AgronicaCoreVisibilitaStd.Models;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.BIZ.Resources;
using AgronicaNetCore.Utenti.DAL.DataLayer.Pratiche;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Utenti.BIZ.Services.VisibilitaCalcolo
{
    public class VisibilitaCalcoloService : BaseServiceUtentiBIZ, IVisibilitaCalcoloService
    {
        private readonly IPratiche _praticheDal;
        private readonly IImpresa _impresaDal;
        private readonly IUtentiVisibilitaAppoggio _uvaDal;
        private readonly ISecurityLayerDAL _securityLayerDAL;

        private const string CfgKeyCapostipiti = "Utenti_Visibilia_Appoggio_Da_Capostipiti";

        public VisibilitaCalcoloService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _praticheDal = provider.GetRequiredService<IPratiche>();
            _impresaDal = provider.GetRequiredService<IImpresa>();
            _uvaDal = provider.GetRequiredService<IUtentiVisibilitaAppoggio>();
            _securityLayerDAL = provider.GetRequiredService<ISecurityLayerDAL>();
        }

        public async Task<VisibilitaCombinataResult> CalcolaPreviewAsync(
            string username,
            string descrizione1,
            string descrizione2,
            bool filtroPraticheAttivo,
            string operatoreFiltri,
            List<PraticaFiltrata_IN> pratiche,
            AgronicaCoreParametriDouble objParametriDouble,
            CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var calcolo = await CalcolaInternoAsync(descrizione1, descrizione2, filtroPraticheAttivo, operatoreFiltri, pratiche, objParametriDouble);
            var totaleImprese = await _impresaDal.ContaTotaleImpreseAsync(objParametriDouble.ObjParametriServer);

            return new VisibilitaCombinataResult
            {
                NumPivaVisibili = calcolo.PiveFinali.Count,
                NumCentriVisibili = calcolo.CentriFinali.Count,
                NumPivaTotali = totaleImprese,
                VisibilitaTotale = calcolo.PiveFinali.Count == totaleImprese,
                TimestampCalcolo = DateTime.UtcNow,
                PersistitoSuUVA = false
            };
        }

        public async Task<VisibilitaCombinataResult> CalcolaEPersistiAsync(
            string username,
            string descrizione1,
            string descrizione2,
            bool filtroPraticheAttivo,
            string operatoreFiltri,
            List<PraticaFiltrata_IN> pratiche,
            AgronicaCoreParametriDouble objParametriDouble,
            CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var calcolo = await CalcolaInternoAsync(descrizione1, descrizione2, filtroPraticheAttivo, operatoreFiltri, pratiche, objParametriDouble);
            var totaleImprese = await _impresaDal.ContaTotaleImpreseAsync(objParametriDouble.ObjParametriServer);

            var objParametriServer = objParametriDouble.ObjParametriServer;
            await OpenConnectionAsync(objParametriServer, OpenTransaction: true, level: IsolationLevel.ReadUncommitted);
            try
            {
                await _uvaDal.CancellaPerUtenteAsync(username, objParametriServer);

                var dtImprese = BuildDataTableImprese(objParametriServer.PivaSuperUser, username, calcolo.PiveFinali);
                if (dtImprese.Rows.Count > 0)
                    await _uvaDal.BulkInsertAsync(dtImprese, objParametriServer);

                var dtCentri = BuildDataTableCentri(objParametriServer.PivaSuperUser, username, calcolo.CentriFinali);
                if (dtCentri.Rows.Count > 0)
                    await _uvaDal.BulkInsertAsync(dtCentri, objParametriServer);

                CloseTransaction(objParametriServer);
            }
            catch
            {
                CloseTransaction(objParametriServer, Rollback: true);
                throw;
            }
            finally
            {
                CloseConnection(objParametriServer);
            }

            return new VisibilitaCombinataResult
            {
                NumPivaVisibili = calcolo.PiveFinali.Count,
                NumCentriVisibili = calcolo.CentriFinali.Count,
                NumPivaTotali = totaleImprese,
                VisibilitaTotale = calcolo.PiveFinali.Count == totaleImprese,
                TimestampCalcolo = DateTime.UtcNow,
                PersistitoSuUVA = true
            };
        }

        private async Task<CalcoloVisibilita> CalcolaInternoAsync(
            string descrizione1,
            string descrizione2,
            bool filtroPraticheAttivo,
            string operatoreFiltri,
            List<PraticaFiltrata_IN> pratiche,
            AgronicaCoreParametriDouble objParametriDouble)
        {
            var daCapostipiti = await LeggiFlagCapostipitiAsync(objParametriDouble.ObjParametriServer);
            var applyPratiche = filtroPraticheAttivo;

            HashSet<string> piveGerarchia;
            HashSet<Tuple<string, int>> centriGerarchia;

            if (daCapostipiti)
            {
                (piveGerarchia, centriGerarchia) = await CalcolaGerarchiaDaCapostipitiAsync(descrizione1, objParametriDouble.ObjParametriServer);
            }
            else
            {
                var dtGerarchiaPive = await _praticheDal.LeggiPiveGerarchiaAsync(descrizione2, objParametriDouble.ObjParametriServer);
                var dtGerarchiaCentri = await _praticheDal.LeggiCentriGerarchiaAsync(descrizione2, objParametriDouble.ObjParametriServer);
                piveGerarchia = ReadPive(dtGerarchiaPive);
                centriGerarchia = ReadCentri(dtGerarchiaCentri);
            }

            HashSet<string> pivePratiche = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<Tuple<string, int>> centriPraticheOnly = new HashSet<Tuple<string, int>>();

            if (applyPratiche && pratiche != null && pratiche.Count > 0)
            {
                var praticheProfilo = pratiche
                    .Select(p => new PraticaProfilo(p.Servizio_Cod, p.ConsideraValiditaTemporale))
                    .ToList();

                var dtPratiche = await _praticheDal.LeggiPiveVisibiliAsync(
                    objParametriDouble.ObjParametriServer.PivaSuperUser,
                    praticheProfilo,
                    objParametriDouble.ObjParametriServer);

                pivePratiche = ReadPive(dtPratiche);

                var piveOnlyPratiche = pivePratiche.Except(piveGerarchia, StringComparer.OrdinalIgnoreCase).ToList();
                if (piveOnlyPratiche.Count > 0)
                {
                    var dtCentriPratiche = await _praticheDal.LeggiCentriPerPiveAsync(
                        piveOnlyPratiche,
                        objParametriDouble.ObjParametriServer);
                    centriPraticheOnly = ReadCentri(dtCentriPratiche);
                }
            }

            var piveFinali = applyPratiche
                ? VisibilitaCombinator.Combina(piveGerarchia, pivePratiche, operatoreFiltri)
                : piveGerarchia;

            var centriFinali = applyPratiche
                ? VisibilitaCombinator.CombinaCentri(centriGerarchia, centriPraticheOnly, pivePratiche, operatoreFiltri)
                : centriGerarchia;

            return new CalcoloVisibilita
            {
                PiveFinali = piveFinali,
                CentriFinali = centriFinali
            };
        }

        private async Task<(HashSet<string> pive, HashSet<Tuple<string, int>> centri)> CalcolaGerarchiaDaCapostipitiAsync(
            string descrizione1,
            AgronicaCoreParametriServer objParametriServer)
        {
            var piveSeed = CapostipitiHelper.ExtractPive(descrizione1);
            if (CapostipitiHelper.IsVisibilitaTotaleONulla(piveSeed))
            {
                return (new HashSet<string>(StringComparer.OrdinalIgnoreCase), new HashSet<Tuple<string, int>>());
            }

            var dt = await _praticheDal.LeggiCapostipitiEspansioneAsync(piveSeed, objParametriServer);
            var pive = ReadPive(dt);
            var centri = new HashSet<Tuple<string, int>>();
            if (dt != null && dt.Columns.Contains("Piva") && dt.Columns.Contains("Sa_Cod"))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Piva"] == DBNull.Value || row["Sa_Cod"] == DBNull.Value) continue;
                    var piva = Convert.ToString(row["Piva"]);
                    if (string.IsNullOrWhiteSpace(piva)) continue;
                    var saCod = Convert.ToInt32(row["Sa_Cod"]);
                    if (saCod > 0)
                        centri.Add(Tuple.Create(piva.Trim(), saCod));
                }
            }
            return (pive, centri);
        }

        // Replica Utenti.vb:622-627: legge Configurazione_Siti.Valore (Sito_Cod=0).
        // Valore vuoto/null/"1" -> capostipiti (path PopolaVisibilitaAppoggioXUtente).
        // Qualsiasi altro valore esplicito -> Filtrone su Descrizione_2.
        private async Task<bool> LeggiFlagCapostipitiAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var dt = await _securityLayerDAL.LeggiConfigurazioneSitiAsync(CfgKeyCapostipiti, objParametriServer);
            string raw = string.Empty;
            if (dt != null && dt.Rows.Count > 0)
                raw = dt.Rows[0]["Valore"]?.ToString() ?? string.Empty;

            return string.IsNullOrWhiteSpace(raw) || raw.Trim() == "1";
        }

        private static DataTable BuildDataTableImprese(string pivaSuperUser, string username, HashSet<string> pive)
        {
            var dt = BuildSchemaUVA();
            foreach (var piva in pive)
            {
                var row = dt.NewRow();
                row["PivaSuperUser"] = pivaSuperUser;
                row["Username"] = username;
                row["Entita_Cod"] = (int)TipiEnumerativi.Enum_TipoEntita.Impresa;
                row["Piva"] = piva;
                row["Sa_Cod"] = 0;
                row["Appezza"] = 0;
                row["Id_Reg"] = 0;
                dt.Rows.Add(row);
            }
            return dt;
        }

        private static DataTable BuildDataTableCentri(string pivaSuperUser, string username, HashSet<Tuple<string, int>> centri)
        {
            var dt = BuildSchemaUVA();
            foreach (var c in centri)
            {
                var row = dt.NewRow();
                row["PivaSuperUser"] = pivaSuperUser;
                row["Username"] = username;
                row["Entita_Cod"] = (int)TipiEnumerativi.Enum_TipoEntita.Centro;
                row["Piva"] = c.Item1;
                row["Sa_Cod"] = c.Item2;
                row["Appezza"] = 0;
                row["Id_Reg"] = 0;
                dt.Rows.Add(row);
            }
            return dt;
        }

        private static DataTable BuildSchemaUVA()
        {
            var dt = new DataTable();
            dt.Columns.Add("PivaSuperUser", typeof(string));
            dt.Columns.Add("Username", typeof(string));
            dt.Columns.Add("Entita_Cod", typeof(int));
            dt.Columns.Add("Piva", typeof(string));
            dt.Columns.Add("Sa_Cod", typeof(int));
            dt.Columns.Add("Appezza", typeof(int));
            dt.Columns.Add("Id_Reg", typeof(int));
            return dt;
        }

        private static HashSet<string> ReadPive(DataTable dt)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (dt == null || !dt.Columns.Contains("Piva"))
                return result;

            foreach (DataRow row in dt.Rows)
            {
                if (row["Piva"] != DBNull.Value)
                {
                    var piva = Convert.ToString(row["Piva"]);
                    if (!string.IsNullOrWhiteSpace(piva))
                        result.Add(piva.Trim());
                }
            }

            return result;
        }

        private static HashSet<Tuple<string, int>> ReadCentri(DataTable dt)
        {
            var result = new HashSet<Tuple<string, int>>();
            if (dt == null || !dt.Columns.Contains("Piva") || !dt.Columns.Contains("Sa_Cod"))
                return result;

            foreach (DataRow row in dt.Rows)
            {
                if (row["Piva"] == DBNull.Value || row["Sa_Cod"] == DBNull.Value)
                    continue;

                var piva = Convert.ToString(row["Piva"]);
                if (!string.IsNullOrWhiteSpace(piva))
                    result.Add(Tuple.Create(piva.Trim(), Convert.ToInt32(row["Sa_Cod"])));
            }

            return result;
        }

        private class CalcoloVisibilita
        {
            public HashSet<string> PiveFinali { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public HashSet<Tuple<string, int>> CentriFinali { get; set; } = new HashSet<Tuple<string, int>>();
        }
    }
}
