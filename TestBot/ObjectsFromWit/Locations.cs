using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TestBot.ObjectsFromWit
{

    public class Locations
    {
        public WitObject data = new WitObject();
        public Locations() { }
        public Locations(string json)
        {
            //
            //Adrian mente vi burde gå bort ifra å bruke regex slik vi bruker det nå.
            //Vi blir ekstremt tilknyttet/avhengig av nøyaktig denne JSON strukturen, 
            //og blir vanskelig dersom strukturen skulle endres, tjenesten byttes ut osv.
            //

            string pattern = @"[\""]?entities[\""]?(\s+)?(:)?(\s+)?{\n\s+\""\w+\""";
            string replacementPattern = @"[\""]?entities[\""]?(\s+)?(:)?(\s+)?{\n\s+\""Test\""";
            var inputString = json;

            Regex regex = new Regex(pattern);
            Match match = regex.Match(inputString);
            Console.WriteLine(match.Value);

            var final = inputString.Replace(pattern, replacementPattern);
            Console.WriteLine(final);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            this.data = (WitObject)serializer.Deserialize(inputString, typeof(WitObject));
        }
    }

    public class WitObject
    {
        public string msg_id { get; set; }
        public string _text { get; set; }
        public Entities entities { get; set; }
        public WitObject() { }
    }

    public class Entities
    {
        public Test[] test { get; set; }
        public Intent[] intent { get; set; }
        //public Okonomi[] okonomi { get; set; }
        //public Organisasjon[] organisasjon{ get; set; }
    }

    public class Test
    {
        public float confidence { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }
    public class Okonomi
    {
        public float confidence { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }
    public class Organisasjon
    {
        public float confidence { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }

    public class Intent
    {
        public float confidence { get; set; }
        public string value { get; set; }
    }
}