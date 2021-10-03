import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IWorldObject } from "../../Entities/IWorldObject";

class Cabinet implements IWorldObject {
  name: string = 'cabinet';
  trashBagPickedUp = false;

  private items: string[] = ['trash bag roll'];
  inspect(): void {    
    if (!this.trashBagPickedUp) {
      Gash.writeLine(<Line>A half-open kitchen cabinet filled with everyday supplies. Among other things, I notice a <ItemComponent item={'trash bag roll'} />.</Line>);
    } else {
      Gash.writeLine(<Line>A half-open kitchen cabinet filled with everyday supplies.</Line>);
    }
  }
  getItems(): string[] {
    return this.items;
  }
  itemPickedUp(itemName: string): void {
    if (itemName === 'trash bag roll') {
      this.trashBagPickedUp = true;
      this.items = [];
    }
  }
}

export const cabinet = new Cabinet();