import {Component, ElementRef, Inject, OnInit, ViewChild} from '@angular/core';
import {QdCService} from "../../../service/qdc.service";
import { QdCTestataService } from 'app/quaderno-di-campagna/agenda-edit/service/testata/testata.service';
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-testata-visita',
  templateUrl: './testata-visita.component.html',
  styleUrls: ['./testata-visita.component.scss']
})
export class TestataVisitaComponent implements OnInit {

      public defaultValue;

      constructor(public qdcservice: QdCService,
                  public testataservice: QdCTestataService,
                  private elementRef: ElementRef,
                  @Inject(LOADING_TOKEN) private loadingService: LoadingService) { }

      ngOnInit() {
        this.defaultValue = new Date();

        this.loadingService.set_isLoading({ isLoading: true, component: this.elementRef });

        this.qdcservice.TestataVisitaForm.get("Azienda_Visita").valueChanges.subscribe(value => { 
          this.loadingService.set_isLoading({ isLoading: false, component: this.elementRef });
        });
      }

      async changeAziendeAgenzie(showAgenzie: boolean) {
        this.loadingService.set_isLoading({ isLoading: true, component: this.elementRef });
        this.testataservice.getListaAziende((this.qdcservice.TestataVisitaForm.get("Operatore_Visita").value).username, true, false);
        this.testataservice.getArray_AttivitaPersonalizzate();
      }

      async changeDominioSpecie(tutteLeSpecie: boolean) {

        await this.testataservice.getArray_UtilizziTerreno(true);

        if (!tutteLeSpecie && this.qdcservice.TestataForm.get("Specie").value.codice === 0) {
            await this.testataservice.CambioSpecie();
            this.qdcservice.RicaricaGridImpianti();
            this.qdcservice.RicaricaProdottiDaMagazzino();
            this.qdcservice.TrovaPoligoniGIS();
        }
      }

}
