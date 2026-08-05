import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DssNutrizionePageComponent } from './pages/dss-nutrizione-page/dss-nutrizione-page.component';

const routes: Routes = [
  {
    path: '',
    component: DssNutrizionePageComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DssNutrizioneRoutingModule { }
