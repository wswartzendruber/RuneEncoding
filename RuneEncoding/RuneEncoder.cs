/*
 * Copyright 2023 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived all
 * copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

using System;
using System.Text;

namespace RuneEncoding;

/// <summary>
///     Automatically handles the common aspects of character encoding, such as surrogate
///     composition, thereby allowing implementers to worry about specific concerns only.
/// </summary>
public abstract class RuneEncoder : Encoder
{
    private char? HighSurrogate = null;

    /// <summary>
    ///     Gets whether or not a pending high surrogate is buffered in the encoder's state.
    /// </summary>
    /// <returns>
    ///     Whether or not a pending high surrogate is buffered in the encoder's state.
    /// </returns>
    public bool Pending
    {
        get
        {
            return HighSurrogate != null;
        }
    }

    /// <summary>
    ///     Calculates the number of bytes produced by encoding a set of characters from the
    ///     specified character array. A parameter indicates whether to flush any trailing high
    ///     surrogate as an encoding error.
    /// </summary>
    /// <param name="chars">
    ///     The character array containing the set of characters to encode.
    /// </param>
    /// <param name="index">
    ///     The index of the first character to encode.
    /// </param>
    /// <param name="count">
    ///     The number of characters to encode.
    /// </param>
    /// <param name="flush">
    ///     <pre>true</pre> to simulate flushing any trailing high surrogate as an encoding
    ///     error; otherwise, <pre>false</pre>.
    /// </param>
    /// <returns>
    ///     The number of bytes produced by encoding the specified characters and any characters
    ///     in the internal buffer.
    /// </returns>
    public override int GetByteCount(char[] chars, int index, int count, bool flush)
    {
        var returnValue = 0;
        var end = index + count;
        char? highSurrogate = HighSurrogate;

        for (int i = index; i < end; i++)
        {
            char value = chars[i];

            if (highSurrogate is char highSurrogateValue)
            {
                if (char.IsLowSurrogate(value))
                {
                    returnValue += ByteCount(char.ConvertToUtf32(highSurrogateValue, value));
                    highSurrogate = null;
                }
                else if (!char.IsSurrogate(value))
                {
                    returnValue += FallbackByteCount(highSurrogateValue, i);
                    highSurrogate = null;
                    returnValue += ByteCount(value);
                }
                else if (char.IsHighSurrogate(value))
                {
                    returnValue += FallbackByteCount(highSurrogateValue, i);
                    highSurrogate = value;
                }
                else
                {
                    throw new InvalidOperationException("Internal state is irrational.");
                }
            }
            else
            {
                if (!char.IsSurrogate(value))
                    returnValue += ByteCount(value);
                else if (char.IsHighSurrogate(value))
                    highSurrogate = value;
                else if (char.IsLowSurrogate(value))
                    returnValue += FallbackByteCount(value, i);
                else
                    throw new InvalidOperationException("Internal state is irrational.");
            }
        }

        if (flush == true && highSurrogate is char highSurrogateValue_)
            returnValue += FallbackByteCount(highSurrogateValue_, end);

        return returnValue;
    }

    /// <summary>
    ///     Encodes any pending high surrogate in the encoder's state followed by a set of
    ///     characters from the specified character array. A parameter indicates whether to
    ///     flush any trailing high surrogate as an encoding error.
    /// </summary>
    /// <param name="chars">
    ///     The character array containing the set of characters to encode.
    /// </param>
    /// <param name="charIndex">
    ///     The index of the first character to encode.
    /// </param>
    /// <param name="charCount">
    ///     The number of characters to encode.
    /// </param>
    /// <param name="bytes">
    ///     The byte array to contain the resulting sequence of bytes.
    /// </param>
    /// <param name="byteIndex">
    ///     The index at which to start writing the resulting sequence of bytes.
    /// </param>
    /// <param name="flush">
    ///     <pre>true</pre> to flush any trailing high surrogate as an encoding error;
    ///     otherwise, <pre>false</pre>.
    /// </param>
    /// <returns>
    ///     The actual number of bytes written into <pre>bytes</pre>.
    /// </returns>
    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes
        , int byteIndex, bool flush)
    {
        var currentByteIndex = byteIndex;
        var charEnd = charIndex + charCount;

        for (int i = charIndex; i < charEnd; i++)
        {
            char value = chars[i];

            if (HighSurrogate is char highSurrogateValue)
            {
                if (char.IsLowSurrogate(value))
                {
                    currentByteIndex += WriteBytes(char.ConvertToUtf32(highSurrogateValue
                        , value), bytes, currentByteIndex);
                    HighSurrogate = null;
                }
                else if (!char.IsSurrogate(value))
                {
                    currentByteIndex += WriteFallbackBytes(highSurrogateValue, charIndex, bytes
                        , currentByteIndex);
                    HighSurrogate = null;
                    currentByteIndex += WriteBytes(value, bytes, currentByteIndex);
                }
                else if (char.IsHighSurrogate(value))
                {
                    currentByteIndex += WriteFallbackBytes(highSurrogateValue, charIndex, bytes
                        , currentByteIndex);
                    HighSurrogate = value;
                }
                else
                {
                    throw new InvalidOperationException("Internal state is irrational.");
                }
            }
            else
            {
                if (!char.IsSurrogate(value))
                {
                    currentByteIndex += WriteBytes(value, bytes, currentByteIndex);
                }
                else if (char.IsHighSurrogate(value))
                {
                    HighSurrogate = value;
                }
                else if (char.IsLowSurrogate(value))
                {
                    currentByteIndex += WriteFallbackBytes(value, charIndex, bytes
                        , currentByteIndex);
                }
                else
                {
                    throw new InvalidOperationException("Internal state is irrational.");
                }
            }
        }

        if (flush == true && HighSurrogate is char highSurrogateValue_)
        {
            currentByteIndex += WriteFallbackBytes(highSurrogateValue_, charEnd, bytes
                , currentByteIndex);
            HighSurrogate = null;
        }

        return currentByteIndex - byteIndex;
    }

    /// <summary>
    ///     Returns the number of bytes needed to encode the specified Unicode scalar value.
    /// </summary>
    /// <param name="scalarValue">
    ///     The Unicode scalar value, guarnanteed to be within one of the following ranges:
    ///     <list>
    ///         <item>U+0000 - U+D799</item>
    ///         <item>U+E000 - U+10FFFF</item>
    ///     </list>
    ///     As such, a surrogate will never be specified, nor will anything outside of Unicode's
    ///     defined maximum code point.
    /// </param>
    /// <returns>
    ///     The number of bytes needed to encode the specified Unicode scalar value.
    /// </returns>
    protected abstract int ByteCount(int scalarValue);

    /// <summary>
    ///     Encodes the specified Unicode scalar value to the provided byte array.
    /// </summary>
    /// <param name="scalarValue">
    ///     The Unicode scalar value, guarnanteed to be within one of the following ranges:
    ///     <list>
    ///         <item>U+0000 - U+D799</item>
    ///         <item>U+E000 - U+10FFFF</item>
    ///     </list>
    ///     As such, a surrogate will never be specified, nor will anything outside of Unicode's
    ///     defined maximum code point.
    /// </param>
    /// <param name="bytes">
    ///     The provided byte array to encode to, which is assumed to be large enough to
    ///     accomodate the complete byte encoding of the scalar value.
    /// </param>
    /// <param name="index">
    ///     The offset within the byte array to encode at.
    /// </param>
    /// <returns>
    ///     The number of bytes used to encode the specified Unicode scalar value.
    /// </returns>
    protected abstract int WriteBytes(int scalarValue, byte[] bytes, int index);

    /// <summary>
    ///     Resets the surrogate state of the encoder; any buffered high surrogate is cleared.
    /// </summary>
    public override void Reset()
    {
        HighSurrogate = null;
        ResetState();
    }

    /// <summary>
    ///     Resets the specific state of the encoding.
    /// </summary>
    protected virtual void ResetState() { }

    private int FallbackByteCount(char charValue, int charIndex)
    {
        var returnValue = 0;
        var fallbackBuffer = Fallback?.CreateFallbackBuffer();

        if (fallbackBuffer is null)
        {
            returnValue += ByteCount(Constants.ReplacementCode);
        }
        else if (fallbackBuffer.Fallback(charValue, charIndex))
        {
            char? highSurrogate = null;

            while (fallbackBuffer.Remaining > 0)
            {
                char value = fallbackBuffer.GetNextChar();

                if (highSurrogate is char highSurrogateValue)
                {
                    if (char.IsLowSurrogate(value))
                    {
                        returnValue += ByteCount(char.ConvertToUtf32(highSurrogateValue
                            , value));
                        highSurrogate = null;
                    }
                    else if (!char.IsSurrogate(value))
                    {
                        returnValue += ByteCount(Constants.ReplacementCode);
                        highSurrogate = null;
                        returnValue += ByteCount(value);
                    }
                    else if (char.IsHighSurrogate(value))
                    {
                        returnValue += ByteCount(Constants.ReplacementCode);
                        highSurrogate = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Internal state is irrational.");
                    }
                }
                else
                {
                    if (!char.IsSurrogate(value))
                        returnValue += ByteCount(value);
                    else if (char.IsHighSurrogate(value))
                        highSurrogate = value;
                    else if (char.IsLowSurrogate(value))
                        returnValue += ByteCount(Constants.ReplacementCode);
                    else
                        throw new InvalidOperationException("Internal state is irrational.");
                }
            }

            if (highSurrogate is not null)
                returnValue += ByteCount(Constants.ReplacementCode);
        }

        return returnValue;
    }

    private int WriteFallbackBytes(char charValue, int charIndex, byte[] bytes, int byteIndex)
    {
        var currentByteIndex = byteIndex;
        var fallbackBuffer = Fallback?.CreateFallbackBuffer();

        if (fallbackBuffer is null)
        {
            currentByteIndex += WriteBytes(Constants.ReplacementCode, bytes, currentByteIndex);
        }
        else if (fallbackBuffer.Fallback(charValue, charIndex))
        {
            char? highSurrogate = null;

            while (fallbackBuffer.Remaining > 0)
            {
                char value = fallbackBuffer.GetNextChar();

                if (highSurrogate is char highSurrogateValue)
                {
                    if (char.IsLowSurrogate(value))
                    {
                        currentByteIndex += WriteBytes(char.ConvertToUtf32(highSurrogateValue
                            , value), bytes, currentByteIndex);
                        highSurrogate = null;
                    }
                    else if (!char.IsSurrogate(value))
                    {
                        currentByteIndex += WriteBytes(Constants.ReplacementCode, bytes
                            , currentByteIndex);
                        highSurrogate = null;
                        currentByteIndex += WriteBytes(value, bytes, currentByteIndex);
                    }
                    else if (char.IsHighSurrogate(value))
                    {
                        currentByteIndex += WriteBytes(Constants.ReplacementCode
                            , bytes, currentByteIndex);
                        highSurrogate = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Internal state is irrational.");
                    }
                }
                else
                {
                    if (!char.IsSurrogate(value))
                    {
                        currentByteIndex += WriteBytes(value, bytes, currentByteIndex);
                    }
                    else if (char.IsHighSurrogate(value))
                    {
                        highSurrogate = value;
                    }
                    else if (char.IsLowSurrogate(value))
                    {
                        currentByteIndex += WriteBytes(Constants.ReplacementCode, bytes
                            , currentByteIndex);
                    }
                    else
                    {
                        throw new InvalidOperationException("Internal state is irrational.");
                    }
                }
            }

            if (highSurrogate is not null)
            {
                currentByteIndex += WriteBytes(Constants.ReplacementCode, bytes
                    , currentByteIndex);
            }
        }

        return currentByteIndex - byteIndex;
    }
}
