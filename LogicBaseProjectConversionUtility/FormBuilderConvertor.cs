using FormDefinitionToPassToConverterTing;
using LogicBase.Components.FormBuilder;
using LogicBase.Core;
using LogicBaseProjectConversionUtility.ProjectConversionService;
using LogicBaseToDecisionsProjectConversionTool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace LogicBaseProjectConversionUtility
{
    public class FormBuilderConvertor
    {
        //public static void WriteFormData(LogicBase.Core.IOrchestrationComponent component)
        //{
        //    FormBuilderComponent fbc = ((FormBuilderComponent)component);

        //    IOrchestrationComponent[] ioc = fbc.FormModel.Components;

        //    FormSurface surface = FormSurfaceHelpers.Create();
        //    SilverGrid grid = new SilverGrid();
        //    surface.RootContainer = grid;

        //    List<DataDescription> formData = new List<DataDescription>();

        //    grid.Columns = new[]
        //        {
        //            new SilverGridColumn() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 20 },
        //            new SilverGridColumn() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Resize, RequestedSize = 10 },
        //            new SilverGridColumn() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 5 },
        //            new SilverGridColumn() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Resize, RequestedSize = 15 },
        //            new SilverGridColumn() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 20 }
        //        };

        //    List<SilverGridRow> rows = new List<SilverGridRow>();

        //    foreach (IOrchestrationComponent eachLBSFormControl in ioc)
        //    {

        //        rows.Add(new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 20 });
        //        SilverGridRow separatorRow = null;

        //        if (eachLBSFormControl is ILabelComponent)
        //        {

        //            ILabelComponent lbLabel = (ILabelComponent)eachLBSFormControl;
        //            formData.Add(new DataDescription((DecisionsNativeType)typeof(String), lbLabel.Name, false, true, false));

        //            SilverLabel label = new SilverLabel();
        //            label.Text = lbLabel.Name;
        //            label.RequestedWidth = 100;
        //            label.RequestedHeight = 25;
        //            grid.Children.Add(new SilverGridChildInfo(label, 1, 1, rows.Count, 1));

        //            //SilverTextBox textBox = new SilverTextBox();
        //            //textBox.DataName = dataName;
        //            //textBox.RequiredOnOutputs = new[] { outcomeName };
        //            //textBox.RequestedWidth = 100;
        //            //textBox.RequestedHeight = 25;
        //            //grid.Children.Add(new SilverGridChildInfo(textBox, 3, 1, rows.Count, 1));

        //            rows.Add(new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 25 });
        //            rows.Add(separatorRow = new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 5 });

        //        }
        //        else if (eachLBSFormControl is ButtonComponent)
        //        {

        //            ButtonComponent button = (ButtonComponent)eachLBSFormControl;

        //            SilverButton submit = new SilverButton();
        //            submit.OutcomePathName = button.PathName;
        //            submit.Text = button.Text;
        //            submit.RequestedWidth = 100;
        //            submit.RequestedHeight = 25;
        //            if (separatorRow == null)
        //                rows.Add(separatorRow = new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 75 });
        //            grid.Children.Add(new SilverGridChildInfo(submit, 3, 1, rows.Count, 1));
        //            rows.Add(new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 25 });
        //            rows.Add(new SilverGridRow() { Id = Guid.NewGuid(), LayoutType = LayoutElementType.Fixed, RequestedSize = 20 });
        //            grid.Rows = rows.ToArray();
        //            surface.DesignerWidth = grid.RequestedWidth = 375;
        //            surface.DesignerHeight = grid.RequestedHeight = rows.Sum(r => r.RequestedSize) + 70;
        //            separatorRow.LayoutType = LayoutElementType.Resize;
        //            separatorRow.RequestedSize = 1;
        //        }
        //    }

        //    newForm.FormData = surface.WriteSurface();

        //    return newForm;
            
        //}

        public static ConvertedForm GetForm(LogicBase.Core.IOrchestrationComponent component)
        {
            FormBuilderComponent fbc = ((FormBuilderComponent)component);

            IOrchestrationComponent[] ioc = fbc.FormModel.Components;

            if (ioc == null || ioc.Length == 0) {
                throw new Exception("Oh snap!");
            }

            IntermediateFormDefinition newForm = new IntermediateFormDefinition();
            newForm.Id = fbc.Name;
            newForm.Name = fbc.Name;
            
            List<FormControl> allControls = new List<FormControl>();
            foreach (IOrchestrationComponent eachLBSFormControl in ioc)
            {

                FormControl newControl = new FormControl();
                newControl.Name = eachLBSFormControl.Name;
                newControl.X = (int)eachLBSFormControl.Location.X;
                newControl.Y = (int)eachLBSFormControl.Location.Y;
                newControl.TypeName = eachLBSFormControl.GetType().Name;
                newControl.Id = eachLBSFormControl.Id;

                if (eachLBSFormControl is LabelComponent)
                {
                    LabelComponent label = ((LabelComponent)eachLBSFormControl);
                    newControl.Text = label.Text;
              
                }
                else if (eachLBSFormControl is TextBoxComponent)
                {

                    TextBoxComponent button = (TextBoxComponent)eachLBSFormControl;

                    newControl.Text = button.Name;
                    string value = button.OutputData;
                    newControl.Value = value.Substring(value.LastIndexOf("]") + 1);

                }
                else if (eachLBSFormControl is ButtonComponent)
                {

                    ButtonComponent button = (ButtonComponent)eachLBSFormControl;

                    newControl.Path = button.PathName;
                    newControl.Text = button.Text;
                   
                }
                allControls.Add(newControl);
            }

            newForm.Controls = allControls.ToArray();

            string tempFileName = Path.GetTempFileName();
            string newFileName = Path.GetTempFileName();
            // Call the other Thing.
            FileStream stream = new FileStream(tempFileName, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, newForm);
            stream.Close();

            string pathToConverter = @"..\..\..\GetFormBytesFromDefinition\bin\Debug\GetFormBytesFromDefinition.exe";

            // Run the other exe
            Process p = System.Diagnostics.Process.Start(pathToConverter, string.Format("\"{0}\" \"{1}\"", tempFileName, newFileName));
            p.WaitForExit();

            ConvertedForm form = new ConvertedForm();
            form.FormId = component.Id;
            form.FormName = component.Name;

            try { 

            // Now read in new file name.
            byte[] allData = File.ReadAllBytes(newFileName);

            if (allData != null)
            {
                form.FormData = allData;
            }
            } catch {
                MessageBox.Show("Error prevented form from being converted.");
            }

            return form;

        }

      
    }
}
