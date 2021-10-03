export interface IWorldObject {
  name: string,
  inspect(): void,
  getItems(): string[],
  itemPickedUp(itemName: string): void,
}