import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IWorldObject } from "../../Entities/IWorldObject";

class FruitBowl implements IWorldObject {
  name: string = 'fruit bowl';

  private items: string[] = ['banana'];
  inspect(): void {
    Gash.writeLine(<Line>A glass bowl filled with various fruits including a couple of <ItemComponent item={'banana'} />s.</Line>);
  }
  getItems(): string[] {
    return this.items;
  }
  itemPickedUp(itemName: string): void {}
}

export const fruitBowl = new FruitBowl();