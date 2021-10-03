import Gash, { Line } from "web-gash";
import { AreaComponent } from "../../Components/AreaComponent";
import { ItemComponent } from "../../Components/ItemComponent";
import { WorldObjectComponent } from "../../Components/WorldObjectComponent";
import { IArea } from "../../Entities/IArea";
import { Game } from "../../Game/Game";
import { chestOfDrawers } from "../Objects/ChesOfDrawers";
import { painting } from "../Objects/Painting";

class Hallway implements IArea {
  name: string = 'hallway';
  visitedAfterChestChange = false;

  visitedAfterStudy = false;

  explore(): void {
    Gash.writeLine(<Line>I am in an entrance hallway of the house on Schmidt Street, 32.</Line>);
    if (Game.tasksDone > 0) {
      Gash.writeLine(<Line>There is a coat hanger, shoes and a <WorldObjectComponent worldObject={'painting'} />.</Line>);
    } else {
      let toolbox: React.ReactNode = <span> with my <ItemComponent item={'tool box'}/> on it</span>;
      if (chestOfDrawers.toolboxPickedUp) {
        toolbox = null;
      }
      Gash.writeLine(<Line>There is a coat hanger, shoes, a <WorldObjectComponent worldObject={'painting'} /> and a <WorldObjectComponent worldObject={'chest of drawers'} />{toolbox}.</Line>);
    }
    const kitchen = Game.tasksDone > 3 ? <AreaComponent area={'living room'} /> : <AreaComponent area={'study'} />;
    Gash.writeLine(<Line> I can go to the <AreaComponent area={'kitchen'} />, to the {kitchen} or to the <AreaComponent area={'landing'} />.</Line>);
  }
  getConnectedAreas(): string[] {
    const areas = ['kitchen', 'landing'];
    if (Game.tasksDone > 3) {
      areas.push('study');
    } else {
      areas.push('living room');
    }
    return areas;
  }
  getObjects(): string[] {
    if (Game.tasksDone > 0) {
      return ['painting'];
    } else {
      return ['chest of drawers', 'painting'];
    }
  }
  entered(): void {
    let extra: React.ReactNode = null;
    if (!this.visitedAfterChestChange && Game.tasksDone > 0) {
      this.visitedAfterChestChange = true;
      extra = <span> I suddenly get a feeling like something is missing here. But what?</span>;
    }
    Gash.writeLine(<Line>I have entered <AreaComponent area={this} />.{extra}</Line>);
    if (!this.visitedAfterStudy && Game.tasksDone > 3) {
      this.visitedAfterStudy = true;
      Gash.writeLine(<Line>As I descend the stairs, I get uneasy feeling. Where there was a wall, there is now door to <AreaComponent area={'study'} />. What is happening?</Line>);
    }
    painting.checkLastSeen();
  }
}

export const hallway = new Hallway();