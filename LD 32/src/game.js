var Game = function() {

}; 

var assetsObj = {
    "sprites": {
        "art/weld.png": {
            "tile": 80,
            "map": { "weld_sprite": [0,0] }
        },
        "art/slap.png": {
            "tile": 113,
            "tileh": 80,
            "map": { "slap_sprite": [0,0] }
        },
        "art/hammer.png": {
            "tile": 94,
            "tileh": 82,
            "map": { "hammer_sprite": [0,0] }
        },
        "art/building.png": {
            "tile": 214,
            "tileh": 404,
            "map": { "building_sprite": [0,0] }
        },
        "art/ramp_scaff.png": {
            "tile": 197,
            "tileh": 164,
            "map": { "ramp_scaff_sprite": [0,0] }
        },
        "art/ramp.png": {
            "tile": 246,
            "tileh": 216,
            "map": { "ramp_sprite": [0,0] }
        },
        "art/meteor.png": {
            "tile": 600,
            "tileh": 350,
            "map": { "meteor_sprite": [0,0] }
        },
        
        
    },
};

// Initialize main scene and game logic
Game.prototype.init = function() { 
  Crafty.background('white');
  this.building = Crafty.e('Building');
  this.rampScaff = Crafty.e('RampScaffolding');
  this.ramp = Crafty.e('Ramp');
  this.meteor = Crafty.e('Meteor');
}

// Load game assets
Game.prototype.loadAssets = function() {
  Crafty.load(assetsObj, 
    function() { //when loaded
        Crafty.scene("main");
    },

    function(e) { //progress
    },

    function(e) { //uh oh, error loading
    }
  );
}

$( window ).load(function() {
  GLOBAL.game = new Game();
    
  Crafty.init(GLOBAL.gameResolution.w, GLOBAL.gameResolution.h, 'game-stage');
  
  Crafty.enterScene("loading");
  
});