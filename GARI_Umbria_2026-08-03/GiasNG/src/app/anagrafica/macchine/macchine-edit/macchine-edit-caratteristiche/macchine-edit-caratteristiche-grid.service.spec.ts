import { TestBed } from '@angular/core/testing';

import { MacchineEditCaratteristicheGridService } from './macchine-edit-caratteristiche-grid.service';

describe('MacchineEditCaratteristicheGridService', () => {
  let service: MacchineEditCaratteristicheGridService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MacchineEditCaratteristicheGridService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
