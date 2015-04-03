GAME.EMOTIONS.list = {};
GAME.EMOTIONS.list['Love'] = {icon:'art/heart.png'};
GAME.EMOTIONS.list['Scream'] = {icon:'art/scream.png'};
GAME.EMOTIONS.list['Open door'] = {icon:'art/opendoor.png'};
GAME.EMOTIONS.list['Danger'] = {icon:'art/danger.png'};
GAME.EMOTIONS.list['Home'] = {icon:'art/home.png'};
GAME.EMOTIONS.list['Listen'] = {icon:'art/listen.png'};
GAME.EMOTIONS.list['Hurry'] = {icon:'art/hurryup.png'};
GAME.EMOTIONS.list['Hide'] = {icon:'art/hide.png'};
GAME.EMOTIONS.list['Hello'] = {icon:'art/hello.png'};
GAME.EMOTIONS.list['Tired'] = {icon:'art/tired.png'};
GAME.EMOTIONS.list['Talk'] = {icon:'art/talk.png'};
GAME.EMOTIONS.list['Go sleep'] = {icon:'art/gosleep.png'};
GAME.EMOTIONS.list['Get out'] = {icon:'art/getout.png'};

GAME.EMOTIONS.init = function() {

	for(key in GAME.EMOTIONS.list)
	{
		var spriteName = 'spr_em_'+key;
		var mapping = {};
		mapping[spriteName] = [0,0];
		Crafty.sprite(32,16,GAME.EMOTIONS.list[key].icon,mapping);
		
		Crafty.c(key, {
			init: function() {
				this.requires('2D, DOM, spr_em_'+this.emname)
				.crop(0,0,16,16)
				.attr({w:16,h:16,z:4});
			},
			emname: key
		});
		
		Crafty.c(key+'_action', {
			init: function() {
				this.requires('2D, Mouse, DOM,'+this.emname)
				.crop(16,0,16,16)
				.attr({w:32,h:32,z:10});			
				this.enabledButton = false;
				return this;
			},
			enableButton: function(enbl) {
				if(this.enabledButton && !enbl) {
					this.crop(16,0,16,16).attr({w:32,h:32});
					this.enabledButton = false;
				}
				
				if(!this.enabledButton && enbl) {
					this.crop(-16,0,16,16).attr({w:32,h:32});
					this.enabledButton = true;
				}
				
				return this;
			},
			emname: key
		})
	}		

	Crafty.sprite(32,'art/cloud.png', {
			spr_cloud:  [4, 0]
	});
	
	Crafty.c('EmotionCloud', {
		init: function() {
			var cloud = this;
			this.animationSpeed = 500;
			
			this.requires('2D, DOM, spr_cloud, SpriteAnimation')
			.reel('Appear', this.animationSpeed, [[4,0],[3,0],[2,0],[1,0],[0,0]])
			.reel('Disappear', this.animationSpeed, [[0,0],[1,0],[2,0],[3,0],[4,0]])
			.attr({w: 32, h:32, z:3});
			this.levX = Math.random() * 2 * Math.PI;
			this.levSpeed = Math.PI * 7 + (Math.random() * 2) - 1;

			this.levHight = 3;
			this.levY = 0;
			this.levNormalY = 0;
			this.levX = 0;
			this.emotionEntity = null;
			this.isOn = false;
			this.bind('EnterFrame',function(data) {
				cloud.levitate(data.dt);
				if(cloud.emotionEntity != null) cloud.emotionEntity.attr({x: cloud.x + 8, y:cloud.y+8});
			});
			return this;
		},
		levitate: function(t) {
			this.levX = (this.levX + (t * 0.0001 * this.levSpeed)) % (2*Math.PI);
			this.levY = this.levHight * Math.sin( this.levX );
			this.attr({y: this.levNormalY + this.levY});
		},
		attachToEntity: function(entity) {
			var cloud = this;
			var atpoint = entity.getAttachPoint();
			cloud.attr({x: atpoint[0]})
			cloud.levNormalY = atpoint[1];
			entity.bind('Move',function() {
				var atp = this.getAttachPoint();
				cloud.attr({x: atp[0]});
				cloud.levNormalY = atp[1];
			});
			return this;
		},
		displayEmotion: function(emotionEntityName) {
			if(this.emotionEntity != null) this.emotionEntity.destroy();
			this.emotionEntity = Crafty.e(emotionEntityName).attr({x: this.x + 8, y:this.y+8});
			return this;
		},
		hideEmotion: function() {
			if(this.emotionEntity != null) this.emotionEntity.destroy();
			return this;
		},
		turnOn: function() {
			var cloud = this;
			this.animate('Appear',1);
			setTimeout(function () {cloud.isOn = true;}, cloud.animSpeed);
		},
		turnOff: function() {
			var cloud = this;
			cloud.isOn = false;
			this.animate('Disappear',1);
		}
	});
	
	Crafty.c('EmotionQueue', {
		init: function() {
			var emqueue = this;
			
			this.queue = [];
			this.curEmTimer = 0;
			this.curEmDuration = 0;
			this.curEmMinDuration = 0;
			this.curEmCanBeInterupted = false;
			this.hasCurEm = false;
			
			this.emCloud = Crafty.e('EmotionCloud').attachToEntity(this);
			this.bind('EnterFrame',function(data) {
				emqueue.processEQ(data.dt);
			});
		},

		addEmotion: function(emotion,duration,canBeInterupted,minDuration) {
			minDuration = minDuration || duration;
			this.queue.push([emotion,duration,canBeInterupted,minDuration]);
			return this;
		},
		processEQ: function(dt) {			
			this.curEmTimer += dt / 1000.0;
			
			//Destroy cur emotion if time is up
			if(this.hasCurEm && this.curEmTimer>this.curEmDuration) {
				this.emCloud.hideEmotion();
				this.curEmTimer = 0;				
				this.hasCurEm = false;
			}
			
			//Also destroy if can be interrupted and was displayed for minimun time
			if(this.hasCurEm && this.curEmTimer>this.curEmMinDuration && this.queue.length > 0 && this.curEmCanBeInterupted) {
				this.emCloud.hideEmotion();
				this.curEmTimer = 0;
				this.hasCurEm = false;				
			}
			
			//Turning on of the cloud
			if(this.queue.length > 0 && !this.emCloud.isOn)
			{
				this.emCloud.turnOn();
			}
			
			//Turning off of the cloud
			if(this.queue.length <= 0 && this.emCloud.isOn && !this.hasCurEm && this.curEmTimer > 5)
			{
				this.emCloud.turnOff();
			}
			
			//Dequeue
			if(this.queue.length > 0 && !this.hasCurEm && this.emCloud.isOn){
				this.hasCurEm = true;
				this.curEmTimer = 0;
				
				var newEm = this.queue.shift();
				
				this.curEmCanBeInterupted = newEm[2];
				this.curEmMinDuration = newEm[3];
				this.curEmDuration = newEm[1];
				this.emCloud.displayEmotion(newEm[0]);
			}
		}		
		
	});
	
	Crafty.c('Behaviour', {
		init: function() {
			this.requires('EmotionQueue');
			this.state = {};
			this.hasBehaviour = false;
			
		},
		unbindCurrentProcessing: function() {
			if(typeof this.state.unbindfunction !== 'undefined')
				this.unbind('EnterFrame',this.state.unbindfunction);
		},
		simpleQueue: function(emotions) {
			for (i = 0; i < emotions.length; i++) {
				var em = emotions[i];
				this.addEmotion(em[0],em[1],em[2],em[3]);
			}			
		},
		clearBehaviour: function() {
			this.stopEmotions();
			this.hasBehaviour = false;
			this.unbindCurrentProcessing();
			this.state = {};
		},
		stopEmotions: function() {
			this.curEmTimer = this.curEmDuration + 1;
		},		
		randomEmotions: function(emotions) {
			var behaviour = this;
			
			this.unbindCurrentProcessing();
			
			behaviour.state = {};
			behaviour.state.emotions = emotions;
			
			this.hasBehaviour = true;
			
			//Queue in first emotion
			var rndEm = behaviour.state.emotions[Math.floor(Math.random()*behaviour.state.emotions.length)];
			behaviour.addEmotion(rndEm[0],rndEm[1],rndEm[2],rndEm[3]);
			
			behaviour.state.unbindfunction = function() {
				behaviour.processRandomEmotions();
			}
			
			behaviour.bind('EnterFrame',behaviour.state.unbindfunction);
			
			return this;
			
		},
		processRandomEmotions: function() {		
			var behaviour = this;
			//If queue is empty queue a rnd emotion
			if(behaviour.queue.length <=0 && this.hasBehaviour) {
				var rndEm = behaviour.state.emotions[Math.floor(Math.random()*behaviour.state.emotions.length)];
				behaviour.addEmotion(rndEm[0],rndEm[1],rndEm[2],rndEm[3]);
			}
		},
		processRandomEmotionsWithPauese: function(dt) {
			var behaviour = this;
			//Update em
			if(behaviour.state.doingEmotion && !behaviour.hasCurEm && behaviour.queue.length <=0 && this.hasBehaviour) {
				behaviour.state.doingPause = true;
				behaviour.state.doingEmotion = false;
				behaviour.state.pauseTimer = 0;
				behaviour.state.curPauseDur = behaviour.state.minPause + 
					Math.random() * (behaviour.state.maxPause - behaviour.state.minPause); 
			}
			
			// Update pause
			if(behaviour.state.doingPause && this.hasBehaviour) {
				behaviour.state.pauseTimer += dt / 1000.0;
				//Paused ended do emotion
				if(behaviour.state.pauseTimer>behaviour.state.curPauseDur) {
					behaviour.state.doingPause = false;
					behaviour.state.doingEmotion = true;						
					var rndEm = behaviour.state.emotions[Math.floor(Math.random()*behaviour.state.emotions.length)];
					behaviour.addEmotion(rndEm[0],rndEm[1],rndEm[2],rndEm[3]);

				}
			}
		},
		randomEmotionsWithPauses: function(emotions, minPause, maxPause) {
			var behaviour = this;
			
			this.unbindCurrentProcessing();

			this.state = {};
			this.state.pauseTimer = 0;
			this.state.minPause = minPause;
			this.state.maxPause = maxPause;
			this.state.curPauseDur = 0;
			this.state.emotions = emotions;
			
			this.hasBehaviour = true;
			
			//Queue in first emotion
			this.state.doingEmotion = true;
			this.state.doingPause = false;
			var rndEm = behaviour.state.emotions[Math.floor(Math.random()*behaviour.state.emotions.length)];
			behaviour.addEmotion(rndEm[0],rndEm[1],rndEm[2],rndEm[3]);			
			
			
			behaviour.state.unbindfunction = function(data) {
				behaviour.processRandomEmotionsWithPauese(data.dt);
			} 
			
			behaviour.bind('EnterFrame',behaviour.state.unbindfunction);
			
			return this;
		}
	
	});
	
	Crafty.c('ManualEmotions', {
		init: function() {
			this.requires('EmotionQueue');
			var manualEmotions = this;
			
			manualEmotions.love = Crafty.e('Love_action').attr({x:604,y:700,w:32,h:32});
			manualEmotions.love.bind('Click',function(){
				if(manualEmotions.love.enabledButton)
				{
					GAME.EVENTS.phases[GAME.EVENTS.curPhase].emotionsUsed.push('Love');
					manualEmotions.addEmotion('Love',3,true,1);
				}
			});
			
			manualEmotions.scream = Crafty.e('Scream_action').attr({x:539,y:700,w:32,h:32});
			manualEmotions.scream.bind('Click',function(){
				if(manualEmotions.scream.enabledButton)
				{
					GAME.EVENTS.phases[GAME.EVENTS.curPhase].emotionsUsed.push('Scream');
					manualEmotions.addEmotion('Scream',3,true,1);
				}
			});
			
			manualEmotions.opendoor = Crafty.e('Open door_action').attr({x:92,y:700,w:32,h:32});
			manualEmotions.opendoor.bind('Click',function(){
				if(manualEmotions.opendoor.enabledButton)
				{
					GAME.EVENTS.phases[GAME.EVENTS.curPhase].emotionsUsed.push('Open door');
					if(GAME.OBJECTS.door.closed)
					{
						GAME.EVENTS.curPhase = 1;
						GAME.OBJECTS.door.open();
						manualEmotions.opendoor.enableButton(false);
						manualEmotions.hello.enableButton(true);
						manualEmotions.scream.enableButton(true);
						manualEmotions.love.enableButton(true);
						manualEmotions.addEmotion('Open door',3,false,2);						
						
						// Opened doors after alarming and before lover has hidden
						if(GAME.EVENTS.phases[0].honeyImHome && !GAME.EVENTS.phases[0].loverHasHidden)
						{
							manualEmotions.getout.enableButton(true);
							GAME.NPCS.lover.interuptHidding();
							GAME.NPCS.wife.supriseInHiding();
							setTimeout(function() {
								if(!GAME.EVENTS.phases[GAME.EVENTS.curPhase].loverFled)
									GAME.NPCS.lover.flee();
							}, 30000);
							
						}
						// Opened door after alarming and after lover has hidden
						else if(GAME.EVENTS.phases[0].honeyImHome && GAME.EVENTS.phases[0].loverHasHidden)
						{
							GAME.NPCS.wife.greetLie();								
							//Eventually start talking if not greeted
							setTimeout(function() {
								if(!GAME.EVENTS.phases[GAME.EVENTS.curPhase].greetedWife && 
								!GAME.EVENTS.phases[GAME.EVENTS.curPhase].loverDiscovered)
									GAME.NPCS.wife.talkCasually();
							}, 10000);
						}
						//Here comes Johny
						else if(!GAME.EVENTS.phases[0].honeyImHome) {
							manualEmotions.getout.enableButton(true);
							GAME.NPCS.lover.supriseByDoor();
							GAME.NPCS.wife.supriseByDoor();
							setTimeout(function() {
								if(!GAME.EVENTS.phases[GAME.EVENTS.curPhase].loverFled)
									GAME.NPCS.lover.flee();
							}, 30000);
						}
					}

				}
			});
			
			manualEmotions.tired = Crafty.e('Tired_action').attr({x:349,y:700,w:32,h:32}).enableButton(true);
			manualEmotions.tired.bind('Click',function(){
				if(manualEmotions.tired.enabledButton)
				{
					GAME.EVENTS.phases[GAME.EVENTS.curPhase].emotionsUsed.push('Tired');
					manualEmotions.addEmotion('Tired',3,true,1);
				}
			});
			
			manualEmotions.home = Crafty.e('Home_action').attr({x:220,y:700,w:32,h:32}).enableButton(true);
			manualEmotions.home.bind('Click',function(){
				if(manualEmotions.home.enabledButton)
				{
					GAME.EVENTS.phases[GAME.EVENTS.curPhase].emotionsUsed.push('Home');
					manualEmotions.addEmotion('Home',3,true,1);
					if(GAME.EVENTS.curPhase === 0 && !GAME.EVENTS.phases[0].honeyImHome)
					{
						//Alarm wife and lover
						GAME.NPCS.lover.alarmByHome();
						GAME.NPCS.wife.alarmByHome();
						GAME.EVENTS.phases[0].honeyImHome = true;
					}
				}
			});
			
			manualEmotions.listen = Crafty.e('Listen_action').attr({x:155,y:700,w:32,h:32});
			manualEmotions.listen.bind('Click',function(){
				if(manualEmotions.listen.enabledButton)
				{
					GAME.EVENTS.phases[GAME.EVENTS.curPhase].emotionsUsed.push('Listen');
					manualEmotions.addEmotion('Listen',3,true,1);
					if(GAME.EVENTS.curPhase === 0)
					{						
						GAME.EVENTS.phases[0].listenToDoor = true;
					}
				}
			});
			
			manualEmotions.hello = Crafty.e('Hello_action').attr({x:285,y:700,w:32,h:32});
			manualEmotions.hello.bind('Click',function(){
				if(manualEmotions.hello.enabledButton)
				{
					GAME.EVENTS.phases[GAME.EVENTS.curPhase].emotionsUsed.push('Hello');
					manualEmotions.addEmotion('Hello',3,false,1);
					//Casual greeting after lover has hidden prompts blabla from wife
					if(GAME.EVENTS.curPhase === 1 
					&& GAME.EVENTS.phases[0].honeyImHome 
					&& GAME.EVENTS.phases[0].loverHasHidden
					&& !GAME.EVENTS.phases[1].greetedWife)
					{						
						GAME.EVENTS.phases[1].greetedWife = true;
						GAME.NPCS.wife.talkCasually();
					}
				}
			});
			
			manualEmotions.talk = Crafty.e('Talk_action').attr({x:414,y:700,w:32,h:32}).enableButton(true);
			manualEmotions.talk.bind('Click',function(){
				if(manualEmotions.talk.enabledButton)
				{
					GAME.EVENTS.phases[GAME.EVENTS.curPhase].emotionsUsed.push('Talk');
					manualEmotions.addEmotion('Talk',3,true,1);
					
				}
			});
			
			manualEmotions.gosleep = Crafty.e('Go sleep_action').attr({x:477,y:700,w:32,h:32});
			manualEmotions.gosleep.bind('Click',function(){
				if(manualEmotions.gosleep.enabledButton)
				{
					GAME.EVENTS.phases[GAME.EVENTS.curPhase].wentToSleep = true;
					GAME.NPCS.wife.clearBehaviour();
					GAME.PLAYER.go.animate('Sleeping',-1);
					GAME.PLAYER.go.disableControl();
					//GAME.PLAYER.go.collision([100,0],[110,0],[110,10],[100,10]);
					var sleepPos = GAME.LANDSCAPE.gridToReal(3,5);
					GAME.PLAYER.go.attr({ x: sleepPos[0], y: sleepPos[1],z: 4});					
					setTimeout(function() {	
						GAME.OBJECTS.spotlight.turnOff();
					},2000);
					setTimeout(function() {
						GAME.unloadPlayScene();
						Crafty.scene('scoreBoard'); 
					},4000);
				}
			});
			
			manualEmotions.getout = Crafty.e('Get out_action').attr({x:669,y:700,w:32,h:32});
			manualEmotions.getout.bind('Click',function(){
				if(manualEmotions.getout.enabledButton)
				{
					GAME.EVENTS.phases[GAME.EVENTS.curPhase].emotionsUsed.push('Get out');
					manualEmotions.addEmotion('Get out',3,true,1);
					if(GAME.EVENTS.curPhase === 1 && !GAME.EVENTS.phases[1].sentLoverOut && !GAME.EVENTS.phases[1].loverFled) {
						GAME.EVENTS.phases[1].sentLoverOut = true;
						GAME.NPCS.lover.flee();
					} else if(GAME.EVENTS.curPhase === 1 && !GAME.EVENTS.phases[1].sentWifeOut) {
						GAME.EVENTS.phases[1].sentWifeOut = true;
						manualEmotions.getout.enableButton(false);
						GAME.NPCS.wife.flee();
					}
					
				}
			});
			
		}
	});

}