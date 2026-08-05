import { Pipe, PipeTransform } from "@angular/core";
import { LinkMenu } from "app/Service/api.service";

@Pipe({
    standalone: false,
    name: 'filterArr',
    pure: false
})
export class FilterPipe implements PipeTransform {
    transform(arr: [], filterText: string) {
        if (arr == null || arr.length === 0 || filterText === "") {
            return arr;
        }

        return arr.filter((obj: LinkMenu) =>
            obj.testo.toLowerCase().includes(filterText.toLowerCase()) || obj.Figli.filter(x => x.testo.toLowerCase().includes(filterText.toLowerCase())).length > 0
        );
    }
}
