import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IWorldObject } from "../../Entities/IWorldObject";

class SecuredPole implements IWorldObject {
  name: string = 'secured pole';
  polePickedUp = false;

  private items: string[] = ['hook pole'];
  inspect(): void {    
    if (!this.polePickedUp) {
      Gash.writeLine(<Line>A nail in a wall on which a <ItemComponent item={'hook pole'} /> is hanging.</Line>);
    } else {
      Gash.writeLine(<Line>A nail in a wall.</Line>);
    }
  }
  getItems(): string[] {
    return this.items;
  }
  itemPickedUp(itemName: string): void {
    if (itemName === 'hook pole') {
      this.polePickedUp = true;
      this.items = [];
    }
  }
}

export const securedPole = new SecuredPole();