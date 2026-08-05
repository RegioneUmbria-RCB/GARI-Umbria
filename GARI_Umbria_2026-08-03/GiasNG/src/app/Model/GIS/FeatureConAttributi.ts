import { enum_FeatureGeometryType } from "app/GIS/GIS-enum/GIS-feature";
import { enum_TipoOperazioneDB } from "../TipiEnumerativi";
import Feature = google.maps.Data.Feature;

export class DatiFeatureConAttributi {

    public TipoOperazione?: enum_TipoOperazioneDB;
    public EntitaCod?: number;
    public Cartografia?: string;
    public LayerElementiGraficiCod?: number;
    public ElementoGraficoDes?: string;
    public FlagGps?: number;
    public FeatureGeometryType?: enum_FeatureGeometryType;
    public GoogleMapsDataGeometry?: google.maps.Data.Geometry;
    public FeatureModified?: Feature;

    // Indica se i dati sono completi
    public DatiCompleti: boolean;

}
