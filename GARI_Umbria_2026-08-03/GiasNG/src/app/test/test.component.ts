import { Component } from '@angular/core';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { GiasMessageService } from 'app/Service/gias-message.service';

@Component({
  standalone: false,
  selector: 'app-test',
  templateUrl: './test.component.html',
    styleUrls: ['./test.component.css']
})
export class TestComponent {

    constructor(private giasDialogService: GiasDialogService,
                private giasNotificationService: GiasMessageService) { }
    openDialog(typeMessage:number) {
        this.giasDialogService.dialogMessageObs_Result("titolo di prova",
            "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
            null, null, null, null, typeMessage);
    }

    openNotification(typeMessage: number, closable: boolean, short: boolean = false) {
        let message = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";
        if (short) message = message.substring(0, 200);
        this.giasNotificationService.message(typeMessage, message, closable);
    }

}
