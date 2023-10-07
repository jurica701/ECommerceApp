using ECommerceApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult SubmitAction(CreditCardModel model)
        {
            long creditCardNumberValue = model.creditCardNumber;
            DateTime creditCardExpiryDateValue = model.expiryDate;
            int creditCardCVVValue = model.cvv;

            //Perform card number validation (card number is between 16 and 19 digits long) 
            int creditCardNumberDigitCount = 0;
            if (creditCardNumberValue != 0 || creditCardNumberValue > 0) { creditCardNumberDigitCount = creditCardNumberValue.ToString().Length; }
            if (creditCardNumberDigitCount < 16 || creditCardNumberDigitCount > 19) { return RedirectToAction("Error", "Home"); }

            //call method for LuhnAlgorithm
            bool luhnAlg = LuhnAlgorithm(creditCardNumberValue);

            //Get card type and validate cvv
            int startingDigits = (int)Math.Truncate(creditCardNumberValue / Math.Pow(10, creditCardNumberDigitCount - 2));
            bool americanExpressType = false;

            if (startingDigits == 34 || startingDigits == 37) { americanExpressType = true; }
            if (americanExpressType && (creditCardCVVValue < 1000 || creditCardCVVValue > 9999)) { return RedirectToAction("Error", "Home"); }
            if (!americanExpressType && (creditCardCVVValue < 100 || creditCardCVVValue > 999)) { return RedirectToAction("Error", "Home"); }

            //Expiry date validation
            if (creditCardExpiryDateValue < DateTime.Now) { return RedirectToAction("Error", "Home"); }

            //Luhn Algorithm validation
            if (luhnAlg) return Content("Credit card information valid");
            else return RedirectToAction("Error", "Home");
        }
        public bool LuhnAlgorithm(long number)
        {
            int numberLenght = number.ToString().Length;
            long lastDigit = 0;
            long sum = 0;
            long luhnDigit = number % 10;
            number = number / 10;

            for (int i = 0; i < numberLenght; i++)
            {
                if (i % 2 == 0)
                {
                    lastDigit = number % 10;
                    sum += GetNumberForSum(lastDigit);
                }
                else
                {
                    lastDigit = number % 10;
                    sum += lastDigit;
                }
                number = number / 10;
            }

            return (10 - (sum % 10)) == luhnDigit;

            int GetNumberForSum(long i)
            {
                switch (i)
                {
                    case 0: return 0;
                    case 1: return 2;
                    case 2: return 4;
                    case 3: return 6;
                    case 4: return 8;
                    case 5: return 1;
                    case 6: return 3;
                    case 7: return 5;
                    case 8: return 7;
                    case 9: return 9;
                    default: return 0;
                }
            }
        }
    }
}