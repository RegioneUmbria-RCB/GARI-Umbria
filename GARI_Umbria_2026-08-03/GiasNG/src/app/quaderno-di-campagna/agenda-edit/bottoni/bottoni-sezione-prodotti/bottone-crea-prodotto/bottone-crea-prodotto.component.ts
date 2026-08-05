import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { Regolamenti } from 'app/Model/metaschema/Regolamenti';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import { SpecieVegetaliService } from 'app/Service/Metaschema/specie-vegetali.service';
import { VarietaService, VarietaxSpecie } from 'app/Service/Metaschema/varieta.service';
import { RegolamentiService } from 'app/Service/Metaschema/regolamenti.service';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { QdCProdottiService } from 'app/quaderno-di-campagna/agenda-edit/service/prodotti.service';
import { QdCService } from 'app/quaderno-di-campagna/agenda-edit/service/qdc.service';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { Crea_Prodotti, ProdottiService } from 'app/Service/Anagrafica/prodotti.service';
import { TranslocoService } from '@jsverse/transloco';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { enum_Cod_Regolamento } from 'app/Model/TipiEnumerativi';
import {Impianti} from '../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';

@Component({
  standalone: false,
  selector: 'app-bottone-crea-prodotto',
  templateUrl: './bottone-crea-prodotto.component.html',
  styleUrls: ['./bottone-crea-prodotto.component.css'],
  providers: [ GiasDropDownTemplateService ]
})
export class BottoneCreaProdottoComponent implements OnInit {

    @ViewChild('CreaSementiTemplate') creaSementiTemplate: TemplateRef<any>;

    public creaProdottoForm: FormGroup;
    public faPlus = faPlus;

    private formDialog: DialogRef;
    private ProdottiForm: FormGroup;
    private _specie: Array<Specie> = [];
    private _varieta: Array<Varieta> = [];
    private _regs: Array<Regolamenti> = []; // enum_Cod_Regolamento

    constructor(private fb: FormBuilder,
              private transloco: TranslocoService,
              private dialogService: GiasDialogService,
              private parent: FormGroupDirective,
              private specieService: SpecieVegetaliService,
              private varietaService: VarietaService,
              private regolamentiService: RegolamentiService,
              private prodottiService: ProdottiService,
              private qdcservice: QdCService,
              public qdcProdottiService: QdCProdottiService,
    ) { }

    // Ritorna la lista di specie selezionabili dall'utente
    get specie(): Array<Specie> {
        if (this._specie.length <= 0) this.loadSpecie();
        return this._specie;
    }

    // Ritorna la lista di varietà selezionabili dall'utente
    get varieta(): Array<Varieta> {
        return this._varieta;
    }

    // Ritorna la lista di regolamenti selezionabili dall'utente
    get regolamenti(): Array<Regolamenti> {
        if (this._regs.length <= 0) this.loadRegolamenti();
        return this._regs;
    }

    ngOnInit(): void {
        this.ProdottiForm = <FormGroup> this.parent.form;
    }

    onClick() {
        this.initForm();
        this.handleFormChanges();

        this.formDialog = this.dialogService.dialogMessageRef(
            this.transloco.translate('CreaNuovoProdotto'),
            this.creaSementiTemplate,
            []
        );
    }

    async Submit() {
        let f = this.creaProdottoForm.value;
        if (!f.specie || !f.varieta?.descrizione || !f.reg) {
            this.dialogService.baseError('', 'TuttiCampiNecessari');
            return;
        }
        let R = await this.prodottiService.Crea_Prodotti(this.setParametriCreaProdotto());
        if (R['RispostaOK']) {
            const rs = R['RispostaStringa']
            let msg = this.transloco.translate('NProdottiCreati', rs)
            if (!(rs.sementiCreati && rs.trasformatiCreati) && (rs.sementiEsistenti || rs.trasformatiEsistenti)) {
                msg = msg.concat('\n');
                msg = msg.concat(this.transloco.translate('NotaProdottiEsistenti'));
            }
            this.dialogService.baseSuccess('', msg,false);
        } else {
          this.dialogService.baseError('', 'ErroreSalvataggio');
        }
        this.formDialog.close();
    }

    private setParametriCreaProdotto() {
        const f = this.creaProdottoForm.value;
        const creaProdotti = new Crea_Prodotti();
        creaProdotti.varieta = new Varieta();
        creaProdotti.varieta.codice = f.varieta.codice;
        creaProdotti.varieta.descrizione = f.varieta.descrizione;
        creaProdotti.varieta.specie = new Specie(f.specie);
        creaProdotti.regolamento = new Regolamenti(f.reg);
        creaProdotti.creaSemente = true;
        creaProdotti.creaTrasformatoVegetale = true;
        return creaProdotti
    }

    isDisabled(){
        let disabilita = false;

        if(!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()
            && this.ProdottiForm.get("Riga_Salvata").value)
          disabilita = true;

        return disabilita;
    }

    private initForm() {
        let specie = this.qdcservice.GetSpeciefromUtilizzoTerreno()

        this.creaProdottoForm = this.fb.group({
          specie: new FormControl(specie.codice, Validators.required),
          varieta: new FormControl('', Validators.required),
          // Regolamento, TODO: sentire con Fede se da proporre fin da subito o meno
          reg: new FormControl(1, Validators.required)
        });
        this.loadVarieta();
    }

    private handleFormChanges() {
        this.creaProdottoForm.get('specie').valueChanges.GiasSubscribe(ch => {
          this.creaProdottoForm.get('varieta').patchValue(new BaseCodeDescr(0));
          this.loadVarieta();
        });
    }

    private async loadSpecie(): Promise<Array<Specie>> {
        let specie: Array<Specie> = await this.specieService.leggi_FiltroUtente();
        this._specie = specie;
        return specie;
    }

    private async loadVarieta(): Promise<Array<Varieta>> {
        let vars: Array<Varieta> = await this.varietaService.leggi(
            new Specie(this.creaProdottoForm.get('specie').value));
        this._varieta = vars;
        return vars;
        }

        private loadRegolamenti() {
        let regs = [];
        regs.push(new BaseCodeDescr(
            enum_Cod_Regolamento.Regolamento_bio,
            this.transloco.translate('RegCE834/07')
        ));
        regs.push(new BaseCodeDescr(
            enum_Cod_Regolamento.Regolamento_Nessuno,
            this.transloco.translate('RegNessuno')
        ));

        this._regs = regs;
        //this.regolamentiService.leggi().GiasSubscribe(regs => this._regs = regs);
    }

}
