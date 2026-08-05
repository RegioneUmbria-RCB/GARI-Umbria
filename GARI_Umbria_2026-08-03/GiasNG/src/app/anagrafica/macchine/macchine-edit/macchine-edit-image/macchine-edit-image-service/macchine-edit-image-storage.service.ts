import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MacchineEditImageStorageService {

  private immagineDepositataGrande: File;
  private immagineDepositataPiccola: File;

  depositaImmagine(immagine: File, grande: boolean): void {
    if(grande){
      this.immagineDepositataGrande = immagine;
    } else {
      this.immagineDepositataPiccola = immagine;
    }
  }

  prelevaImmagine(grande: boolean): File {
    if(grande){
      return this.immagineDepositataGrande;
    } else {
      return this.immagineDepositataPiccola;
    }
  }

}
