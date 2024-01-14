using MudBlazor;
using ShopManager.Client.Requests;

namespace ShopManager.Client.Models;

public sealed class EditDiscountModel
{
    public decimal Percentage { get; set; }
    public DateRange StartEndDate { get; set; }
    public Guid ProductId { get; set; }
}

public static class EditDiscountModelExtensions
{
    public static EditDiscountRequest ToRequest(this EditDiscountModel model)
    {
        return new EditDiscountRequest
        {
            Percentage = model.Percentage,
            StartDate = model.StartEndDate.Start.Value,
            EndDate = model.StartEndDate.End.Value,
            ProductId = model.ProductId
        };
    }
}