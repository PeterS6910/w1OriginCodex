using System.Text;
using System.Windows.Forms;

namespace Contal.IwQuick.UI
{
    public class NumericUpDownWithCustomTextFormat : NumericUpDown
    {
        public string Prefix { get; set; }
        public string Sufix { get; set; }
        public int MinimalValueLength { get; set; }

        protected override void UpdateEditText()
        {
            Text = string.Format(
                "{0}{1}{2}",
                !string.IsNullOrEmpty(Prefix)
                    ? Prefix
                    : string.Empty,
                FormattedValue(),
                !string.IsNullOrEmpty(Sufix)
                    ? Sufix
                    : string.Empty);
        }

        private string FormattedValue()
        {
            var stringValue = new StringBuilder(
                DecimalPlaces > 0
                    ? Value.ToString(
                        string.Format(
                            "F{0}",
                            DecimalPlaces))
                    : Value.ToString());

            if (stringValue.Length < MinimalValueLength)
            {
                var zeros = new StringBuilder();

                for (var i = 0; i < MinimalValueLength - stringValue.Length; i++)
                {
                    zeros.Append("0");
                }

                stringValue.Insert(0, zeros.ToString());
            }

            return stringValue.ToString();
        }
    }
}
