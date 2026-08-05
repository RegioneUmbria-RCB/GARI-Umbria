import { Injectable } from '@angular/core';
import { GridPublicService } from 'gias-kendo-grid';
import { PublicServices } from './utils';

type EnumDictionary<T extends number, U> = {
    [K in T]: U;
};

@Injectable({ providedIn: 'root' })
export class EditGruppiMerceService {


    public gridServices: EnumDictionary<PublicServices, GridPublicService> = {
        [PublicServices.EditGruppiMerce]: null,
    }

    initializePubService(name: PublicServices, service: GridPublicService) {
        this.gridServices[name] = service;
    }
}