import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IItem } from "../../Entities/IItem";
import { Game } from "../../Game/Game";
import { lamp } from "../Objects/Lamp";


class LightBulb implements IItem {
  name: string = 'light bulb';
  pickedUp(): void {
    Gash.writeLine(<Line>I now have <ItemComponent item={'light bulb'}/>.</Line>)
  }
  inspect(): void {
    Gash.writeLine(<Line>Spare clear light bulb.</Line>)
  }
  usedOnObject(objectName: string): void {
    if (objectName === 'lamp' && !lamp.repaired) {
      Gash.writeLine(<Line>I replace the bulb in the lamp and that fixes it.</Line>);
      Game.looseItem(this.name);
      lamp.repaired = true;
      Game.taskDone('lamp');
    } else {
      Gash.writeLine(<Line>That did not do anything.</Line>);
    }
  }
  combinedWith(itemName: string): void {
    Gash.writeLine(<Line>That did not do anything.</Line>);
  }
}

export const lightBulb = new LightBulb();