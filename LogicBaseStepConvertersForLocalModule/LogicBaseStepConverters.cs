using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.CoreSteps;
using DecisionsFramework.Design.Flow.CoreSteps.StandardSteps;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.Mapping.InputImpl;
using DecisionsFramework.Design.Flow.Mapping.OutputImpl;
using DecisionsFramework.Design.Flow.StepImplementations;
using DecisionsFramework.Design.Form;
using DecisionsFramework.ProjectConversion;
using DecisionsFramework.ServiceLayer.Utilities;
using DecisionsFramework.Utilities;
using DecisionsFramework.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBaseStepConvertersForLocalModule
{
    public class StartIStepConverter : IStepConverter
    {

        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            FlowStep result = new FlowStep(new StartStep());
            result.Name = stepToConvert.StepName;
            return result;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Process.StartComponent";
        }

        public string ConvertPath(string originalPath)
        {
            return "done";
        }


        #endregion
    }

    public class LogIStepConverter : IStepConverter
    {

        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            LogStep logStep = new LogStep();

            logStep.Category = stepToConvert.StepProperties.FirstOrDefault(x => x.Name == "Category").ConstantValue;
            
            string logLevel = stepToConvert.StepProperties.FirstOrDefault(x => x.Name == "Type").ConstantValue;
            switch (logLevel) { 
                case "Debug" :
                    logStep.Type = LogStep.LogType.Debug;
                    break;
                case "Error" :
                    logStep.Type = LogStep.LogType.Error;
                        break;
                case "Info" :
                    logStep.Type = LogStep.LogType.Info;
                        break;
                case "Fatal" :
                    logStep.Type = LogStep.LogType.Fatal;
                        break;
                case "Warn" :
                        logStep.Type = LogStep.LogType.Warn;
                        break;
                default :
                        logStep.Type = LogStep.LogType.Info;
                        break;
            }
            
            FlowStep result = new FlowStep(logStep);
            result.Name = stepToConvert.StepName;

            MergeStringInputMapping text = new MergeStringInputMapping() {
            //ConstantInputMapping text = new ConstantInputMapping();
            InputDataName = "Value",
            MergeResultType = DecisionsFramework.Utilities.Data.MergeDataType.PlainText,
            MergeString = stepToConvert.InputData.FirstOrDefault(x => x.Name == "Value").ConstantValue
            };

            result.AddInputMapping(text);

            return result;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Logging.CreateLogEntryComponent";
        }

        public string ConvertPath(string originalPath)
        {
            return "Done";
        }


        #endregion
    }

    public class SendEmailIStepConverter : IStepConverter
    {

        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            SendEmailStep emailStep = new SendEmailStep();

            FlowStep result = new FlowStep(emailStep);

            result.Name = stepToConvert.StepName;

            //Add From Input
            result.AddInputMapping(ConvertUtility.ConvertStepInputToInputMapping(stepToConvert.InputData.FirstOrDefault(x => x.Name == "From"), "From"));
            
            //Add To Input
            result.AddInputMapping(ConvertUtility.ConvertStepInputToInputMapping(stepToConvert.InputData.FirstOrDefault(x => x.Name == "To"), "To"));

            //Add Subject Input
            result.AddInputMapping(ConvertUtility.ConvertStepInputToInputMapping(stepToConvert.InputData.FirstOrDefault(x => x.Name == "Subject"), "Subject"));

            //Add Body Input
            result.AddInputMapping(ConvertUtility.ConvertStepInputToInputMapping(stepToConvert.InputData.FirstOrDefault(x => x.Name == "Body"), "Body", true));

            //MergeStringInputMapping subject = new MergeStringInputMapping()
            //{
            //    InputDataName = "Subject",
            //    MergeResultType = DecisionsFramework.Utilities.Data.MergeDataType.PlainText,
            //    MergeString = stepToConvert.InputData.FirstOrDefault(x => x.Name == "Subject").ConstantValue
            //};

            ////get To addresses together
            //List<IInputMapping> toIM = new List<IInputMapping>();
            //int counter = -1;
            //foreach (StepInput toAddress in stepToConvert.InputData.FirstOrDefault(x => x.Name == "To").ArrayParts)
            //{
            //    counter = counter + 1;
            //    string inDataName = string.Format("Item {0}", counter);

            //    if (toAddress.MappingType == InputMappingType.SelectValue)
            //    {
            //        toIM.Add(new SelectValueInputMapping() { InputDataName = inDataName, DataPath = toAddress.SelectValuePathName });
            //    }

            //    else
            //    {
            //        toIM.Add(new ConstantInputMapping() { InputDataName = inDataName, Value = toAddress.ConstantValue });
            //    }
            //}

            ////get from address together
            //StepInput fromSI = stepToConvert.InputData.FirstOrDefault(x => x.Name == "From");
            //string inDataName = "From"; //why is inDataName saying it is already declared since I delcared it in a foreach above?

            //   //IInputMapping fromAddress = new IInputMapping();
            //if (fromSI.MappingType == InputMappingType.SelectValue)
            //{
            //    //do select value stuff
            //    SelectValueInputMapping fromAddress = new SelectValueInputMapping() { InputDataName = inDataName, DataPath = fromSI.SelectValuePathName };

            //}
            //if (fromSI.MappingType == InputMappingType.MergeText)
            //{
            //    //do merge text stuff
            //    MergeStringInputMapping fromAddress = new MergeStringInputMapping() { InputDataName = inDataName, MergeResultType = MergeDataType.PlainText, MergeString = fromSI.ConstantValue };
            //}
            //else
            //{
            //    //do constant stuff
            //    ConstantInputMapping fromAddress = new ConstantInputMapping() { InputDataName = inDataName, Value = fromSI.ConstantValue };
            //}

            //result.AddInputMapping(subject);
            //result.AddInputMapping(new ArrayInputMapping() { InputDataName = "To", SubMappings = toIM.ToArray() });
            //result.AddInputMapping(fromAddress);
            
            return result;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Communication.SendEmailComponent";
        }

        public string ConvertPath(string originalPath)
        {
            return "sent";
        }


        #endregion
    }

    public class EndIStepConverter : IStepConverter
    {

        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            FlowStep result = new FlowStep(new EndStep());
            result.Name = stepToConvert.StepName;
            return result;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Process.EndComponent";
        }

        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }


        #endregion
    }

    public class AssignDataStepConverter : IStepConverter
    {

        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            FlowStep result = new FlowStep(new CreateDataStep());
            result.Name = stepToConvert.StepName;
            return result;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Process.AssignDataStep";
        }

        public string ConvertPath(string originalPath)
        {
            return "done";
        }


        #endregion
    }

    public class MatchesIStepConverter : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            StringMatchStep sms = new StringMatchStep();
            sms.PossibleMatches = stepToConvert.StepProperties.FirstOrDefault(x => x.Name == "PossibleMatches").ConstantValue.Split(',');

            FlowStep result = new FlowStep(sms);
            result.Name = stepToConvert.StepName;
            return result;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Rules.MatchesRule";
        }

        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }

        #endregion

   
     
    }

    public class CompareNumbersConvertor : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            InvokeMethodStep ims = new InvokeMethodStep("DecisionsFramework.Design.Flow.CoreSteps.StandardSteps.NumberSteps", "GreaterThan", null);

            FlowStep fs = new FlowStep(ims);
            fs.Name = stepToConvert.StepName;

            return fs;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Rules.CompareNumbersRule";
        }

        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }

        #endregion
    }

    public class InsertDataStepConverter : IStepConverter
    {

        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            CreateDataStep ps = new CreateDataStep();
            FlowStep fs = new FlowStep(ps);

            List<PlaceholderData> allDataDefinitions = new List<PlaceholderData>();
            foreach (OutcomeDefinition outcome in stepToConvert.OutcomeData) {
                foreach (DataDefinition outputData in outcome.OutcomeData) {

                    Type t = TypeUtilities.FindTypeByFullName(outputData.FullTypeName);

                    //Type t might be null if we aren't able to resolve the type of a custom type
                    //so let's just set it to string.
                    if (t == null)
                    {
                        t = typeof(string);
                    }

                    allDataDefinitions.Add(
                            new PlaceholderData(new DecisionsNativeType(t), outputData.Name, outputData.IsList)
                        );
                }
            }

            ps.DataDefinitions = allDataDefinitions.ToArray();

            // Now do the mapping.
            List<IOutputMapping> outputMapping = new List<IOutputMapping>();
            foreach (OutcomeDefinition outcome in stepToConvert.OutcomeData)
            {
                foreach (DataDefinition outputData in outcome.OutcomeData)
                {
                    RenameOutputMapping mapping = new RenameOutputMapping();
                    mapping.DataName = outputData.Name;
                    mapping.OutputDataName = outputData.Name;
                    outputMapping.Add(mapping);
                }
            }
            fs.OutputMapping = outputMapping.ToArray();
            //string varName = (ConvertUtility.ConvertStepInputToInputMapping(stepToConvert.InputData.FirstOrDefault())).InputDataName;
            string varName = (stepToConvert.InputData.FirstOrDefault()).Name;

            Type type = Type.GetType(stepToConvert.InputData.FirstOrDefault().FullTypeName);
            
            object value = new bool();

            //if (type == typeof(bool))
            //{
            //    value = Convert.ToBoolean(stepToConvert.InputData.FirstOrDefault().ConstantValue);
            //}

            bool result;
            if (bool.TryParse(stepToConvert.InputData.FirstOrDefault().ConstantValue, out result))
            {
                value = result;
            }


            //if (type == typeof(int))
            //{
            //    value = Convert.ToInt32(stepToConvert.InputData.FirstOrDefault().ConstantValue);
            //}

            //else
            //{
            //    value = stepToConvert.InputData.FirstOrDefault().ConstantValue;
            //}

            //fs.AddInputMapping(new ConstantInputMapping() { InputDataName = varName, Value = stepToConvert.InputData.FirstOrDefault().ConstantValue });
            fs.AddInputMapping(new ConstantInputMapping() { InputDataName = varName, Value = result });

            return fs;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Process.InsertDataComponent";
        }

        #endregion
       
        public string ConvertPath(string originalPath)
        {
            return "Done";
        }

        //public override FlowStep DoMappings(FlowStep result, IOrchestrationComponent component)
        //{
        //    InsertDataComponent comp = (InsertDataComponent)component;

        //    result.ShowAsRule = false;

        //    // When you add an outcome scenario and call 'OutputMappings'
        //    // the rename mapping gets created automatically.  so now we 
        //    // need to edit it
        //    RenameOutputMapping mapping = (RenameOutputMapping)result.OutputMapping[0];
        //    mapping.DataName = comp.GetAddedData()[0].Name;

        //    return result;
        //}
    }

    //public class TextEqualsStepConverter : IStepConverter
    //{

    //    #region IStepConverter Members

    //    public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
    //    {
    //        FlowStep result;
    //        InvokeMethodStep ms = new InvokeMethodStep("DecisionsFramework.Design.Flow.CoreSteps.StandardSteps.StringSteps", "EqualsString", null);
    //        result = new FlowStep(ms);
    //        result.Name = stepToConvert.StepName;

    //        List<IInputMapping> inputs = new List<IInputMapping>();
    //        SelectValueInputMapping svim = new SelectValueInputMapping();
    //        svim.InputDataName = "value1";
    //        svim.DataPath = stepToConvert.InputData.First(x => x.Name = "value1");
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

    //    public string GetTypeICanConvertFrom()
    //    {
    //        return "LogicBase.Components.Default.Process.AssignDataStep";
    //    }

    //    #endregion
      
    //    public string ConvertPath(string originalPath)
    //    {
    //        return originalPath;
    //    }

    //}

    //public class TextExistsStepConverter : IStepConverter
    //{
    //    #region IStepConverter Members

    //    public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
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

    //    public string GetTypeICanConvertFrom()
    //    {
    //        return "LogicBase.Components.Default.Process.AssignDataStep";
    //    }

    //    #endregion
      
    //    public string ConvertPath(string originalPath)
    //    {
    //        // Flip the paths since we're checking for == null;
    //        //if (originalPath == TextExists.EXISTS)
    //        //{
    //        //    return "false";
    //        //}
    //        return "true";
    //    }
      
    //}

    public class EqualsRuleConvertor : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            FlowStep result;
            InvokeMethodStep ms = new InvokeMethodStep("DecisionsFramework.Design.Flow.CoreSteps.StandardSteps.ObjectSteps", "ObjectsAreEqual", null);
            result = new FlowStep(ms);
            result.Name = stepToConvert.StepName;

            SelectValueInputMapping svim = new SelectValueInputMapping();
            svim.DataPath = stepToConvert.InputData.First(x => x.Name == "value1").SelectValuePathName;
            svim.InputDataName = "value1";
            result.AddInputMapping(svim);

            SelectValueInputMapping svim2 = new SelectValueInputMapping();
            svim2.DataPath = stepToConvert.InputData.First(x => x.Name == "value2").SelectValuePathName;
            svim2.InputDataName = "value2";
            result.AddInputMapping(svim2);


            return result;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Rules.EqualsRule";
        }

        #endregion

        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }
       
        //public override FlowStep DoMappings(FlowStep result, IOrchestrationComponent component)
        //{
        //    List<IInputMapping> inputs = new List<IInputMapping>();
        //    SelectValueInputMapping svim = new SelectValueInputMapping();
        //    svim.InputDataName = "value1";
        //    svim.DataPath = ((EqualsRule)component).VariableName;
        //    inputs.Add(svim);

        //    VariableOrValueDataType var = ((EqualsRule)component).CompareTo;
        //    IInputMapping inputMapping = VVDTUtility.ConvertVariableOrValueDataTypeToInputMapping(var, "value2");
        //    if (inputMapping != null)
        //        inputs.Add(inputMapping);

        //    // Now stick these inputs into the flow step to be added to the flow.);
        //    foreach (IInputMapping eachMapping in inputs)
        //    {
        //        result.AddInputMapping(eachMapping);
        //    }
        //    return result;
        //}
    }


    public class FormBuilderStepConverter : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            // This step is really special.  We need a Form
            // step on the flow, but we also need to create the form and add
            // it to the currently being converted project. 

            FormWrapperStep fws = new FormWrapperStep();
            
            // Get element registration id from form id
            // in already converted data.
            string erId = allConvertData.OldFormIdToNewFormIdMap[stepToConvert.UniqueStepIdForConnections];
            
            // We should WARN if no id found, but whatever.
            fws.RegistrationId = erId;
            
            FlowStep fs = new FlowStep(fws);
            
            return fs;
        }

        public string GetTypeICanConvertFrom()
        {
            return "FormWrapperStep";
        }

        #endregion

        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }
    }
    


    public class GoToComponentConvertor : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            GoToStep gs = new GoToStep();
            FlowStep result = new FlowStep(gs);
            result.Name = stepToConvert.StepName;
            //gs.GoToStepName = ((GoToComponentByName)component).ComponentName;

            return result;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.FlowControl.GoToComponentByName";
        }

        #endregion

        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }
    }



    public class GetCurrentDateConvertor : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            InvokeMethodStep ims = new InvokeMethodStep("DecisionsFramework.Design.Flow.CoreSteps.StandardSteps.DateSteps", "GetCurrentDate", null);


            FlowStep fs = new FlowStep(ims);
            fs.Name = stepToConvert.StepName;
            return fs;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.DateHandling.GetCurrentDate";
        }

        #endregion


        public string ConvertPath(string originalPath)
        {
            return "Done";
        }

        //public override FlowStep DoMappings(FlowStep result, IOrchestrationComponent component)
        //{
        //    RenameOutputMapping ren = new RenameOutputMapping();
        //    ren.DataName = ((GetCurrentDate)component).OutputVariableName;

        //    // @TODO HSO We need to map the output mapping
        //    //result.AddOutputMapping(ren);
        //    return result;
        //}
    }

    public class ItemByIndexConvertor : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            DecisionsFramework.Design.Flow.CoreSteps.GetItemByIndex getItemStep = new DecisionsFramework.Design.Flow.CoreSteps.GetItemByIndex();
            FlowStep fs = new FlowStep(getItemStep);
            fs.Name = stepToConvert.StepName;
            return fs;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Process.GetItemByIndex";
        }

        #endregion
       

        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }

     
    }

    public class IndexOfItemInListConvertor : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            IndexOfItemInList itemInList = new IndexOfItemInList();

            return new FlowStep(itemInList);
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Process.IndexOfItemInCollection";
        }

        #endregion
     

        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }

     
    }

    public class ParseDateConvertor : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            InvokeMethodStep ims = new InvokeMethodStep("DecisionsFramework.Design.Flow.CoreSteps.StandardSteps.DateSteps", "ParseDate", null);

            FlowStep fs = new FlowStep(ims);
            fs.Name = stepToConvert.StepName;

            return fs;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.DateHandling.ConvertStringToDate";
        }

        #endregion
       

        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }

        //public override FlowStep DoMappings(FlowStep result, IOrchestrationComponent component)
        //{
        //    SelectValueInputMapping i = new SelectValueInputMapping();
        //    i.DataPath = ((ConvertStringToDate)component).InputVariableName;
        //    i.InputDataName = "date";
        //    result.AddInputMapping(i);
        //    return result;
        //}
    }

    public class ParseNumberConvertor : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            InvokeMethodStep ims = new InvokeMethodStep("DecisionsFramework.Design.Flow.CoreSteps.StandardSteps.NumberSteps", "ParseNumber", null);

            FlowStep fs = new FlowStep(ims);
            fs.Name = stepToConvert.StepName;
            return fs;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Process.GetNumberFromString";
        }

        #endregion
       
        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }

    }

    public class LinkedModelConvertor : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            SystemUserContext suc = new SystemUserContext();
            using (UserContextHolder.Register(suc))
            {
                LinkedFlowStep lfs = new LinkedFlowStep();

                // Get name of flow to run / id of flow to run.
                StepInput flowName = stepToConvert.StepProperties.FirstOrDefault(x => x.Name == "LinkedFlowName");

                string flowId = flowName.ConstantValue;
                foreach (Flow cf in allConvertData.ConvertedFlows)
                {
                    if (cf.Name == flowName.ConstantValue)
                    {
                        flowId = cf.Id;
                    }
                }


                lfs.RegistrationId = flowId;

                return new FlowStep(lfs);
            }
            
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Process.LinkedModelComponent";
        }

        #endregion
       
        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }
    }

    public class EmbeddedModelConverter : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            SystemUserContext suc = new SystemUserContext();
            using (UserContextHolder.Register(suc))
            {
                LinkedFlowStep lfs = new LinkedFlowStep();

                // Get name of flow to run / id of flow to run.
                StepInput flowId = stepToConvert.StepProperties.FirstOrDefault(x => x.Name == "EmbeddedFlowId");

                //string flowId = flowName.ConstantValue;
                //foreach (Flow cf in allConvertData.ConvertedFlows)
                //{
                //    if (cf.Name == flowName.ConstantValue)
                //    {
                //        flowId = cf.Id;
                //    }
                //}


                lfs.RegistrationId = flowId.ConstantValue;

                return new FlowStep(lfs);
            }

        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Process.EmbeddedModelComponent";
        }

        #endregion

        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }
    }

    public class ShortDateConvertor : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            InvokeMethodStep ims = new InvokeMethodStep("DecisionsFramework.Design.Flow.CoreSteps.StandardSteps.DateSteps", "GetShortDateString", null);

            FlowStep fs = new FlowStep(ims);
            fs.Name = stepToConvert.StepName;
            return fs;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.DateHandling.ToShortDateString";
        }

        #endregion
       
        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }

        //public override FlowStep DoMappings(FlowStep result, IOrchestrationComponent component)
        //{
        //    IInputMapping newMap = VVDTUtility.ConvertVariableOrValueDataTypeToInputMapping(((ToShortDateString)component).date, "date");
        //    result.AddInputMapping(newMap);
        //    return result;

        //}
    }

    public class VariableExistsRuleConvertor : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            InvokeMethodStep ims = new InvokeMethodStep("DecisionsFramework.Design.Flow.CoreSteps.StandardSteps.ObjectSteps", "ObjectIsNull", null);

            FlowStep fs = new FlowStep(ims);
            fs.Name = stepToConvert.StepName;
            return fs;
        }

        public string GetTypeICanConvertFrom()
        {

            return "LogicBase.Components.Default.FlowControl.VariableExistsRule";
        }

        #endregion
       
        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }

        //public override FlowStep DoMappings(FlowStep result, IOrchestrationComponent component)
        //{
        //    SelectValueInputMapping inputA = new SelectValueInputMapping();
        //    inputA.DataPath = ((VariableExistsRule)component).VariableName;
        //    inputA.InputDataName = "value";
        //    result.AddInputMapping(inputA);
        //    return result;
        //}
    }

    public class CreateTextFromArrayConvertor : IStepConverter
    {
        #region IStepConverter Members

        public DecisionsFramework.Design.Flow.FlowStep ConvertStep(ConversionData allConvertData, ConvertedStep stepToConvert)
        {
            // JoinStrings(string[] source, string separator
            InvokeMethodStep ims = new InvokeMethodStep("DecisionsFramework.Design.Flow.CoreSteps.StandardSteps.StringSteps", "JoinStrings", null);

            FlowStep fs = new FlowStep(ims);
            fs.Name = stepToConvert.StepName;
            return fs;
        }

        public string GetTypeICanConvertFrom()
        {
            return "LogicBase.Components.Default.Process.BuildTextFromElements";
        }

        
        #endregion




        #region IStepConverter Members

        public string ConvertPath(string originalPath)
        {
            return originalPath;
        }

        #endregion
    }
}
