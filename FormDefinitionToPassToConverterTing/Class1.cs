using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormDefinitionToPassToConverterTing
{
    [Serializable]
    public class IntermediateFormDefinition
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public FormControl[] Controls { get; set; }



    }
    [Serializable]
    public class FormControl
    {
        public int Order { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string TypeName { get; set; }

        public string Value { get; set; }

        public string Text { get; set; }

    }
}
