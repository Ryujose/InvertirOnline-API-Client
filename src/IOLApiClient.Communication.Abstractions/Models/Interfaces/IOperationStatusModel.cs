namespace IOLApiClient.Communication.Abstractions.Models.Interfaces
{
    public interface IOperationStatusModel
    {
        string Date { get; set; }
        decimal Quantity { get; set; }
        decimal Price { get; set; }
    }
}
