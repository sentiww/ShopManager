using MudBlazor;
using ShopManager.Common.Requests;

namespace ShopManager.Client.Models;

public sealed class EditDiscountModel
{
    public decimal Percentage { get; set; }
    public DateRange StartEndDate { get; set; }
    public Guid ProductId { get; set; }
}

public static class EditDiscountModelExtensions
{
    public static UpdateDiscountRequest ToRequest(this EditDiscountModel model)
    {
        return new UpdateDiscountRequest
        {
            Percentage = model.Percentage,
            StartDate = model.StartEndDate.Start.Value,
            EndDate = model.StartEndDate.End.Value,
            ProductId = model.ProductId
        };
    }
}