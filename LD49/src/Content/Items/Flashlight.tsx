import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IItem } from "../../Entities/IItem";
import { Game } from "../../Game/Game";
import { roofWindow } from "../Objects/RoofWindow";

class Flashlight implements IItem {
  name: string = 'flashlight';
  pickedUp(): void {
    Gash.writeLine(<Line>I now have <ItemComponent item={'flashlight'}/>.</Line>)
  }
  inspect(): void {
    Gash.writeLine(<Line>Ordinary battery powered flashlight.</Line>);
  }
  usedOnObject(objectName: string): void {
    if (objectName === 'roof window' && !roofWindow.opened ) {
      Gash.writeLine(<Line>I can see the window handle now and carefully open it. Fresh air and light pour in.</Line>);
      roofWindow.opened = true;
      Game.taskDone('window');
    } else {
      Gash.writeLine(<Line>That did not do anything.</Line>);
    }
  }
  combinedWith(itemName: string): void {
    Gash.writeLine(<Line>That did not do anything.</Line>);
  }
}

export const flashlight = new Flashlight();