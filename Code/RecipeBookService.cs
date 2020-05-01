using Microsoft.AspNetCore.Hosting;

namespace irudd_cooking.Code
{
    public class RecipeBookService
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private RecipeBook recipeBook;

        public RecipeBookService(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public RecipeBook GetRecipeBook(bool forceReload = false)
        {
            if (forceReload || recipeBook == null)
            {
                recipeBook = RecipeBook.CreateFromRecipeFolder(webHostEnvironment.WebRootFileProvider.GetDirectoryContents("recipies"));
            };
            return recipeBook;
        }
    }
}