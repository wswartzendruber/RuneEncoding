<!--
    Copyright 2023 William Swartzendruber

    To the extent possible under law, the person who associated CC0 with this file has waived
    all copyright and related or neighboring rights to this file.

    You should have received a copy of the CC0 legalcode along with this work. If not, see
    <http://creativecommons.org/publicdomain/zero/1.0/>.

    SPDX-License-Identifier: CC0-1.0
-->

# RuneEncoding

## Introduction

The RuneEncoding library eases the implementation of custom character encodings. It does this by
sitting as an abstraction layer on top of the `System.Text.Encoder` and `System.Text.Decoder`
classes, handling what are essentially boilerplate concerns to Unicode. Primarily, RuneEncoding
handles surrogate composition and decomposition for the implementor. The `RuneEncoder` and
`RuneDecoder` abstract classes allow implementors to only worry about complete Unicode scalar
values.

While a custom encoding built on top of RuneEncoding does not promise to be as fast or as
performant as an optimized one built directly on top of the native .NET classes, it does promise
to considerably simplify the implementation of such an encoding.

## Example

The code snippet below outlines how to implement the `RuneEncoder` class:

```csharp
```

This next code snippet outlines how to implement the `RuneDecoder` class:

```csharp
```
