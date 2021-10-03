import Gash, { AutoCompleteResult, CommandAutoCompleter, CommandParser, ICommand, Line, ParsingResult } from "web-gash";
import { AssertDefined } from "../Asserts";
import { ItemComponent } from "../Components/ItemComponent";
import { Game } from "../Game/Game";

class InventoryCommand implements ICommand {
  name: string = 'inventory';

  public exec() {
    const itemNames = Game.inventory;

    if (itemNames.length === 0) {
      Gash.writeLine(<Line>There are no items in my inventory.</Line>);
      Gash.writeLine(<Line />);
      return;
    }

    const items: React.ReactNode[] = [];

    for (let i = 0; i < itemNames.length; i++) {
      const itemName = itemNames[i];
      const item = Game.findItem(itemName);
      AssertDefined(item);
      if (item !== undefined) {
        if (i !== 0) {
          items.push(', ');
        }
        items.push(<ItemComponent item={item} />);
      }
    }
    Gash.writeLine(<Line>I have following items in my inventory: {items}</Line>);
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

export const inventoryCommand = new InventoryCommand();

Gash.registerCommand(inventoryCommand);