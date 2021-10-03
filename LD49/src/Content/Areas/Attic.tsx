import Gash, { Line } from "web-gash";
import { AreaComponent } from "../../Components/AreaComponent";
import { WorldObjectComponent } from "../../Components/WorldObjectComponent";
import { IArea } from "../../Entities/IArea";
import { roofWindow } from "../Objects/RoofWindow";

class Attic implements IArea {
  name: string = 'attic';
  explore(): void {
    if (!roofWindow.opened) {
      Gash.writeLine(<Line>I can’t see anything but I think I am in the attic. The only barely visible object is a <WorldObjectComponent worldObject={'roof window'} />. I can climb down to the <AreaComponent area={'landing'} />.</Line>);
    } else {
      Gash.writeLine(<Line>I am in an attic filled with junk and a lot of <WorldObjectComponent worldObject={'boxes'} />.</Line>);
      Gash.writeLine(<Line>There is an opened <WorldObjectComponent worldObject={'roof window'} />. I can climb down to the <AreaComponent area={'landing'} />.</Line>);
    }
  }
  getConnectedAreas(): string[] {
    return ['landing'];
  }
  getObjects(): string[] {
    const objects = ['roof window'];
    if (roofWindow.opened) {
      objects.push('boxes');
    }
    return objects;
  }
  entered(): void {
    Gash.writeLine(<Line>I have entered <AreaComponent area={this} />.</Line>);
  }
}

export const attic = new Attic();