import { TestBed } from '@angular/core/testing';

import { CaratteristicheService } from './caratteristiche.service';

describe('CaratteristicheService', () => {
  let service: CaratteristicheService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CaratteristicheService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
