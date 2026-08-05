// https://en.wikipedia.org/wiki/Disjoint-set_data_structure
export class MFSet<Entity> {
  private parent: Map<Entity, Entity> = new Map<Entity, Entity>();
  private rank: Map<Entity, number> = new Map<Entity, number>();

  constructor(elements: Entity[]) {
    for (const element of elements) {
      this.parent.set(element, element);
      this.rank.set(element, 0);
    }
  }

  add(element: Entity): void {
    this.parent.set(element, element);
    this.rank.set(element, 0);
  }

  has(key: Entity): boolean {
    return this.parent.has(key);
  }

  find(x: Entity): Entity {
    if (this.parent.get(x) != x) {
      this.parent.set(x, this.find(this.parent.get(x)));
    }

    return this.parent.get(x);
  }

  merge(x: Entity, y: Entity): void {
    const rx = this.find(x);
    const ry = this.find(y);
    if (rx == ry) {
      return;
    }

    if (this.rank.get(rx) > this.rank.get(ry)) {
      this.parent.set(ry, rx);
      return;
    }

    if (this.rank.get(ry) > this.rank.get(rx)) {
      this.parent.set(rx, ry);
      return;
    }

    this.parent.set(rx, ry);
    this.rank.set(ry, this.rank.get(ry) + 1);
  }
  
  reset(): void {
    for (const key of this.parent.keys()) {
      this.parent.set(key, key);
      this.rank.set(key, 0);
    }
  }
}