using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace TestBot.ObjectsFromWit
{

    public class WitObjectStructure
    {
        public WitObject data = new WitObject();
        public WitObjectStructure() { }
        public WitObjectStructure(string json)
        {
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
        public Organisasjon[] organisasjon { get; set; }
        public Okonomi[] okonomi { get; set; }
    }

    public class Test:CategoryBase
    {
        public override string Name => "Test";

        public override string GetResponse(Intent intent)
        {
            return "Test vellykket";
        }
    }

    public class Okonomi:CategoryBase
    {
        public override string Name => "Okonomi";
        public string[] Lonn = new[] { "lønn", "lønning", "lønnsutbetaling" , "lønna"};
        public string[] Feriepenger = new[] { "feriepenger", "feriepenga", "ferie-penger" };
        public string[] Mengde = new[] { "mye", "masse", "mange" };

        public override string GetResponse(Intent intent)
        {
            if (Lonn.Contains(value.ToLower()))
            {
                if (intent.value.Equals("Tidspunkt"))
                    return "Lønnen kommer den 20de hver måned";
                if (intent.value.Equals("Mengde"))
                    return "Mer enn nok";
            }
            else if (Feriepenger.Contains(value.ToLower())) {

            }
            
            return "Nå skjønte jeg ikke hva du mente. Kan du omformulere spørsmålet?";
        }
    }

    public class Organisasjon: CategoryBase
    {
        public override string Name => "Organisasjon";

        public override string GetResponse(Intent intent)
        {
            return "Velkommen til Creuna :)";
        }
    }

    public class Intent
    {
        public float confidence { get; set; }
        public string value { get; set; }
    }

    public abstract class CategoryBase
    {
        public abstract string GetResponse(Intent intent);
        public abstract string Name { get; }
        public float confidence { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }
}