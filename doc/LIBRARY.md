<div align="center">

  # DotLox Standard Library

</div>

The standard library for DotLox contains a number of functions that were implemented natively in C# for better performance and access to necessary features.

## `clock()`

A function which returns the current system time in milliseconds since the Unix epoch.

### Arguments

**Types:** `nil`

This function takes no arguments.

### Returns

**Types:** `number`

This function returns a `number`.

## `read()`

A function which reads a line of input.

### Arguments

**Types:** `nil`

This function takes no arguments.

### Returns

**Types:** `string`

This function returns a `string` of text.

## `readFile()`

A function which reads the entire contents of a file.

### Arguments

**Types:** `string`

This function takes a `string` containing the directory of a file.
The function will notify the user if the given argument is invalid.

### Returns

**Types:** `string`

This function returns a `string` of text.

## `writeFile(file, text)`

A function which writes the given text to the specified file.

> [!WARNING]
> Will overwrite the file's contents if it exists and isn't empty.

### Arguments

**Types:** `string`

This function takes a `string` containing the directory of a file and a `string` containing the text to output to the file.
The function will notify the user if any given arguments are invalid.

### Returns

**Types:** `boolean`

This function returns a `boolean` depending on whether it successfully wrote the output to a file. 

## `mod(n1, n2)`

A function which applies the modulo operation to two arguments.

### Arguments

**Types:** `number`, `string`

This function takes two arguments. It was made with numbers in mind, but it can also handle strings.

### Returns

**Types:** `number`, `NaN`

If both arguments are numbers or strings that contain only numbers, the function will return a `number`.
If one or both arguments are strings that contain non-numbers the function will return `NaN`.
