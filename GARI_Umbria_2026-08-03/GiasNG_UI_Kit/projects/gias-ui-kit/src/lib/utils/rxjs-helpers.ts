
import { map, Observable, OperatorFunction } from "rxjs";
import { rispostaStandard } from "./models";

export function as<R>(): OperatorFunction<unknown, R> {
  return (source: Observable<unknown>) => source.pipe(map(x => x as unknown as R));
}

export function asStandard<R>(): OperatorFunction<unknown, rispostaStandard<R>> {
  return (source: Observable<unknown>) => source.pipe(map(x => x as unknown as rispostaStandard<R>));
}
