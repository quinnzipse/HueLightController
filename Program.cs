using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HueLightController
{
    class Program
    {
        //Config Settings
        static string username = MyCreds.username;
        static string ipAdress = MyCreds.ipAdress;

        private static void MakeRequest(String category, String subControl, object controlThing, Method method)
        {
            String action = "";
            if (category == "groups" || category == "scenes") action = "action";
            else if (category == "lights") action = "state";
            else Console.WriteLine("Error: Line 24");

            var client = new RestClient();
            client.BaseUrl = new Uri("http://" + ipAdress + "/api/" + username);

            String path = "/" + category + "/" + subControl + "/" + action;

            IRestRequest request = new RestRequest(path)
            {
                Method = method
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(controlThing), ParameterType.RequestBody);
            client.Execute(request);
        }

        //TODO: This needs to be implemented
        private static String[] overrideDefaults(String dWhat, String dSub, String[] args)
        {
            String[] targetCommand = { dWhat, dSub };
            String[] command = { "groups", "scenes", "lights" };
            String[] subControls = { "1", "2", "3", "4" };
            String[][] mainCommand = {command, subControls};
            for (int i = 0; i < mainCommand.Length; i++)
            {
                for (int x = 0; x < mainCommand[i].Length; x++)
                {
                    for (int a = 0; a < args.Length; a++)
                    {
                        if (args[a] == mainCommand[i][x])
                        {
                            targetCommand[i] = args[a];
                            if (i == mainCommand.Length - 1) return targetCommand;
                            else break;
                        }
                    }
                }
            }
            return targetCommand;
        }

        static void Main(string[] args)
        {
            //Default Values
            String dWhatDoIControl = "groups";
            String dSubControl = "1";

            PutState lightState = new PutState();

            bool continueRunning = true;

            do
            {
                Console.Write("HueCommand: ");
                String input = Console.ReadLine().ToLower();
                String[] arguments = input.Split(' ');

                //TODO: This needs to be implimented
                int something = 0;
                String[] targetCommand = overrideDefaults(dWhatDoIControl, dSubControl, arguments);
                for (int i = 0; i < targetCommand.Length; i++)
                {
                    for (int x = 0; x < arguments.Length; x++)
                    {
                        if (arguments[x] == targetCommand[i]) something = x + 1;
                    }
                }
                String whatToControl = targetCommand[0];
                String subControl = targetCommand[1];

                object thing = new Def();
                IRestRequest request = new RestRequest();

                switch(arguments[something]){
                    case "off":
                        lightState.on = false;
                        thing = lightState;
                        break;
                    case "on":
                        lightState.on = true;
                        thing = lightState;
                        break;
                    case "red":
                        
                    case "green":
                    case "purple":
                    case "white":
                    case "blue":
                        
                        break;
                    default:
                        Console.WriteLine("Incorrect Syntax, Please Try Again.");
                        break;
                }
                //Console.WriteLine("What to control: " + whatToControl + "\n" +
                //    "Subcontrol: " + subControl + "\n" + "Thing: " + thing.ToString() + "\n");
                MakeRequest(whatToControl, subControl, thing, Method.PUT);
            }
            while (continueRunning);

            //var response = client.Execute(request);

            //RootObject obj = JsonConvert.DeserializeObject<RootObject>(response.Content);

            //Console.WriteLine(response.Content + "\n\n");
            //try
            //{
            //    Console.WriteLine( obj.groups.id1.name.ToString() + "\n");
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e.StackTrace.ToString()); 
            //}
        }
    }
}

public class Def { }

public class PutState
{
    public bool on { get; set; }
}

public class State
{
    public bool all_on { get; set; }
    public bool any_on { get; set; }
}

public class Action
{
    public bool on { get; set; }
    public int bri { get; set; }
    public int hue { get; set; }
    public int sat { get; set; }
    public string effect { get; set; }
    [JsonProperty("xy")]
    public List<double> xy { get; set; }
    public int ct { get; set; }
    public string alert { get; set; }
    public string colormode { get; set; }
}

public class Id1
{
    public string name { get; set; }
    [JsonProperty("lights")]
    public List<string> lights { get; set; }
    public string type { get; set; }
    [JsonProperty("state")]
    public State state { get; set; }
    public bool recycle { get; set; }
    [JsonProperty("class")]
    public string @class { get; set; }
    [JsonProperty("action")]
    public Action action { get; set; }
}

public class Action2
{
    public bool on { get; set; }
    public int bri { get; set; }
    public int hue { get; set; }
    public int sat { get; set; }
    public string effect { get; set; }
    [JsonProperty("xy")]
    public List<double> xy { get; set; }
    public int ct { get; set; }
    public string alert { get; set; }
    public string colormode { get; set; }
}

public class Id2
{
    public string name { get; set; }
    [JsonProperty("lights")]
    public List<string> lights { get; set; }
    public string type { get; set; }
    [JsonProperty("state")]
    public State state { get; set; }
    public bool recycle { get; set; }
    [JsonProperty("action")]
    public Action2 action { get; set; }
}

public class Stream
{
    public string proxymode { get; set; }
    public string proxynode { get; set; }
    public bool active { get; set; }
    public object owner { get; set; }
}

public class Locations
{
    [JsonProperty("4")]
    public List<double> loc { get; set; }
}

public class Action3
{
    public bool on { get; set; }
    public int bri { get; set; }
    public int hue { get; set; }
    public int sat { get; set; }
    public string effect { get; set; }
    [JsonProperty("xy")]
    public List<double> xy { get; set; }
    public int ct { get; set; }
    public string alert { get; set; }
    public string colormode { get; set; }
}

public class Id3
{
    public string name { get; set; }
    [JsonProperty("lights")]
    public List<string> lights { get; set; }
    public string type { get; set; }
    [JsonProperty("state")]
    public State state { get; set; }
    public bool recycle { get; set; }
    public string @class { get; set; }
    [JsonProperty("stream")]
    public Stream stream { get; set; }
    [JsonProperty("locations")]
    public Locations locations { get; set; }
    [JsonProperty("action")]
    public Action3 action { get; set; }
}

public class Action4
{
    public bool on { get; set; }
    public string alert { get; set; }
}

public class Id4
{
    public string name { get; set; }
    [JsonProperty("lights")]
    public List<object> lights { get; set; }
    public string type { get; set; }
    [JsonProperty("state")]
    public State state { get; set; }
    public bool recycle { get; set; }
    public string @class { get; set; }
    [JsonProperty("action")]
    public Action4 action { get; set; }
}

public class Groups
{
    [JsonProperty("1")]
    public Id1 id1 { get; set; }
    [JsonProperty("2")]
    public Id2 id2 { get; set; }
    [JsonProperty("3")]
    public Id3 id3 { get; set; }
    [JsonProperty("4")]
    public Id4 id4 { get; set; }
}

public class RootObject
{
    [JsonProperty("groups")]
    public Groups groups { get; set; }
}


