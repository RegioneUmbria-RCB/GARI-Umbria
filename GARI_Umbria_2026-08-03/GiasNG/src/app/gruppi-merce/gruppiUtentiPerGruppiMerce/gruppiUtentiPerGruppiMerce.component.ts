import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ControlContainer, FormGroupDirective } from '@angular/forms';
import { MessageService } from '@progress/kendo-angular-l10n';
import { NotificationService } from '@progress/kendo-angular-notification';
import { KendoImpresaRow } from 'app/anagrafica/imprese/imprese.model';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { ImpreseService } from 'app/Service/Anagrafica/imprese.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { DropdownListItem, KendoGridRow, KendoServerResult } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { map } from 'rxjs/operators';
import { GruppiUtentiPerGruppiMerceSerivce } from './gruppiUtentiPerGruppiMerce.service';
import { GruppiMerceGridRow, GruppiPanelCtrl, GruppiUtentiGridRow, ImpreseDDLCtrlObj, PublicServices, RigheSelezionate } from './utils';

@Component({
  standalone: false,
  selector: 'gruppi-utentix-gruppi-merce',
  templateUrl: './gruppiUtentiPerGruppiMerce.component.html',
  styleUrls: ['./gruppiUtentiPerGruppiMerce.component.scss'],
  providers: [GruppiUtentiPerGruppiMerceSerivce]
})
export class GruppiUtentiPerGruppiMerce implements OnInit
{
    get impreseCtrl()
    {
        return this.masterPageService.impreseCtrl;
    }

    get gridServices()
    {
        return this.masterPageService.gridServices;
    }

    public gruppiPanelCtrl = new GruppiPanelCtrl({
        disabled: true,
        expanded: false,
        shown: false
    });


    constructor(
        private masterPageService: GruppiUtentiPerGruppiMerceSerivce,
        private impreseSer: ImpreseService,
        private notify: GiasMessageService,
        private permessiUtenteService: PermessiUtenteService)
    {
        this.impreseCtrl.modificaAbilitata = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Gruppi_Merce, 2);
    }

    ngOnInit(): void
    {
        this.impreseSer.leggiImprese().pipe(map((data: any) => {
            this.impreseCtrl.imprese = (data.kendo_rows as KendoImpresaRow[])
                .map(impresa => new DropdownListItem(impresa.Piva, impresa.Rag_Soc));

            this.impreseCtrl.source = this.impreseCtrl.imprese.slice();
            this.impreseCtrl.loading = false;
        })).subscribe(); // unsubscribe is made automatically
    }

    onValueChange(event: any)
    {
        this.impreseCtrl.selected = event;

        let mostraTutto = this.impreseCtrl.voceSelezionataMostraTutto();
        this.gruppiPanelCtrl.changeStateIfNeeded(mostraTutto);

        this.masterPageService.aggiornaGruppiMerceGrid.next();
    }

    onUpdatePermissions()
    {
        let righeSelezionate: RigheSelezionate = this.parseSelectedData();

        if(righeSelezionate.GruppiMerce.length === 0 || righeSelezionate.GruppiUtenti.length === 0)
        {
            this.notify.errorMessage('grMerci.GruppiNonSelezionati', false, true);
            return;
        }


        this.masterPageService.setRigheSelezionate(righeSelezionate);
        this.masterPageService.gridServices[PublicServices.GruppiUtentiPerGruppiMerce].refresh(true);
    }


    parseSelectedData(): RigheSelezionate
    {

        const verificaSelezionate = (righe: KendoGridRow[]) => righe.filter(riga => riga['Selected']);
        const parseRigheSelezionate =
            (pubService: GridPublicService) => verificaSelezionate(pubService.value.data.rows);

        let utenti = parseRigheSelezionate(this.gridServices[PublicServices.GruppiUtenti]) as GruppiUtentiGridRow[];
        let merce = parseRigheSelezionate(this.gridServices[PublicServices.GruppiMerce]) as GruppiMerceGridRow[];

        return new RigheSelezionate({
            GruppiUtenti: utenti,
            GruppiMerce: merce,
        });
    }

    handleFilter(value: any)
    {
        this.impreseCtrl.imprese = this.impreseCtrl.source.filter(
            (s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1
          );
    }
}
