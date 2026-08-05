import { inject, Injectable } from "@angular/core";
import { Translation, TranslocoLoader } from "@jsverse/transloco";
import { HttpClient } from "@angular/common/http";
import { TranslocoLoaderData } from "@jsverse/transloco/lib/transloco.loader";
import { giasGridTranslocoLoader } from 'gias-kendo-grid';
import { map, of, tap } from "rxjs";

@Injectable({ providedIn: 'root' })
export class TranslocoHttpLoader implements TranslocoLoader {
    private http = inject(HttpClient);

    getTranslation(lang: string, data?: TranslocoLoaderData) {
        if (data?.scope == 'giasgrid') {
            // lang is in the form of 'giasgrid/it'
            return of(giasGridTranslocoLoader(lang.split('/')[1]));
        }

        return this.http.get<Translation>(`assets/i18n/${lang}.json`);
    }
}
