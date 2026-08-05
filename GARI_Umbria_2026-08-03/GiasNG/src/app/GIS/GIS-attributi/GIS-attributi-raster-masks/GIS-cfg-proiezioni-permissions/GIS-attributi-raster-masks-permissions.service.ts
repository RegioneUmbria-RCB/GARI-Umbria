import { Injectable } from '@angular/core';

@Injectable()
export class GisAttributiRasterMasksPermissionsService {
  private remainingAuths = 0;

  public resetAuths(): void {
    this.remainingAuths = 0;
  }

  public addRemainingAuths(remainingAuths: number): void {
    this.remainingAuths += remainingAuths;
  }

  public get currentRemainingAuths(): number | null {
    return this.remainingAuths;
  }
}
