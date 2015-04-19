var Game = function() {
  // Goals
  this.planksPerDelivery = 3;
  this.planksRequired = 105;
  this.beamsPerDelivery = 1;
  this.beamsRequired = 28;
  
  //Tents appearing
  this.tentsTiming = [0.0,2.0,4.0,6.0,40.0,42.0,44.0,60.0,62.0,64.0,66.0];
  //this.tentsTiming = [0.0,0.0,0.0,0.0, 0,0,0, 0,0,0,0];
  // Hammer tent
  this.planksSecondsPerSpawn = 30.0;
  
  // Weld tent
  this.beamsSecondsPerSpawn = 30.0;
  
  // Slap tent
  this.slapSecondsPer = 5;
  
  //Madness mechanic
  this.madnessPerSec = 1;
  this.madnessThreshold = 80;
  this.madnessDiminishPerSec = 2;
  this.madnessExhaustionDamage = 20;
  
  //Dificulty
  //this.meteorTimer = 11;
  this.meteorTimer = 7*60;
  
  //Constructor stuff
   this.emptyTents = [];
  this.slapTents = [];
  this.weldTents = [];
  this.hammerTents = [];
  this.humans = [];
  this.hasEnded = false;
  
  this.tentsTimer = 0;
  this.nextToAppearTent = null;
  this.tentsToAppear = [];
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
        "art/tent_upgraded.png": {
            "tile": 115,
            "tileh": 75,
            "map": { "tent_up_sprite": [0,0] }
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
  
  this.effects = Crafty.e('Effects');
  
  var x1RowStart = 20, x1Ofset = 150, y1Row=530;
  var x2RowStart = x1RowStart + 75, y2Row = y1Row - 170, scale2Row = 0.7, x2Offset=150;
  var x3RowStart = x1RowStart, y3Row = y2Row - 170, scale3Row = 0.5, x3Offset=150;  
  
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x1RowStart+ 0*x1Ofset,y:y1Row,w:115,h:75}));
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x1RowStart+ 1*x1Ofset,y:y1Row,w:115,h:75}));
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x1RowStart+ 2*x1Ofset,y:y1Row,w:115,h:75}));
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x1RowStart+ 3*x1Ofset,y:y1Row,w:115,h:75}));
  
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x2RowStart + 0*x2Offset,y:y2Row,w:115,h:75}));
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x2RowStart + 1*x2Offset,y:y2Row,w:115,h:75}));
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x2RowStart + 2*x2Offset,y:y2Row,w:115,h:75}));
  
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x3RowStart+ 0*x3Offset,y:y3Row,w:115,h:75}));
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x3RowStart+ 1*x3Offset,y:y3Row,w:115,h:75}));
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x3RowStart+ 2*x3Offset,y:y3Row,w:115,h:75}));
  this.emptyTents.push(Crafty.e('EmptyTent').attr({x:x3RowStart+ 3*x3Offset,y:y3Row,w:115,h:75}));
  
  this.tentsToAppear = this.emptyTents.slice();
  
  Crafty.addEvent(this, Crafty.stage.elem, "click", function(e) {
		if(e.target === Crafty.stage.elem)
			Crafty.trigger('NothingClicked'); 
  });
  
  var that = this;
  Crafty.bind('EnterFrame',function(data) { that.update(data.dt/1000.0); });    
}

Game.prototype.findFreeSlapTent = function(productionTent) {
  var freeTents = [];
   this.slapTents.forEach(function(st) {
      if(st.canDoSlapping())
        freeTents.push(st);
    });
    
    if(freeTents.length == 0)
      return null;
    
    var smallestDistance = Infinity;
    var smallestIndex = -1;
    for(var i=0; i<freeTents.length; i++)
    {
      var dist = Crafty.math.distance(productionTent._x,productionTent._y,freeTents[i]._x,freeTents[i]._y);
      if(dist<smallestDistance)
      {
        smallestDistance = dist;
        smallestIndex = i;
      }
    }
    
    if(smallestIndex<0)
      return null;
    
    return freeTents[smallestIndex];
}

Game.prototype.appearTents = function(dt) {  
  this.tentsTimer += dt;  
  
  
  if(this.nextToAppearTent === null && this.tentsToAppear.length>0) {
    this.nextToAppearTent = this.tentsToAppear.shift();
  }
   
   if(this.nextToAppearTent != null && this.tentsTimer>this.tentsTiming[0]) {
       this.nextToAppearTent.appear();
       
       this.tentsTiming.shift();
       this.nextToAppearTent = null;
   }
}

Game.prototype.updateEffects = function(dt) {
   if(this.meteorTimer<=10.0) {
    GLOBAL.game.effects.shakeScreen();
    GLOBAL.game.effects.setShakeParams({magnitude:20,frequency:0.01});
  }
  
  if(this.meteorTimer<=5.0) {
    GLOBAL.game.effects.setShakeParams({magnitude:40,frequency:0.005});
  }
  
  if(this.meteorTimer<=-5.0) {
    GLOBAL.game.effects.setShakeParams({magnitude:20,frequency:0.01});
  }
}

Game.prototype.update = function(dt) {
  this.meteorTimer -= dt;
  
  
  this.updateEffects(dt);
  
  this.appearTents(dt);
  
  //End
  if(this.meteorTimer<=0 && !this.hasEnded) {
    this.hasEnded = true;
    this.slapTents.forEach(function(e) {
        e.stop();
    });
    this.weldTents.forEach(function(e) {
        e.stop();
    });
    this.hammerTents.forEach(function(e) {
        e.stop();
    });
    this.humans.forEach(function(e) {
        e.stop();
    });

    if(this.rampScaff.planksActual>=GLOBAL.game.planksRequired && this.ramp.beamsActual>=GLOBAL.game.beamsRequired) {
      this.meteor.animate('diverted');
      setTimeout(function() {
            GLOBAL.game.effects.fadeIn(2);
      }, 4000);
    }
    else
    {
      this.meteor.jacQueueAnimation('hit');
      this.meteor.jacQueueCallback(function() {
        GLOBAL.game.effects.fadeIn(0.1);
      });      
    }
    
  }
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