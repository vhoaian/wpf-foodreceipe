using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecipe.DTO
{
    class FoodStep
    {
        public string StepName { get; set; }
        public string DescriptionStep { get; set; }
        public List<string> ImageStepPath { get; set; } = new List<string>();
    }
}
