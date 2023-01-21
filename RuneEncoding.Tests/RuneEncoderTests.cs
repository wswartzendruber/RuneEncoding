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
using RuneEncoding;

namespace RuneEncoding.Tests;

public class RuneEncoderTests
{
    [Fact]
    public void ByteLengthsValidFlush()
    {
        var totalString = new StringBuilder();
        var encoder = new TestEncoder();

        foreach (int value in Common.AllScalarValues)
            totalString.Append(char.ConvertFromUtf32(value));

        var byteCount = encoder.GetByteCount(totalString.ToString(), true);

        Assert.True(byteCount == Common.AllScalarValues.Count());
        Assert.True(encoder.ByteCounts.SequenceEqual(Common.AllScalarValues));
    }

    [Fact]
    public void ByteLengthsValidNoFlush()
    {
        var totalString = new StringBuilder();
        var encoder = new TestEncoder();

        foreach (int value in Common.AllScalarValues)
            totalString.Append(char.ConvertFromUtf32(value));

        var byteCount = encoder.GetByteCount(totalString.ToString(), false);

        Assert.True(byteCount == Common.AllScalarValues.Count());
        Assert.True(encoder.ByteCounts.SequenceEqual(Common.AllScalarValues));
    }

    [Fact]
    public void ByteLengthsTrailingHSFlush()
    {
        var totalString = new StringBuilder();
        var encoder = new TestEncoder();

        foreach (int value in Common.AllScalarValues)
            totalString.Append(char.ConvertFromUtf32(value));
        totalString.Append('\uD800');

        var byteCount = encoder.GetByteCount(totalString.ToString(), true);
        var scalarValues = Common.AllScalarValues.ToList();

        scalarValues.Add(0xFFFD);

        Assert.True(byteCount == scalarValues.Count());
        Assert.True(encoder.ByteCounts.SequenceEqual(scalarValues));
    }

    [Fact]
    public void ByteLengthsTrailingHSNoFlush()
    {
        var totalString = new StringBuilder();
        var encoder = new TestEncoder();

        foreach (int value in Common.AllScalarValues)
            totalString.Append(char.ConvertFromUtf32(value));
        totalString.Append('\uD800');

        var byteCount = encoder.GetByteCount(totalString.ToString(), false);

        Assert.True(byteCount == Common.AllScalarValues.Count());
        Assert.True(encoder.ByteCounts.SequenceEqual(Common.AllScalarValues));
    }

    [Fact]
    public void ByteLengthsTrailingLSFlush()
    {
        var totalString = new StringBuilder();
        var encoder = new TestEncoder();

        foreach (int value in Common.AllScalarValues)
            totalString.Append(char.ConvertFromUtf32(value));
        totalString.Append('\uDC00');

        var byteCount = encoder.GetByteCount(totalString.ToString(), true);
        var scalarValues = Common.AllScalarValues.ToList();

        scalarValues.Add(0xFFFD);

        Assert.True(byteCount == scalarValues.Count());
        Assert.True(encoder.ByteCounts.SequenceEqual(scalarValues));
    }

    [Fact]
    public void ByteLengthsTrailingLSNoFlush()
    {
        var totalString = new StringBuilder();
        var encoder = new TestEncoder();

        foreach (int value in Common.AllScalarValues)
            totalString.Append(char.ConvertFromUtf32(value));
        totalString.Append('\uDC00');

        var byteCount = encoder.GetByteCount(totalString.ToString(), false);
        var scalarValues = Common.AllScalarValues.ToList();

        scalarValues.Add(0xFFFD);

        Assert.True(byteCount == scalarValues.Count());
        Assert.True(encoder.ByteCounts.SequenceEqual(scalarValues));
    }
}
