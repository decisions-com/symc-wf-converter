using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.Mapping.InputImpl;
using DecisionsFramework.ProjectConversion;
using DecisionsFramework.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBaseStepConvertersForLocalModule
{
    public class ConvertUtility
    {
        public static IInputMapping ConvertStepInputToInputMapping(StepInput stepInput, string inputDataName, bool ifMergeUseHTML = false)
        {
            //Convert Select Value
            if (stepInput.MappingType == InputMappingType.SelectValue)
            {                
                return new SelectValueInputMapping() { InputDataName = inputDataName, DataPath = stepInput.SelectValuePathName };

            }

            //Convert Merge Text
            if (stepInput.MappingType == InputMappingType.MergeText)
            {
                if (ifMergeUseHTML)
                {
                    return new MergeStringInputMapping() { InputDataName = inputDataName, MergeResultType = MergeDataType.Html, MergeString = stepInput.ConstantValue };
                }
                else
                {
                    return new MergeStringInputMapping() { InputDataName = inputDataName, MergeResultType = MergeDataType.PlainText, MergeString = stepInput.ConstantValue };
                }
            }

            //Convert Array Builder
            if (stepInput.MappingType == InputMappingType.ArrayBuilder)
            {
                List<IInputMapping> toIM = new List<IInputMapping>();
                int counter = -1;
                foreach (StepInput si in stepInput.ArrayParts)
                {
                    counter = counter + 1;
                    string inDataName = string.Format("Item {0}", counter);

                    toIM.Add(ConvertStepInputToInputMapping(si, inDataName, false));
                }

                return new ArrayInputMapping() { InputDataName = inputDataName, SubMappings = toIM.ToArray() };
            }

            //Convert Constant - We tried everything else so this must be constant
            else
            {
                return new ConstantInputMapping() { InputDataName = inputDataName, Value = stepInput.ConstantValue };
            }
        }
    }
}
