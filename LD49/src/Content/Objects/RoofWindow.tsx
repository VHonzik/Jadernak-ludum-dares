import Gash, { Line } from "web-gash";
import { IWorldObject } from "../../Entities/IWorldObject";

class RoofWindow implements IWorldObject {
  name: string = 'roof window';
  opened = false;

  inspect(): void {    
    if (!this.opened) {
      Gash.writeLine(<Line>A completely covered roof window. It’s really dark in here. I will need some light to open the window.</Line>);
    } else {
      Gash.writeLine(<Line>A fresh breeze and plenty of sun come from the opened window.</Line>);
    }
  }
  getItems(): string[] { return []; }
  itemPickedUp(itemName: string): void {}
}

export const roofWindow = new RoofWindow();