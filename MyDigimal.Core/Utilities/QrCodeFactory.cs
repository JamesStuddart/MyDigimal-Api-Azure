using System;
using System.Drawing;
using QRCoder;

namespace MyDigimal.Core.Utilities
{
    public class QrCodeFactory : IQrCodeFactory
    {
        private const string DefaultCodeColor = "#FFFFFF";
        private const string DefaultBkgColor = "#652148";
        
        private string _value;
        private string _codeColor = DefaultCodeColor;
        private string _bkgColor = DefaultBkgColor;

        
        private bool IsValid => 
            !string.IsNullOrWhiteSpace(_value) && !string.IsNullOrWhiteSpace(_codeColor) && !string.IsNullOrWhiteSpace(_bkgColor);
        
        public QrCodeFactory SetValue(string value)
        {
            _value = value;
            return this;
        }

        public QrCodeFactory SetCodeColor(string color)
        {
            _codeColor = color;
            return this;
        }

        public QrCodeFactory SetBackgroundColor(string color)
        {
            _bkgColor = color;
            return this;
        }

        public string Build()
        {
            if (!IsValid)
                throw new Exception("Invalid Setup");

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(_value, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new SvgQRCode(qrCodeData);
            var svgString = qrCode.GetGraphic(20, _codeColor, _bkgColor, false, SvgQRCode.SizingMode.ViewBoxAttribute);
            Reset();
            return svgString;
        }

        public QrCodeFactory Reset()
        {
            _value = string.Empty;
            _codeColor = DefaultCodeColor;
            _bkgColor = DefaultBkgColor;
            return this;
        }

    }
}