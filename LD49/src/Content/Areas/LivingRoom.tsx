import Gash, { Line } from "web-gash";
import { AreaComponent } from "../../Components/AreaComponent";
import { WorldObjectComponent } from "../../Components/WorldObjectComponent";
import { IArea } from "../../Entities/IArea";
import { Game } from "../../Game/Game";

class LivingRoom implements IArea {
  name: string = 'living room';
  visitedBeforeChestChange = false;
  visitedAfterChestChange = false;

  explore(): void {
    const hallway = Game.tasksDone > 3 ? <AreaComponent area={'landing'} /> : <AreaComponent area={'hallway'} />;
    if (Game.tasksDone > 0) {
      Gash.writeLine(<Line>I am in the living room with all the necessities such as a sofa, an armchair, a reading <WorldObjectComponent worldObject={'lamp'} />, a <WorldObjectComponent worldObject={'chest of drawers'} /> and bookshelves. I can go to the {hallway}.</Line>);
    } else {
      Gash.writeLine(<Line>I am in the living room with all the necessities such as a sofa, an armchair, a reading <WorldObjectComponent worldObject={'lamp'} /> and bookshelves. I can go to the {hallway}.</Line>);
    }

  }
  getConnectedAreas(): string[] {
    return [Game.tasksDone > 3 ? 'landing' : 'hallway'];
  }
  getObjects(): string[] {
    if (Game.tasksDone > 0) {
      return ['lamp', 'chest of drawers'];
    } else {
      return ['lamp'];
    }
  }
  entered(): void {
    if (Game.tasksDone === 0 && !this.visitedBeforeChestChange) {
      this.visitedBeforeChestChange = true;
    }
    let extra: React.ReactNode = null;
    if (!this.visitedAfterChestChange && Game.tasksDone > 0 && this.visitedBeforeChestChange) {
      this.visitedAfterChestChange = true;
      extra = <span> Was the <WorldObjectComponent worldObject={'chest of drawers'} /> always there? Strange.</span>;
    }
    Gash.writeLine(<Line>I have entered <AreaComponent area={this} />.{extra}</Line>);
  }
}

export const livingRoom = new LivingRoom();