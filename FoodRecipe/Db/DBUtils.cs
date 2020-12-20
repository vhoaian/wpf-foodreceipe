using FoodRecipe.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;

namespace FoodRecipe.Db
{
    class DBUtils
    {
        public static List<Food> read()
        {
            List<Food> data = new List<Food>();

            XDocument xDocument = XDocument.Load("./Db/DB.xml");
            IEnumerable<XElement> foods = xDocument.Root.Elements();
            foreach (var foodEl in foods)
            {
                var foodItem = new Food();
                foodItem.Id = foodEl.Element("id").Value;
                foodItem.Name = foodEl.Element("name").Value;
                foodItem.Description = foodEl.Element("description").Value;
                foodItem.ThumbnailPath = foodEl.Element("thumbnailPath").Value;
                foodItem.IsFavorite = Boolean.Parse(foodEl.Element("isFavorite").Value);

                foreach(var fStep in foodEl.Element("steps").Elements())
                {
                    var stepItem = new FoodStep();
                    stepItem.StepName = fStep.Element("stepname").Value;
                    stepItem.DescriptionStep = fStep.Element("stepdescription").Value;
                    foreach (var fImg in fStep.Element("stepimagepaths").Elements())
                    {
                        stepItem.ImageStepPath.Add(fImg.Value);
                    }
                    foodItem.Steps.Add(stepItem);
                }

                data.Add(foodItem);
            }

            return data;
        }

        public static void write(Food myFood)
        {
            List<Food> listFood = read();
            listFood.Add(myFood);
            XmlWriter xmlWriter = XmlWriter.Create("./Db/DB.xml");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("foods");
            foreach (Food f in listFood)
            {
                xmlWriter.WriteStartElement("record");
                xmlWriter.WriteStartElement("id");
                myFood.Id = (listFood.Count).ToString();
                xmlWriter.WriteString(f.Id);
                xmlWriter.WriteEndElement(); //</id>
                xmlWriter.WriteStartElement("name");
                xmlWriter.WriteString(f.Name);
                xmlWriter.WriteEndElement(); //</name>
                xmlWriter.WriteStartElement("description");
                xmlWriter.WriteString(f.Description);
                xmlWriter.WriteEndElement(); //</description>
                xmlWriter.WriteStartElement("thumbnailPath");
                xmlWriter.WriteString(f.ThumbnailPath);
                xmlWriter.WriteEndElement(); //</thumbnailPath>
                xmlWriter.WriteStartElement("video");
                xmlWriter.WriteString(f.VideoLink);
                xmlWriter.WriteEndElement(); //</video>
                xmlWriter.WriteStartElement("isFavorite");
                xmlWriter.WriteString(f.IsFavorite.ToString());
                xmlWriter.WriteEndElement(); //</isFavorite>
                xmlWriter.WriteStartElement("steps");
                int index = 0;
                foreach (FoodStep step in f.Steps)
                {
                    xmlWriter.WriteStartElement("astep");
                    xmlWriter.WriteStartElement("stepname");
                    xmlWriter.WriteString(step.StepName);
                    xmlWriter.WriteEndElement(); //</stepname>
                    xmlWriter.WriteStartElement("stepdescription");
                    xmlWriter.WriteString(step.DescriptionStep);
                    xmlWriter.WriteEndElement(); //</stepdescription>
                    xmlWriter.WriteStartElement("stepimagepaths");
                    foreach (var path in step.ImageStepPath)
                    {
                        xmlWriter.WriteStartElement("stepimagepath");
                        xmlWriter.WriteString(path);
                        xmlWriter.WriteEndElement(); //</stepimagepath>
                    }
                    xmlWriter.WriteEndElement(); //</stepimagepaths>
                    xmlWriter.WriteEndElement(); //</astep>
                }

                xmlWriter.WriteEndElement(); //</steps>
                xmlWriter.WriteEndElement(); //</record>
            }
            xmlWriter.WriteEndDocument(); //</foods>
            xmlWriter.Close();
        }
    }
}
