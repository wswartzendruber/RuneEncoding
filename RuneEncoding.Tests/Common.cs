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

public static class Common
{
    public static readonly List<int> AllScalarValues;

    static Common()
    {
        AllScalarValues = new();

        for (int value = 0x0000; value <= 0xD7FF; value++)
            AllScalarValues.Add(value);
        for (int value = 0xE000; value <= 0xFFFF; value++)
            AllScalarValues.Add(value);
        for (int value = 0x010000; value <= 0x10FFFF; value++)
            AllScalarValues.Add(value);
    }
}
