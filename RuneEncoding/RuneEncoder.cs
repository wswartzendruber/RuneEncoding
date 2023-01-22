/*
 * Copyright 2023 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived
 * all copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

using System.Text;

namespace RuneEncoding;

/// <summmary>
///     Automatically handles the common aspects of character encoding, such as surrogate
///     composition, thereby allowing implementers to worry about specific concerns only.
/// </summary>
public abstract class RuneEncoder : Encoder
{
    private readonly SurrogateComposer ConvertComposer = new();
    private readonly SurrogateComposer CountComposer = new();

    public override int GetByteCount(char[] chars, int index, int count, bool flush)
    {
        var returnValue = 0;
        var end = index + count;
        var callback = delegate(int value)
        {
            returnValue += ByteCount(value);
        };

        CountComposer.Reset();

        if (ConvertComposer.HighSurrogate is char highSurrogate)
            CountComposer.Input(highSurrogate, callback);

        for (int i = index; i < end; i++)
            CountComposer.Input(chars[i], callback);

        if (flush == true)
            CountComposer.Flush(callback);

        return returnValue;
    }

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes
        , int byteIndex, bool flush)
    {
        var startByteIndex = byteIndex;
        var charEnd = charIndex + charCount;
        var callback = delegate(int value)
        {
            byteIndex += WriteBytes(value, bytes, byteIndex);
        };

        for (int i = charIndex; i < charEnd; charIndex++)
            ConvertComposer.Input(chars[i], callback);

        if (flush == true)
            ConvertComposer.Flush(callback);

        return byteIndex - startByteIndex;
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
    /// <param name="index">The offset within the byte array to encode at.</param>
    /// <returns>
    ///     The number of bytes used to encode the specified Unicode scalar value.
    /// </returns>
    protected abstract int WriteBytes(int scalarValue, byte[] bytes, int index);

    /// <summary>
    ///     Resets the surrogate state of the encoder; any buffered high surrogate is cleared.
    /// </summary>
    public override void Reset()
    {
        ConvertComposer.Reset();
        ResetState();
    }

    /// <summary>
    ///     Resets the specific state of the encoding.
    /// </summary>
    protected virtual void ResetState() { }
}
