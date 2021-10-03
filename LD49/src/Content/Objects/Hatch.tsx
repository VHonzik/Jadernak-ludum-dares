import Gash, { Line } from "web-gash";
import { IWorldObject } from "../../Entities/IWorldObject";

class Hatch implements IWorldObject {
  name: string = 'hatch';
  opened = false;

  inspect(): void {    
    if (!this.opened) {
      Gash.writeLine(<Line>A hatch in the ceiling, presumably leading to the attic. I can’t quite reach the pull ring.</Line>);
    } else {
      Gash.writeLine(<Line>An opened hatch with a retractable ladder leading up.</Line>);
    }
  }
  getItems(): string[] {
    return [];
  }
  itemPickedUp(itemName: string): void {
  }
}

export const hatch = new Hatch();