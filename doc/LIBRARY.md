<div align="center">

  # DotLox Standard Library

</div>

The standard library for DotLox contains a number of functions that were implemented natively in C# for better performance and access to necessary features.

## `clock()`

A function which returns the current system time in seconds since the Unix epoch.

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

## `readNum()`

A function which reads a numeric line of input.

### Arguments

**Types:** `nil`

This function takes no arguments.

### Returns

**Types:** `number`, `NaN`

This function returns the `number` retrieved from the terminal or `NaN`.
The function will notify the user if the given argument is not a number.

## `readFile(file)`

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

## `stringify(n)`

A function which converts a value into a string.

### Arguments

**Types:** any

This function takes one argument.

### Returns

**Types:** `string`

This function returns a `string` representation of the argument.

## `numberify(val)`

A function which converts a string into a number.

### Arguments

**Types:** any

This function takes one argument. It was made with strings in mind but won't crash with other types.

### Returns

**Types:** `number`, `NaN`

This function returns a `number` if given a string containing a numeric value, or `NaN`.

## `isNum(val)`

A function which checks if a value is numeric.

### Arguments

**Types:** any

This function takes one argument.

### Returns

**Types:** `bool`

This function returns `True` if value is numeric, or `False` if it is not.

## `rand(n1, n2)`

A function which returns a random number in the range of argument one and two.

### Arguments

**Types:** `number`, `string`

This function takes two arguments. It was made with numbers in mind, but it can also handle strings.

### Returns

**Types:** `number`, `NaN`

If both arguments are numbers or strings that contain only numbers, the function will return a `number`.
If one or both arguments are strings that contain non-numbers the function will return `NaN`.

## `int(n)`

A function which trims the floating point off of a number.

### Arguments

**Types:** `number`, `string`

This function takes one argument. It was made with numbers in mind and will give an error if a non-numeric value is given.

### Returns

**Types:** `number`, `NaN`

This function returns a `number` if given a string containing a numeric value or a number, or `NaN`.
