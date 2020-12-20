using FoodRecipe.DTO;
using FoodRecipe.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FoodRecipe.DAO
{
    class FoodDAO
    {
        public static int GetLengthAll()
        {
            var result = 0;

            XDocument xdocument = XDocument.Load("./Db/DB.xml");
            IEnumerable<XElement> foodsElements = xdocument.Root.Elements();
            result = foodsElements.Count();

            return result;
        }

        public static int CountProducts(string searchKey, string searchMode)
        {
            var result = 0;

            XDocument xdocument = XDocument.Load("./Db/DB.xml");
            IEnumerable<XElement> foodsElements = xdocument.Root.Elements();
            

            if(searchKey == "")
            {
                foodsElements = xdocument.Root.Elements();
            } else
            {
                if (searchMode == "default")
                {
                    foodsElements = xdocument.Root.Elements();
                }
                else if (searchMode == "exact")
                {
                    foodsElements = foodsElements.Where(e => e.Element("name").Value == searchKey);
                }
                else
                {
                    foodsElements = foodsElements.Where(e => SearchHelper.ConvertToUnSign(e.Element("name").Value) == searchKey);
                }
            }
            

            result = foodsElements.Count();
            return result;
        }

        public static BindingList<Food> GetProducts(int perPage, int page, string sortBy, string searchKey, string searchMode)
        {
            var result = new BindingList<Food>();

            int skipValue = page == 1 ? 0 : (page - 1) * perPage;


            XDocument xdocument = XDocument.Load("./Db/DB.xml");
            IEnumerable<XElement> foodsElements = xdocument.Root.Elements();

            if(searchKey == "")
            {
                foodsElements = xdocument.Root.Elements();
            } else
            {
                if (searchMode == "default")
                {
                    foodsElements = xdocument.Root.Elements();
                }
                else if (searchMode == "exact")
                {
                    foodsElements = foodsElements.Where(e => e.Element("name").Value == searchKey);
                }
                else
                {
                    foodsElements = foodsElements.Where(e => SearchHelper.ConvertToUnSign(e.Element("name").Value) == searchKey);
                }
            }

            if (sortBy == "az")
            {
                foodsElements = foodsElements.OrderBy(s => s.Element("name").Value);
            }

            if (sortBy == "za")
            {
                foodsElements = foodsElements.OrderByDescending(s => s.Element("name").Value);
            }

            if (sortBy == "newold")
            {
                foodsElements = foodsElements.OrderByDescending(s => int.Parse(s.Element("id").Value));
            }

            if (sortBy == "oldnew")
            {
                foodsElements = foodsElements.OrderBy(s => int.Parse(s.Element("id").Value));
            }

            foodsElements = foodsElements.Skip(skipValue).Take(perPage);

            result = ConvertListXmlElementToFoods(foodsElements);

            return result;
        }

        public static int GetLengthFavs()
        {
            var result = 0;

            XDocument xdocument = XDocument.Load("./Db/DB.xml");
            IEnumerable<XElement> foods = xdocument.Root.Elements()
                .Where(el => Boolean.Parse(el.Element("isFavorite").Value));
            result = foods.Count();

            return result;
        }

        public static BindingList<Food> ConvertListXmlElementToFoods(IEnumerable<XElement> listElement)
        {
            var result = new BindingList<Food>();

            foreach (var xelement in listElement)
            {
                var foodItem = new Food();
                foodItem.Id = xelement.Element("id").Value;
                foodItem.Name = xelement.Element("name").Value;
                foodItem.Description = xelement.Element("description").Value;
                foodItem.ThumbnailPath = xelement.Element("thumbnailPath").Value;
                foodItem.IsFavorite = Boolean.Parse(xelement.Element("isFavorite").Value);

                result.Add(foodItem);
            }

            return result;
        }

        public static BindingList<Food> GetFavorites(int perPage, int page)
        {
            var result = new BindingList<Food>();

            int skipValue = page == 1 ? 0 : (page - 1) * perPage;

            XDocument xdocument = XDocument.Load("./Db/DB.xml");
            IEnumerable<XElement> foods = xdocument.Root.Elements()
                .Where(el => Boolean.Parse(el.Element("isFavorite").Value))
                .Skip(skipValue).Take(perPage);

            result = ConvertListXmlElementToFoods(foods);
            

            return result;
        }

        public static Food getById(string id)
        {
            dynamic result = null;

            XDocument xdocument = XDocument.Load("./Db/DB.xml");
            IEnumerable<XElement> foods = xdocument.Root.Elements();
            foreach(var foodEl in foods)
            {
                if(id == foodEl.Element("id").Value)
                {
                    var findedFood = new Food();
                    findedFood.Id = foodEl.Element("id").Value;
                    findedFood.Name = foodEl.Element("name").Value;
                    findedFood.Description = foodEl.Element("description").Value;
                    findedFood.ThumbnailPath = foodEl.Element("thumbnailPath").Value;
                    findedFood.VideoLink = foodEl.Element("video").Value;
                    findedFood.IsFavorite = Boolean.Parse(foodEl.Element("isFavorite").Value);
                    var steps = foodEl.Element("steps").Elements();
                    foreach (var stepEl in steps)
                    {
                        FoodStep fs = new FoodStep();
                        fs.DescriptionStep = stepEl.Element("stepdescription").Value;
                        fs.StepName = stepEl.Element("stepname")?.Value;
                        foreach (var img in stepEl.Element("stepimagepaths").Elements())
                        {
                            fs.ImageStepPath.Add(img.Value);
                        }
                        findedFood.Steps.Add(fs);
                    }
                    result = findedFood;
                    break;
                }
            }

            return result;
        }

        public static bool updateIsFavorite(string id, bool isFav)
        {
            var result = false;

            XDocument xdocument = XDocument.Load("./Db/DB.xml");
            IEnumerable<XElement> foods = xdocument.Root.Elements();
            foreach (var foodEl in foods)
            {
                if (id == foodEl.Element("id").Value)
                {
                    foodEl.SetElementValue("isFavorite", isFav.ToString());

                    result = true;
                    break;
                }
            }

            xdocument.Save("./Db/DB.xml");
            return result;
        }
    }
}