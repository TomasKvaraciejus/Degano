using System;
using System.Text.RegularExpressions;

namespace Degano.Helpers
{
    public class PasswordValidator
    {
        public bool isValid { get; set; } = false;
        public string ErrorMessage { get; set; }
        public bool IsPasswordValid(string text)
        {
            if (text.Length < 10)
            {
                ErrorMessage = "Password should contain at least 10 characters!";
                isValid = false;
                return false;
            }
            if (Regex.IsMatch(text, @"\s"))
            {
                ErrorMessage = "Password should not contain whitespaces!";
                isValid = false;
                return false;
            }
            if (!text.Any(char.IsDigit))
            {
                ErrorMessage = "Password should contain at least one digit!";
                isValid = false;
                return false;
            }
            isValid = true;
            return true;
        }
    }
}
