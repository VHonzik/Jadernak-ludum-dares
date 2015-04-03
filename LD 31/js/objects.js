GAME.OBJECTS.init = function() {
	Crafty.sprite(384,192,'art/bed.png', {
			spr_bed:  [0, 0]
	});
	
	Crafty.c('BedUp', { 
		init: function() {
			var bedup = this;
			this.requires('2D, DOM, spr_bed, Obstacle')
			.attr({w: 192, h:96,z:3})
			.crop(0,0,192,96)
			.collision([0,20],[192,20],[192,96],[0,96]);
			bedup.bind('EnterFrame',bedup.checkPlayerClose);
		},
		changeState: function() {
			this.crop(192,0,192,96);
			return this;
		},
		checkPlayerClose: function() {
				//Player opened door
				if(GAME.EVENTS.curPhase>0) {
					// Are we close?
					var plPos = new Crafty.math.Vector2D(GAME.PLAYER.go._x,GAME.PLAYER.go._y);
					var bedPos = new Crafty.math.Vector2D(this._x,this._y);
					if(plPos.distance(bedPos)<100) {
						if(!GAME.PLAYER.go.gosleep.enabledButton) GAME.PLAYER.go.gosleep.enableButton(true);
					}
					else {
						if(GAME.PLAYER.go.gosleep.enabledButton) GAME.PLAYER.go.gosleep.enableButton(false);
					}
				}
		}		
	});
	
	Crafty.c('BedDown', { 
		init: function() {
			this.requires('2D, DOM, spr_bed')
			.crop(0,96,192,96)
			.attr({w: 192, h:96,z:1});
		},
		changeState: function() {
			this.crop(192,96,192,96);
			return this;
		}		
	});
	
	Crafty.sprite(64,128,'art/closet.png', {
			spr_closet:  [0, 0]
	});
	
	Crafty.c('ClosetUp', { 
		init: function() {
			this.requires('2D, DOM, spr_closet, Obstacle')
			.attr({w: 64, h:64,z:3})
			.crop(0,0,64,64)
			.collision([2,60],[60,60],[60,70],[0,70]);
		}	
	});
	
	Crafty.c('ClosetDown', { 
		init: function() {
			this.requires('2D, DOM, spr_closet')
			.attr({w: 64, h:64,z:1})
			.crop(0,64,64,64);
		}	
	});
	
	Crafty.sprite(32,576,'art/wall.png', {
			spr_wall:  [0, 0]
	});
	
	Crafty.c('Wall', { 
		init: function() {
			this.requires('2D, DOM, spr_wall, Obstacle')
			.attr({w: 31, h:576,z:3})
			.collision([20,-20],[32,-20],[32,256],[20,256]);
		}	
	});
	
	Crafty.sprite(1024,40,'art/ui.png', {
			spr_ui:  [0, 0]
	});
	
	Crafty.c('UIbar', { 
		init: function() {
			this.requires('2D, DOM, spr_ui')
			.attr({x:0,y:696,w: 1024, h:40,z:2})		
		}	
	});
	
	
	Crafty.sprite(80,165,'art/door.png', {
			spr_door:  [0, 0]
	});
	
	Crafty.c('Door', { 
		init: function() {
			var door = this;
			this.animSpeed = 200;
			this.requires('2D, DOM, spr_door, Obstacle, SpriteAnimation')
			.attr({w: 80, h:165, z:2})
			.reel('Open', this.animSpeed, [[0,0],[1,0],[2,0],[3,0],[4,0],[5,0]])
			.reel('Close', this.animSpeed, [[5,0],[4,0],[3,0],[2,0],[1,0],[0,0]])
			.collision([69,-17],[80,-17],[80,150],[69,150]);
			this.bind('EnterFrame',door.checkPlayerClose);
			this.closed = true;
			
		},
		open: function() {
			if(this.closed) {
				this.closed = false;
				this.animate('Open').collision([69,148],[80,148],[80,243],[69,243]);
			}
			return this;
		},
		close: function() {
			if(!this.closed) {
				this.closed = true;
				this.animate('Close').collision([69,-17],[80,-17],[80,150],[69,150])
			}
			
			return this;
		},
		checkPlayerClose: function() {
				// Doors are still closed
				if(GAME.EVENTS.curPhase===0) {
					// Are we close?
					var plPos = new Crafty.math.Vector2D(GAME.PLAYER.go._x,GAME.PLAYER.go._y);
					var doorPos = new Crafty.math.Vector2D(this._x,this._y);
					if(plPos.distance(doorPos)<100) {
						if(!GAME.PLAYER.go.listen.enabledButton) GAME.PLAYER.go.listen.enableButton(true);
						if(!GAME.PLAYER.go.opendoor.enabledButton) GAME.PLAYER.go.opendoor.enableButton(true);
					}
					else {
						if(GAME.PLAYER.go.listen.enabledButton) GAME.PLAYER.go.listen.enableButton(false);
						if(GAME.PLAYER.go.opendoor.enabledButton) GAME.PLAYER.go.opendoor.enableButton(false);
					}
				}
				//Shouldn't be called after opening doors
				else {
					this.unbind('EnterFrame',this.checkPlayerClose);
				
				}
			
		}
	});
	
	Crafty.c('Spotlight', {
		init: function() {
			this.requires('2D,DOM,Image').image('art/lightoff.png').attr({x:0,y:0,w:1024,h:768,z:20});
		},
		turnOn: function() {
			this.image('art/lighton.png');		
		},
		turnOff: function() {
			this.image('art/lightoff.png');		
		}
	});
	
		
	Crafty.c('IntroImage', {
		init: function() {
			this.requires('2D,DOM,Image,Mouse').image('art/ldtimespromo.png').attr({x:0,y:0,w:1024,h:768,z:20});
			this.bind('KeyDown', function(e) {
				Crafty.scene('playScene');
			});
			this.bind('Click', function(e) {
				Crafty.scene('playScene');
			});
		}
		
	});
	
	Crafty.c('FinalImage', {
		init: function() {
			this.requires('2D,DOM,Image,Mouse').image('art/ldtimesfinal.png').attr({x:0,y:0,w:1024,h:768,z:20});
		}		
	});
	
	Crafty.sprite(16,'art/soundicon.png', {
			spr_soundtool:  [0, 0]
	});
	
	Crafty.c('SoundTool', {
		init: function() {
			this.soundon = true;
			this.requires('2D,DOM,spr_soundtool,Mouse,Persist').crop(0,0,16,16).attr({x:992,y:0,w:32,h:32,z:20});
			this.bind('Click', function(e) {
				if(this.soundon) {
					Crafty.audio.mute();
					this.soundon = false;
					this.crop(16,0,16,16).attr({w:32,h:32});
				}
				else 
				{
					this.soundon = true;
					this.crop(-16,0,16,16).attr({w:32,h:32});
					Crafty.audio.unmute();
				}
			});
		}
	})
	
}