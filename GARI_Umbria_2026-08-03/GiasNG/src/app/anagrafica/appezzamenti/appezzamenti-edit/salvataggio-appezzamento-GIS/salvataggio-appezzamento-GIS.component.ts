import {Component, EventEmitter, Inject, Input, OnDestroy, OnInit, Output, TemplateRef} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import { ObjParametriAgendaService} from '../../../../Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {ActivatedRoute} from '@angular/router';
import {Enum_AssociazioneGIS} from '../../../../GIS/GIS-kendo-window/polygon-window/polygon-window.component';
import {Subject, takeUntil} from 'rxjs';
import {TranslocoService} from '@jsverse/transloco';
import {BaseCodeDescrStr} from 'app/Model/baseClass/baseCodeDescrStr';
import {BaseCodeDescr} from 'app/Model/baseClass/baseCodeDescr';
import { ImpiantiFactoryService, IMPIANTI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/impianti.factory.service';
import {Entita_Info_Out, VerificaVicini_In} from '../../../../Service/api.service';
import {SalvataggioAppezzamentoGISService} from './salvataggio-appezzamento-GIS.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import {GiasDialogService} from '../../../../Service/gias-dialog.service';

@Component({
    standalone: false,
    styleUrls: ['./salvataggio-appezzamento-GIS.component.css'],
    selector: 'salvataggio-appezzamento-GIS',
    templateUrl: './salvataggio-appezzamento-GIS.component.html',
    providers: [SalvataggioAppezzamentoGISService]
})
export class SalvataggioAppezzamentoGISComponent implements OnInit, OnDestroy{

    public signal$: Subject<void> = new Subject();

    @Input('presaveForm') PresaveForm: FormGroup = this.fb.group({});
    @Output() ModificaDatiAnagrafica: EventEmitter<boolean> = new EventEmitter<boolean>();
    public objParametriAgenda: ObjParametriAgenda;
    public prossimitaList: Array<Entita_Info_Out>;

    public isNew: boolean = false;
    public createFrom: boolean = false;
    private wkt: string;
    private entitaCod: string = '';

    objNessuno: BaseCodeDescrStr = null;
    objImpianto: BaseCodeDescrStr = null;
    objAppezzamento: BaseCodeDescrStr = null;
    objImpiantoAppezzamento: BaseCodeDescrStr = null;

    objImpiantoNum: BaseCodeDescr = null;
    objAppezzamentoNum: BaseCodeDescr = null;
    objImpiantoAppezzamentoNum: BaseCodeDescr = null;

    tipoPoligono: Array<any> = [];
    associa: Array<any> = [];
    memorizza: Array<any> = [];

    ripartoCatastoPresente: boolean = null;
    readonly nessunaAssociazione = Enum_AssociazioneGIS.Nessuno.toString();

    constructor(
        private route: ActivatedRoute,
        private fb: FormBuilder,
        private parametriAgendaService: ObjParametriAgendaService,
        @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
        private translocoService: TranslocoService,
        private salvataggioAppezzamentoGISService: SalvataggioAppezzamentoGISService,
        private sharedDataService: SharedDataService,
        private giasDialogService: GiasDialogService
    ) {
        this.objParametriAgenda = this.parametriAgendaService.getObjParamValue();
        this.isNew = this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write;

        this.caricaArrayObjBase();
        this.caricaArrayTipoPoligono();

        this.appezzamentiService.leggiGenerazionePoligoniDefaultValue(this.objParametriAgenda).subscribe(r => {
            if(r != '') {
                this.PresaveForm.controls['generaSuiLayer'].setValue({codice: Number.parseInt(r)});
            } else {
                this.PresaveForm.controls['generaSuiLayer'].setValue({codice: Enum_AssociazioneGIS.AppezzamentoEImpianto});
            }
        });

        this.appezzamentiService.currentRipartoCatastoPresente
            .pipe(takeUntil(this.signal$)).subscribe(ripartoCatastoPresente => {
                this.seCaricaArrayAssocia(ripartoCatastoPresente);
        });
    }

    caricaArrayObjBase() {
        this.objNessuno = new BaseCodeDescrStr(Enum_AssociazioneGIS.Nessuno.toString());
        this.objNessuno.descrizione = this.translocoService.translate("Nessuno");
        //
        this.objImpianto = new BaseCodeDescrStr(Enum_AssociazioneGIS.Impianto.toString());
        this.objImpianto.descrizione = this.translocoService.translate("Impianto");
        //
        this.objAppezzamento = new BaseCodeDescrStr(Enum_AssociazioneGIS.Appezzamento.toString());
        this.objAppezzamento.descrizione = this.translocoService.translate("Appezzamento");
        //
        this.objImpiantoAppezzamento = new BaseCodeDescrStr(Enum_AssociazioneGIS.AppezzamentoEImpianto.toString());
        this.objImpiantoAppezzamento.descrizione = this.translocoService.translate("ImpiantoAppezzamento");
        //
        this.objImpiantoNum = new BaseCodeDescr(Enum_AssociazioneGIS.Impianto);
        this.objImpiantoNum.descrizione = this.translocoService.translate("Impianto");
        //
        this.objAppezzamentoNum = new BaseCodeDescr(Enum_AssociazioneGIS.Appezzamento);
        this.objAppezzamentoNum.descrizione = this.translocoService.translate("Appezzamento");
        //
        this.objImpiantoAppezzamentoNum = new BaseCodeDescr(Enum_AssociazioneGIS.AppezzamentoEImpianto);
        this.objImpiantoAppezzamentoNum.descrizione = this.translocoService.translate("AppezzamentoImpianto");
    }

    ngOnDestroy(): void {
        this.signal$.next();
        this.signal$.complete();
    }

    ngOnInit(): void {
        // Memorizza
        this.caricaArrayMemorizza();
        let supOriginalePoligono = 0;
        if (this.isAppezzamento()) {
            this.PresaveForm.controls['memorizza'].setValue(this.objAppezzamento);
            supOriginalePoligono = this.PresaveForm.controls['supOriginaleApp'].value;
        } else {
            this.PresaveForm.controls['memorizza'].setValue(this.objImpiantoAppezzamento);
            supOriginalePoligono = this.PresaveForm.controls['supOriginaleImp'].value;
        }
        this.PresaveForm.controls['supOriginale'].setValue(supOriginalePoligono);
        // Associa
        this.seCaricaArrayAssocia(false);
        this.PresaveForm.controls['associa'].setValue(this.objNessuno);
        // Query Params
        this.handleQueryParams();

        if (this.createFrom) {
            this.PresaveForm.controls['associa'].setValue(this.objImpianto);
            this.PresaveForm.controls['memorizza'].setValue(this.objImpiantoAppezzamento);
        }

        this.PresaveForm.controls['associa'].valueChanges.subscribe( v => {
            if(v.codice !== Enum_AssociazioneGIS.Nessuno)
                this.PresaveForm.controls['supNuova']?.setValue(this.PresaveForm.controls['supCalcolata'].value);
        });
    }

    caricaArrayTipoPoligono() {
        this.tipoPoligono.push(this.objAppezzamentoNum);
        this.tipoPoligono.push(this.objImpiantoNum);
        this.tipoPoligono.push(this.objImpiantoAppezzamentoNum);
    }

    caricaArrayMemorizza() {
        this.memorizza = [];
        if (this.isAppezzamento()) {
            this.memorizza.push(this.objAppezzamento);
        } else {
            this.memorizza.push(this.objImpianto);
            this.memorizza.push(this.objImpiantoAppezzamento);
        }
    }

    seCaricaArrayAssocia(ripartoCatastoPresente: boolean) {
        if (ripartoCatastoPresente !== this.ripartoCatastoPresente) {
            this.ripartoCatastoPresente = ripartoCatastoPresente;
            this.caricaArrayAssocia();
        }
    }

    caricaArrayAssocia() {
        this.associa = [];
        this.associa.push(this.objNessuno);
        if (this.isAppezzamento()) {
            if (!this.ripartoCatastoPresente) {
                this.associa.push(this.objAppezzamento);
            }
        } else {
            this.associa.push(this.objImpianto);
            if (!this.ripartoCatastoPresente) {
                this.associa.push(this.objImpiantoAppezzamento);
            }
        }
    }

    private isAppezzamento(): boolean {
        return this.objParametriAgenda.Id_Reg === 0 || this.objParametriAgenda.Id_Reg === undefined
    }

    private handleQueryParams(): void {
        const params = this.route.snapshot.queryParams;
        if (params.area !== undefined && params.area !== '') {
            this.PresaveForm.controls['supCalcolata'].setValue(params.area);
        }

        if (params.createFrom !== undefined && params.createFrom !== '') {
            this.createFrom = params.createFrom;
        }

        if (params.wkt != undefined && params.wkt != '') {
            this.wkt = params.wkt;
        }

        if (params.entitaCod != undefined && params.entitaCod != '') {
            this.entitaCod = params.entitaCod;
        }
    }

    public modificaDatiAnagrafici() {
        this.ModificaDatiAnagrafica.emit(true);
    }

    public setSupNuova(): void {
        if (this.PresaveForm.controls['associa'].value !== Enum_AssociazioneGIS.Nessuno)
            this.PresaveForm.controls['supNuova'].setValue(this.PresaveForm.controls['supCalcolata'].value);
    }

    public isSementi(): boolean {
        return this.sharedDataService.getCfgSementiAsValue() != undefined;
    }

    public verficaVicini(template: TemplateRef<any>): void {
        if(this.isSementi) {
            console.log('Vicini in corso di verifica');

            let vv: VerificaVicini_In = {};
            vv.Sementi = this.sharedDataService.getCfgSementiAsValue().Sementi;
            vv.SementiMappaturaLibera = this.sharedDataService.getCfgSementiAsValue().SementiMappaturaLibera;
            vv.Entita_Cod = this.entitaCod;
            vv.HiddenPunti =
                '(' +
                this.wkt.substring(this.wkt.lastIndexOf('('))
                    .replace('))','')
                    .replace('(', '')
                    .split(', ')
                    .map(ll => {
                        let cc = ll.split(' ');
                        return cc[1] + ' ' + cc[0];
                    })
                    .join('),(')
                    .concat(')')
                    .split(' ')
                    .join(', ');
            vv.Validita_Inizio = this.PresaveForm.controls['validita'].getRawValue().inizio;
            vv.Validita_Fine = this.PresaveForm.controls['validita'].getRawValue().fine;
            vv.Grva_Cod = this.PresaveForm.controls['grvaCod'].getRawValue();
            vv.Veg_Cod = this.PresaveForm.controls['vegCod'].getRawValue();

            if (vv.Grva_Cod == 0 || vv.Veg_Cod == 0) {
                this.giasDialogService.baseInfo('appezzamento.VerificaProssimita', 'appezzamento.PerPoterUtilizzareQuestaFunzionalitaSelezionaPrimaUnaSpecieEdUnaTipologiaVarietale');
                return;
            }

            this.salvataggioAppezzamentoGISService.verificaVicini(vv).subscribe(v => {
                console.log(v);
                if(v.RispostaStringa.Messaggio != '')
                    this.giasDialogService.baseSuccess(this.translocoService.translate('appezzamento.VerificaProssimita'), v.RispostaStringa.Messaggio);
                else {
                    this.prossimitaList = v.RispostaStringa.Elenco_EntitaVicine;
                    //     .forEach(ev => {
                    //     let row: string = ev.Referente + ' ' + ev.Indirizzo + ' ' + ev.Veg_Des + ' ' + ev.Distanza;
                    //     this.prossimitaList.push(row);
                    // });
                    this.giasDialogService.baseInfo(this.translocoService.translate('appezzamento.VerificaProssimita'), template);
                }
            });
        }
    }
}
