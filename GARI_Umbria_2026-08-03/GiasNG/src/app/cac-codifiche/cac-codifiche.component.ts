import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { generateGridProviders, GiasKendoGridModule } from 'gias-kendo-grid';
import { CommonModule } from '@angular/common';
import { GridModule,} from '@progress/kendo-angular-grid';
import { ReactiveFormsModule } from '@angular/forms';
import { CacCodificheGridConfigervice } from './cac-codifiche-grid-config.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { Subscription } from 'rxjs';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-cac-codifiche',
  standalone: true, 
  templateUrl: './cac-codifiche.component.html',
  styleUrls: ['./cac-codifiche.component.css'],
  imports: [CommonModule, GiasKendoGridModule, GridModule, ReactiveFormsModule],
   providers: [...generateGridProviders(CacCodificheGridConfigervice, CacCodificheComponent)],
})

export class CacCodificheComponent implements OnInit {
    

  constructor() {
   }

  ngOnInit(){
 
  }


}
