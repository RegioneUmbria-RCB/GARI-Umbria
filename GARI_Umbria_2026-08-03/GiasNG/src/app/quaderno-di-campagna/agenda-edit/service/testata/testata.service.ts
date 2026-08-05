import {DatePipe} from '@angular/common';
import {Inject, Injectable, LOCALE_ID, OnDestroy} from '@angular/core';
import {TranslocoService} from '@jsverse/transloco';
import {Campo, PKCampo} from 'app/Model/anagrafiche/Campo';
import {CentroAziendale, PKCentroAziendale} from 'app/Model/anagrafiche/CentroAziendale';
import {Lavorazione} from 'app/Model/attivita/Lavorazione';
import {
  AGRODATAFINE,
  AGRODATAINIZIO,
  DpiBio, ElencoLavCodRilieviSenzaImpianti,
  LAVCOD_ALTRE_OPERAZIONI, NessunaSpecieQdC,
  NessunDpi,
  NessunDpiNessunaEtichetta
} from 'app/Model/CostantiPersonalizzate';
import {Specie} from 'app/Model/metaschema/utilizzi/Specie';
import {enum_LAVCOD, enum_Servizi} from 'app/Model/TipiEnumerativi';
import {AgendaService, Controllo_Sportello} from 'app/Service/Agenda/Agenda.service';
import {AttivitaPersonalizzataService} from 'app/Service/Agenda/attivita-personalizzata.service';
import {CampiService, LeggiCampi} from 'app/Service/Anagrafica/campi.service';
import {CentriAziendaliService, LeggiCentriAziendali} from 'app/Service/Anagrafica/centri.service';
import {ImpiantiService} from 'app/Service/Anagrafica/impianti.service';
import {DpiService, LeggiDisciplinari} from 'app/Service/DPI/dpi.service';
import {enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity} from 'app/Service/master.service';
import {LeggiOperazioni, OperazioniService} from 'app/Service/Metaschema/operazioni.service';
import { ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {
    DropdownListAttivitaPersonalizzata,
    DropdownListCampo,
    DropdownListDisciplinare,
    GridImpiantoSelezionatoModel
} from '../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import {QdCService} from '../qdc.service';
import {BehaviorSubject, lastValueFrom, Subscription} from "rxjs";
import {GridDataWithFilter} from 'gias-kendo-grid';
import {Dialog_Type} from "../../../../Service/gias-dialog.service";
import {MisceleService} from "../miscele.service";
import {SpecieVegetaliService} from 'app/Service/Metaschema/specie-vegetali.service';
import {DestinazioneUso} from 'app/Model/metaschema/utilizzi/DestinazioneUso';
import {AziendaDDLService, LeggiAziende} from './azienda-ddl-service';
import {Impresa} from "app/Model/anagrafiche/Impresa";
import { DropdownListSpecieAnimali, SpecieAnimaliService } from './specie-animali-service';
import { RisorsaZootecnica } from 'app/Model/attivita/risorse/RisorsaZootecnica';
import {Gias2010Redirector} from "../../../../menu-agenda/components/grid-qdc/Gias2010Redirector.service";
import { ObjParametriAgenda } from 'gias-ui-kit';


@Injectable()

export class QdCTestataService implements OnDestroy{

    Array_Aziende: Impresa[] = [];

    Array_UtilizziTerreno: Array<Specie> | Array<DestinazioneUso>;

    Array_Operazioni: Array<Lavorazione>;

    Array_CentriAziendale: Array<CentroAziendale>;

    Array_Disciplinari: Array<DropdownListDisciplinare>;

    Array_Campi: Array<DropdownListCampo>;

    Array_AttivitaPersonalizzate: Array<DropdownListAttivitaPersonalizzata>;

    Array_Specie_Animali: Array<DropdownListSpecieAnimali>;

    usernameOperatore: string = "";

    UsernameOperatoreSource: BehaviorSubject<string> = new BehaviorSubject(this.usernameOperatore);

    showSwitchAziende: boolean = true;

    Subs = new Subscription();

    constructor(private operazioniservice: OperazioniService,
        private centriservice: CentriAziendaliService,
        private impiantiservice: ImpiantiService,
        private dpiservice: DpiService,
        private qdcservice: QdCService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private campiservice: CampiService,
        private agendaService: AgendaService,
        private translocoService: TranslocoService,
        private datePipe: DatePipe,
        private attivitapersonalizzataservice: AttivitaPersonalizzataService,
        @Inject(LOCALE_ID) private locale_id: string,
        private misceleservice: MisceleService,
        private specieService: SpecieVegetaliService,
        private aziendaDDLService: AziendaDDLService,
        private specieAnimaliService: SpecieAnimaliService,
        private giasRedirectService: Gias2010Redirector) {

    }

    async getArray_UtilizziTerreno(fromSwitch?: boolean){

        const flagVisitaValue = this.qdcservice.TestataForm.get("flagVisita").value;
        const specieValue = this.qdcservice.TestataForm.get("Specie").value;
        const isSpecieEmpty = !specieValue;
        const hasSpecieClassType = specieValue?.hasOwnProperty("classType");
        const isSpecieDescNotUtilizzoTerreno = specieValue?.codice !== 3262;
        const operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").getRawValue();

        if (!flagVisitaValue
            || (flagVisitaValue
                && ((fromSwitch && !this.qdcservice.TestataVisitaForm.get("Visualizza_Specie").value)
                    || (!fromSwitch && (isSpecieEmpty || (hasSpecieClassType && isSpecieDescNotUtilizzoTerreno)))
                    )
                )
        ) {
            const objParamsAgenda = this.objParametriAgendaService.getObjParamValue();
            const impresa = this.qdcservice.getImpresa_Model();
            const centroAziendale = null;
            const data = this.qdcservice.TestataForm.get('Data').value;
            const consideraTerrenoNudo = this.qdcservice.getCONSIDERATERRENONUDO();
            const operazioni: Lavorazione[] = this.qdcservice.TestataForm.get('Operazioni').value as Lavorazione[];
            this.Array_UtilizziTerreno = await this.impiantiservice.getSpecieQdCA(centroAziendale, data, impresa as any, consideraTerrenoNudo,operazioni);
        } else {
            this.Array_UtilizziTerreno = await lastValueFrom(this.specieService.leggiTutteLeSpecieAPI());
            let destUso = new DestinazioneUso();
            destUso.codice = 3262;
            destUso.descrizione = "Utilizzo Terreno";
            this.Array_UtilizziTerreno.push(destUso);
        }

        if (flagVisitaValue && !fromSwitch && !isSpecieEmpty && !(hasSpecieClassType && isSpecieDescNotUtilizzoTerreno)) {
            this.qdcservice.TestataVisitaForm.patchValue({
                Visualizza_Specie: true
            });
        }

        //Imposto in automatico il default della Specie
        if(this.Array_UtilizziTerreno && this.Array_UtilizziTerreno.length > 0){

            //Aggiungo la voce 'Nessuna Specie' per i Rilievi Indici Coltura che possono essere registrati senza impianti selezionati
            if(!flagVisitaValue && operazioni && operazioni.length > 0 &&
              operazioni.filter(o=> ElencoLavCodRilieviSenzaImpianti.includes(+o.primaryKey.codice)).length ===  operazioni.length){

              this.Array_UtilizziTerreno.splice(1, 0, this.qdcservice.Obj_NessunaSpecie);
            }else{
              this.Array_UtilizziTerreno = this.Array_UtilizziTerreno.filter(u=>u.codice !== NessunaSpecieQdC);
            }

            let Specie_Default=null;

            let Specie_Selezionata = this.qdcservice.TestataForm.get("Specie").value;

            if(Specie_Selezionata && Specie_Selezionata.codice !== -1){
                Specie_Default = this.Array_UtilizziTerreno.find(u=>u.codice === Specie_Selezionata.codice);
            }

            if(this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB === Enum_DBTypeOperation.Write && this.Array_UtilizziTerreno.length === 2){
                Specie_Default = this.Array_UtilizziTerreno[1];
            }

            if(!Specie_Default){
                Specie_Default=new Specie(-1);

                Specie_Default.descrizione=this.translocoService.translate("NessunaSelezione");
            }

            this.qdcservice.TestataForm.patchValue({
                Specie:  Specie_Default
            });

        }

        return this.Array_UtilizziTerreno;

    }

    async getListaAziende(usernameOperatore: string, fromSwitch: boolean, fromChangeOperatore: boolean) {

        if (usernameOperatore === '') {
            this.qdcservice.TestataVisitaForm.controls['Azienda_Visita'].patchValue(null);
            return;
        }

        let objParametriAgenda: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        let param: LeggiAziende = new LeggiAziende(usernameOperatore);
        let result;

        if (fromChangeOperatore) {          //devo capire come nascondere lo switch se non son presenti agenzie
            result = await lastValueFrom(this.aziendaDDLService.leggiListaAgenzie(param));
            let arrResult = JSON.parse(JSON.stringify(result));

            if (arrResult.length === 0) {       //nascondo lo switch
                this.showSwitchAziende = false;
                this.qdcservice.TestataVisitaForm.controls['Aziende_Agenzie'].patchValue(false);
            } else {
                this.showSwitchAziende = true;
            }
        }

        if (this.qdcservice.TestataVisitaForm.get("Aziende_Agenzie").value)
            result = await lastValueFrom(this.aziendaDDLService.leggiListaAgenzie(param));
        else
            result = await lastValueFrom(this.aziendaDDLService.leggiListaAziende(param));

        this.Array_Aziende = JSON.parse(JSON.stringify(result));

        if (this.Array_Aziende.length != 0) {
            if (objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write || fromSwitch) {
                this.qdcservice.TestataVisitaForm.controls['Azienda_Visita'].patchValue(this.Array_Aziende[0]);
            } else {
                let aziendaUpdate = this.qdcservice.TestataVisitaForm.get("Azienda_Visita").value;
                let aziendaUpd = this.Array_Aziende.filter((i: Impresa) => {return i.partitaIva === aziendaUpdate.partitaIva});

                if (aziendaUpd.length === 0) {     // non ho trovato l'azienda nell'array caricato, potrebbe essere nell'altro dominio
                    if (this.qdcservice.TestataVisitaForm.get("Aziende_Agenzie").value)
                        result = await lastValueFrom(this.aziendaDDLService.leggiListaAziende(param));
                    else
                        result = await lastValueFrom(this.aziendaDDLService.leggiListaAgenzie(param));

                    this.qdcservice.TestataVisitaForm.controls['Aziende_Agenzie'].patchValue(!(this.qdcservice.TestataVisitaForm.get("Aziende_Agenzie").value));
                    this.Array_Aziende = JSON.parse(JSON.stringify(result));

                    aziendaUpd = this.Array_Aziende.filter((i: Impresa) => {return i.partitaIva === aziendaUpdate.partitaIva});
                }

                this.qdcservice.TestataVisitaForm.controls['Azienda_Visita'].patchValue(aziendaUpd[0]);

                //disabilito la DDL
                this.qdcservice.TestataVisitaForm.get("Azienda_Visita").disable({emitEvent: false});

                //disabilito anche lo switch
                this.qdcservice.TestataVisitaForm.get("Aziende_Agenzie").disable({emitEvent: false});
            }
            await this.getArray_CentriAziendale();
            await this.getArray_UtilizziTerreno();
            this.qdcservice.RicaricaGridImpianti();
            this.qdcservice.TrovaPoligoniGIS();
        } else {
            this.qdcservice.TestataVisitaForm.controls['Azienda_Visita'].patchValue(null);
        }

        if (!this.showSwitchAziende)
            this.getArray_AttivitaPersonalizzate();
    }


    async getArray_Operazioni(operazioni: LeggiOperazioni,impostaArrayallaDDL: boolean = true){

        let risp;

        if (this.qdcservice.TestataForm.get("flagVisita").value) {
            risp = await lastValueFrom(this.operazioniservice.LeggiOperazioniVisite());
            //occorre distinguere se nello switch abbiamo selezionato Aziende o Agenzie
            if (this.qdcservice.TestataVisitaForm.get("Aziende_Agenzie").value) {
                risp = risp.filter(item => +item.primaryKey.codice === LAVCOD_ALTRE_OPERAZIONI);
            }
        } else
            risp = await this.operazioniservice.Leggi_Operazioni_Modello_QdC(operazioni);

        if(impostaArrayallaDDL)
            this.Array_Operazioni = risp;

        return risp;
    }


    async getArray_CentriAziendale(impostaArrayallaDDL: boolean = true){

        //caso in cui un operatore non abbia associata alcuna azienda/agenzia, perciò passo null
        if (this.qdcservice.TestataForm.get("flagVisita").value && this.qdcservice.TestataVisitaForm.get("Azienda_Visita").value === null) {
            this.Array_CentriAziendale = [];
            this.qdcservice.TestataForm.patchValue({
                Centro_Aziendale:  null
            });
            this.qdcservice.elencoCentriAziendali = [];
            return [];
        }

        let LeggiCentriAziendali;

        LeggiCentriAziendali=<LeggiCentriAziendali>{
            impresa:  this.qdcservice.getImpresa_Model(),
            data: this.qdcservice.TestataForm.get('Data').value,
            utilizzoTerreno: this.qdcservice.getUtilizzoTerrenoModel()
        };

        //Se flagVisita è true allora non ci deve essere la voce 'Tutti i centri Aziendali' nella combo
        let flag_prima_riga: boolean = true;

        if(this.qdcservice.TestataForm.get("flagVisita").getRawValue())
          flag_prima_riga = false;

        let risp = [];

        if (this.Leggi_Tutti_Centri_Senza_Filtro_X_Specie()) {

            risp=await this.centriservice.leggiCentriAziendaliModelloQdC(LeggiCentriAziendali,flag_prima_riga);
        } else {

          LeggiCentriAziendali.filtra_validita_esercizi = true;

          risp=await this.centriservice.Leggi_Centri_Aziendali_perSpecieImpianti(LeggiCentriAziendali,flag_prima_riga);
        }

        if(impostaArrayallaDDL){

            this.Array_CentriAziendale = risp;

            if(this.Array_CentriAziendale && this.Array_CentriAziendale.length > 0){

                let Centri_Default=null;

                let Centro_Selezionato: CentroAziendale = this.qdcservice.TestataForm.get("Centro_Aziendale").value;

                if(Centro_Selezionato && Centro_Selezionato.primaryKey.codice !== 0){
                    Centri_Default = this.Array_CentriAziendale.find(c=>c.primaryKey.codice === Centro_Selezionato.primaryKey.codice &&
                        c.primaryKey.partitaIva === Centro_Selezionato.primaryKey.partitaIva);
                }

                if(this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB === Enum_DBTypeOperation.Write && this.Array_CentriAziendale.length === 2){
                    Centri_Default = this.Array_CentriAziendale[1];
                }

                if(!Centri_Default && !this.qdcservice.TestataForm.get("flagVisita").value){
                    Centri_Default = this.qdcservice.Obj_TuttiICentriAziendali;
                } else {
                    if (!Centri_Default && this.qdcservice.TestataForm.get("flagVisita").value)    // per le visite, dobbiamo fare in modo che il centro di default sia sempre un centro
                        Centri_Default = this.Array_CentriAziendale[0];
                }

                //TODO Da riguaradre bene questo caso
                this.qdcservice.TestataForm.patchValue({
                    Centro_Aziendale:  Centri_Default
                });

            }
        }

        this.qdcservice.elencoCentriAziendali = risp;

        return risp;

    }

    async getArray_Disciplinari(impostaArrayallaDDL: boolean = true){


        const LeggiDisciplinari=<LeggiDisciplinari>{
            lavorazioni: this.qdcservice.TestataForm.get('Operazioni').getRawValue(),
            specie: this.qdcservice.GetSpeciefromUtilizzoTerreno(),
            data: this.qdcservice.TestataForm.get('Data').value,
            leggiPianoNutrizionale: this.qdcservice.Lettura_Piano_Nutrizionale
        };

        let risp = await this.dpiservice.Leggi_Disciplinari_Testata_conRegolamentoConcimazione(LeggiDisciplinari) as DropdownListDisciplinare[];

        if(risp){
            let rispMapped = risp.map(d=>{
                d = this.qdcservice.setCodDescrDdlDisciplinare(d);
            });
        }


        if(impostaArrayallaDDL)
            this.Array_Disciplinari = risp;

        this.qdcservice.Default_DPI_da_Impostazione = null;

        return risp;

    }

    async getArray_Campi(){

        const LeggiCampi=<LeggiCampi>{
            centro: this.qdcservice.TestataForm.get('Centro_Aziendale').value,
            data: this.qdcservice.TestataForm.get('Data').value,
            utilizzoTerreno: this.qdcservice.getUtilizzoTerrenoModel()
        };

        this.Array_Campi =(await this.campiservice.LeggiCampi_perSpecieImpianti(LeggiCampi,true, "Tutti")) as DropdownListCampo[];

        let Array_CampiMapped = this.Array_Campi.map(c=>{
            c = this.qdcservice.setCodDescrDdlCampo(c);
        });

        if(this.Array_Campi && this.Array_Campi.length > 0){

            let Campo_Default=null;

            let Campo_Selezionato: DropdownListCampo = this.qdcservice.TestataForm.get("Campo").value;

            if(Campo_Selezionato && Campo_Selezionato.Codice_Concatenato !== ""){
                Campo_Default = this.Array_Campi.find(c=>c.Codice_Concatenato === Campo_Selezionato.Codice_Concatenato);
            }

            if(!Campo_Default){
                let c = new Campo({centroAziendalePK: (this.qdcservice.TestataForm.get('Centro_Aziendale').value as CentroAziendale).primaryKey, codice: 0} as PKCampo);

                c.descrizione = this.translocoService.translate("Tutti");

                Campo_Default = this.qdcservice.setCodDescrDdlCampo(c as DropdownListCampo);
            }

            this.qdcservice.TestataForm.patchValue({
                Campo:  Campo_Default
            });

        }

        return this.Array_Campi;

    }

    async getArray_AttivitaPersonalizzate(){

        if (this.qdcservice.TestataForm.get("flagVisita").value) {
            this.Array_AttivitaPersonalizzate = await lastValueFrom(this.attivitapersonalizzataservice.Leggi_AttivitaPersonalizzataVisita()) as DropdownListAttivitaPersonalizzata[];
            if (this.showSwitchAziende && this.qdcservice.TestataVisitaForm.get("Aziende_Agenzie").value) {
                const arrValoriRilievi = [6, 7, 12];
                this.Array_AttivitaPersonalizzate = this.Array_AttivitaPersonalizzate.filter(item => !arrValoriRilievi.includes(item.codice));
            }
        } else {
            this.Array_AttivitaPersonalizzate = (await this.attivitapersonalizzataservice.Leggi_AttivitaPersonalizzata(true,0,"",this.qdcservice.TestataForm.get('Operazioni').value)) as DropdownListAttivitaPersonalizzata[];
        }

        let Array_AttivitaPersonalizzateMapped = this.Array_AttivitaPersonalizzate.map(a=>{
            a = this.qdcservice.setCodDescrDdlAttivitaPersonalizzata(a);
        });

        return this.Array_AttivitaPersonalizzate;
    }

    async CambioData(){
        // TODO da verificare bene quli letture fare per il cambio data

        if(this.qdcservice.TestataForm.get("Data").valid){

            let Data: Date = this.qdcservice.TestataForm.get("Data").value;

            let Piva: string = this.qdcservice.getImpresa_Model().partitaIva;

            this.Subs.add(this.giasRedirectService.CheckServizioQDC(Piva,Data,this.objParametriAgendaService.getObjParamValue().TipoOperazioneAgenda).subscribe(async (pratica_trovata)=>{
              if(!pratica_trovata){
                this.qdcservice.MostrabtnSalva = false;
              }else{
                this.qdcservice.MostrabtnSalva = true;

                if(Data &&  this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB === Enum_DBTypeOperation.Write){

                  let Controllo_Sportello=<Controllo_Sportello>{
                    Piva: Piva,
                    Servizio_Cod: enum_Servizi.Quaderno_Campagna_Caa,
                    Data_Riferimento: new Date(),
                    SportelloAperto: true,
                    Data_Min: AGRODATAINIZIO,
                    Data_Max: AGRODATAFINE
                  };

                  let c: Controllo_Sportello = await this.agendaService.Controllo_Sportello(Controllo_Sportello);
                  // Se i dati non sono cambiati dopo il controllo allora controllo per il servizio Bio
                  if(c.SportelloAperto && c.Data_Min.getTime() == AGRODATAINIZIO.getTime() && c.Data_Max.getTime() == AGRODATAFINE.getTime()){
                    Controllo_Sportello.Servizio_Cod = enum_Servizi.QuadernoCampagnaBio;
                    c = await this.agendaService.Controllo_Sportello(Controllo_Sportello);
                  }

                  let messaggio = "";

                  if(Data < c.Data_Min){
                    this.qdcservice.TestataForm.patchValue({
                      Data: c.Data_Min
                    });

                    let Data_Min_Str = this.datePipe.transform(c.Data_Min,'dd/MM/yy',undefined,this.locale_id);

                    messaggio = this.translocoService.translate('qdc.Non_possibile_inserire_un_operazione_prima_del', {DataStr: Data_Min_Str});
                  }

                  if(Data > c.Data_Max){
                    this.qdcservice.TestataForm.patchValue({
                      Data: c.Data_Max
                    });

                    let Data_Max_Str = this.datePipe.transform(c.Data_Max,'dd/MM/yy',undefined,this.locale_id);

                    messaggio = this.translocoService.translate('qdc.Non_possibile_inserire_un_operazione_dopo_il', {DataStr: Data_Max_Str});
                  }

                  if(messaggio !== ""){

                    let listErroriGias: ErroreGias[] = [];

                    listErroriGias.push(<ErroreGias>{
                      severity: ErroreGias_Severity.Warning,
                      tipo:  enum_ErroreGias_Tipo.Generico,
                      messaggio: messaggio
                    });

                    this.qdcservice.gestisci_ErroriGias(listErroriGias,true,true).then();

                    this.qdcservice.TestataForm.get("Data").markAsTouched();
                  }
                }

                await this.getArray_UtilizziTerreno();

                await this.getArray_Disciplinari();

                await this.getArray_CentriAziendale();

                await this.getArray_Campi();

                this.qdcservice.RicaricaGridImpianti();

                this.qdcservice.RicaricaProdottiDaMagazzino();

                this.qdcservice.TrovaPoligoniGIS();

                this.ResettaNumericTextboxSuperfici();

                this.ResettaNumericTextboxQuantita();

                this.qdcservice.RicaricaGridMacchine().then();

                this.qdcservice.RicaricaGridOperatori().then();

                //Se cambio la data per la raccolta coloro le righe della grid impianti che non sono conformi

                let Operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").getRawValue();

                if(Operazioni && Operazioni.findIndex(o=>+ o.primaryKey.codice === enum_LAVCOD.RACCOLTA) > -1){

                  await this.qdcservice.Controlla_DataCarenza();

                  this.qdcservice.AggiornaFormArrayImpiantiSelezionati();

                  this.qdcservice.RicaricaGridImpianti();

                  this.qdcservice.RicaricaProdottiDaMagazzino();
                }

                this.qdcservice.Imposta_PUA(null,true).then();

                if(Operazioni && Operazioni.findIndex(o=>+ o.primaryKey.codice === enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA) > -1) {

                    this.qdcservice.caricaCausali(enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA, Data);
                }

              }
            }));

        }

    }

    async CambioCentroAziendale(){
        // TODO da verificare bene qui le letture fare per il cambio centro

        await this.getArray_Campi();

        this.qdcservice.getImpostazioni_QdC_Centro_Azienda(this.qdcservice.TestataForm.get("Operazioni").getRawValue());

    }

  /** Per ora imposto il Disciplinare con nessun disciplinare al cambio della
   * Specie poi saranno da implementare tutti i vari default che vengono caricari nella trattamenti2
   * e OperazioneBootstrap
   */
    async CambioSpecie(){
        this.qdcservice.isNoteInitialized = false;
        let Operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").getRawValue();
        if(Operazioni && Operazioni.findIndex(o=>{ return this.qdcservice.MostraDisciplinare(o,this.qdcservice.Sezioni_ProdottoFormArray); }) > -1){
            await this.getArray_Disciplinari();
            //Imposto NessunDpiNessunaEtichetta se non ho la specie ma ho la Destinazione d'Uso
            let Specie = this.qdcservice.GetSpeciefromUtilizzoTerreno();
            if(Specie.codice > 0){
                this.qdcservice.TestataForm.patchValue({
                    Disciplinare:  this.qdcservice.setCodDescrDdlDisciplinare(this.qdcservice.Obj_NessunDpi as DropdownListDisciplinare)
                });
            }else{
                this.qdcservice.TestataForm.patchValue({
                    Disciplinare:  this.qdcservice.setCodDescrDdlDisciplinare(this.qdcservice.Obj_NessunDpiNessunaEtichetta as DropdownListDisciplinare)
                });
            }
        }
        await this.getArray_CentriAziendale();
        await this.getArray_Campi();

        this.qdcservice.MessaggioImpiantiVisualizzato.DPI = false;
        this.qdcservice.RicaricaGridMacchine().then();
        this.qdcservice.RicaricaGridOperatori().then();
        this.qdcservice.GestisciValidatorImpiantiSuperficie(Operazioni);
    }

    CambioDisciplinare(disciplinare: DropdownListDisciplinare){

        this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti_Tutti();

        //Controllo se il Disciplinare selezionato è meno restrittivo rispetto agli impianti scelti
        this.qdcservice.MessaggioImpiantiVisualizzato.DPI = false;

        this.qdcservice.GestisciColoreRigheGridImpianti();

        this.qdcservice.AvvisoImpianticonDpiRestrittivo((this.qdcservice.GridImpiantiPublicService as BehaviorSubject<GridDataWithFilter>).getValue().data.rows);

        //Controllo se il Disciplinare selezionato ha una copertura/finalità diversa rispetto agli impianti scelti
        //Se viene scelto un dpi BIO,NessunDpi,NessunDpiNessunaEtichetta non ha una copertura o finalità quindi non deve venire fuori il messaggio di copertura/finalità diversa
        if(disciplinare && disciplinare.codice !== DpiBio && disciplinare.codice !== NessunDpi && disciplinare.codice !== NessunDpiNessunaEtichetta && !this.qdcservice.MessaggioImpiantiVisualizzato){
            let impianti_selezionati:Array<GridImpiantoSelezionatoModel> = this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue();

            if(impianti_selezionati && impianti_selezionati.length > 0){

                let flag_copertura_diversa = false;

                let flag_finalita_diversa = false;

                let listErroriGias = [];

                for(let i of impianti_selezionati){
                    if(disciplinare.gruppoFinalita && this.qdcservice.Controlla_GruppoFinalita_Disciplinari(disciplinare,i.Obj_Disciplinare)){
                        //Finalità Compatibile
                    }else{
                        flag_finalita_diversa = true;
                    }

                    if(this.qdcservice.Controlla_Flag_Protetto_Impianti_con_Disciplinari(i.Obj_Disciplinare,disciplinare)){
                        //Copertura Compatibile
                    }else{
                        flag_copertura_diversa = true;
                    }


                    if(flag_finalita_diversa && flag_copertura_diversa){

                        listErroriGias.push(<ErroreGias>{
                            severity: ErroreGias_Severity.WarningBloccante,
                            tipo:  enum_ErroreGias_Tipo.Generico,
                            messaggio: this.translocoService.translate("qdc.AttenzioneCoperturaFinalitaDPIDiversaRispettoAgliImpiantiSelezionati",{ Disciplinare: disciplinare.Descrizione_Concatenata })
                        });
                    }else{
                        if(flag_finalita_diversa){
                            listErroriGias.push(<ErroreGias>{
                                severity: ErroreGias_Severity.WarningBloccante,
                                tipo:  enum_ErroreGias_Tipo.Generico,
                                messaggio: this.translocoService.translate("qdc.AttenzioneFinalitaDPIDiversaRispettoAgliImpiantiSelezionati",{ Disciplinare: disciplinare.Descrizione_Concatenata })
                            });
                        }

                        if(flag_copertura_diversa){
                            listErroriGias.push(<ErroreGias>{
                                severity: ErroreGias_Severity.WarningBloccante,
                                tipo:  enum_ErroreGias_Tipo.Generico,
                                messaggio: this.translocoService.translate("qdc.AttenzioneCoperturaDPIDiversaRispettoAgliImpiantiSelezionati",{ Disciplinare: disciplinare.Descrizione_Concatenata })
                            });
                        }
                    }

                    if(listErroriGias && listErroriGias.length > 0){
                        this.qdcservice.gestisci_ErroriGias(listErroriGias,false,false,"",Dialog_Type.info,'45%','30%').then();
                        break;
                    }

                }


            }
        }


    }

    private ResettaNumericTextboxSuperfici(){
        this.qdcservice.SuperficiForm.patchValue({
            Sup_Selezionata: 0,
            Sup_Trattata: 0
        });

        this.misceleservice.calcoloMiscele('Sup_Trattata',null,0, this.qdcservice.SuperficiForm.get("Sup_Trattata").value);
    }

    private ResettaNumericTextboxQuantita(): void {
        this.qdcservice.QuantitaForm.get('Qta_Selezionata').setValue(0);
        this.qdcservice.QuantitaForm.get('Qta_Trattata').setValue(0);
    }

    async getListaSpecieAnimali() {

        if (!this.Array_Specie_Animali) {

            let result = await lastValueFrom(this.specieAnimaliService.leggiListaSpecieAnimali());
            this.Array_Specie_Animali = JSON.parse(JSON.stringify(result));

            this.Array_Specie_Animali.map((c) => this.specieAnimaliService.setCodDescrDDLSpecieAnimale(c));

            //inseriamo l'elemento 'Nessuna Selezione' per la specie animali
            let specieAnimaleSelected = this.qdcservice.TestataVisitaForm.get("SpecieAnimali_Visita").value;
            if (this.Array_Specie_Animali.findIndex(item => item.genere.codice == -1 && item.indirizzoProd.codice == -1 && item.specie.codice == -1) === -1) {
                specieAnimaleSelected = new RisorsaZootecnica();
                specieAnimaleSelected = this.specieAnimaliService.setDefaultDDLSpecieAnimale(specieAnimaleSelected, "NessunaSelezione");
                this.Array_Specie_Animali.unshift(specieAnimaleSelected);
            }

            let defaultItemZoo = this.qdcservice.TestataVisitaForm.get("SpecieAnimali_Visita").value;

            if (!defaultItemZoo) {
                defaultItemZoo = this.Array_Specie_Animali[0];
            }

            this.qdcservice.TestataVisitaForm.patchValue({
                SpecieAnimali_Visita:  defaultItemZoo
            });
        }

    }

    /**
    * @description: Carico tutti i centri aziendali senza fare filtro sulla Specie se:
     * - sto facendo una visita
    *  - sto facendo una Dichiarazione di non utilizzo senza aver selezionato la Specie
     *  - sto facendo un rilievo senza impianti
    * */
    public Leggi_Tutti_Centri_Senza_Filtro_X_Specie():boolean{
      let leggi = false;

      const Operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").getRawValue();

      const flagVisitaValue: boolean = this.qdcservice.TestataForm.get("flagVisita").getRawValue();

      const specie: Specie = this.qdcservice.GetSpeciefromUtilizzoTerreno();

      if(flagVisitaValue ||
        (Operazioni &&
          Operazioni.findIndex(o=>this.qdcservice.Elenco_Operazioni_Non_Utilizzo.includes(+ o.primaryKey.codice)) >-1 &&
          specie.codice === -1) ||
         this.qdcservice.RilievoSenzaImpianti()){

        leggi = true;

      }

      return leggi;
    }

    ngOnDestroy() {
        this.Subs.unsubscribe();
    }

}
