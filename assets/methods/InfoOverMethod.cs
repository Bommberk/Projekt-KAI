namespace Methods;

using System;
using System.Reflection;

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

    public void callMethod(string methodName, string? parameter)
    {
        // Instanz der Methods-Klasse erstellen
        Methods methodsInstance = new Methods();

        // Den Typ der Methods-Klasse ermitteln
        Type type = methodsInstance.GetType();

        // Methode über den Namen suchen
        MethodInfo method = type.GetMethod(methodName);

        // Wenn die Methode gefunden wurde, dann aufrufen
        if (method != null)
        {
            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length == 0 || parameter == "null")
            {
                // Methode hat keine Parameter
                Console.WriteLine("⏳ Methode wird ohne Parameter ausgeführt.");
                method.Invoke(methodsInstance, null);
                Console.WriteLine("✅ Methode wurde ohne Parameter ausgeführt.");
            }
            else if (parameters.Length == 1 && parameter != null)
            {
                // Methode hat einen Parameter und du hast einen übergeben
                method.Invoke(methodsInstance, new object[] { parameter });
                Console.WriteLine("Methode wurde mit Parameter ausgeführt.");
            }
            else
            {
                Console.WriteLine("Falsche Parameteranzahl oder ungültiger Parameter.");
            }
        }
        else
        {
            Console.WriteLine("Methode nicht gefunden.");
        }

    }
}