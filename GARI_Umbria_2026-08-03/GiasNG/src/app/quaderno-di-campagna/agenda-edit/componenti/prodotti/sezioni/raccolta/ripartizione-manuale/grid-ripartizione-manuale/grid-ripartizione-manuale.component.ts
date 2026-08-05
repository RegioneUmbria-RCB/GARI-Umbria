import { Component, DoCheck } from '@angular/core';
import { GridRipManualeConfigService } from 'app/quaderno-di-campagna/agenda-edit/componenti/prodotti/sezioni/raccolta/ripartizione-manuale/grid-ripartizione-manuale/grid-ripartizione-manuale.service';
import { generateGridProviders } from 'gias-kendo-grid';

@Component({
  standalone: false,
  selector: 'app-grid-ripartizione-manuale',
  templateUrl: './grid-ripartizione-manuale.component.html',
  styleUrls: ['./grid-ripartizione-manuale.component.css'],
  providers: [...generateGridProviders(GridRipManualeConfigService, GridRipartizioneManualeComponent)]
})
export class GridRipartizioneManualeComponent implements DoCheck {
  private readonly ENABLE_LOG = false;
  private _hasListener = false;
  private _hasBlurredInput = false;
  private _inputPresetn = false;

  ngDoCheck() {
    this.handleNumericTextboxFocus();
  }

  private handleNumericTextboxFocus() {
    const gridBody = document.getElementById('gridRipMan').querySelector('tbody');
    const numericTextbox = gridBody?.querySelector('kendo-numerictextbox');
    if (!!numericTextbox && !this._inputPresetn) {
      this._hasBlurredInput = false;
      this.log('INPUT_FOUND');
      const input = numericTextbox.querySelector('input');
      if (document.activeElement !== input && !this._hasBlurredInput) {
        input.focus();
        this.log('INPUT_FOCUSED');
      }
      if (!this._hasListener) {
        document.addEventListener('click', (event) => {
          if (event.target !== input && !this._hasBlurredInput) {
            input.blur();
            this._hasBlurredInput = true;
            this.log('BLURRING_INPUT');
          }
        });
        this._hasListener = true;
        this.log('LISTENER_SET');
      }
    }
    this._inputPresetn = !!numericTextbox; // here to avoid skipping logic
  }

  private log(text: string) {
    if (this.ENABLE_LOG) {
      console.debug(text);
    }
  }
}
