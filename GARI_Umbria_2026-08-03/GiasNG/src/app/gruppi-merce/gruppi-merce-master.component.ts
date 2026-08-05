import { Component, ElementRef, ViewChild } from '@angular/core';
import { faAddressCard, faPeopleArrows, faUniversalAccess, faUser, faUsers } from '@fortawesome/free-solid-svg-icons';




@Component({
  standalone: false,
  selector: 'app-gruppi-merce',
  templateUrl: './gruppi-merce-master.component.html',
  styleUrls: ['./gruppi-merce-master.component.scss'],
})
export class GruppiMerceMasterComponent {
    @ViewChild('anchor', { static: false })
    public anchor: ElementRef<HTMLElement>;

    public show = false;
    public permessoEdit: boolean = true;

    faUser = faUser;
    faUsers= faUsers;
    faPermessi = faUniversalAccess;
    faProfili = faAddressCard;
    faPeopleArrow = faPeopleArrows;

}
