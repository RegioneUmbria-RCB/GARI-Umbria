import { Component, OnDestroy, OnInit } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { CatastoCampoEditService } from './catasto-campo-edit-grid.service';
import { CatastoCampoService } from './catasto-campo.service';

@Component({
  standalone: false,
  selector: 'app-catasto-campo-edit',
  templateUrl: './catasto-campo-edit.component.html',
  styleUrls: ['./catasto-campo-edit.component.css'],
  providers: [
    ...generateGridProviders(CatastoCampoEditService, CatastoCampoEditComponent), CatastoCampoService
  ]
})

export class CatastoCampoEditComponent implements OnInit, OnDestroy {

  constructor() {  }

  ngOnInit(): void {  }

  ngOnDestroy(): void {  }

}

