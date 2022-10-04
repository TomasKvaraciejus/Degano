namespace Behaviors
{
    internal class TextValidatorBehavior : Behavior<Entry>
    {
        public bool IsValid { get; set; } = false;
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnTextChange;
            base.OnAttachedTo(entry);
        }
        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -=  OnTextChange;
            base.OnDetachingFrom(entry);
        }

        private void OnTextChange(object sender, TextChangedEventArgs args)
        {
            ((Entry)sender).TextColor = args.NewTextValue.Length < 10 ? Colors.Red : Colors.Black;
            IsValid = !(args.NewTextValue.Length < 10);
        }
    }
}
