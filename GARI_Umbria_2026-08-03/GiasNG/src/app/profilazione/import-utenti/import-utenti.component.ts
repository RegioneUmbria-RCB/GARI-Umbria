import { Component, OnInit, ViewChild } from '@angular/core';
import { FileRestrictions, SelectEvent } from "@progress/kendo-angular-upload";
import { FileInfo } from "@progress/kendo-angular-upload/types";
import { AjaxAgronicaAPIService } from "../../Service/ajax-agronica.api.service";
import { take } from "rxjs";

@Component({
  standalone: false,
  selector: 'app-import-utenti',
  templateUrl: './import-utenti.component.html',
  styleUrls: ['./import-utenti.component.css']
})
export class ImportUtentiComponent implements OnInit {
  protected usersOk: string[] = [];
  protected usersError: string[] = [];
  protected readonly fileRestrictions: FileRestrictions = {
    allowedExtensions: ['.xlsx', '.xls']
  };
  private _file: FileInfo = null;

  constructor(
    private APIService: AjaxAgronicaAPIService
  ) { }

  protected get hasSelectedFile(): boolean {
    return !!this._file;
  }

  ngOnInit(): void { }

  onFileSelected(event: SelectEvent) {
    if (this.fileRestrictions.allowedExtensions.includes(event.files[0].extension)) {
      this._file = event.files[0];
    } else {
      this._file = null;
    }
  }

  onFileRemoved() {
    this._file = null;
  }

  importUsers() {
    const formData = new FormData();
    formData.append('file', this._file.rawFile, this._file.name);
    this.APIService.ajaxAPIPostFormData<any>('Profilazione/ImportaUtentiDaExcel', formData, true)
      .pipe(take(1)).subscribe((R) => {
        if (R.RispostaOK) {
          this.usersOk = R.RispostaStringa;
          this.usersError = JSON.parse(R.Errore);
        }
      });
  }

}
