using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using irudd_cooking.Code;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace irudd_cooking.Pages
{
    public class RecipeModel : PageModel
    {
        private readonly ILogger<RecipeModel> logger;
        private readonly RecipeBookService recipeBookService;

        public RecipeModel(ILogger<RecipeModel> logger, Code.RecipeBookService recipeBookService)
        {
            this.logger = logger;
            this.recipeBookService = recipeBookService;
        }

        public RecipeBook.Recipe Recipe { get; set; }

        public int? GetKcalSum()
        {
            if (Recipe == null)
                return null;
            var sum = 0;
            var any = false;
            foreach (var i in Recipe.Ingredients)
            {
                if (i.Kcal.HasValue)
                {
                    sum += i.Kcal.Value;
                    any = true;
                }

            }
            return any ? sum : new int?(); //Dont print 0 when the writer hasnt given any kcal value at all
        }

        public int? GetKcalPerPortion()
        {
            if ((Recipe?.NrOfPortions ?? 0) == 0)
                return null;
            var kcals = GetKcalSum();
            if (kcals == null)
                return null;
            return (int)Math.Round(((decimal)kcals) / ((decimal)Recipe.NrOfPortions.Value));
        }

        public IActionResult OnGet(string name)
        {
            this.Recipe = this.recipeBookService.GetRecipeBook().Recipes.FirstOrDefault(x => x.Name == name);
            if (this.Recipe == null)
                return NotFound();
            else
                return Page();
        }
    }
}
