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
    var pos = Math.min(1.0*this.planksActual/GLOBAL.game.planksRequired,0.99);
    this.reelPosition(pos);
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
    var pos = Math.min(1.0*this.beamsActual/GLOBAL.game.beamsRequired,0.99);
    this.reelPosition(pos);
  }
});

Crafty.c('Meteor', {
  init: function() {  
		this.requires('2D, DOM, jacAnimation').attr({x:370,y:-70,w:600,h:350})
    .initFromAnimSheet('meteor_sprite',95,1)
    .jacReelSheet([{animationName:'diverted',frameCount:3*24},{animationName:'hit',frameCount:95-(3*24)}]);
	}
});

Crafty.c('Effects', {
  init: function() {
    this.fading = false;
    this.fadeDuration = 0;
    this.fadeTimer = 0;
    
    this.fadeCallback = null;
    
    this.shake = false;    
    this.shakeFrequency = 0.01;
    this.shakeTimer = 0;
    this.shakeMagnitude = 40;
    this.shakeDirection = {x:0,y:0};
    
    this.requires('2D, DOM, Color').attr({x:-50,y:-50,w:0,h:0,z:1}).color("#FFFFFF",0.0);
    
    var that = this;
    Crafty.bind('EnterFrame',function(data) { that.update(data.dt/1000.0); });    
    
  },
  update: function(dt) {
    if(this.fading)
    {
      this.fadeTimer += dt/this.fadeDuration;
      this.color("#FFFFFF",this.fadeTimer/this.fadeDuration);
      if(this.fadeCallback !== null && this.fadeTimer/this.fadeDuration >= 1.0) {
        this.fadeCallback();
        this.fadeCallback = null;
        this.fading = false;
      }
    }
    if(this.shake) {
      this.shakeTimer += dt;
      if(this.shakeTimer>this.shakeFrequency) {
        this.shakeDirection = {x:Math.random()*2.0 - 1.0,y:Math.random()*2.0 - 1.0};
        this.shakeTimer = 0;        
      }
      Crafty.viewport.x += this.shakeDirection.x * dt * this.shakeMagnitude;
      Crafty.viewport.y += this.shakeDirection.y * dt * this.shakeMagnitude;
    }
  },
  fadeIn: function(duration, callback) {
    if(!this.fading) {
      this.fading = true;
      this.fadeDuration = duration;
      this.fadeTimer = 0;
      this.attr({w:GLOBAL.gameResolution.w+50,h:GLOBAL.gameResolution.h+50,z:100});
      this.fadeCallback = callback;
    }
  },
  shakeScreen: function() {
    this.shake = true;
  },
  setShakeParams: function(params) {
    if(typeof params.magnitude !== 'undefined') 
      this.shakeMagnitude = params.magnitude;
    if(typeof params.frequency !== 'undefined') 
      this.shakeFrequency = params.frequency;
  }
});

Crafty.c('MusicPlayer', {
  init: function() {
    this.soundOn = true;
    this.requires('2D, DOM, sound_toggle_sprite, Mouse').attr({x:928,y:610,w:28,h:26});
    this.bind('Click',$.proxy(this.toggleSound,this));
    
    this.currentMusic = -1;
    this.music = []
  },
  toggleSound: function() {
    this.soundOn =  !this.soundOn;
    this.sprite(this.soundOn ? 1 : 0,0);
  },
  play: function(sound) {
    if(this.soundOn) {
      Crafty.audio.play(sound);
    }
  }
});