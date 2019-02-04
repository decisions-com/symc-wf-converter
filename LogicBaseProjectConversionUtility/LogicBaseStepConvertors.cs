using System;
using System.Linq;
using System.Collections.Generic;
using LogicBase.Components.Default.DateHandling;
using LogicBase.Components.Default.FlowControl;
using LogicBase.Components.Default.Process;
using LogicBase.Components.Default.Rules;
using LogicBase.Core;
using LogicBase.Core.Data.DataTypes;
using GetItemByIndex = LogicBase.Components.Default.Process.GetItemByIndex;
using LogicBaseProjectConversionUtility.ProjectConversionService;
using LogicBase.Components.Default.Communication;
using LogicBase.Components.Default.Logging;
using System.Text.RegularExpressions;
using System.Reflection;

namespace LogicBaseToDecisionsProjectConversionTool
{
    public class MatchesLogicBaseStepConverter : LogicBaseStepConverter
    {
       
        //public override string ConvertPath(string originalPath)
        //{
        //    if (originalPath.Equals("no match", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return "No Matches";
        //    }
        //    return originalPath;
        //}

        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            MatchesRule comp = (MatchesRule)component;

            StepInput[] results = new StepInput[1];
            results[0] = new StepInput();
            results[0].Name = "String to Match";
            results[0].MappingType = InputMappingType.SelectValue;
            results[0].SelectValuePathName = comp.CompareToVariable;

            return results;
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return null;
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            MatchesRule comp = (MatchesRule)component;
            string[] matches = comp.CompareToList;
            return new StepInput[] { new StepInput() { Name = "PossibleMatches", ConstantValue = string.Join(",", matches) } };

        }
    }

    //public class AssignDataStepConvertor : LogicBaseStepConverter
    //{
    //    public override FlowStep CreateStep(IOrchestrationComponent component)
    //    {
    //        AssignDataComponent comp = (AssignDataComponent)component;
    //        CreateDataStep ps = new CreateDataStep();

    //        FlowStep newFS = new FlowStep(ps);
    //        newFS.Name = component.Name;

    //        // Map Inputs
    //        ps.DataDefinitions = new PlaceholderData[]
    //                        {
    //                            new PlaceholderData(new DecisionsNativeType(comp.DataType), comp.DataFromVariableName, comp.GetAddedData()[0].IsArray)
    //                        };

    //        return newFS;
    //    }

    //    public override string ConvertPath(string originalPath)
    //    {
    //        return "done";
    //    }

    //    public override Tuple<StepInput[], StepOutcome[]> GetMappings(IOrchestrationComponent component)
    //    {

    //        Tuple<StepInput[], StepOutcome[]> results = new Tuple<StepInput[], StepOutcome[]>();

    //        AssignDataComponent comp = (AssignDataComponent)component;
    //        // Map Inputs
    //        results.Item1 = new StepInput[1];
    //        results.Item1[0] = new StepInput()
    //        {
    //            MappingType = InputMappingType.SelectValue,
    //            Name = ((AssignDataComponent)component).DataFromVariableName,
    //            SelectValuePathName = ((AssignDataComponent)component).DataFromVariableName
    //        };

    //        results.Item2 = new StepOutcome[1];
    //        results.Item2[0] = new StepOutcome()
    //        {
    //            // Map outputs
    //            OutputMappingType = OutputMappingtype.Rename,
    //            RenameName = comp.GetAddedData()[0].Name
    //        };

    //        return results;
    //    }
    //}

    //public class InsertDataStepConvertor : LogicBaseStepConverter
    //{
    //    public override FlowStep CreateStep(IOrchestrationComponent component)
    //    {
    //        InsertDataComponent comp = (InsertDataComponent)component;
    //        PlaceholderStep ps = new PlaceholderStep();
    //        FlowStep fs = new FlowStep(ps);
    //        fs.Name = component.Name;

    //        ps.Outcomes = new PlaceholderOutcome[]
    //                          {
    //                              new PlaceholderOutcome("done", new PlaceholderData[]
    //                                                                  {
    //                                                                      new PlaceholderData(new DecisionsNativeType(comp.DataType), comp.GetAddedData()[0].Name, comp.GetAddedData()[0].IsArray)
    //                                                                 })
    //                            };

    //        // Map outputs
    //        ps.SelectedPath = "done";

    //        return fs;

    //    }

    //    public override string ConvertPath(string originalPath)
    //    {
    //        return "done";
    //    }

    //    public override FlowStep DoMappings(FlowStep result, IOrchestrationComponent component)
    //    {
    //        InsertDataComponent comp = (InsertDataComponent)component;

    //        result.ShowAsRule = false;

    //        // When you add an outcome scenario and call 'OutputMappings'
    //        // the rename mapping gets created automatically.  so now we 
    //        // need to edit it
    //        RenameOutputMapping mapping = (RenameOutputMapping)result.OutputMapping[0];
    //        mapping.DataName = comp.GetAddedData()[0].Name;

    //        return result;
    //    }
    //}

    //public class TextEqualsStepConvertor : LogicBaseStepConverter
    //{
    //    public override FlowStep CreateStep(IOrchestrationComponent component)
    //    {
    //        FlowStep result;
    //        InvokeMethodStep ms = new InvokeMethodStep("DecisionsFramework.Design.Flow.CoreSteps.StandardSteps.StringSteps", "EqualsString", null);
    //        result = new FlowStep(ms);
    //        result.Name = component.Name;

    //        List<IInputMapping> inputs = new List<IInputMapping>();
    //        SelectValueInputMapping svim = new SelectValueInputMapping();
    //        svim.InputDataName = "value1";
    //        svim.DataPath = ((TextEqualsRule)component).VariableName;
    //        inputs.Add(svim);

    //        VariableOrValueDataType var = ((TextEqualsRule)component).CompareVariable;
    //        IInputMapping inputMapping = VVDTUtility.ConvertVariableOrValueDataTypeToInputMapping(var, "value2");
    //        if (inputMapping != null)
    //            inputs.Add(inputMapping);

    //        // Now stick these inputs into the flow step to be added to the flow.);
    //        foreach (IInputMapping eachMapping in inputs)
    //        {
    //            result.AddInputMapping(eachMapping);
    //        }
    //        return result;
    //    }

    //    public override string ConvertPath(string originalPath)
    //    {
    //        return originalPath;
    //    }

    //    public override FlowStep DoMappings(FlowStep result, IOrchestrationComponent component)
    //    {
    //        return result;
    //    }
    //}

    //public class TextExistsStepConvertor : LogicBaseStepConverter
    //{
    //    public override FlowStep CreateStep(IOrchestrationComponent component)
    //    {
    //        FlowStep result;
    //        InvokeMethodStep ms = new InvokeMethodStep("DecisionsFramework.Design.Flow.CoreSteps.StandardSteps.StringSteps", "StringIsNullOrEmpty", null);
    //        result = new FlowStep(ms);
    //        result.Name = component.Name;

    //        // Use Equals String against Null value and map true -> false.

    //        List<IInputMapping> inputs = new List<IInputMapping>();
    //        SelectValueInputMapping svim = new SelectValueInputMapping();
    //        svim.InputDataName = "value1";
    //        svim.DataPath = ((TextExists)component).VariableName;
    //        inputs.Add(svim);

    //        IInputMapping inputMapping = new NullInputMapping();
    //        inputMapping.InputDataName = "value2";
    //        inputs.Add(inputMapping);

    //        // Now stick these inputs into the flow step to be added to the flow.);
    //        foreach (IInputMapping eachMapping in inputs)
    //        {
    //            result.AddInputMapping(eachMapping);
    //        }
    //        return result;
    //    }

    //    public override string ConvertPath(string originalPath)
    //    {
    //        // Flip the paths since we're checking for == null;
    //        if (originalPath == TextExists.EXISTS)
    //        {
    //            return "false";
    //        }
    //        return "true";
    //    }
    //    public override FlowStep DoMappings(FlowStep result, IOrchestrationComponent component)
    //    {
    //        return result;
    //    }
    //}

    public class EqualsRuleConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }

        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            //if (((EqualsRule)component).DataType
            StepInput input1 = new StepInput() { 
                Name = "value1",
                SelectValuePathName = ((EqualsRule)component).VariableName
            };
            VariableOrValueDataType var = ((EqualsRule)component).CompareTo;
            StepInput input2 = VVDTUtility.ConvertVariableOrValueDataTypeToInputMapping(var, "value2");
           

            return new StepInput[] {
                input1, input2
            };
            

        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[0];
        }


        
    }

    public class StartStepConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[0];
        }


        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }
    }

    public class EndStepConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            List<StepInput> allInputs = new List<StepInput>();

            EndComponent ec = (EndComponent)component;
            DataMappingCollection outputMapping = ec.Mapping;

            foreach (DataMapping mapping in outputMapping)
            {
                allInputs.Add(new StepInput() { 
                    Name = mapping.MappedVariableName
                });
            }

            return allInputs.ToArray();
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[0];
        }


        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }

        
    }

    public class GoToComponentConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            return new StepInput[] {
                new StepInput() {
                    Name = "GoToStepName",
                    ConstantValue = ((GoToComponentByName)component).ComponentName
                }
            };
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[0];
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }

        
    }

    public class CompareNumbersConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            StepInput input1 = VVDTUtility.ConvertVariableOrValueDataTypeToInputMapping(((CompareNumbersRule)component).Value1, "value1");
            StepInput input2 = VVDTUtility.ConvertVariableOrValueDataTypeToInputMapping(((CompareNumbersRule)component).Value2, "value2");
            
            return new StepInput[] {
                input1, input2
            };
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[0];
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }
    }

    public class GetCurrentDateConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[] {
                new StepOutcome() {
                    PathName = "Done",
                    OutcomeData = new DataDefinition[] {
                        new DataDefinition() {
                            FullTypeName = typeof(DateTime).FullName,
                            Name = ((GetCurrentDate)component).OutputVariableName
                        }
                    }

                }
            };
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }
    }

    public class ItemByIndexConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            string arrayName = ((GetItemByIndex)component).ArrayVariableName;
            StepInput itemIndex = VVDTUtility.ConvertVariableOrValueDataTypeToInputMapping(((GetItemByIndex)component).IndexOfItem, "IndexOfItem");

            return new StepInput[2] {
                new StepInput() { Name = "ArrayVariableName", MappingType = InputMappingType.SelectValue, SelectValuePathName = arrayName },
                itemIndex
            };
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[] {
                new StepOutcome() {
                    PathName = "Done",
                    OutcomeData = new DataDefinition[] {
                        new DataDefinition() {
                            FullTypeName = ((GetItemByIndex)component).ArrayVariableType.FullName,
                            Name = ((GetItemByIndex)component).ItemOutputVariableName
                        }
                    }

                }
            };
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            string arrayType = ((GetItemByIndex)component).ArrayVariableType.FullName;

            return new StepInput[1] {
                new StepInput() { ConstantValue = arrayType }
            };

        }
    }

    public class IndexOfItemInListConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            string arrayName = ((IndexOfItemInCollection)component).ArrayVariableName;
            StepInput itemItself = VVDTUtility.ConvertVariableOrValueDataTypeToInputMapping(((IndexOfItemInCollection)component).Item, "Item");

            return new StepInput[2] {
                new StepInput() { Name = "ArrayVariableName", MappingType = InputMappingType.SelectValue, SelectValuePathName = arrayName },
                itemItself
            };
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[] {
                new StepOutcome() {
                    PathName = "Done",
                    OutcomeData = new DataDefinition[] {
                        new DataDefinition() {
                            FullTypeName = ((IndexOfItemInCollection)component).DataType.FullName,
                            Name = ((IndexOfItemInCollection)component).ArrayCountVariableName
                        }
                    }

                }
            };
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            string arrayType = ((IndexOfItemInCollection)component).DataType.FullName;

            return new StepInput[1] {
                new StepInput() { ConstantValue = arrayType }
            };

        }
    }

    public class ParseDateConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            StepInput dateFormat = VVDTUtility.ConvertVariableOrValueDataTypeToInputMapping(((ConvertStringToDate)component).ExactDatetimeFormat, "DateFormat");

            return new StepInput[2] {
                new StepInput() { Name = "InputVariableName", MappingType = InputMappingType.SelectValue, SelectValuePathName =  ((ConvertStringToDate)component).InputVariableName},
                dateFormat
            };
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[] {
                new StepOutcome() {
                    PathName = "Done",
                    OutcomeData = new DataDefinition[] {
                        new DataDefinition() {
                            FullTypeName = typeof(DateTime).FullName,
                            Name = ((ConvertStringToDate)component).OutputVariableName
                        }
                    }

                }
            };
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }
    }

    public class ParseNumberConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            return new StepInput[1] {
                new StepInput() { Name = "InputVariableName", MappingType = InputMappingType.SelectValue, SelectValuePathName =  ((GetNumberFromString)component).InputVariableName }
            };
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[] {
                new StepOutcome() {
                    PathName = "Done",
                    OutcomeData = new DataDefinition[] {
                        new DataDefinition() {
                            FullTypeName = typeof(decimal).FullName,
                            Name = ((GetNumberFromString)component).OutputVariableName
                        }
                    }

                }
            };
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
           return new StepInput[0];
        }
    }

    public class LinkedModelConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            // Linked model outputs will need to be recreated to work well
            // so no need to worry about the outcomes from the old engine here.
            return new StepOutcome[0];
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            string linkedFlowId = ((LinkedModelComponent)component).ComponentModelName;

            return new StepInput[1] {
                new StepInput() { Name = "LinkedFlowName", ConstantValue = linkedFlowId }
            };
        }

    }

    public class EmbeddedModelConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            // Linked model outputs will need to be recreated to work well
            // so no need to worry about the outcomes from the old engine here.
            List<StepOutcome> results = new List<StepOutcome>();
            StepOutcome result = new StepOutcome();
            result.PathName = "End";

            List<DataDefinition> data = new List<DataDefinition>();
            foreach (LogicBase.Core.Data.DataDefinition x in component.Model.OutputData)
            {
                data.Add(new DataDefinition() { 
                    Name = x.Name,
                    IsList = x.IsArray,
                    FullTypeName = x.DataType.FullName
                });
            }
            result.OutcomeData = data.ToArray();
            return results.ToArray();
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {

            FieldInfo[] fields = component.GetType().GetFields(
                        BindingFlags.NonPublic |
                        BindingFlags.Instance);

            IComponentModel internalModel = (IComponentModel)fields.First(xx => xx.Name == "_embeddedModel").GetValue(component);

            string linkedFlowId = internalModel.Id;

            return new StepInput[1] {
                new StepInput() { Name = "EmbeddedFlowId", ConstantValue = linkedFlowId }
            };
        }

    }

    public class ShortDateConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            StepInput dateValue = VVDTUtility.ConvertVariableOrValueDataTypeToInputMapping(((ToShortDateString)component).date, "Date");

            return new StepInput[1] {
                dateValue
            };
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[] {
                new StepOutcome() {
                    PathName = "Done",
                    OutcomeData = new DataDefinition[] {
                        new DataDefinition() {
                            FullTypeName = typeof(string).FullName,
                            Name = ((ToShortDateString)component).ResultVariableName
                        }
                    }

                }
            };
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }
    }

    public class VariableExistsRuleConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            StepInput varNameToTest = new StepInput()
            {
                Name = ((VariableExistsRule)component).VariableName,
                MappingType = InputMappingType.SelectValue
            };
                
            return new StepInput[1] {
                varNameToTest
            };
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[] {
                new StepOutcome() {
                    PathName = "Done",
                    OutcomeData = new DataDefinition[0]
                }
            };
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }
    }

    public class CreateTextFromArrayConvertor : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            StepInput arrayToMerge = VVDTUtility.ConvertArrayBuilderDataType(((BuildTextFromElements)component).Values);

            return new StepInput[1] {
                arrayToMerge
            };
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[] {
                new StepOutcome() {
                    PathName = "Done",
                    OutcomeData = new DataDefinition[] {
                        new DataDefinition() {
                            FullTypeName = typeof(string).FullName,
                            Name = ((BuildTextFromElements)component).OutputVariableName
                        }
                    }
                }
            };
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[1] {
                new StepInput() {
                    ConstantValue = ((BuildTextFromElements)component).DelimiterText,
                    Name = "separator"
                }
            };
        }
    }

    public class CreateNSCredentialsConverter : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {

            return new StepInput[0];
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[] {
                new StepOutcome() {
                    PathName = "Done",
                    OutcomeData = new DataDefinition[0]
                }
            };
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }
    }

    public class SendEmailStepConverter : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            StepInput fromAddy = VVDTUtility.ConvertVariableOrValueDataTypeToInputMapping(((SendEmailComponent)component).FromAddress, "From");

            StepInput toAddresses = VVDTUtility.ConvertArrayBuilderDataType(((SendEmailComponent)component).ToAddresses);
            toAddresses.Name = "To";

            StepInput subject = VVDTUtility.ConvertAsciiMergeDataTypeToInputMapping(((SendEmailComponent)component).Subject, "Subject");

            StepInput body = VVDTUtility.ConvertHTMLMergeDataTypeToInputMapping(((SendEmailComponent)component).HtmlContent, "Body");

            return new StepInput[4] {
                fromAddy,
                toAddresses,
                subject,
                body
            };
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[] {
                new StepOutcome() {
                    PathName = "Done",
                    OutcomeData = new DataDefinition[0]
                }
            };
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }
    }

    //This is not done. Not sure how do it.
    public class InsertDataStepConverter : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {
            StepInput si = new StepInput();

            si.ConstantValue = ((InsertDataComponent)component).Value.ToString();
            si.FullTypeName = ((InsertDataComponent)component).DataType.FullName;
            si.Name = ((InsertDataComponent)component).VariableName;
            si.MappingType = InputMappingType.Constant;

            return new StepInput[1] { si };
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            DataDefinition dd = new DataDefinition();
            dd.FullTypeName = ((InsertDataComponent)component).DataType.FullName;
            dd.IsList = ((InsertDataComponent)component).IsArray;
            dd.Name = ((InsertDataComponent)component).VariableName;
            
            return new StepOutcome[] {
                new StepOutcome() {
                    PathName = "Done",
                    OutcomeData = new DataDefinition[1] { dd }
                }
            };
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }
    }

    public class CreateLogEntryComponentConverter : LogicBaseStepConverter
    {
        public override StepInput[] GetInputs(IOrchestrationComponent component)
        {

            StepInput logText = VVDTUtility.ConvertAsciiMergeDataTypeToInputMapping(((CreateLogEntryComponent)component).LogEntryText, "Value");

            return new StepInput[1] { logText };
        }

        public override StepOutcome[] GetOutcomes(IOrchestrationComponent component)
        {
            return new StepOutcome[] {
                new StepOutcome() {
                    PathName = "Done",
                    OutcomeData = new DataDefinition[0]
                }
            };
        }

        public override StepInput[] GetProperties(IOrchestrationComponent component)
        {
            StepInput logType = new StepInput()
            {
                ConstantValue = ((CreateLogEntryComponent)component).LogEntryLevel.ToString(),
                Name = "Type"
            };

            StepInput logCat = new StepInput()
            {
                ConstantValue = "Process Log",
                Name = "Category"
            };
            
            return new StepInput[2] { logType, logCat};
        }
    }
}
