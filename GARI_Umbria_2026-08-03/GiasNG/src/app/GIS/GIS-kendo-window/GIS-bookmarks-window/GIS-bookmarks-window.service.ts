import { Injectable, Injector } from '@angular/core';
import { map, Observable, switchMap } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';
import { Bookmark, Enum_Tipo_Operazione_Bookmark, GisClient, RispostaStandard } from 'app/Service/api.service';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';

@Injectable()
export class GISBookmarksWindowService {
  constructor(
    protected injector: Injector,
    protected transloco: TranslocoService,
    private googleMapService: GoogleMapService,
    private translocoService: TranslocoService,
    private gisClient: GisClient
  ) { }

  public readBookmarks(): Observable<Bookmark[]> {
    return this.gisClient.gisLeggiBookmark(0)
      .pipe(map(res => res.RispostaStringa.sort(GISBookmarksWindowService.sortBookmarks)));
  }

  public insertBookmark(bookmark: Bookmark): Observable<RispostaStandard> {
    this.fillWIthMapInformation(bookmark);
    return this.gisClient.gisOperazioniBookmark({ bookmark: bookmark, TipoOperazione: Enum_Tipo_Operazione_Bookmark.INSERT });
  }

  public deleteBookmark(Bookmark_Cod: number): Observable<RispostaStandard> {
    return this.gisClient.gisOperazioniBookmark({ bookmark: { Bookmark_Cod: Bookmark_Cod }, TipoOperazione: Enum_Tipo_Operazione_Bookmark.DELETE });
  }

  public updateLastPosition(lat: number, lng: number, zoom: number): Observable<RispostaStandard> {
    return this.readBookmarks()
      .pipe(switchMap(bookmarks => {
        const bookmark = bookmarks.find(f => f.Posizioni_Speciali == 1);
        if (bookmark == null) {
          const bookmarkToAdd = {
            Bookmark_Cod: 0,
            Bookmark_Des: this.translocoService.translate('gis.UltimaPosizione'),
            Posizioni_Speciali: 1,
            Center_Lat: lat,
            Center_Lng: lng,
            Zoom: zoom
          } as Bookmark;
          return this.gisClient.gisOperazioniBookmark({ bookmark: bookmarkToAdd, TipoOperazione: Enum_Tipo_Operazione_Bookmark.INSERT });
        }

        bookmark.Center_Lat = lat;
        bookmark.Center_Lng = lng;
        bookmark.Zoom = zoom;
        return this.gisClient.gisOperazioniBookmark({ bookmark: bookmark, TipoOperazione: Enum_Tipo_Operazione_Bookmark.UPDATE });
      }));
  }

  private fillWIthMapInformation(bookmark: Bookmark): void {
    const map = this.googleMapService.googleMapWrapper.googleMap;
    const center = map.getCenter();

    bookmark.Center_Lat = center.lat();
    bookmark.Center_Lng = center.lng();
    bookmark.Zoom = map.getZoom();
  }

  private static sortBookmarks = (a: Bookmark, b: Bookmark): number => {
    if (a.Posizioni_Speciali == 1) {
      return -1;
    }

    if (b.Posizioni_Speciali == 1) {
      return 1;
    }

    return a.Bookmark_Cod - b.Bookmark_Cod;
  };
}