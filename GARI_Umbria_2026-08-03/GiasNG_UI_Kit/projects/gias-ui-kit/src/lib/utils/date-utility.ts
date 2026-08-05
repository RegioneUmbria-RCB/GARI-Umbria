export namespace DateUtility {
  export const MILLISECONDS_PER_SECOND = 1000;
  export const SECONDS_PER_MINUTE = 60;
  export const MINUTES_PER_HOUR = 60;
  export const HOURS_PER_DAY = 24;
  export const DAYS_PER_WEEK = 7;
  export const MONTHS_PER_YEAR = 12;

  export const SECOND = MILLISECONDS_PER_SECOND;
  export const MINUTE = SECOND * SECONDS_PER_MINUTE;
  export const HOUR = MINUTE * MINUTES_PER_HOUR;
  export const DAY = HOUR * HOURS_PER_DAY;
  export const WEEK = DAY * DAYS_PER_WEEK;
  export const YEAR = DAY * 365.24;
  export const NORMAL_YEAR = DAY * 365;
  export const LEAP_YEAR = DAY * 366;

  export enum months {
    JANUARY = 0,
    FEBUARY,
    MARCH,
    APRIL,
    MAY,
    JUNE,
    JULY,
    AUGUST,
    SEPTEMBER,
    OCTOBER,
    NOVEMBER,
    DECEMBER
  }

  export function getCurrentYear(): number {
    return new Date().getFullYear();
  }
}
