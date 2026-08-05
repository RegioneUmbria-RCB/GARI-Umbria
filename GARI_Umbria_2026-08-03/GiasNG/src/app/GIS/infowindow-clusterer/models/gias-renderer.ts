import { GiasCluster } from './gias-cluster';

export declare class GiasClusterStats {
  readonly markers: {
    sum: number;
  };

  readonly giasClusters: {
    count: number;
    infoWindows: {
      mean: number;
      sum: number;
      min: number;
      max: number;
    };
  };

  constructor(infoWindows: google.maps.InfoWindow[], clusters: Array<GiasCluster>);
}

export interface GiasRenderer {
  render(cluster: GiasCluster, stats: GiasClusterStats): google.maps.Marker;
}

export class GiasDefaultRenderer implements GiasRenderer {
  private readonly MAX_NUM_MARKERS: number = 90;
  private readonly NUM_MARKERS_STEP: number = 30;

  render(cluster: GiasCluster, _: GiasClusterStats): google.maps.Marker {
    const colors: Array<string> = [
      'success',
      'danger',
      'error'
    ];

    // conto solo le labels che sarebbero effettivamente visibili
    const numberOfLabels: number = cluster.infoWindows.filter(iw => iw.visible).length;
    if (numberOfLabels == 0) {
      return null;
    }

    const color = colors[(numberOfLabels > this.MAX_NUM_MARKERS ? colors.length - 1 : Math.floor(numberOfLabels / this.NUM_MARKERS_STEP))];
    return new google.maps.Marker({
      position: cluster.position,
      icon: {
        url: 'https://www.google.com/mapfiles/arrowtransparent.png',
      },
      label: {
        text: numberOfLabels.toString(),
        color: 'white',
        className: 'maps-polygon-clusterer ' + color,
      },
      map: cluster.map,
      clickable: true,
      draggable: false,
      visible: true
    });
  }
}
