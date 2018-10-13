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
        static double HueMax = 255, userInputMax = 100; //this creates a 0-100 for user input and a 0-255 output for the Hue API
        static AutoDim autoDim = new AutoDim();

        private static IRestResponse MakeRequest(String category, String subControl, object controlThing, Method method)
        {
            String action = "action";
            if (controlThing == null) action = "";
            else if (category == "lights") action = "state";

            var client = new RestClient();
            client.BaseUrl = new Uri("http://" + ipAdress + "/api/" + username);

            String path;
            if (subControl != "") path = "/" + category + "/" + subControl + "/" + action;
            else path = "/" + category + "/" + action;

            IRestRequest request = new RestRequest(path)
            {
                Method = method
            };

            if(controlThing != null) request.AddParameter("application/json", JsonConvert.SerializeObject(controlThing), ParameterType.RequestBody);

            return client.Execute(request);
        }

        public static int percentToHueBri(double a)
        {
            return (int)(HueMax * (a / userInputMax));
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
            Color lightColor = new Color();
            Dim lightBrightness = new Dim();

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
                        lightColor.on = true;
                        lightColor.hue = 65535;
                        lightColor.sat = 255;
                        thing = lightColor;
                        break;

                    case "green":
                        lightColor.on = true;
                        lightColor.hue = 21845;
                        lightColor.sat = 255;
                        thing = lightColor;
                        break;

                    case "purple":
                        lightColor.on = true;
                        lightColor.hue = 54000;
                        lightColor.sat = 255;
                        thing = lightColor;
                        break;

                    case "white":
                        lightColor.on = true;
                        lightColor.hue = 65535;
                        lightColor.sat = 0;
                        thing = lightColor;
                        break;

                    case "blue":
                        lightColor.on = true;
                        lightColor.hue = 43690;
                        lightColor.sat = 255;
                        thing = lightColor;
                        break;

                    case "dim":
                        lightBrightness.on = true;

                        if (arguments[arguments.Length - 1].ToLower() == "dim")
                        {
                            IRestResponse response;
                            if (whatToControl == "lights")
                            {
                                Console.WriteLine("This feature does not support the lights agrument yet.");
                                continue;
                            }
                            //response = MakeRequest(whatToControl, "", null, Method.GET);
                            response = MakeRequest(whatToControl, subControl, null, Method.GET);

                            //Console.WriteLine(response.Content);

                            Group responseObjectG = null;
                            //Lights responseObjectL = null;
                            if (whatToControl == "groups") responseObjectG = JsonConvert.DeserializeObject<Group>(response.Content);
                            //else responseObjectL = JsonConvert.DeserializeObject<Lights>(response.Content);

                            int brightness = 0;
                            if(responseObjectG != null) brightness = responseObjectG.Action.Bri;
                            //if (responseObjectL != null && arguments.Length == 3) Console.WriteLine(responseObjectL.lightID[Int16.Parse(arguments[arguments.Length - 2])]);
                            //else if (responseObjectL != null) Console.WriteLine(responseObjectL.lightID[0];

                            lightBrightness.bri = brightness - 40;
                            thing = lightBrightness;
                        }
                        else
                        {
                            try {
                                //dim to specified brightness
                                int brightness = Int32.Parse(arguments[arguments.Length - 1]);

                                if (brightness < 0 || brightness > 255) throw new Exception();
                                else if (brightness < 100) brightness = percentToHueBri(brightness);

                                lightBrightness.bri = brightness;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Incorrect Syntax, Please Try Again.\n" +
                                    "[Controller System] [ID] dim [% Brightness]\n");
                                continue;
                            }
                        }
                        thing = lightBrightness;
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
        }
    }
}
public class Def { }

public class AutoDim { }

public class PutState
{
    public bool on { get; set; }
}

public class Color
{
    public bool on { get; set; }
    public int hue { get; set; }
    public int sat { get; set; }
}

public class Dim
{
    public bool on { get; set; }
    public int bri { get; set; }
}

//Anything under this was autogenerated at app.quicktype.io

public partial class HueResponse
{
    [JsonProperty("lights")]
    public Lights Lights { get; set; }

    [JsonProperty("groups")]
    public List<Group> Groups { get; set; }
}

public partial class Group
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("lights")]
    public long[] Lights { get; set; }

    [JsonProperty("state")]
    public State State { get; set; }

    [JsonProperty("action")]
    public Action Action { get; set; }
}

//This class is used by groups and lights
public partial class Action
{
    [JsonProperty("on")]
    public bool On { get; set; }

    [JsonProperty("bri")]
    public int Bri { get; set; }

    [JsonProperty("hue")]
    public long Hue { get; set; }

    [JsonProperty("sat")]
    public long Sat { get; set; }

    [JsonProperty("reachable", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Reachable { get; set; }
}

//used by only groups
public partial class State
{
    [JsonProperty("all_on")]
    public bool AllOn { get; set; }

    [JsonProperty("any_on")]
    public bool AnyOn { get; set; }
}

public partial class Lights
{
    [JsonProperty("lightID")]
    public long[] lightID { get; set; }

    [JsonProperty("uniqueid")]
    public string Uniqueid { get; set; }

    [JsonProperty("swversion")]
    public string Swversion { get; set; }
}

public partial class Light
{
    [JsonProperty("state")]
    public Action State { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("modelid")]
    public string Modelid { get; set; }
}