Crafty.defineScene("loading", function() {
    Crafty.background('white');
    Crafty.e("2D, DOM, Text")
          .attr({ w: 100, h: 20, x: 750, y: 550 })
          .text("Loading...")
          .textColor("#00000")
          .textFont({ size: '30px', weight: 'bold' });
    GLOBAL.game.loadAssets();
});

Crafty.defineScene("main", function() {
    Crafty.background('white');
    GLOBAL.game.init();
});

Crafty.defineScene("difficulty", function() {
  var easy_met = Crafty.e('2D, DOM, easy_met_sprite, Mouse').attr({x:182,y:161,w:152,h:125});
  easy_met.bind('Click',function() {
    Crafty.scene("main");
    GLOBAL.game.setEasy();
  });
  
  var easy_face = Crafty.e('2D, DOM, easy_face_sprite, Mouse').attr({x:169,y:362,w:118,h:238});
  easy_face.bind('Click',function() {
    Crafty.scene("main");
    GLOBAL.game.setEasy();
  });
  
  var hard_met = Crafty.e('2D, DOM, hard_met_sprite, Mouse').attr({x:557,y:107,w:286,h:234});
  hard_met.bind('Click',function() {
    Crafty.scene("main");
    GLOBAL.game.setHard();
  });
  
  var hard_face = Crafty.e('2D, DOM, hard_face_sprite, Mouse').attr({x:584,y:362,w:118,h:238});
  hard_face.bind('Click',function() {
    Crafty.scene("main");
    GLOBAL.game.setHard();
  });
  
  
});

Crafty.defineScene("endingSucces", function() {
  Crafty.background('white');
  Crafty.e('2D, DOM, building_sprite').attr({x:363,y:230,w:214,h:404});
  var replay = Crafty.e('2D, DOM, replay_sprite, Mouse').attr({x:409,y:45,w:150,h:149});
  replay.bind('Click',function() {
    location.reload();
  });
});

Crafty.defineScene("endingFail", function() {
  Crafty.background('white');
  Crafty.e('2D, DOM, building_destroyed_sprite').attr({x:271,y:318,w:421,h:316});
  var replay = Crafty.e('2D, DOM, replay_sprite, Mouse').attr({x:409,y:45,w:150,h:149});
  replay.bind('Click',function() {
    location.reload();
  });
});