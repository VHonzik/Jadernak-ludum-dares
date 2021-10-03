export interface IArea {
  name: string,
  explore(): void,
  getConnectedAreas(): string[],
  getObjects(): string[],
  entered(): void
}