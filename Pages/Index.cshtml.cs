using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using irudd_cooking.Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace irudd_cooking.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;

        public IndexModel(ILogger<IndexModel> logger, Code.RecipeBookService recipeBookService)
        {
            this.logger = logger;
            RecipeBookService = recipeBookService;
        }

        public RecipeBookService RecipeBookService { get; }

        public RecipeBook Book { get; set; }

        public void OnGet()
        {
            this.Book = RecipeBookService.GetRecipeBook();
        }
    }
}
