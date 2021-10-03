import Gash, { Line } from "web-gash";
import { IWorldObject } from "../../Entities/IWorldObject";

class TrashBin implements IWorldObject {
  name: string = 'trash bin';
  takenOut = false;

  inspect(): void {    
    if (!this.takenOut) {
      Gash.writeLine(<Line>A bin that does need emptying. Luckily it doesn’t smell yet. I should put a new bag in it as well.</Line>);
    } else {
      Gash.writeLine(<Line>An empty trash bin.</Line>);
    }
  }
  getItems(): string[] { return []; }
  itemPickedUp(itemName: string): void {}
}

export const trashBin = new TrashBin();