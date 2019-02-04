using System;
using LogicBase.Core;
using LogicBase.Core.Data.DataTypes;
using LogicBaseProjectConversionUtility.ProjectConversionService;

namespace LogicBaseToDecisionsProjectConversionTool
{
    public abstract class LogicBaseStepConverter
    {

        public abstract StepInput[] GetProperties(IOrchestrationComponent component);

        public abstract StepInput[] GetInputs(IOrchestrationComponent component);

        public abstract StepOutcome[] GetOutcomes(IOrchestrationComponent component);

        //public abstract Tuple<StepInput[], StepOutcome[]> GetMappings(IOrchestrationComponent component);

        //public StepOutcome ConvertDataMappingToOutputMappping(DataMapping workflowMapping)
        //{
        //    StepOutcome result = new StepOutcome();
        //    result.MappingType = OutputMappingType.Rename;
        //    result.TypeOfOutputData = workflowMapping.OutputType;
        //    result.RenameName = workflowMapping.OutputVariableName;
        //    result.IsArray = false;
            
        //    return result;
        //}
    }
}
