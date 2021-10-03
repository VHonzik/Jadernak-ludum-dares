import Gash, { Line } from "web-gash";
import { ItemComponent } from "../../Components/ItemComponent";
import { IItem } from "../../Entities/IItem";
import { resonanceCollider } from "../Objects/ResonanceCollider";

class SimpleInstructions implements IItem {
  name: string = 'simple instructions';
  pickedUp(): void {
    Gash.writeLine(<Line>I now have <ItemComponent item={'simple instructions'}/>.</Line>)
  }
  inspect(): void {
    Gash.writeLine(<Line>A list of easy to comprehend steps on how to calibrate Resonance Collider.</Line>)
    Gash.writeLine(<Line>Curiously, the last step says “Eat a banana next to it”.</Line>)
  }
  usedOnObject(objectName: string): void {
    if (objectName === 'resonance collider' && !resonanceCollider.instructionsUsed ) {
      Gash.writeLine(<Line>After a few knob turns, toggle switches and button presses the device emits low mechanical humming. Now for the last step.</Line>);
      resonanceCollider.instructionsUsed = true;
    } else {
      Gash.writeLine(<Line>That did not do anything.</Line>);
    }
  }
  combinedWith(itemName: string): void {
    Gash.writeLine(<Line>That did not do anything.</Line>);
  }
}

export const simpleInstructions = new SimpleInstructions();