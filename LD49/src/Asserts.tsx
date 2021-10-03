import Gash, { Colored, Line, red600 } from "web-gash";

function Assert(condition: boolean, message?: string): void {
  if (!condition) {
    if (process.env.NODE_ENV === 'development') {
      Gash.writeLine(<Line><Colored foreground={red600}>Assertion failed: {message || 'No message provided'}</Colored></Line>)
    } else if (process.env.NODE_ENV === 'test') {
      console.log(`Assertion failed: ${message || 'No message provided'}`)
    }
  }
}


export function AssertTrue(condition: boolean, message?: string): void {
  Assert(condition, message);
}

export function AssertDefined(value: any, message?: string) {
  Assert(value !== undefined, message);
}

