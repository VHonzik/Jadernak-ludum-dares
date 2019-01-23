var STORYMANAGER = function (my) {

  my.person_style = function (text) {
    return '[[;#e6e6e6;black]' + text + ']';
  };

  my.person_name = function (name) {
    return '[[b;#e6e6e6;black]' + name + ': ]';
  };

  my.processNarativeItem = function (item, callback) {
    if (item.type === 'personText') {
      GAMEMANAGER.print(my.person_name(item.name) + my.person_style(item.value));
      if (item.next) {
        my.storyQueue.push(my.currentNarative[item.next]);
      }
      callback();
    } else if (item.type === 'hintText') {
      GAMEMANAGER.print(GAMEMANAGER.hint_style(item.value));
      if (item.next) {
        my.storyQueue.push(my.currentNarative[item.next]);
      }
      callback();
    } else if (item.type === 'choice') {
      var choices = item.choices.map(function (x) {
        return x.value;
      });
      GAMEMANAGER.expect(GAMEMANAGER.hint_style(item.hint), GAMEMANAGER.hint_style(item.repeatedhint), choices, function (choice) {
        for (var i = 0, len = item.choices.length; i < len; i++) {
          if (item.choices[i].value === choice) {
            if (item.choices[i].fnc) {
              item.choices[i].fnc(GAMEMANAGER);
            }
            if (item.choices[i].next) {
              my.storyQueue.push(my.currentNarative[item.choices[i].next]);
              break;
            }
          }
        }
        callback();
      });
    }
  };

  my.startNarative = function (narative) {
    my.storyQueue.remove(function (data) {
      return true;
    });
    my.currentNarative = narative;
    my.storyQueue.push(my.currentNarative['k0']);
  };

  my.start = function () {
    my.storyQueue = async.queue(my.processNarativeItem, 1);
    my.startNarative(STORY);
    // my.storyQueue.drain = function() {
    //     GAMEMANAGER.terminal.pause(true);
    // };
  };

  return my;
}(STORYMANAGER || {});
