import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { CatastoCentroAziendale } from 'app/Model/anagrafiche/CatastoCentroAziendale';
import { Controllo, TipoControllo } from 'app/Model/Controllo';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { Comune, Provincia } from 'app/Model/MetaschemaModel';
import { MasterService } from 'app/Service/master.service';
import { from, map, of, Subject, Subscription, switchMap, take, takeUntil, tap } from 'rxjs';
import { CatastoEditService } from './catasto-edit.service';
import { Location } from '@angular/common';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { CentriAziendaliService, LeggiCentriAziendali } from 'app/Service/Anagrafica/centri.service';
import { GiasDropDownTemplateSComponent, ObjParametriAgenda } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { PossessoParticella } from 'app/Model/anagrafiche/PossessoParticella';
import { ParticelleCatastaliZona } from 'app/Model/anagrafiche/ParticelleCatastaliZona';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { PossessiService } from './catasto-possessi-edit/Possessi.service';
import { Router } from '@angular/router';
import { ParticelleCatastali } from 'app/Model/anagrafiche/ParticelleCatastali';
import { ImpreseFactoryService, IMPRESE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/imprese.factory.service';
import { CatastoFactoryService, CATASTO_SERVICE_TOKEN } from 'app/Service/ServiceFactory/catasto.factory.service';
import { DialogCloseResult } from '@progress/kendo-angular-dialog';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { ActivatedRoute } from '@angular/router';
import { MenuContestualeService } from "../../../Master/menu-contestuale/menu-contestuale.service";
import { TranslocoService } from '@jsverse/transloco';
import { CatastoEditUtilityService } from './catasto-edit-utility.service';
import {GiasIstatService} from '../../../Service/istat/gias-istat.service';

@Component({
  standalone: false,
  selector: 'app-catasto-edit',
  templateUrl: './catasto-edit.component.html',
  styleUrls: ['./catasto-edit.component.css'],
  providers: [CatastoEditService, GiasDropDownTemplateService, CatastoEditUtilityService],
})
export class CatastoEditComponent implements OnInit, OnDestroy {
  subscriptions: Subscription[] = new Array<Subscription>();
  saving: boolean = false;
  mostraForm: boolean;

  catastoForm: FormGroup = this.fb.group({
    centro: this.fb.group({
      codice: [],
      partitaIva: []
    }),
    particella: this.catastoEditUtilityService.getFormParticella(),
    possessiParticella: this.fb.array([]),
    flag_cancellazione: [false]
  });

  objParametriAgenda: ObjParametriAgenda;
  lista_Centri: Array<{ codice: number; descrizione: string }>;

  private signal: Subject<void> = new Subject();
  private inFrame: boolean = false;
  private nuovoPossesso: PossessoParticella = new PossessoParticella()

  private firstChangeProv = true;

  Provincie: Provincia[];
  Comuni: Comune[];

  constructor(
    private catastoEditService: CatastoEditService,
    @Inject(CATASTO_SERVICE_TOKEN) private catastoService: CatastoFactoryService,
    private fb: FormBuilder,
    private istatService: GiasIstatService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private centriService: CentriAziendaliService,
    private giasMessageService: GiasMessageService,
    private location: Location,
    private masterService: MasterService,
    private possessiHandlerService: PossessiService,
    private giasDialogService: GiasDialogService,
    private router: Router,
    private translocoService: TranslocoService,
    private menuContestualeService: MenuContestualeService,
    private route: ActivatedRoute,
    @Inject(IMPRESE_SERVICE_TOKEN) private impreseService: ImpreseFactoryService,
    private catastoEditUtilityService: CatastoEditUtilityService
  ) {
    this.catastoForm.controls["centro"].valueChanges.subscribe(() => {
      this.CentroChanges();
    });
  }

  ngOnInit() {
    this.saving = false
    this.route.queryParams
      .subscribe(params => {
        if (params.seFrame == 1) {

          this.inFrame = true;

          let header = this.masterService.getHeader();
          header.visible = false;
          this.masterService.changeHeader(header);

          let footer = this.masterService.getFooter();
          footer.visible = false;
          this.masterService.changeFooter(footer);

          let menuContestuale = this.menuContestualeService.getMenuContestualeSettings();
          menuContestuale.show = false;
          this.menuContestualeService.changeMenuContestualeSettings(menuContestuale);

          if (params.IxP_TitoloPossesso != undefined
            && params.IxP_ValiditaFine != undefined
            && params.IxP_ValiditaInizio != undefined) {
            this.nuovoPossesso.codice = parseInt(params.IxP_TitoloPossesso)
            this.nuovoPossesso.validita = new IntervalloTemporale(
              new Date(params.IxP_ValiditaInizio.substring(0, 4)
                + "-" + params.IxP_ValiditaInizio.substring(4, 6)
                + "-" + params.IxP_ValiditaInizio.substring(6, 8)),
              new Date(params.IxP_ValiditaFine.substring(0, 4)
                + "-" + params.IxP_ValiditaFine.substring(4, 6)
                + "-" + params.IxP_ValiditaFine.substring(6, 8)))
          } else {
            this.nuovoPossesso.codice = 0
            this.nuovoPossesso.validita = new IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE)
          }
        }
      }
      );

    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
      this.catastoForm.disable();
    }

    this.mostraForm = false;
    this.subscriptions.push(this.catastoEditService.particellaEditSource.subscribe(
      async (data: CatastoCentroAziendale) => {
        this.catastoForm.patchValue(data);
        if (data.possessiParticella !== undefined) {
          for (const possesso of data.possessiParticella) {
            const possessoForm = this.catastoEditUtilityService.getRowPossessiParticella();
            possessoForm.patchValue(possesso, { emitEvent: false });
            const faPossessi = <FormArray>this.catastoForm.controls['possessiParticella'];
            faPossessi.push(possessoForm);
          }

          for (const metodoProduzione of data.particella.metodoProduzione) {
            const metodoProduzioneForm = this.catastoEditUtilityService.getRowMetodoProduzioneParticella();
            metodoProduzioneForm.patchValue(metodoProduzione, { emitEvent: false });
            const faMetodiProduzione = <FormArray>(<FormGroup>this.catastoForm.controls['particella']).controls['metodoProduzione'];
            faMetodiProduzione.push(metodoProduzioneForm);
          }

          for (const macrouso of data.particella.macrousi) {
            const macrousoForm = this.catastoEditUtilityService.getRowMacrousoParticella();
            macrousoForm.patchValue(<any>macrouso, { emitEvent: false });
            const faMacrousi = <FormArray>(<FormGroup>this.catastoForm.controls['particella']).controls['macrousi'];
            faMacrousi.push(macrousoForm);
          }

          for (const zona of data.particella.zonizzazione) {
            const zonaForm = this.catastoEditUtilityService.getRowZonaParticella();
            zonaForm.patchValue(zona, { emitEvent: false });
            const faZone = <FormArray>(<FormGroup>this.catastoForm.controls['particella']).controls['zonizzazione'];
            faZone.push(zonaForm);
          }

          for (const classamento of data.particella.classamento) {
            const classamentoForm = this.catastoEditUtilityService.getRowClassamentoParticella();
            classamentoForm.patchValue(<any>classamento, { emitEvent: false });
            const faClassamenti = <FormArray>(<FormGroup>this.catastoForm.controls['particella']).controls['classamento'];
            faClassamenti.push(classamentoForm);
          }

          this.Provincie = await this.istatService.leggiProvincie('');
          if (this.catastoForm.get('particella.primaryKey.Prov').value !== '000' && this.catastoForm.get('particella.primaryKey.Prov').value !== '') {
            this.Comuni = await this.istatService.leggiComuni(this.catastoForm.get('particella.primaryKey.Prov').value);
          }
          this.mostraForm = true;
          if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
            this.catastoForm.disable();
          }
        }
      }
    ));

    this.subscriptions.push(this.catastoService.particellaEditSource.subscribe((catastoEdit) => {
      if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Update) {
        this.catastoEditService.leggiParticellaEdit(catastoEdit);
      } else if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
        this.catastoEditService.leggiParticellaEdit(catastoEdit);
      } else if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
        let lastParticella = this.catastoService.ultimaParticellaInserita;
        if (lastParticella == null) {
          lastParticella = {
            sa_cod: 0,
            sa_des: '',
            prov: '000',
            prov_des: '',
            com: '000',
            com_des: '',
            sezione: '',
            foglio: 0
          }
        }
        this.impreseService.impresaBiologica(this.objParametriAgenda.Piva).pipe(
          take(1),
          tap((bio) => {
            let metodoProduzione_Cod = 1
            let metodoProduzione_Des = "Integrato"
            if (bio) {
              metodoProduzione_Cod = 3
              metodoProduzione_Des = "Biologico"
            }
            let catasto: CatastoCentroAziendale = {
              centro: {
                codice: lastParticella.sa_cod,
                partitaIva: this.objParametriAgenda.Piva
              },
              particella: {
                Area: 0,
                macrousi: [],
                zonizzazione: [],
                classamento: [],
                metodoProduzione: [{
                  metodoProduzione: { codice: metodoProduzione_Cod, descrizione: metodoProduzione_Des },
                  validita: new IntervalloTemporale()
                }],
                proprietario: '',
                primaryKey: {
                  Prov: lastParticella.prov,
                  Com: lastParticella.com,
                  Sezione: lastParticella.sezione,
                  Foglio: lastParticella.foglio,
                  Numero: 0,
                  Subalterno: ''
                }
              },
              possessiParticella: [{
                codice: 0,
                Area: 0,
                titolo_Di_Possesso: { codice: (this.nuovoPossesso.codice != undefined ? this.nuovoPossesso.codice : 1), descrizione: 'Proprietà' },
                validita: (this.nuovoPossesso.validita != undefined ? this.nuovoPossesso.validita : new IntervalloTemporale()),
                flag_cancellazione: false,
                codice_particella: ''
              }],
              flag_cancellazione: false
            }
            this.catastoEditService.changeParticellaEdit(catasto);
          })
        ).subscribe();
      }
    }));

    if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
      let impresa = new Impresa();
      impresa.partitaIva = this.objParametriAgenda.Piva;

      this.centriService.leggiCentriAziendaliModelloQdC(<LeggiCentriAziendali>{
        impresa: impresa,
        data: new Date()
      }, false).then(vals => {
        this.lista_Centri = vals.map(el => { return { codice: el.primaryKey.codice, descrizione: el.nome } });
        if (this.catastoService.ultimaParticellaInserita == null) {
          this.catastoForm.get('centro').setValue({ codice: this.lista_Centri[0].codice, partitaIva: this.objParametriAgenda.Piva });
        } else {
          this.catastoForm.get('centro').setValue({
            codice: this.catastoService.ultimaParticellaInserita.sa_cod,
            partitaIva: this.objParametriAgenda.Piva
          });
        }
      });

      this.catastoForm.get('particella').get('Area').valueChanges.subscribe((val) => {
        const possessi = this.possessiHandlerService.getPossessoParticella();
        possessi.forEach((poss) => {
          poss.Area = val;
        });
        this.possessiHandlerService.setPossessoParticella(possessi);
      })

    }
  }

  ngOnDestroy(): void {
    if (this.subscriptions) {
      for (const subscription of this.subscriptions) {
        subscription.unsubscribe();
      }
    }
  }

  SalvaParticellaNuovo() {
    this.saving = true;
    this.catastoForm.markAllAsTouched();
    this.catastoForm.setValue(this.catastoForm.getRawValue());

    if (this.catastoForm.valid) {
      const controlli = this.controlliParticella(this.catastoForm.value as CatastoCentroAziendale);

      controlli.forEach((controllo) => {
        this.giasMessageService.errorMessage(controllo.message);
      });

      if (controlli.length == 0) {
        this.masterService.set_isLoading({ isLoading: true });
        this.subscriptions.push(this.catastoEditService.particellaEditSource.subscribe(
          (oldValue) => {
            if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
              oldValue = null;
            }
            this.catastoEditService.scriviParticella(oldValue, this.catastoForm.value).subscribe((res) => {
              this.masterService.set_isLoading({ isLoading: false });
              this.saving = false;
              if (res.RispostaOK) {
                let particellaInserita: CatastoCentroAziendale = this.catastoForm.value;
                this.catastoService.ultimaParticellaInserita = {
                  sa_cod: particellaInserita.centro.codice,
                  sa_des: '',
                  prov: particellaInserita.particella.primaryKey.Prov,
                  prov_des: '',
                  com: particellaInserita.particella.primaryKey.Com,
                  com_des: '',
                  sezione: particellaInserita.particella.primaryKey.Sezione,
                  foglio: particellaInserita.particella.primaryKey.Foglio
                }

                this.setParamSalvaNuovo();
                this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

                let currentUrl = this.router.url;
                this.router.routeReuseStrategy.shouldReuseRoute = () => false;
                this.router.onSameUrlNavigation = 'reload';
                this.router.navigate([currentUrl]);

              }
            });
          }
        ));
      } else {
        this.saving = false;
      }
    } else {
      this.saving = false;
      this.catastoForm.markAllAsTouched();
    }
  }

  private setParamSalvaNuovo() {
    let catastoEdit = new CatastoCentroAziendale()
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    catastoEdit.centro = { partitaIva: this.objParametriAgenda.Piva, codice: 0 };
    catastoEdit.particella = new ParticelleCatastali();
    catastoEdit.particella.primaryKey =
    {
      Prov: '000',
      Com: '000',
      Sezione: '',
      Foglio: 0,
      Numero: 0,
      Subalterno: ''
    };
    this.catastoService.changeParticellaEdit(catastoEdit);
  }

  SalvaParticella(nuovo: boolean) {
    this.saving = true;
    this.catastoForm.markAllAsTouched();
    this.catastoForm.setValue(this.catastoForm.getRawValue());

    if (this.catastoForm.valid) {
      this.catastoService.LeggiParticellaAzienda(this.catastoForm.value as CatastoCentroAziendale).pipe(takeUntil(this.signal)).pipe(
        switchMap((res1) => {
          if (res1.particella != undefined && this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
            return this.giasDialogService.dialogMessageObs_Result(
              '',
              this.translocoService.translate('ParticellaEsisteGia'),
              [
                { text: this.translocoService.translate('Ok'), primary: true, returnObj: true }
                //{text: 'Annulla', returnObj: false}
              ],
              undefined,
              undefined,
              e => e instanceof DialogCloseResult
            );
          } else {
            return of([null]);
          }
        }),
        switchMap((res2) => {
          if (!res2['returnObj'] && res2['returnObj'] == undefined && res2[0] == null) {
            if (this.catastoForm.valid) {
              const controlli = this.controlliParticella(this.catastoForm.value as CatastoCentroAziendale);

              controlli.forEach((controllo) => {
                this.giasMessageService.errorMessage(controllo.message);
              });

              if (controlli.length == 0) {
                this.masterService.set_isLoading({ isLoading: true });
                this.subscriptions.push(this.catastoEditService.particellaEditSource.subscribe(
                  (oldValue) => {
                    if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
                      oldValue = null;
                    }
                    this.catastoEditService.scriviParticella(oldValue, this.catastoForm.value)
                      .pipe(take(1)).subscribe((res) => {
                        this.masterService.set_isLoading({ isLoading: false });
                        this.saving = false;
                        if (res != undefined) {
                          if (res.RispostaOK) {
                            let particellaInserita: CatastoCentroAziendale = this.catastoForm.value;
                            this.catastoService.ultimaParticellaInserita = {
                              sa_cod: particellaInserita.centro.codice,
                              sa_des: '',
                              prov: particellaInserita.particella.primaryKey.Prov,
                              prov_des: '',
                              com: particellaInserita.particella.primaryKey.Com,
                              com_des: '',
                              sezione: particellaInserita.particella.primaryKey.Sezione,
                              foglio: particellaInserita.particella.primaryKey.Foglio
                            }
                            if (nuovo) {
                              this.setParamSalvaNuovo();
                              this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
                            }
                            this.gestioneRedirect(nuovo);
                          }
                        }
                      });
                  }
                ));
                return of(null);
              } else {
                this.saving = false;
                return of(null);
              }
            } else {
              this.saving = false;
              this.catastoForm.markAllAsTouched();
              return of(null);
            }
          } else {
            this.saving = false;
            return of(null);
          }
        })
      ).subscribe();
    } else {
      this.saving = false;
    }
  }

  private gestioneRedirect(nuovo: boolean) {
    if (nuovo) {
      let currentUrl = this.router.url;
      this.router.routeReuseStrategy.shouldReuseRoute = () => false;
      this.router.onSameUrlNavigation = 'reload';
      this.router.navigate([currentUrl]);
    } else {
      if (this.inFrame) {
        this.router.navigate(["Anagrafica/Catasto"])
        window.parent.postMessage("chiudiFinestra")
      } else {
        this.location.back();
      }
    }
  }

  private checkParticellaExists(value: CatastoCentroAziendale) {
    return this.catastoService.LeggiParticellaAzienda(value).pipe(
      (res1) => {
        if (res1) {
          return this.giasDialogService.dialogMessageObs_Result(
            '',
            this.translocoService.translate('ParticellaEsisteGia'),
            [
              { text: this.translocoService.translate('Ok'), primary: true, returnObj: true }
              //{text: 'Annulla', returnObj: false}
            ],
            undefined,
            undefined,
            e => e instanceof DialogCloseResult
          );
        } else {
          return of([null]);
        }
      }
    ).subscribe();
  }

  controlliParticella(value: CatastoCentroAziendale): Controllo[] {

    const controlli = new Array<Controllo>();
    // Warning
    const sup_cat = value.particella.Area;

    if (sup_cat == 0) {
      controlli.push({ type: TipoControllo.WARNING, message: "Superficie catastale non può essere 0" })
    }

    // value.possessiParticella.forEach((el) => {
    //   if (el.Area > sup_cat) {
    //     // Warning
    //     controlli.push({ type: TipoControllo.WARNING, message: "Superficie condotta maggiore di superficie catastale" })
    //   }
    // });

    this.controlloValidita(value.possessiParticella, 'possessi').forEach(c => {
      controlli.push(c)
    });

    this.controlloSuperficie(value.possessiParticella, sup_cat).forEach(c => {
      controlli.push(c)
    });

    this.controlloZone(value.particella.zonizzazione).forEach(c => {
      controlli.push(c)
    });

    // Errori
    if (value.centro == undefined) {
      controlli.push({ type: TipoControllo.ERROR, message: "E' necessario impostare un centro" });
    }
    if (value.possessiParticella.length == 0) {
      controlli.push({ type: TipoControllo.ERROR, message: "La particella deve avere almeno un possesso" });
    }
    if (value.particella.primaryKey.Foglio == 0) {
      controlli.push({ type: TipoControllo.ERROR, message: "Il campo Foglio non può essere 0" });
    }
    if (value.particella.primaryKey.Numero == 0) {
      controlli.push({ type: TipoControllo.ERROR, message: "Il campo Numero non può essere 0" });
    }
    if (value.particella.primaryKey.Prov == '' || value.particella.primaryKey.Prov == '000') {
      controlli.push({ type: TipoControllo.ERROR, message: "E' necessario impostare una Provincia valida" });
    }
    if (value.particella.primaryKey.Com == '' || value.particella.primaryKey.Com == '000') {
      controlli.push({ type: TipoControllo.ERROR, message: "E' necessario impostare un Comune valido" });
    }

    this.controlloValidita(value.particella.metodoProduzione, 'Metodi di Produzione').forEach(c => {
      controlli.push(c)
    });

    return controlli;
  }

  private controlloValidita(array: any[], campoDiControllo: string): Controllo[] {
    let periodi = new Array<IntervalloTemporale>();
    let controlli = new Array<Controllo>();
    let e;
    for (e of array) {
      let p;
      for (p of periodi) {
        if ((e.validita.inizio >= p.inizio && e.validita.inizio <= p.fine) ||
          ((e.validita.fine >= p.inizio && e.validita.fine <= p.fine))) {
          controlli.push({ type: TipoControllo.ERROR, message: 'Date ' + campoDiControllo + ' non coerenti/sovrapposte' });
          return controlli;
        }
      }
      periodi.push(new IntervalloTemporale(e.validita.inizio, e.validita.fine));
    }
    return controlli;
  }

  private controlloSuperficie(array: PossessoParticella[], sup_cat: number): Controllo[] {
    let controlli = new Array<Controllo>();
    let e;
    for (e of array) {
      if (e.Area == 0 && sup_cat > 0) {
        controlli.push({ type: TipoControllo.ERROR, message: 'Superficie condotta non può essere 0' });
        return controlli;
      }
    }
    return controlli;
  }

  private controlloZone(array: ParticelleCatastaliZona[]): Controllo[] {
    let zone = new Array<BaseCodeDescr>();
    let controlli = new Array<Controllo>();
    let e;
    for (e of array) {
      let z;
      for (z of zone) {
        if (e.zona.codice == z.codice) {
          controlli.push({ type: TipoControllo.ERROR, message: 'Non è possibile avere Zone ripetute' });
          return controlli;
        }
      }
      zone.push(new BaseCodeDescr(e.zona.codice));
    }
    return controlli;
  }

  private controlloEsistenzaParticella(value: CatastoCentroAziendale) {
    return this.catastoService.LeggiParticellaAzienda(value);
  }

  async ProvinciaChange(val: string) {
    this.catastoForm.get('particella.primaryKey.Com').setValue('000');
    this.Comuni = await this.istatService.leggiComuni(this.catastoForm.get('particella.primaryKey.Prov').value);
  }

  CentroChanges() {
    let sa_cod = this.catastoForm.controls["centro"].value.codice;
    if (sa_cod != null
      && sa_cod != ""
      && sa_cod != 0
      && this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {

      const objP = this.objParametriAgendaService.getObjParamValue();
      from(this.impreseService.leggiImpresa(objP)).pipe(
        take(1),
        map((val) => {
          return val.RispostaStringa;
        }),
        tap((impresa) => {
          this.centriService.leggiCentriAziendaliModelloQdC(<LeggiCentriAziendali>{ impresa: impresa, data: this.objParametriAgenda.Validita_Inizio }, false)
            .then(centri => {
              let centro = centri.find(c => c.primaryKey.codice == this.catastoForm.controls["centro"].value.codice);

              const indirizzo = centro?.indirizzi[0]?.indirizzo;
              const prov = indirizzo?.istatComune?.prov;
              const com = indirizzo?.istatComune?.com;
              const comDes = indirizzo?.istatComune?.comuni_prov;
              if (this.firstChangeProv && this.catastoService.ultimaParticellaInserita != null) {
                this.catastoForm.get('particella.primaryKey.Prov').setValue(this.catastoService.ultimaParticellaInserita.prov);
                this.ProvinciaChange('');
                this.catastoForm.get('particella.primaryKey.Com').setValue(this.catastoService.ultimaParticellaInserita.com);
              } else {
                this.catastoForm.get('particella.primaryKey.Prov').setValue(prov);
                this.ProvinciaChange('');
                this.catastoForm.get('particella.primaryKey.Com').setValue(com);
              }

            });
        })
      ).pipe(take(1)).subscribe();
    }
  }

  async openDdl(ddlEl: GiasDropDownTemplateSComponent) {
    switch (ddlEl.giasFormControlName) {
      case 'codice':
        let impresa = new Impresa();
        impresa.partitaIva = this.objParametriAgenda.Piva;
        ddlEl.listItems = (await this.centriService.leggiCentriAziendaliModelloQdC(<LeggiCentriAziendali>{ impresa: impresa, data: AGRODATAINIZIO }, false)).map(el => {
          return { codice: el.primaryKey.codice, descrizione: el.nome };
        });
        break;
    }
  }

}
