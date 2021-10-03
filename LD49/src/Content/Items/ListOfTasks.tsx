import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IItem } from "../../Entities/IItem";

class ListOfTasks implements IItem {
  name: string = 'list of tasks';

  trashDone = false;
  doorDone = false;
  windowDone = false;
  colliderDone = false;
  lampDone = false;

  pickedUp(): void {
    Gash.writeLine(<Line>I now have <ItemComponent item={'list of tasks'}/>.</Line>)
  }
  inspect(): void {
    Gash.writeLine(<Line>List of tasks I was paid to perform in this house. It reads:</Line>);
    Gash.writeLine(<Line tabs={1}>- Take out the trash {this.trashDone ? '(DONE)' : ''}</Line>);
    Gash.writeLine(<Line tabs={1}>- Repair a squeaky door upstairs {this.doorDone ? '(DONE)' : ''}</Line>);
    Gash.writeLine(<Line tabs={1}>- Open a roof window {this.windowDone ? '(DONE)' : ''}</Line>);
    Gash.writeLine(<Line tabs={1}>- Calibrate the Resonance Collider {this.colliderDone ? '(DONE)' : ''}</Line>);
    Gash.writeLine(<Line tabs={1}>- Fix the lamp in living room {this.lampDone ? '(DONE)' : ''}</Line>);
  }
  usedOnObject(objectName: string): void {
    Gash.writeLine(<Line>That did not do anything.</Line>);
  }
  combinedWith(itemName: string): void {
    Gash.writeLine(<Line>That did not do anything.</Line>);
  }
}

export const listOfTasks = new ListOfTasks();