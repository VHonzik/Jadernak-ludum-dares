import Gash, { AutoCompleteResult, AutoCompleteTextParam, CommandAutoCompleter, CommandColored, CommandParser, ICommand, Line, ParsingFailureReason, ParsingResult, TextParameter } from "web-gash";
import { AssertDefined } from "../Asserts";
import { Game } from "../Game/Game";

class GoCommand implements ICommand {
  name: string = 'go';

  public exec(wantedAreaName: string) {
    const currentArea = Game.findArea(Game.currentArea);
    AssertDefined(currentArea);
    if (currentArea !== undefined) {
      const areaNames = currentArea.getConnectedAreas();
      for (let i = 0; i < areaNames.length; i++) {
        if (areaNames[i] === wantedAreaName) {
          const area = Game.findArea(areaNames[i]);
          AssertDefined(area);
          if (area !== undefined) {
            Game.goToArea(wantedAreaName);
            area.entered();
            Gash.writeLine(<Line />);
            return;
          }
        }
      }

      this.failureInvalidArea(wantedAreaName);
    }
    Gash.writeLine(<Line />);
  }

  private failureInvalidArea(areaName: string) {
    Gash.writeLine(<Line systemMessage>'{areaName}' is not a valid destination to go. Try <CommandColored>explore</CommandColored>-ing around.</Line>);
    Gash.writeLine(<Line />);
  }

  private failureMissingParam() {
    Gash.writeLine(<Line systemMessage><CommandColored>go</CommandColored> command expects an area as a parameter. Try <CommandColored>explore</CommandColored>-ing around.</Line>);
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
    let areas: string[] = [];
    const currentArea = Game.findArea(Game.currentArea);
    if (currentArea !== undefined) {
      const areaNames = currentArea.getConnectedAreas();
      for (let i = 0; i < areaNames.length; i++) {
        const area = Game.findArea(areaNames[i]);
        AssertDefined(area);
        if (area !== undefined) {
          areas.push(area.name);
        }
      }
    }
    if (areas.length > 0) {
      return CommandAutoCompleter(this, AutoCompleteTextParam(areas)).autocomplete(line);
    } else {
      return CommandAutoCompleter(this).autocomplete(line);
    }
  }

  printManPage(): void {}
  available(): boolean { return true; }
}

export const goCommand = new GoCommand();

Gash.registerCommand(goCommand);