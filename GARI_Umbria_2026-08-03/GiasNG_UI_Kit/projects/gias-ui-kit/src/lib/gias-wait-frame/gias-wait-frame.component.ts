import { Component, Input } from '@angular/core';

@Component({
    standalone: false,
    selector: 'gias-wait-frame',
    templateUrl: './gias-wait-frame.component.html',
    styleUrls: ['./gias-wait-frame.component.scss']
})
export class GiasWaitFrameComponent {
    @Input() message = '';
}