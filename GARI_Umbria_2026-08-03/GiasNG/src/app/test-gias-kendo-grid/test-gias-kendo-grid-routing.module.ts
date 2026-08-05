import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestGiasGiasKendoGridComponent } from './test-gias-kendo-grid.component';

const routes: Routes = [
  {
    path: '',
    component: TestGiasGiasKendoGridComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TestGiasKendoGridRoutingModule { }
