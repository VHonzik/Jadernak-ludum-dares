GAME = {
	start: function() {
		//Crafty.init(window.innerWidth * 0.9, window.innerHeight * 0.9,'crafty-stage');
		Crafty.init(1024, 768,'crafty-stage');
		Crafty.background('#000000');
		Crafty.scene('loadingScene');
	},
	LANDSCAPE: {
		gridSize: 64,
		tileSets: [],
		maps: {}
	},
	PLAYER: {},
	CAMERA: {},
	NPCS: {},
	OBJECTS: {},
	EMOTIONS: {},
	EVENTS: {
		curPhase: 0,
		phases: [ 
			{
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
				sentWifeOut: false,
				left: false,
			}
		]
	},
}

GAME.init = function() {
	GAME.CAMERA.init();
	GAME.PLAYER.init();
	GAME.LANDSCAPE.init();
	GAME.NPCS.init();
	GAME.OBJECTS.init();
	GAME.EMOTIONS.init();
};

GAME.unloadPlayScene = function() {
	GAME.PLAYER.go.destroy();
	GAME.NPCS.lover.destroy();
	GAME.NPCS.wife.destroy();
	GAME.OBJECTS.spotlight.destroy();
	GAME.OBJECTS.door.destroy();
	GAME.OBJECTS.bed[0].destroy();
	GAME.OBJECTS.bed[1].destroy();
	GAME.OBJECTS.closet[0].destroy();
	GAME.OBJECTS.closet[1].destroy();
	for(i=0;i<GAME.LANDSCAPE.allEntities.length;i++) {
		GAME.LANDSCAPE.allEntities[i].destroy();
	}
}

//Camera
GAME.CAMERA.init = function() {
	Crafty.viewport.clampToEntities = false;
	Crafty.viewport.scale(4);
}

GAME.CAMERA.followEntity = function(entity) {
	GAME.CAMERA.reset();
	Crafty.viewport.centerOn(entity);
	Crafty.viewport.follow(entity, 0, 0);
}

GAME.CAMERA.relativeCoords = function(rx,ry,rw,rh) {
	rw = rw || 0;
	rh = rh || 0;
	return {x:rx*Crafty.viewport.width - rw/2, y:ry*Crafty.viewport.height - rh/2, w:rw, h:rh};	
}

GAME.CAMERA.reset = function() {
	Crafty.viewport.mouselook(false);
	Crafty.trigger("StopCamera");
}