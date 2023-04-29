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
///     Automatically handles the common aspects of character decoding, such as surrogate
///     decomposition, thereby allowing implementers to worry about specific concerns only.
/// </summary>
public abstract class RuneDecoder : Decoder
{
    /// <summary>
    ///     Calculates the number of characters produced by decoding a sequence of bytes from
    ///     the specified byte array.
    /// </summary>
    /// <param name="bytes">
    ///     The byte array containing the sequence of bytes to decode.
    /// </param>
    /// <param name="index">
    ///     The index of the first byte to decode.
    /// </param>
    /// <param name="count">
    ///     The number of bytes to decode.
    /// </param>
    /// <returns>
    ///     The number of characters produced by decoding the specified sequence of bytes and
    ///     any bytes in the internal buffer.
    /// </returns>
    public override int GetCharCount(byte[] bytes, int index, int count)
    {
        var returnValue = 0;
        var end = index + count;
        var currentIndex = index;

        while (currentIndex < end)
        {
            bool? isBasic;

            currentIndex += IsScalarValueBasic(bytes, currentIndex, count, out isBasic);

            if (isBasic is not null)
                returnValue += (isBasic == true) ? 1 : 2;
        }

        return returnValue;
    }

    /// <summary>
    ///     Decodes a sequence of bytes from the specified byte array and any bytes in the
    ///     internal buffer into the specified character array.
    /// </summary>
    /// <param name="bytes">
    ///     The byte array containing the sequence of bytes to decode.
    /// </param>
    /// <param name="byteIndex">
    ///     The index of the first byte to decode.
    /// </param>
    /// <param name="byteCount">
    ///     The number of bytes to decode.
    /// </param>
    /// <param name="chars">
    ///     The character array to contain the resulting set of characters.
    /// </param>
    /// <param name="charIndex">
    ///     The index at which to start writing the resulting set of characters.
    /// </param>
    /// <returns>
    ///     The actual number of characters written into <pre>chars</pre>.
    /// </returns>
    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars
        , int charIndex)
    {
        int byteEnd = byteIndex + byteCount;
        int lastByteIndex = byteIndex;
        int currentByteIndex = byteIndex;
        int currentCharIndex = charIndex;

        while (currentByteIndex < byteEnd)
        {
            int? scalarValue;

            currentByteIndex += ReadScalarValue(bytes, currentByteIndex, byteCount
                , out scalarValue);

            if (scalarValue is int _scalarValue)
            {
                if ((0 <= _scalarValue && _scalarValue <= 0xD7FF)
                    || (0xE000 <= _scalarValue && _scalarValue <= 0xFFFF))
                {
                    chars[currentCharIndex++] = (char)_scalarValue;
                }
                else if (0x010000 <= scalarValue && scalarValue <= 0x10FFFF)
                {
                    chars[currentCharIndex++] = HighSurrogate(_scalarValue);
                    chars[currentCharIndex++] = LowSurrogate(_scalarValue);
                }
                else
                {
                    throw new NotImplementedException("DecoderFallback not handled yet.");
                }
            }

            lastByteIndex = currentByteIndex;
        }

        return currentCharIndex - charIndex;
    }

    /// <summary>
    ///     Determines if a Unicode scalar value is represented by a single <pre>char</pre> or
    ///     if it requires two and returns that result in an output parameter.
    /// </summary>
    /// <param name="bytes">
    ///     The byte array containing the scalar value to check.
    /// </param>
    /// <param name="index">
    ///     The index of the first byte of the encoded scalar value.
    /// </param>
    /// <param name="limit">
    ///     The maximum number of bytes after the <pre>index</pre> to check.
    /// </param>
    /// <param name="isBasic">
    ///     <ul>
    ///         <li>
    ///             <pre>true</pre> — The checked scalar value is represented by a single
    ///             <pre>char</pre> value.
    ///         </li>
    ///         <li>
    ///             <pre>false</pre> — The checked scalar value is represented by two
    ///             <pre>char</pre> values.
    ///         </li>
    ///         <li>
    ///             <pre>null</pre> — The complete scalar value could not be measured before
    ///             exceeding <pre>limit</pre>.
    ///         </li>
    ///     </ul>
    /// </param>
    /// <returns>
    ///     The number of bytes read to check the scalar value.
    /// </returns>
    protected virtual int IsScalarValueBasic(byte[] bytes, int index, int limit
        , out bool? isBasic)
    {
        int? scalarValue;
        int bytesRead = ReadScalarValue(bytes, index, limit, out scalarValue);

        if (scalarValue is null)
        {
            isBasic = null;
        }
        else if ((0 <= scalarValue && scalarValue <= 0xD7FF)
            || (0xE000 <= scalarValue && scalarValue <= 0xFFFF))
        {
            isBasic = true;
        }
        else if (0x010000 <= scalarValue && scalarValue <= 0x10FFFF)
        {
            isBasic = false;
        }
        else
        {
            throw new NotImplementedException("DecoderFallback not handled yet.");
        }

        return bytesRead;
    }

    /// <summary>
    ///     Decodes the next encoded scalar value from a byte array and returns the result in an
    ///     output parameter.
    /// </summary>
    /// <param name="bytes">
    ///     The byte array containing the sequence of bytes to decode.
    /// </param>
    /// <param name="index">
    ///     The index of the first byte of the encoded scalar value.
    /// </param>
    /// <param name="limit">
    ///     The maximum number of bytes after the <pre>index</pre> to decode.
    /// </param>
    /// <param name="scalarValue">
    ///     The next decoded Unicode scalar value, or <pre>null</pre> if the complete scalar
    ///     value could not be decoded before exceeding <pre>limit</pre>.
    /// </param>
    /// <returns>
    /// </returns>
    protected abstract int ReadScalarValue(byte[] bytes, int index, int limit
        , out int? scalarValue);

    private static char HighSurrogate(int scalarValue) =>
        (char)((((uint)scalarValue - 0x10000U) >> 10) + 0xD800);

    private static char LowSurrogate(int scalarValue) =>
        (char)(((scalarValue - 0x10000) & 0x3FF) + 0xDC00);
}
