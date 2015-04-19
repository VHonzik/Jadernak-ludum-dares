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
    this.isOn = true;
    this.jacQueueAnimation('appear');
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

Crafty.c('ProductionIcon', {
  init: function() {
    this.requires('Mouse');
    this.parentTent = null;
    this.bind('Click',$.proxy(this.mClick,this));
  },
  createAboveTent: function(tent) {
    this.parentTent = tent;
    this.jacAttr({rtpcX:0,rtpcY:-70,rtpElement:tent,rtpFollow:true});
    return this;
  },
  mClick: function() {
    if(this.parentTent != null)
      this.parentTent.clickedIcon();
  }
  
});


Crafty.c('WeldIcon', {
  init: function() {
    this.requires('jac2D, DOM, weld_sprite, ProductionIcon').jacAttr({w:80,h:80});
  },
  on: function() {
      this.sprite(0,0);
  },
  off: function() {
    this.sprite(3,4);
  }
});

Crafty.c('HammerIcon', {
  init: function() {
    this.requires('jac2D, DOM, hammer_sprite,ProductionIcon').jacAttr({w:94,h:82});
  },
  on: function() {
      this.sprite(0,0);
  },
  off: function() {
    this.sprite(1,5);
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