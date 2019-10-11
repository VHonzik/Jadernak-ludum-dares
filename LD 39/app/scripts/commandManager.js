var COMMANDMANAGER = (function (my) {
  my.tryParseCommand = function(command) {
    var parseResult = [];
    try {
        var parseResult = CMDPARSER.parse(command);
        return { success:true, result: parseResult};  
    }
    catch(e)
    {
      //console.log(e);
      return { success:false };
    }         
  }

  my.commands = [
    {
      name: 'man',
      parse: function(parseResult) {
        if(parseResult.result[0] === this.name)
        {
          if(parseResult.result[1].length == 0) {
            GAMEMANAGER.print(GAMEMANAGER.hint_style('Missing command to display man page for. See ')+my.command_style('man man'));
            return true;
          }

          if(parseResult.result[1].length > 1) {
            GAMEMANAGER.print(GAMEMANAGER.hint_style('Too many arguments to display man page for a command. See ')+my.command_style('man man'));
            return true;
          }

          for (var i = 0, len = my.commands.length; i < len; i++) {
            if(my.commands[i].name == parseResult.result[1][0]) {
              my.commands[i].manpage();
              return true;
            }
          }

          return true;
        }        
      },
      manpage: function() {
        GAMEMANAGER.print(GAMEMANAGER.hint_style('This is a manual page for command ')+my.command_style(this.name));
        GAMEMANAGER.print(GAMEMANAGER.hint_style('DESCRIPTION:'));
        GAMEMANAGER.print('\t'+
          my.command_style(this.name)+
          GAMEMANAGER.hint_style(' simply displays a manual page for all other commands.'));
       
      }
    }
  ]

  my.command_style = function(text) {
    return '[[;#a0c4ff;black]'+text+']';
  }

  my.processLine = function(command) {
    var result = my.tryParseCommand(command);
    if(result.success === true) {
      for (var i = 0, len = my.commands.length; i < len; i++) {
        if(my.commands[i].parse(result)) {
          break;
        }
      }
    }
    else {
      GAMEMANAGER.print(GAMEMANAGER.hint_style('Unexpected text. Try ')+my.command_style('man man'));
    }    
  }

  return my;

}(COMMANDMANAGER || {}));