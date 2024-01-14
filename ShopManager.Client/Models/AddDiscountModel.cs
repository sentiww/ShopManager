using MudBlazor;
using ShopManager.Client.Requests;

namespace ShopManager.Client.Models;

public sealed class AddDiscountModel
{
    public decimal Percentage { get; set; }
    public DateRange StartEndDate { get; set; }
    public Guid ProductId { get; set; }
}

public static class AddDiscountModelExtensions
{
    public static AddDiscountRequest ToRequest(this AddDiscountModel model)
    {
        return new AddDiscountRequest
        {
            Percentage = model.Percentage,
            StartDate = model.StartEndDate.Start.Value,
            EndDate = model.StartEndDate.End.Value,
            ProductId = model.ProductId
        };
    }
}