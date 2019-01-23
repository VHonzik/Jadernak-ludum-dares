var GAMEMANAGER = (function (my) {

    my.hint_style = function(text) {
      return '[[;#999999;black]'+text+']';
    }

    my.print = function(text) {
      my.terminal.echo(text);
    }

    my.currentAutoComplete = [];

    my.autocomplete = function(command, callback) {
      callback(my.currentAutoComplete);
    }

    my.expect = function(hint, repeated_hint, values, callback) {
      my.print(hint);
      my.currentAutoComplete = values;
      my.terminal.push(function(command){
        var found = false;
        var mcommand = command.trim();
        for (var i = 0, len = values.length; i < len; i++) {
          if(values[i] === mcommand)
          {
            found = true;
            my.terminal.pop();
            my.currentAutoComplete = [];
            callback(mcommand);
          }
        }

        if(found === false)
        {
          my.print(repeated_hint);
        }
      },{prompt: '>'});
    }

    my.processLine = function(command) {
      //COMMANDMANAGER.processLine(command);
    }

    my.StevenScored = function(amount) {
      my.StevenScore += amount;
      my.print(my.hint_style('Your understanding of Steven has increased.\n\n'));
    }

    my.start = function() {
      my.StevenScore = 0;
    }

    return my;
}(GAMEMANAGER || {}));