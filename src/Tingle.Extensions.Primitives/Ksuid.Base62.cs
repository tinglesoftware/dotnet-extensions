namespace Tingle.Extensions.Primitives;

public readonly partial struct Ksuid
{
    // This Base62 implementation is suited and optimized for KSUID hence should remain nested.
    static class KsuidBase62
    {
        /* 
         * Sorting works in order of digits -> upper case -> lower case
         * This is the ASCII order and it is the most common one.
         * Sorting in .NET uses Unicode by default but we need to be standard so ignore that.
         * 
         * This was changed from non-inverted to inverted in Merged PR 20338: Fixes and Updates for 2022 October 03 (Part 1) [545a21528dabdefe31fe940a78771bb564a76069]
         * but it was reverted to non-inverted on 2023-Nov-15
         * */
        internal static readonly char[] Base62Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
        internal const uint BaseValue = 62;
        internal const ulong MaxUIntCount = 4294967296;
        internal const int OffsetUppercase = 10;
        internal const int OffsetLowercase = 36;

        /// <summary>
        /// Converts an array of 8-bit unsigned integers to its equivalent string representation
        /// that is encoded with base-62 digits.
        /// </summary>
        /// <param name="src">An array of 8-bit unsigned integers.</param>
        /// <returns>The string representation, in base 64, of the contents of <paramref name="src"/>.</returns>
        public static string ToBase62(byte[] src)
        {
            var converted = FastEncodeKsuidToBase62(src);
            var encode62Chars = Base62Characters;
            Span<char> buffer = stackalloc char[27];
            for (int i = converted.Length - 1; i >= 0; i--)
            {
                buffer[i] = encode62Chars[converted[i]];
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Converts the specified string, which encodes binary data as base-62 digits, to
        /// an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="src">The string to convert.</param>
        /// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="src"/>.</returns>
        public static byte[] FromBase62(string src)
        {
            return FastDecodeBase62Ksuid(src.AsSpan());
        }

        private static byte[] FastEncodeKsuidToBase62(byte[] src)
        {
            var dest = new byte[27];

            // To avoid bound checking in the subsequent statements.
            _ = src[19];
            var parts = new uint[5]
            {
                ((uint)src[0]) << 24 | ((uint)src[1]) << 16 | ((uint)src[2]) << 8 | src[3],
                ((uint)src[4]) << 24 | ((uint)src[5]) << 16 | ((uint)src[6]) << 8 | src[7],
                ((uint)src[8]) << 24 | ((uint)src[9]) << 16 | ((uint)src[10]) << 8 | src[11],
                ((uint)src[12]) << 24 | ((uint)src[13]) << 16 | ((uint)src[14]) << 8 | src[15],
                ((uint)src[16]) << 24 | ((uint)src[17]) << 16 | ((uint)src[18]) << 8 | src[19],
            };
            var destLength = dest.Length;
            Span<uint> quotient = stackalloc uint[5];
            while (parts.Length > 0)
            {
                // reusing the quotient array
                quotient.Clear();
                ulong remainder = 0;
                int counter = 0;
                foreach (var part in parts)
                {
                    ulong accumulator = part + remainder * MaxUIntCount;
                    var digit = accumulator / BaseValue;
                    remainder = accumulator % BaseValue;
                    if (counter != 0 || digit != 0)
                    {
                        quotient[counter] = (uint)digit;
                        counter++;
                    }
                }
                destLength--;
                dest[destLength] = (byte)remainder;
                parts = quotient[..counter].ToArray();
            }
            return dest;
        }

        private static byte[] FastDecodeBase62Ksuid(ReadOnlySpan<char> src)
        {
            var dest = new byte[20];
            // To avoid bound checking in the subsequent statements.
            _ = src[26];
            var parts = new uint[27]
            {
                ConvertToBase62Value(src[0]),
                ConvertToBase62Value(src[1]),
                ConvertToBase62Value(src[2]),
                ConvertToBase62Value(src[3]),
                ConvertToBase62Value(src[4]),
                ConvertToBase62Value(src[5]),
                ConvertToBase62Value(src[6]),
                ConvertToBase62Value(src[7]),
                ConvertToBase62Value(src[8]),
                ConvertToBase62Value(src[9]),

                ConvertToBase62Value(src[10]),
                ConvertToBase62Value(src[11]),
                ConvertToBase62Value(src[12]),
                ConvertToBase62Value(src[13]),
                ConvertToBase62Value(src[14]),
                ConvertToBase62Value(src[15]),
                ConvertToBase62Value(src[16]),
                ConvertToBase62Value(src[17]),
                ConvertToBase62Value(src[18]),
                ConvertToBase62Value(src[19]),

                ConvertToBase62Value(src[20]),
                ConvertToBase62Value(src[21]),
                ConvertToBase62Value(src[22]),
                ConvertToBase62Value(src[23]),
                ConvertToBase62Value(src[24]),
                ConvertToBase62Value(src[25]),
                ConvertToBase62Value(src[26]),
            };
            var destLength = dest.Length;
            Span<uint> quotient = stackalloc uint[27];
            while (parts.Length > 0)
            {
                // reusing the quotient array
                quotient.Clear();
                ulong remainder = 0;
                int counter = 0;
                foreach (var part in parts)
                {
                    ulong accumulator = part + remainder * BaseValue;
                    var digit = accumulator / MaxUIntCount;
                    remainder = accumulator % MaxUIntCount;
                    if (counter != 0 || digit != 0)
                    {
                        quotient[counter] = (uint)digit;
                        counter++;
                    }
                }

                dest[destLength - 4] = (byte)(remainder >> 24);
                dest[destLength - 3] = (byte)(remainder >> 16);
                dest[destLength - 2] = (byte)(remainder >> 8);
                dest[destLength - 1] = (byte)remainder;
                destLength -= 4;

                parts = quotient[..counter].ToArray();
            }
            return dest;
        }

        private static byte ConvertToBase62Value(char digit)
        {
            if (digit >= '0' && digit <= '9')
            {
                return (byte)(digit - '0');
            }
            else if (digit >= 'A' && digit <= 'Z')
            {
                return (byte)(OffsetUppercase + (digit - 'A'));
            }
            else
            {
                return (byte)(OffsetLowercase + (digit - 'a'));
            }
        }
    }
}
