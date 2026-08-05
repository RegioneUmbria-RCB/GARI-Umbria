import {Injectable} from '@angular/core';
import {GoogleMapService} from '../google-map/google-map.service';

@Injectable()
export class PositionService {
  constructor(private googleMapService: GoogleMapService) {  }

  public goToPosition(lat: number, lng: number, zoom?: number) {
    // let newposition: google.maps.LatLng = new google.maps.LatLng(lat, lng);
    let newposition: google.maps.LatLngLiteral = {
      lat: lat,
      lng: lng
    }
    this.googleMapService.googleMapWrapper.googleMap.setCenter(newposition);

    if (!!zoom && this.googleMapService.googleMapWrapper.getZoom() < zoom) {
      this.googleMapService.googleMapWrapper.googleMap.setZoom(zoom);
    }
  }

  public createPlaceAutocomplete(input: HTMLInputElement) {
    const options = {
      fields: ["formatted_address", "geometry", "name"],
      strictBounds: false,
      types: ["establishment"]
    };

    const autocomplete = new google.maps.places.Autocomplete(input, options);
  }
}
