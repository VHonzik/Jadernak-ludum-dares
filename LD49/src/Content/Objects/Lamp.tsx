import Gash, { Line } from "web-gash";
import { IWorldObject } from "../../Entities/IWorldObject";

class Lamp implements IWorldObject {
  name: string = 'lamp';
  repaired = false;

  inspect(): void {    
    if (!this.repaired) {
      Gash.writeLine(<Line>A reading lamp by the armchair that does not work. I should try to find a spare light bulb.</Line>);
    } else {
      Gash.writeLine(<Line>A lamp next to the armchair.</Line>);
    }
  }
  getItems(): string[] {
    return [];
  }
  itemPickedUp(itemName: string): void {
  }
}

export const lamp = new Lamp();