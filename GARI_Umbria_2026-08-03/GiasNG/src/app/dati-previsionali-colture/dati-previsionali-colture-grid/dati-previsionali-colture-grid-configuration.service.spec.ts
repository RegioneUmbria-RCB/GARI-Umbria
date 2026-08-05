import { TestBed } from '@angular/core/testing';

import { DatiPrevisionaliColtureGridConfigurationService } from './dati-previsionali-colture-grid-configuration.service';

describe('DatiPrevisionaliColtureGridConfigurationService', () => {
  let service: DatiPrevisionaliColtureGridConfigurationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DatiPrevisionaliColtureGridConfigurationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
