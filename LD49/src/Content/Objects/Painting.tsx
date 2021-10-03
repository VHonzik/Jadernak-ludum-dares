import Gash, { Line } from "web-gash";
import { WorldObjectComponent } from "../../Components/WorldObjectComponent";
import { IWorldObject } from "../../Entities/IWorldObject";
import { Game } from "../../Game/Game";

class Painting implements IWorldObject {
  name: string = 'painting';
  lastSeenOnTask: number = -1;
  inspect(): void {    
    if (Game.tasksDone === 0) {
      Gash.writeLine(<Line>The painting is a hill scenery with a small red dot on one of the hills.</Line>);
    } else if (Game.tasksDone === 1) {
      Gash.writeLine(<Line>The painting features a red sports car, far away on a hill, coming down a winding road.</Line>);
    } else if (Game.tasksDone === 2) {
      Gash.writeLine(<Line>On the painting there is a red sports car coming down a winding hill road with a blond-haired driver.</Line>);
    } else if (Game.tasksDone === 3) {
      Gash.writeLine(<Line>The painting features a blond girl in a red sports car cruising on a hilly road.</Line>);
    } else if (Game.tasksDone === 4) {
      Gash.writeLine(<Line>A blond girl with a colorful headscarf is speeding in a red sports car, sharply taking a turn.</Line>);
    }
  }
  getItems(): string[] { return []; }
  itemPickedUp(itemName: string): void {}
  checkLastSeen() {
    if (Game.tasksDone > this.lastSeenOnTask) {
      this.lastSeenOnTask = Game.tasksDone;
      if (Game.tasksDone === 0) {
        Gash.writeLine(<Line>I feel there is something strange about the <WorldObjectComponent worldObject={'painting'} />.</Line>);
      } else if (Game.tasksDone === 1) {
        Gash.writeLine(<Line>I glance at the <WorldObjectComponent worldObject={'painting'} /> and it feels different. I should inspect it.</Line>);
      } else if (Game.tasksDone === 2) {
        Gash.writeLine(<Line>The <WorldObjectComponent worldObject={'painting'} /> definitely changed. Didn't it?</Line>);
      } else if (Game.tasksDone === 3) {
        Gash.writeLine(<Line>I am drawn to the <WorldObjectComponent worldObject={'painting'} />. It feels like it's changing right in front of my eyes.</Line>);
      } else if (Game.tasksDone === 4) {
        Gash.writeLine(<Line>I am now positively afraid to look at the <WorldObjectComponent worldObject={'painting'} />.</Line>);
      }
    }
  }
}

export const painting = new Painting();