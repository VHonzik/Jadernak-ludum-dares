Crafty.c('EmptyTent', {	
  init: function() {
    this.isOn = false;
    this.requires('jac2D, DOM, Mouse, jacAnimation')
      .initFromAnimSheet('tent_sprite',7,8)
      .jacReelSheet([{animationName:'appear',frameCount:25},{animationName:'upgrade',frameCount:25}]);
      
    this.bind('Click',$.proxy(this.mClick,this));
		this.deselectCallback = $.proxy(this.deselect,this);
		this.bind('EmptyTentClicked',this.deselectCallback);
		this.bind('NothingClicked',this.deselectCallback);
    
    this.upgradesIcons = [];
    this.upgradesIcons.push(
			Crafty.e('SlapUPIcon').jacAttr({rtpcX:0,rtpcY:-70,rtpElement:this,rtpFollow:true, w: 56, h: 40}).setParentTent(this)
		);
    this.upgradesIcons.push(
			Crafty.e('HammerUPIcon').jacAttr({rtpcX:45,rtpcY:-40,rtpElement:this,rtpFollow:true, w: 47, h: 41}).setParentTent(this)
		);
    this.upgradesIcons.push(
			Crafty.e('WeldUPIcon').jacAttr({rtpcX:-40,rtpcY:-40,rtpElement:this,rtpFollow:true, w: 40, h: 40}).setParentTent(this)
		);
  },
  appear: function() {
    this.jacQueueAnimation('appear').jacQueueCallback(function() {  this.isOn = true;});
    return this;
  },
  mClick: function() {
		if(this.isOn)
		{
			// Don't trigger "EmptyTentClicked" for this instance
			this.unbind('EmptyTentClicked',this.deselectCallback);
			Crafty.trigger('EmptyTentClicked');
			this.bind('EmptyTentClicked',this.deselectCallback);
			
      if(!this.clicked)
			{
        this.clicked = true;
			
        this.upgradesIcons.forEach(function(ui) {
          ui.appear();
        });
      }
		}
		
	},
	deselect: function() {
		if(this.isOn)
		{
			this.clicked = false;
			
			this.upgradesIcons.forEach(function(ui) {
				ui.disappear();
			});
		}
	},
  upgradeGeneral: function(newTentName,newTentIconName,tentsContainer) {
    Crafty.trigger('EmptyTentClicked');
    this.isOn = false;
    this.jacQueueAnimation('upgrade');
    this.jacQueueCallback(function() {      
      GLOBAL.game.emptyTents.splice( $.inArray(this, GLOBAL.game.emptyTents), 1 );
      var attrs = {x:this._x,y:this._y,w:this._w,h:this._h};
      var newTent = Crafty.e(newTentName);
      newTent.attr(attrs);
      tentsContainer.push(newTent);
      newTent.icon = Crafty.e(newTentIconName).createAboveTent(newTent);      
      this.destroy();
    });    
  },
  upgradeToWeldTent: function() {
    this.upgradeGeneral('WeldTent','WeldIcon',GLOBAL.game.weldTents);
  },
  upgradeToSlapTent: function() {
    this.upgradeGeneral('SlapTent','SlapIcon',GLOBAL.game.slapTents);
  },
  upgradeToHammerTent: function() {
    this.upgradeGeneral('HammerTent','HammerIcon',GLOBAL.game.hammerTents);
  }
});

Crafty.c('ProducingTent', {
  init: function() {
    this.requires('2D,DOM,tent_up_sprite');
    
    this.madness = 0;
    this.isProducing = true;
    this.stoppedByPlayer = false;
    this.isOccupied = true;    
    
    this.isStopped = false;
    
  
    var that = this;
    Crafty.bind('EnterFrame',function(data) { that.update(data.dt/1000.0); });   
  },
  updateSmileState: function() {
    if(!this.isOccupied) {
      this.sprite(4,0);
    }
    else if(!this.isProducing) {
      this.sprite(3,0);
    }
    else if(this.madness>0.9*GLOBAL.game.madnessThreshold) {
      this.sprite(2,0);
    }
    else if(this.madness>0.5*GLOBAL.game.madnessThreshold) {
      this.sprite(1,0);
    }
    else {
      this.sprite(0,0);
    }
  },
  handleSpawn: function(dt) {
    this.spanwTimer -= dt;
    if(this.spanwTimer <= 0)
    {
      this.spawn();
      
      this.spanwTimer =  GLOBAL.game.planksSecondsPerSpawn;
    }      
  },
  diminishMadness: function(dt) {
    this.madness -= GLOBAL.game.madnessDiminishPerSec * dt;
    this.madness = Math.max(0,this.madness);
    
    //back to work
    if(this.madness <= 0 && this.isOccupied) {
      this.isProducing = true;
      this.stoppedByPlayer = false;
      this.icon.on();
    }
  },
  increaseMadness: function(dt) {
      this.madness += GLOBAL.game.madnessPerSec * dt;
      if(this.madness >= GLOBAL.game.madnessThreshold) {
        this.isProducing = false;
        this.madness += GLOBAL.game.madnessExhaustionDamage;
        this.icon.off();
      }
  },
  update: function(dt) {
    if(this.isStopped)
      return;
    
    //smile state
    this.updateSmileState();
    // working
    if(this.isProducing && this.isOccupied) {
      this.handleSpawn(dt);
      this.increaseMadness(dt);
    }
    // stopped
    else if(!this.isProducing && this.isOccupied && this.stoppedByPlayer) {
      this.diminishMadness(dt);
    }
    // madness meltdown
    else if(!this.isProducing && this.isOccupied && !this.stoppedByPlayer) {
      //try go for a slap tent if still long way to go
      if(this.madness >= 0.5 * GLOBAL.game.madnessThreshold)
      {
        var slaptent = GLOBAL.game.findFreeSlapTent(this);
        // go for a slap 
        if(slaptent != null) {
          slaptent.reserveSpot();
          this.isOccupied = false;
          Crafty.e('HumanWithMadmen').createAndDeliver(this,slaptent);          
        }
        // no slap tent - just chill
        else {
          this.diminishMadness(dt);
        }
      }
      // Just chill
      else
      {
        this.diminishMadness(dt);
      }
    }    
  },
  humanArrived: function(human) {
    console.error('Human was not supposed to arrive here!');
  },
  humanReturned: function() {
    this.madness = 0;
    this.isOccupied = true;
    this.isProducing = true;
    this.stoppedByPlayer = false;
    this.icon.on();
  },
  clickedIcon: function() {
    if(!this.isOccupied)
        return;
    
    if(this.isProducing) {
      this.stoppedByPlayer = true;
      this.isProducing = false;
      this.icon.off();
    }
    else if(!this.isProducing && this.madness < GLOBAL.game.madnessThreshold) {
      this.stoppedByPlayer = false;
      this.isProducing = true;
      this.icon.on();
    }
  },
  stop: function() {
    this.isStopped = true;
  }
});


Crafty.c('HammerTent', {
  init: function() {
    this.requires('ProducingTent');
    this.spanwTimer = GLOBAL.game.planksSecondsPerSpawn;
  },
  spawn: function(dt) {
    Crafty.e('HumanWithPlanks').createAndDeliver(this);  
  }
});

Crafty.c('WeldTent', {
  init: function() {
    this.requires('ProducingTent');
    this.spanwTimer =  GLOBAL.game.beamsSecondsPerSpawn;
    
  },
  spawn: function(dt) {
    Crafty.e('HumanWithBeams').createAndDeliver(this);
  }

});

Crafty.c('SlapTent', {
  init: function() {
    this.requires('2D,DOM,tent_up_sprite');
    this.handlingSomeone = false;
    this.handlingHuman = null;
    this.isStopped = false;
    this.slapTimer = 0;
    
    var that = this;
    Crafty.bind('EnterFrame',function(data) { that.update(data.dt/1000.0); });   
  },
  canDoSlapping: function() {
    return !this.handlingSomeone;
  },
  reserveSpot: function() {
    this.handlingSomeone = true;
  },
  humanArrived: function(human) {
    this.handlingHuman = human;    
  },
  releaseHuman: function() {
    this.slapTimer = 0;
    this.handlingSomeone = false;
    var human = this.handlingHuman;
    
    human.jacQueueAnimation('appear');
    human.walkToTent(human.madmansTent,function() {
      this.jacQueueAnimation('disappear');
      this.jacQueueCallback(function() {
        human.madmansTent.humanReturned();
        human.remove();
      });
    });
    
    this.handlingHuman = null;
    
  },
  update: function(dt) {
    if(this.isStopped)
      return;
    
    if(this.handlingHuman!=null && this.handlingSomeone) {
      this.slapTimer += dt;
      
      if(this.slapTimer>=GLOBAL.game.slapSecondsPer) {        
        this.releaseHuman();
      }
    }    
  },
  stop: function() {
    this.isStopped = true;
  }
});


