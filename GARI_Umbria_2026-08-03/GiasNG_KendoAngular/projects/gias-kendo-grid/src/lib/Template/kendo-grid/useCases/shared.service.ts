
export class Shared {
    public static changeGridPage(index: number, pageSize: number): number {
        return Math.floor(index / pageSize) * pageSize;
    }
}
