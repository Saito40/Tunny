using Tunny.Core.Enum;

namespace Tunny.Core.Settings
{
    public class Plot
    {
        public PlotActionType PlotActionType { get; set; }
        public string PlotTypeName { get; set; }
        public string TargetStudyName { get; set; }
        public string[] TargetObjectiveName { get; set; }
        public int[] TargetObjectiveIndex { get; set; }
        public string[] TargetVariableName { get; set; }
        public int[] TargetVariableIndex { get; set; }
        public int ClusterCount { get; set; }
        public bool IncludeDominatedTrials { get; set; }
    }
}
