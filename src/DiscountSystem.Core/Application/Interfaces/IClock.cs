using System;

namespace DiscountSystem.Core.Application.Interfaces;

public interface IClock { DateTime UtcNow { get; } }
