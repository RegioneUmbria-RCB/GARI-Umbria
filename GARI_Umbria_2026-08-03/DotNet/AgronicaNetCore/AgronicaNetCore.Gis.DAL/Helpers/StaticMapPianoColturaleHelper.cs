using System.Data;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Gis.Shared;
using AgronicaNetCore.Gis.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.Gis.DAL.Helpers
{
    public class StaticMapPianoColturaleHelper : IStaticMapPianoColturaleHelper
    {
        private readonly IGisElementiGrafici _gisElementiGrafici;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggingService _loggingService;

        public StaticMapPianoColturaleHelper(
            IServiceProvider serviceProvider,
            IGisElementiGrafici gisElementiGrafici
        )
        {
            _serviceProvider = serviceProvider;
            _gisElementiGrafici = gisElementiGrafici;
            _loggingService = serviceProvider.GetRequiredService<ILoggingService>();
        }

        /// <summary>
        /// Per ogni impianto nel DataTable privo di StaticMapBase64String ma con cartografia,
        /// genera la mappa statica tramite <see cref="GoogleStaticMaps"/> e aggiorna la colonna
        /// StaticMapBase64String in tutte le righe corrispondenti (stesso piva/sa_cod/appezza).
        /// </summary>
        public async Task GeneraStaticMapDaSincroApp(
            DataTable dtImpianti,
            GeneraMappaStaticaInData staticMapCfg,
            AgronicaCoreParametri objParametriServer
        )
        {
            try
            {
                // Raggruppa per chiave impianto (piva, sa_cod, appezza, id_reg):
                // righe senza immagine, con entita_cod valido e con cartografia presente
                var impiantiDaAggiornare = dtImpianti
                    .AsEnumerable()
                    .Where(row =>
                        (
                            row.IsNull("StaticMapBase64String")
                            || string.IsNullOrEmpty(row["StaticMapBase64String"].ToString())
                        )
                        && !row.IsNull("entita_cod")
                        && (int)row["entita_cod"] != 0
                        && !row.IsNull("cartografia")
                        && !string.IsNullOrEmpty(row["cartografia"].ToString())
                    )
                    .GroupBy(row =>
                        (
                            (string)row["piva"],
                            (int)row["sa_cod"],
                            (int)row["appezza"],
                            (int)row["id_reg"]
                        )
                    )
                    .ToDictionary(
                        g => g.Key,
                        g =>
                            (
                                EntitaCod: (int)g.First()["entita_cod"],
                                Geo: g.First().Field<string>("geo")
                            )
                    );

                if (impiantiDaAggiornare.Count == 0)
                    return;

                var gestioneStaticMaps = new GoogleStaticMaps(
                    _serviceProvider,
                    _gisElementiGrafici
                );

                foreach (var impianto in impiantiDaAggiornare)
                {
                    staticMapCfg.EntitaCod = impianto.Value.EntitaCod;
                    staticMapCfg.StrWkt = impianto.Value.Geo;
                    staticMapCfg.ObjParametriServer = objParametriServer;

                    byte[]? bytesMappa =
                        await gestioneStaticMaps.AggiornaElementoGraficoConMappaStaticaAsync(
                            staticMapCfg
                        );

                    if (bytesMappa is null)
                    {
                        _loggingService.LogWarning(
                            $"Anomalia nella generazione della staticMap per Entita_Cod {impianto.Value.EntitaCod}",
                            objParametriServer
                        );
                        continue;
                    }

                    var image = Convert.ToBase64String(bytesMappa);

                    // Aggiorna StaticMapBase64String su tutte le righe con la stessa chiave impianto
                    var rowsToUpdate = dtImpianti
                        .AsEnumerable()
                        .Where(row =>
                            (string)row["piva"] == impianto.Key.Item1
                            && (int)row["sa_cod"] == impianto.Key.Item2
                            && (int)row["appezza"] == impianto.Key.Item3
                            && (int)row["id_reg"] == impianto.Key.Item4
                        )
                        .ToList();

                    foreach (var rowToUpdate in rowsToUpdate)
                        rowToUpdate["StaticMapBase64String"] = image;
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError(
                    $"Errore nella generazione della StaticMap: {ex.Message}",
                    objParametriServer,
                    ex
                );
            }
        }
    }
}

public class GoogleStaticMaps : Serilog_Base
{
    private readonly IGisElementiGrafici _gisElementiGrafici;
    private readonly CallHttp _callHttp;

    public GoogleStaticMaps(
        IServiceProvider serviceProvider,
        IGisElementiGrafici gisElementiGrafici
    )
        : base(serviceProvider)
    {
        _gisElementiGrafici = gisElementiGrafici;
        _callHttp = new CallHttp(serviceProvider.GetRequiredService<ILoggingService>());
    }

    /// <summary>
    /// Genera la mappa statica per l'impianto, la persiste su DB e restituisce i byte PNG.
    /// Restituisce <c>null</c> se la geometria non è un poligono o la generazione fallisce.
    /// </summary>
    public async Task<byte[]?> AggiornaElementoGraficoConMappaStaticaAsync(
        GeneraMappaStaticaInData staticMap
    )
    {
        byte[]? myBytes = null;
        try
        {
            if (staticMap.EntitaCod == 0)
                throw new ArgumentException("Static map: Missing EntitaCod", nameof(staticMap));

            GeneraMappaStaticaVerifyParam(staticMap);

            if (staticMap.StrWkt is null || !staticMap.StrWkt.Contains("POLYGON"))
                throw new InvalidOperationException("Static map: Not a polygon");

            myBytes = await GeneraMappaStaticaComeByteArrayAsync(staticMap);

            if (myBytes is null || myBytes.Length == 0)
                return null;

            await _gisElementiGrafici.AggiornaStaticMapAsync(
                staticMap.EntitaCod,
                myBytes,
                staticMap.ObjParametriServer!
            );

            if (!string.IsNullOrEmpty(staticMap.DebugImgPathPerDump))
            {
                var dir = staticMap.DebugImgPathPerDump;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var filePath = Path.Combine(dir, $"{Guid.NewGuid():N}.png");
                File.WriteAllBytes(filePath, myBytes);
            }

            return myBytes;
        }
        catch (Exception ex)
        {
            LogError(
                "Errore nella generazione della StaticMap da SincroAPP: " + ex.Message,
                staticMap.ObjParametriServer,
                ex
            );
        }

        return myBytes;
    }

    private static void GeneraMappaStaticaVerifyParam(GeneraMappaStaticaInData inData)
    {
        if (inData.EntitaCod == 0 && string.IsNullOrEmpty(inData.StrWkt))
            throw new InvalidOperationException("Static map: Missing WKT");

        if (string.IsNullOrEmpty(inData.MapsApikey))
            throw new InvalidOperationException("Static map: Missing Gmaps api key");
    }

    private async Task<byte[]?> GeneraMappaStaticaComeByteArrayAsync(
        GeneraMappaStaticaInData staticMap
    )
    {
        // Esempio URL: https://maps.googleapis.com/maps/api/staticmap?size=400x400
        //   &maptype=satellite&path=fillcolor:0xAA000033%7Ccolor:0xFFFFFF00%7Cenc:___DATA___&key=YOUR_API_KEY
        var urlToGet =
            $"https://maps.googleapis.com/maps/api/staticmap?size={staticMap.SizeX}x{staticMap.SizeY}"
            + $"&maptype={staticMap.TipoMappaSfondo}"
            + $"&path=fillcolor:{staticMap.FillColor}%7Ccolor:{staticMap.Color}%7Cenc:___DATA___"
            + $"&key={staticMap.MapsApikey}";

        var polylineEncoded = GoogleStaticMapsConverter.GeneraEncodedPolyline(staticMap.StrWkt!);

        urlToGet = urlToGet.Replace("___DATA___", polylineEncoded);

        if (!string.IsNullOrEmpty(staticMap.SignPrivateKey))
            urlToGet = GoogleStaticMapsConverter.Sign(urlToGet, staticMap.SignPrivateKey);

        return await _callHttp.ReadByteArrayFromUrlGetAsync(
            urlToGet,
            staticMap.TimeoutRequestInSeconds,
            staticMap.ObjParametriServer
        );
    }
}
