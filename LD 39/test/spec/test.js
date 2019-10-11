(function () {
  'use strict';

  describe('Command parser parsing of', function () {
    describe('trivial command', function(){
      it('works', function () {
        var result = window.CMDPARSER.parse('command');
        expect(result).to.deep.equal(['command',[],[]]);
      });
      it('works with trailing whitespace', function () {
        var result = window.CMDPARSER.parse('command ');
        expect(result).to.deep.equal(['command',[],[]]);
      });
      it('fails with leading whitespace', function () {
        expect(window.CMDPARSER.parse.bind(' command')).to.throw();
      });
    });
    describe('command with arguments', function(){
      it('works', function () {
        var result = window.CMDPARSER.parse('command argument');
        expect(result).to.deep.equal(['command',['argument'],[]]);
      });
      it('works with extra spaces', function () {
        var result = window.CMDPARSER.parse('command   argument ');
        expect(result).to.deep.equal(['command',['argument'],[]]);
      });
      it('works with capital letters in argument', function () {
        var result = window.CMDPARSER.parse('command ArguMenT');
        expect(result).to.deep.equal(['command',['ArguMenT'],[]]);
      });
      it('works with multiple arguments', function () {
        var result = window.CMDPARSER.parse('command argument argumenttwo');
        expect(result).to.deep.equal(['command',['argument','argumenttwo'],[]]);
      });       
    });

    describe('command with flags', function(){
      it('works', function () {
        var result = window.CMDPARSER.parse('command -flag');
        expect(result).to.deep.equal(['command',[],['flag']]);
      });
      it('works with extra spaces', function () {
        var result = window.CMDPARSER.parse('command   -flag ');
        expect(result).to.deep.equal(['command',[],['flag']]);
      });
      it('works with capital letters in flag', function () {
        var result = window.CMDPARSER.parse('command -FlAg');
        expect(result).to.deep.equal(['command',[],['FlAg']]);
      });
      it('works with multiple flags', function () {
        var result = window.CMDPARSER.parse('command -flagone -flagtwo');
        expect(result).to.deep.equal(['command',[],['flagone','flagtwo']]);
      });       
    });
    
    describe('complicated command', function(){
      it('works', function () {
        var result = window.CMDPARSER.parse('command argument argumenttwo -flag -flagtwo');
        expect(result).to.deep.equal(['command',['argument','argumenttwo'],['flag', 'flagtwo']]);
      });
      it('fails when arguments follow flags', function () {
        expect(window.CMDPARSER.parse.bind('command -flag argument')).to.throw();
      });
    }); 

  });

})();
