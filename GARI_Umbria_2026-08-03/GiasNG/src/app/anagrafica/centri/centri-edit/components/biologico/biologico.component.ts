import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormGroupDirective } from '@angular/forms';
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { EditCentroStore } from '../../services/centri-store.service';


@Component({
  standalone: false,
  selector: 'centri-biologico',
  templateUrl: './biologico.component.html',
  styleUrls: ['./biologico.component.scss'],
  providers: [GiasMultiSelectTemplateService]
})
export class BiologicoComponent implements OnInit {
    @Input() isFormDisabled: boolean;

    form: FormGroup;


    constructor(private rootFormGroup: FormGroupDirective, public store: EditCentroStore)
    {

    }

    ngOnInit(): void {
        this.form = this.rootFormGroup.form as FormGroup;
    }

}
