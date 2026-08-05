import { Injectable } from "@angular/core";
import { ImpreseService } from "app/Service/Anagrafica/imprese.service";

@Injectable({
    providedIn: 'root'
})
export class TestAnagraficaService {
    public PivaSelezionata: string = "";
}
