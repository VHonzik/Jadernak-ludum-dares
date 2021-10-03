import Gash, { AutoCompleteResult, CommandAutoCompleter, CommandParser, ICommand, Line, ParsingResult } from "web-gash";
import { AssertDefined } from "../Asserts";
import { Game } from "../Game/Game";

class ExploreCommand implements ICommand {
  name: string = 'explore';

  public exec() {
    const currentArea = Game.findArea(Game.currentArea);
    AssertDefined(currentArea);
    if (currentArea !== undefined) {
      currentArea.explore();
    }
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

export const exploreCommand = new ExploreCommand();

Gash.registerCommand(exploreCommand);