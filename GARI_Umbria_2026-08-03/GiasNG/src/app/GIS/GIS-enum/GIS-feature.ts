export enum enum_FeatureProperty {
    id = 'id',
    layer = 'layer',
    entitaCod = 'Entita_Cod',
    chiaveAlbero = 'chiavealbero',
    etichetta = 'etichetta',
    layerAppartenenza = 'StandardEntita_layerDiAppartenenza',
    inserimento = 'inserimento',
    modifica = 'modifica',
    cancellazione = 'cancellazione',
    informazioni = 'informazioni',
    flagGps = 'flag_gps',
    nodeInfo = 'nodeInfo',
    zIndex = 'zindex',
    tipologiaGML = 'TipologiaGML',
    inOsservazione = 'InOsservazione',
    coloreRetinatura = 'Colore_Retinatura',
    trasparenza = 'Trasparenza',
    testo = 'Testo',
    featureTypeId = 'FeatureTypeId',
    tipoIcona = 'tipoicona',
    layerAppartenenzaDes = 'StandardEntita_layerDiAppartenenza_Des',
    layerAppartenenzaIcona32 = 'StandardEntita_layerDiAppartenenza_Icona32',
    vegCod = 'veg_cod',
    appIdRate = 'AppIdRate',
    colorePrimario = 'Colore_Primario',
    entitaGuid = 'Entita_GUID',
    parametriVisualizzazioneLayer = 'ParametriVisualizzazioneLayer',

    retinatura = 'Retinatura',
    Area_Cod = 'Area_Cod',
    GMapsZoomLevel = "GMapsZoomLevel",
    TotalOriginalArea = "TotalOriginalArea",
    TotalOriginalFeatureNumber = "TotalOriginalFeatureNumber",
    Clustered = "Clustered"
}

// Al momento gestiamo solo alcuni tipi di geometrie nel GIS.
// Elenco completo valori gestiti in GeoJSon:
// 'Point', 'MultiPoint', 'LineString', 'MultiLineString', 'LinearRing', 'Polygon', 'MultiPolygon', 'GeometryCollection'.

export enum enum_FeatureGeometryType {
    Point = 'Point',
    LineString = 'LineString',
    Polygon = 'Polygon',
    Raster = 'Raster'
}

export enum GISModality {
    Full = 0,
    Trattamento = 1,
    PaesePoligoniMultiAzienda = 2,
    PaeseDistribuzioneMultiAzienda = 3,
    AnalisiTerreno = 4
};

export enum AnalisiTerrenoType {
    Impresa,
    Centro,
    Campo,
    Appezzamento,
    Impianto,
    Particella
}