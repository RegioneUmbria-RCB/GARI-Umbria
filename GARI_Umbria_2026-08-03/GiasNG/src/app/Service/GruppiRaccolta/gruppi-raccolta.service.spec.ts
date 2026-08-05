import { TestBed } from '@angular/core/testing';

import { GruppiRaccoltaService } from './gruppi-raccolta.service';

describe('GruppiRaccoltaService', () => {
  let service: GruppiRaccoltaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GruppiRaccoltaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
