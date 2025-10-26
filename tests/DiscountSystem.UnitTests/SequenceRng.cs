using DiscountSystem.Core.Application.Interfaces;

namespace DiscountSystem.UnitTests;

internal sealed class SequenceRng : IRandomGenerator
{
    private readonly Queue<string> _queue;
    public SequenceRng(IEnumerable<string> seq) => _queue = new Queue<string>(seq);
    public string NextCode(int length) => _queue.Dequeue();
}
