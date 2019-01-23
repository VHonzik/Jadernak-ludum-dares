var CAMPAING_INTRO = {
  'k0':{
    type: 'hintText',
    value: 'You name is Lar Philocrius Slabio. You are a young nobleman of Philocrate family.\n\n',
    next: 'k1'
  },
  'k1':{
    type: 'personText',
    name: 'Marcus Philocrius Slabio',
    value: 'My son. It is time... Will you allow your father to say a few words?\n\n',
    next: 'k2'
  },  
  'k2':{
    type: 'choice',
    choices: [
      {
        value: 'agree',
        next: 'k200'
      },
      {
        value: 'tease',
        next: 'k210'
      },     
    ],
    hint: 'Answer with \'agree\' or \'tease\'. You can use tab for auto-completion.\n'+
    '  \'agree\': \'Yes father.\'\n'+
    '  \'tease\': \'If you must father.\'',
    repeatedhint: 'Answer with \'agree\' or \'tease\'.'
  },
  'k200':{
    type: 'personText',
    name: '\nLar Philocrius Slabio',
    value: 'Yes father.\n\n',
    next: 'k3'
  },
  'k210':{
    type: 'personText',
    name: '\nLar Philocrius Slabio',
    value: 'If you must father.\n\n',
    next: 'k3'
  },
  'k3':{
    type: 'personText',
    name: 'Marcus Philocrius Slabio',
    value: 'Opesia knows no mercy. It\'s as if the planet itself does not wish for our presence there.\n'+
    'I know you are eager to prove your worth but don\'t be reckless on your Primera. Extract sooner rather than later.\n\n',   
    next: 'k4'
  },
  'k4':{
    type: 'hintText',
    value: 'You are about the undergo the Rite of Primera, first deployment of Mobile Mining Station (MMS) under\n'+
    'a remote control of unexperienced young nobleman on the hostile planet of Opesia.\n'+
    'Numerous families compete with each other in a mining race of valuable minerals.\n\n',
    next: 'k5'
  },
  'k5':{
    type: 'choice',
    choices: [
      {
        value: 'continue',
        next: 'k6'
      },  
    ],
    hint: 'Advance conversation with \'continue\'. You can use tab for auto-completion.\n',
    repeatedhint: 'Continue with \'continue\'.\n'
  },
  'k6':{
    type: 'personText',
    name: '\nMarcus Philocrius Slabio',
    value: 'Expect obstacles at every corner. Don\'t underestimate the Savages.\n'+
    'They may be weak but they are reckless and plentiful.\n'+
    'And ALWAYS keep an eye on your Power level.\n\n',   
    next: 'k7'
  },  
  'k7':{
    type: 'hintText',
    value: 'Your primary resource is Power. Your MMS will be deployed from orbit with a limited Power supply.\n'+
    'Almost all actions cost Power, including act of Extraction when your MMS will return to you on orbit.\n'+
    'Failure to Extract before running out of Power will result of losing MMS and all of its contents.\n'+
    'The planet is inhabited by hostile primitive race collectively called the Savages.\n'+
    'They are naturally attracted to Power spending and will attempt to destroy your MMS.\n\n',
    next: 'k8'
  },
  'k8':{
    type: 'choice',
    choices: [
      {
        value: 'continue',
        next: 'k9'
      },  
    ],
    hint: 'Advance conversation with \'continue\'. You can use tab for auto-completion.\n',
    repeatedhint: 'Continue with \'continue\'.\n'
  },
  'k9':{
    type: 'personText',
    name: '\nMarcus Philocrius Slabio',
    value: 'Good luck son. Is Laria with us?\n\n',   
    next: 'k10'
  },
  'k10':{
    type: 'personText',
    name: 'Laria',
    value: 'Yes Marcus, I am here.\n\n',   
    next: 'k11'
  },
  'k11':{
    type: 'hintText',
    value: 'Laria is your personal AI and through her you will be controlling the MMS.\n\n',
  },  
// 'And ALWAYS keep an eye on your power level.\n\n',
  //Opesia
  // 'k120':{
  //   type: 'narratorText',
  //   value: 'Well, maybe another time \'ey?\n',
  //   next: 'k121'
  // },
};