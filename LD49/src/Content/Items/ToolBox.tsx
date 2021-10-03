import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IItem } from "../../Entities/IItem";
import { Game } from "../../Game/Game";
import { door } from "../Objects/Door";

class ToolBox implements IItem {
  name: string = 'tool box';
  pickedUp(): void {
    Gash.writeLine(<Line>I now have <ItemComponent item={'tool box'}/>.</Line>)
  }
  inspect(): void {
    Gash.writeLine(<Line>A wooden box filled with handy tools for everyday maintenance. I have had it for years now.</Line>);
  }
  usedOnObject(objectName: string): void {
    if (objectName === 'door' && !door.oiledUp ) {
      Gash.writeLine(<Line>I clean up and oil the hinges and the door is silent again. Job well done.</Line>);
      door.oiledUp = true;
      Game.taskDone('door');
    } else {
      Gash.writeLine(<Line>That did not do anything.</Line>);
    }
  }
  combinedWith(itemName: string): void {
    Gash.writeLine(<Line>That did not do anything.</Line>);
  }
}

export const toolBox = new ToolBox();