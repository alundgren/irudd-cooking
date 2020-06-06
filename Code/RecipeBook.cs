using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace irudd_cooking.Code
{
    public class RecipeBook
    {
        public List<Recipe> Recipes { get; set; }
        public class Recipe
        {
            public string Name { get; set; }
            public string MainImageUrl { get; set; }
            public List<string> Steps { get; set; }
            public List<string> Comments { get; set; }
            public List<Ingredient> Ingredients { get; set; }
        }

        public class Ingredient
        {
            public string Name { get; set; }
            public string Quantity { get; set; }
            public int? Kcal { get; set; }
        }

        public static RecipeBook CreateFromRecipeFolder(IDirectoryContents folder)
        {
            var b = new RecipeBook
            {
                Recipes = new List<Recipe>()
            };
            foreach (var file in folder)
            {
                if (file.Name.EndsWith(".txt"))
                {
                    using (var s = file.CreateReadStream())
                    using (var r = new StreamReader(s, Encoding.UTF8))
                    {
                        var lines = new List<string>();
                        string line;
                        while ((line = r.ReadLine()) != null)
                            lines.Add(line);

                        b.Recipes.Add(CreateFromFileLines(lines, Path.GetFileNameWithoutExtension(file.Name)));
                    }
                }
            }
            return b;
        }

        public static Recipe CreateFromFileLines(List<string> lines, string name)
        {
            var r = new Recipe
            {
                Name = name,
                Steps = new List<string>(),
                Ingredients = new List<Ingredient>(),
                Comments = new List<string>()
            };

            string currentSection = null;

            foreach (var lineRaw in lines)
            {
                var line = lineRaw?.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }
                else if (line == "i:" || line == "s:" || line == "k:" || line == "n:" || line == "b:")
                {
                    currentSection = line.Substring(0, 1);
                }
                else if (currentSection == "n")
                {
                    r.Name = line;
                }
                else if (currentSection == "i")
                {
                    var parts = line.Split(";").Select(x => x?.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    var i = new Ingredient
                    {
                        Name = parts[0]
                    };
                    foreach (var p in parts.Skip(1))
                    {
                        if (p.ToLowerInvariant().EndsWith("kcal"))

                            i.Kcal = int.Parse(new string(p.Where(char.IsDigit).ToArray()));
                        else
                            i.Quantity = p;
                    }
                    r.Ingredients.Add(i);
                }
                else if (currentSection == "s")
                {
                    r.Steps.Add(line);
                }
                else if (currentSection == "k")
                {
                    r.Comments.Add(line);
                }
                else if (currentSection == "b")
                {
                    r.MainImageUrl = line;
                }
            }


            return r;
        }
    }
}