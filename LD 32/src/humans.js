Crafty.c('Human',{
	init: function() {
    this.goUpXThreshold = 600;
		this.requires('jac2D, DOM, jacAnimation').jacAttr({w:54,h:40});    
    
    this.speedX = 40;
    this.speedY = 40;
    this.moveQueue = [];
    this.doingMove = false;
    this.currentMove = null;
    
    this.isStopped = false;
    
    this.flippedX = false;
    
    var that = this;
    Crafty.bind('EnterFrame',function(data) { that.update(data.dt/1000.0); });    
    
    GLOBAL.game.humans.push(this);
	},
  getTentEntryPoint: function(tent) {
    var res = {};
    res.x = tent._x + tent._w * 0.75 - this._w / 2.0;
    res.y = tent._y + 0.75 * tent._h;
    return res;
  },
  getBuildingEntryPoint: function() {
    var res = {};
    res.x = GLOBAL.game.building._x + GLOBAL.game.building._w * 0.5 - this._w / 2.0;
    res.y = GLOBAL.game.building._y + GLOBAL.game.building._h - this._h / 2.0;
    return res;    
  },
  spawnAtTent: function(tent) {
    var tentEntryPoint = this.getTentEntryPoint(tent);
    this.jacAttr({x:tentEntryPoint.x,y:tentEntryPoint.y}).jacQueueAnimation('appear');
    return this;
  },
  walkToBuilding: function(finishCallback) {
    var buildEntry = this.getBuildingEntryPoint();
    this.moveQueue.push({type:'move',dir:'x',until:this.goUpXThreshold});
    this.moveQueue.push({type:'move',dir:'y',until:buildEntry.y});
    this.moveQueue.push({type:'move',dir:'x',until:buildEntry.x});
    this.moveQueue.push({type:'callback',fun:$.proxy(finishCallback,this)});
    return this;
  },
  walkToTent: function(tent,humanCallback) {
    var tentEntry = this.getTentEntryPoint(tent);
    // Need to change lines?
    if(Math.abs(this._y-tentEntry.y)> 3) {
      this.moveQueue.push({type:'move',dir:'x',until:this.goUpXThreshold});
      this.moveQueue.push({type:'move',dir:'y',until:tentEntry.y});
      this.moveQueue.push({type:'move',dir:'x',until:tentEntry.x});
      this.moveQueue.push({type:'callback',fun:$.proxy(humanCallback,this)});
    }
    // Go directly
    else {
      this.moveQueue.push({type:'move',dir:'x',until:tentEntry.x});
      this.moveQueue.push({type:'callback',fun:$.proxy(humanCallback,this)});
    }
    return this;
  },
  walkingUpdate: function(dt) {    
    if(!this.doingMove && this.moveQueue.length > 0)
    {
      var move = this.moveQueue.shift();
      this.doingMove = true;
      if(move.type==='move' && move.dir==='x')
      {        
        this.jacQueueCallback(function(){
            this.jacQueueAnimation('startWalking');
            this.currentMove = move;
        });
      }
      if(move.type==='move' && move.dir==='y') 
      {        
        this.jacQueueCallback(function(){
            this.jacQueueAnimation('side');
            this.currentMove = move;
        });
      }
      
      if(move.type==='callback') {
        move.fun();
        this.doingMove = false;
        this.currentMove = null;
      }
    }
    
    if(this.doingMove && this.currentMove != null && this.currentMove.dir === 'x')
    {      
      var dif = this.currentMove.until - this._x;
      var difAbs = Math.abs(dif);
      var difSign = dif < 0 ? -1 : 1;
      var wantedMove = difSign * Math.min(difAbs,this.speedX*dt);
      this.x += wantedMove;
      
      if(difSign<0 && !this.flippedX) {
        this.flip("X");
        this.flippedX = true;
      }
      
      if(difSign>0 && this.flippedX) {
        this.unflip("X");
        this.flippedX = false;
      }
      
      if(Math.abs(this.x-this.currentMove.until)<0.1)
      {        
        this.currentMove = null;        
        this.jacQueueCallback(function(){
          this.doingMove = false;          
        });
        return;
      }
      else
      {
        if(this.animQueue.length == 0) 
        { 
          if(difAbs>this.speedX*40*GLOBAL.msPerAnimFrame/1000.0)
          {
            this.jacQueueAnimation('walking');           
          }
          else if(difAbs>this.speedX*15*GLOBAL.msPerAnimFrame/1000.0)
          {
             this.jacQueueAnimation('endWalking');
          }          
        }
      }
    }
    
    if(this.doingMove && this.currentMove != null  && this.currentMove.dir === 'y')
    {
      var dif = this.currentMove.until - this._y;
      var difAbs = Math.abs(dif);
      var difSign = dif < 0 ? -1 : 1;
      var wantedMove = difSign * Math.min(difAbs,this.speedY*dt);
      this.y += wantedMove;
      
      if(Math.abs(this.y-this.currentMove.until)<0.1)
      {
        this.currentMove = null;
        this.jacQueueCallback(function(){
          this.doingMove = false;          
        });
        return;
      }
    }
  },
  update: function(dt) {
    if(this.isStopped)
      return;
    this.walkingUpdate(dt);    
  },
  stop: function() {
    this.isStopped = true;
  },
  removeFromGame: function() {
    GLOBAL.game.humans.splice( $.inArray(this, GLOBAL.game.humans), 1 );
    this.destroy();
  }
});

Crafty.c('HumanWithPlanks', {
  init: function() {
    this.requires('Human')
    .initFromAnimSheet('human_planks_sprite',6,18)
    .jacReelSheet([
      {animationName:'appear',frameCount:25},
      {animationName:'startWalking',frameCount:12},
      {animationName:'walking',frameCount:25},
      {animationName:'endWalking',frameCount:12},
      {animationName:'side',frameCount:1},
      {animationName:'disappear',frameCount:25},
    ]);
    
  },
  createAndDeliver: function(tent) {
  this.spawnAtTent(tent).walkToBuilding(function() {
    this.jacQueueAnimation('disappear');
    this.jacQueueCallback(function(){
      GLOBAL.game.rampScaff.planksDelivered();
      this.removeFromGame();
    });
  });
  }
});

Crafty.c('HumanWithBeams', {
  init: function() {
    this.requires('Human')
    .initFromAnimSheet('human_beams_sprite',6,18)
    .jacReelSheet([
      {animationName:'appear',frameCount:25},
      {animationName:'startWalking',frameCount:12},
      {animationName:'walking',frameCount:25},
      {animationName:'endWalking',frameCount:12},
      {animationName:'side',frameCount:1},
      {animationName:'disappear',frameCount:25},
    ]);
  },
  createAndDeliver: function(tent) {
  this.spawnAtTent(tent).walkToBuilding(function() {
    this.jacQueueAnimation('disappear');
    this.jacQueueCallback(function(){
      GLOBAL.game.ramp.beamsDelivered();
      this.removeFromGame();
    });
  });
  }
});


Crafty.c('HumanWithMadmen', {
  init: function() {
    this.madmansTent = null;
    this.requires('Human')
    .initFromAnimSheet('human_madman_sprite',6,18)
    .jacReelSheet([
      {animationName:'appear',frameCount:25},
      {animationName:'startWalking',frameCount:12},
      {animationName:'walking',frameCount:25},
      {animationName:'endWalking',frameCount:12},
      {animationName:'side',frameCount:1},
      {animationName:'disappear',frameCount:25},
    ])
  },
  createAndDeliver: function(origTent,slapTent) {
    this.madmansTent = origTent;
    this.spawnAtTent(origTent).walkToTent(slapTent,function(){
      this.jacQueueAnimation('disappear');
      this.jacQueueCallback(function(){
        slapTent.humanArrived(this);
    });
    });
  }
});