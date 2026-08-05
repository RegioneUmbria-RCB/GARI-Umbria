import { Component, OnInit } from '@angular/core';
import {generateGridProviders} from 'gias-kendo-grid';
import {InvestimentoCatastaleAppezzamentoGridService} from './investimento-catastale-appezzamento-grid.service';
import {ImpiantiServiceProvider} from '../../../Service/ServiceFactory/impianti.factory.provider';
import {InvestimentoCatastaleAppezzamentoDataService} from './investimento-catastale-appezzamento-data.service';

@Component({
  standalone: false,
  selector: 'app-investimento-catastale-appezzamento',
  templateUrl: './investimento-catastale-appezzamento.component.html',
  styleUrls: ['./investimento-catastale-appezzamento.component.css'],
  providers: [
    ...generateGridProviders(InvestimentoCatastaleAppezzamentoGridService, InvestimentoCatastaleAppezzamentoComponent), ImpiantiServiceProvider
  ]
})
export class InvestimentoCatastaleAppezzamentoComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
