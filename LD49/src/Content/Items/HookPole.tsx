import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IItem } from "../../Entities/IItem";
import { hatch } from "../Objects/Hatch";

class HookPole implements IItem {
  name: string = 'hook pole';
  pickedUp(): void {
    Gash.writeLine(<Line>I now have <ItemComponent item={'hook pole'}/>.</Line>)
  }
  inspect(): void {
    Gash.writeLine(<Line>A long stick with a hook at the end.</Line>);
  }
  usedOnObject(objectName: string): void {
    if (objectName === 'hatch' && !hatch.opened ) {
      Gash.writeLine(<Line>The hatch opens and I extend the ladder, ready to climb.</Line>);
      hatch.opened = true;
    } else {
      Gash.writeLine(<Line>That did not do anything.</Line>);
    }
  }
  combinedWith(itemName: string): void {
    Gash.writeLine(<Line>That did not do anything.</Line>);
  }
}

export const hookPole = new HookPole();