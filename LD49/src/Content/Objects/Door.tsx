import Gash, { Line } from "web-gash";
import { IWorldObject } from "../../Entities/IWorldObject";

class Door implements IWorldObject {
  name: string = 'door';
  oiledUp = false;

  inspect(): void {    
    if (!this.oiledUp) {
      Gash.writeLine(<Line>An ordinary wooden door. As it opens and closes, its hinges make annoying squeaking.</Line>);
    } else {
      Gash.writeLine(<Line>An ordinary wooden door. Nice and quiet now.</Line>);
    }
  }
  getItems(): string[] {
    return [];
  }
  itemPickedUp(itemName: string): void {
  }
}

export const door = new Door();