import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GISCfgProiezioniComponent } from './GIS-cfg-proiezioni/GIS-cfg-proiezioni.component';
import { GISComponent } from './GIS.component';
import { GisLoadedGuard } from './guard/GisLoadedGuard';

const routes: Routes = [
    {
        path: '',
        component: GISComponent,
        canActivate: [GisLoadedGuard]
    },
    {
        path: 'cfg-proiezioni',
        component: GISCfgProiezioniComponent,
        // canActivate: [GisLoadedGuard]
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class GISRoutingModule { }

