using FoodRecipe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FoodRecipe.DAO
{
    class FoodPreviewDAO
    {
        public static BindingList<FoodPreview> GetAll()
        {
            var result = new BindingList<FoodPreview>();

            XDocument xdocument = XDocument.Load("./Db/PreviewFoods.xml");
            IEnumerable<XElement> foods = xdocument.Root.Elements();
            foreach (var foodEl in foods)
            {
                var foodItem = new FoodPreview();
                foodItem.Id = foodEl.Element("id").Value;
                foodItem.ImageNameFile = foodEl.Element("imageNameFile").Value;
                foodItem.Intro = foodEl.Element("intro").Value;

                result.Add(foodItem);
            }

            return result;
        }
    }
}
