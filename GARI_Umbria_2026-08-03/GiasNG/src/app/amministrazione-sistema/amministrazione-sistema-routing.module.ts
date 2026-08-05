import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ConsultaSincroDatiAppComponent } from './dati-app/consulta-sincro-dati-app/consulta-sincro-dati-app.component';

const routes: Routes = [
    { path: 'Consulta-Sincro-Dati-App', component: ConsultaSincroDatiAppComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AmministrazionSistemaRoutingModule { }
