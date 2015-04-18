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
  upgradeGeneral: function() {
    Crafty.trigger('EmptyTentClicked');
    this.isOn = false;
    this.jacQueueAnimation('upgrade');
    this.jacQueueCallback(function() {
      var attrs = {x:this._x,y:this._y,w:this._w,h:this._h};
      GLOBAL.game.emptyTents.splice( $.inArray(this, GLOBAL.game.emptyTents), 1 );
      this.removeComponent('EmptyTent',false).removeComponent('Mouse',false)
      this.jacReel('normal',[1,6], 1).animate('normal');
      this.attr(attrs);      
    });    
  },
  upgradeToWeldTent: function() {
    this.upgradeGeneral();
    this.jacQueueCallback(function() {
      this.addComponent('WeldTent');      
      GLOBAL.game.weldTents.push(this);
      this.icon = Crafty.e('WeldIcon').createAboveTent(this);
    });    
  },
  upgradeToSlapTent: function() {
    this.upgradeGeneral();
    this.jacQueueCallback(function() {
      this.addComponent('SlapTent');      
      GLOBAL.game.slapTents.push(this);
      this.icon = Crafty.e('SlapIcon').createAboveTent(this);
    });    
  },
  upgradeToHammerTent: function() {
    this.upgradeGeneral();
    this.jacQueueCallback(function() {
      this.addComponent('HammerTent');      
      GLOBAL.game.hammerTents.push(this);
      this.icon = Crafty.e('HammerIcon').createAboveTent(this);
    });      
  }
});

Crafty.c('SlapTent', {
  init: function() {

  }
});

Crafty.c('HammerTent', {
  init: function() {
    this.spanwTimer = GLOBAL.game.planksSecondsPerSpawn;
    
    var that = this;
    Crafty.bind('EnterFrame',function(data) { that.update(data.dt/1000.0); });   
  },
  update: function(dt) {
    this.spanwTimer -= dt;
    if(this.spanwTimer <= 0)
    {
      Crafty.e('HumanWithPlanks').createAndDeliver(this);
      this.spanwTimer =  GLOBAL.game.planksSecondsPerSpawn;
    }
  }
});

Crafty.c('WeldTent', {
  init: function() {
    this.spanwTimer =  GLOBAL.game.beamsSecondsPerSpawn;
    
    var that = this;
    Crafty.bind('EnterFrame',function(data) { that.update(data.dt/1000.0); });   
  },
  update: function(dt) {
    this.spanwTimer -= dt;
    if(this.spanwTimer <= 0)
    {
      Crafty.e('HumanWithBeams').createAndDeliver(this);
      this.spanwTimer =  GLOBAL.game.beamsSecondsPerSpawn;
    }
  }
});

Crafty.c('UPIcon', {
  init: function() {
    this.requires('Mouse');
    this.pTent = null;
    this.isOn = false;
    this.bind('Click',$.proxy(this.mClick,this));
  },
  setParentTent: function(tent) {
		this.pTent = tent;
		return this;
	},
  appear: function() {
    this.jacQueueAnimation('appear');
    this.jacQueueCallback(function() {  this.isOn = true;});
  },
  disappear: function() {
    if(this.isOn) {
      this.jacQueueAnimation('disappear');
      this.isOn = false;
    }
  },
  mClick: function() {
    if(this.isOn) {      
      this.upgrade();
      this.isOn = false;            
    }
  }
});

Crafty.c('SlapUPIcon', {
  init: function() {
    this.requires('jac2D, DOM, jacAnimation, UPIcon')
      .initFromAnimSheet('slap_sprite',6,9)
      .jacReelSheet([{animationName:'normal',frameCount:1},
      {animationName:'appear',frameCount:25},
      {animationName:'disappear',frameCount:25}])
      .jacReel('hidden',[1,0],1).animate('hidden');
      
  },
  upgrade: function() {
		this.pTent.upgradeToSlapTent();
	}
});

Crafty.c('HammerUPIcon', {
  init: function() {
    this.requires('jac2D, DOM, jacAnimation, UPIcon')
      .initFromAnimSheet('hammer_sprite',6,10)
      .jacReelSheet([{animationName:'normal',frameCount:1},
      {animationName:'appear',frameCount:25},
      {animationName:'disappear',frameCount:25}])
      .jacReel('hidden',[1,0],1).animate('hidden');
      
  },
  upgrade: function() {
		this.pTent.upgradeToHammerTent();
	}
});

Crafty.c('WeldUPIcon', {
  init: function() {
    this.requires('jac2D, DOM, jacAnimation, UPIcon')
      .initFromAnimSheet('weld_sprite',5,12)
      .jacReelSheet([{animationName:'normal',frameCount:1},
      {animationName:'appear',frameCount:25},
      {animationName:'disappear',frameCount:25}])
      .jacReel('hidden',[1,0],1).animate('hidden');
      
  },
  upgrade: function() {
		this.pTent.upgradeToWeldTent();
	}
});


Crafty.c('WeldIcon', {
  init: function() {
    this.requires('jac2D, DOM, weld_sprite').jacAttr({w:80,h:80});
  },
  createAboveTent: function(tent) {
    this.jacAttr({rtpcX:0,rtpcY:-70,rtpElement:tent,rtpFollow:true});
    return this;
  }
});

Crafty.c('HammerIcon', {
  init: function() {
    this.requires('jac2D, DOM, hammer_sprite').jacAttr({w:94,h:82});
  },
  createAboveTent: function(tent) {
    this.jacAttr({rtpcX:0,rtpcY:-70,rtpElement:tent,rtpFollow:true});
    return this;
  }
});

Crafty.c('SlapIcon', {
  init: function() {
    this.requires('jac2D, DOM, slap_sprite').jacAttr({w:113,h:80});
  },
  createAboveTent: function(tent) {
    this.jacAttr({rtpcX:0,rtpcY:-70,rtpElement:tent,rtpFollow:true});
    return this;
  }
});