namespace GitAuthForManagedIdentity;

public class Program
{
    const char delimiter = '=';

    public static async Task<int> Main(params string[] args)
    {
        var git_config = Environment.GetEnvironmentVariable("GIT_CONFIG_PARAMETERS");
#if DEBUG
        System.Diagnostics.Debugger.Launch();
#endif
        if (args.Length == 0)
        {
            Console.WriteLine("Missing argument {get}");
            Console.WriteLine("https://git-scm.com/docs/gitcredentials");
            return 2;
        }

        var data = new List<(string key, string value)>();
        Dictionary<string, string?> result = [];

        bool hasCapabilityAuthType = false;
        //bool hasCapabilityState = false;
        //string? state = null;
        do
        {
            var line = Console.ReadLine();
            if (line == null)
            {
                break;
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine(line);
#endif

            var splitted = line.Split(delimiter, 2);
            if (splitted.Length != 2)
            {
                continue;
            }
            data.Add((splitted[0], splitted[1]));

            if (splitted[0].Equals("capability[]"))
            {
                if (splitted[1].Equals("authtype"))
                {
                    hasCapabilityAuthType = true;
                }
                //if (splitted[1].Equals("state"))
                //{
                //    hasCapabilityState = true;
                //}
            }
            //if (splitted[0].StartsWith("state[]") && splitted[1].StartsWith("abc"))
            //{
            //    state = splitted[1];
            //}

            if (result.ContainsKey(splitted[0]))
            {
                result[splitted[0]] = splitted[1];
            }
        } while (true);

        switch (args[0])
        {
            case "get":
                //Console.WriteLine("capability[]=authtype");
                //Console.WriteLine("capability[]=state");
                TokenResponse? token = await TokenResponse.GetTokenAsync();

                if (token is not null)
                {
                    if (hasCapabilityAuthType)
                    {
                        result["credential"] = token.access_token;
                        result["authtype"] = token.token_type;
                    }
                    else
                    {
                        result["username"] = token.client_id;
                        result["password"] = token.access_token;
                    }
                    result["password_expiry_utc"] = token.expires_on;

                    //if (hasCapabilityState)
                    //{
                    //    result["state[]"] = "abc=zzz";
                    //}
                }
                break;
        }

        foreach (var item in result)
        {
            Console.Write(item.Key);
            Console.Write(delimiter);
            Console.WriteLine(item.Value);
        }

        Console.WriteLine();
        return 0;
    }
}
