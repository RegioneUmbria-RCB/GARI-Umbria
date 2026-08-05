import { NgModule } from '@angular/core';
import { ServerModule } from '@angular/platform-server';
import { AppComponent } from './app.component';
import { AppModule } from './app.module';
import { NotificationModule } from '@progress/kendo-angular-notification';
import { IconsModule } from '@progress/kendo-angular-icons';
import { MenusModule } from '@progress/kendo-angular-menu';

@NgModule({
    imports: [AppModule, ServerModule, NotificationModule, IconsModule, MenusModule],
    bootstrap: [AppComponent]
})
export class AppServerModule { }
