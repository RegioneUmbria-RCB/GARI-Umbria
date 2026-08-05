import { debounceTime, startWith, Subscription } from "rxjs";
import { ChangeDetectorRef, Component, Inject, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, ViewChild } from "@angular/core";
import { DialogGridProdsSommComponent } from "./grid/dialog-grid-prods-somm.component";
import { GRID_HTTP_TOKEN, generateGridProviders } from "gias-kendo-grid";
import { DialogGridProdsSommService, DialogValidation, ProdottoDistribuito } from "./grid/dialog-grid-prods-somm.service";

@Component({
  standalone: false,
  selector: 'app-dialog-qta-diff-aics',
  templateUrl: './dialog-qta-diff-aics.component.html',
  styleUrls: ['./dialog-qta-diff-aics.component.css'],
  providers: [
    ...generateGridProviders(
      DialogGridProdsSommService,
      DialogGridProdsSommComponent
    )
  ]
})
export class DialogQtaDiffAICsComponent implements OnInit, OnDestroy, OnChanges {

  @Input() qtaTotReq: number = 0;
  @Input() products: ProdottoDistribuito[] = [];

  public qtaTotDistribuito: number = 0;

  private dataInitialized: boolean = false;
  private gridChangeSub: Subscription;

  @ViewChild(DialogGridProdsSommComponent) gridComponent!: DialogGridProdsSommComponent;

  constructor(
    @Inject(GRID_HTTP_TOKEN) public gridService: DialogGridProdsSommService,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit(){
    // this.gridService.setInitialData(this.products, this.qtaTotReq);

    this.gridChangeSub = this.gridService.data$
      .pipe(
        startWith(this.gridService.gridRows),
        debounceTime(50)
      )
      .subscribe((rows: ProdottoDistribuito[]) => {
        this.qtaTotDistribuito = rows.reduce((acc, item) => acc + (Number(item.QtaToUse) || 0), 0);
        this.cdRef.detectChanges();
      });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.products.length > 0 && this.qtaTotReq > 0 && !this.dataInitialized) {
      this.dataInitialized = true;
      this.gridService.setInitialData(this.products, this.qtaTotReq);
    }
  }

  ngOnDestroy(): void {
    if (this.gridChangeSub) {
      this.gridChangeSub.unsubscribe();
    }
  }

  // public onDataChange(rows: KendoGridRow[]): void {
  //   this.gridService.gridRows = rows as ProdottoDistribuito[];
  // }

  // public calculateTotal(): number {
  //   return Math.round(
  //     this.gridService.gridRows.reduce((acc, item) => acc + (item.QtaToUse || 0), 0)
  //   );
  // }

  public validate(): DialogValidation {
    return this.gridService.validate(this.qtaTotDistribuito, this.qtaTotReq);
  }

  public getData(): ProdottoDistribuito[] {
    return this.gridService.getData();
  }

  // --- Gestione Editing Griglia ---
  // public cellClickHandler(args: CellClickEvent): void {
  //   if (args.column.field === 'QtaDaUsare') {
  //     args.sender.editCell(args.rowIndex, args.columnIndex, this.createFormGroup(args.dataItem));
  //   }
  // }

  // public createFormGroup(dataItem: any): FormGroup {
  //   return new FormGroup({
  //     'QtaDaUsare': new FormControl(dataItem.QtaDaUsare)
  //   });
  // }

  // public calculateDialogSum(): void {
  //   const total = this.gridData.reduce((acc, item) => acc + (item.QtaDaUsare || 0), 0);
  //   this.totalDialogSum = { qtaDaUsare: total };
  // }

  // // --- Metodi Pubblici per il Parente ---

  // /**
  //  * Metodo pubblico chiamato dal genitore prima di chiudere il dialogo.
  //  */
  // public validate(): DialogValidation {
  //   // 1. Controlla il totale
  //   const totalEditedQta = Math.round(this.totalDialogSum.qtaDaUsare);
  //   const totalRequired = Math.round(this.qtaTotReq);

  //   if (totalEditedQta !== totalRequired) {
  //     return {
  //       isValid: false,
  //       messageKey: 'zoo.LaQuantitaDistribuitaNonCorrisponde',
  //       messageParams: { required: totalRequired, actual: totalEditedQta }
  //     };
  //   }

  //   // 2. Controlla che QtaDaUsare non superi la Giacenza (Qta)
  //   const overStock = this.gridData.find(item => (item.QtaDaUsare || 0) > item.Qta);
  //   if (overStock) {
  //     return {
  //       isValid: false,
  //       messageKey: 'zoo.LaQuantitaDaSomministrareEccedeLaGiacenza',
  //       messageParams: { lotto: overStock.Lotto }
  //     };
  //   }

  //   // 3. Controlla che sia stata usata almeno una riga
  //   if (totalEditedQta <= 0) {
  //       return {
  //           isValid: false,
  //           messageKey: 'zoo.LaQuantitaDaSomministrareNonPuòEssereZero',
  //       };
  //   }

  //   return { isValid: true, messageKey: null };
  // }

  // /**
  //  * Metodo pubblico che restituisce i dati finali al genitore.
  //  * Restituisce solo i prodotti che hanno una QtaDaUsare > 0.
  //  */
  // public getData(): ProdottoDistribuito[] {
  //   return this.gridData.filter(item => item.QtaDaUsare > 0);
  // }
}