GAME.NPCS.init = function() {
	Crafty.sprite(96, 'art/lover.png', {
			spr_lover:  [0, 0]
	});
	
	Crafty.c('OrderWalking', {	
		init: function() {
			this.requires('2D, DOM');
			this.curStepDistance = 0;
			this.curStepWanted = null;
		},
		stopSteps: function() {
			this.stepsWanted = [];
			this.curStepWanted = null;
		},
		goSteps: function(steps) {
			var ent = this;
			this.curStepDistance = 0;
			this.curStepWanted = null;
			this.stepsWanted = steps;
			this.bind('EnterFrame',function(data) {ent.processSteps(data.dt)});
		},
		processSteps: function(dt) {
			if(this.curStepWanted != null) {
				var dist = this.walkingSpeed;
				if(this.curStepWanted[0]==='down')
					this.move('s',dist);
				else if(this.curStepWanted[0]==='up')
					this.move('n',dist);
				else if(this.curStepWanted[0]==='right')
					this.move('e',dist);
				else if(this.curStepWanted[0]==='left') 
					this.move('w',dist);
				else 
					console.log('unknown direction');
				this.curStepDistance += dist;
				if(this.curStepDistance >= this.curStepWanted[1])
				{
					this.curStepWanted = null;
				}
			}
			else if(this.stepsWanted.length > 0){
				this.curStepWanted = this.stepsWanted.shift();
				this.curStepDistance = 0;
				
				if(this.curStepWanted[0]==='down')
					this.animate('MovingDown',-1);
				else if(this.curStepWanted[0]==='up')
					this.animate('MovingUp',-1);
				else if(this.curStepWanted[0]==='right')
					this.animate('MovingRight',-1);
				else if(this.curStepWanted[0]==='left') 
					this.animate('MovingLeft',-1);
			}
			else {
				this.reelPosition(1).pauseAnimation();
			}
		}
	});
	
	Crafty.c('LoverCharacter', {
		init: function() {
			this.animSpeed = 600;
			this.requires('2D, DOM, spr_lover, OrderWalking, OverheadAttachable, SpriteAnimation, Behaviour')
				.reel('MovingDown',    this.animSpeed, 0, 0, 3)
				.reel('MovingLeft', this.animSpeed, 0, 3, 3)
				.reel('MovingRight',  this.animSpeed, 0, 1, 3)
				.reel('MovingUp',  this.animSpeed, 0, 2, 3)
				.reel('LoverInBedLookingAtWife',  this.animSpeed, 1, 4, 1)
				.reel('LoverInBedLookingUp',  this.animSpeed, 0, 4, 1)
				.reel('Hidden',  this.animSpeed, 2, 4, 1)
				.setAttachPoint(-25,0)
				.attr({w: 96, h: 96, z:2})
				.randomEmotionsWithPauses([['Love',3,true,1]],2,5);
			this.walkingSpeed = 2;
				
		},
		inBedLookingAtWife: function() {
			this.animate('LoverInBedLookingAtWife').pauseAnimation().attr({z: 4});
		},
		inBedLookingUp: function() {
			this.animate('LoverInBedLookingUp').pauseAnimation().attr({z: 4});
		},
		getOutOfBed: function(tryToHide) {
			var lovr = this;
			this.animate('MovingUp').reelPosition(1).pauseAnimation()
			.setAttachPoint(32,-35).attr({x:this._x+20,y:this._y-60,z:2});
			if(tryToHide) setTimeout(function() {lovr.tryToHide()},1000);
		},
		tryToHide: function() {
			var lovr = this;
			//If player entered the room it stop searching for cover
			if(GAME.EVENTS.curPhase === 0)
			{
				this.animate('MovingLeft').reelPosition(1).pauseAnimation();	
				//Look around
				setTimeout(function() {if(GAME.EVENTS.curPhase == 0) lovr.animate('LoverMovingRight').reelPosition(1).pauseAnimation()},1000);
				
				//Go hiding
				setTimeout(function() {
					if(GAME.EVENTS.curPhase === 0) {
						lovr.animate('MovingUp').reelPosition(1).pauseAnimation();
						lovr.simpleQueue([['Hide',3,true,1]]);						
						lovr.goSteps([['right',40],['up',80]]);
					}
				},2000);
				
				//Hide
				setTimeout(function() {
					if(GAME.EVENTS.curPhase === 0) {
						lovr.animate('Hidden').reelPosition(1).pauseAnimation();
						GAME.NPCS.wife.clearBehaviour();
						GAME.EVENTS.phases[0].loverHasHidden = true;
					}
				},5000);
			}
		},
		alarmByHome: function() {
			var lovr = this;
			this.inBedLookingUp();
			this.clearBehaviour();
			this.simpleQueue([['Danger',5,true,1]]);
			setTimeout(function() {lovr.getOutOfBed(true)},2000);
		},
		interuptHidding: function() {
			var lover = this;
			lover.stopSteps();
			lover.animate('MovingRight').reelPosition(1).pauseAnimation();
			this.clearBehaviour();
			this.simpleQueue([['Danger',2,true,1]]);			
		},
		supriseByDoor: function() {
			var lovr = this;
			this.inBedLookingUp();
			this.clearBehaviour();
			this.simpleQueue([['Danger',3,true,1]]);
			setTimeout(function() {
				lovr.getOutOfBed(false);
				lovr.animate('MovingRight').reelPosition(1).pauseAnimation();
			},3000);

		},
		flee: function() {
			GAME.EVENTS.phases[GAME.EVENTS.curPhase].loverFled = true;
			var lovr = this;
			lovr.goSteps([['right',300],['down',100],['right',400]]);
		}
	});

	Crafty.sprite(96, 'art/wife.png', {
			spr_wife:  [0, 0]
	});
	
	Crafty.c('WifeCharacter', {
		init: function() {
			this.animSpeed = 600;
			this.walkingSpeed = 2;
			this.requires('2D, DOM, spr_wife, OrderWalking, OverheadAttachable, SpriteAnimation, Behaviour')
				.reel('MovingDown',    this.animSpeed, 0, 0, 3)
				.reel('MovingLeft', this.animSpeed, 0, 3, 3)
				.reel('MovingRight',  this.animSpeed, 0, 1, 3)
				.reel('MovingUp',  this.animSpeed, 0, 2, 3)
				.reel('InBedLookingAtLover',  this.animSpeed, 1, 4, 1)
				.reel('InBedLookingUp',  this.animSpeed, 0, 4, 1)
				.setAttachPoint(-25,0)
				.attr({w: 96, h: 96, z:2})
				.randomEmotionsWithPauses([['Love',3,true,0]],2,5);
				
		},
		lookUp: function() {
			this.animate('InBedLookingUp', -1).attr({z: 4});
			this.attr({x: this._x+5,y:this._y+4});
			return this;
		},
		inBedLookingAtLover: function() {
			this.animate('InBedLookingAtLover', -1).attr({z: 4});
			return this;
		},
		alarmByHome: function() {
			this.lookUp();
			this.clearBehaviour();
			this.stopEmotions();
			this.simpleQueue([['Danger',5,false,5]]);
			this.randomEmotions([['Danger',4,true,1],['Hurry',4,true,1]]);
		},
		flee: function() {
			var wife = this;
			this.clearBehaviour();
			this.animate('MovingDown').reelPosition(1).setAttachPoint(32,-35).attr({x:this._x+20,y:this._y+60,z:2});
			
			this.goSteps([['right',300],['up',50],['right',400]]);
			
		},
		supriseInHiding: function() {
			var wife = this;
			this.clearBehaviour();
			this.simpleQueue([['Danger',2,true,1],['Talk',3,true,1]]);
			setTimeout(function() {
				wife.randomEmotionsWithPauses([['Talk',3,true,1]],1,2);
			},5000);
		},
		greetLie: function() {
			this.clearBehaviour();
			this.simpleQueue([['Hello',5,true,1]]);
		},
		talkCasually: function() {
			this.clearBehaviour();
			this.randomEmotionsWithPauses([['Talk',3,true,1]],2,6);
		},
		supriseByDoor: function() {
			var wife = this;
			this.lookUp();
			this.clearBehaviour();
			this.simpleQueue([['Danger',3,true,1]]);
			setTimeout(function() {
				wife.randomEmotionsWithPauses([['Talk',3,true,1]],1,2);
			},5000);
		},

	});		
}