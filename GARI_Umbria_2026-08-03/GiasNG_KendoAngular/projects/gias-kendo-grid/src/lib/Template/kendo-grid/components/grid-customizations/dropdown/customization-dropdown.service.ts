import { Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { BehaviorSubject } from 'rxjs';
import { DropdownList, DropdownListItem as DropdownItem, DropdownListItem } from '../../../models/grid.model';
import { DropdownChanged, InMemoryView } from '../model';
import { GiasMessageService } from '../../../../../shared/gias-message.service';


@Injectable()
export class CustomizeDropdownService
  extends BehaviorSubject<DropdownChanged> {

  constructor(
    private transloco: TranslocoService,
    private giasMessageService: GiasMessageService) {
    super(null);
  }

  public extractDropdownItems(data: InMemoryView[]): DropdownItem[] {
    const items: DropdownItem[] = [];
    data.forEach((view: InMemoryView) => {
      items.push(new DropdownItem(view.IdVista, view.NomeVista, { 
        Predefinita: view.Predefinita, 
        NomeUtente: view.NomeUtente, 
        GridId: view.GridId,
        IsNew: view.IsNew
      }));
    });
    return items;
  }

  public generateDropdownId(GridId: string): string {
    return `${GridId}|dropdown`;
  }

  public getItemId(GridId: string, id: string, NomeUtente: string): string {
    // return `${GridId}-${NomeUtente}-item-${id}`;
    return `${GridId}-${NomeUtente}`;
  }

  public generateUID() {
    //return Math.random().toString(16).slice(2);
    return window.crypto.getRandomValues(new Uint32Array(1))[0].toString(16)
  }

  public nextSelected(dropdown: DropdownList): DropdownItem {
    const data = this.value?.dropdown?.data;
    const next = data.filter(d => d.id !== dropdown.id);

    if (next.length) {
      return next[0];
    }
    return dropdown.defaultValue;
  }

  public getNewDDL(input: DropdownList, items: DropdownListItem[]) {
    const r = Object.assign({}, input);
    r.data = items.slice();
    return r;
  }

  public showMessage(message) {
    let translatedMessage = this.transloco.translate(message, {});
    this.giasMessageService.warningMessage(translatedMessage);
  }

}
