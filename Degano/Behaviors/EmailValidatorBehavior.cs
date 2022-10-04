using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Behaviors
{
    internal class EmailValidatorBehavior : Behavior<Entry>
    {
        private Regex regex { get; } = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        public bool IsValid { get; set; } = false;
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEmailChange;
            base.OnAttachedTo(entry);
        }
        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEmailChange;
            base.OnDetachingFrom(entry);
        }

        private void OnEmailChange(object sender, TextChangedEventArgs args)
        {
            string email = args.NewTextValue;
            Match match = regex.Match(email);
            IsValid = match.Success;
            ((Entry)sender).TextColor = match.Success ? Colors.Black : Colors.Red;
        }
    }
}
