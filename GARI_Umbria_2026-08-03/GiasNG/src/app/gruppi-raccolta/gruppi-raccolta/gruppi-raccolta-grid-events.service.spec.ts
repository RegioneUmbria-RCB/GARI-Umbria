import { TestBed } from '@angular/core/testing';

import { GruppiRaccoltaGridEventsService } from './gruppi-raccolta-grid-events.service';

describe('GruppiRaccoltaGridEventsService', () => {
  let service: GruppiRaccoltaGridEventsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GruppiRaccoltaGridEventsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
