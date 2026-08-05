export class CommonFunctionsService {

  public static getOrigins(): string {
    if (window.location.origin.indexOf("localhost") > -1) {
      return '*'
    } else {
      return window.location.origin
    }
  }

  public static isJSON(str: string): boolean {
    if (/^\s*$/.test(str)) return false;
    str = str.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@');
    str = str.replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']');
    str = str.replace(/(?:^|:|,)(?:\s*\[)+/g, '');
    return (/^[\],:{}\s]*$/).test(str);
  }
};
