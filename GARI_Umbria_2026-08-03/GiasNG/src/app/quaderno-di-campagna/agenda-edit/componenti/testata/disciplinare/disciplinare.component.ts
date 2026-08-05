import { Component, OnDestroy, OnInit } from '@angular/core';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import {enum_LAVCOD, enum_PUARegolamenti_Tipo} from 'app/Model/TipiEnumerativi';
import { QdCService } from 'app/quaderno-di-campagna/agenda-edit/service/qdc.service';
import { QdCTestataService } from 'app/quaderno-di-campagna/agenda-edit/service/testata/testata.service';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { skip, Subscription } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-disciplinare',
  templateUrl: './disciplinare.component.html',
  styleUrls: ['./disciplinare.component.css'],
  providers:[GiasDropDownTemplateService]
})
export class DisciplinareComponent implements OnInit, OnDestroy {

  Subs: Subscription = new Subscription();

  constructor(private ddlService:GiasDropDownTemplateService,
              public testataservice: QdCTestataService,
              public qdcservice: QdCService) { }

  ngOnInit(): void {

    this.Subs.add(this.ddlService.currentDropDownValueObject.pipe(skip(1)).subscribe(async ddlElem=>{
      switch(ddlElem.FormControlName){
        case 'Disciplinare':
          this.testataservice.CambioDisciplinare(ddlElem.Value);
          break;
      }
    }));

  }

  //Carico la ddl solo quando scatta l'evento di open
  async openDdlDisciplinare(ddlEl: GiasDropDownTemplateSComponent, formName: string) {

    let fn:any;

    switch (formName) {
      case 'Disciplinare':
        fn = async () => {return await this.testataservice.getArray_Disciplinari()};
        UtilityFunctions.loadDropDownMultiSelectItemsOnlyWhenUndefined(ddlEl, fn);
        break;
    }
  }

  ngOnDestroy(): void {
    this.Subs.unsubscribe();
  }


  mostraDisciplinare(){

    let mostra = false;

    let Operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").value;

    let index = -1;

    if(Operazioni){
      index = Operazioni.findIndex(o=>
      {
        let disciplinare = this.qdcservice.getDisciplinareModelValue(+ o.primaryKey.codice);

        if(this.qdcservice.MostraDisciplinare(o,this.qdcservice.Sezioni_ProdottoFormArray) &&
          disciplinare?.regolamentoConcimazione?.tipo !== enum_PUARegolamenti_Tipo.PUA){
          return o;
        }
      });
    }


    if(index > -1)
      mostra = true;

    return mostra;

  }

  MostradivAlertTerrenoNudo(){

    let mostra = false;

    let Operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").value;

    if(this.qdcservice.GetSpeciefromUtilizzoTerreno().codice > 0){
      mostra = false;
    }else{

      let index = -1;

      if(Operazioni){
        index = Operazioni.findIndex(o=>
        {
          if(+ o.primaryKey.codice === enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO ||
            + o.primaryKey.codice === enum_LAVCOD.CONCIA_SEME ||
            + o.primaryKey.codice === enum_LAVCOD.GEODISINFESTAZIONE ||
            + o.primaryKey.codice === enum_LAVCOD.DISERBO ||
            + o.primaryKey.codice === enum_LAVCOD.DISSECCAMENTO ||
            + o.primaryKey.codice === enum_LAVCOD.TRATTAMENTO_FITOREGOLATORE)
            return o;
        });
      }



      if(index > -1)
        mostra = true;
    }

    return mostra;
  }

}
