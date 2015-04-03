GAME.PLAYER.init = function() {
	Crafty.c('OverheadAttachable', {
		init: function() {
			this.requires('2D');
			return this;
		},
		attachPoint: [32,-35],
		setAttachPoint: function(rx,ry) {
			this.attachPoint = [rx,ry];
			return this;
		},
		getAttachPoint: function() {
			return [this.attachPoint[0]+this._x,this.attachPoint[1]+this._y];
		}
	});

	Crafty.sprite(96, 'art/man.png', {
			spr_char:  [1, 0]
	});
	
	Crafty.c('PlayerCharacter', {
		init: function() {
			var pl = this;
			this.leaveTimer = 0;
			var walkingAnimSpeed = 800;
			//this.requires('2D, DOM, spr_char, Collision, Fourway, SpriteAnimation, OverheadAttachable, ManualEmotions, WiredHitBox, DebugCanvas')
			this.requires('2D, DOM, spr_char, Collision, OverheadAttachable, Fourway, SpriteAnimation, ManualEmotions')
			.fourway(2)
			.reel('PlayerMovingDown',    walkingAnimSpeed, [[1,0],[0,0],[1,0],[2,0]])
			.reel('PlayerMovingLeft', walkingAnimSpeed, [[1,3],[0,3],[1,3],[2,3]])
			.reel('PlayerMovingRight',  walkingAnimSpeed, [[1,1],[0,1],[1,1],[2,1]])
			.reel('PlayerMovingUp',  walkingAnimSpeed,[[1,2],[0,2],[1,2],[2,2]])
			.reel('Sleeping', 200,  0, 4, 1)
			.animate('PlayerMovingLeft',-1)
			.reelPosition(0)
			.pauseAnimation()
			.attr({ x: 0, y: 0, w: 96, h: 96, z:2})
			.collision([23,6],[66,6],[66,96],[23,96])
			.stopOnCollision();
			
			this.one('Move',function() {
				pl.bind('EnterFrame',function(data) {
					pl.checkForLeavingStage(data.dt);
				});
			});
			
			this.bind('NewDirection', function(data) {
				if(this.disableControls) return;
				if (data.x > 0) {
					pl.animate('PlayerMovingRight', -1);
				} else if (data.x < 0) {
					pl.animate('PlayerMovingLeft', -1);
				} else if (data.y > 0) {
					pl.animate('PlayerMovingDown', -1);
				} else if (data.y < 0) {
					pl.animate('PlayerMovingUp', -1);
				} else {
					pl.reelPosition(0);
					pl.pauseAnimation();					
				}
			});

			return this;
		},
		stopOnCollision: function() {
			this.onHit('Obstacle', this.stopMovement);		 
			return this;
		},
		stopMovement: function() {
			this._speed = 0;
			if (this._movement) {
			  this.x -= this._movement.x;
			  this.y -= this._movement.y;
			}
		},
		checkForLeavingStage: function(dt) {
			if(this._x>=883 || this._y<=27 || this._x<=49 )	{
				this.leaveTimer += dt / 1000;
				if(this.leaveTimer >= 2)
				{
					this.unbind('EnterFrame');
					this.unbind('Move');
					GAME.EVENTS.phases[GAME.EVENTS.curPhase].left = true;
					GAME.NPCS.wife.clearBehaviour();
					GAME.NPCS.lover.clearBehaviour();
					GAME.PLAYER.go.disableControl();
					setTimeout(function() {	
						GAME.OBJECTS.spotlight.turnOff();						
					},2000);
					setTimeout(function() {
						GAME.unloadPlayScene();
						Crafty.scene('scoreBoard'); 
					},4000);
				}
			}
			else {
				this.leaveTimer = 0;
			}
		}
	});
}