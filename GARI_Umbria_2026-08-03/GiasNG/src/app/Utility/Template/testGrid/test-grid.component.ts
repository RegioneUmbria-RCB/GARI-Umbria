import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ProductsService } from './products.service';

const createFormGroup = (dataItem) =>
    new FormGroup({
        ProductID: new FormControl(dataItem.ProductID),
        ProductName: new FormControl(dataItem.ProductName, Validators.required),
    });

@Component({
    standalone: false,
    selector: 'test-grid',
    template: 'test-grid.component.html',
})
export class TestGridComponent implements OnInit {
    public gridData: any[];
    public formGroup: FormGroup;
    private editedRowIndex: number;

    constructor(private service: ProductsService) {}

    public ngOnInit(): void {
        this.gridData = this.service.products();
    }

    public editHandler({ sender, rowIndex, dataItem }) {
        this.closeEditor(sender);

        this.formGroup = createFormGroup(dataItem);

        this.editedRowIndex = rowIndex;

        sender.editRow(rowIndex, this.formGroup);
    }

    public saveHandler({ dataItem, sender, rowIndex, formGroup, isNew }): void {
        const product = formGroup.value;

        // this.service.save(product, isNew);

        sender.closeRow(rowIndex);
    }

    private closeEditor(grid, rowIndex = this.editedRowIndex) {
        grid.closeRow(rowIndex);
        this.editedRowIndex = undefined;
        this.formGroup = undefined;
    }
}
