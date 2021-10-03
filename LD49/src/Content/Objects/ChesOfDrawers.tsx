import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IWorldObject } from "../../Entities/IWorldObject";

class ChestOfDrawers implements IWorldObject {
  name: string = 'chest of drawers';
  toolboxPickedUp = false;

  private items: string[] = ['tool box'];
  inspect(): void {    
    if (!this.toolboxPickedUp) {
      Gash.writeLine(<Line>On the chest is my trusty <ItemComponent item={'tool box'} />.</Line>);
    } else {
      Gash.writeLine(<Line>Nothing of interest here anymore.</Line>);
    }
  }
  getItems(): string[] {
    return this.items;
  }
  itemPickedUp(itemName: string): void {
    if (itemName === 'tool box') {
      this.toolboxPickedUp = true;
      this.items = [];
    }
  }
}

export const chestOfDrawers = new ChestOfDrawers();