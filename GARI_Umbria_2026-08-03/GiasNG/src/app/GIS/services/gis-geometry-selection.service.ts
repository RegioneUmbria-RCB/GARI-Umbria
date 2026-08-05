import { Injectable } from "@angular/core";
import { BehaviorSubject, Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class GISGeometrySelectionService {

    geometrySelectedFromOutside$ = new Subject<[string, boolean][]>();
    geometrySelectedFromMap$ = new Subject<[string, boolean][]>();

    private allSelected$: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
    private isInitialized$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    public resetFromOutside(): void {
        const codes = this.allSelected$.getValue();

        for (const code of codes) {
            this.removeFromOutside(code);
        }

        this.isInitialized$.next(false);
    }

    public getCurrentSelection(): [string, boolean][] {
        return this.allSelected$.getValue().map(x => [x, true]);
    }

    public isInitialized(): boolean {
        return this.isInitialized$.getValue();
    }

    public addFromOutside(...codes: string[]): void {
        this.add(this.geometrySelectedFromOutside$, ...codes);
    }

    public removeFromOutside(...codes: string[]): void {
        this.remove(this.geometrySelectedFromOutside$, ...codes);
    }

    public addFromMap(...codes: string[]): void {
        this.add(this.geometrySelectedFromMap$, ...codes);
    }

    public removeFromMap(...codes: string[]): void {
        this.remove(this.geometrySelectedFromMap$, ...codes);
    }

    private add(obs: Subject<[string, boolean][]>, ...codes: string[]): void {
        this.isInitialized$.next(true);
        const geometrySelected = new Set(this.allSelected$.getValue());

        // avoid duplicates
        const toEmit: [string, boolean][] = [];
        for (const tmpCode of codes) {
            const code = this.clearCode(tmpCode);
            if (!geometrySelected.has(code)) {
                geometrySelected.add(code);
                toEmit.push([code, true]);
            }
        }

        this.allSelected$.next(Array.from(geometrySelected));

        if (toEmit.length > 0) {
            obs.next(toEmit);
        }
    }

    private remove(obs: Subject<[string, boolean][]>, ...codes: string[]): void {
        this.isInitialized$.next(true);
        const geometrySelected = new Set(this.allSelected$.getValue());

        const toEmit: [string, boolean][] = [];
        for (const tmpCode of codes) {
            const code = this.clearCode(tmpCode);
            if (geometrySelected.has(code)) {
                geometrySelected.delete(code);
                toEmit.push([code, false]);
            }
        }

        this.allSelected$.next(Array.from(geometrySelected));

        if (toEmit.length > 0) {
            obs.next(toEmit);
        }
    }

    private clearCode(code: string): string {
        const parts = code.split('§');
        return parts.slice(1, 6).map(x => `§${x}`).join('');
    }
}
