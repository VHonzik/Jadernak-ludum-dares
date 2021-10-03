import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IItem } from "../../Entities/IItem";
import { Game } from "../../Game/Game";
import { trashBin } from "../Objects/TrashBin";

class TrashBagRoll implements IItem {
  name: string = 'trash bag roll';
  pickedUp(): void {
    Gash.writeLine(<Line>I now have <ItemComponent item={'trash bag roll'}/>.</Line>)
  }
  inspect(): void {
    Gash.writeLine(<Line>Roll of black plastic trash bags that smells vaguely of some flower.</Line>);
  }
  usedOnObject(objectName: string): void {
    if (objectName === 'trash bin') {
      Gash.writeLine(<Line>I switch the full and the empty. I can take the full trash bag out when I am leaving.</Line>);
      trashBin.takenOut = true;
      Game.taskDone('trash');
    } else {
      Gash.writeLine(<Line>That did not do anything.</Line>);
    }
  }
  combinedWith(itemName: string): void {
    Gash.writeLine(<Line>That did not do anything.</Line>);
  }
}

export const trashBagRoll = new TrashBagRoll();