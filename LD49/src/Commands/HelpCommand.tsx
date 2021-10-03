import Gash, { AutoCompleteResult, black900, Colored, CommandAutoCompleter, CommandColored, CommandParser, ICommand, Line, ParsingResult } from "web-gash";
import { CArea, CItem, CWorldObject } from "../Content/Constants";

class HelpCommand implements ICommand {
  name: string = 'help';

  public exec() {
    Gash.writeLine(<Line systemMessage>"Girl in a red car" is an old-school adventure game played in a terminal.</Line>);
    Gash.writeLine(<Line systemMessage>You interact with the world by typing a command, sometimes with parameters, and hitting Enter.</Line>);
    Gash.writeLine(<Line systemMessage>You can also use Tab for smart auto-complete and up/down arrows for command history.</Line>);
    Gash.writeLine(<Line systemMessage>Following commands are at your disposal:</Line>);
    Gash.writeLine(<Line systemMessage tabs={1}><CommandColored>explore</CommandColored>, <CommandColored>inspect</CommandColored> (<Colored background={CWorldObject} foreground={black900}>object</Colored> or <Colored background={CItem} foreground={black900}>item</Colored>), <CommandColored>inventory</CommandColored>, <CommandColored>go</CommandColored> (<Colored background={CArea} foreground={black900}>location</Colored>), <CommandColored>pickup</CommandColored> (<Colored background={CItem} foreground={black900}>item</Colored>) and <CommandColored>use</CommandColored> (<Colored background={CItem} foreground={black900}>item</Colored>)</Line>);
    Gash.writeLine(<Line systemMessage>You can always display this message again by typing <CommandColored>help</CommandColored> command.</Line>);
    Gash.writeLine(<Line systemMessage>Note that your progress is not saved so don't refresh or close this web page unless you are done.</Line>);
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

export const helpCommand = new HelpCommand();

Gash.registerCommand(helpCommand);