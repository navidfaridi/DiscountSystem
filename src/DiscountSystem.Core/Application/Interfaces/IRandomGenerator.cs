namespace DiscountSystem.Core.Application.Interfaces;

public interface IRandomGenerator
{
    string NextCode(int length);
}
