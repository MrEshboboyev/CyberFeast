using BuildingBlocks.Core.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace FoodDelivery.Services.Catalogs.Products.ValueObjects;

public record ProductInformation
{
    // EF
    private ProductInformation() { }

    public string Title { get; private set; } = null!;
    public string Content { get; private set; } = null!;

    public static ProductInformation Of([NotNull] string? title, [NotNull] string? content)
    {
        // validations should be placed here instead of constructor
        title.NotBeNullOrWhiteSpace();
        content.NotBeNullOrWhiteSpace();

        return new ProductInformation { Title = title, Content = content };
    }

    public void Deconstruct(out string title, out string content) => (title, content) = (Title, Content);
}