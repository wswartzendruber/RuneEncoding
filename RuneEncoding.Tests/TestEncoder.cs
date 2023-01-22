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

using RuneEncoding;

namespace RuneEncoding.Tests;

public class TestEncoder : RuneEncoder
{
    public List<int> ByteCounts = new();
    public List<int> ByteValues = new();

    protected override int ByteCount(int scalarValue)
    {
        ByteCounts.Add(scalarValue);

        return 1;
    }

    protected override int WriteBytes(int scalarValue, byte[] bytes, int index)
    {
        ByteValues.Add(scalarValue);

        return 1;
    }

    protected override void ResetState()
    {
        ByteCounts.Clear();
        ByteValues.Clear();
    }
}
