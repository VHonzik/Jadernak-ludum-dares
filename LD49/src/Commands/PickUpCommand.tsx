import Gash, { AutoCompleteResult, AutoCompleteTextParam, CommandAutoCompleter, CommandColored, CommandParser, ICommand, Line, ParsingFailureReason, ParsingResult, TextParameter } from "web-gash";
import { AssertDefined } from "../Asserts";
import { Game } from "../Game/Game";

class PickUpCommand implements ICommand {
  name: string = 'pickup';

  public exec(wantedItemName: string) {
    const currentArea = Game.findArea(Game.currentArea);
    AssertDefined(currentArea);
    if (currentArea !== undefined) {
      const objectNames = currentArea.getObjects();
      for (let i = 0; i < objectNames.length; i++) {
        const objectName = objectNames[i];
        const object = Game.findWorldObject(objectName);
        AssertDefined(object);
        if (object !== undefined) {
          const itemNames = object.getItems();
          for (let j = 0; j < itemNames.length; j++) {
            const itemName = itemNames[j];
            if (itemName === wantedItemName) {

              object.itemPickedUp(itemName);
              Game.pickUpItem(itemName);

              const item = Game.findItem(itemName);
              AssertDefined(item);
              if (item !== undefined) {
                item.pickedUp();
              }

              Gash.writeLine(<Line />);
              return;
            }
          }
        }
      }
    }

    this.failureItemNotFound(wantedItemName);
  }

  private failureItemNotFound(wantedItemName: string) {
    Gash.writeLine(<Line systemMessage>There is no '{wantedItemName}' around to pick up. Try to <CommandColored>inspect</CommandColored> world objects.</Line>);
    Gash.writeLine(<Line />);
  }

  private failureMissingParam() {
    Gash.writeLine(<Line systemMessage><CommandColored>pickup</CommandColored> command expects an item as a parameter. Try to <CommandColored>inspect</CommandColored> world objects.</Line>);
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
      const objectNames = currentArea.getObjects();
      for (let i = 0; i < objectNames.length; i++) {
        const objectName = objectNames[i];
        const object = Game.findWorldObject(objectName);
        AssertDefined(object);
        if (object !== undefined) {
          items.push(...object.getItems());
        }
      }
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

export const pickUpCommand = new PickUpCommand();

Gash.registerCommand(pickUpCommand);