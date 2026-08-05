import { DropdownListItem } from '../../models/grid.model';

export function parseJson(str: string): DropdownListItem {
    try {
        return JSON.parse(str);
    } catch (e) {
        return null;
    }
}
