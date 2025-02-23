using Microsoft.AspNetCore.Mvc;
using RegularExpressionTask3.Data;
using RegularExpressionTask3.Models;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace RegularExpressionTask3.Controllers
{
    public class PhoneController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhoneController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                ViewBag.Message = "Please enter a valid text.";
                return View();
            }

            var contacts = ExtractPhoneNumbers(inputText);

            foreach (var contact in contacts)
            {
                if (!_context.PhoneNumbers.Any(p => p.Number == contact.Number))
                {
                    _context.PhoneNumbers.Add(contact);
                }
            }

            _context.SaveChanges();
            ViewBag.Contacts = _context.PhoneNumbers.ToList();
            return View();
        }

        private List<PhoneNumber> ExtractPhoneNumbers(string text)
        {
            var phoneRegex = new Regex(@"\b(?:\+?\d{1,3}[-.\s]?)?\(?\d{2,4}\)?[-.\s]?\d{2,4}[-.\s]?\d{2,4}\b", RegexOptions.Compiled);
            var matches = phoneRegex.Matches(text);

            var contacts = new List<PhoneNumber>();

            foreach (Match match in matches)
            {
                string phoneNumber = match.Value.Trim();
                string ownerName = ExtractName(text, phoneNumber);

                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    contacts.Add(new PhoneNumber
                    {
                        Number = phoneNumber,
                        OwnerName = ownerName
                    });
                }
            }

            return contacts;
        }

        private string ExtractName(string text, string phoneNumber)
        {
            string pattern = $@"
                (?:
                    (?<name>[А-ЯA-Z][а-яa-z]+(?:-[А-ЯA-Z][а-яa-z]+)?(?:\s+[А-ЯA-Z][а-яa-z]+)?)\s+
                    (?:'s\s+phone\s+number\s+is|телефонният му номер е|phone\s+number\s+is|номерът му е|
                    може да се свържете с него на|ползва|има|използва|принадлежи на|has|uses|owns|belongs to)?\s*
                    {Regex.Escape(phoneNumber)}|

                    {Regex.Escape(phoneNumber)}\s+
                    (?:belongs to|is used by|може да се свържете с|е на|номерът на|принадлежи на)\s+
                    (?<name>[А-ЯA-Z][а-яa-z]+(?:-[А-ЯA-Z][а-яa-z]+)?(?:\s+[А-ЯA-Z][а-яa-z]+)?)|

                    {Regex.Escape(phoneNumber)}\s*-\s*
                    (?<name>[А-ЯA-Z][а-яa-z]+(?:-[А-ЯA-Z][а-яa-z]+)?(?:\s+[А-ЯA-Z][а-яa-z]+)?)|

                    (?<name>[А-ЯA-Z][а-яa-z]+(?:-[А-ЯA-Z][а-яa-z]+)?(?:\s+[А-ЯA-Z][а-яa-z]+)?)\s*-\s*
                    {Regex.Escape(phoneNumber)}
                )";

            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            if (match.Success)
            {
                string extractedName = match.Groups["name"].Value.Trim();

                // Exclude common false-positive words
                var invalidWords = new HashSet<string> { "на", "номер", "number", "телефон", "is", "used", "by", "the", "belongs", "to" };

                if (!string.IsNullOrEmpty(extractedName) && !invalidWords.Contains(extractedName.ToLower()))
                    return extractedName;
            }

            return "Unknown";
        }




    }
}
