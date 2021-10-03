export interface IItem {
  name: string,
  pickedUp(): void,
  inspect(): void,
  usedOnObject(objectName: string): void;
  combinedWith(itemName: string): void;
}