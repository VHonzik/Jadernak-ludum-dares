import Gash, { AutoCompleteResult, CommandAutoCompleter, CommandColored, CommandParser, ICommand, Line, ListDataInput, ParsingResult } from "web-gash";
import { AssertDefined } from "../Asserts";
import { ItemComponent } from "../Components/ItemComponent";
import { IItem } from "../Entities/IItem";
import { Game } from "../Game/Game";

class CombineCommand implements ICommand {
  name: string = 'combine';

  public async exec() {
    const inventoryItemNames = Game.inventory;
    if (inventoryItemNames.length <= 1) {
      this.failureNoItemsInInventory();
      return;
    }

    const inventoryItems: IItem[] = [];

    for (let i = 0; i < inventoryItemNames.length; i++) {
      const itemName = inventoryItemNames[i];
      const item = Game.findItem(itemName);
      AssertDefined(item);
      if (item !== undefined) {
        inventoryItems.push(item);
      }
    }

    const itemsComponents = inventoryItems.map(item => {
      return <ItemComponent item={item} />;
    });

    let pickedItems: IItem[] = [];
    try {
      pickedItems = await ListDataInput<IItem>(inventoryItems, itemsComponents, 2, <Line>Pick two items to combine:</Line>);
    } catch {
      Gash.writeLine(<Line />);
      return;
    }

    pickedItems[0].combinedWith(pickedItems[1].name);
    Gash.writeLine(<Line />);
  }

  private failureNoItemsInInventory() {
    Gash.writeLine(<Line systemMessage>You need at least two items in your inventory. Try to <CommandColored>pickup</CommandColored> some items.</Line>);
    Gash.writeLine(<Line />);
  }

  parse(line: string): ParsingResult {
    const result = CommandParser(this).parse(line);
    if (result.success) {
      this.exec();
    }
    return result;
  }
  autocomplete(line: string): AutoCompleteResult {
    return CommandAutoCompleter(this).autocomplete(line);
  }
  printManPage(): void {}
  available(): boolean { return true; }
}

export const combineCommand = new CombineCommand();

Gash.registerCommand(combineCommand);