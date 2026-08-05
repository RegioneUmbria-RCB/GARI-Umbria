import { enum_logDebugArea, enum_logDebugTipo } from 'app/Model/log-debug';
import {
  KendoGridColumn, KendoGridModel,
  KendoServerResult,
  ServerResult
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { environment } from 'environments/environment';
import { Utente } from './api.service';

export const isNullOrUndefined = (obj) => obj == null;
export const isUndefined = (obj) => obj === undefined;

export const separatoreChiaveAlbero = '§';
export const separatoreFiltroAlbero = '<$&§>';

export const qualsiasiLogDebug = '*';

export function parseServerResult(table: ServerResult): KendoServerResult
{
  const columns = new Array<KendoGridColumn>();
  const model: KendoGridModel = table.kendo_model;

  table.kendo_columns.forEach(elem => {
    const col: KendoGridColumn = { ...elem };

    col.editable = false;

    if(model[col.field].type === CELL_TYPES.STRING){
      col.showHTMLAsString = true;
    }

    columns.push(col);
  });

  return {
    columns: columns,
    model: model,
    rows: table.kendo_rows
  }
}

export function NumToStr(num: number): string {
  return num + "";
}

export function StringToDate(data: any): Date {
  var giornoMeseAnno = data.split("/");

  if(giornoMeseAnno[2].includes(' ')) {
    const annoTempo = giornoMeseAnno[2].split(" ");
    const hourMinsSecs = annoTempo[1].split(":");
    return new Date(creaDataConTempo(annoTempo, giornoMeseAnno, hourMinsSecs));
  }

  return new Date(+giornoMeseAnno[2], giornoMeseAnno[1] - 1, +giornoMeseAnno[0]);
}

export function DateToString(data: Date): string {
  let str: string = data.toLocaleString();
  let result = str.split(',')[0];
  return result;
}

function creaDataConTempo(annoTempo, giornoMeseAnno, hourMinsSecs): Date {
  return new Date(+annoTempo[0], giornoMeseAnno[1] - 1, +giornoMeseAnno[0],
    hourMinsSecs[0], hourMinsSecs[1], hourMinsSecs[2]);
}

export function getServiceIdAndLog(
  nomeServizio: string,
  messaggio: string): string {
  const serviceId = Date.now().toString();
  consoleLogDebugParam(enum_logDebugArea.App, enum_logDebugTipo.TraceServiceId, nomeServizio, messaggio, serviceId);
  return serviceId;
}

export function getComponentIdAndLog(
  nomeComponente: string,
  messaggio: string): string {
  const componentId = Date.now().toString();
  consoleLogDebugParam(enum_logDebugArea.App, enum_logDebugTipo.TraceComponentId, nomeComponente, messaggio, componentId);
  return componentId;
}

export function consoleLogDebug(
  debugArea: enum_logDebugArea,
  debugTipo: enum_logDebugTipo,
  nomeComponenteServizio: string,
  messaggio: string): void {
  const tagLogDebug = componiTag(debugArea, debugTipo)
  if (isTipoAttivo(tagLogDebug)) {
    console.log(`###${tagLogDebug}###[${nomeComponenteServizio}] ${messaggio}`);
  }
}

export function consoleLogDebugParam(
  debugArea: enum_logDebugArea,
  debugTipo: enum_logDebugTipo,
  nomeComponenteServizio: string,
  messaggio: string,
  param: any): void {
  const tagLogDebug = componiTag(debugArea, debugTipo)
  if (isTipoAttivo(tagLogDebug)) {
    console.log(`###${tagLogDebug}###[${nomeComponenteServizio}] ${messaggio}`,param);
  }
}

export function consoleLogDebugMultiParam(
  debugArea: enum_logDebugArea,
  debugTipo: enum_logDebugTipo,
  nomeComponenteServizio: string,
  messaggio: string,
  param: any[]): void {
  const tagLogDebug = componiTag(debugArea, debugTipo)
  if (isTipoAttivo(tagLogDebug)) {
    console.log(`###${tagLogDebug}###[${nomeComponenteServizio}] ${messaggio}`,param);
  }
}

function componiTag(debugArea: enum_logDebugArea, debugTipo: enum_logDebugTipo): string {
  return (debugArea + debugTipo).toString();
}

function isTipoAttivo(tagLogDebug: string): boolean {
  return environment.logDebugTipiAbilitati.includes(qualsiasiLogDebug) ||
    environment.logDebugTipiAbilitati.includes(tagLogDebug)
}

export function isSuperUser(user: Utente): boolean {
  const username = user?.UserNameCommerciale || "";
  return username?.toLowerCase() === 'superuser';
}

export function isInRange(start: number, end: number, value: number): boolean {
  return value > start && value < end;
}

export function isInRangeClose(start: number, end: number, value: number): boolean {
  return value >= start && value <= end;
}
