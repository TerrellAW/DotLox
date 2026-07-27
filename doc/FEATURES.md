<div align="center">

  # Unique Features

</div>

DotLox has a number of features that differ from the original [Lox](https://github.com/munificent/craftinginterpreters).

## Comments

DotLox supports two extra comment styles:

1. Multi-line C-style comments
```c
/*
comment
comment
*/
```

2. Python-style single-line comments
```
# comment
```

### Unix Shebang Compatibility

The Python-style single-line comments were added for compatibility with the Unix shebang feature:

```bash
#!/usr/bin/dotlox

print "Hello, World!";
```

This allows a DotLox script to be run with `./script-name` on Unix and Unix-like operating systems if the `dotlox` binary has been installed with `make install`.

## Concatenation

DotLox allows non-strings to be concatenated into strings, like so:

```bash
print 8 + " is a number.";
```

## Standard Library

DotLox has extra native functions:

### [`read()`](LIBRARY.md#read)

Takes a line of input from the terminal.

### [`readNum()`](LIBRARY.md#readNum)

Takes a numeric line of input from the terminal.

### [`readFile(file)`](LIBRARY.md#readFilefile)

Reads and returns the entire contents of a file.

### [`writeFile(file, text)`](LIBRARY.md#writeFilefile-text)

Outputs given text to a file.

### [`mod(n1, n2)`](LIBRARY.md#modn1-n2)

Applies the modulo operation to two variables.

### [`stringify(n)`](LIBRARY.md#stringifyn)

Converts a non-string into a string.

### [`numberify(val)`](LIBRARY.md#numberifyval)

Converts a numeric string into a number.

### [`isNum(val)`](LIBRARY.md#isNumval)

Checks if a value is numeric.

### Documentation

Information on the entire DotLox Standard Library can be found [here](LIBRARY.md).
