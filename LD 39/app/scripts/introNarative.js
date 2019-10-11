var MAIN_MENU = {
  'k0':{
    type: 'hintText',
    value: 'Welcome to a text based strategy game made for LD 39:\n\n'+
      'Opesia: Running out of power\n\n',
    next: 'k1'
  },
  'k1':{
    type: 'choice',
    choices: [
      {
        value: 'campaign',
        fnc: function(gm) {
          gm.StartCampaign();
        } 
      },
      {
        value: 'quickplay',
        next: 'k2',
      },     
    ],
    hint: 'You can either play Campaign or Quickplay.\n\n'+
    '  * Campaign is recommended way to experience this game if you have time to spare and patience for story.\n\n'+
    '  * Quickplay is for \'You have one minute of my time, go!\' people. It jumps right in the core gameplay\n'+
    '    and doesn\'t bother with story.\n\n'+
    'Select which mode your would like to play by typing either \'campaign\' or \'quickplay\' (without quotes).\n'+
    'Every time you are presented with choice you can use Tab for auto-completion.\n\n',
    repeatedhint: 'Type \'campaign\' or \'quickplay\'.'
  },
  'k2':{
    type: 'choice',
    choices: [
      {
        value: 'no',
        fnc: function(gm) {
          gm.StartQuickPlay();
        } 
      },
      {
        value: 'yes',
        next: 'k3',
      },     
    ],
    hint: 'Would you like to see a brief \'How to play?\' section?\n'+
    'Answer with \'yes\' or \'no\'. You can use tab for auto-completion.',
    repeatedhint: 'Answer with \'yes\' or \'no\'.'
  },

  'k3':{
    type: 'hintText',
    value: '\nYou control the game through dialogue with Laria, a personal AI.\n'+
      'Your goal is to mine as many minerals as possible via Mobile Mining Station (MMS) before you run out of Power.\n'+
      'Building, researching, extracting, environment effects, random events and hostile Savages all cost Power.\n'+
      'You start with limited Power supply and you must Extract MMS to end the mission and gain mined resources.\n\n',
    next: 'k4'
  },
  'k4':{
    type: 'choice',
    choices: [
      {
        value: 'begin',
        fnc: function(gm) {
          gm.StartQuickPlay();
        } 
      },  
    ],
    hint: 'Start quickplay with \'begin\'. You can use tab for auto-completion.\n',
    repeatedhint: 'Continue with \'begin\'.\n'
  },


}