import { Injectable } from '@angular/core';
import {DateFormatOptions, IntlService } from '@progress/kendo-angular-intl';
import {AGRODATAFINE, AGRODATAINIZIO} from 'app/Model/CostantiPersonalizzate';
import moment from 'moment';
import momentTimezone from 'moment-timezone';
import {lessOrEqualIcon} from "@progress/kendo-svg-icons";
import {MasterService} from "./master.service";
import { isArray } from 'lodash';
import { filter, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConversionService {

  private dtRegex1 = new RegExp(/\b\d{4}[\/-]\d{1,2}[\/-]\b\d{1,2} (0\d|1[01]):[0-5]\d:[0-5]\d$\b/);
  private dtRegex2 = new RegExp(/\b(0?[1-9]|([1-2]?[0-9]|3[0-1]))[\/-]([0]?[1-9]|1[0-2])[\/-]\b\d{4} ([0-1]?[0-9]|2[0-3]):[0-5]\d$\b/);
  private dtRegex3 = new RegExp(/\b(0?[1-9]|([1-2]?[0-9]|3[0-1]))[\/-]([0]?[1-9]|1[0-2])[\/-]\b\d{4} (0\d|1[01]):[0-5]\d:[0-5]\d$\b/);
  private dtRegex4 = new RegExp(/\b(0?[1-9]|([1-2]?[0-9]|3[0-1]))[\/-]([0]?[1-9]|1[0-2])[\/-]\b\d{4} ([0-1]?[0-9]|2[0-3]):[0-5]\d:[0-5]\d$\b/)
  //private MappedData: Date[] = new Array<Date>();
  private MappedData: Map<string, Date> = new Map<string, Date>();
  private serverTimeZone: string;
  private clientTimeZone: string;

  constructor(private intlService: IntlService,
    private masterService: MasterService) {
  }

  ConversionDateInObject<T>(object: T): T {
    this.serverTimeZone = "Europe/Rome";
    if (this.masterService.serverTimeZoneOffset != undefined && this.masterService.serverTimeZoneOffset != "") {
      this.serverTimeZone = this.masterService.serverTimeZoneOffset;
    }

    this.clientTimeZone = moment.tz.guess();

    if (object == null || object == undefined) {
      return null;
    }
    let fieldArray;
    try {
      fieldArray = Object.getOwnPropertyNames(object);
    } catch (e) {
      return object;
    }
    fieldArray.forEach((val, i) => {
      if (object[val] != null && object[val] !== undefined){

        //Tipo oggetto, funzione ricorsiva, la richiamo
        if (typeof object[val] == 'object') {
          object[val] = this.ConversionDateInObject(object[val]);
        }

        if (typeof object[val] == 'string' && this.MappedData[object[val]] != undefined){
          object[val] = this.MappedData[object[val]];
          return;
        }

        if (typeof object[val] == 'string' && (<string>object[val]).includes('Date(') ) {
          //Tipo di date che venivano fuori dai CoreWS, dovrebbe essere obsoleto
          var a2 = momentTimezone(object[val]).tz(this.serverTimeZone);

          let data = this.intlService.parseDate((<string>object[val]));
          if (data != null && a2 != null && data.getTimezoneOffset() != a2.utcOffset() && data.getMilliseconds() == 0 && (this.serverTimeZone !=  this.clientTimeZone)){
            let timeZoneMilliseconds = data.getTimezoneOffset() * 60 * 1000
            let serverMillisecondsOffset = a2.utcOffset() * 60 * 1000;
            data.setTime(data.getTime() + (timeZoneMilliseconds + serverMillisecondsOffset))
            data = this.handleAGRODATAINIZIOFromServer(data);
          }
          this.MappedData[object[val]] = data;
          object[val] = data;
        }
        else if (typeof object[val] == 'string' && moment(<string>object[val], moment.ISO_8601, true).isValid() ) {
          //Data formato ISO
          let dateVal = this.intlService.parseDate((<string>object[val]));
          if ((<string>object[val]).length > 20 && ((<string>object[val])[19] == '+' || (<string>object[val])[19] == '-')){
            var a2 = momentTimezone(object[val]).tz(this.serverTimeZone);
            let data = this.intlService.parseDate((<string>object[val]));
            if (data.getTimezoneOffset() != a2.utcOffset() && (this.serverTimeZone != this.clientTimeZone)) {
              let timeZoneMilliseconds = data.getTimezoneOffset() * 60 * 1000
              let serverMillisecondsOffset = a2.utcOffset() * 60 * 1000;
              data.setTime(data.getTime() - (timeZoneMilliseconds + serverMillisecondsOffset))
              data = this.handleAGRODATAINIZIOFromServer(data);
            } else {
              data = this.intlService.parseDate(a2.format());
            }
            dateVal = data;
          } else if (dateVal != null && dateVal != undefined && (dateVal.getHours() != 0 || dateVal.getMinutes() != 0 || dateVal.getSeconds() != 0 || dateVal.getMilliseconds() != 0)) {
            //Data con fuso Orario
            var a2 = momentTimezone(object[val]).tz(this.serverTimeZone);
            let data = this.intlService.parseDate((<string>object[val]));
            if (data != null && a2 != null && data.getTimezoneOffset() != a2.utcOffset() && data.getMilliseconds() == 0 && (this.serverTimeZone !=  this.clientTimeZone)){
              let timeZoneMilliseconds = data.getTimezoneOffset() * 60 * 1000
              let serverMillisecondsOffset = a2.utcOffset() * 60 * 1000;
              data.setTime(data.getTime() + (timeZoneMilliseconds + serverMillisecondsOffset))
              data = this.handleAGRODATAINIZIOFromServer(data);
            }
            dateVal = data;
          }
          if (dateVal != null) {
            this.MappedData[object[val]] = dateVal;
            object[val] = dateVal;
          }
        }
        else if (this.verifyMyDate(object[val])){
          //Verifica delle regEx per altri tipi di formati di date
          let dateVal = this.intlService.parseDate((<string>object[val]));
          if (dateVal != null && dateVal != undefined && (dateVal.getHours() != 0 || dateVal.getMinutes() != 0 || dateVal.getSeconds() != 0 || dateVal.getMilliseconds() != 0)){
            var a2 = momentTimezone(object[val]).tz(this.serverTimeZone);
            let data = this.intlService.parseDate((<string>object[val]));
            if (data.getTimezoneOffset() != a2.utcOffset() && (this.serverTimeZone !=  this.clientTimeZone)){
              let timeZoneMilliseconds = data.getTimezoneOffset() * 60 * 1000
              let serverMillisecondsOffset = a2.utcOffset() * 60 * 1000;
              if (!isNaN(serverMillisecondsOffset)){
                data.setTime(data.getTime() - (timeZoneMilliseconds + serverMillisecondsOffset))
                data = this.handleAGRODATAINIZIOFromServer(data);
              }
            }
            dateVal = data;

          }
          if (dateVal != null) {
            this.MappedData[object[val]] = dateVal;
            object[val] = dateVal;
          }
        }
      }
    });
    return object;
  }

  ConversionDateInObjectNew<T>(object: T): T {
    // if (this.masterService.serverTimeZoneOffset == Intl.DateTimeFormat().resolvedOptions().timeZone){
    //   return object;
    // }
    if (object == null || object == undefined) {
      return null;
    }
    if (isArray(object)){
      object = <any>this.ConversionDateInObject_Array(object);
      return object;
    }
    let fieldArray;
    try {
      fieldArray = Object.getOwnPropertyNames(object);
    } catch (e) {
      return object;
    }
    fieldArray.forEach((val, i) => {
      if (val.toString().toLowerCase() == 'data_creazione' || val.toString().toLowerCase() == 'data_modifica'){
        return object;
      }
      if (object[val] != null && object[val] !== undefined){
        if (typeof object[val] == 'object') {
          object[val] = this.ConversionDateInObject(object[val]);
        }
        if (typeof object[val] == 'string' && (<string>object[val]).includes('Date(') ) {
          //console.log("qui non ci vado più");
          if (this.MappedData[object[val]] != undefined){
            object[val] = this.MappedData[object[val]];
            return;
          }
          // var a1 = momentTimezone().tz("Europe/Rome");
          var a2 = momentTimezone(object[val]).tz("Europe/Rome");
          //var a2 = momentTimezone(object[val]).tz(this.masterService.serverTimeZoneOffset);
          // console.log(a1);
          // console.log(a2);

          let data = this.intlService.parseDate((<string>object[val]));
          //let data = new Date((<string>object[val]));

          if (data.getTimezoneOffset() != a2.utcOffset() && data.getMilliseconds() == 0){
            let timeZoneMilliseconds = data.getTimezoneOffset() * 60 * 1000
            let serverMillisecondsOffset = a2.utcOffset() * 60 * 1000;
            data.setTime(data.getTime() + (timeZoneMilliseconds + serverMillisecondsOffset))
            data = this.handleAGRODATAINIZIOFromServer(data);
          }
          this.MappedData[object[val]] = data;
          object[val] = data;
        }
        if (typeof object[val] == 'string' && moment(<string>object[val], moment.ISO_8601, true).isValid() && isNaN(Number(object[val]))) {
          object[val] = this.ConversioneStringaData(<string>object[val]);
        }
      }
    });
    return object;
  }

  private ConversioneStringaData(str: string): Date {
    if (str == null || str == undefined){
      return null;
    }
    if (this.MappedData.get(str) != undefined){
      return this.MappedData.get(str);
    }
    let dateVal = this.intlService.parseDate(str);

    if (dateVal != null && (dateVal.getHours() != 0 || dateVal.getMinutes() != 0 || dateVal.getSeconds() != 0 || dateVal.getMilliseconds() != 0)){
      var a2 = momentTimezone(str).tz("Europe/Rome");
      let data = this.intlService.parseDate(str);
      if (data.getTimezoneOffset() != a2.utcOffset()){
        let millisecondsxHour = 1000 * 60 * 60
        let timeZoneMilliseconds = data.getTimezoneOffset() * 60 * 1000
        let serverMillisecondsOffset = a2.utcOffset() * 60 * 1000;
        data.setTime(data.getTime() - (timeZoneMilliseconds + serverMillisecondsOffset))
        data = this.handleAGRODATAINIZIOFromServer(data);
      }
      dateVal = data;
    }
    if (str.length > 20 && (str[19] == '+' || str[19] == '-')){
      var a2 = momentTimezone(str).tz("Europe/Rome");
      let data = this.intlService.parseDate(str);
      if (data.getTimezoneOffset() != a2.utcOffset() && data.getMilliseconds() == 0){
        let millisecondsxHour = 1000 * 60 * 60
        let timeZoneMilliseconds = data.getTimezoneOffset() * 60 * 1000
        let serverMillisecondsOffset = a2.utcOffset() * 60 * 1000;
        data.setTime(data.getTime() + (timeZoneMilliseconds + serverMillisecondsOffset))
        data = this.handleAGRODATAINIZIOFromServer(data);
      }
      dateVal = data;
    }
    if (dateVal != null) {
      this.MappedData.set(str, dateVal);
    }
    return dateVal;
  }

  verifyMyDate(d) {
    return this.dtRegex1.test(d) || this.dtRegex2.test(d) || this.dtRegex3.test(d) || this.dtRegex4.test(d);
  }

  ConversionDateInObject_Array<T>(object: T[]): T[] {
    // if (this.masterService.serverTimeZoneOffset == Intl.DateTimeFormat().resolvedOptions().timeZone){
    //   return object;
    // }
    if (object == null || object == undefined) {
      return null;
    }
    if(object.length == 0){
      return object;
    }
    let dateObjectField = this.returnDateObjectField(object[0]);
    object.forEach((val) => {
      dateObjectField.dateField.forEach((field) => {
        try {
          val[field] = this.ConversioneStringaData(val[field]);
        } catch (e) {
          console.log("errore", e);
        }
      })
      dateObjectField.objectField.forEach((field) => {
        try {
          val[field] = this.ConversionDateInObject(val[field]);
        } catch (e) {
          console.log("errore", e);
        }

      })
    })
    return object;
  }

  private returnDateObjectField(object: any): {dateField: string[], objectField: string[] }{
    let dateField: string[] = [];
    let objectField: string[] = [];
    let fieldArray;
    try {
      fieldArray = Object.getOwnPropertyNames(object);
    } catch (e) {
      return object;
    }

    fieldArray.forEach((val, i) => {
      if (object[val] != null && object[val] !== undefined){
        if (typeof object[val] == 'object') {
          objectField.push(val);
        }
        if (typeof object[val] == 'string' && moment(<string>object[val], moment.ISO_8601, true).isValid() && isNaN(Number(object[val]))) {
          if ((this.masterService.serverTimeZoneOffset == Intl.DateTimeFormat().resolvedOptions().timeZone) && (val.toString().toLowerCase() == 'data_creazione' || val.toString().toLowerCase() == 'data_modifica')){

          } else {
            dateField.push(val);
          }
        } else if (typeof object[val] == 'string' && this.verifyMyDate(object[val]) && isNaN(Number(object[val]))) {
          if ((this.masterService.serverTimeZoneOffset == Intl.DateTimeFormat().resolvedOptions().timeZone) && (val.toString().toLowerCase() == 'data_creazione' || val.toString().toLowerCase() == 'data_modifica')){

          } else {
            dateField.push(val);
          }
        }
      }
    });

    return { dateField: dateField, objectField: objectField };
  }

  public convertStringToDate(inputVal: string): Date{
    let dateVal = this.intlService.parseDate(inputVal);
    if (this.masterService.serverTimeZoneOffset == Intl.DateTimeFormat().resolvedOptions().timeZone){
      return dateVal;
    }
    if (dateVal != null && (dateVal.getHours() != 0 || dateVal.getMinutes() != 0 || dateVal.getSeconds() != 0 || dateVal.getMilliseconds() != 0)){
      var a2 = momentTimezone(inputVal).tz("Europe/Rome");
      let data = this.intlService.parseDate(inputVal);
      if (data.getTimezoneOffset() != a2.utcOffset()){
        let millisecondsxHour = 1000 * 60 * 60
        let timeZoneMilliseconds = data.getTimezoneOffset() * 60 * 1000
        let serverMillisecondsOffset = a2.utcOffset() * 60 * 1000;
        data.setTime(data.getTime() - (timeZoneMilliseconds + serverMillisecondsOffset))
        data = this.handleAGRODATAINIZIOFromServer(data);
      }
      dateVal = data;
    }
    if (inputVal.length > 20 && (inputVal[19] == '+' || (inputVal[19] == '-'))){
      var a2 = momentTimezone(inputVal).tz("Europe/Rome");
      let data = this.intlService.parseDate(inputVal);
      if (data.getTimezoneOffset() != a2.utcOffset() && data.getMilliseconds() == 0){
        let millisecondsxHour = 1000 * 60 * 60
        let timeZoneMilliseconds = data.getTimezoneOffset() * 60 * 1000
        let serverMillisecondsOffset = a2.utcOffset() * 60 * 1000;
        data.setTime(data.getTime() + (timeZoneMilliseconds + serverMillisecondsOffset))
        data = this.handleAGRODATAINIZIOFromServer(data);
      }
      dateVal = data;
    }
    return dateVal;
  }

  remove__type<T>(object: T): T {
    if (object == null || object == undefined) {
      return null;
    }
    let fieldArray;
    try {
      fieldArray = Object.getOwnPropertyNames(object);
    } catch (e) {
      return object;
    }
    fieldArray.forEach((val, i) => {
      if (object[val] != null && object[val] !== undefined){
        if (typeof object[val] == 'object') {
          object[val] = this.remove__type(object[val]);
        }

        if (val === '__type') {
          delete object[val];
        }
      }
    });
    return object;
  }


  ConversionStrDateInStrDateTimeToServer<T>(object: T): T {
    if (object == null || object == undefined) {
      return null;
    }

    let fieldArray;
    try {
      fieldArray = Object.getOwnPropertyNames(object);
    } catch (e) {
      return object;
    }

    fieldArray.forEach((val, i) => {
      if (object[val] != null && object[val] !== undefined){
        if (typeof object[val] == 'object') {
          object[val] = this.ConversionStrDateInStrDateTimeToServer(object[val]);
        }
        if (typeof object[val] == 'string' && (<string>object[val]).includes('Date(') ) {
          object[val] = this.intlService.parseDate((<string>object[val]));
        }
        if (typeof object[val] == 'string' && moment(<string>object[val], moment.ISO_8601, true).isValid() && this.isIsoDate(<string>object[val]) ) {
          if ((<string>object[val]).substring(0, 4) == '0001' ) {
            return;
          }
          let data = new Date(object[val]);
          // var a2 = momentTimezone(object[val]).tz("Europe/Rome");
          // let millisecondsxHour = 1000 * 60 * 60
          // let timeZoneMilliseconds = data.getTimezoneOffset() * 60 * 1000
          // data.setTime(data.getTime() - (timeZoneMilliseconds + this.masterService.serverTimeZoneOffset))
          // let dataModified = this.handleAGRODATAINIZIOToServer(data);
          // let dateIso = dataModified.toISOString();
          // object[val] = dateIso;
          if (data.getMilliseconds() == 0){
            let year = ('' + data.getFullYear()).padStart(2, '0');
            let month = ('' + (data.getMonth() + 1)).padStart(2, '0');
            let day = ('' + data.getDate()).padStart(2, '0');
            let hours = ('' + data.getHours()).padStart(2, '0');
            let minutes = ('' + data.getMinutes()).padStart(2, '0');
            let seconds = ('' + data.getSeconds()).padStart(2, '0');
            let res = year + '-' + month + '-' + day + 'T' + hours + ':' + minutes + ':' + seconds
            //object[val] = data.toLocaleDateString('en-CA') + 'T' + data.toLocaleTimeString()
            object[val] = res;
          } else {
            object[val] = data.toISOString();
          }
          // let b = this.convertTZ(dateVal, 'Europe/Rome');
          // var newDateObj = new Date(dateVal.getTime() - (dateVal.getTimezoneOffset() + 60)*60000);
          // var newDateStr = newDateObj.toISOString();
          // if (dateVal != null) {
          //     object[val] = b.toJSON();
          // }
        }
      }
    });
    return object;
  }

  handleAGRODATAINIZIOToServer(date: Date) : Date {
    let millisecondsxday = 1000 * 60 * 60 * 24;
    let agro_inizio_top = new Date();
    agro_inizio_top.setTime(AGRODATAINIZIO.getTime() + (millisecondsxday * 2))
    let agro_inizio_bottom = new Date();
    agro_inizio_bottom.setTime(AGRODATAINIZIO.getTime() - (millisecondsxday * 2))

    if (date >=  agro_inizio_bottom && date <= agro_inizio_top ){
      //return new Date(-2208992400000);
      let d = new Date(1900, 0, 1, 0, 0, 0, 0)
      // d.setUTCSeconds(0);
      // d.setUTCMilliseconds(0);
      // let timeZoneMilliseconds = d.getTimezoneOffset() * 60 * 1000
      // d.setTime(d.getTime() - (timeZoneMilliseconds + this.masterService.serverTimeZoneOffset))
      return d;
    }
    return date;
  }

  handleAGRODATAINIZIOFromServer(date: Date) : Date {
    let millisecondsxday = 1000 * 60 * 60 * 24;
    let agro_inizio_top = new Date();
    agro_inizio_top.setTime(AGRODATAINIZIO.getTime() + (millisecondsxday * 2))
    let agro_inizio_bottom = new Date();
    agro_inizio_bottom.setTime(AGRODATAINIZIO.getTime() - (millisecondsxday * 2))

    if (date >=  agro_inizio_bottom && date <= agro_inizio_top ){
      //return new Date(-2208992400000);
      let d = new Date(1900, 0, 1, 0, 0, 0, 0)
      // d.setUTCSeconds(0);
      // d.setUTCMilliseconds(0);
      // let timeZoneMilliseconds = d.getTimezoneOffset() * 60 * 1000
      // d.setTime(d.getTime() + (timeZoneMilliseconds + this.masterService.serverTimeZoneOffset))
      return d;
    }
    return date;
  }

  isIsoDate(str) {
    if (!/\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3}Z/.test(str)) return false;
    const d : any = new Date(str);
    return d instanceof Date && d.toISOString()===str; // valid date
  }

  // getDate(dateStr: string): Date {
  //     let returnDate: Date;
  //     let dates_str = dateStr.split('T');
  //     let date = dates_str[0].split('-')
  //     let time = dates_str[1].split(':')
  //     let year: number = parseInt(date[0]);
  //     let month: number = parseInt(date[1]) - 1;
  //     let day: number = parseInt(date[2]);
  //     let hour: number = parseInt(time[0]);
  //     let minutes: number = parseInt(time[1]);
  //     let second: number = parseInt(time[2].split('.')[0]);
  //     let milliseconds: number = parseInt(time[2].split('.')[1].replace('Z', ''));
  //     returnDate = new Date(year, month, day, hour, minutes, second, milliseconds)
  //     return returnDate;
  // }

}
