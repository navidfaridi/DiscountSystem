using System;

namespace DiscountSystem.Core.Application.Commands;
public record GenerateCodesCommand(ushort Count, byte Length);
