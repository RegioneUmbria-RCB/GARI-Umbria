import { Pipe, PipeTransform } from "@angular/core";
import { CustomLinkMenu } from "./sidemenu-services.component";

@Pipe({
  standalone: false,
  name: 'filterChildren',
  pure: false
})
export class FilterLinkMenuChildrenPipe implements PipeTransform {
  transform(menu: CustomLinkMenu, filterText: string) {
    const text = filterText.toLowerCase();
    if (text === "" || menu.testo.toLowerCase().includes(text)) {
      menu.hidden = false;
      return menu.Figli;
    }

    const result = menu.Figli.filter(obj => obj.testo.toLowerCase().includes(text));
    menu.hidden = result.length == 0;
    return result;
  }
}
