namespace Methods;

class InfoOverMethod
{
    Methods methods = new Methods();
    public string[] getListOfAllMethods()
    {
        var methodInfos = methods.GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        string[] methodNames = methodInfos.Select(m => m.Name).ToArray();

        return methodNames;
    }

    public string[] getMethodRequirement()
    {
        var methodInfos = methods.GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

        var methodRequirements = methodInfos.Select(m =>
        {
            var parameters = m.GetParameters();
            var paramDict = new Dictionary<string, string>
            {
                { "action", m.Name }
            };
            for (int i = 0; i < parameters.Length; i++)
            {
                paramDict.Add($"parameter{i + 1}", parameters[i].Name);
            }
            // Convert to string representation
            var paramList = paramDict.Select(kvp => $"\"{kvp.Key}\" => \"{kvp.Value}\"");
            return "[" + string.Join(", ", paramList) + "]";
        }).ToArray();

        return methodRequirements;
    }
}