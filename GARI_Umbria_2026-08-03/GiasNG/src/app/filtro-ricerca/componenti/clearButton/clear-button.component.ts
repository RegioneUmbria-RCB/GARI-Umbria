import { Component, EventEmitter, Input, Output } from "@angular/core";
import { faEraser } from "@fortawesome/free-solid-svg-icons";
import { Enum_Type_Button_Clear } from "app/filtro-ricerca/utils";

@Component({
    standalone: false,
    selector: 'app-clear-button',
    templateUrl: './clear-button.component.html',
    styleUrls: ['./clear-button.component.scss']
})
export class ClearButtonComponent {

    @Output() onClear = new EventEmitter<boolean>();
    @Input() typeButton: Enum_Type_Button_Clear = Enum_Type_Button_Clear.fromFilters;
    faClear = faEraser;

    public Enum_Type_Button_Clear = Enum_Type_Button_Clear;

    clearFilters() {
        this.onClear.emit(null);
    }

}
