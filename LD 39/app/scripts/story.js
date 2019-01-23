var STORY = {
  'k0':{
    type: 'personText',
    name: 'Narrator',
    value: 'The incident of Easter, Florida in summer 2055 remains mostly unexplained.\n\n',
    next: 'k1'
  },
  'k1':{
    type: 'personText',
    name: 'Narrator',
    value: 'Our probe into the events can\'t be introduced any better than by quoting Psy.D. Joshua Pravdivy:\n'+
    '"Much like the village itself, the morality of those people seems to run on batteries."\n\n',
    next: 'k2'
  },
  'k2':{
    type: 'personText',
    name: 'Narrator',
    value: 'I have taken the liberty of selecting three well documented participants of the incident:\n'+
    'Stay-at-home dad Steven Bortz, auto mechanic Amber Koch and barman Xavier Torres.\n'+
    'We will start with ...\n\n',
    next: 'k3'
  },
  'k3':{
    type: 'choice',
    choices: [
      {
        value: 'Steven',
        next: 'kS0'
      },
      {
        value: 'Amber',
        next: 'kA0'
      },
      {
        value: 'Xavier',
        next: 'kX0'
      },                
    ],
    hint: 'Select by typing \'Steven\', \'Amber\ or \'Xavier\', without the quotes. You can use Tab for auto-completion.\n',
    repeatedhint: 'Select \'Steven\', \'Amber\ or \'Xavier\'.'     
  },
  'kS0':{
    type: 'personText',
    name: 'Narrator',
    value: 'Steven, 30 years old native from Florida, has seen his share of summer lightning storms.\n'+
    'But what happened in the Thursday afternoon, 29th of July, was something else.\n'+
    'Steven was playing with Autumn when the sky suddenly became dark.\n\n',
    next: 'kS1'
  },
  'kS1':{
    type: 'choice',
    choices: [
      {
        value: 'Autumn',
        fnc: function() {
          GAMEMANAGER.StevenScored(1);
        },
        next: 'kS2'
      },
      {
        value: 'Steven',
        next: 'kS3'
      }              
    ],
    hint: 'Who noticed it first \'Steven\' or \'Autumn\? You can use Tab for auto-completion.\n',
    repeatedhint: 'Select \'Autumn\' or \'Steven\.'     
  },
  'kS2':{
    type: 'personText',
    name: 'Narrator',
    value: 'In Autumn\'s presence reality hardly mattered to Steven. His love for her was unending.\n'+
    'The same thing couldn\'t be said about his wife. \n\n',
    next: 'kS4'
  },
  'kS3':{
    type: 'personText',
    name: 'Narrator',
    value: 'It was in fact Autumn who noticed that the sky got suddenly very dark.\n'+
    'When Steven was with Autumn he had eyes for her and only her. He loved his daughter very much. Unlike his wife.\n\n',
    next: 'kS4'
  },    
  'kS4':{
    type: 'personText',
    name: 'Autumn',
    value: 'Papa, papa look! It\'s night outside.\n\n',
    //next: 'kS4'
  },
}