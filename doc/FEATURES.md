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

## Standard Library

DotLox has extra native functions:

### [`read()`](LIBRARY.md#read)

Takes a line of input from the terminal.

### [`readFile(file)`](LIBRARY.md#readFile)

Reads and returns the entire contents of a file.

### [`mod(n1, n2)`](LIBRARY.md#modn1-n2)

Applies the modulo operation to two variables.

### Documentation

Information on the entire DotLox Standard Library can be found [here](LIBRARY.md).
