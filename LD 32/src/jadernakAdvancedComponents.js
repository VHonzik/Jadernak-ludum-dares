// Component which improves crafty 2D component
Crafty.c('jac2D', {  
  init: function() {
		this.requires('2D');
		this.parent = null;
	},
  // returns current center as an object with "x" and "y" properties
  center: function() {
    return {x:this._x + this._w/2.0, y: this._y + this._h/2.0};
  },
  // improves crafty attr with following attributes:
  //    rtvX, rtvY, rtvW, rtvH - sets x,y,w,h, relative to the viewport size 
  //    cX, cY - positions entity so its center has x:cX and y:cY
  //    rtpX, rtpY, rtpcX, rptcY, rtpElement, rtpFollow - sets x,y (or cx, cy) relative to x,y (or cx, cy) of rtpElement and sticks to it if rtpFollow==true
  //    cRotation - rotate around center
  jacAttr: function(attributes) {
    var craftyAttributes = {};
    
    // backward compatible with attr
    if(typeof attributes.x !== 'undefined') 
      craftyAttributes.x = attributes.x;
    if(typeof attributes.y !== 'undefined') 
      craftyAttributes.y = attributes.y;
    if(typeof attributes.alpha !== 'undefined') 
      craftyAttributes.alpha = attributes.alpha;
    if(typeof attributes.w !== 'undefined') 
      craftyAttributes.w = attributes.w;
    if(typeof attributes.h !== 'undefined') 
      craftyAttributes.h = attributes.h;
    if(typeof attributes.rotation !== 'undefined') 
      craftyAttributes.rotation = attributes.rotation;
    
    // relative to the viewport
    if(typeof attributes.rtvX !== 'undefined') 
      craftyAttributes.x = attributes.rtvX * Crafty.viewport._width;
    if(typeof attributes.rtvY !== 'undefined') 
      craftyAttributes.y = attributes.rtvY * Crafty.viewport._height;
    if(typeof attributes.rtvW !== 'undefined') 
      craftyAttributes.w = attributes.rtvW * Crafty.viewport._width;
    if(typeof attributes.rtvH !== 'undefined') 
      craftyAttributes.h = attributes.rtvH * Crafty.viewport._height;
    
    // center
    if(typeof attributes.cX !== 'undefined') 
      craftyAttributes.x = attributes.cX - this._w * 0.5;
    if(typeof attributes.cY !== 'undefined') 
      craftyAttributes.y = attributes.cY - this._h * 0.5;
    
    // relative to element
    if(typeof attributes.rtpElement !== 'undefined')
    {
      this.parent = attributes.rtpElement;
      this.makeRelativeToElement(attributes);
      
      // follow ?
      if(typeof attributes.rtpFollow !== 'undefined' && attributes.rtpFollow===true)
      {
        		var makeRelToEleFun = $.proxy(this.makeRelativeToElement,this,attributes);		
            this.parent.bind('Move',makeRelToEleFun);
      }
    }

    // rotation around center
    if(typeof attributes.cRotation !== 'undefined')
    {
      this.origin('center');
      craftyAttributes.rotation = attributes.cRotation;
    }
    
    // finally apply crafty attr
    this.attr(craftyAttributes);
    
    return this;
  },
  // helper function for jacAttr relative to element
  makeRelativeToElement: function(attributes) {
    var jacAttributes = {};
    if(this.parent != null && typeof attributes.rtpX !== 'undefined') 
      jacAttributes.x = this.parent._x + attributes.rtpX;
    if(this.parent != null && typeof attributes.rtpY !== 'undefined') 
      jacAttributes.y = this.parent._y + attributes.rtpY;
    if(this.parent != null && typeof attributes.rtpcX !== 'undefined') 
      jacAttributes.cX = this.parent._x + this.parent._w / 2.0 + attributes.rtpcX;
    if(this.parent != null && typeof attributes.rtpcY !== 'undefined') 
      jacAttributes.cY = this.parent._y +  this.parent._h / 2.0 + attributes.rtpcY;
    this.jacAttr(jacAttributes);
  }
});

// Component which improves crafty SpriteAnimation component
Crafty.c('jacAnimation', {
  init: function() {
		this.requires('SpriteAnimation');
    this.animQueue = [];
    this.bind('AnimationEnd', $.proxy(this.endOfAnimation,this))
	},
  // Prepare for reeling animations from animation sheet
  //    spriteName - name of the sprite under which was animation sheet loaded
  //    rws - number of rows
  //    cols - number of columns
  initFromAnimSheet: function(spriteName,rws,cols) {
    this.requires(spriteName);
    this.columns = cols;
    this.rows = rws;
    return this;
  },
  // Reel animation from the sheet initiated in initFromAnimSheet
  //    animationName - name of the animation
  //    startFrame - indexes of the the first frame of the animation in format [column,row]
  //    frameCount - number of frames including the startFrame
  //    nextFrame - object which will contain indexes of the next-to-the-last frame in format [column,row]
  jacReel: function(animationName, startFrame, frameCount) {
    this.nextFrame = {};
    var duration = (frameCount > 1) ? (frameCount - 1) * GLOBAL.msPerAnimFrame : GLOBAL.msPerAnimFrame;
		var frames = [startFrame];
		var frameCounter = frameCount-1;
		var i = this.clampI(startFrame[0]+1);
		var j = this.clampJ(startFrame[1], startFrame[0]+1);
    
    this.nextFrame.i = i;
    this.nextFrame.j = j; 
    
		while(frameCounter>0)
		{
			frames.push([i,j]);
			frameCounter -= 1;   
			
      j = this.clampJ(j,i+1);
      i = this.clampI(i+1);
      
      this.nextFrame.i = i;
      this.nextFrame.j = j;   
		}
    
		this.reel(animationName,duration,frames);
		
		return this;
  },
  // Reel the whole sheet
  //    animations - array of animations definitions in the order they are in the sheet
  //        animation definition is an object with 'animationName' and 'frameCount' properties
  jacReelSheet: function(animations) {
    this.nextFrame = {i:0,j:0};
    for(var i=0;i<animations.length;i++)
    {
      var animDef = animations[i];
      this.jacReel(animDef.animationName,[this.nextFrame.i,this.nextFrame.j],animDef.frameCount);
    }
    return this;
  },
  
  // Helper methods for index clamping
  clampJ : function(currentJ, currentI) {
    var res = currentJ;
    if(currentI>this.columns-1) {
      res += 1;
    }
    res = Math.min(this.rows-1,res);
    
    return res;
  },
  clampI: function(currentI) {
    return currentI % this.columns;
  },
  
  endOfAnimation: function() {
    this.isAnimating = false;
    this.processQueue();
  },  
  // Adds animation to the queue
  //    animation - name of the animation
  jacQueueAnimation: function(animation, loops) {
    loops = loops || 1;
    this.animQueue.push({type:'anim',name:animation,loop: loops});
    if(this.animQueue.length == 1 && !this.isAnimating)
      this.processQueue();
    return this;
  },
  // Adds general callback to the queue
  //    callback - function to call, it will have entity as this context
  jacQueueCallback: function(callback) {
    this.animQueue.push({type:'fun',fun:$.proxy(callback,this)});
    if(this.animQueue.length == 1 && !this.isAnimating)
      this.processQueue();
    return this;
  },
  
  // Process animation queue, will hook up itself until animation queue is not empty
  processQueue: function() {
    if(this.animQueue.length > 0) {
      var element = this.animQueue.shift();
      if(element.type==='anim') {
        this.animate(element.name,element.loop);
        this.isAnimating = true;
      }
      else if(element.type==='fun') {
        element.fun();
        if(this.animQueue.length > 0 && this.processQueue !== undefined)
            this.processQueue();
      }
      else {
        console.error('Unknown animation queue type.');
      }
    }
  }
  
});