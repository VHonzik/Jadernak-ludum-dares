import Gash, { Line } from "web-gash";
import { AreaComponent } from "../../Components/AreaComponent";
import { WorldObjectComponent } from "../../Components/WorldObjectComponent";
import { IArea } from "../../Entities/IArea";
import { Game } from "../../Game/Game";
import { hatch } from "../Objects/Hatch";
import { painting } from "../Objects/Painting";

class Landing implements IArea {
  visitedBeforePainting = false;
  visitedAfterPainting = false;

  visitedBeforeStudy = false;
  visitedAfterStudy = false;

  name: string = 'landing';
  explore(): void {
    Gash.writeLine(<Line> I am on the second story landing which opens up to various rooms.</Line>);
    if (Game.tasksDone > 1) {
      Gash.writeLine(<Line>There is a <WorldObjectComponent worldObject={'hatch'} /> on the ceiling, a half open <WorldObjectComponent worldObject={'door'} />, a <WorldObjectComponent worldObject={'painting'} /> and a <WorldObjectComponent worldObject={'secured pole'} /> on the wall.</Line>);
    } else {
      Gash.writeLine(<Line>There is a <WorldObjectComponent worldObject={'hatch'} /> on the ceiling, a half open <WorldObjectComponent worldObject={'door'} /> and a <WorldObjectComponent worldObject={'secured pole'} /> on the wall.</Line>);
    }
    const study = Game.tasksDone > 3 ? <AreaComponent area={'living room'} /> : <AreaComponent area={'study'} />;
    Gash.writeLine(<Line>I can go to the {study} and to the <AreaComponent area={'hallway'} />.</Line>);
    if (hatch.opened) {
      Gash.writeLine(<Line>With the hatch opened I can also go to the <AreaComponent area={'attic'} />.</Line>);
    }
  }
  getConnectedAreas(): string[] {
    const areas = ['hallway'];
    if (hatch.opened) {
      areas.push('attic');
    }
    if (Game.tasksDone > 3) {
      areas.push('living room');
    } else {
      areas.push('study');
    }
    return areas;
  }
  getObjects(): string[] {
    if (Game.tasksDone > 1) {
      return ['hatch', 'door', 'secured pole', 'painting'];
    } else {
      return ['hatch', 'door', 'secured pole'];
    }
  }
  entered(): void {
    if (Game.tasksDone <= 1 && !this.visitedBeforePainting) {
      this.visitedBeforePainting = true;
    }
    if (Game.tasksDone <= 3 && !this.visitedBeforeStudy) {
      this.visitedBeforeStudy = true;
    }

    let extra: React.ReactNode = null;
    if (!this.visitedAfterPainting && Game.tasksDone > 1 && this.visitedBeforePainting) {
      this.visitedAfterPainting = true;
      extra = <span> The <WorldObjectComponent worldObject={'painting'} /> on the wall... I would swear it wasn't here. Is it still downstairs?</span>;
    }
    Gash.writeLine(<Line>I have entered <AreaComponent area={this} />.{extra}</Line>);
    if (!this.visitedAfterStudy && Game.tasksDone > 3 && this.visitedBeforeStudy) {
      this.visitedAfterStudy = true;
      Gash.writeLine(<Line>As I ascend the stairs I get uneasy feeling. Where there was a wall, there is door to <AreaComponent area={'living room'} />. What is happening?</Line>);
    }
    if (Game.tasksDone > 1) {
      painting.checkLastSeen();
    }
  }
}

export const landing = new Landing();