import { Injectable } from "@angular/core";
import { enum_GISDrawingOperations } from "app/GIS/GIS-enum/GIS-drawing-operations";
import { BehaviorSubject, Observable } from "rxjs";

@Injectable()
export class DrawWindowOperationService {
  private operation = new BehaviorSubject<enum_GISDrawingOperations>(enum_GISDrawingOperations.none);

  public setOperation(operation: enum_GISDrawingOperations): void {
    this.operation.next(operation);
  }

  public get operation$(): Observable<enum_GISDrawingOperations> {
    return this.operation.asObservable();
  }
}
