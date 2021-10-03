import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IWorldObject } from "../../Entities/IWorldObject";

class NightStand implements IWorldObject {
  name: string = 'night stand';
  flashlightPickedUp = false;

  private items: string[] = ['flashlight'];
  inspect(): void {    
    if (!this.flashlightPickedUp) {
      Gash.writeLine(<Line>I will attempt to erase the contents of the second drawer of this night stand from my memory. First drawer has a <ItemComponent item={'flashlight'} />.</Line>);
    } else {
      Gash.writeLine(<Line>I would rather not open the night stand again.</Line>);
    }
  }
  getItems(): string[] {
    return this.items;
  }
  itemPickedUp(itemName: string): void {
    if (itemName === 'flashlight') {
      this.flashlightPickedUp = true;
      this.items = [];
    }
  }
}

export const nightStand = new NightStand();