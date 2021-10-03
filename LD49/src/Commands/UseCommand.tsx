import Gash, { AutoCompleteResult, AutoCompleteTextParam, CommandAutoCompleter, CommandColored, CommandParser, ICommand, Line, ParsingFailureReason, ParsingResult, RadioDataInput, TextParameter } from "web-gash";
import { AssertDefined } from "../Asserts";
import { ItemComponent } from "../Components/ItemComponent";
import { WorldObjectComponent } from "../Components/WorldObjectComponent";
import { IWorldObject } from "../Entities/IWorldObject";
import { Game } from "../Game/Game";

class UseCommand implements ICommand {
  name: string = 'use';

  public async exec(itemName: string) {
    const inventoryItems = Game.inventory;
    if (inventoryItems.indexOf(itemName) === -1) {
      this.failureMissingItem(itemName);
      return;
    }

    const item = Game.findItem(itemName);
    AssertDefined(item);
    if (item === undefined) {
      Gash.writeLine(<Line />);
      return;
    }

    const currentArea = Game.findArea(Game.currentArea);
    AssertDefined(currentArea);
    if (currentArea === undefined) {
      Gash.writeLine(<Line />);
      return;
    }

    const worldObjectNames = currentArea.getObjects();
    const worldObjects: IWorldObject[] = [];
    const worldObjectComponents: JSX.Element[] = [];

    for (let i = 0; i < worldObjectNames.length; i++) {
      const worldObjectName = worldObjectNames[i];
      const worldObject = Game.findWorldObject(worldObjectName);
      AssertDefined(worldObject);
      if (worldObject !== undefined) {
        worldObjects.push(worldObject);
        worldObjectComponents.push(<WorldObjectComponent worldObject={worldObject}/>);
      }
    }

    if (worldObjects.length === 0) {
      this.failureMissingTarget(itemName);
    }

    let pickedWorldObject: IWorldObject;
    try {
      pickedWorldObject = await RadioDataInput<IWorldObject>(worldObjects, worldObjectComponents, <Line>Pick an object to use <ItemComponent item={item} /> on:</Line>);
    } catch {
      Gash.writeLine(<Line />);
      return;
    }

    item.usedOnObject(pickedWorldObject.name);
    Gash.writeLine(<Line />);
  }

  private failureMissingTarget(itemName:string) {
    Gash.writeLine(<Line systemMessage>There is no valid object to use the <ItemComponent item={itemName} /> on. Try to <CommandColored>explore</CommandColored> your surrounding.</Line>);
    Gash.writeLine(<Line />);
  }

  private failureMissingItem(itemName: string) {
    Gash.writeLine(<Line systemMessage>There is no '{itemName}' item in my inventory to use. Try to <CommandColored>pickup</CommandColored> some items.</Line>);
    Gash.writeLine(<Line />);
  }

  private failureMissingParam() {
    Gash.writeLine(<Line systemMessage><CommandColored>use</CommandColored> command expects an item in my inventory as a parameter. Try to <CommandColored>pickup</CommandColored> some items.</Line>);
    Gash.writeLine(<Line />);
  }

  parse(line: string): ParsingResult {
    const result = CommandParser(this, TextParameter()).parse(line);
    if (result.success) {
      this.exec(result.params[0]);
    } else if (result.failureReason === ParsingFailureReason.MissingParam) {
      result.success = true;
      this.failureMissingParam();
    }
    return result;
  }

  autocomplete(line: string): AutoCompleteResult {
    let items: string[] = [];
    const currentArea = Game.findArea(Game.currentArea);
    if (currentArea !== undefined) {
      items = Game.inventory;
    }
    if (items.length > 0) {
      return CommandAutoCompleter(this, AutoCompleteTextParam(items)).autocomplete(line);
    } else {
      return CommandAutoCompleter(this).autocomplete(line);
    }
  }
  printManPage(): void {}
  available(): boolean { return true; }
}

export const useCommand = new UseCommand();

Gash.registerCommand(useCommand);