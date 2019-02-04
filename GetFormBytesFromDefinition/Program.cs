using DecisionsFramework.ComponentData;
using DecisionsFramework.Design.Flow.Mapping;
using FormDefinitionToPassToConverterTing;
using Silverdark.Components;
using Silverdark.Designers.Forms.Containers;
using Silverdark.Designers.Forms.Containers.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GetFormBytesFromDefinition
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileIn = args[0];
            string fileOut = args[1];

            FileStream stream = new FileStream(fileIn, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            IntermediateFormDefinition halfConverted = (IntermediateFormDefinition)formatter.Deserialize(stream);

            byte[] result = DoConversion(halfConverted);

            File.WriteAllBytes(fileOut, result);

            System.Environment.Exit(0);
        }

        private static byte[] DoConversion(IntermediateFormDefinition halfConverted)
        {
            string outcomeName = "";

            FormSurface surface = FormSurfaceHelpers.Create(new Guid().ToString());
            SilverGrid grid = new SilverGrid();
            surface.RootContainer = grid;

            List<DataDescription> formData = new List<DataDescription>();

            grid.Columns = new[]
                    {
                        new SilverGridColumn() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 20 },
                        new SilverGridColumn() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Resize, RequestedSize = 10 },
                        new SilverGridColumn() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 5 },
                        new SilverGridColumn() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Resize, RequestedSize = 15 },
                        new SilverGridColumn() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 20 }
                    };

            List<SilverGridRow> rows = new List<SilverGridRow>();

            //if (Debugger.IsAttached == false) {
            //    Debugger.Launch();
            //}


            foreach (FormControl eachLBSFormControl in halfConverted.Controls)
            {

                rows.Add(new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 20 });
                SilverGridRow separatorRow = null;

                if (eachLBSFormControl.TypeName == "LabelComponent")
                {

                    formData.Add(new DataDescription((DecisionsNativeType)typeof(String), eachLBSFormControl.Name, false, true, false));

                    SilverLabel label = new SilverLabel();
                    label.Text = eachLBSFormControl.Text;
                    label.RequestedWidth = 100;
                    label.RequestedHeight = 25;
                    grid.Children.Add(new SilverGridChildInfo(label, 1, 1, rows.Count, 1));

                    rows.Add(new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 25 });
                    rows.Add(separatorRow = new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 5 });

                }
                else if (eachLBSFormControl.TypeName == "TextBoxComponent") 
                {
                    formData.Add(new DataDescription((DecisionsNativeType)typeof(String), eachLBSFormControl.Name, false, true, false));

                    SilverTextBox textBox = new SilverTextBox();
                    textBox.DataName = eachLBSFormControl.Value;
                    textBox.ComponentName = eachLBSFormControl.Name;
                    textBox.RequiredOnOutputs = new[] { outcomeName };
                    textBox.OutputOnly = true;
                    textBox.RequestedWidth = 100;
                    textBox.RequestedHeight = 25;
                    grid.Children.Add(new SilverGridChildInfo(textBox, 3, 1, rows.Count, 1));

                    rows.Add(new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 25 });
                    rows.Add(separatorRow = new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 5 });
                }
                else if (eachLBSFormControl.TypeName.EndsWith("ButtonComponent"))
                {
                    outcomeName = eachLBSFormControl.Value;
                    SilverButton submit = new SilverButton();
                    submit.OutcomePathName = eachLBSFormControl.Value;
                    submit.Text = eachLBSFormControl.Text;
                    submit.RequestedWidth = 100;
                    submit.RequestedHeight = 25;
                    if (separatorRow == null)
                        rows.Add(separatorRow = new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 75 });
                    grid.Children.Add(new SilverGridChildInfo(submit, 3, 1, rows.Count, 1));
                    rows.Add(new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 25 });
                    rows.Add(new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 20 });
                }

                grid.Rows = rows.ToArray();
                surface.DesignerWidth = grid.RequestedWidth = 375;
                surface.DesignerHeight = grid.RequestedHeight = rows.Sum(r => r.RequestedSize) + 70;
                if (separatorRow != null)
                {
                    separatorRow.LayoutType = LayoutElementType.Resize;
                    separatorRow.RequestedSize = 1;
                }
            }

            return surface.WriteSurface();

        }
    }
}
