namespace SovereigntyBot.Services.Endpoints.Profile
{
    public class GroupProfileResult
    {
        public string Name { get; set; }
        public bool Success { get; set; }
        public int TotalConditionsPass { get; set; }
        public Dictionary<string, GroupProfileResult> groupResults { get; set; } = new();
        public Dictionary<string, ConditionProfileResult> conditionResults { get; set; } = new();

        public void traverse(int currentDepth, int targetDepth) // NEED MORE DEVELOPMENT
        {
            if(currentDepth == targetDepth)
            {
                var conditions = collectConditions();
                conditions.AddRange(conditionResults.Values.ToList());
                return;
            }
            foreach(GroupProfileResult gpr in groupResults.Values)
            {
                gpr.traverse(currentDepth + 1, targetDepth);
            }
        }

        public List<ConditionProfileResult> collectConditions() // NEED MORE DEVELOPMENT
        {
            if(groupResults.Count == 0)
            {
                return conditionResults.Values.ToList();
            }
            foreach(GroupProfileResult gpr in groupResults.Values)
            {
                var conditions = gpr.collectConditions();
                conditions.AddRange(conditionResults.Values.ToList());
                return conditions;
            }
            return null;
        }
    }
}