using DiscountSystem.Core.Application.Commands;
using DiscountSystem.Core.Application.Interfaces;
using DiscountSystem.Core.Domain.Entites;
using DiscountSystem.Core.Domain.Enums;
using DiscountSystem.Services.Services;
using DiscountSystem.UnitTests;
using FluentValidation;
using Moq;

public class DiscountAppServiceTests
{
    private static DiscountAppService CreateSut(
       Mock<IDiscountCodeRepository> repoMock,
       IRandomGenerator rng,
       IClock clock,
       Mock<ICacheService>? cacheMock = null)
    {
        // Provide a default loose cache mock if not supplied
        cacheMock ??= new Mock<ICacheService>(MockBehavior.Loose);

        // Common default setup for cache so that tests don't fail unexpectedly
        cacheMock.Setup(c => c.GetAsync<SimpleDiscountCode>(
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SimpleDiscountCode?)null);

        cacheMock.Setup(c => c.SetAsync(
                It.IsAny<string>(), It.IsAny<object>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        cacheMock.Setup(c => c.RemoveAsync(
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new DiscountAppService(
            repoMock.Object,
            rng,
            clock,
            ValidatorsFactory.Use(),
            ValidatorsFactory.Generate(),
            cacheMock.Object);
    }

    [Fact]
    public async Task GenerateAsync_ValidInput_AddsRequestedCount()
    {
        // Arrange
        var list = new List<string>(new[] { "ABCDEFG", "HIJKLMN", "PQRSTUV" });
        var repo = new Mock<IDiscountCodeRepository>(MockBehavior.Strict);
        repo.Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repo.Setup(r => r.AddRangeUniqueBatchAsync(It.IsAny<IEnumerable<DiscountCode>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(list.Count));

        var rng = new SequenceRng(list);
        var clock = new FixedClock(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var sut = CreateSut(repo, rng, clock);

        // Act
        var result = await sut.GenerateAsync(new GenerateCodesCommand(3, 7), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        repo.Verify(r => r.AddRangeUniqueBatchAsync(
            It.Is<IEnumerable<DiscountCode>>(list => list.Count() == 3
                                                     && list.All(c => c.Code.Value.Length == 7)
                                                     && list.All(c => c.Status == DiscountStatus.Available)
                                                     && list.All(c => c.CreatedAtUtc == clock.UtcNow)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateAsync_InvalidCount_ThrowsValidationException()
    {
        var repo = new Mock<IDiscountCodeRepository>(MockBehavior.Loose);
        var rng = new SequenceRng(new[] { "ABCDEFG" });
        var clock = new FixedClock(DateTime.UtcNow);
        var sut = CreateSut(repo, rng, clock);

        await Assert.ThrowsAsync<ValidationException>(() =>
            sut.GenerateAsync(new GenerateCodesCommand(0, 7), CancellationToken.None));
    }

    [Fact]
    public async Task GenerateAsync_InvalidLength_ThrowsValidationException()
    {
        var repo = new Mock<IDiscountCodeRepository>(MockBehavior.Loose);
        var rng = new SequenceRng(new[] { "ABCDEFGH" });
        var clock = new FixedClock(DateTime.UtcNow);
        var sut = CreateSut(repo, rng, clock);

        await Assert.ThrowsAsync<ValidationException>(() =>
            sut.GenerateAsync(new GenerateCodesCommand(1, 9), CancellationToken.None));
    }

    [Fact]
    public async Task UseAsync_WhenTryConsumeSucceeds_ReturnsSuccess_AndSkipsGet()
    {
        // Arrange
        var repo = new Mock<IDiscountCodeRepository>(MockBehavior.Strict);
        repo.Setup(r => r.TryConsumeAsync("ABCDEFG", It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var cache = new Mock<ICacheService>(MockBehavior.Strict);
        cache.Setup(c => c.GetAsync<SimpleDiscountCode>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((SimpleDiscountCode?)null);
        cache.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var sut = CreateSut(repo, new SequenceRng(Array.Empty<string>()), new FixedClock(DateTime.UtcNow), cache);

        // Act
        var res = await sut.UseAsync(new UseCodeCommand("ABCDEFG"), CancellationToken.None);

        // Assert
        Assert.Equal(UseCodeResult.Success, res);
        repo.Verify(r => r.GetStatusAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UseAsync_WhenNotConsumed_AndNotFound_ReturnsNotFound()
    {
        // Arrange
        var repo = new Mock<IDiscountCodeRepository>(MockBehavior.Strict);
        repo.Setup(r => r.TryConsumeAsync("ABCDEFG", It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repo.Setup(r => r.GetStatusAsync("ABCDEFG", It.IsAny<CancellationToken>()))
            .ReturnsAsync((DiscountStatus?)null);

        var sut = CreateSut(repo, new SequenceRng(Array.Empty<string>()), new FixedClock(DateTime.UtcNow));

        // Act
        var res = await sut.UseAsync(new UseCodeCommand("ABCDEFG"), CancellationToken.None);

        // Assert
        Assert.Equal(UseCodeResult.NotFound, res);
    }

    [Fact]
    public async Task UseAsync_WhenNotConsumed_ButAlreadyUsed_ReturnsAlreadyUsed()
    {
        // Arrange
        var repo = new Mock<IDiscountCodeRepository>(MockBehavior.Strict);
        repo.Setup(r => r.TryConsumeAsync("ZZZZZZZ", It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repo.Setup(r => r.GetStatusAsync("ZZZZZZZ", It.IsAny<CancellationToken>()))
            .ReturnsAsync(DiscountStatus.Used);

        var sut = CreateSut(repo, new SequenceRng(Array.Empty<string>()), new FixedClock(DateTime.UtcNow));

        // Act
        var res = await sut.UseAsync(new UseCodeCommand("ZZZZZZZ"), CancellationToken.None);

        // Assert
        Assert.Equal(UseCodeResult.AlreadyUsed, res);
    }

    [Fact]
    public async Task UseAsync_WhenNotConsumed_ButEntityAvailable_ReturnsUnknownError()
    {
        // Arrange
        var repo = new Mock<IDiscountCodeRepository>(MockBehavior.Strict);
        repo.Setup(r => r.TryConsumeAsync("ABCDEFG", It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repo.Setup(r => r.GetStatusAsync("ABCDEFG", It.IsAny<CancellationToken>()))
            .ReturnsAsync(DiscountStatus.Available);

        var sut = CreateSut(repo, new SequenceRng(Array.Empty<string>()), new FixedClock(DateTime.UtcNow));

        // Act
        var res = await sut.UseAsync(new UseCodeCommand("ABCDEFG"), CancellationToken.None);

        // Assert
        Assert.Equal(UseCodeResult.UnknownError, res);
    }

    [Fact]
    public async Task UseAsync_InvalidCode_ThrowsValidationException()
    {
        // Arrange
        var repo = new Mock<IDiscountCodeRepository>(MockBehavior.Loose);
        var sut = CreateSut(repo, new SequenceRng(Array.Empty<string>()), new FixedClock(DateTime.UtcNow));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            sut.UseAsync(new UseCodeCommand("!"), CancellationToken.None));
    }
}
