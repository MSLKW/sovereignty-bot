namespace SovereigntyBot.Services.Endpoints.Profile
{
    public class MainProfileResult
    {
        public string Name { get; set; }
        public bool Success { get; set; }
        public string Uuid { get; set; }
        public Dictionary<string, GroupProfileResult> groupResults { get; set; } = new();
        public Dictionary<string, ConditionProfileResult> conditionResults { get; set; } = new();

        private void traverse(int targetDepth) // still need more development
        {
            int currentDepth = 1;
            foreach(GroupProfileResult gpr in groupResults.Values)
            {
                gpr.traverse(currentDepth, targetDepth);
            }
        }
    }
}