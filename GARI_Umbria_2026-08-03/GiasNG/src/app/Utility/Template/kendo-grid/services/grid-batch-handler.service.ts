import { Injectable } from "@angular/core";

@Injectable()
export class GridBatchHandlerService {
  private added: any[] = [];
  private updated: any[] = [];
  private deleted: any[] = [];

  public get hasChanges(): boolean {
    return this.added.length > 0 || this.updated.length > 0 || this.deleted.length > 0;
  }

  public getData(): any {
    return {
      added: this.added,
      updated: this.updated,
      deleted: this.deleted
    };
  }

  public add(row: any) {
    // Add is always called on a new row
    this.added.push(row);
  }

  public update(row: any, rowKey: string) {
    // Edit row that has been added but not saved
    const added = this.added.find(x => x[rowKey] == row[rowKey]);
    if (added != null) {
      this.added = this.added.filter(x => x[rowKey] != row[rowKey]);
      this.added.push(row);
      return;
    }

    // Edit row that has been updated but not saved
    const updated = this.updated.find(x => x[rowKey] == row[rowKey]);
    if (updated != null) {
      this.updated = this.updated.filter(x => x[rowKey] != row[rowKey]);
      this.updated.push(row);
      return;
    }

    this.updated.push(row);
  }

  public remove(rows: any[], rowKey: string) {
    for (const row of rows) {
      const added = this.added.find(x => x[rowKey] == row[rowKey]);
      if (added != null) {
        // Remove row that has just been added but not saved
        this.added = this.added.filter(x => x[rowKey] != row[rowKey]);
        continue;
      }

      const updated = this.updated.find(x => x[rowKey] == row[rowKey]);
      if (updated != null) {
        // Remove row that has just been updated but not saved
        this.updated = this.updated.filter(x => x[rowKey] != row[rowKey]);
      }

      if (!this.deleted.includes(row))
        this.deleted.push(row);
    }
  }

  public reset() {
    this.added = [];
    this.updated = [];
    this.deleted = [];
  }
}
