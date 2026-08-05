import { NgStyle } from "@angular/common";
import { Component, Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root',
})
export class CustomMessageService {
    private contextString: string;
    private action: string;
    private color: string;
    private customFunction;

    getContextString(){
        return this.contextString;
    }
    getAction() {
        return this.action;
    }
    getColor(){
        return this.color;
    }
    getCustomFunction() {
        return this.customFunction;
    }

    createCustomMessage(contextString: string, action: string, color: string, customFunction) {
        this.contextString = contextString;
        this.action = action;
        this.customFunction = customFunction;
        this.color = color;
    }

    getCustomMessageComponent(): typeof CustomMessageComponent {
        return CustomMessageComponent;
    }
}

@Component({
    standalone: true,
    selector: "custom-message-component",
    template: `
    <div [ngStyle]="{ 'color': color }">
        {{ message }} <span (click)="onClick()">{{ action }}</span>
    </div>
    `,
    imports: [NgStyle]
})

export class CustomMessageComponent {

    constructor(private customMessageService: CustomMessageService) {}

    message = this.customMessageService.getContextString();
    action = this.customMessageService.getAction();
    color = this.customMessageService.getColor();
    customFunction = this.customMessageService.getCustomFunction();

    onClick() {
        this.customFunction();
    }
}
