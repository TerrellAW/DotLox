<div align="center">

  # DotLox Standard Library

</div>

The standard library for DotLox contains a number of functions that were implemented natively in C# for better performance and access to necessary features.

## `read()`

A function which reads a line of input.

### Arguments

**Types:** `nil`

This function takes no arguments.

### Returns

**Type:** `string`

This function returns a `string` of text.

## `mod(n1, n2)`

A function which applies the modulo operation to two arguments.

### Arguments

**Types:** `number`, `string`

This function takes two arguments. It was made with numbers in mind, but it can also handle strings.

### Returns

**Type:** `number`, `NaN`

If both arguments are numbers or strings that contain only numbers, the function will return a `number`.
If one or both arguments are strings that contain non-numbers the function will return `NaN`.
