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

namespace RuneEncoding.Tests;

public class TestEncoder : RuneEncoder
{
    public byte[]? CurrentBuffer;
    public int ExpectedIndex;
    public long CountState;
    public long EncodeState;
    public List<int> CountValues = new();
    public List<int> EncodeValues = new();
    public List<int> CountIncrements = new();
    public List<int> EncodeIncrements = new();

    public void AssertConsistency()
    {
        if (!CountValues.SequenceEqual(EncodeValues))
            throw new InvalidOperationException("CountValues != EncodeValues");
        if (!CountIncrements.SequenceEqual(EncodeIncrements))
            throw new InvalidOperationException("CountIncrements != EncodeIncrements");
    }

    protected override int ByteCount(int scalarValue)
    {
        var increment = scalarValue % 4;

        CountValues.Add(scalarValue);
        CountIncrements.Add(increment);

        return increment;
    }

    protected override int WriteBytes(int scalarValue, byte[] bytes, int index)
    {
        if (bytes == CurrentBuffer)
        {
            if (index != ExpectedIndex)
                throw new InvalidOperationException("Provided index and ExpectedIndex differ.");
        }
        else
        {
            CurrentBuffer = bytes;
            ExpectedIndex = index;
        }

        var increment = scalarValue % 4;

        EncodeValues.Add(scalarValue);
        EncodeIncrements.Add(increment);

        ExpectedIndex += increment;

        return increment;
    }

    protected override void ResetState()
    {
        CurrentBuffer = null;
        ExpectedIndex = 0;
        CountState = 0;
        EncodeState = 0;
        CountValues.Clear();
        EncodeValues.Clear();
        CountIncrements.Clear();
        EncodeIncrements.Clear();
    }
}
