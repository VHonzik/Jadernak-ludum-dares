import Gash, { Line } from "web-gash";
import { AreaComponent } from "../../Components/AreaComponent";
import { WorldObjectComponent } from "../../Components/WorldObjectComponent";
import { IArea } from "../../Entities/IArea";
import { Game } from "../../Game/Game";

class Study implements IArea {
  name: string = 'study';
  explore(): void {
    const landing = Game.tasksDone > 3 ? <AreaComponent area={'hallway'} /> : <AreaComponent area={'landing'} />;
    Gash.writeLine(<Line>I am in a spacious room with a bed, a <WorldObjectComponent worldObject={'night stand'} /> and a small table with a large device that must be the <WorldObjectComponent worldObject={'resonance collider'} />. I can go to {landing}.</Line>);
  }
  getConnectedAreas(): string[] {
    return [Game.tasksDone > 3 ? 'hallway' : 'landing'];
  }
  getObjects(): string[] {
    return ['night stand', 'resonance collider'];
  }
  entered(): void {
    Gash.writeLine(<Line>I have entered <AreaComponent area={this} />.</Line>);
  }
}

export const study = new Study();