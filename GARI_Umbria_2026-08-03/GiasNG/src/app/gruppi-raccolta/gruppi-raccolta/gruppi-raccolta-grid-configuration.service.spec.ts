import { TestBed } from '@angular/core/testing';

import { GruppiRaccoltaGridConfigurationService } from './gruppi-raccolta-grid-configuration.service';

describe('GruppiRaccoltaGridConfigurationService', () => {
  let service: GruppiRaccoltaGridConfigurationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GruppiRaccoltaGridConfigurationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
