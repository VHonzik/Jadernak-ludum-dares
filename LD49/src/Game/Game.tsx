import Gash, { Line } from "web-gash";
import { AssertDefined, AssertTrue } from "../Asserts";
import { attic } from "../Content/Areas/Attic";
import { hallway } from "../Content/Areas/Hallway";
import { kitchen } from "../Content/Areas/Kitchen";
import { landing } from "../Content/Areas/Landing";
import { livingRoom } from "../Content/Areas/LivingRoom";
import { study } from "../Content/Areas/Study";
import { banana } from "../Content/Items/Banana";
import { flashlight } from "../Content/Items/Flashlight";
import { hookPole } from "../Content/Items/HookPole";
import { lightBulb } from "../Content/Items/LightBulb";
import { listOfTasks } from "../Content/Items/ListOfTasks";
import { simpleInstructions } from "../Content/Items/SimpleInstructions";
import { toolBox } from "../Content/Items/ToolBox";
import { trashBagRoll } from "../Content/Items/TrashBagRoll";
import { boxes } from "../Content/Objects/Boxes";
import { cabinet } from "../Content/Objects/Cabinet";
import { chestOfDrawers } from "../Content/Objects/ChesOfDrawers";
import { door } from "../Content/Objects/Door";
import { fruitBowl } from "../Content/Objects/FruitBowl";
import { hatch } from "../Content/Objects/Hatch";
import { lamp } from "../Content/Objects/Lamp";
import { nightStand } from "../Content/Objects/NightStand";
import { painting } from "../Content/Objects/Painting";
import { resonanceCollider } from "../Content/Objects/ResonanceCollider";
import { roofWindow } from "../Content/Objects/RoofWindow";
import { securedPole } from "../Content/Objects/SecuredPole";
import { trashBin } from "../Content/Objects/TrashBin";
import { IArea } from "../Entities/IArea";
import { IItem } from "../Entities/IItem";
import { IWorldObject } from "../Entities/IWorldObject";

class GameManager {
  private Areas: Record<string, IArea> = {
    [attic.name]: attic,
    [hallway.name]: hallway,
    [kitchen.name]: kitchen,
    [landing.name]: landing,
    [livingRoom.name]: livingRoom,
    [study.name]: study,
  };
  private WorldObjects: Record<string, IWorldObject> = {
    [boxes.name]: boxes,
    [cabinet.name]: cabinet,
    [chestOfDrawers.name]: chestOfDrawers,
    [door.name]: door,
    [fruitBowl.name]: fruitBowl,
    [hatch.name]: hatch,
    [lamp.name]: lamp,
    [nightStand.name]: nightStand,
    [painting.name]: painting,
    [resonanceCollider.name]: resonanceCollider,
    [roofWindow.name]: roofWindow,
    [securedPole.name]: securedPole,
    [trashBin.name]: trashBin,
  };
  private Items: Record<string, IItem> = {
    [banana.name]: banana,
    [flashlight.name]: flashlight,
    [hookPole.name]: hookPole,
    [lightBulb.name]: lightBulb,
    [listOfTasks.name]: listOfTasks,
    [simpleInstructions.name]: simpleInstructions,
    [toolBox.name]: toolBox,
    [trashBagRoll.name]: trashBagRoll,
  };

  public currentArea: string = hallway.name;
  public inventory: string[] = [ listOfTasks.name ];
  public tasksDone: number = 0;

  public init() {
  }

  public pickUpItem(itemName: string) {
    this.inventory.push(itemName);
  }

  public looseItem(itemName: string) {
    for (let i = this.inventory.length - 1; i >= 0; i--) {
      const inventoryItemName = this.inventory[i];
      if (inventoryItemName === itemName) {
        this.inventory.splice(i, 1);
      }
    }
  }

  public removeItem(itemName: string) {
    this.looseItem(itemName);
    delete this.Items[itemName];
  }

  public goToArea(areaName: string) {
    AssertDefined(this.findArea(areaName));
    this.currentArea = areaName;
  }

  public addArea(area: IArea) {
    this.Areas[area.name] = area;
  }

  public addItem(item: IItem) {
    this.Items[item.name] = item;
  }

  public findArea(name: string): IArea | undefined {
    return this.findRecordValue(name, this.Areas);
  }

  public findWorldObject(name: string): IWorldObject | undefined {
    return this.findRecordValue(name, this.WorldObjects);
  }

  public findItem(name: string): IItem | undefined {
    return this.findRecordValue(name, this.Items);
  }

  public taskDone(taskName: string) {
    switch (taskName) {
      case 'collider': listOfTasks.colliderDone = true;  break;
      case 'window': listOfTasks.windowDone = true;  break;
      case 'lamp': listOfTasks.lampDone = true;  break;
      case 'door': listOfTasks.doorDone = true;  break;
      case 'trash': listOfTasks.trashDone = true;  break;
      default:
        AssertTrue(false, 'Unknown task done');
        break;
    }
    this.tasksDone += 1;
    if (this.tasksDone === 3) {
      Gash.writeLine(<Line>I... I feel little dizzy. I think I spaced out. Where am I?</Line>);
      this.currentArea = kitchen.name;
    }
    if (this.tasksDone === 5) {
      this.endGame();
    }
  }

  public endGame() {
    Gash.writeLine(<Line>All tasks done, I am ready to leave. My head hurts and I don't feel well.</Line>);
    Gash.writeLine(<Line>I return to the hallway and on the way I stumble, almost falling.</Line>);
    Gash.writeLine(<Line>The room gets brighter. I look around.</Line>);
    Gash.writeLine(<Line>I am blinded by headlights that are coming from the painting.</Line>);
    Gash.writeLine(<Line>I hear tires screeching. I can’t look away.</Line>);
    Gash.writeLine(<Line>I can’t look away.</Line>);
    Gash.writeLine(<Line>Time is going slow.</Line>);
    Gash.writeLine(<Line>There is a blond girl with a colorful headscarf in the driver seat.</Line>);
    Gash.writeLine(<Line>She looks surprised.</Line>);
    Gash.writeLine(<Line>My last realization is that it is me standing on the road...</Line>);
    Gash.writeLine(<Line />);
    Gash.writeLine(<Line />);

    Gash.writeLine(<Line>Thanks for playing my Ludum Dare 49 compo entry!</Line>);
    Gash.writeLine(<Line>You can rate it here: <a href='https://ldjam.com/events/ludum-dare/49/girl-in-a-red-car' target="_blank">Girl in a red car</a></Line>);
    Gash.writeLine(<Line systemMessage>If you want to play again, you can simply refresh the page.</Line>);
    Gash.writeLine(<Line />);
    Gash.disableInput();
  }

  private findRecordValue<ValueType>(key: string, record: Record<string, ValueType>): ValueType | undefined {
    const names = Object.keys(record);
    for (let i = 0; i < names.length; i++) {
      const objectName = names[i];
      if (objectName === key) {
        return record[key];
      }
    }

    return undefined;
  }
}

export const Game: GameManager = new GameManager();