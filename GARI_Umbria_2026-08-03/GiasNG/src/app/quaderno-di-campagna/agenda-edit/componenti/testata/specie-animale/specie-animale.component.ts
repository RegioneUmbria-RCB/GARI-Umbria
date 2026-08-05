import { Component, OnInit } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { RisorsaZootecnica } from 'app/Model/attivita/risorse/RisorsaZootecnica';
import { Genere } from 'app/Model/metaschema/utilizzi/Genere';
import { IndirizzoProduttivo } from 'app/Model/metaschema/utilizzi/IndirizzoProduttivo';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { QdCService } from 'app/quaderno-di-campagna/agenda-edit/service/qdc.service';
import { QdCTestataService } from 'app/quaderno-di-campagna/agenda-edit/service/testata/testata.service';
import { lastValueFrom } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-specie-animale',
  templateUrl: './specie-animale.component.html',
  styleUrls: ['./specie-animale.component.css']
})
export class SpecieAnimaleComponent implements OnInit {

  defaultItemZoo: RisorsaZootecnica;

  constructor(public qdcservice: QdCService,
    public testataservice: QdCTestataService,
    public translocoService: TranslocoService,
    private ddlService: GiasDropDownTemplateService) { }

  ngOnInit(): void {
    this.testataservice.getListaSpecieAnimali();
  }

  openDdlSpecieAnimali() {
    this.testataservice.getListaSpecieAnimali();
  }
}
