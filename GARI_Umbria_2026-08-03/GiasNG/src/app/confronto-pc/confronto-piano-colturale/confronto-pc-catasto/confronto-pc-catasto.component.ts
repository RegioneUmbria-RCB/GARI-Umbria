import {Component, OnInit} from '@angular/core';
import {ConfrontoPianoColturaleDataService} from '../confronto-piano-colturale-data.service';
import {PianoColturale} from '../../../Model/confronto-piano-colturale/confronto-piano-colturale';

@Component({
  standalone: false,
  selector: 'app-confronto-pc-catasto',
  templateUrl: './confronto-pc-catasto.component.html',
  styleUrls: ['./confronto-pc-catasto.component.css']
})
export class ConfrontoPcCatastoComponent {
  constructor(
    private confrontoPianoColturaleDataService: ConfrontoPianoColturaleDataService
  ) { }

  protected isPlanSelected(): boolean {
    return true;
    // let plan: PianoColturale = this.confrontoPianoColturaleDataService.pianoColturale;
    // return plan != undefined && plan.Programmazione_Cod > 0;
  }
}
