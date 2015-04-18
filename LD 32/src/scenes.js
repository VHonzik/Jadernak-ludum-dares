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