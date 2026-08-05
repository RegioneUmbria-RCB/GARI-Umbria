import { TestBed } from '@angular/core/testing';

import { MacchineEditImageStorageService } from './macchine-edit-image-storage.service';

describe('MacchineEditImageStorageService', () => {
  let service: MacchineEditImageStorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MacchineEditImageStorageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
