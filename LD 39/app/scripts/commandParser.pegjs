Command "command"
 = cmdfs : (CommandBody _ Argument* Flag*) { return [cmdfs[0],cmdfs[2],cmdfs[3]]; } / CommandBody

Flag
	= fullflag : ('-' Word  _) { return fullflag[1]; }
    
Argument
	= arg : (Word _) { return arg[0]; }
    
Word
	=[a-zA-Z]+ { return text(); }

CommandBody
 = body : [a-z]+ { return body.join("") }

_ "whitespace"
  = [ \t\n\r]*
              