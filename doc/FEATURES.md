<div align="center">

  # Unique Features

</div>

DotLox currently has two additional features that make it stand out from the original Lox:

1. Multi-line C-style comments
```
/*
comment
comment
*/
```

2. Python style single-line comment
```
# comment
```

## Unix Shebang Compatibility

The Python style single-line comment was added for compatibility with the Unix shebang feature:

```
#!/usr/bin/dotlox

print "Hello, World!";
```

This allows a DotLox script to be run with `./script-name` on Unix and Unix-like operating systems if the `dotlox` binary has been installed with `make install`.
