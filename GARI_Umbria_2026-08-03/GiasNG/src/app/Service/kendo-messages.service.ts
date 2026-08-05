import { EventEmitter, Injectable, Output } from '@angular/core';
import { MessageService } from '@progress/kendo-angular-l10n';
import { frComponentMessages } from 'app/messages/fr-FR';
import { itComponentMessages } from 'app/messages/it-IT';
import { ptComponentMessages } from 'app/messages/pt-PT';
import { enComponentMessages } from 'app/messages/en-GB';

const componentMsgs: any = {
    ['en']: enComponentMessages,
    ['it']: itComponentMessages,
    ['pt']: ptComponentMessages,
    ['fr']: frComponentMessages
};

@Injectable()
export class CustomMessagesService extends MessageService {
    @Output() public localeChange = new EventEmitter();
    private localeId: string = 'it';

    public set language(value: string) {
        const locale = componentMsgs[value];
        if (locale) {
            this.localeId = value;
            this.localeChange.emit();
            this.notify();
        }
    }

    public get language(): string {
        return this.localeId;
    }

    private get messages(): any {
        return componentMsgs[this.localeId];
    }

    public override get(key: string): string {
        return this.messages[key];
    }
}
