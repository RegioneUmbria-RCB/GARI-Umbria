export const Enum_TipoDiOverlay = {
    Sentinel2Agronica: { value: 0, name: "Agronica_Sentinel2", code: 0 },
    wms: { value: 1, name: "wms", code: 1 }
}

export const Enum_GmapTilesFormat = {
    Gdal2Tiles: { value: 0, name: "Gdal2Tiles", code: 0 },
    AgronicaOld: { value: 1, name: "AgronicaOld", code: 1 }
}

export const wmsCatasto = "Wms - Catasto";

export function WmsUrlAE(bbox) {

    var url = "https://wms.cartografia.agenziaentrate.gov.it/inspire/wms/ows01.php?SERVICE=WMS";
    url += "&VERSION=1.3.0";
    url += "&REQUEST=GetMap";
    url += "&BBOX=" + bbox;
    url += "&CRS=EPSG:6706";
    url += "&WIDTH=1024"; // Da GetCapabilities questa è la nuova larghezza massima
    url += "&HEIGHT=872";
    url += "&LAYERS=CP.CadastralZoning,strade,acque,CP.CadastralParcel,fabbricati,vestizioni";
    url += "&STYLES=default";
    url += "&FORMAT=image/png";
    url += "&DPI=96";
    url += "&MAP_RESOLUTION=96";
    url += "&FORMAT_OPTIONS=dpi:96";
    url += "&TRANSPARENT=TRUE";

    return url;

}
