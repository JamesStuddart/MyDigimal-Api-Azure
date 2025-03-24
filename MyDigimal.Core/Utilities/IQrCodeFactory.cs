namespace MyDigimal.Core.Utilities
{
    public interface IQrCodeFactory
    {
        QrCodeFactory SetValue(string value);
        QrCodeFactory SetCodeColor(string color);
        QrCodeFactory SetBackgroundColor(string color);

        string Build();
        QrCodeFactory Reset();

    }
}