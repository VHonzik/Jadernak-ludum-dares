import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IItem } from "../../Entities/IItem";
import { Game } from "../../Game/Game";
import { resonanceCollider } from "../Objects/ResonanceCollider";

class Banana implements IItem {
  name: string = 'banana';
  pickedUp(): void {
    Gash.writeLine(<Line>I now have <ItemComponent item={'banana'}/>.</Line>)
  }
  inspect(): void {
    Gash.writeLine(<Line>Tasty looking banana... I have a job to do first though.</Line>)
  }
  usedOnObject(objectName: string): void {
    if (objectName === 'resonance collider') {
      if (!resonanceCollider.instructionsUsed) {
        Gash.writeLine(<Line>I probably shouldn’t start by the last step. Who knows what would happen to this space-reality dimension. Ha Ha. Ha.</Line>);
      } else {
        resonanceCollider.bananaUsed = true;
        Game.looseItem(this.name);
        Gash.writeLine(<Line>Banana finished, the pitch of the humming increases briefly and then the device goes silent.</Line>);
        Gash.writeLine(<Line>I guess this is my first successful Resonance Collider calibration.</Line>);
        Game.taskDone('collider');
      }
    } else {
      Gash.writeLine(<Line>That did not do anything.</Line>);
    }
  }
  combinedWith(itemName: string): void {
    Gash.writeLine(<Line>That did not do anything.</Line>);
  }
}

export const banana = new Banana();