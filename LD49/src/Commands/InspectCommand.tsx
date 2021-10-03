import Gash, { AutoCompleteResult, AutoCompleteTextParam, CommandAutoCompleter, CommandColored, CommandParser, ICommand, Line, ParsingFailureReason, ParsingResult, TextParameter } from "web-gash";
import { AssertDefined } from "../Asserts";
import { Game } from "../Game/Game";

class InspectCommand implements ICommand {
  name: string = 'inspect';

  public exec(objectName: string) {
    let presentObjects: string[] = [];
    const currentArea = Game.findArea(Game.currentArea);
    AssertDefined(currentArea);
    if (currentArea !== undefined) {
      presentObjects = currentArea.getObjects();
      if (presentObjects.indexOf(objectName) === -1) {
        const inventoryItems = Game.inventory;
        if (inventoryItems.indexOf(objectName) === -1) {
          this.failureMissingObject(objectName);
          return;
        } else {
          const item = Game.findItem(objectName);
          AssertDefined(item);
          if (item !== undefined) {
            item.inspect();
          }
        }
      } else {
        const worldObject = Game.findWorldObject(objectName);
        AssertDefined(worldObject);
        if (worldObject !== undefined) {
          worldObject.inspect();
        }
      }
    }
    Gash.writeLine(<Line />);
  }

  private failureMissingObject(obj: string) {
    Gash.writeLine(<Line systemMessage>There is no '{obj}' around or in the inventory to inspect. Try <CommandColored>explore</CommandColored>-ing around.</Line>);
    Gash.writeLine(<Line />);
  }

  private failureMissingParam() {
    Gash.writeLine(<Line systemMessage><CommandColored>inspect</CommandColored> command expects an object in world as a parameter. Try <CommandColored>explore</CommandColored>-ing around.</Line>);
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
    let objects: string[] = [];
    const currentArea = Game.findArea(Game.currentArea);
    if (currentArea !== undefined) {
      objects = currentArea.getObjects();
      objects = [...objects, ...Game.inventory];
    }
    if (objects.length > 0) {
      return CommandAutoCompleter(this, AutoCompleteTextParam(objects)).autocomplete(line);
    } else {
      return CommandAutoCompleter(this).autocomplete(line);
    }
  }
  printManPage(): void {}
  available(): boolean { return true; }
}

export const inspectCommand = new InspectCommand();

Gash.registerCommand(inspectCommand);