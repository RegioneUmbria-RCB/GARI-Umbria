import { Injectable } from '@angular/core';
import { Appezzamento } from 'app/Model/anagrafiche/Appezzamento';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { Observable, BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EserciziCopiaSpostaAppezzamentiService {

  private appezzamenti: Appezzamento[];

  private mostraPopup: boolean = false;

  private mostraPopupSource = new BehaviorSubject(this.mostraPopup);
  private ricaricaImpianti: boolean = false;
  private ricaricaImpiantiSource = new BehaviorSubject(this.ricaricaImpianti);

  public getAppezzamenti(): Appezzamento[] {
    return this.appezzamenti;
  }

  public setAppezzamenti(appezzamenti: Appezzamento[]): void {
    this.appezzamenti = appezzamenti;
  }

  public setMostraPopup(mostra: boolean): void {
    this.mostraPopupSource.next(mostra);
  }

  public getMostraPopup(): Observable<boolean> {
    return this.mostraPopupSource.asObservable();
  }

  public setRicaricaImpianti(ricarica: boolean): void {
      this.ricaricaImpiantiSource.next(ricarica);
  }

  public getRicaricaImpianti(): Observable<boolean> {
      return this.ricaricaImpiantiSource.asObservable();
  }

}

export class CopiaSpostaAppezzamenti
{
  Appezzamenti: Appezzamento[];

  NuovoCentro: CentroAziendale;

  SpostaEliminaOrigine: boolean;
  CopiaCatasto: boolean;
}
