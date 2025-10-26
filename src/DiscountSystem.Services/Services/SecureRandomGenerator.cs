using DiscountSystem.Core.Application.Interfaces;
using System.Security.Cryptography;

namespace DiscountSystem.Services.Services;

public class SecureRandomGenerator : IRandomGenerator
{
    private static readonly char[] Alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789".ToCharArray();

    public string NextCode(int length)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

        var bytes = RandomNumberGenerator.GetBytes(length);
        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = Alphabet[bytes[i] % Alphabet.Length];
        }
        return new string(chars);
    }
}