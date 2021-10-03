import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IWorldObject } from "../../Entities/IWorldObject";

class ResonanceCollider implements IWorldObject {
  name: string = 'resonance collider';
  instructionsPickedUp = false;
  instructionsUsed = false;
  bananaUsed = false;

  private items: string[] = ['simple instructions'];
  inspect(): void {    
    if (!this.instructionsPickedUp) {
      Gash.writeLine(<Line>A complicated device for who-knows-what. Next to it is a sheet of paper with <ItemComponent item={'simple instructions'} /> on how to operate it.</Line>);
    } else {
      if (this.instructionsUsed && this.bananaUsed) {
        Gash.writeLine(<Line>A quiet, still and calibrated Resonance Collider. I think.</Line>);
      } else if (this.instructionsUsed) {
        Gash.writeLine(<Line>The strange device emits low mechanical humming.</Line>);
      } else {
        Gash.writeLine(<Line>A complicated device called Resonance Collider.</Line>);
      }
    }
  }
  getItems(): string[] {
    return this.items;
  }
  itemPickedUp(itemName: string): void {
    if (itemName === 'simple instructions') {
      this.instructionsPickedUp = true;
      this.items = [];
    }
  }
}

export const resonanceCollider = new ResonanceCollider();