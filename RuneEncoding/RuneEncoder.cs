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

    protected abstract int ByteCount(int scalarValue);

    protected abstract int WriteBytes(int scalarValue, byte[] bytes, int index);

    public override void Reset()
    {
        ConvertComposer.Reset();
        ResetState();
    }

    protected virtual void ResetState() { }
}
