GAME.LANDSCAPE.init = function() {
	this.tileSets[0] = new this.TileSet('art/tileset.png',GAME.LANDSCAPE.gridSize,2,3);	
	Crafty.c('Obstacle', {
		init: function() {
			//this.requires('2D, Collision, WiredHitBox, DebugCanvas');
			this.requires('2D, Collision');
		},
		rectangle: function(ex,ey,ew,eh) {
			this.attr({x:Math.max(0,ex),y:Math.max(0,ey),w:ew,h:eh}).collision();
			return this;
		}
	});	
}

GAME.LANDSCAPE.TileSet = function(imageFile,tileW,rows,clms) {
	var mapping = {}
	this.image = imageFile;
	this.tiles = rows*clms;
	for (var i = 0; i < rows; i++) {
		for (var j=0; j < clms; j++) {
			mapping[imageFile+(i*clms+j).toString()] = [j,i];
		}	   
	}
	
	Crafty.sprite(tileW, imageFile, mapping);
	
	for (var i = 0; i < rows; i++) {
		for (var j=0; j < clms; j++) {
			var tilename = imageFile+(i*clms+j).toString()
			Crafty.c('Tile'+tilename, {
				tname: tilename,
				init: function() {
					this.requires('2D, DOM, '+this.tname);
					return this;
				}
			});
		}	   
	}
}

GAME.LANDSCAPE.TileSet.prototype.getTile = function(index) {
	if(index>=0 && index<this.tiles) {
		return 'Tile'+this.image+index.toString();
	}
	else {
		console.log("Wrong index "+index.toString()+" to tile set "+this.image);
	}
};

GAME.LANDSCAPE.gridToReal = function(i,j) {
	return [i*GAME.LANDSCAPE.gridSize+1,j*GAME.LANDSCAPE.gridSize+1];
}

GAME.LANDSCAPE.realToGrid = function(x,y) {
	return [Math.floor(x/GAME.LANDSCAPE.gridSize+1),Math.floor(y/GAME.LANDSCAPE.gridSize+1)];
}

GAME.LANDSCAPE.initMap = function(mapIndex) {
	var map = GAME.LANDSCAPE.maps[mapIndex];
	var context = {"width":map.width,"height":map.height,"tileset":map.tileset};
	GAME.LANDSCAPE.allEntities = [];
	
	function processLayer(element, index, array) {
		if(element.type == "tilelayer"){
			for(i=0;i<this.height;i++) {
				for(j=0;j<this.width;j++) {
					var tid = element.data[i*this.width+j]-1;
					var xy = GAME.LANDSCAPE.gridToReal(i,j);
					if(tid>=0)
					{
						GAME.LANDSCAPE.allEntities.push(Crafty.e(GAME.LANDSCAPE.tileSets[this.tileset].getTile(tid))
							.attr({ x: xy[1], y: xy[0], w: GAME.LANDSCAPE.gridSize, h: GAME.LANDSCAPE.gridSize, z: element.z}));
					}
				}
			}
		}
		else if(element.type == "objectgroup") {
			for(i=0;i<element.objects.length;i++) {
				GAME.LANDSCAPE.allEntities.push(Crafty.e("Obstacle").rectangle(element.objects[i].x,element.objects[i].y,
				element.objects[i].width,element.objects[i].height));
			}
		}
		
		
	}
	map.layers.forEach(processLayer,context);
}