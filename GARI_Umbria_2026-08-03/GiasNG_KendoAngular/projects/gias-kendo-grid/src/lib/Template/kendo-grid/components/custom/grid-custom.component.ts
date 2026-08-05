import { Component, ComponentFactoryResolver, ComponentRef, Input, OnDestroy, OnChanges, ViewChild, ViewContainerRef, AfterViewInit, ChangeDetectorRef, SimpleChanges, Type } from '@angular/core';
import { FormGroup } from "@angular/forms";
import { CustomComponent } from '../../models/custom-component';

@Component({
    standalone: false,
    selector: 'gias-grid-custom',
    templateUrl: './grid-custom.component.html',
    styleUrls: ['./grid-custom.component.css'],
})
export class GridCustomComponent implements OnDestroy, AfterViewInit, OnChanges {
    @ViewChild('component', { read: ViewContainerRef }) entry: ViewContainerRef;
    @Input() component: Type<CustomComponent>;
    @Input() input: any;
    @Input() field: any;
    @Input() edit: any;
    @Input() formGroup: FormGroup;

    componentRef: ComponentRef<any>;

    constructor(private resolver: ComponentFactoryResolver, private changeDetector: ChangeDetectorRef) {
    }

    ngOnChanges(changes: SimpleChanges): void {
        this.handleComponent();
    }

    ngAfterViewInit(): void {
        this.handleComponent();
    }

    private handleComponent() {
        if (this.component && this.entry) {
            if (this.componentRef) {
                this.componentRef.destroy();
            }
            this.componentRef = this.entry.createComponent(this.component);
            this.componentRef.instance.input = this.input;
            this.componentRef.instance.edit = this.edit;
            this.componentRef.instance.field = this.field;
            this.componentRef.instance.formGroup = this.formGroup;
            this.changeDetector.detectChanges();
        }
    }

    ngOnDestroy(): void {
        if (this.componentRef) {
            this.componentRef.destroy();
        }
    }
}
