using System.Collections.Generic;
using System.Linq;
using TestBot.ObjectsFromWit;

namespace TestBot.Models
{
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
                          .Select(x => new Intent { Confidence = x.Confidence, Value = x.Value })
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
}