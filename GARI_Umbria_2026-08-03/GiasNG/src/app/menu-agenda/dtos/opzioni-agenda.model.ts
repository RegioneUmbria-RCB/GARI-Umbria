import {ImpiantoItem} from "../components/utils";

/**
 * Represents the options for the menu agenda page.
 */
export class OpzioniAgenda {
  /**
   * Indicates whether side-page filters should be hidden.
   * @type {boolean}
   */
  public nascondiFiltri = false;

  /**
   * The source of the data, which can be 'GIS' or undefined if coming from the normal menu.
   * @type {'GIS' | undefined}
   */
  public source: 'GIS' | undefined;

  /**
   * A list of impianti items used to filter the activities.
   * @type {ImpiantoItem[]}
   */
  public impianti: ImpiantoItem[];

  /**
   * Creates an instance of OpzioniAgenda.
   * @param {Partial<{ nascondiFiltri: boolean, source: 'GIS' | undefined, impianti: ImpiantoItem[] }>} data - The initial data for the options.
   */
  constructor(data: Partial<{ nascondiFiltri: boolean; source: 'GIS' | undefined; impianti: ImpiantoItem[] }>) {
    this.nascondiFiltri = data.nascondiFiltri ?? false;
    this.source = data.source ?? undefined;
    this.impianti = data.impianti ?? [];
  }

  /**
   * Checks if the source is 'GIS'.
   * @returns {boolean} True if the source is 'GIS', otherwise false.
   */
  get isFromGis(): boolean {
    return this.source === 'GIS';
  }
}
