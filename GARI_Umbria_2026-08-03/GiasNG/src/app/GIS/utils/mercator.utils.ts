export class Mercator {
  static fromLatLngToPoint(latLng: google.maps.LatLng, tileSize: number = 256): google.maps.Point {
    const siny = Math.min(Math.max(Math.sin(latLng.lat() * (Math.PI / 180)), -.9999), .9999);
    const x = 128 + latLng.lng() * (tileSize / 360);
    const y = 128 + 0.5 * Math.log((1 + siny) / (1 - siny)) * -(tileSize / (2 * Math.PI));
    return new google.maps.Point(x, y);
  }

  static fromPointToLatLng(point: google.maps.Point, tileSize: number = 256): google.maps.LatLng {
    const lat = (2 * Math.atan(Math.exp((point.y - 128) / -(tileSize / (2 * Math.PI)))) - Math.PI / 2) / (Math.PI / 180);
    const lng = (point.x - 128) / (tileSize / 360);
    return new google.maps.LatLng(lat, lng);
  }

  static getTileAtLatLng(latLng: google.maps.LatLng, zoom: number, tileSize: number = 256): Tile {
    const t = Math.pow(2, zoom);
    const s = tileSize / t;
    const p = this.fromLatLngToPoint(latLng);
    return { x: Math.floor(p.x / s), y: Math.floor(p.y / s), zoom: zoom };
  }

  static getTileBounds(tile: Tile, tileSize: number = 256): google.maps.LatLng[] {
    const nomalizedTile = this.normalizeTile(tile);
    const t = Math.pow(2, nomalizedTile.zoom);
    const s = tileSize / t;

    const sw = new google.maps.Point(
      nomalizedTile.x * s,
      nomalizedTile.y * s + s
    );
    const se = new google.maps.Point(
      nomalizedTile.x * s,
      nomalizedTile.y * s
    );
    const ne = new google.maps.Point(
      nomalizedTile.x * s + s,
      nomalizedTile.y * s
    );
    const nw = new google.maps.Point(
      nomalizedTile.x * s + s,
      nomalizedTile.y * s + s
    );

    return [
      this.fromPointToLatLng(sw),
      this.fromPointToLatLng(se),
      this.fromPointToLatLng(ne),
      this.fromPointToLatLng(nw)
    ];
  }

  static normalizeTile(tile: Tile): Tile {
    const t = Math.pow(2, tile.zoom);
    const result = {
      x: ((tile.x % t) + t) % t,
      y: ((tile.y % t) + t) % t,
      zoom: tile.zoom
    } as Tile;
    return result;
  }
}

export interface Tile {
  x: number;
  y: number;
  zoom: number;
}
