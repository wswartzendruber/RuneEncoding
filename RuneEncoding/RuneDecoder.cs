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
    ///     Measures the number of characters that would be needed to encode any pending bytes
    ///     in the decoder state followed by an array of bytes. The decoder state
    ///     <strong>is not</strong> modified.
    /// </summary>
    /// <param name="bytes">
    ///     The array of bytes to measure.
    /// </param>
    /// <param name="index">
    ///     The index of the first byte to measure.
    /// </param>
    /// <param name="count">
    ///     The number of bytes to measure.
    /// </param>
    /// <returns>
    ///     The number of characters that would be needed to decode any pending bytes in the
    ///     decoder state followed by an array of bytes.
    /// </returns>
    public override int GetCharCount(byte[] bytes, int index, int count)
    {
        var returnValue = 0;
        var end = index + count;
        var currentIndex = index;
        var first = true;
        bool? isBasic;

        do
        {
            var limit = end - currentIndex;

            currentIndex += IsScalarValueBasic(bytes, currentIndex, limit, first, out isBasic);
            first = false;

            if (isBasic is bool isBasic_)
                returnValue += (isBasic_ == true) ? 1 : 2;
        }
        while (isBasic is not null);

        return returnValue;
    }

    /// <summary>
    ///     Decodes any pending bytes in the decoder state followed by an array of bytes. The
    ///     decoder state <strong>is</strong> modified.
    /// </summary>
    /// <param name="bytes">
    ///     The array of bytes to decode.
    /// </param>
    /// <param name="byteIndex">
    ///     The index of the first byte to decode.
    /// </param>
    /// <param name="byteCount">
    ///     The number of bytes to decode.
    /// </param>
    /// <param name="chars">
    ///     The character array to contain the resulting sequence of characters.
    /// </param>
    /// <param name="charIndex">
    ///     The index at which to start writing the resulting sequence of characters.
    /// </param>
    /// <returns>
    ///     The number of characters written.
    /// </returns>
    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars
        , int charIndex)
    {
        int byteEnd = byteIndex + byteCount;
        int currentByteIndex = byteIndex;
        int currentCharIndex = charIndex;

        while (currentByteIndex < byteEnd)
        {
            var byteLimit = byteEnd - currentByteIndex;

            currentByteIndex += ReadScalarValue(bytes, currentByteIndex, byteLimit
                , out int? scalarValue);

            if (scalarValue is int scalarValue_)
            {
                if ((0 <= scalarValue_ && scalarValue_ <= 0xD7FF)
                    || (0xE000 <= scalarValue_ && scalarValue_ <= 0xFFFF))
                {
                    chars[currentCharIndex++] = (char)scalarValue_;
                }
                else if (0x010000 <= scalarValue && scalarValue <= 0x10FFFF)
                {
                    chars[currentCharIndex++] = HighSurrogate(scalarValue_);
                    chars[currentCharIndex++] = LowSurrogate(scalarValue_);
                }
                else
                {
                    chars[currentCharIndex++] = Constants.ReplacementChar;
                }
            }
        }

        return currentCharIndex - charIndex;
    }

    /// <summary>
    ///     Measures if a Unicode scalar value is represented by a single <pre>char</pre> or if
    ///     it requires two and returns that result in an output parameter. The decoder state
    ///     <strong>should not</strong> be modified.
    /// </summary>
    /// <param name="bytes">
    ///     The byte array containing the scalar value to measure.
    /// </param>
    /// <param name="index">
    ///     The index of the first byte of the scalar value to measure.
    /// </param>
    /// <param name="limit">
    ///     The maximum number of bytes after the <pre>index</pre> to measure.
    /// </param>
    /// <param name="first">
    ///     Indicates if this is the first time this function is being invoked for an invocation
    ///     of <pre>GetCharCount</pre>. If <pre>true</pre>, the decoder state should be read
    ///     and factored in as there may be trailing bytes left over from the last invocation of
    ///     <pre>GetChars</pre>.
    /// </param>
    /// <param name="isBasic">
    ///     <ul>
    ///         <li>
    ///             <pre>true</pre> — The measured scalar value is represented by a single
    ///             <pre>char</pre> value.
    ///         </li>
    ///         <li>
    ///             <pre>false</pre> — The measured scalar value is represented by two
    ///             <pre>char</pre> values.
    ///         </li>
    ///         <li>
    ///             <pre>null</pre> — The complete scalar value could not be measured before
    ///             exceeding <pre>limit</pre>.
    ///         </li>
    ///     </ul>
    /// </param>
    /// <returns>
    ///     The number of bytes read to measure the scalar value.
    /// </returns>
    protected abstract int IsScalarValueBasic(byte[] bytes, int index, int limit, bool first
        , out bool? isBasic);

    /// <summary>
    ///     Decodes the next encoded scalar value from a byte array and returns the result in an
    ///     output parameter. The decoder state <strong>should</strong> be modified.
    /// </summary>
    /// <param name="bytes">
    ///     The byte array containing the scalar value to decode.
    /// </param>
    /// <param name="index">
    ///     The index of the first byte of the scalar value to decode.
    /// </param>
    /// <param name="limit">
    ///     The maximum number of bytes after the <pre>index</pre> to decode.
    /// </param>
    /// <param name="scalarValue">
    ///     The next decoded Unicode scalar value, or <pre>null</pre> if the complete scalar
    ///     value could not be decoded before exceeding <pre>limit</pre>. If the value is
    ///     outside of the standard U+0000 and U+10FFFF range then the decoder will use the
    ///     U+FFFD replacement character instead.
    /// </param>
    /// <returns>
    ///     The number of bytes read to decode the scalar value.
    /// </returns>
    protected abstract int ReadScalarValue(byte[] bytes, int index, int limit
        , out int? scalarValue);

    private static char HighSurrogate(int scalarValue) =>
        (char)((((uint)scalarValue - 0x10000U) >> 10) + 0xD800);

    private static char LowSurrogate(int scalarValue) =>
        (char)(((scalarValue - 0x10000) & 0x3FF) + 0xDC00);
}
