var Game = function() {

}; 

// Initialize scene and game logic
Game.prototype.init = function() { 
  Crafty.background('white');
}

// Load game assets
Game.prototype.loadAssets = function() {

}

$( window ).load(function() {
  GLOBAL.game = new Game();
  GLOBAL.game.loadAssets();
  
  Crafty.init(GLOBAL.gameResolution.w, GLOBAL.gameResolution.h, 'game-stage');
  GLOBAL.game.init();
  
});