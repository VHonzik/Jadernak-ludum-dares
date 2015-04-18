Crafty.c('Building',{
	init: function() {
    this.planks = 0;
		this.requires('2D, DOM, building_sprite').attr({x:665,y:208,w:214,h:404});
	}
});

Crafty.c('RampScaffolding', {
  init: function() {  
    this.planksActual = 0;
    this.planksStored = 0;
		this.requires('2D, DOM, jacAnimation').attr({x:683,y:49,w:197,h:164})
    .initFromAnimSheet('ramp_scaff_sprite',20,5).jacReelSheet([{animationName:'construct',frameCount:5*20}])
    .animate('construct', -1).pauseAnimation().reelPosition(0.00);
    
    var that = this;
    Crafty.bind('EnterFrame',function(data) { that.update(data.dt/1000.0); });  
	},
  planksDelivered: function() {
        this.planksStored += GLOBAL.game.planksPerDelivery;       
  },
  update: function(dt) {
    this.planksActual = Math.min(this.planksActual+1,this.planksStored);
    this.reelPosition(1.0*this.planksActual/GLOBAL.game.planksRequired);
  }
});

Crafty.c('Ramp', {
  init: function() {  
    this.beamsActual = 0;
    this.beamsStored = 0;
		this.requires('2D, DOM, jacAnimation').attr({x:669,y:1,w:246,h:216})
    .initFromAnimSheet('ramp_sprite',25,4).jacReelSheet([{animationName:'construct',frameCount:4*25}])
    .animate('construct', -1).pauseAnimation().reelPosition(0.00);
    
    var that = this;
    Crafty.bind('EnterFrame',function(data) { that.update(data.dt/1000.0); });  
	},
  beamsDelivered: function() {
    this.beamsStored += GLOBAL.game.beamsPerDelivery;       
  },
  update: function(dt) {
    var wanted = Math.min(this.beamsActual+1,this.beamsStored);
    // Never construct more ramp than scaffolding
    if(wanted/GLOBAL.game.beamsRequired<=GLOBAL.game.rampScaff.planksActual/GLOBAL.game.planksRequired)
    {
      this.beamsActual = wanted;
    }
    this.reelPosition(1.0*this.beamsActual/GLOBAL.game.beamsRequired);
  }
});

Crafty.c('Meteor', {
  init: function() {  
		this.requires('2D, DOM, jacAnimation').attr({x:370,y:-70,w:600,h:350})
    .initFromAnimSheet('meteor_sprite',95,1)
    .jacReelSheet([{animationName:'diverted',frameCount:3*24},{animationName:'hit',frameCount:95-(3*24)}]);
	}
});