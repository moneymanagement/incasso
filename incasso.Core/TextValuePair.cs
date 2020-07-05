namespace incasso
{
    public class TextValuePair
    {
        public TextValuePair()
        {
        }

        public TextValuePair(string text, string value)
        {
            Text = text;
            Value = value;
        }
        // name of the string variable
        public string Text { get; set; }
        // value of the string variable
        public string Value { get; set; }
    }
}