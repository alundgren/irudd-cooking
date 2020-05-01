using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace irudd_cooking.Pages
{
    public class RecipeModel : PageModel
    {
        private readonly ILogger<RecipeModel> _logger;

        public RecipeModel(ILogger<RecipeModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
