import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IWorldObject } from "../../Entities/IWorldObject";

class Boxes implements IWorldObject {
  name: string = 'boxes';
  bulbBagPickedUp = false;

  private items: string[] = ['light bulb'];
  inspect(): void {    
    if (!this.bulbBagPickedUp) {
      Gash.writeLine(<Line>A collection of boxes with all kinds of things in them. I found spare <ItemComponent item={'light bulb'} />s in one of them.</Line>);
    } else {
      Gash.writeLine(<Line>A collection of boxes with all kinds of things in them.</Line>);
    }
  }
  getItems(): string[] {
    return this.items;
  }
  itemPickedUp(itemName: string): void {
    if (itemName === 'light bulb') {
      this.bulbBagPickedUp = true;
      this.items = [];
    }
  }
}

export const boxes = new Boxes();