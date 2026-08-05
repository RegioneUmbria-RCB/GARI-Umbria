import { TestBed } from '@angular/core/testing';

import { RequisitiStabilimentoService } from './requisiti-stabilimento.service';

describe('RequisitiStabilimentoService', () => {
  let service: RequisitiStabilimentoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RequisitiStabilimentoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
