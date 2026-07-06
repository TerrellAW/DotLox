<div align="center">

  # DotLox

</div>

DotLox is a C# implementation of the tree-walk interpreter for Robert Nystrom's Lox programming language which is explained in his book; [Crafting Interpreters](https://craftinginterpreters.com).

## Build

DotLox requires the [.NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) SDK or newer to build. 
It can be built directly with `dotnet` or with `make`. There are two build configurations.

### Debug

With `make`, run:

```bash
make
```

With `dotnet`, run:

```bash
dotnet build DotLox.csproj
```

### Release

With `make`, run:

```bash
make release
```

With `dotnet`, run:

```bash
dotnet build DotLox.csproj --configuration Release
```

## Installation

After building the interpreter it can be installed using `make`.

```bash
make install
```

This command will move the binary from `bin/` to `/usr/bin/`.

## Usage

DotLox requires the [.NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) runtime or newer to run.

DotLox has two modes, command line prompt and file execution.

### Command Line Prompt

Simply run the binary from your terminal:

```bash
dotlox
```

You will be given a prompt like so `> ` and can type in and run valid Lox code.

This program can be run with [rlwrap](https://github.com/hanslub42/rlwrap) like so:

```bash
rlwrap -a dotlox
```

> [!WARNING]
> The first line in the REPL will be garbled with `rlwrap`

There is also a `make` target which can automatically run a build that is in the `bin/` directory:

```bash
make test
```

### File Execution

The file you wish to run can be passed to `dotlox` to run it in file execution mode.

```bash
dotlox path/to/file.lox
```

## Syntax

The grammar can be found [here](doc/GRAMMAR.md). Further information on the Lox programming language can be found [here](https://github.com/munificent/craftinginterpreters).

## References

Robert Nystrom's book, [Crafting Interpreters](https://craftinginterpreters.com).

Nrosa01's implementation, [CSLox](https://github.com/Nrosa01/CSLox), sometimes used for comparison.
