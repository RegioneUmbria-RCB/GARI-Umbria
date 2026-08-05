
import { Injectable } from '@angular/core';
import { SIMBOLO_M3_HA, SIMBOLO_MM } from 'app/Model/TipiEnumerativi';

@Injectable({
  providedIn: 'root'
})
export class IrrigazioneAcquaService {

    public calcolaPortataFertirrigazione(row?: any): any {
        if (!row){
            return;
        }

        if (row.OreIrrigazione > 0 ){
            row.Portata = Math.max(Math.round(row.QtaTotaleAcquaGiornaliera * 1000 / row.OreIrrigazione), 1);
            //recalculate row.OreIrrigazione to avoid having values that are not consistent with Portata
            row.OreIrrigazione = row.QtaTotaleAcquaGiornaliera * 1000 / row.Portata;
        } else {
            row.Portata = 0;
        }
    }

    public calcolaOreFertirrigazione(row?: any): void {
        if (!row){
            return;
        }

        if (row.Portata > 0){
            row.OreIrrigazione = row.QtaTotaleAcquaGiornaliera * 1000 / row.Portata;
        } else {
            row.OreIrrigazione = 0;
        }
    }

    public calculateFertirrigationValuesFromDoseAcqua(row: any, doseAcquaGiornaliera: number, superficie: number, dataInizio: Date, dataFine: Date, efficienza: any): void {
        if (!row || !dataInizio || !dataFine) {
            return;
        }

        const daysDiff = this.getDaysDifference(dataInizio, dataFine);
        const daysDiffWithFreq = Math.max(Math.floor(daysDiff / row.FrequenzaIrrigazioneMedia), 1);
        const acquaHa = doseAcquaGiornaliera * 10 * daysDiffWithFreq;
        this.calculateFertirrigationValuesInternal(row, acquaHa, superficie, efficienza, daysDiffWithFreq);
    }

    public gestisciCambioPortataIrrigazione(impianto: any, superficie: number, dataInizio: Date, dataFine: Date, efficienza: number, portata: number): void {
        if (!impianto || !dataInizio || !dataFine){
            return;
        }

        if (portata == 0) {
            impianto.OreIrrigazione = 0;
            return;
        }

        if (impianto.OreIrrigazione > 0){
            this.calculateIrrigationValuesFromPortataAndOreIrrigazione(superficie, impianto, portata, impianto.OreIrrigazione, dataInizio, dataFine, efficienza);
        } else if (impianto.QtaTotaleAcquaGiornaliera > 0){
            impianto.OreIrrigazione = impianto.QtaTotaleAcquaGiornaliera * 1000 / impianto.Portata;
        }
    }


    public gestisciCambioOreIrrigazione(impianto: any, superficie: number, dataInizio: Date, dataFine: Date, efficienza: number, oreIrrigazione: number): void {
        if (!impianto || !dataInizio || !dataFine){
            return;
        }

        if (oreIrrigazione == 0) {
            impianto.Portata = 0;
            return;
        }

        if (impianto.Portata > 0){
            this.calculateIrrigationValuesFromPortataAndOreIrrigazione(superficie, impianto, impianto.Portata, oreIrrigazione, dataInizio, dataFine, efficienza);
        } else if (impianto.QtaTotaleAcquaGiornaliera > 0){
            impianto.Portata = Math.max(Math.round(impianto.QtaTotaleAcquaGiornaliera * 1000 / impianto.OreIrrigazione), 1);
            //recalculate oreIrrigazione to avoid having values that are not consistent with Portata
            impianto.OreIrrigazione = impianto.QtaTotaleAcquaGiornaliera * 1000 / impianto.Portata;
        }
    }

    private calculateIrrigationValuesFromPortataAndOreIrrigazione(superficie: number, impianto: any, portata: number, oreIrrigazione: number, dataInizio: Date, dataFine: Date, efficienza: number) {
        if (superficie > 0) {

            impianto.QtaTotaleAcquaGiornaliera = (oreIrrigazione * portata) / 1000; //conversione da litri/gg a m3/gg
            if (impianto.UdmDose === SIMBOLO_M3_HA) {
                impianto.DoseAcquaGiornaliera = impianto.QtaTotaleAcquaGiornaliera / superficie; // m3/Ha
            } else if (impianto.UdmDose === SIMBOLO_MM) {
                impianto.DoseAcquaGiornaliera = impianto.QtaTotaleAcquaGiornaliera / (superficie * 10); // mm
            }

            const daysDiff = this.getDaysDifference(dataInizio, dataFine);
            const daysDiffWithFreq = Math.max(Math.floor(daysDiff / impianto.FrequenzaIrrigazioneMedia), 1);

            impianto.QtaTotaleAcquaUtilizzataPeriodo = impianto.QtaTotaleAcquaGiornaliera * daysDiffWithFreq;
            this.calcolaValoriAssorbitiIrrigazione(impianto, efficienza);

        } else {
            impianto.QtaTotaleAcquaUtilizzataPeriodo = 0;
            impianto.QtaTotaleAcquaAssorbitaPeriodo = 0;
            impianto.QtaTotaleAcquaGiornaliera = 0;
            impianto.QtaTotaleAcquaAssorbitaGiornaliera = 0;
        }
    }

    public calculateIrrigationWaterValues(impianto: any, dose: number, superficie: number, dataInizio: Date, dataFine: Date, efficienza: number): void {
        if (!impianto || !dataInizio || !dataFine){
            return;
        }

        if (superficie > 0 && dose > 0){
            if (impianto.UdmDose === SIMBOLO_M3_HA){
                impianto.QtaTotaleAcquaGiornaliera = dose * superficie;
            } else if (impianto.UdmDose === SIMBOLO_MM){
                impianto.QtaTotaleAcquaGiornaliera = dose * superficie * 10;
                // 1m=1000mm e 1ha=10000 m2; dividiamo per 1000 (per trasformare mm in m) e moltiplichiamo per 10000 
                // (per trasformare m2 in ha); equivale a moltiplicare per 10
            }

            const daysDiff = this.getDaysDifference(dataInizio, dataFine);
            const daysDiffWithFreq = Math.max(Math.floor(daysDiff / impianto.FrequenzaIrrigazioneMedia), 1);

            impianto.QtaTotaleAcquaUtilizzataPeriodo = impianto.QtaTotaleAcquaGiornaliera * daysDiffWithFreq;
            this.calcolaValoriAssorbitiIrrigazione(impianto, efficienza);

            if (impianto.Portata > 0){
                impianto.OreIrrigazione = impianto.QtaTotaleAcquaGiornaliera * 1000 / impianto.Portata;
            } else{
                impianto.OreIrrigazione = 0;
            }
        } else {
            impianto.QtaTotaleAcquaUtilizzataPeriodo = 0;
            impianto.QtaTotaleAcquaAssorbitaPeriodo = 0;
            impianto.QtaTotaleAcquaGiornaliera = 0;
            impianto.QtaTotaleAcquaAssorbitaGiornaliera = 0;
        }
    }

    public calculateFertirrigationWaterValues(row: any, acquaHa: number, superficie: number, dataInizio: Date, dataFine: Date, efficienza: any): void {
        if (!row || !dataInizio || !dataFine) {
            return;
        }

        const daysDiff = this.getDaysDifference(dataInizio, dataFine);
        const daysDiffWithFreq = Math.max(Math.floor(daysDiff / row.FrequenzaIrrigazioneMedia), 1);
        row.DoseAcquaGiornaliera = acquaHa / 10 / daysDiffWithFreq;
        this.calculateFertirrigationValuesInternal(row, acquaHa, superficie, efficienza, daysDiffWithFreq);
    }

    private calculateFertirrigationValuesInternal(row: any, acquaHa: number, superficie: number, efficienza: any, daysDiffWithFreq: number): void {
        if (superficie > 0){
            // Calculate total water used for the period
            row.QtaTotaleAcquaUtilizzataPeriodo = acquaHa * superficie / 10;
            // Calculate daily water usage
            row.QtaTotaleAcquaGiornaliera = row.QtaTotaleAcquaUtilizzataPeriodo / daysDiffWithFreq;

            this.calcolaValoriAssorbitiIrrigazione(row, efficienza);

            if (row.Portata > 0){
                row.OreIrrigazione = row.QtaTotaleAcquaGiornaliera * 1000 / row.Portata;
            } else{
                row.OreIrrigazione = 0;
            }
        } else {
            row.DoseAcquaGiornaliera = 0;
            row.QtaTotaleAcquaUtilizzataPeriodo = 0;
            row.QtaTotaleAcquaAssorbitaPeriodo = 0;
            row.QtaTotaleAcquaGiornaliera = 0;
            row.QtaTotaleAcquaAssorbitaGiornaliera = 0;
            row.OreIrrigazione = 0;
        }
    }

    public calcolaValoriAssorbitiIrrigazione(row: any, efficienza: number): void {
        row.QtaTotaleAcquaAssorbitaPeriodo = (row.QtaTotaleAcquaUtilizzataPeriodo * efficienza) / 100;
        row.QtaTotaleAcquaAssorbitaGiornaliera = (row.QtaTotaleAcquaGiornaliera * efficienza) / 100;
    }

    public getDaysDifference(dataInizio: Date | string, dataFine: Date | string): number {
        if (!dataInizio || !dataFine) return 0;

        const parseInputDate = (input: Date | string): Date => {
            if (input instanceof Date) {
                return new Date(input.getFullYear(), input.getMonth(), input.getDate());
            } else if (typeof input === 'string') {
                // Handle dd/MM/yyyy format
                const parts = input.split('/');
                if (parts.length === 3) {
                    const day = parseInt(parts[0], 10);
                    const month = parseInt(parts[1], 10) - 1; // JS months are 0-based
                    const year = parseInt(parts[2], 10);
                    return new Date(year, month, day);
                } else {
                    // Fallback to built-in parser if format doesn't match
                    const fallback = new Date(input);
                    if (isNaN(fallback.getTime())) {
                        throw new Error(`Invalid date format: ${input}`);
                    }
                    return new Date(fallback.getFullYear(), fallback.getMonth(), fallback.getDate());
                }
            }

            throw new Error('Invalid input type for date');
        };

        const d1 = parseInputDate(dataInizio);
        const d2 = parseInputDate(dataFine);

        // per evitare differenze nei mesi di marzo / ottobre dovuti al cambio di ora
        const utcMs1 = Date.UTC(d1.getFullYear(), d1.getMonth(), d1.getDate());
        const utcMs2 = Date.UTC(d2.getFullYear(), d2.getMonth(), d2.getDate());
        const diffMs = Math.abs(utcMs2 - utcMs1);
        return Math.floor(diffMs / (1000 * 60 * 60 * 24)) + 1;
    }
}