var Game = function() {
  // Goals
  this.planksPerDelivery = 3;
  this.planksRequired = 300;
  this.beamsPerDelivery = 1;
  this.beamsRequired = 100;
  
  // Hammer tent
  this.planksSecondsPerSpawn = 30.0;
  
  // Weld tent
  this.beamsSecondsPerSpawn = 30.0;
}; 

var assetsObj = {
    "sprites": {
        "art/weld.png": {
            "tile": 80,
            "tileh": 80,
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
        "art/tent.png": {
            "tile": 115,
            "tileh": 75,
            "map": { "tent_sprite": [0,0] }
        },
        "art/human_planks.png": {
            "tile": 54,
            "tileh": 40,
            "map": { "human_planks_sprite": [0,0] }
        },
        "art/human_madman.png": {
            "tile": 54,
            "tileh": 40,
            "map": { "human_madman_sprite": [0,0] }
        },
        "art/human_beams.png": {
            "tile": 54,
            "tileh": 40,
            "map": { "human_beams_sprite": [0,0] }
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
  
  this.emptyTents = [];
  this.slapTents = [];
  this.weldTents = [];
  this.hammerTents = [];
  
  //this.walkingLines = [y1Row+115+10,y2Row+scale2Row*115+10,y3Row+scale3Row*115+10]
  
  var x1RowStart = 20, x1Ofset = 150, y1Row=530;
  var x2RowStart = x1RowStart + 75, y2Row = y1Row - 80, scale2Row = 0.7, x2Offset=160;
  var x3RowStart = x1RowStart + 30, y3Row = y1Row - 150, scale3Row = 0.5, x3Offset=150;  
  
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x1RowStart+ 0*x1Ofset,y:y1Row,w:115,h:75}).appear());
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x1RowStart+ 1*x1Ofset,y:y1Row,w:115,h:75}).appear());
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x1RowStart+ 2*x1Ofset,y:y1Row,w:115,h:75}).appear());
  // this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x1RowStart+ 3*x1Ofset,y:y1Row,w:115,h:75}));
  
  // this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x2RowStart + 0*x2Offset,y:y2Row,w:Math.round(scale2Row*115),h:Math.round(scale2Row*75)}));
  // this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x2RowStart + 1*x2Offset,y:y2Row,w:Math.round(scale2Row*115),h:Math.round(scale2Row*75)}));
  // this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x2RowStart + 2*x2Offset,y:y2Row,w:Math.round(scale2Row*115),h:Math.round(scale2Row*75)}));
  
  // this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x3RowStart+ 0*x3Offset,y:y3Row,w:Math.round(scale3Row*115),h:Math.round(scale3Row*75)}));
  // this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x3RowStart+ 1*x3Offset,y:y3Row,w:Math.round(scale3Row*115),h:Math.round(scale3Row*75)}));
  // this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x3RowStart+ 2*x3Offset,y:y3Row,w:Math.round(scale3Row*115),h:Math.round(scale3Row*75)}));
  // this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x3RowStart+ 3*x3Offset,y:y3Row,w:Math.round(scale3Row*115),h:Math.round(scale3Row*75)}));
  
  Crafty.addEvent(this, Crafty.stage.elem, "click", function(e) {
		if(e.target === Crafty.stage.elem)
			Crafty.trigger('NothingClicked'); 
  });
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