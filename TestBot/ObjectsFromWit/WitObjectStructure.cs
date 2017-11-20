using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace TestBot.ObjectsFromWit
{
    public class WitObjectStructure
    {
        public WitObject Data;

        public WitObjectStructure(string json)
        {
            var inputString = json;
            var serializer = new JavaScriptSerializer();
            Data = (WitObject)serializer.Deserialize(inputString, typeof(WitObject));
        }

        public static string DataAsJson(WitObject data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }

    public class WitObject
    {
        public string Msg_id { get; set; }
        public string _text { get; set; }
        public WitEntities Entities { get; set; }
    }

    public class WitEntities
    {
        public WitIntent[] Intent { get; set; }
        public WitOrganisasjon[] Organisasjon { get; set; }
        public WitOkonomi[] Okonomi { get; set; }
        public WitGjenstand[] Gjenstand { get; set; }
    }

    public class WitGjenstand : WitCategoryBase
    {
        public override string Name => "Gjenstand";
    }

    public class WitOkonomi : WitCategoryBase
    {
        public override string Name => "Okonomi";
    }

    public class WitOrganisasjon : WitCategoryBase
    {
        public override string Name => "Organisasjon";
    }

    public class WitIntent
    {
        public float Confidence { get; set; }
        public string Value { get; set; }
    }

    public abstract class WitCategoryBase
    {
        public abstract string Name { get; }
        public float Confidence { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}