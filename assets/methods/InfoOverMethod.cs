namespace Methods;

using System;
using System.Reflection;
using System.Text.Json;

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
                paramDict.Add($"parameter{i + 1}", parameters[i].Name ?? "unknown");
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
        MethodInfo? method = type.GetMethod(methodName);

        // Wenn die Methode gefunden wurde, dann aufrufen
        if (method != null)
        {
            ParameterInfo[] parameters = method.GetParameters();
            Console.WriteLine($"🔍 Aufruf der Methode: {methodName} mit den Parameter/n: {parameter}");
            Console.WriteLine();

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
                Console.WriteLine("⏳ Methode wird mit Parameter ausgeführt.");
                method.Invoke(methodsInstance, new object[] { parameter });
                Console.WriteLine("✅ Methode wurde mit Parameter ausgeführt.");
            }
            else if (parameters.Length == 2 && parameter != null)
            {
                Console.WriteLine("⏳ Methode wird mit zwei Parametern ausgeführt.");

                var paramArray = System.Text.Json.JsonSerializer.Deserialize<object[]>(parameter);

                if (paramArray != null)
                {
                    var convertedParams = paramArray.Select(p =>
                {
                    if (p is JsonElement element)
                    {
                        if (element.ValueKind == JsonValueKind.Null)
                            return null;

                        var str = element.ToString();
                        return str == "null" ? null : str;
                    }

                    // Falls kein JsonElement (z. B. direkt string)
                    return p?.ToString() == "null" ? null : p;
                }).ToArray();

                    method.Invoke(methodsInstance, new object?[] { convertedParams[0], convertedParams[1] });

                    Console.WriteLine("✅ Methode wurde mit zwei Parametern ausgeführt.");
                }
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