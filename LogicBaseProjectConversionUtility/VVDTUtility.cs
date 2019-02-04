using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicBase.Core.Data.DataTypes;
using LogicBaseProjectConversionUtility.ProjectConversionService;
using System.Text.RegularExpressions;

namespace LogicBaseToDecisionsProjectConversionTool
{
    public class VVDTUtility
    {
        public static StepInput ConvertAsciiMergeDataTypeToInputMapping(AsciiMergeDataType var, string newVariableName)
        {
            StepInput result = new StepInput()
            {
                MappingType = InputMappingType.MergeText, 
                ConstantValue = CleanUpAsciiMergeText(var.PreMergedData),
                Name = newVariableName
            };

            return result;
        }

        public static StepInput ConvertHTMLMergeDataTypeToInputMapping(HTMLMergeDataType var, string newVariableName)
        {
            StepInput result = new StepInput()
            {
                MappingType = InputMappingType.MergeText,
                ConstantValue = CleanUpProjectPropertyReferenceInText(var.PreMergedData),
                Name = newVariableName
            };

            return result;
        }
        public static StepInput ConvertVariableOrValueDataTypeToInputMapping(VariableOrValueDataType var, string newVariableName)
        {
            StepInput result = new StepInput();
            if (var != null)
            {
                if (var.IsFilledIn())
                {
                    if (var.IsConstant())
                    {
                        result.MappingType = InputMappingType.Constant;
                        result.Name = newVariableName;
                        result.ConstantValue = var.DefaultValue.ToString();
                        
                        return result;
                    }
                    else
                    {
                        // It's probably a variable or a merge
                        if (var.HasMergeData)
                        {
                            result.MappingType = InputMappingType.MergeText;
                            result.ConstantValue = var.StringMerge.PreMergedData;
                            result.Name = newVariableName;
                            return result;
                        }
                        else
                        {
                            // It's just a variable
                            result.MappingType = InputMappingType.SelectValue;
                            result.Name = newVariableName;
                            result.SelectValuePathName = var.Variables[0].ToString();

                            return result;
                        }
                    }

                }
            }
            return null;
        }

        public static StepInput ConvertArrayBuilderDataType(ArrayBuilderDataType var)
        {

            StepInput newArray = new StepInput();
            newArray.MappingType = InputMappingType.ArrayBuilder;

            List<StepInput> allInputs = new List<StepInput>();

            if (var != null)
            {
                if (var.IsFilledIn())
                {
                    if (var.Values != null && var.Values.Length > 0)
                    {
                        foreach (object o in var.Values)
                        {
                            StepInput input = new StepInput();
                            input.MappingType = InputMappingType.Constant;
                            input.ConstantValue = o == null ? null : o.ToString();
                            allInputs.Add(input);
                        }
                    }
                    else
                    {
                       // It's a var array
                        foreach (object o in var.Variables)
                        {
                            StepInput selectInput = new StepInput();
                            selectInput.MappingType = InputMappingType.SelectValue;
                            selectInput.SelectValuePathName = o.ToString();
                            allInputs.Add(selectInput);
                        }
                    }

                }
            }
            newArray.ArrayParts = allInputs.ToArray();
            return newArray;
        }

        public static string CleanUpProjectPropertyReferenceInText(string input)
        {
            //need to find way to clean up project properties references in text.
            //Below is a sample of what it looks like

            //<A 
            //id=8c7651eb-790c-43ef-8390-b01f8fe7a9e7 contentEditable=true 
            //href="mergefield://8c7651eb-790c-43ef-8390-b01f8fe7a9e7/" atomicselection="true" 
            //unselectable="off">[ProjectProperties].Version</A>

            return input;
        }
        public static string CleanUpAsciiMergeText(string input)
        {
            input = CleanUpProjectPropertyReferenceInText(input);

            //get HTML out of this string
            input = Regex.Replace(input, "<.*?>", string.Empty);

            return input;
        }
    }
}
