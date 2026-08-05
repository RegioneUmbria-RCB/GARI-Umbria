export const CLUSTER_ICON_SIZE = 50;
export const CLUSTER_MARKER_URL = 'https://maps.google.com/mapfiles/ms/icons/purple.png';
const MAX_ZOOM = 21;

export abstract class Cluster {
  features: string[] = [];

  protected marker: google.maps.Marker | null = null;
  protected listener: google.maps.MapsEventListener | null = null;

  constructor(public id: string) { }

  setMarker(googleMap: google.maps.Map): google.maps.Marker {
    if (this.marker != null) {
      return this.marker;
    }

    const text = this.getClusterText();
    const position = this.getClusterPosition();
    this.marker = new google.maps.Marker({
      position: position,
      label: {
        text: text,
        color: 'white',
      },
      icon: {
        url: CLUSTER_MARKER_URL,
        scaledSize: new google.maps.Size(CLUSTER_ICON_SIZE, CLUSTER_ICON_SIZE),
        labelOrigin: new google.maps.Point((CLUSTER_ICON_SIZE / 2) - 1, CLUSTER_ICON_SIZE / 3),
      },
      clickable: true,
      draggable: false,
      visible: true,
      map: googleMap
    });

    this.setListener(googleMap);

    return this.marker;
  }

  remove(): void {
    this.marker?.setMap(null);
    this.marker = null;

    this.listener?.remove();
    this.listener = null;
  }

  abstract getClusterText(): string;

  abstract getClusterPosition(): google.maps.LatLng;

  abstract setListener(googleMap: google.maps.Map): void;
}

export class FrontendCluster extends Cluster {
  positionPoint: google.maps.Point | null = null;
  position: google.maps.LatLng | null = null;

  private allPositions: google.maps.LatLng[] = [];

  constructor(public id: string) {
    super(id);
  }

  addFeature(feature: string, position: google.maps.LatLng, positionPoint: google.maps.Point): void {
    this.features.push(feature);

    // To improve performance let's take the first feature as the center of the cluster
    this.position ??= position;
    this.positionPoint ??= positionPoint;
    this.allPositions.push(position);
  }

  getClusterPosition(): google.maps.LatLng {
    return this.position;
  }

  getClusterText(): string {
    return this.features.length.toString();
  }

  setListener(googleMap: google.maps.Map): void {
    this.listener = this.marker.addListener('click', () => {
      const bounds = new google.maps.LatLngBounds();
      for (const position of this.allPositions) {
        bounds.extend(position);
      }

      googleMap.fitBounds(bounds);
      googleMap.setCenter(bounds.getCenter());
    });
  }
}

export class BackendCluster extends Cluster {
  position: google.maps.LatLng | null = null;

  private text: string = null;

  constructor(public id: string) {
    super(id);
  }

  setFeature(feature: string, position: google.maps.LatLng, text: string): void {
    this.features.push(feature);
    this.position = position;
    this.text = text;
  }

  getClusterPosition(): google.maps.LatLng {
    return this.position;
  }

  getClusterText(): string {
    return this.text;
  }

  setListener(googleMap: google.maps.Map): void {
    this.listener = this.marker.addListener('click', () => {
      const zoom = Math.min(MAX_ZOOM, googleMap.getZoom() + 2);
      googleMap.setCenter(this.position);
      googleMap.setZoom(zoom);
    });
  }
}