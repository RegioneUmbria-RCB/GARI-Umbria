export class Balance { 
    year: number;
    costs: number;
    revenues: number; 
    explanation: string;
}

export class CostsCrop {
    id: string;
    description: string;
    costs: number;
    explanation: string;
}

export class RevenuesCrop {
    id: string;
    description: string;
    revenues: number;
    explanation: string;
}

export class CostsRevenuesTot {
    Balances: Balance[];
    CostsCrops: CostsCrop[];
    RevenuesCrops: RevenuesCrop[];
}