Crafty.defineScene('loadingScene', function() {
	
    var loadingText = Crafty.e('2D, DOM, Text')
	.attr(GAME.CAMERA.relativeCoords(0.5,0.5,150,30))
	.text('Loading 0%')
	.textFont({ size: '30px', family: 'VT323' })
	.textColor('#FFFFFF').unselectable();

	var emotionsSprites = [];
	for(key in GAME.EMOTIONS.list)
	{
		emotionsSprites.push(GAME.EMOTIONS.list[key].icon);
	}
	
    Crafty.load({ "audio":{ "music": ["art/music.wav", "art/music.mp3", "art/music.ogg"],
	"door": ["art/door.wav", "art/door.mp3", "art/door.ogg"]},
	"images": emotionsSprites.concat(['art/tileset.png','art/lover.png',
	'art/man.png','art/bed.png','art/wall.png','art/door.png',
	'art/wife.png','art/cloud.png','art/ui.png','art/lightoff.png','art/lighton.png',
	'art/ldtimespromo.png','art/ldtimesfinal.png','art/soundicon.png'])}, function() {
		GAME.init();			
		Crafty.scene('introScene');
    },
    function(e) {
      loadingText.text('Loading '+e.percent.toFixed(0)+'%');
    });	
});

Crafty.defineScene('playScene', function() {
	Crafty.background('#000000');
	GAME.LANDSCAPE.initMap(0);
	var playerPos = GAME.LANDSCAPE.gridToReal(13.5,5);
	GAME.PLAYER.go = Crafty.e('PlayerCharacter').attr({ x: playerPos[0], y: playerPos[1]});
	//GAME.CAMERA.followEntity(GAME.PLAYER.go);
	
	var bedPos = GAME.LANDSCAPE.gridToReal(3,5.25);
	GAME.OBJECTS.bed = [
		Crafty.e('BedUp').attr({ x: bedPos[0], y: bedPos[1]}),
		Crafty.e('BedDown').attr({ x: bedPos[0], y: (bedPos[1]+96)})
	];
	
	Crafty.e('UIbar');
	
	var closetPos = GAME.LANDSCAPE.gridToReal(4,2);
	GAME.OBJECTS.closet = [
		Crafty.e('ClosetUp').attr({ x: closetPos[0], y: closetPos[1]}),
		Crafty.e('ClosetDown').attr({ x: closetPos[0], y: (closetPos[1]+64)})
	];
	
	var wallPos = GAME.LANDSCAPE.gridToReal(11,1);
	GAME.OBJECTS.bed = [
		Crafty.e('Wall').attr({ x: wallPos[0], y: wallPos[1]}),
		Crafty.e('Obstacle').rectangle( wallPos[0]+20, wallPos[1]+430,12,180)
	];
	
	var doorPos = GAME.LANDSCAPE.gridToReal(10.4,5.3);
	GAME.OBJECTS.door = Crafty.e('Door').attr({ x: doorPos[0], y: doorPos[1]});

	
	var loverPos = GAME.LANDSCAPE.gridToReal(3,5);
	GAME.NPCS.lover = Crafty.e('LoverCharacter').attr({ x: loverPos[0], y: loverPos[1]});
	GAME.NPCS.lover.inBedLookingAtWife();
	
	var wifePos = GAME.LANDSCAPE.gridToReal(3,5.7);
	GAME.NPCS.wife = Crafty.e('WifeCharacter').attr({ x: wifePos[0], y: wifePos[1]});
	GAME.NPCS.wife.inBedLookingAtLover();
	
	GAME.OBJECTS.spotlight = Crafty.e('Spotlight');
	setTimeout(function() {	GAME.OBJECTS.spotlight.turnOn(); },1500);
	
});

Crafty.defineScene('scoreBoard', function() {
	Crafty.e('FinalImage');
	/*
	name: 'beforeOpeningDoor',
			honeyImHome:false,
			listenToDoor:false,
			loverHasHidden: false,
			left: false,
			emotionsUsed: []
		},
		{
			name: 'enteringRoom',
			greetedWife:false,
			emotionsUsed: [],
			wentToSleep: false,
			sentLoverOut: false,
			loverFled: false,
			loverDiscovered: false,
			left: false,
		}
	*/
	var review = '';
	var p1 = GAME.EVENTS.phases[0];
	var p2 = GAME.EVENTS.phases[1];
	//Left before opening door	
	if(p1.left) {
		if(!p1.listenToDoor) {
			review = ' and ... decided to leave again? While it may have saved his fictional marriage, it certainly left \
			the audience confused.';
			if(p1.honeyImHome) 
				review +=' What\'s even worse is the fact he said Hi and then left. As youngster now say:\'WTF.\'';
			else
				review +=' And I don\'t mean that as a pleasantly confused.'
			review += ' A disappointing start of the evening.';
		}
		
		if(p1.listenToDoor) {
			review = '. Upon listening from behind the doors of the bedroom, Simon realized that he was just cheated on and, \
more importantly, it is time for a drink or two. While I can only imagine what would happen if he \
decided to go in, I certainly can\'t blame him for not doing so. Interesting start of the evening.';		
		}
	}
	else if(p2.wentToSleep) {
		if((p1.honeyImHome && !p1.loverHasHidden) || !p1.honeyImHome) {
			review = ' only to find out he was cheated on.';
			if(!p2.sentWifeOut) {
			review += ' Apparently Simon was playing the God of Apathy, since he couldn\'t care less about his wife cheating on \
him and just went to bed.';
			if(p2.sentLoverOut && p2.emotionsUsed.indexOf('Scream')<0)
				review +=' He politely asked the lover to leave. Then he took lover\'s former place next to his shocked wife. \ After some thinking I must say Simon pulled it off. Improv comedy right there folks.';
			else if(p2.sentLoverOut && p2.emotionsUsed.indexOf('Scream')>=0)
				review +=' Yeah he might have screamed a little but I guess it was just a long day. It worked for some, not \
for me though. Thumbs down.';
			else if(!p2.sentLoverOut && p2.emotionsUsed.indexOf('Scream')>=0)
				review += ' After some brief shouting Simon must have lost inspiration and quickly ended the scene. \
How about asking the lover to leave? That\'s not so hard to come up with, is it? Simon should step up his game.'
			else if(!p2.sentLoverOut && p2.emotionsUsed.indexOf('Scream')<0)
				review += ' My other explanation is that he was playing a person on drugs. Or perhaps he was on drugs. \
Whatever was the case, it wasn\'t entertaining at all.';
			}
			else if(p2.sentWifeOut && !p2.sentLoverOut)
			{
				if(p2.emotionsUsed.indexOf('Scream')>=0 || p2.emotionsUsed.indexOf('Talk')>=0)
					review += 'Simon argued and talked for so long that the lover just saw himself out. Disgusted by his wife \
he orders her to follow the lover and leave. Good flow Simon, me gusta.';
				else
					review += 'I think I will remember Simon\'s \'Maybe you should leave too\' for a long time. In this case \
Simon\'s coldness made the whole situation even more serious.';
			}
			else
			{
				review += ' Simon showed us how macho man does it. Kick everyone out and leave it for tomorrow. ';
				if(p2.emotionsUsed.indexOf('Scream')>=0) {
					review += ' And he has done so in style. Good job Simon.';
				}
				else {
					review += ' I would appreciate a little more emotions. But otherwise nice acting.';
				}
			}
		}else if(p1.honeyImHome && p1.loverHasHidden) {
			if(p1.emotionsUsed.indexOf('Talk')>=0 || p1.emotionsUsed.indexOf('Tired')>=0)
				review = '. Simon kept us entertained in the hall while lover managed to hide in the closet. This would be a \
good set-up for a bad adultery joke but fortunately good acting turned it into an interesting show.';
			else
				review = '. I\'m not sure why Simon spent so much time in the hallway. Foreboding or bad acting? What followed \
was mediocre. ';				
		}		
	}
	// He left in p2
	else {
		review = ' only to find out he was cheated on.';
		if(p1.honeyImHome && p1.loverHasHidden) {
			if(p1.emotionsUsed.indexOf('Talk')>=0 || p1.emotionsUsed.indexOf('Tired')>=0)
				review += ' Simon kept us entertained in the hall while the lover managed to hide in the closet. Fortunately for \
the wife and her lover, Simon\'s character must have forgotten something at work and we have a happy end. \
Barely believable happy end.';
			else
				review += ' Me and many others were confused by Simon\'s lingering in the hallway. Leaving later did not help \
either. Not much else to say about this performance...';
		}
		else if((p1.honeyImHome && !p1.loverHasHidden) || !p1.honeyImHome) {
			if(!p2.sentWifeOut) {
			if(p2.sentLoverOut && p2.emotionsUsed.indexOf('Scream')<0)
				review += ' As much as I respect cool guys in real life they don\'t really belong in a theater. Simon more \
drama next time please.';
			else if(p2.sentLoverOut && p2.emotionsUsed.indexOf('Scream')>=0)
				review += ' If you supplement your boring ordinary life with virtual drama, such as I do, then you would enjoy \
				Simon in this scene. Well done sir.';
			else if(!p2.sentLoverOut && !p2.loverFled && p2.emotionsUsed.indexOf('Scream')>=0)
				review += ' Simon was so furious about the whole situation that he just leaves. I was impressed by his \
perfomance in this scene.';	
			else if(!p2.sentLoverOut && p2.loverFled && p2.emotionsUsed.indexOf('Scream')>=0)
				review += ' Simon was so furious about the whole situation that he forgot there is a strange man in his room. \
Personally I found that weird and funny at the same time.';
			else if(!p2.sentLoverOut && p2.emotionsUsed.indexOf('Scream')<0)
				review += ' Simon kept it cool. Maybe too cool for the audience\'s sake. It\'s hard to develop \
any sort of character in such a short format but I can see where Simon was going with that and for that \
he has my appreciation.';
			else if(!p2.loverFled)
				review += ' Simon went with tried and trusted strategy of leaving quickly. Some imaginary barman will have a \
good costumer tonight. And I had a good start of the night.';
			}
			else if(p2.sentWifeOut && !p2.sentLoverOut)
			{
				if(p2.emotionsUsed.indexOf('Scream')>=0 || p2.emotionsUsed.indexOf('Talk')>=0)
					review += 'Simon argued and talked for so long that the lover just saw himself out. Disgusted by his wife \
he orders her to follow the lover and leave. And then he leaves. Everyones leaves, the end.';
				else
					review += 'The serious situation Simon created by evenetually ordering the wife to leave was somehow \
downplayed by the fact he left shortly after. Above-average performance.';
			}
			else {
				review += ' Simon asks both of the guilties to leave. And then leaves himself. \
That\'s just how he rolls. YEAH!';
			}
		}
	}
	
	review += '<br><br>Next we had Sasha in ...'
	
    var gotext = Crafty.e('2D, DOM, Text')
	.attr({x:400,y:250,w:500,h:400,z:20})
	.text('It was a long night in the Kasprzak theater. Before diving deep into \
	my impressions and conclusions I would like to make a few comments on the new faces emerging in the L.D. acting scene \
	who took part in the last night\'s show.<br><br> In the first scene we had Simon Brimley as a husband who just \
	returned from work'+review)
	.textFont({ size: '18px', family: 'VT323' })
	.textColor('#000000').unselectable();

});

Crafty.defineScene('introScene', function() {
   Crafty.e('IntroImage');
   Crafty.e('SoundTool');
   Crafty.audio.play("music", -1);
});