using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

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
            return new JavaScriptSerializer().Serialize(data);
        }
    }

    public class MessageData
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public List<Intent> Intents { get; set; }
        public List<Entity> Entities { get; set; }

        public MessageData(WitObject wit)
        {
            Id = wit.Msg_id;
            Text = wit._text;
            Intents = wit.Entities?.Intent?
                .Select(x => new Intent {Confidence = x.Confidence, Value = x.Value})
                .ToList() ?? new List<Intent>();
            Entities = GetEntitiesFromWitEntities(wit.Entities);
        }

        private static List<Entity> GetEntitiesFromWitEntities(WitEntities witEntities)
        {
            if (witEntities == null)
                return new List<Entity>();

            var maybeGjenstandList = witEntities.Gjenstand?.Select(x => new Entity(x)) ?? new List<Entity>();
            var maybeOkonomiList = witEntities.Okonomi?.Select(x => new Entity(x)) ?? new List<Entity>();
            var maybeOrganisasjonList = witEntities.Organisasjon?.Select(x => new Entity(x)) ?? new List<Entity>();

            return new List<Entity>()
                .Concat(maybeGjenstandList)
                .Concat(maybeOkonomiList)
                .Concat(maybeOrganisasjonList)
                .ToList();
        }
    }

    public class Intent
    {
        public float Confidence { get; set; }
        public string Value { get; set; }
    }

    public class Entity
    {
        public string Name { get; set; }
        public float Confidence { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

        public Entity(WitCategoryBase wit)
        {
            Name = wit.Name;
            Confidence = wit.Confidence;
            Value = wit.Value;
            Type = wit.Type;
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