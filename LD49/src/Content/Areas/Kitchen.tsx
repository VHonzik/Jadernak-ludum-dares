import Gash, { Line } from "web-gash";
import { AreaComponent } from "../../Components/AreaComponent";
import { WorldObjectComponent } from "../../Components/WorldObjectComponent";
import { IArea } from "../../Entities/IArea";

class Kitchen implements IArea {
  name: string = 'kitchen';
  explore(): void {
    Gash.writeLine(<Line>I am in a small but cozy kitchen. I can see a <WorldObjectComponent worldObject={'cabinet'} />, a <WorldObjectComponent worldObject={'trash bin'} /> and a table with a <WorldObjectComponent worldObject={'fruit bowl'} /> on it. I can go to the <AreaComponent area={'hallway'} />.</Line>);
  }
  getConnectedAreas(): string[] {
    return ['hallway'];
  }
  getObjects(): string[] {
    return ['cabinet', 'trash bin', 'fruit bowl'];
  }
  entered(): void {
    Gash.writeLine(<Line>I have entered <AreaComponent area={this} />.</Line>);
  }
}

export const kitchen = new Kitchen();