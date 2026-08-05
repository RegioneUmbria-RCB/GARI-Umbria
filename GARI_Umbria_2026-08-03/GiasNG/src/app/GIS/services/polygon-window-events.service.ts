import {Injectable} from '@angular/core';
import {Subject} from 'rxjs';

@Injectable()
export class PolygonWindowEventsService {
    public reloadFeatures: Subject<any> = new Subject<any>();
}
