import { NgModule } from '@angular/core';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { PesateAccrescimentoComponent } from './pesate-accrescimento.component';
import { PesateAccrescimentoRoutingModule } from './pesate-accrescimento-routing.module';

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`../../i18n/${lang}.json`);
  return acc;
}, {});

@NgModule({
  imports: [
    PesateAccrescimentoComponent,
    PesateAccrescimentoRoutingModule,
  ],
  providers: [
    {
      provide: TRANSLOCO_SCOPE,
      useValue: {
        scope: 'zoo',
        loader,
        multi: true,
      },
    },
  ],
})
export class PesateAccrescimentoModule {}
