import { Component, ElementRef, Inject, Input, LOCALE_ID, OnInit, ViewChild } from '@angular/core';
import { IndicatoreRatingUnico, WidgetKPI, WidgetsClient } from 'app/Service/api.service';
import { catchError, finalize, map, of, switchMap, tap } from 'rxjs';
import { PDFExportComponent } from "@progress/kendo-angular-pdf-export";
import { DatePipe } from "@angular/common";
import { TranslocoService } from '@jsverse/transloco';
import { WidgetIndiciClient } from 'app/Service/net-core6-api.service';

@Component({
  standalone: false,
  selector: 'app-unique-rating-widget',
  templateUrl: './unique-rating-widget.component.html',
  styleUrls: ['./unique-rating-widget.component.css']
})
export class UniqueRatingWidgetComponent implements OnInit {
  @Input() piva: string | null = null;
  @Input() rag_soc: string | null = null;
  @Input() params: string | null = null;

  @ViewChild("pdf") Pdf: PDFExportComponent;

  @ViewChild("pdf", { read: ElementRef }) PdfElRef: ElementRef;

  @ViewChild("InfoPrint") private SpanElement: ElementRef;

  loading = true;
  data: WidgetKPI[] = [];
  IndicatoreRatingUnico = IndicatoreRatingUnico;

  constructor(private widgetsClient: WidgetIndiciClient,
    public datepipe: DatePipe,
    private translocoService: TranslocoService,
    @Inject(LOCALE_ID) public locale_id: string) { }

  ngOnInit(): void {

    if (this.piva == null) {
      this.loading = false;
      return;
    }

    this.loading = true;

    this.widgetsClient
      .widgetIndiciGetAvailableYearsIndiciProduttivita(this.piva)
      .pipe(
        map(res => JSON.parse(res.RispostaStringa) as number[]),
        catchError(() => of([])),
        map(res => res.length > 0 ? Math.max(...res.map(s => +s)) : (new Date().getFullYear() - 1)),
        switchMap(year => this.widgetsClient.widgetIndiciGetWidgetKpi({ piva: this.piva, year: year })),
        map(res => JSON.parse(res.RispostaStringa) as WidgetKPI[]),
        catchError(() => of([])),
        tap(res => this.data = res),
        finalize(() => this.loading = false)
      ).subscribe();
  }

  isOutOfRange(elem: WidgetKPI): boolean {
    return elem.val < elem.min || elem.val > elem.max;
  }

  public exportKPI() {

    this.loading = true;

    const PdfNativeEl = this.PdfElRef.nativeElement;

    const SpanNativeEl = this.SpanElement.nativeElement;

    const allWidgetColumn = PdfNativeEl.getElementsByClassName("widget-column");

    const allValueColumn = PdfNativeEl.getElementsByClassName("w-value");

    const allWDescriptionColumn = PdfNativeEl.getElementsByClassName("w-description");

    const allError = PdfNativeEl.getElementsByClassName("w-error");

    const allKPI = PdfNativeEl.getElementsByClassName("w-kpi");

    const obj = this.FormatForExport(PdfNativeEl, SpanNativeEl, allWidgetColumn, allValueColumn, allWDescriptionColumn, allError, allKPI);

    this.Pdf.export().then(group => {

      this.Pdf.saveAs(this.piva + "_KPI.pdf");

      this.OriginalFormat(obj, PdfNativeEl, SpanNativeEl, allWidgetColumn, allValueColumn, allWDescriptionColumn, allError, allKPI);
    });

    this.loading = false;
  }

  private FormatForExport(PdfNativeEl, SpanNativeEl, allWidgetColumn, allValueColumn, allWDescriptionColumn, allError, allKPI): Object {

    const obj = {
      PdfNativeElFontSyle: PdfNativeEl.style.getPropertyValue("font-size"),
      originalWidthWidgetColumn: "",
      originalWidthValueColumn: "",
      originalWidthWDescriptionColumn: "",
      originalWidthError: "",
      originalWidthKPI: "",
    }

    PdfNativeEl.style.setProperty("font-size", "10px", "important");

    for (let i = 0; i < allWidgetColumn.length; i++) {
      obj.originalWidthWidgetColumn = allWidgetColumn[i].style.getPropertyValue("width");
      allWidgetColumn[i].style.setProperty("width", "50%", "important");
    }


    for (let i = 0; i < allValueColumn.length; i++) {
      obj.originalWidthValueColumn = allValueColumn[i].style.getPropertyValue("width");
      allValueColumn[i].style.setProperty("width", "30%", "important");
    }

    for (let i = 0; i < allWDescriptionColumn.length; i++) {
      obj.originalWidthWDescriptionColumn = allWDescriptionColumn[i].style.getPropertyValue("width");
      allWDescriptionColumn[i].style.setProperty("width", "50%", "important");
    }

    for (let i = 0; i < allError.length; i++) {
      obj.originalWidthError = allError[i].style.getPropertyValue("width");
      allError[i].style.setProperty("width", "50%", "important");
    }

    for (let i = 0; i < allKPI.length; i++) {
      obj.originalWidthKPI = allKPI[i].style.getPropertyValue("width");
      allKPI[i].style.setProperty("width", "20%", "important");
    }

    const dateStr: string = this.datepipe.transform(new Date(), 'dd/MM/yyyy', undefined, this.locale_id);

    SpanNativeEl.innerHTML = this.translocoService.translate("DataDiStampaDuePunti") + " " + dateStr + " - " + this.rag_soc;

    return obj;
  }

  private OriginalFormat(obj, PdfNativeEl, SpanNativeEl, allWidgetColumn, allValueColumn, allWDescriptionColumn, allError, allKPI) {
    PdfNativeEl.style.setProperty("font-size", obj.PdfNativeElFontSyle, "");

    for (let i = 0; i < allWidgetColumn.length; i++) {
      allWidgetColumn[i].style.setProperty("width", obj.originalWidthWidgetColumn, "");
    }

    for (let i = 0; i < allValueColumn.length; i++) {
      allValueColumn[i].style.setProperty("width", obj.originalWidthValueColumn, "");
    }

    for (let i = 0; i < allWDescriptionColumn.length; i++) {
      allWDescriptionColumn[i].style.setProperty("width", obj.originalWidthWDescriptionColumn, "");
    }

    for (let i = 0; i < allError.length; i++) {
      allError[i].style.setProperty("width", obj.originalWidthError, "");
    }

    for (let i = 0; i < allKPI.length; i++) {
      allKPI[i].style.setProperty("width", obj.originalWidthKPI, "");
    }

    SpanNativeEl.innerHTML = "";
  }


}
