using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BookLibrary.Models;
using Services.Interfaces;
using Services.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BookLibrary.ViewModels.Home;
using System.Threading.Tasks;
using Services.Filters;

namespace BookLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IRateService _rateService;
        private readonly IWishListService _wlService;

        public HomeController(IBookService bookService, IAuthorService authorService,  IRateService rateService, IWishListService wlService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _rateService = rateService;
            _wlService = wlService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_bookService.GetAll().ToList().OrderByDescending(b => b.Rate));
        }
      
        [HttpPost]
        public IActionResult Search(string req)
        {
            if (string.IsNullOrEmpty(req))
            {
                return RedirectToAction("Index");
            }
            List<BookDTO> param = new List<BookDTO>();
            List<string> keys = req.Trim().Split(' ').ToList();
            List<BookDTO> allBooks = _bookService.GetAll().ToList();
            for(int i = 0; i < keys.Count; i++)
            {
                keys[i] = keys[i].ToLower().Trim();
                foreach  (BookDTO book  in allBooks)
                {
                    if (book.Title.ToLower().Contains(keys[i]))
                    {
                        if (!param.Exists(b => b.Id == book.Id))
                        {
                            param.Add(book);
                        }
                    }
                    if (book.Year.ToString() == keys[i])
                    {
                        if (!param.Exists(b => b.Id == book.Id))
                        {
                            param.Add(book);
                        }
                    }
                    if (book.Genre.ToLower().ToString().Contains(keys[i]))
                    {
                        if (!param.Exists(b => b.Id == book.Id))
                        {
                            param.Add(book);
                        }
                    }
                    if (_authorService.Get(book.AuthorId).Name.ToLower() == keys[i])
                    {
                        if (!param.Exists(b => b.Id == book.Id))
                        {
                            param.Add(book);
                        }
                    }
                    if (_authorService.Get(book.AuthorId).Surname.ToLower() == keys[i])
                    {
                        if (!param.Exists(b => b.Id == book.Id))
                        {
                            param.Add(book);
                        }
                    }
                }
            }

            return View("Index", param.ToList());
        }

        [HttpGet]
        public IActionResult GetAuthorInfo(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Error");
            }
            AuthorDTO currentAuthor = _authorService.Get(id);
            if (currentAuthor == null)
            {
                return RedirectToAction("Error");
            }
            return View(currentAuthor);
        }

        [HttpGet]
        public IActionResult GetBookInfo(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Error");
            }
            BookDTO currentBook = _bookService.Get(id);
            if (currentBook == null)
            {
                return RedirectToAction("Error");
            }
            return View(currentBook);
        }

        [Authorize]
        public IActionResult DownloadBook(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Error");
            }
            BookDTO book = _bookService.Get(id);
            if (book == null)
            {
                return RedirectToAction("Error");
            }
            string file_type = "application/pdf";
            string file_name = book.Title + ".pdf";

            return File(book.FileBook, file_type, file_name);
        }


        [Authorize]
        [HttpPost]
        public IActionResult RateBook(RateViewModel rateVM)
        {
            if (string.IsNullOrEmpty(rateVM.UserId) && string.IsNullOrEmpty(rateVM.RatedEssenceId) && (rateVM.Value < 1 || rateVM.Value > 5))
            {
                return RedirectToAction("Error");
            }
            BookDTO bookTORate = _bookService.Get(rateVM.RatedEssenceId);
            if (bookTORate == null)
            {
                return RedirectToAction("Error");
            }
            RateDTO yourRate = new RateDTO {
                                               BookId = rateVM.RatedEssenceId,
                                               UserId = rateVM.UserId,
                                               Value = rateVM.Value
                                           };

            List<RateDTO> allRates = _rateService.GetAll().ToList();
            if (allRates != null)
            {
                bool isFinded = false;
                foreach (var r in allRates)
                {
                    if (r.BookId == rateVM.RatedEssenceId && r.UserId == rateVM.UserId)
                    {
                        isFinded = true;
                        yourRate.Id = r.Id;
                        bookTORate.Rate += (yourRate.Value - r.Value) / bookTORate.RatesAmount;
                        _rateService.Update(yourRate);
                        _bookService.Update(bookTORate);
                        break;
                    }
                }
                if (!isFinded)
                {
                    uint amount = bookTORate.RatesAmount;
                    bookTORate.RatesAmount++;
                    bookTORate.Rate = (bookTORate.Rate * amount + yourRate.Value) / bookTORate.RatesAmount;
                    _bookService.Update(bookTORate);
                    _rateService.Add(yourRate);
                }
            }
            else
            {
                uint amount = bookTORate.RatesAmount;
                bookTORate.RatesAmount++;
                bookTORate.Rate = (bookTORate.Rate * amount + yourRate.Value) / bookTORate.RatesAmount;
                _bookService.Update(bookTORate);
                _rateService.Add(yourRate);
            }
            
            return RedirectToAction("GetBookInfo", "Home", new { id = bookTORate.Id });
        }

        [HttpPost]
        public IActionResult AddToWishList(WishListDTO model)
        {
            if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.BookId) || string.IsNullOrEmpty(model.Name))
            {
                return RedirectToAction("Error");
            }
            WishListDTO item = new WishListDTO
            {
                BookId = model.BookId,
                UserId = model.UserId,
                Name = model.Name
            };

            List<WishListDTO> wishListToFind = _wlService.Get(new WishListFullFilter { BookId = model.BookId, UserId = model.UserId, Name = model.Name }).ToList();
            if (wishListToFind.Any())
            {
                return RedirectToAction("GetBookInfo", "Home", new { id = model.BookId });
            }

            _wlService.Add(item);

            return RedirectToAction("GetBookInfo", "Home", new { id = model.BookId });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
