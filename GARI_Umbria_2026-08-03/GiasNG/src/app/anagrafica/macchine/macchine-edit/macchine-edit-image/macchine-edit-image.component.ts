import { HttpHandler, HttpRequest } from '@angular/common/http';
import { AfterViewInit, Component, Input, SecurityContext} from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { SuccessEvent } from '@progress/kendo-angular-upload';
import { Immagine } from 'app/Model/anagrafiche/Immagine';
import { CustomComponent } from 'gias-kendo-grid';
import { MacchineEditImageStorageService } from './macchine-edit-image-service/macchine-edit-image-storage.service';

@Component({
  standalone: false,
  selector: 'app-macchine-edit-image',
  templateUrl: './macchine-edit-image.component.html',
  styleUrls: ['./macchine-edit-image.component.css']
})
export class MacchineEditImageComponent implements CustomComponent, AfterViewInit {
  @Input() edit: boolean;
  @Input() input: string;
  @Input() immagineGrandeControl: AbstractControl;
  @Input() immaginePiccolaControl: AbstractControl;

  immagineGrande : Immagine;
  immaginePiccola : Immagine;

  saveLarge: string = "saveLarge"
  removeLarge: string = "removeLarge"
  saveSmall: string = "saveSmall"
  removeSmall: string = "removeSmall"
  req: HttpRequest<unknown>
  next: HttpHandler

  allowedExtensions: [".jpg", ".jpeg", ".png", ".tiff"]

  thumbnail: SafeResourceUrl;
  thumbnailGrande: SafeResourceUrl;

  constructor(private _sanitizer: DomSanitizer,
    private macchineEditImageStorageService: MacchineEditImageStorageService) {
  }

  ngAfterViewInit(): void {
    this.immagineGrande = this.immagineGrandeControl.value;
    this.immaginePiccola = this.immaginePiccolaControl.value;
    this.caricaImmagini();
  }

  caricaImmagini(): void {

    if(this.immagineGrande != undefined)  {
      //this.thumbnailGrande = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/'+ this.immagineGrande.estensione +';base64,' + this.immagineGrande.immagine);
      this.thumbnailGrande = this._sanitizer.sanitize(
        SecurityContext.URL,
        'data:image/'+ this.immagineGrande.estensione +';base64,' + this.immagineGrande.immagine
      )
    }
    if(this.immaginePiccola != undefined)  {
      //this.thumbnail = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/'+ this.immaginePiccola.estensione +';base64,' + this.immaginePiccola.immagine);
      this.thumbnail = this._sanitizer.sanitize(
        SecurityContext.URL,
        'data:image/'+ this.immaginePiccola.estensione +';base64,' + this.immaginePiccola.immagine
      )
    }

  }

  cancellaImmagine(grande: boolean) {
    if (grande) {
      this.immagineGrande = new Immagine('','','')
      this.immagineGrandeControl.patchValue(this.immagineGrande)
    } else {
      this.immaginePiccola = new Immagine('','','')
      this.immaginePiccolaControl.patchValue(this.immaginePiccola)
    }
  }

  impostaImmagine(grande: boolean, event: SuccessEvent): void {
    // Imposta l'immagine caricata come immagine corrente in base64.
    if(event.operation == 'upload') {
      let immagineRicevuta = this.macchineEditImageStorageService.prelevaImmagine(grande)
      var fReader = new FileReader();
      fReader.readAsDataURL(immagineRicevuta);
      fReader.onloadend = e => {
        if (grande) {
          //this.thumbnailGrande = this._sanitizer.bypassSecurityTrustResourceUrl(e.target.result.toString())
          this.thumbnailGrande = this._sanitizer.sanitize(
            SecurityContext.URL,
            e.target.result.toString()
          )
          this.immagineGrande = new Immagine(
            immagineRicevuta.name,
            immagineRicevuta.type.split('/')[1],
            e.target.result.toString().split(',')[1]
          );
          this.immagineGrandeControl.patchValue(this.immagineGrande)
        } else {
          //this.thumbnail = this._sanitizer.bypassSecurityTrustResourceUrl(e.target.result.toString())
          this.thumbnail = this._sanitizer.sanitize(
            SecurityContext.URL,
            e.target.result.toString()
          )
          this.immaginePiccola = new Immagine(
            immagineRicevuta.name,
            immagineRicevuta.type.split('/')[1],
            e.target.result.toString().split(',')[1]
          );
          this.immaginePiccolaControl.patchValue(this.immaginePiccola)
        }
      }
    }
  }
}
