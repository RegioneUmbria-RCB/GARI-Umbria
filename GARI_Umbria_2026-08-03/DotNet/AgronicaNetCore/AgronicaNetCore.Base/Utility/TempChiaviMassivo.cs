using System.Dynamic;
using System.Text;
using AgronicaDataProvider6.Utils;
using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Base.Utility
{
    public class TempChiaviMassivo : DAL_Base
    {
        // RICORDARSI DI APRIRE E CHIUDERE LA CONNESSIONE ANCHE IN CASO DI EXCEPTION
        // APERTURA CONNESSIONE--> AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(true, objParametri)
        // CHIUSURA TRANSAZIONE OK --> AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri)
        // CHIUSURA TRANSAZIONE KO --> AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
        // CHIUSURA CONNESSIONE --> AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)

        // SQL Server hard limit is 2100 parameters per statement.
        // Use 2000 to stay safely below it.
        private const int SqlServerMaxParams = 2000;

        private static int GetChunkSize(int numColumns) =>
            Math.Max(1, SqlServerMaxParams / numColumns);

        public TempChiaviMassivo(IServiceProvider provider, bool securityServiceBypass = false)
            : base(provider, securityServiceBypass) { }

        #region Filtro Piva
        public async Task CreaTabellaTemp_FiltroPiva(
            List<string> chiaviList,
            AgronicaCoreParametriServer objParametri
        )
        {
            if (chiaviList == null || !chiaviList.Any())
                throw new Exception("chiaviList obbligatorio per le letture massive!");

            var stb = new StringBuilder();

            stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempPiva') IS NULL BEGIN ");
            stb.AppendLine("    CREATE TABLE #TempPiva ( ");
            stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ");
            stb.AppendLine("    )");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());

            stb.Length = 0;
            if (chiaviList != null && chiaviList.Any())
            {
                var chunks = ListExtension.ChunkBy<string>(chiaviList, GetChunkSize(1));
                foreach (var chunk in chunks)
                {
                    var expandoObj = new ExpandoObject();
                    stb.Append("INSERT INTO #TempPiva (Piva) VALUES ");
                    int _idx = 0;
                    foreach (string p in chunk)
                    {
                        string _pn = $"@p{_idx}";
                        if (_idx > 0)
                            stb.Append(",");
                        stb.Append($"({_pn})");
                        expandoObj.TryAdd(_pn, p);
                        _idx++;
                    }
                    await GetDataProvider(objParametri)
                        .Execute_WriteAsync(stb.ToString(), expandoObj);
                    stb.Clear();
                }
            }
        }

        public async Task EliminaTabellaTemp_FiltroPiva(AgronicaCoreParametriServer objParametri)
        {
            var stb = new StringBuilder();

            stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempPiva') IS NULL BEGIN ");
            stb.AppendLine("    DROP TABLE #TempPiva  ");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());
        }
        #endregion

        #region Filtro Appezzamento
        public async Task CreaTabellaTemp_FiltroAppezzamenti(
            List<(string, int, int)> chiaviList,
            AgronicaCoreParametriServer objParametri
        )
        {
            if (chiaviList == null || !chiaviList.Any())
                throw new Exception("chiaviList obbligatorio per le letture massive!");

            var stb = new StringBuilder();

            stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempAppezzamento') IS NULL BEGIN ");
            stb.AppendLine("    CREATE TABLE #TempAppezzamento ( ");
            stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ");
            stb.AppendLine("      , Sa_Cod INT NULL");
            stb.AppendLine("      , Appezza INT NULL");
            stb.AppendLine("    )");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());

            stb.Length = 0;

            if (chiaviList != null && chiaviList.Any())
            {
                var chunks = ListExtension.ChunkBy<(string, int, int)>(chiaviList, GetChunkSize(3));
                foreach (var chunk in chunks)
                {
                    var expandoObj = new ExpandoObject();
                    stb.Append("INSERT INTO #TempAppezzamento (Piva, Sa_Cod, Appezza) VALUES ");
                    int _idx = 0;
                    foreach (var p in chunk)
                    {
                        string _p0 = $"@p{_idx}_0",
                            _p1 = $"@p{_idx}_1",
                            _p2 = $"@p{_idx}_2";
                        if (_idx > 0)
                            stb.Append(",");
                        stb.Append($"({_p0},{_p1},{_p2})");
                        expandoObj.TryAdd(_p0, p.Item1);
                        expandoObj.TryAdd(_p1, p.Item2);
                        expandoObj.TryAdd(_p2, p.Item3);
                        _idx++;
                    }
                    await GetDataProvider(objParametri)
                        .Execute_WriteAsync(stb.ToString(), expandoObj);
                    stb.Clear();
                }
            }
        }

        public async Task EliminaTabellaTemp_FiltroAppezzamenti(
            AgronicaCoreParametriServer objParametri
        )
        {
            var stb = new StringBuilder();

            stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempAppezzamento') IS NULL BEGIN ");
            stb.AppendLine("    DROP TABLE #TempAppezzamento ");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());
        }
        #endregion

        #region Filtro Impianto
        public async Task CreaTabellaTemp_FiltroImpianti(
            List<(string, int, int, int)> chiaviList,
            AgronicaCoreParametriServer objParametri
        )
        {
            var stb = new StringBuilder();

            stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempImpianto') IS NULL BEGIN ");
            stb.AppendLine("    CREATE TABLE #TempImpianto ( ");
            stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ");
            stb.AppendLine("      , Sa_Cod INT NULL");
            stb.AppendLine("      , Appezza INT NULL");
            stb.AppendLine("      , Id_Reg INT NULL");
            stb.AppendLine("    )");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());

            stb.Length = 0;

            if (chiaviList != null && chiaviList.Any())
            {
                var chunks = ListExtension.ChunkBy<(string, int, int, int)>(
                    chiaviList,
                    GetChunkSize(4)
                );
                foreach (var chunk in chunks)
                {
                    var expandoObj = new ExpandoObject();
                    stb.Append("INSERT INTO #TempImpianto (Piva, Sa_Cod, Appezza, Id_Reg) VALUES ");
                    int _idx = 0;
                    foreach (var p in chunk)
                    {
                        string _p0 = $"@p{_idx}_0",
                            _p1 = $"@p{_idx}_1",
                            _p2 = $"@p{_idx}_2",
                            _p3 = $"@p{_idx}_3";
                        if (_idx > 0)
                            stb.Append(",");
                        stb.Append($"({_p0},{_p1},{_p2},{_p3})");
                        expandoObj.TryAdd(_p0, p.Item1);
                        expandoObj.TryAdd(_p1, p.Item2);
                        expandoObj.TryAdd(_p2, p.Item3);
                        expandoObj.TryAdd(_p3, p.Item4);
                        _idx++;
                    }
                    await GetDataProvider(objParametri)
                        .Execute_WriteAsync(stb.ToString(), expandoObj);
                    stb.Clear();
                }
            }
        }

        public async Task CreaTabellaTemp_FiltroImpianti_ConDate(List<(string, int, int, int, DateTime)> chiaviList,
            AgronicaCoreParametriServer objParametri)
        {
            var stb = new StringBuilder();

            stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempImpianto') IS NULL BEGIN ");
            stb.AppendLine("    CREATE TABLE #TempImpianto ( ");
            stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ");
            stb.AppendLine("      , Sa_Cod INT NULL");
            stb.AppendLine("      , Appezza INT NULL");
            stb.AppendLine("      , Id_Reg INT NULL");
            stb.AppendLine("      , Data DATETIME NULL");
            stb.AppendLine("    )");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());

            stb.Length = 0;

            if (chiaviList != null && chiaviList.Any())
            {
                var chunks = ListExtension.ChunkBy(
                    chiaviList,
                    GetChunkSize(5)
                );
                foreach (var chunk in chunks)
                {
                    var expandoObj = new ExpandoObject();
                    stb.Append("INSERT INTO #TempImpianto (Piva, Sa_Cod, Appezza, Id_Reg, Data) VALUES ");
                    int _idx = 0;
                    foreach (var p in chunk)
                    {
                        string _p0 = $"@p{_idx}_0",
                            _p1 = $"@p{_idx}_1",
                            _p2 = $"@p{_idx}_2",
                            _p3 = $"@p{_idx}_3",
                            _p4 = $"@p{_idx}_4";
                        if (_idx > 0)
                            stb.Append(",");
                        stb.Append($"({_p0},{_p1},{_p2},{_p3},{_p4})");
                        expandoObj.TryAdd(_p0, p.Item1);
                        expandoObj.TryAdd(_p1, p.Item2);
                        expandoObj.TryAdd(_p2, p.Item3);
                        expandoObj.TryAdd(_p3, p.Item4);
                        expandoObj.TryAdd(_p4, p.Item5);
                        _idx++;
                    }
                    await GetDataProvider(objParametri)
                        .Execute_WriteAsync(stb.ToString(), expandoObj);
                    stb.Clear();
                }
            }
        }

        public async Task EliminaTabellaTemp_FiltroImpianti(
            AgronicaCoreParametriServer objParametri
        )
        {
            var stb = new StringBuilder();

            stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempImpianto') IS NULL BEGIN ");
            stb.AppendLine("    DROP TABLE #TempImpianto ");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());
        }
        #endregion

        #region Filtro Esercizio
        public async Task CreaTabellaTemp_FiltroEsercizi(
            List<(string, int, int, int, int)> chiaviList,
            AgronicaCoreParametriServer objParametri
        )
        {
            if (chiaviList == null || !chiaviList.Any())
                throw new Exception("chiaviList obbligatorio per le letture massive!");

            var stb = new StringBuilder();

            stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempEsercizio') IS NULL BEGIN ");
            stb.AppendLine("    CREATE TABLE #TempEsercizio ( ");
            stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ");
            stb.AppendLine("      , Sa_Cod INT NULL");
            stb.AppendLine("      , Appezza INT NULL");
            stb.AppendLine("      , Id_Reg INT NULL");
            stb.AppendLine("      , Progetto_Cod INT NULL");
            stb.AppendLine("    )");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());

            stb.Length = 0;

            if (chiaviList != null && chiaviList.Any())
            {
                var chunks = ListExtension.ChunkBy<(string, int, int, int, int)>(
                    chiaviList,
                    GetChunkSize(5)
                );
                foreach (var chunk in chunks)
                {
                    var expandoObj = new ExpandoObject();
                    stb.Append(
                        "INSERT INTO #TempEsercizio (Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod) VALUES "
                    );
                    int _idx = 0;
                    foreach (var p in chunk)
                    {
                        string _p0 = $"@p{_idx}_0",
                            _p1 = $"@p{_idx}_1",
                            _p2 = $"@p{_idx}_2",
                            _p3 = $"@p{_idx}_3",
                            _p4 = $"@p{_idx}_4";
                        if (_idx > 0)
                            stb.Append(",");
                        stb.Append($"({_p0},{_p1},{_p2},{_p3},{_p4})");
                        expandoObj.TryAdd(_p0, p.Item1);
                        expandoObj.TryAdd(_p1, p.Item2);
                        expandoObj.TryAdd(_p2, p.Item3);
                        expandoObj.TryAdd(_p3, p.Item4);
                        expandoObj.TryAdd(_p4, p.Item5);
                        _idx++;
                    }
                    await GetDataProvider(objParametri)
                        .Execute_WriteAsync(stb.ToString(), expandoObj);
                    stb.Clear();
                }
            }
        }

        public async Task EliminaTabellaTemp_FiltroEsercizi(
            AgronicaCoreParametriServer objParametri
        )
        {
            var stb = new StringBuilder();

            stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempEsercizio') IS NULL BEGIN ");
            stb.AppendLine("    DROP TABLE #TempEsercizio ");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());
        }

        public async Task CreaTabellaTemp_FiltroProgetto(
            List<int> chiaviList,
            AgronicaCoreParametriServer objParametri
        )
        {
            if (chiaviList == null || !chiaviList.Any())
                throw new Exception("chiaviList obbligatorio per le letture massive!");

            var stb = new StringBuilder();

            stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempProgetto') IS NULL BEGIN ");
            stb.AppendLine("    CREATE TABLE #TempProgetto ( ");
            stb.AppendLine("        Progetto_Cod INT NULL");
            stb.AppendLine("    )");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());

            stb.Length = 0;

            if (chiaviList != null && chiaviList.Any())
            {
                var chunks = ListExtension.ChunkBy<int>(chiaviList, GetChunkSize(1));
                foreach (var chunk in chunks)
                {
                    var expandoObj = new ExpandoObject();
                    stb.Append("INSERT INTO #TempProgetto (Progetto_Cod) VALUES ");
                    int _idx = 0;
                    foreach (int p in chunk)
                    {
                        string _pn = $"@p{_idx}";
                        if (_idx > 0)
                            stb.Append(",");
                        stb.Append($"({_pn})");
                        expandoObj.TryAdd(_pn, p);
                        _idx++;
                    }
                    await GetDataProvider(objParametri)
                        .Execute_WriteAsync(stb.ToString(), expandoObj);
                    stb.Clear();
                }
            }
        }

        public async Task EliminaTabellaTemp_FiltroProgetto(
            AgronicaCoreParametriServer objParametri
        )
        {
            var stb = new StringBuilder();

            stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempProgetto') IS NULL BEGIN ");
            stb.AppendLine("    DROP TABLE #TempProgetto ");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());
        }

        #endregion

        #region Filtro Fabbricato
        public async Task CreaTabellaTemp_FiltroFabbricati(
            List<(string, int, int)> chiaviList,
            AgronicaCoreParametriServer objParametri
        )
        {
            if (chiaviList == null || !chiaviList.Any())
                throw new Exception("chiaviList obbligatorio per le letture massive!");

            var stb = new StringBuilder();

            stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempFabbricato') IS NULL BEGIN ");
            stb.AppendLine("    CREATE TABLE #TempFabbricato ( ");
            stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ");
            stb.AppendLine("      , Sa_Cod INT NULL");
            stb.AppendLine("      , Fabbricato_Cod INT NULL");
            stb.AppendLine("    )");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());

            stb.Length = 0;

            if (chiaviList != null && chiaviList.Any())
            {
                var chunks = ListExtension.ChunkBy<(string, int, int)>(chiaviList, GetChunkSize(3));
                foreach (var chunk in chunks)
                {
                    var expandoObj = new ExpandoObject();
                    stb.Append(
                        "INSERT INTO #TempFabbricato (Piva, Sa_Cod, Fabbricato_Cod) VALUES "
                    );
                    int _idx = 0;
                    foreach (var p in chunk)
                    {
                        string _p0 = $"@p{_idx}_0",
                            _p1 = $"@p{_idx}_1",
                            _p2 = $"@p{_idx}_2";
                        if (_idx > 0)
                            stb.Append(",");
                        stb.Append($"({_p0},{_p1},{_p2})");
                        expandoObj.TryAdd(_p0, p.Item1);
                        expandoObj.TryAdd(_p1, p.Item2);
                        expandoObj.TryAdd(_p2, p.Item3);
                        _idx++;
                    }
                    await GetDataProvider(objParametri)
                        .Execute_WriteAsync(stb.ToString(), expandoObj);
                    stb.Clear();
                }
            }
        }

        public async Task EliminaTabellaTemp_FiltroFabbricati(
            AgronicaCoreParametriServer objParametri
        )
        {
            var stb = new StringBuilder();

            stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempFabbricato') IS NULL BEGIN ");
            stb.AppendLine("    DROP TABLE #TempFabbricato ");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());
        }
        #endregion

        #region Filtro Generico Chiave Stringa
        public async Task CreaTabellaTemp_FiltroChiaveStringa(
            List<string> chiaviList,
            AgronicaCoreParametriServer objParametri,
            int varcharSize = 50
        )
        {
            if (chiaviList == null || !chiaviList.Any())
                throw new Exception("chiaviList obbligatorio per le letture massive!");

            var stb = new StringBuilder();

            stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempChiave') IS NULL BEGIN ");
            stb.AppendLine("    CREATE TABLE #TempChiave ( ");
            stb.AppendLine($"        chiave varchar({varcharSize}) COLLATE DATABASE_DEFAULT NULL ");
            stb.AppendLine("    )");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());

            stb.Length = 0;

            if (chiaviList != null && chiaviList.Any())
            {
                var chunks = ListExtension.ChunkBy<string>(chiaviList, GetChunkSize(1));
                foreach (var chunk in chunks)
                {
                    var expandoObj = new ExpandoObject();
                    stb.Append("INSERT INTO #TempChiave (chiave) VALUES ");
                    int _idx = 0;
                    foreach (string p in chunk)
                    {
                        string _pn = $"@p{_idx}";
                        if (_idx > 0)
                            stb.Append(",");
                        stb.Append($"({_pn})");
                        expandoObj.TryAdd(_pn, p);
                        _idx++;
                    }
                    await GetDataProvider(objParametri)
                        .Execute_WriteAsync(stb.ToString(), expandoObj);
                    stb.Clear();
                }
            }
        }

        public async Task EliminaTabellaTemp_FiltroChiaveStringa(
            AgronicaCoreParametriServer objParametri
        )
        {
            var stb = new StringBuilder();

            stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempChiave') IS NULL BEGIN ");
            stb.AppendLine("    DROP TABLE #TempChiave ");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());
        }
        #endregion

        #region Filtro Generico Chiave Stringa
        public async Task CreaTabellaTemp_FiltroChiaveInt(
            List<int> chiaviList,
            AgronicaCoreParametriServer objParametri
        )
        {
            if (chiaviList == null || !chiaviList.Any())
                throw new Exception("chiaviList obbligatorio per le letture massive!");

            var stb = new StringBuilder();

            stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempChiaveInt') IS NULL BEGIN ");
            stb.AppendLine("    CREATE TABLE #TempChiaveInt ( ");
            stb.AppendLine($"        chiave INT NULL ");
            stb.AppendLine("    )");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());

            stb.Length = 0;

            if (chiaviList != null && chiaviList.Any())
            {
                var chunks = ListExtension.ChunkBy<int>(chiaviList, GetChunkSize(1));
                foreach (var chunk in chunks)
                {
                    var expandoObj = new ExpandoObject();
                    stb.Append("INSERT INTO #TempChiaveInt (chiave) VALUES ");
                    int _idx = 0;
                    foreach (int p in chunk)
                    {
                        string _pn = $"@p{_idx}";
                        if (_idx > 0)
                            stb.Append(",");
                        stb.Append($"({_pn})");
                        expandoObj.TryAdd(_pn, p);
                        _idx++;
                    }
                    await GetDataProvider(objParametri)
                        .Execute_WriteAsync(stb.ToString(), expandoObj);
                    stb.Clear();
                }
            }
        }

        public async Task EliminaTabellaTemp_FiltroChiaveInt(
            AgronicaCoreParametriServer objParametri
        )
        {
            var stb = new StringBuilder();

            stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempChiaveInt') IS NULL BEGIN ");
            stb.AppendLine("    DROP TABLE #TempChiaveInt ");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());
        }
        #endregion

        #region Filtro Centro
        public async Task CreaTabellaTemp_FiltroCentro(
            List<(string, int)> chiaviList,
            AgronicaCoreParametriServer objParametri
        )
        {
            if (chiaviList == null || !chiaviList.Any())
                throw new Exception("chiaviList obbligatorio per le letture massive!");

            var stb = new StringBuilder();

            stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempCentro') IS NULL BEGIN ");
            stb.AppendLine("    CREATE TABLE #TempCentro ( ");
            stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ");
            stb.AppendLine("      , Sa_Cod INT NULL");
            stb.AppendLine("    )");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());

            stb.Length = 0;

            if (chiaviList != null && chiaviList.Any())
            {
                var chunks = ListExtension.ChunkBy<(string, int)>(chiaviList, GetChunkSize(2));
                foreach (var chunk in chunks)
                {
                    var expandoObj = new ExpandoObject();
                    stb.Append("INSERT INTO #TempCentro (Piva, Sa_Cod) VALUES ");
                    int _idx = 0;
                    foreach (var p in chunk)
                    {
                        string _p0 = $"@p{_idx}_0",
                            _p1 = $"@p{_idx}_1";
                        if (_idx > 0)
                            stb.Append(",");
                        stb.Append($"({_p0},{_p1})");
                        expandoObj.TryAdd(_p0, p.Item1);
                        expandoObj.TryAdd(_p1, p.Item2);
                        _idx++;
                    }
                    await GetDataProvider(objParametri)
                        .Execute_WriteAsync(stb.ToString(), expandoObj);
                    stb.Clear();
                }
            }
        }

        public async Task EliminaTabellaTemp_FiltroCentro(AgronicaCoreParametriServer objParametri)
        {
            var stb = new StringBuilder();

            stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempCentro') IS NULL BEGIN ");
            stb.AppendLine("    DROP TABLE #TempCentro ");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());
        }
        #endregion

        #region Filtro Contatto
        public async Task CreaTabellaTemp_FiltroContatto(
            List<(string, int)> chiaviList,
            AgronicaCoreParametriServer objParametri
        )
        {
            if (chiaviList == null || !chiaviList.Any())
                throw new Exception("chiaviList obbligatorio per le letture massive!");

            var stb = new StringBuilder();

            stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempContatto') IS NULL BEGIN ");
            stb.AppendLine("    CREATE TABLE #TempContatto ( ");
            stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ");
            stb.AppendLine("      , Cod_Contatto INT NULL");
            stb.AppendLine("    )");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());

            stb.Length = 0;

            if (chiaviList != null && chiaviList.Any())
            {
                var chunks = ListExtension.ChunkBy<(string, int)>(chiaviList, GetChunkSize(2));
                foreach (var chunk in chunks)
                {
                    var expandoObj = new ExpandoObject();
                    stb.Append("INSERT INTO #TempContatto (Piva, Cod_Contatto) VALUES ");
                    int _idx = 0;
                    foreach (var p in chunk)
                    {
                        string _p0 = $"@p{_idx}_0",
                            _p1 = $"@p{_idx}_1";
                        if (_idx > 0)
                            stb.Append(",");
                        stb.Append($"({_p0},{_p1})");
                        expandoObj.TryAdd(_p0, p.Item1);
                        expandoObj.TryAdd(_p1, p.Item2);
                        _idx++;
                    }
                    await GetDataProvider(objParametri)
                        .Execute_WriteAsync(stb.ToString(), expandoObj);
                    stb.Clear();
                }
            }
        }

        public async Task EliminaTabellaTemp_FiltroContatto(
            AgronicaCoreParametriServer objParametri
        )
        {
            var stb = new StringBuilder();

            stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempContatto') IS NULL BEGIN ");
            stb.AppendLine("    DROP TABLE #TempContatto ");
            stb.AppendLine(" END ");

            await GetDataProvider(objParametri).Execute_WriteAsync(stb.ToString());
        }
        #endregion
    }
}
