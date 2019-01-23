jQuery(document).ready(function($) {
  GAMEMANAGER.terminal = $('body').terminal(GAMEMANAGER.processLine, 
    { prompt: '>', name: 'test', greetings: '', tabcompletion: true, completion: function(terminal,value,callback){
      GAMEMANAGER.autocomplete(value, callback);
    } });

  GAMEMANAGER.start();
  STORYMANAGER.start();

});


