export class Layout {
  public constructor(public id: string, public path: string) {}

  public toString(): string {
    return `Layout: ${this.id}`;
  }
}
