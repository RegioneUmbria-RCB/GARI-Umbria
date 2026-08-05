import { PanelBarItemModel } from '@progress/kendo-angular-layout';

export const panelItems: Array<PanelBarItemModel> = [
  <PanelBarItemModel>{
      title: 'First item',
      content: 'First item content',
      expanded: true,
  },
  <PanelBarItemModel>{
      title: 'Second item',
      children: [<PanelBarItemModel>{ title: 'Second item child item' }],
  },
];

export const FavoriteLinks = {
    //OperazioniPreferite_Old: '/AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_OperazioniPreferite',
    OperazioniPreferite: 'AgronicaCoreUtentiBIZ/Get_OperazioniPreferite',
    //OperazioniPerRicette_Old: 'AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_OperazioniPreferite_PerRicette',
    OperazioniPerRicette: '/AgronicaCoreUtentiBIZ/Get_OperazioniPreferite_PerRicette'
};


export type Operations = Operation | Operation2;
export interface Operation { lav_cod: string; lav_des: string }
export interface Operation2 { lav_cod2: string; lav_des2: string }

export class OperationSelector {
    static current: Operations;

    static isOperation1(pet: Operations): pet is Operation {
        return (<Operation>pet) !== undefined;
    }

    static GetCurrentType() {
        if(this.isOperation1(OperationSelector.current)) {
            return OperationSelector.current;
        }
    }
}



export function ConvertToPanelItems(data: Operation[]): PanelBarItemModel[] {
    const result: PanelBarItemModel[] = [];
    data.forEach((item) => {
        result.push( ItemSelector.Parse(item) );
    });

    return result;
}

export class ItemSelector {
    static isOperation(op: Operations): op is Operation {
        return (op as Operation).lav_cod !== undefined;
    }

    static Parse(item: Operations): PanelBarItemModel {

        if(this.isOperation(item)) {
            return <PanelBarItemModel>{ title: item.lav_des, id: item.lav_cod };
        } else {
            return <PanelBarItemModel>{ title: item.lav_des2, id: item.lav_cod2 };
        }
    }
}
