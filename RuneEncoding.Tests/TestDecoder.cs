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

public class TestDecoder : RuneDecoder
{
    public List<byte> CountBytes = new();
    public List<byte> EncodeBytes = new();
    public List<int> CountIncrements = new();
    public List<int> EncodeIncrements = new();

    protected override int IsScalarValueBasic(byte[] bytes, int index, int limit
        , out bool? isBasic)
    {
        isBasic = null;

        return 0;
    }

    protected override int ReadScalarValue(byte[] bytes, int index, int limit
        , out int? scalarValue)
    {
        scalarValue = null;

        return 0;
    }
}
