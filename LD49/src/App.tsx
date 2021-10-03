import './App.css';
import Gash, { BlockTitle, Line, Terminal } from 'web-gash';
import { helpCommand } from './Commands/HelpCommand';
import './Commands/CombineCommand';
import './Commands/ExploreCommand';
import './Commands/GoCommand';
import './Commands/InspectCommand';
import './Commands/InventoryCommand';
import './Commands/PickUpCommand';
import './Commands/UseCommand';
import { Game } from './Game/Game';

Gash.onTerminalMounted(() => {
  Gash.writeLine(<BlockTitle primaryColor="red">GIRL IN A RED CAR</BlockTitle>);
  Gash.writeLine(<Line>It is Friday afternoon and I have one last property to visit. Schmidt Street, 32.</Line>);
  Gash.writeLine(<Line>There is a work order on a few small chores and repairs that I have scribbled down.</Line>);
  Gash.writeLine(<Line>As I enter the house, I realize I haven’t taken my medication today. Hmm.</Line>);
  Gash.writeLine(<Line>I can’t be bothered to go back to the car. I should be fine.</Line>);
  Gash.writeLine(<Line />);
  helpCommand.exec();
  Gash.writeLine(<Line />);
});

Game.init();

function App() {
  return (
    <Terminal />
  );
}

export default App;
